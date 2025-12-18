using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using NUnit.Framework;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.HelperClasses;

namespace TournamentCalendar.Tests;

//[NonParallelizable]
[TestFixture]
public class BasicIntegrationTests
{
    // OneTimeSetUp to create WebApplicationFactory only once for all tests
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private TestServer _server = null!;
    
    private readonly string _credentials =
           """
           {
               // Password is 'password'
               "Authentication": [
                   {
                       "UserName": "admin",
                       "Email": "admin@tourney.net",
                       "PasswordHash": "W6ph5Mm5Pz8GgiULbPgzG37mj9g=",
                       "Roles": [ "Editor", "Admin" ]
                   }
               ]
           }
           """;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // Only called once for all tests in this class
        // to avoid race conditions during parallel test execution
        _factory = new TournamentAppFactory(Environments.Development)
            .WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureAppConfiguration((context, configurationBuilder) =>
                    {
                        // runs AFTER Program.cs
                        var absoluteConfigurationPath = Path.Combine(context.HostingEnvironment.ContentRootPath,
                            "Configuration");
                        configurationBuilder.SetBasePath(absoluteConfigurationPath);
                        configurationBuilder.AddJsonStream(new MemoryStream(Encoding.ASCII.GetBytes(_credentials)));
                    });
                    builder.ConfigureServices((context, services) =>
                    {
                        var dbContext = new DbContext();
                        context.Configuration.Bind(nameof(DbContext), dbContext);
                        dbContext.ConnectionString = context.Configuration.GetConnectionString(dbContext.ConnectionKey)!;

                        services.AddSingleton<IDbContext>(dbContext);
                        services.AddScoped<IAppDb>(s => s.GetRequiredService<IDbContext>().AppDb);
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
        // _factory.Server is null until factory.CreateClient() has been called
        _server = _factory.Server;
    }


    // Tests for Urls used from April 2023
    [TestCase("/")]
    [TestCase("/calendar")]
    [TestCase("/calendar/#id#")]
    [TestCase("/calendar/entry")]
    [TestCase("/calendar/entry/#guid#")]
    [TestCase("/volley-news/register")]
    [TestCase("/contact/message")]
    [TestCase("/organization/apps")]
    [TestCase("/synd/calendar.html")]
    [TestCase("/synd/calendar.css")]
    [TestCase("/synd/calendar.js")]
    [TestCase("/info/about-us")]
    [TestCase("/info/privacy")]
    [TestCase("/cron/heartbeat")]
    [TestCase("/captcha")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
    {
        // Only for endpoints that do not require authentication

        // Act
        if (url.Contains('#')) url = await GetUrlForAnyActiveTournament(url);
        var response = await _client.GetAsync(url);

        // Assert
        Assert.That(() => response.EnsureSuccessStatusCode(), Throws.Nothing); // Status Code 200-299 or throws
    }

    private async Task<string> GetUrlForAnyActiveTournament(string url)
    {
        using var scope = _factory.Services.CreateScope();
        var appDb = scope.ServiceProvider.GetRequiredService<IAppDb>();
        var tournaments = new EntityCollection<CalendarEntity>();
        await appDb.CalendarRepository.GetAllActiveTournaments(tournaments, CancellationToken.None);
        var t = tournaments.First();
        return url.Replace("#id#", t.Id.ToString()).Replace("#guid#", t.Guid);
    }

    [Test]
    public void Registered_Routes()
    {
        var routes = RouteHelper.GetRegisteredRoutes(_factory.Services);

        Assert.That(routes.Any(r => r.Route.Equals("/sign-in", StringComparison.OrdinalIgnoreCase)), Is.True);
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
        const string requestUri = "/sign-in";

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
