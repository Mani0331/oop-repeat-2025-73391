
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CarServPro.Domain;
using Xunit;

namespace CarServPro.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetCars_ReturnsCarList()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase("CarListDb")
                .Options;

            using var context = new MyDbContext(options);
            context.Cars.Add(new Car { CarId = 1, RegistrationNumber = "ABC123", CustomerId = 1 });
            context.Cars.Add(new Car { CarId = 2, RegistrationNumber = "XYZ789", CustomerId = 2 });
            await context.SaveChangesAsync();

            var cars = await context.Cars.ToListAsync();

            Assert.NotNull(cars);
            Assert.Equal(2, cars.Count);
            Assert.Contains(cars, c => c.RegistrationNumber == "ABC123");
        }

        [Theory]
        [InlineData(1.0, 75)]
        [InlineData(1.5, 150)]
        [InlineData(2.0, 150)]
        [InlineData(2.5, 225)]
        [InlineData(3.0, 225)]
        [InlineData(0.1, 75)]
        [InlineData(0.0, 0)]
        public void CalculateCost(decimal hours, decimal expectedCost)
        {
            var service = new Service
            {
                Hours = hours
            };

            service.CalculateCost();

            Assert.Equal(expectedCost, service.TotalCost);
        }
    }
}
