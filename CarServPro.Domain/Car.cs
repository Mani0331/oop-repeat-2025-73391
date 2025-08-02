using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarServPro.Domain;

public class Car
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CarId { get; set; }

    [Required]
    public string RegistrationNumber { get; set; } = null!;

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; } = null!;

    public ICollection<Service>? Services { get; set; } = new List<Service>();
}
