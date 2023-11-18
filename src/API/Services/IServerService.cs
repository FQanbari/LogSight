using LogSight.API.Model;

namespace API.Services
{
    public interface IServerService
    {
        Task<List<Server>> GetAll(CancellationToken cancellationToken);
    }
}