﻿using System.ComponentModel.DataAnnotations;

namespace IoTBay.web.Models;

public class UpdatePorfileModel
{
    [Required]
    [Display(Name = "Full Name")]
    public string Name { get; set; }

    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
    
}