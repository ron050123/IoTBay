using IoTBay.web.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IoTBay.web.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Only call the base method if no options have been configured yet
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=IotBayDatabase.db");
        }
    }
}