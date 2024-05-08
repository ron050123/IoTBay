namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(25, ErrorMessage = "Name cannot be longer than 25 characters.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Description is required.")]
    [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
    [MinLength(2, ErrorMessage = "Description must be at least 2 characters long.")]
    public string Description { get; set; }

    [Range(10, 500, ErrorMessage = "Price must be between 100 and 500.")]
    public decimal Price { get; set; }
    
    
}