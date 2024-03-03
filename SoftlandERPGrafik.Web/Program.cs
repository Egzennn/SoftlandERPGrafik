using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using SoftlandERGrafik.Data.Entities.Forms;
using SoftlandERPGrafik.Core.Repositories;
using SoftlandERPGrafik.Core.Repositories.Interfaces;
using SoftlandERPGrafik.Data.Configurations;
using SoftlandERPGrafik.Data.DB;
using SoftlandERPGrafik.Data.Entities.Staff.AD;
using SoftlandERPGrafik.Data.Entities.Views;
using SoftlandERPGrafik.Data.Entities.Vocabularies.Forms.Ogolne;
using SoftlandERPGrafik.Web.Components;
using SoftlandERPGrafik.Web.Components.Adaptor;
using SoftlandERPGrafik.Web.Components.Services;
using Syncfusion.Blazor;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pl-PL");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pl-PL");
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme).AddNegotiate();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddScoped<UserDetailsService>();
builder.Services.AddScoped<GrafikService>();
builder.Services.AddScoped<GrafikAdaptor>();

builder.Services.AddTransient<IRepository<OgolneStan>, Repository<OgolneStan>>();
builder.Services.AddTransient<IRepository<GrafikForm>, Repository<GrafikForm>>();
builder.Services.AddTransient<IADRepository, ADRepository>();

builder.Services.AddMudServices();

builder.Services.AddTransient<IRepository<OrganizacjaLokalizacje>, Repository<OrganizacjaLokalizacje>>();
builder.Services.AddTransient<IRepository<ZatrudnieniDzialy>, Repository<ZatrudnieniDzialy>>();
builder.Services.AddTransient<IRepository<ZatrudnieniZrodlo>, Repository<ZatrudnieniZrodlo>>();
builder.Services.AddTransient<IRepository<Kierownicy>, Repository<Kierownicy>>();

builder.Services.AddDbContext<MainContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MainConnection")));
builder.Services.AddDbContext<OptimaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OptimaConnection")));

builder.Services.Configure<ADConfiguration>(builder.Configuration.GetRequiredSection("ADConfiguration"));

builder.Services.AddScoped<ADConfiguration>();
builder.Services.AddScoped<ADUser>();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA0NDM4M0AzMjM0MmUzMDJlMzBUS0ZyTGNkcDZTdnFWeHpIbHBrTHpQSnc2MTFVZmxHKzRRM2xGRzRReW5VPQ==");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
