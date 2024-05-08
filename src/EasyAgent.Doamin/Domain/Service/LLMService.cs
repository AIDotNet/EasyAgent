using AutoGen.Mistral;
using AutoGen.OpenAI;
using EasyAgent.Doamin.Common.DependencyInjection;
using EasyAgent.Domain.Domain.Interface;
using EasyAgent.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAgent.Domain.Domain.Service
{
    [ServiceDescription(typeof(ILLMService), ServiceLifetime.Scoped)]
    public class LLMService(IAIModels_Repositories _aIModels_Repositories) : ILLMService
    {
        public dynamic GetLLMConfig()
        { 
            var aimodel= _aIModels_Repositories.GetFirst(p=>true);
            if (aimodel.AIType == Model.Enum.AIType.OpenAI)
            {
                return new OpenAIConfig(aimodel.ModelKey, aimodel.ModelName);
            }
            else
            {
                return new AzureOpenAIConfig(aimodel.EndPoint, aimodel.ModelName, aimodel.ModelKey);
            }
        }
    }
}
