using System.Collections.Generic;
using EasyAgent.Models;
using Microsoft.AspNetCore.Components;

namespace EasyAgent.Pages.Account.Center
{
    public partial class Articles
    {
        [Parameter] public IList<ListItemDataType> List { get; set; }
    }
}