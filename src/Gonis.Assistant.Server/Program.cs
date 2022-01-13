using Gonis.Assistant.Core.Middlewares;
using Gonis.Assistant.Server.Extensions;
using Gonis.Assistant.Server.HealthCheckers;
using Gonis.Assistant.Telegram;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logger
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddLogger(builder.Configuration);
builder.Services.AddSettings(builder.Configuration);
builder.Services.AddTelegramBot();
builder.Services.AddHttpClientForWebApi(builder.Configuration);
builder.Services.AddCustomHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gonis Assistant v1"));

app.UseMiddleware<HttpStatusCodeExceptionMiddleware>();

// app.UseMiddleware<OptionsVerbMiddleware>();
app.UseMiddleware<LoggerWebApiRequestMiddleware>();

app.UseStaticFiles();

app.UseRouting();
app.UseCustomHealthCheckEndpoints();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});
app.Run();