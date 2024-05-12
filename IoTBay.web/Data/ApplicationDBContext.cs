using IoTBay.web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.web.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Usr> Usrs { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payment { get; set; }
    
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<AccessLog> AccessLogs { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=IotBayDatabase.db");
        }
    }
}