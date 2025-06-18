using System;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Instantly.OpenApiClient;

namespace Soenneker.Instantly.ClientUtil.Abstract;

/// <summary>
/// A .NET thread-safe singleton HttpClient for 
/// </summary>
public interface IInstantlyOpenApiClientUtil: IDisposable, IAsyncDisposable
{
    ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default);
}
