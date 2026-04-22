using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Instantly.ClientUtil.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class InstantlyOpenApiClientUtilTests : HostedUnitTest
{
    private readonly IInstantlyOpenApiClientUtil _kiotaclient;

    public InstantlyOpenApiClientUtilTests(Host host) : base(host)
    {
        _kiotaclient = Resolve<IInstantlyOpenApiClientUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
