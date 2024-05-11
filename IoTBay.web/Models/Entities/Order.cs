using System;
using System.ComponentModel.DataAnnotations;

namespace IoTBay.web.Models.Entities;

public class Order {
        
        public int OrderId { get; set; }

        public int UserId { get; set; } // Changed to int to match Usr.Id

        public Guid ProductId { get; set; } // Changed to Guid to match Product.Id

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public string DeliveryAddress { get; set; }

        public DateTime OrderDate { get; set; }

        // Navigation property for the product associated with the order
        public Product Product { get; set; }
}