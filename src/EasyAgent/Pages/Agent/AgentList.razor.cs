using AntDesign;
using EasyAgent.Domain.Repositories;
using Microsoft.AspNetCore.Components;

namespace EasyAgent.Pages.Agent
{
    public partial class AgentList
    {
        [Inject] IAgents_Repositories _agents_Repositories { get; set; }
        [Inject] IConfirmService _confirmService { get; set; }

        private Agents[] _data = { };
        private readonly ListGridType _listGridType = new ListGridType
        {
            Gutter = 16,
            Xs = 1,
            Sm = 2,
            Md = 3,
            Lg = 3,
            Xl = 4,
            Xxl = 4
        };
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitData("");
        }

        private async Task InitData(string searchKey)
        {
            var list = new List<Agents> { new Agents() };
            List<Agents> data;
            if (string.IsNullOrEmpty(searchKey))
            {
                data = await _agents_Repositories.GetListAsync();
            }
            else
            {
                data = await _agents_Repositories.GetListAsync(p => p.Name.Contains(searchKey));
            }

            list.AddRange(data);
            _data = list.ToArray();
            await InvokeAsync(StateHasChanged);
        }

        private async Task Search(string searchKey)
        {
            await InitData(searchKey);
        }

        private void NavigateToAddAgent()
        {
            NavigationManager.NavigateTo("/agent/add");
        }


        private async Task Update(string id)
        {
            NavigationManager.NavigateTo($"/agent/add/{id}");
        }
        private async Task Delete(string id)
        {

            var content = "是否删除Agent？";
            var title = "删除";
            var result = await _confirmService.Show(content, title, ConfirmButtons.YesNo);
            if (result == ConfirmResult.Yes)
            {
              
                await _agents_Repositories.DeleteAsync(id);
                await InitData("");
            }
        }
    }
}
