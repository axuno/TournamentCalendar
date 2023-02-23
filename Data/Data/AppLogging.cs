using Microsoft.Extensions.Logging;

namespace TournamentCalendar;

/// <summary>
/// Allows for using the <see cref="Microsoft.Extensions.Logging"/> facade in library classes 
/// without dependency injection.
/// </summary>
/// <remarks>
/// Note: LoggerFactory.CreateLogger is thread-safe.
/// Idea going back to https://stackify.com/net-core-loggerfactory-use-correctly/
/// </remarks>
public static class AppLogging
{
    private static ILoggerFactory? _factory;
    public static void Configure(ILoggerFactory factory)
    {
        // ASP.NET will only write its internal logging to the LoggerFactory object 
        // that it creates at app startup.
        // So in Startup.Configure, we grab that reference of the LoggerFactory
        // and set it to this logging class so it becomes the primary reference.
        _factory = factory;
            
        // _factory.AddProvider(new NLog.Extensions.Logging.NLogLoggerProvider());
    }

    public static ILoggerFactory LoggerFactory
    {
        get
        {
            if (_factory == null)
            {
                // Default LoggerFactory does not contain any ILoggerProvider 
                // and must therefore be overwritten with the proper implementation.
                Configure(new LoggerFactory());
            }
            return _factory!;
        }
        set { _factory = value; }
    }
    public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

    public static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);
}