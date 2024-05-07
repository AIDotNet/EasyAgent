
using SqlSugar;
using System.ComponentModel.DataAnnotations;

namespace EasyAgent.Domain.Repositories
{
    [SugarTable("Agents")]
    public partial class Agents
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        /// <summary>
        /// 头像
        /// </summary>

        [Required(ErrorMessage = "请输上传头像")]
        public string Icon { get; set; } = "";
        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "请输入Agent名称")]
        public string Name { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [Required(ErrorMessage = "请输入智能体提示词描述")]
        [SugarColumn(ColumnDataType = "varchar(2000)")]
        public string Prompt { get; set; }
    }
}