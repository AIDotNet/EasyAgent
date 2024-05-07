
using EasyAgent.Doamin.Common.DependencyInjection;
using EasyAgent.Doamin.Repositories.Base;

namespace EasyAgent.Domain.Repositories
{
    [ServiceDescription(typeof(IAgents_Repositories), ServiceLifetime.Scoped)]
    public class Agents_Repositories : Repository<Agents>, IAgents_Repositories
    {
    }
}
