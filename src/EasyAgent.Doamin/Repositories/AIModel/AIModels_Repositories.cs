

using EasyAgent.Doamin.Common.DependencyInjection;
using EasyAgent.Doamin.Repositories.Base;

namespace EasyAgent.Domain.Repositories
{
    [ServiceDescription(typeof(IAIModels_Repositories), ServiceLifetime.Scoped)]
    public class AIModels_Repositories : Repository<AIModels>, IAIModels_Repositories
    {
    }
}
