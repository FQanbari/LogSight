using LogSight.API.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Users")]
public class User :BaseModel<int>
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}
