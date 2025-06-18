using Microsoft.Extensions.Configuration;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Extensions.Configuration;
using Soenneker.Instantly.Client.Abstract;
using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Instantly.OpenApiClient;
using Soenneker.Kiota.BearerAuthenticationProvider;
using Soenneker.Utils.AsyncSingleton;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Extensions.ValueTask;

namespace Soenneker.Instantly.ClientUtil;

///<inheritdoc cref="IInstantlyOpenApiClientUtil"/>
public sealed class InstantlyOpenApiClientUtil : IInstantlyOpenApiClientUtil
{
    private readonly AsyncSingleton<InstantlyOpenApiClient> _client;

    public InstantlyOpenApiClientUtil(IInstantlyClient httpClientUtil, IConfiguration configuration)
    {
        _client = new AsyncSingleton<InstantlyOpenApiClient>(async (token, _) =>
        {
            HttpClient httpClient = await httpClientUtil.Get(token).NoSync();

            var apiKey = configuration.GetValueStrict<string>("Instantly:ApiKey");

            var requestAdapter = new HttpClientRequestAdapter(new BearerAuthenticationProvider(apiKey), httpClient: httpClient);

            return new InstantlyOpenApiClient(requestAdapter);
        });
    }

    public ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default)
    {
        return _client.Get(cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _client.DisposeAsync();
    }
}
