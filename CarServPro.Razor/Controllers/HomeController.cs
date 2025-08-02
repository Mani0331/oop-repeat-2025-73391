using CarServPro.Domain;
using CarServPro.Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Http;
using ErrorViewModel = CarServPro.Domain.ErrorViewModel;

namespace CarServPro.Razor.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext context;

        public HomeController(MyDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            var sessionId = HttpContext.Session.GetString("UserSession");
            if (!string.IsNullOrEmpty(sessionId))
            {
                if (sessionId == "admin")
                {
                    return RedirectToAction("Administrator");
                }
                
                if (int.TryParse(sessionId, out int userId))
                {
                    var mechanic = context.Mechanics.FirstOrDefault(m => m.MechanicId == userId);
                    if (mechanic != null)
                    {
                        return RedirectToAction("Mechanic");
                    }
                    
                    var customer = context.Customers.FirstOrDefault(c => c.CustomerId == userId);
                    if (customer != null)
                    {
                        return RedirectToAction("Customer");
                    }
                }
            }
            
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Customer user)
        {
            string AdminEmail = "admin@carservice.com";
            string Password = "Dorset001^";
            string MmechanicEmail1 = "mechanic1@carservice.com";
            string MmechanicEmail2 = "mechanic2@carservice.com";

            var Mycus = context.Customers.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (((user.Email == MmechanicEmail1) || (user.Email == MmechanicEmail2)) && (user.Password == Password))
            {
                var Mechanic = context.Mechanics.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
                HttpContext.Session.SetString("UserSession", $"{Mechanic.MechanicId}");

                return RedirectToAction("Mechanic");

            }
            if ((user.Email == AdminEmail) && (user.Password == Password))
            {
                HttpContext.Session.SetString("UserSession", "admin");
                return RedirectToAction("Administrator");
            }
            if (Mycus != null)
            {
                HttpContext.Session.SetString("UserSession", $"{Mycus.CustomerId}");
                if (Mycus.Email == user.Email)
                {
                    return RedirectToAction("Customer");

                }
            }
            else
            {
                ViewBag.Message = "Bad Credential Email or Password.";
            }
            return View();
        }

        public IActionResult Signup()
        {
            List<SelectListItem> Gender = new()
            {
                new SelectListItem {Value="Male",Text="Male"},
                new SelectListItem {Value="Female",Text="Female"}
            };
            ViewBag.Gender = Gender;
            return View();
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewData["MySession"] = HttpContext.Session.GetString("UserSession").ToString();
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public async Task<IActionResult> Mechanic()
        {
            string sessionId = HttpContext.Session.GetString("UserSession");
            int.TryParse(sessionId, out int mechanicId);

            var services = await context.Services
                .Include(s => s.Car)
                .Include(s => s.Mechanic)
                .Where(s => s.MechanicId == mechanicId)
                .Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceDate = s.ServiceDate,
                    MechanicName = s.Mechanic != null ? s.Mechanic.Name : string.Empty,
                    WorkDescription = s.WorkDescription,
                    Hours = s.Hours,
                    CarId = s.CarId,
                    MechanicId = s.MechanicId,
                    TotalCost = s.TotalCost,
                    RegistrationNumber = s.Car != null ? s.Car.RegistrationNumber : string.Empty,
                    Status = s.Status ?? "Pending",
                    CompletionDate = s.CompletionDate
                })
                .ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Customer()
        {
            string sessionId = HttpContext.Session.GetString("UserSession");
            if (string.IsNullOrEmpty(sessionId))
            {
                return RedirectToAction("Login", "Account");
            }
            int.TryParse(sessionId, out int customerId);

            var customerCars = await context.Cars
                .Where(car => car.CustomerId == customerId)
                .Include(car => car.Services)
                    .ThenInclude(service => service.Mechanic)
                .ToListAsync();

            var carDTOs = customerCars.Select(car => new CarDTO
            {
                CarId = car.CarId,
                RegistrationNumber = car.RegistrationNumber,
                Services = car.Services.Select(s => new ServiceDTO
                {
                    ServiceId = s.ServiceId,
                    ServiceDate = s.ServiceDate,
                    MechanicName = s.Mechanic != null ? s.Mechanic.Name : "N/A",
                    WorkDescription = s.WorkDescription,
                    Status = s.Status,
                    Hours = s.Hours,
                    CompletionDate = s.CompletionDate,
                    TotalCost = s.TotalCost
                }).ToList()
            }).ToList();

            return View("Customer", carDTOs);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteService(int id, string workDescription, decimal hours)
        {
            var service = context.Services
                .Include(s => s.Car)
                .FirstOrDefault(s => s.ServiceId == id);

            if (service == null)
            {
                return NotFound();
            }

            service.WorkDescription = workDescription;
            service.Hours = hours;
            service.Status = "Completed";
            service.CompletionDate = DateTime.UtcNow;

            service.CalculateCost();

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                throw;
            }
            return RedirectToAction("Mechanic");
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            bool emailExists = await context.Customers
                .AnyAsync(c => c.Email == customer.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered");
            }

            if (!ModelState.IsValid)
            {
                var allCustomers = await context.Customers
                    .Select(c => new CustomerDTO
                    {
                        CustomerId = c.CustomerId,
                        Name = c.Name,
                        Email = c.Email,
                        Password = c.Password
                    })
                    .ToListAsync();

                ViewBag.NewCustomer = customer;
                return View("Administrator", allCustomers);
            }


            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            TempData["Success"] = "Customer added successfully!";

            var updatedCustomers = await context.Customers
        .Select(c => new CustomerDTO
        {
            CustomerId = c.CustomerId,
            Name = c.Name,
            Email = c.Email,
            Password = c.Password
        })
        .ToListAsync();

            return View("Administrator", updatedCustomers);
        }

        [HttpGet]
        public async Task<IActionResult> Administrator()
        {
            var customers = await context.Customers
                .Select(c => new CustomerDTO
                {
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    Email = c.Email,
                    Password = c.Password,

                }).ToListAsync();

            return View(customers);
        }
        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            var cars = await context.Cars
                .Include(c => c.Customer)
                .Include(c => c.Services)
                .Where(c => c.CustomerId == id)
                .Select(c => new CarDTO
                {
                    CarId = c.CarId,

                    RegistrationNumber = c.RegistrationNumber,
                    CustomerId = c.CustomerId,
                    Services = c.Services.Select(s => new ServiceDTO
                    {
                        ServiceId = s.ServiceId

                    }).ToList()
                })
                .ToListAsync();

            ViewBag.CustomerId = id;

            var CusName = await context.Customers
                .Where(c => c.CustomerId == id)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();

            ViewBag.Name = CusName;

            ViewBag.Mechanics = await context.Mechanics
                .Select(m => new MechanicDTO
                {
                    MechanicId = m.MechanicId,
                    Name = m.Name
                })
                .ToListAsync();

            return View(cars);
        }




        [HttpPost]
        public IActionResult AddNewCar(string RegistrationNumber, int CustomerId)
        {
            var exists = context.Cars.Any(c => c.RegistrationNumber == RegistrationNumber && c.CustomerId == CustomerId);

            if (exists)
            {
                TempData["CarExists"] = "Car with this registration number already exists.";
                return RedirectToAction("CustomerDetails", new { id = CustomerId });
            }

            var newCar = new Car
            {
                RegistrationNumber = RegistrationNumber,
                CustomerId = CustomerId
            };

            context.Cars.Add(newCar);
            context.SaveChanges();

            return RedirectToAction("CustomerDetails", new { id = CustomerId });
        }
        [HttpPost]
        public IActionResult AddServiceToCar(Service service)
        {
            
            service.WorkDescription = "Service scheduled";
            service.Hours = 0;
            service.Status = "Pending";
            
            service.CalculateCost(); 
            service.ServiceDate = DateTime.SpecifyKind(service.ServiceDate, DateTimeKind.Utc);

            context.Services.Add(service);
            context.SaveChanges();

            var customerId = context.Cars
                .Where(c => c.CarId == service.CarId)
                .Select(c => c.CustomerId)
                .FirstOrDefault();

            return RedirectToAction("CustomerDetails", new { id = customerId });
        }
        [HttpGet]
        public IActionResult GetServicesForCar(int carId)
        {
            try
            {
                Console.WriteLine($"GetServicesForCar called with carId: {carId}");

                var totalServices = context.Services.Count();
                Console.WriteLine($"Total services in database: {totalServices}");

                var servicesForCar = context.Services.Where(s => s.CarId == carId).ToList();
                Console.WriteLine($"Services found for carId {carId}: {servicesForCar.Count}");

                var services = context.Services
                    .Where(s => s.CarId == carId)
                    .Include(s => s.Mechanic)
                    .Select(s => new
                    {
                        ServiceId = s.ServiceId,
                        ServiceDate = s.ServiceDate,
                        MechanicName = s.Mechanic != null ? s.Mechanic.Name : null,
                        WorkDescription = s.WorkDescription,
                        Hours = s.Hours
                    })
                    .ToList();

                Console.WriteLine($"Final services count: {services.Count}");
                return Json(services);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServicesForCar: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, "Error fetching services");
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var service = context.Services.FirstOrDefault(s => s.ServiceId == id);
            if (service != null)
            {
                context.Services.Remove(service);
                context.SaveChanges();
                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCar(int id)
        {
            var car = context.Cars.Include(c => c.Services)
                                   .FirstOrDefault(c => c.CarId == id);

            if (car == null)
            {
                return NotFound();
            }

            if (car.Services.Any())
            {
                context.Services.RemoveRange(car.Services);
            }

            context.Cars.Remove(car);
            context.SaveChanges();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = context.Customers.Find(id);
            if (customer != null)
            {
                context.Customers.Remove(customer);
                context.SaveChanges();
                TempData["Success"] = "Customer deleted successfully.";
            }
            var updatedCustomers = await context.Customers
         .Select(c => new CustomerDTO
         {
             CustomerId = c.CustomerId,
             Name = c.Name,
             Email = c.Email,
             Password = c.Password
         })
         .ToListAsync();

            return View("Administrator", updatedCustomers);
        }





        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
                return RedirectToAction("Login");
            }

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}