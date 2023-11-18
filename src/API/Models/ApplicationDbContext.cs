using API.Models;
using Microsoft.EntityFrameworkCore;

namespace LogSight.API.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Server> Servers { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<User> User { get; set; }
}
