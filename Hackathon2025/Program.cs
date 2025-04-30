using Hackathon2025.Abstractions;
using Hackathon2025.Components;
using Hackathon2025.Services;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
using MudBlazor;
using MudBlazor.Services;

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<InterestsAgentService>();
builder.Services.AddSingleton<VectorStoreService>();


//Configure mudservices to use elements
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

var configuration = builder.Configuration;
ConfigurationHelper.Initialize(configuration);

app.UseExceptionHandler("/Error");

//Handles 404 primarily [when directly hit]
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
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
