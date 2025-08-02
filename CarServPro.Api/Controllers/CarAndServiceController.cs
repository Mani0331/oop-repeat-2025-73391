using CarServPro.Domain;
using CarServPro.Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarServPro.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarAndServiceController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CarAndServiceController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Select(c => new CustomerDTO
                    {
                        CustomerId = c.CustomerId,
                        Name = c.Name,
                        Email = c.Email,
                        Password = c.Password
                    })
                    .ToListAsync();

                return Ok(customers);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok("Car Service API is running successfully");
        }

        [HttpGet("mechanics")]
        public async Task<IActionResult> GetAllMechanics()
        {
            try
            {
                var mechanics = await _context.Mechanics
                    .Select(m => new
                    {
                        m.MechanicId,
                        m.Name,
                        m.Email
                    })
                    .ToListAsync();

                return Ok(mechanics);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetAllCars()
        {
            try
            {
                var cars = await _context.Cars
                    .Include(c => c.Customer)
                    .Select(c => new
                    {
                        c.CarId,
                        c.RegistrationNumber,
                        CustomerName = c.Customer.Name,
                        CustomerEmail = c.Customer.Email
                    })
                    .ToListAsync();

                return Ok(cars);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}