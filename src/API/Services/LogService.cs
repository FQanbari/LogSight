using API.Repository;
using LogSight.API.Model;

namespace API.Services;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;

    public LogService(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<List<Log>> GetLogs(int pageNumber, int pageSize, CancellationToken cancellationToken) 
        => await _logRepository.GetLogs(pageNumber, pageSize, cancellationToken);
}