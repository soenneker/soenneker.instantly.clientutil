using System;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Instantly.OpenApiClient;

namespace Soenneker.Instantly.ClientUtil.Abstract;

/// <summary>
/// Provides a lazily created Instantly generated client over a reusable authenticated transport.
/// </summary>
public interface IInstantlyOpenApiClientUtil: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets the Instantly generated client.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the configured client.</returns>
    ValueTask<InstantlyOpenApiClient> Get(CancellationToken cancellationToken = default);
}
