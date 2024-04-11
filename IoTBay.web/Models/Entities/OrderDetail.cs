﻿namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public int OrderId { get; set; }
    public int ProductId { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }
    
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
}
