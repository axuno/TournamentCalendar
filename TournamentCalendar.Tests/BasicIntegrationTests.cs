using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    // Tests for (mostly localized) Urls before April 2023
    [TestCase("/kalender")]
    [TestCase("/kalender/id/1")]
    [TestCase("/kalender/eintrag")]
    [TestCase("/volley-news")]
    [TestCase("/info/impressum")]
    [TestCase("/info/datenschutz")]
    [TestCase("/kontakt")]
    [TestCase("/orga/auswertung")]
    // Tests for Urls used from April 2023
    [TestCase("/")]
    [TestCase("/calendar")]
    [TestCase("/calendar/1")]
    [TestCase("/calendar/entry")]
    [TestCase("/calendar/entry/0a98470df3424be2acf75d36dcc08ebd")]
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
        #region *** Documentation of routes until March 2023 ***
/*
+		[0]	("GET", "/Admin/NetCoreInfo", "Admin.NetCoreInfo", "TournamentCalendar.Controllers.Admin:NetCoreInfo")	(string, string, string, string)
+		[1]	("GET", "/Admin/Restart", "Admin.Restart", "TournamentCalendar.Controllers.Admin:Restart")	(string, string, string, string)
+		[2]	("GET", "/Admin/HeartBeat", "Admin.HeartBeat", "TournamentCalendar.Controllers.Admin:HeartBeat")	(string, string, string, string)
+		[3]	("GET", "/anmelden", "Auth.SignIn", "TournamentCalendar.Controllers.Auth:SignIn")	(string, string, string, string)
+		[4]	("GET", "/Auth/SignIn", "Auth.SignIn", "TournamentCalendar.Controllers.Auth:SignIn")	(string, string, string, string)
+		[5]	("POST", "/anmelden", "Auth.SignIn", "TournamentCalendar.Controllers.Auth:SignIn")	(string, string, string, string)
+		[6]	("POST", "/Auth/SignIn", "Auth.SignIn", "TournamentCalendar.Controllers.Auth:SignIn")	(string, string, string, string)
+		[7]	("GET", "/abmelden", "Auth.SignOut", "TournamentCalendar.Controllers.Auth:SignOut")	(string, string, string, string)
+		[8]	("GET", "/Auth/SignOut", "Auth.SignOut", "TournamentCalendar.Controllers.Auth:SignOut")	(string, string, string, string)
+		[9]	("GET", "/", "Calendar.Index", "TournamentCalendar.Controllers.Calendar:Index")	(string, string, string, string)
+		[10]	("GET", "/kalender", "Calendar.All", "TournamentCalendar.Controllers.Calendar:All")	(string, string, string, string)
+		[11]	("GET", "/kalender/Id/{id:long}", "Calendar.Id", "TournamentCalendar.Controllers.Calendar:Id")	(string, string, string, string)
+		[12]	("GET", "/kalender/eintrag", "Calendar.NewEntry", "TournamentCalendar.Controllers.Calendar:NewEntry")	(string, string, string, string)
+		[13]	("GET", "/kalender/eintrag/{guid}", "Calendar.Entry", "TournamentCalendar.Controllers.Calendar:Entry")	(string, string, string, string)
+		[14]	("POST", "/kalender/eintrag", "Calendar.Entry", "TournamentCalendar.Controllers.Calendar:Entry")	(string, string, string, string)
+		[15]	("GET", "/kalender/bestaetigen/{guid?}", "Calendar.Approve", "TournamentCalendar.Controllers.Calendar:Approve")	(string, string, string, string)
+		[16]	("GET", "/kalender/integrieren", "Calendar.Integrate", "TournamentCalendar.Controllers.Calendar:Integrate")	(string, string, string, string)
+		[17]	("GET", "/Captcha", "Captcha.Index", "TournamentCalendar.Controllers.Captcha:Index")	(string, string, string, string)
+		[18]	("GET", "/Collect/show/{id?}", "Collect.Show", "TournamentCalendar.Controllers.Collect:Show")	(string, string, string, string)
+		[19]	("GET", "/kontakt", "Contact.Index", "TournamentCalendar.Controllers.Contact:Index")	(string, string, string, string)
+		[20]	("GET", "/kontakt/nachricht", "Contact.Message", "TournamentCalendar.Controllers.Contact:Message")	(string, string, string, string)
+		[21]	("POST", "/kontakt/nachricht", "Contact.Message", "TournamentCalendar.Controllers.Contact:Message")	(string, string, string, string)
+		[22]	("GET", "/synd/kalender.html", "ContentSynd.CalendarList", "TournamentCalendar.Controllers.ContentSynd:CalendarList")	(string, string, string, string)
+		[23]	("GET", "/synd/kalender.css", "ContentSynd.CalendarListCss", "TournamentCalendar.Controllers.ContentSynd:CalendarListCss")	(string, string, string, string)
+		[24]	("GET", "/synd/kalender.js", "ContentSynd.CalendarListJs", "TournamentCalendar.Controllers.ContentSynd:CalendarListJs")	(string, string, string, string)
+		[25]	("GET", "/synd/test", "ContentSynd.Test", "TournamentCalendar.Controllers.ContentSynd:Test")	(string, string, string, string)
+		[26]	("GET", "/Cron", "Cron.Index", "TournamentCalendar.Controllers.Cron:Index")	(string, string, string, string)
+		[27]	("GET", "/Cron/Heartbeat/{id?}", "Cron.Heartbeat", "TournamentCalendar.Controllers.Cron:Heartbeat")	(string, string, string, string)
+		[28]	("GET", "/Cron/Collect/{key}", "Cron.Collect", "TournamentCalendar.Controllers.Cron:Collect")	(string, string, string, string)
+		[29]	("GET", "/error/{id?}", "Error.Index", "TournamentCalendar.Controllers.Error:Index")	(string, string, string, string)
+		[30]	("GET", "/info/impressum", "Info.LegalDetails", "TournamentCalendar.Controllers.Info:LegalDetails")	(string, string, string, string)
+		[31]	("GET", "/info/datenschutz", "Info.PrivacyPolicy", "TournamentCalendar.Controllers.Info:PrivacyPolicy")	(string, string, string, string)
+		[32]	("GET", "/volley-news", "InfoService.Index", "TournamentCalendar.Controllers.InfoService:Index")	(string, string, string, string)
+		[33]	("GET", "/volley-news/eintrag/{guid}", "InfoService.Entry", "TournamentCalendar.Controllers.InfoService:Entry")	(string, string, string, string)
+		[34]	("POST", "/volley-news/eintrag", "InfoService.Entry", "TournamentCalendar.Controllers.InfoService:Entry")	(string, string, string, string)
+		[35]	("POST", "/volley-news/Unsubscribe", "InfoService.Unsubscribe", "TournamentCalendar.Controllers.InfoService:Unsubscribe")	(string, string, string, string)
+		[36]	("GET", "/volley-news/bestaetigen/{guid}", "InfoService.Approve", "TournamentCalendar.Controllers.InfoService:Approve")	(string, string, string, string)
+		[37]	("GET", "/nl/show", "Newsletter.Show", "TournamentCalendar.Controllers.Newsletter:Show")	(string, string, string, string)
+		[38]	("GET", "/nl/send", "Newsletter.Send", "TournamentCalendar.Controllers.Newsletter:Send")	(string, string, string, string)
+		[39]	("GET", "/orga", "Organization.Index", "TournamentCalendar.Controllers.Organization:Index")	(string, string, string, string)
+		[40]	("GET", "/orga/Index", "Organization.Index", "TournamentCalendar.Controllers.Organization:Index")	(string, string, string, string)
+		[41]	("GET", "/orga/auswertung", "Organization.Evaluation", "TournamentCalendar.Controllers.Organization:Evaluation")	(string, string, string, string)
+		[42]	("GET", "/orga/download/{id?}", "Organization.Download", "TournamentCalendar.Controllers.Organization:Download")	(string, string, string, string)
*/
        #endregion

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
