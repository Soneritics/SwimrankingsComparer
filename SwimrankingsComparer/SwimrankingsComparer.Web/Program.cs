using SwimRankings.Api;
using SwimrankingsComparer.Application.Repositories;
using SwimrankingsComparer.Application.Services;
using SwimrankingsComparer.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ISwimmerApi, SwimmerApi>();
builder.Services.AddSingleton<SwimmerService>();
builder.Services.AddSingleton<IRepository>(r => new CosmosRepository(
    builder.Configuration["CosmosConnectionString"]!,
    builder.Configuration["CosmosDatabaseName"]!));

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(7);
});

Constants.AppName = builder.Configuration["AppName"] ?? Constants.AppName;

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();