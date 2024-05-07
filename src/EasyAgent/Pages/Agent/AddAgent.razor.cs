using AntDesign;
using EasyAgent.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using NPOI.SS.Formula.Functions;

namespace EasyAgent.Pages.Agent
{
    public partial class AddAgent
    {
        [Parameter]
        public string AgentId { get; set; }

        [Inject] IAgents_Repositories _agents_Repositories { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected MessageService? Message { get; set; }

        private Agents _agentModel = new Agents();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!string.IsNullOrEmpty(AgentId))
            {
                _agentModel = _agents_Repositories.GetFirst(p=>p.Id==AgentId);
            }
        }
        private void HandleSubmit()
        {
            if (string.IsNullOrEmpty(AgentId))
            {
                _agentModel.Id = Guid.NewGuid().ToString();
                if (_agents_Repositories.IsAny(p => p.Name == _agentModel.Name))
                {
                    _ = Message.Error("名称已存在！", 2);
                    return;
                }
                _agents_Repositories.Insert(_agentModel);
            }
            else
            {
                _agents_Repositories.Update(_agentModel);
            }

            NavigationManager.NavigateTo("/agentlist");
        }
    }
}
