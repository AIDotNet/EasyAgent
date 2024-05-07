using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyAgent.Domain.Domain.Model.Enum
{
    public enum AIType
    {
        [Display(Name = "Open AI")]
        OpenAI = 1,

        [Display(Name = "Azure Open AI")]
        AzureOpenAI = 2,
    }
}
