using AutoGen;
using AutoGen.Core;
using AutoGen.Mistral;
using AutoGen.OpenAI;
using EasyAgent.Domain.Domain.Interface;
using EasyAgent.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using NPOI.POIFS.Properties;

namespace EasyAgent.Pages.Chat
{
    public partial class Chat
    {

        [Inject] IAgents_Repositories _agents_Repositories { get; set; }
        [Inject] ILLMService _iLLMService { get; set; }

        protected List<Agents> _list = new List<Agents>();

        IEnumerable<string> _agentIds = [];

        protected string? _messageInput;
        protected string _json = "";
        protected bool Sendding = false;
        protected List<ChatMessage> MessageList = [];
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _list = _agents_Repositories.GetList();
        }

        protected async Task OnSendAsync()
        {
            var llm = _iLLMService.GetLLMConfig();

            List<MiddlewareAgent> agentList = new List<MiddlewareAgent>();
            foreach (var agentid in _agentIds)
            { 
                var agent = _list.FirstOrDefault(x => x.Id == agentid);
                var assistantAgent = new AssistantAgent(
                     name: agent.Name,
                     systemMessage: agent.Prompt,
                     llmConfig: llm).RegisterPrintMessage();
                agentList.Add(assistantAgent);
            }

            var groupChat = new RoundRobinGroupChat(agents: agentList);

            var groupChatAgent = new GroupChatManager(groupChat);

            var userProxyAgent = new UserProxyAgent(
                  name: "user",
                  humanInputMode: HumanInputMode.AUTO).RegisterPrintMessage(); ;

            var conversationHistory = await userProxyAgent.InitiateChatAsync(
                  receiver: groupChatAgent,
                  message: _messageInput,
                  maxRound: 30);

        }
    }
}
