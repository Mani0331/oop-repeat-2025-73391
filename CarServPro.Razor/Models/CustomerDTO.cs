using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServPro.Razor.Models;

public class CustomerDTO
{
    public int CustomerId { get; set; }

    
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

  
    public string Password { get; set; } = null!;
    public List<int>? CarIds = new List<int>();
    public ICollection<CarDTO>? Cars { get; set; } = new List<CarDTO>();
}