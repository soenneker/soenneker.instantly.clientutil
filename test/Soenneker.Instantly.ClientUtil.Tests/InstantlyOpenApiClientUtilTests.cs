using Soenneker.Instantly.ClientUtil.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Instantly.ClientUtil.Tests;

[Collection("Collection")]
public sealed class InstantlyOpenApiClientUtilTests : FixturedUnitTest
{
    private readonly IInstantlyOpenApiClientUtil _kiotaclient;

    public InstantlyOpenApiClientUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _kiotaclient = Resolve<IInstantlyOpenApiClientUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
