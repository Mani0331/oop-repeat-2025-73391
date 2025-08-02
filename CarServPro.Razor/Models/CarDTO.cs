using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServPro.Razor.Models
{
    public class CarDTO
    {
        
        public int CarId { get; set; }

        
        public string RegistrationNumber { get; set; } = null!;

        public int CustomerId { get; set; }
        public CustomerDTO? Customer { get; set; } = null!;

        public ICollection<ServiceDTO>? Services { get; set; } = new List<ServiceDTO>();
    }
}
