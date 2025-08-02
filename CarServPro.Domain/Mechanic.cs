using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServPro.Domain;

public class Mechanic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MechanicId { get; set; }

    [Required]
    public string Name { get; set; } = null!;   

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    public ICollection<Service> Services { get; set; } = new List<Service>();
}
