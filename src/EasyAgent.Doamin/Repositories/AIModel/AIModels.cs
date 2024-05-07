using EasyAgent.Domain.Domain.Model.Enum;
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace EasyAgent.Domain.Repositories
{
    [SugarTable("AIModels")]
    public partial class AIModels
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// AI类型
        /// </summary>


        public AIType AIType { get; set; } = AIType.OpenAI;
      

        public string EndPoint { get; set; } = "";
        /// <summary>
        /// 模型名称
        /// </summary>
        [Required(ErrorMessage = "请输入部署名/模型名")]
        public string ModelName { get; set; } = "";
        /// <summary>
        /// 模型秘钥
        /// </summary>
        [Required(ErrorMessage = "请输入模型秘钥")]
        public string ModelKey { get; set; } = "";
    }
}
