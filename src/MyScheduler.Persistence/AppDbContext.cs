using Microsoft.EntityFrameworkCore;

namespace MyScheduler.Persistence;

public class AppDbContext : DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
