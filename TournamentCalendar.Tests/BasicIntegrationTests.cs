using System.Net;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;

namespace TournamentCalendar.Tests;

[TestFixture]
public class BasicIntegrationTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    private readonly string _credentials = @"{
    // Password is 'password'
    ""Authentication"": [
        {
            ""UserName"": ""admin"",
            ""Email"": ""admin@tourney.net"",
            ""PasswordHash"": ""W6ph5Mm5Pz8GgiULbPgzG37mj9g="",
            ""Roles"": [ ""Editor"", ""Admin"" ]
        }
    ]
}";
    public BasicIntegrationTests()
    {
        _factory = new TournamentAppFactory(Environments.Development)
            .WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                    {
                        // runs AFTER Program.cs
                        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(_credentials)));
                    });
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddAntiforgery(t =>
                        {
                            t.Cookie.Name = AntiForgeryTokenExtractor.AntiForgeryCookieName;
                            t.FormFieldName = AntiForgeryTokenExtractor.AntiForgeryFieldName;
                        });
                    });
                });

        _client = _factory.CreateClient();
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
    [TestCase("/cron/heartbeat")]
    [TestCase("/captcha")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Only for endpoints that do not require authentication

        // Act
        var response = await _client.GetAsync(url);
        
        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299 or throws
    }

    [Test]
    public void Registered_Routes()
    {
        var routes = RouteHelper.GetRegisteredRoutes(_factory.Services);

        Assert.That(routes.Any(r => r.Route.Equals("/anmelden", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    /// <summary>
    /// <b>Create a new HttpClient for each TestCase</b> here:
    /// HttpClient is sending request cookies from previous requests' responses (https://stackoverflow.com/questions/70595366/httpclient-is-sending-request-cookies-from-other-requests-responses)
    /// In this case, repeated AntiForgeryTokenExtractor.ExtractAntiForgeryValues(...) would fail,
    /// because the server sends the cookie only if not present in the request.
    /// The good thing: Re-using the HttpClient also keeps the Authentication cookie for subsequent requests.
    /// </summary>
    [TestCase("not-existing", "not-existing", "Benutzer nicht gefunden")]
    [TestCase("admin", "password", "/admin/netcoreinfo")]
    public async Task TryToLogin_ReturnsExpectedString(string user, string password, string successContent)
    {
        const string requestUri = "/anmelden";

        var client = _factory.CreateClient();
        var initResponse = await client.GetAsync(requestUri);
        var antiForgeryValues = await AntiForgeryTokenExtractor.ExtractAntiForgeryValues(initResponse);

        var postRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
        postRequest.Headers.Add("Cookie", new CookieHeaderValue(AntiForgeryTokenExtractor.AntiForgeryCookieName, antiForgeryValues.cookieValue).ToString());
        var formModel = new Dictionary<string, string>
        {
            { AntiForgeryTokenExtractor.AntiForgeryFieldName, antiForgeryValues.fieldValue },
            { "EmailOrUsername", user },
            { "Password", password }
        };
        postRequest.Content = new FormUrlEncodedContent(formModel);
        var response = await client.SendAsync(postRequest);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        
        Assert.That(responseString, Does.Contain(successContent));
    }
}
