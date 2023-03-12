using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace TournamentCalendar.Tests;

internal class TournamentAppFactory : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public TournamentAppFactory(string environment = nameof(Environments.Development))
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        builder.ConfigureAppConfiguration(config =>
        {
            // runs AFTER Program.cs
        });

        // Add mock/test services to the builder here
        builder.ConfigureServices(services =>
        {
        });
        
        return base.CreateHost(builder);
    }
}
