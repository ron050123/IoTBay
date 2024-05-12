namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual Usr User { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
}
