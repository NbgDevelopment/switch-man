using Microsoft.Extensions.Options;
using NbgDev.SwitchMan.App.Components;
using NbgDev.SwitchMan.App.Services;
using NbgDev.SwitchMan.Switches.OmadaController;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 8080
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Data Protection for encrypting sensitive configuration values
builder.Services.AddDataProtection();

// Register Configuration service as singleton
builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

// Register VLAN service as singleton
builder.Services.AddSingleton<VlanService>();

// Register Switch service as singleton
builder.Services.AddSingleton<SwitchService>();

// Register switch access service
builder.Services.AddOmadaControllerSwitchAccess();

// Configure OmadaControllerOptions from the settings file
builder.Services.AddSingleton<IConfigureOptions<OmadaControllerOptions>>(sp =>
{
    var configService = sp.GetRequiredService<IConfigurationService>();
    var omadaSettings = configService.LoadOmadaSettings();
    return new ConfigureOptions<OmadaControllerOptions>(opts =>
    {
        if (omadaSettings is not null)
        {
            opts.ControllerUrl = omadaSettings.ControllerUrl;
            opts.OmadaId = omadaSettings.OmadaId;
            opts.ClientId = omadaSettings.ClientId;
            opts.ClientSecret = omadaSettings.ClientSecret;
            opts.AllowInvalidCertificate = omadaSettings.AllowInvalidCertificate;
        }
    });
});

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
