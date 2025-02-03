using Service;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureServices(builder.Configuration);

WebApplication app = builder.Build();

app.MapOpenApi("/openapi.json").CacheOutput();

app.MapGet("/events", RouteHandlers.GetEventsAsync)
    .WithDescription("Gets events by topic")
    .WithTags("demo");

await app.RunAsync();
