namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
public class Usr
{
    [Key]
    public int UserId { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
}
