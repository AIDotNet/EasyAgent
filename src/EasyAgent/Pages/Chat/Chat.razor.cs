using AntDesign;
using AutoGen;
using AutoGen.Core;
using AutoGen.Mistral;
using AutoGen.OpenAI;
using EasyAgent.Domain.Domain.Interface;
using EasyAgent.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using System.Text;
using System.Text.RegularExpressions;
using Match = System.Text.RegularExpressions.Match;

namespace EasyAgent.Pages.Chat
{
    public partial class Chat
    {

        [Inject] IAgents_Repositories _agents_Repositories { get; set; }
        [Inject] ILLMService _iLLMService { get; set; }

        protected List<Agents> _list = new List<Agents>();

        IEnumerable<string> _agentIds = [];

        private bool agentStart = false;

        protected string? _messageInput;

        protected string? _messageInputNew;
        protected string _json = "";
        protected bool Sendding = false;
        protected List<ChatMessage> MessageList = [];

        private MiddlewareAgent< UserProxyAgent> userProxyAgent { get; set; }
        private GroupChatManager groupChatManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _list = _agents_Repositories.GetList();
        }

        protected async Task OnSendAsync()
        {
            try
            {
                MessageList.Add(new ChatMessage(ChatMessage.RoleEnum.User, _messageInput));
                await InvokeAsync(StateHasChanged);

                if (!agentStart)
                {
                    agentStart = true;
                    var llm = _iLLMService.GetLLMConfig();

                    List<IAgent> agentList = new List<IAgent>();
                    foreach (var agentid in _agentIds)
                    {
                        var agent = _list.FirstOrDefault(x => x.Id == agentid);
                        var assistantAgent = new AssistantAgent(
                            name: agent.Name,
                            systemMessage: agent.Prompt,
                            llmConfig: new ConversableAgentConfig
                            {
                                Temperature = 0,
                                ConfigList = [llm],
                            }).RegisterMiddleware(async (messages, options, agent, ct) =>
                            {
                                var reply = await agent.GenerateReplyAsync(messages, options, ct);
                                var formattedMessage = reply.FormatMessage();
                                string pattern = @"TextMessage from (\S+)[\s\S]*?-{20}\s*(?:\r?\n)?([\s\S]*?)(?:\r?\n)?-{20}";

                                Match match = Regex.Match(formattedMessage, pattern);

                                if (match.Success)
                                {
                                    string name = match.Groups[1].Value; // 提取括号内的分组
                                    string msg = match.Groups[2].Value;
                                    MessageList.Add(new ChatMessage(ChatMessage.RoleEnum.Assistant, $"{name}：{msg}"));
                                    await InvokeAsync(StateHasChanged);
                                }                            
                                return reply;
                            });
                        agentList.Add(assistantAgent);
                    }

                    userProxyAgent = new UserProxyAgent(
                          name: "user",
                          humanInputMode: HumanInputMode.AUTO)
                       .RegisterMiddleware(async (messages, options, agent, ct) =>
                       {

                           MessageList.Add(new ChatMessage(ChatMessage.RoleEnum.System, "该你输入了"));
                           await InvokeAsync(StateHasChanged);

                           while (true)
                           {
                               await Task.Delay(1000);
                               if (!string.IsNullOrEmpty(_messageInputNew))
                               {
                                   var temp = _messageInputNew;
                                   _messageInputNew = "";
                                   return new TextMessage(Role.User, temp);
                               }
                      
                           }
                       });

                    agentList.Add(userProxyAgent);


                    var groupChat = new RoundRobinGroupChat(
                        agents: agentList.ToArray());

                    groupChatManager = new GroupChatManager(groupChat);

                    var conversationHistory = await userProxyAgent.InitiateChatAsync(
                          receiver: groupChatManager,
                          message: _messageInput,
                          maxRound: 30);
                    _messageInput = "";
                }
                else
                {
                    _messageInputNew = _messageInput;
                }
            }
            catch (Exception ex)
            { 
            
            }

        }
    }
}
