namespace Service;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

using NATS.Client.Core;
using NATS.Net;

internal static class ProgramConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(ConfigureOpenApiOptions);
        services.AddHttpContextAccessor();
        services.AddScoped(NatsClientFactory(configuration));
    }

    private static void ConfigureOpenApiOptions(OpenApiOptions options)
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    }

    private static Func<IServiceProvider, INatsClient> NatsClientFactory(IConfiguration configuration)
    {
        return Implementation;

        INatsClient Implementation(IServiceProvider provider)
        {
            string url = configuration["natsUrl"] ?? throw new InvalidOperationException("missing nats url");
            return new NatsClient(new NatsOpts { Url = url });
        }
    }
}
