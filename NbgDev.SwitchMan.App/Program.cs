using NbgDev.SwitchMan.App.Components;
using NbgDev.SwitchMan.App.Services;
using NbgDev.SwitchMan.Switches.TLSG108PE;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 8080
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register Configuration service as singleton
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

// Register VLAN service as singleton
builder.Services.AddSingleton<VlanService>();

// Register Switch service as singleton
builder.Services.AddSingleton<SwitchService>();

// Register switch access service
builder.Services.AddTlSg108PeSwitchAccess();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
