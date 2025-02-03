using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Service;

using NATS.Client.Core;
using NATS.Net;

internal static class ProgramConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(ConfigureOpenApiOptions);
        services.AddCors(ConfigureCors);
        services.AddHttpContextAccessor();

        services.AddTransient<HttpContext>(provider =>
        {
            var accessor = ActivatorUtilities.GetServiceOrCreateInstance<IHttpContextAccessor>(provider);

            return accessor.HttpContext!;
        });

        services.AddScoped(NatsClientFactory(configuration));
    }

    private static Func<IServiceProvider, INatsClient> NatsClientFactory(IConfiguration configuration)
    {
        return Implementation;

        INatsClient Implementation(IServiceProvider provider)
        {
            string url = configuration["natsUrl"] ?? throw new InvalidOperationException("missing nats url");
            NatsOpts options = new() { Url = url };
            var client = new NatsClient(options);
            return client;
        }
    }

    private static void ConfigureCors(CorsOptions options)
    {
        options.AddPolicy("default", ConfigureDefaultPolicy);

        // ReSharper disable once SeparateLocalFunctionsWithJumpStatement
        static void ConfigureDefaultPolicy(CorsPolicyBuilder builder)
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        }
    }

    private static void ConfigureOpenApiOptions(OpenApiOptions options)
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    }
}
