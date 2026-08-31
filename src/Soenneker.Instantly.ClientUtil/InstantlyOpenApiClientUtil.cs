using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.ValueTask;
using Soenneker.HttpClients.LoggingHandler;
using Soenneker.Instantly.Client.Abstract;
using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Instantly.OpenApiClient;
using Soenneker.Utils.AsyncSingleton;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Instantly.ClientUtil;

public sealed class InstantlyOpenApiClientUtil : IInstantlyOpenApiClientUtil, IDisposable, IAsyncDisposable
{
    private readonly AsyncSingleton<InstantlyOpenApiClient> _client;

    private readonly IInstantlyClient _httpClientUtil;
    private readonly IConfiguration _configuration;
    private readonly ILogger<InstantlyOpenApiClientUtil> _logger;

    private HttpClient? _httpClient;
    private bool _ownsHttpClient;

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
        var apiKey = _configuration.GetValueStrict<string>("Instantly:ApiKey");

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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _ownsHttpClient = true;
        }
        else
        {
            _httpClient = await _httpClientUtil.Get(token)
                                               .NoSync();
        }

        var requestAdapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: _httpClient);

        return new InstantlyOpenApiClient(requestAdapter);
    }

    public ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default) => _client.Get(cancellationToken);

    public void Dispose()
    {
        _client.Dispose();

        if (_ownsHttpClient)
            _httpClient?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync().ConfigureAwait(false);

        if (_ownsHttpClient)
            _httpClient?.Dispose();
    }
}
