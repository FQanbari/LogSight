using LogSight.API.Model;

namespace API.Repository
{
    public interface IServerRepository
    {
        Task<List<Server>> GetAll(CancellationToken cancellationToken);
    }
}