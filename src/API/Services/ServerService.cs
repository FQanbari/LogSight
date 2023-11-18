using API.Repository;
using LogSight.API.Model;

namespace API.Services;

public class ServerService : IServerService
{
    private readonly IServerRepository _serverRepository;

    public ServerService(IServerRepository serverRepository)
    {
        _serverRepository = serverRepository;
    }
    public async Task<List<Server>> GetAll(CancellationToken cancellationToken)
    {
        return await _serverRepository.GetAll(cancellationToken);
    }
}