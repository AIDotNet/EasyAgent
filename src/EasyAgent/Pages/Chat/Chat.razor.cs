using AntDesign;
using AntDesign.Charts;
using AutoGen;
using AutoGen.Core;
using AutoGen.Mistral;
using AutoGen.OpenAI;
using EasyAgent.Domain.Domain.Interface;
using EasyAgent.Domain.Repositories;
using Markdig;
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
                Sendding = true;
                MessageList.Add(new ChatMessage(ChatMessage.RoleEnum.User, _messageInput));
                var msg= _messageInput;
                _messageInput = "";
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
                                    MessageList.Add(new ChatMessage(ChatMessage.RoleEnum.Assistant, Markdown.ToHtml($"{name}：{msg}")));
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
                           Sendding = false;
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




                    //var groupChat = new RoundRobinGroupChat(
                    //    agents: agentList.ToArray());

                    var admin = new GPTAgent(
                     name: "admin",
                     systemMessage: "您是群管理员，一旦用户任务完成，请说[TERMINATE]加上最终答案，终止群聊",
                     temperature: 0,
                     config: llm);
                     //.RegisterMiddleware(async (messages, option, agent, _) =>
                     //{
                     //    var reply = await agent.GenerateReplyAsync(messages, option);
                     //    if (reply is TextMessage textMessage && textMessage.Content.Contains("TERMINATE") is true)
                     //    {
                     //        var content = $"{textMessage.Content}\n\n {GroupChatExtension.TERMINATE}";

                     //        return new TextMessage(Role.Assistant, content, from: reply.From);
                     //    }
                     //    return reply;
                     //});

                    agentList.Add(userProxyAgent);

                    agentList.Add(admin);

                    var groupChat = new GroupChat(
                        admin: admin,
                        members: agentList.ToArray()
                    );
                    userProxyAgent.SendIntroduction("我是上市公司老总，湖北首富", groupChat);

                    groupChatManager = new GroupChatManager(groupChat);

                    var conversationHistory = await userProxyAgent.InitiateChatAsync(
                          receiver: groupChatManager,
                          message: msg,
                          maxRound: 30);

                }
                else
                {
                    _messageInputNew = msg;
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }

        }
    }
}
