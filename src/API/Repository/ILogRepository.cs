using LogSight.API.Model;

namespace API.Repository
{
    public interface ILogRepository
    {
        Task<List<Log>> GetLogs(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}