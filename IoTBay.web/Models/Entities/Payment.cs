namespace IoTBay.web.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }

    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual Usr User { get; set; }

    public int OrderId { get; set; }
    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }
}
