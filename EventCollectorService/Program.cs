using EventCollectorService.Services;
using EventCollectorService.Background;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddSingleton<IEventsCollectorService, EventsCollectorService>();
builder.Services.AddSingleton<IQueueService, MsmqQueueService>();
builder.Services.AddHostedService<EventBackgroundWorker>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventCollectorService API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventCollectorService API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
