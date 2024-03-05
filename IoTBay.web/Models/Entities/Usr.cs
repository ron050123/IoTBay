using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;

namespace IoTBay.web.Models.Entities;

public class Usr
{
    [Key]
    public Guid Id { get; set; }
    
    public string Username { get; set; }

    //for password requirements
    [Required]
    [StringLength(30, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Permission { get; set; } //For user permission

    
}