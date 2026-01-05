using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.ValueTask;
using Soenneker.HttpClients.LoggingHandler;
using Soenneker.Instantly.Client.Abstract;
using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Instantly.OpenApiClient;
using Soenneker.Kiota.BearerAuthenticationProvider;
using Soenneker.Utils.AsyncSingleton;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Instantly.ClientUtil;

/// <inheritdoc cref="IInstantlyOpenApiClientUtil"/>
public sealed class InstantlyOpenApiClientUtil : IInstantlyOpenApiClientUtil, IDisposable, IAsyncDisposable
{
    private readonly AsyncSingleton<InstantlyOpenApiClient> _client;

    private readonly IInstantlyClient _httpClientUtil;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InstantlyOpenApiClientUtil> _logger;

    private HttpClient? _httpClient;

    public InstantlyOpenApiClientUtil(IInstantlyClient httpClientUtil, IConfiguration configuration, ILogger<InstantlyOpenApiClientUtil> logger)
    {
        _httpClientUtil = httpClientUtil;
        _configuration = configuration;
        _logger = logger;

        // Method group → no closure allocation
        _client = new AsyncSingleton<InstantlyOpenApiClient>(CreateClient);
    }

    private async ValueTask<InstantlyOpenApiClient> CreateClient(CancellationToken token)
    {
        var logging = _configuration.GetValue<bool>("Instantly:RequestResponseLogging");

        if (logging)
        {
            var loggingHandler = new HttpClientLoggingHandler(_logger, new HttpClientLoggingOptions
            {
                LogLevel = LogLevel.Debug
            })
            {
                InnerHandler = new HttpClientHandler()
            };

            _httpClient = new HttpClient(loggingHandler);
        }
        else
        {
            _httpClient = await _httpClientUtil.Get(token)
                                               .NoSync();
        }

        var apiKey = _configuration.GetValueStrict<string>("Instantly:ApiKey");

        var requestAdapter = new HttpClientRequestAdapter(new BearerAuthenticationProvider(apiKey), httpClient: _httpClient);

        return new InstantlyOpenApiClient(requestAdapter);
    }

    public ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default) => _client.Get(cancellationToken);

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