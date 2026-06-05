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
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default);
}
