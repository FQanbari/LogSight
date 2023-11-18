using LogSight.API.Model;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class LogRepository : ILogRepository
{
    private readonly ApplicationDbContext _context;

    public LogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Log>> GetLogs(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Logs
         .OrderByDescending(x => x.CreateOn)
         .Skip((pageNumber - 1) * pageSize)
         .Take(pageSize)
         .AsQueryable();
        return await query.ToListAsync(cancellationToken);
    }
}
