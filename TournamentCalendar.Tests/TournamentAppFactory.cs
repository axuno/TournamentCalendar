using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace TournamentCalendar.Tests;

public class TournamentAppFactory : WebApplicationFactory<Program>
{
    
    public TournamentAppFactory()
    {
    }
    
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        var builder = base.CreateWebHostBuilder();
        return builder!;
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder
            .ConfigureServices(services =>
            {

            }).ConfigureAppConfiguration(app =>
            {

            });
        builder.Build();
    }
}
