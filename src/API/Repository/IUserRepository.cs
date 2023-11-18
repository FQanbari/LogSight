using API.Models;
using System.Threading;

namespace API.Repository;

public interface IUserRepository
{
    Task<User> Get(string userName, string password, CancellationToken cancellationToken);
}