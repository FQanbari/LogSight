using System.ComponentModel.DataAnnotations.Schema;

namespace LogSight.API.Model;

[Table("Services")]
public class Server : BaseModel<Guid>
{
    public string Url { get; set; }
    public string Name { get; set; }
}
