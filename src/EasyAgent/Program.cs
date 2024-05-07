using Microsoft.AspNetCore.Components;
using AntDesign.ProLayout;
using EasyAgent.Doamin.Common.Map;
using EasyAgent.Doamin.Options;
using EasyAgent.Doamin.Common.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddAntDesign();
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(sp.GetService<NavigationManager>()!.BaseUri)
});
builder.Services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));

builder.Services.AddServicesFromAssemblies("EasyAgent");
builder.Services.AddServicesFromAssemblies("EasyAgent.Domain");
builder.Services.AddMapper();
builder.Configuration.GetSection("DBConnection").Get<DBConnectionOption>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

//扩展初始化实现
app.CodeFirst();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();