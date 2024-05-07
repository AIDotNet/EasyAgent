using System.Threading.Tasks;
using AntDesign.Charts;
using EasyAgent.Services;
using Microsoft.AspNetCore.Components;

namespace EasyAgent.Pages.Dashboard.Analysis
{
    public partial class SalesCard
    {
        private readonly ColumnConfig _saleChartConfig = new ColumnConfig
        {
            AutoFit = true,
            Padding = "auto",
            XField = "x",
            YField = "y"
        };

        private readonly ColumnConfig _visitChartConfig = new ColumnConfig
        {
            AutoFit = true,
            Padding = "auto",
            XField = "x",
            YField = "y"
        };

        private IChartComponent _saleChart;
        private IChartComponent _visitChart;

        [Parameter]
        public SaleItem[] Items { get; set; } =
        {
            new SaleItem {Id = 1, Title = "Gongzhuan No.0 shop", Total = "323,234"},
            new SaleItem {Id = 2, Title = "Gongzhuan No.1 shop", Total = "323,234"},
            new SaleItem {Id = 3, Title = "Gongzhuan No.2 shop", Total = "323,234"},
            new SaleItem {Id = 4, Title = "Gongzhuan No.3 shop", Total = "323,234"},
            new SaleItem {Id = 5, Title = "Gongzhuan No.4 shop", Total = "323,234"},
            new SaleItem {Id = 6, Title = "Gongzhuan No.5 shop", Total = "323,234"},
            new SaleItem {Id = 7, Title = "Gongzhuan No.6 shop", Total = "323,234"}
        };

        [Inject] public IChartService ChartService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await OnTabChanged("1");
            }
        }

        private async Task OnTabChanged(string activeKey)
        {
            var data = await ChartService.GetSalesDataAsync();
            if (activeKey == "1")
                await _saleChart.ChangeData(data);
            else
                await _visitChart.ChangeData(data);
        }
    }
}