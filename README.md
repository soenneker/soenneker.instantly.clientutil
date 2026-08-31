[![](https://img.shields.io/nuget/v/soenneker.instantly.clientutil.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.instantly.clientutil/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.instantly.clientutil/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.instantly.clientutil/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.instantly.clientutil.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.instantly.clientutil/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.instantly.clientutil/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.instantly.clientutil/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Instantly.ClientUtil

Create and reuse an authenticated Instantly generated client, with optional HTTP request and response logging.

## Install

```bash
dotnet add package Soenneker.Instantly.ClientUtil
```

## Configure

```json
{
  "Instantly": {
    "ApiKey": "<API key>",
    "RequestResponseLogging": false
  }
}
```

`ApiKey` is required. Set `RequestResponseLogging` to `true` to log HTTP traffic at debug level; consider the sensitivity of message bodies and headers before enabling it.

## Register

```csharp
using Soenneker.Instantly.ClientUtil.Registrars;

services.AddInstantlyOpenApiClientUtilAsScoped();
```

The scoped utility deliberately keeps `IInstantlyClient` singleton. Without HTTP logging, disposing a scope releases only the generated-client wrapper and keeps the shared transport available to later scopes. With HTTP logging enabled, each utility creates and disposes its own logging transport.

Use `AddInstantlyOpenApiClientUtilAsSingleton()` when the generated client should live for the application lifetime.

## Usage

```csharp
using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Instantly.OpenApiClient;
using Soenneker.Instantly.OpenApiClient.Models;

InstantlyOpenApiClient client = await clientUtil.Get(cancellationToken);

ListAccount200Response? accounts = await client.Api.V2.Accounts.GetAsync(
    config => config.QueryParameters.Limit = 10,
    cancellationToken);
```

Concurrent and repeated `Get()` calls on the same utility reuse its lazily created generated client. Cancellation affects first-time initialization; pass the token separately to generated request methods as shown above.

Authentication is applied by the selected HTTP transport, so the Kiota adapter does not add a second bearer header. Let the service container dispose the utility and underlying provider.
