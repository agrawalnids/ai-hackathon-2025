using Hackathon2025.Abstractions;
using Hackathon2025.Components;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.V8;
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

builder.Services.AddMudServices();


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
