namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
public class Usr
{
    [Key]
    public int UserId { get; set; }
    //i added fullname and phone number since it's in the requirements
 
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Role { get; set; } = "Customer";
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; } 
    public string VerificationCode { get; set; }
    
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
    public void GenerateVerificationCode() {
        this.VerificationCode = new Random().Next(100000, 999999).ToString();
    }
    
    
}
