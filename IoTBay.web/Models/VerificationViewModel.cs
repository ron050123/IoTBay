namespace IoTBay.web.Models;
using System.ComponentModel.DataAnnotations;

public class VerificationViewModel
{
    //I created this to make the verification better since if i use the usr entities it will requires all the other entities wehre in verificaiton only code and email needed
    [Required]
    public string? Email { get; set; }

    [Required]
    [Display(Name = "Verification Code")]
    public string? VerificationCode { get; set; }
}