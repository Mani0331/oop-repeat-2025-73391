using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServPro.Razor.Models;

public class MechanicDTO
{
    public int MechanicId { get; set; }


    public string Name { get; set; } 

    
    public string Password { get; set; } = null!;
    
    public string Email { get; set; } = null!;

    public ICollection<ServiceDTO> Services { get; set; } = new List<ServiceDTO>();
}
