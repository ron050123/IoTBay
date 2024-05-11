namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
public class Usr
{
    [Key]
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
<<<<<<< Updated upstream
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Type { get; set; }
    public bool IsActive { get; set; }
=======
    public string Status { get; set; } = "Active";
>>>>>>> Stashed changes
    
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}
