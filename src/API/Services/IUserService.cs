using API.Models;
using API.ViewModel;

namespace API.Services;

public interface IUserService
{
    Task<UserViewModel> Get(UserLogin user, CancellationToken cancellationToken);
}
