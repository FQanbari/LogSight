using LogSight.API.Model;

namespace API.Services;

public interface ILogService
{
    Task<List<Log>> GetLogs(int pageNumber, int pageSize, CancellationToken cancellationToken);
}
