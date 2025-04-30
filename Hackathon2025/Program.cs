using Hackathon2025.Abstractions;
using Hackathon2025.Components;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using MudBlazor;
using MudBlazor.Services;
//using Microsoft.SemanticKernel;
//using OpenAI.VectorStores;

var builder = WebApplication.CreateBuilder(args);

//Add JavaScriptEngineSwitcher services to the services container.(needed for WebOptimizer.sass)
builder.Services.AddJsEngineSwitcher(options => options.DefaultEngineName = V8JsEngine.EngineName).AddV8();
//Configure WebOptimizer
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddScssBundle("/css/main.min.css", "/sass/main.scss");
});

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();


builder.Services.AddHttpClient();
builder.Services.AddSingleton<OnboardingAgentService>();
builder.Services.AddSingleton<VectorStoreService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

//Add WebOptimizer
app.UseWebOptimizer();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();
