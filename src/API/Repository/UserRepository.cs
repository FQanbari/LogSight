using API.Models;
using LogSight.API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> Get(string userName, string password, CancellationToken cancellationToken)
    {
       return await _context.User.Where(x => x.UserName.ToLower() == userName.ToLower() && x.Password.ToLower() == password).FirstOrDefaultAsync(cancellationToken);
    }
}
