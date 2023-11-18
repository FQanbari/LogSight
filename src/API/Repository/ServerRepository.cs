using LogSight.API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class ServerRepository : IServerRepository
{
    private readonly ApplicationDbContext _context;

    public ServerRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Server>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Servers.ToListAsync(cancellationToken);
    }
}
