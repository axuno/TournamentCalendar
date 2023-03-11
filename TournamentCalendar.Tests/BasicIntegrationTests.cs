using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace TournamentCalendar.Tests;

[TestFixture]
public class BasicIntegrationTests
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public BasicIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder.UseEnvironment(Environments.Development);
                        builder.ConfigureAppConfiguration((context, configurationBuilder) => { });
                        builder.ConfigureTestServices(services =>
                        {
                        });
                    });
    }

    [TestCase("/")]
    [TestCase("/kalender")]
    [TestCase("/kalender/id/1")]
    [TestCase("/kalender/eintrag")]
    [TestCase("/volley-news")]
    [TestCase("/synd/test")]
    [TestCase("/info/impressum")]
    [TestCase("/info/datenschutz")]
    [TestCase("/kontakt")]
    [TestCase("/orga/auswertung")]
    [TestCase("/anmelden")]
    [TestCase("/cron/heartbeat")]
    [TestCase("/captcha")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Todo:
        // https://code-maze.com/aspnet-core-testing-anti-forgery-token/

        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);
        
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
    }

    [Test]
    public void Registered_Routes()
    {
        var routes = RouteHelper.GetRegisteredRoutes(_factory.Services);

        Assert.That(routes.Any(r => r.Route.Equals("/anmelden", StringComparison.OrdinalIgnoreCase)), Is.True);
    }
}
