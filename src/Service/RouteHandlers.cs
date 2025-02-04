namespace Service;

using Microsoft.AspNetCore.Mvc;

using NATS.Client.Core;

internal static class RouteHandlers
{
    public static async Task GetEventsAsync(
        string topic,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] INatsClient natsClient,
        CancellationToken cancellationToken)
    {
        HttpContext context = contextAccessor.HttpContext!;

        context.Response.Headers.ContentType = "text/event-stream";
        context.Response.Headers.CacheControl = "no-cache";
        context.Response.Headers.Connection = "key-alive";
        context.Response.Headers.AccessControlAllowOrigin = "*";

        await foreach (NatsMsg<string> message in natsClient.SubscribeAsync<string>(topic, cancellationToken: cancellationToken))
        {
            await StreamMessageAsync(message, context.Response, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task StreamMessageAsync(NatsMsg<string> natsMessage, HttpResponse response, CancellationToken cancellationToken)
    {
        await response.WriteAsync($"event: message\ndata: {natsMessage.Data}\n\n", cancellationToken).ConfigureAwait(false);
        await response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}
