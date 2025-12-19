using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.ValueTask;
using Soenneker.Instantly.Client.Abstract;
using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Instantly.OpenApiClient;
using Soenneker.Kiota.BearerAuthenticationProvider;
using Soenneker.Utils.AsyncSingleton;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.HttpClients.LoggingHandler;

namespace Soenneker.Instantly.ClientUtil;

///<inheritdoc cref="IInstantlyOpenApiClientUtil"/>
public sealed class InstantlyOpenApiClientUtil : IInstantlyOpenApiClientUtil
{
    private readonly AsyncSingleton<InstantlyOpenApiClient> _client;

    private HttpClient? _httpClient;

    public InstantlyOpenApiClientUtil(IInstantlyClient httpClientUtil, IConfiguration configuration, ILogger<InstantlyOpenApiClientUtil> logger)
    {
        _client = new AsyncSingleton<InstantlyOpenApiClient>(async token =>
        {
            var logging = configuration.GetValue<bool>("Instantly:RequestResponseLogging");

            if (logging)
            {
                var loggingHandler = new HttpClientLoggingHandler(logger, new HttpClientLoggingOptions
                {
                    LogLevel = LogLevel.Debug
                });

                loggingHandler.InnerHandler = new HttpClientHandler();

                _httpClient = new HttpClient(loggingHandler);
            }
            else
            {
                _httpClient = await httpClientUtil.Get(token).NoSync();
            }

            var apiKey = configuration.GetValueStrict<string>("Instantly:ApiKey");

            var requestAdapter = new HttpClientRequestAdapter(new BearerAuthenticationProvider(apiKey), httpClient: _httpClient);

            return new InstantlyOpenApiClient(requestAdapter);
        });
    }

    public ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default)
    {
        return _client.Get(cancellationToken);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();

        _client.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _httpClient?.Dispose();

        return _client.DisposeAsync();
    }
}
