using System.ComponentModel.DataAnnotations;

namespace IoTBay.web.Models.Entities;

public class AccessLog
{
    [Key]
    public int LogId { get; set; }
    public int UserId { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }  

    public virtual Usr User { get; set; }
}