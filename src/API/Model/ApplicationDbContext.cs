using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogSight.API.Model;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Service> Services { get; set; }
    public DbSet<Log> Logs { get; set; }
}

public class Service : BaseModel<Guid>
{
    public string Url { get; set; }
    public string Name { get; set; }
}
public class BaseModel<T>
{
    public T Id { get; set; }
}

[Table("AppLogs", Schema = "Log")]
public class Log : BaseModel<int>
{
    public string UserName { get; set; }
    public string Level { get; set; }
    public string RequestUrl { get; set; }
    public string RequestType { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string ErrorClass { get; set; }
    public int ErrorLine { get; set; }
    public string Path { get; set; }
    public DateTime CreateOn { get; set; }
    public string PersonCode { get; set; }
    public string IP { get; set; }
    public string UserAgent { get; set; }
    public string Body { get; set; }
    public string UrlReferer { get; set; }

}