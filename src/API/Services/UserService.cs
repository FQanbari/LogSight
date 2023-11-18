using API.Models;
using API.Repository;
using API.ViewModel;

namespace API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserViewModel> Get(UserLogin model, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(model.UserName, model.Password, cancellationToken);
        if (user == null)
            throw new Exception("User not found!");

        
        return new UserViewModel { Id = user.Id, UserName = user.UserName, FullName = user.FullName, Password = user.Password, Role = "Admin"};
    }
}
