using EasyAgent.Domain.Repositories;
using Microsoft.AspNetCore.Components;
using EasyAgent.Utils;
using EasyAgent.Domain.Utils;
using AntDesign;
using EasyAgent.Domain.Domain.Model.Enum;

namespace EasyAgent.Pages.Model
{
    public partial class ModelSetting
    {
        [Inject] IAIModels_Repositories _aIModels_Repositories { get; set; }

        [Inject] protected MessageService? Message { get; set; }
        private AIModels _aiModel { get; set; } = new AIModels();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var model=_aIModels_Repositories.GetFirst(p=>true);
            if (model.IsNotNull())
            {
                _aiModel = model;
            }
        }
        private void HandleSubmit()
        {
            var model=_aIModels_Repositories.GetFirst(p=>true);
            if (model.IsNull())
            {
                if (_aiModel.AIType == AIType.AzureOpenAI && string.IsNullOrEmpty(_aiModel.EndPoint))
                {
                    _ = Message.Error("Azure OpenAI 必须填写EndPoint", 2);
                }
                //新增
                _aiModel.Id = "001";
                _aIModels_Repositories.Insert(_aiModel);
                _ = Message.Info("保存成功", 2);
            }
            else 
            {
                model.ModelKey = _aiModel.ModelKey;
                model.ModelName = _aiModel.ModelName;
                model.EndPoint = _aiModel.EndPoint;
                _aIModels_Repositories.Update(_aiModel);
                _ = Message.Info("保存成功", 2);
            }
        }
    }
}
