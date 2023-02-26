using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NLog.Extensions.Logging;

namespace TournamentCalendar;

public class Program
{
    /// <summary>
    /// The name of the configuration folder, which is relative to HostingEnvironment.ContentRootPath.
    /// Constant is also used in components where IWebHostEnvironment is injected
    /// </summary>
    public const string ConfigurationFolder = "Configuration";

    public static async Task Main(string[] args)
    {
        // NLog: setup the logger first to catch all errors
        var currentDir = Directory.GetCurrentDirectory();
        var logger = NLogBuilder
            .ConfigureNLog($@"{currentDir}{Path.DirectorySeparatorChar}{Program.ConfigurationFolder}{Path.DirectorySeparatorChar}NLog.Internal.config")
            .GetCurrentClassLogger();

        // Allows for <target name="file" xsi:type="File" fileName = "${var:logDirectory}logfile.log"... >
        NLog.LogManager.Configuration.Variables["logDirectory"] = currentDir + Path.DirectorySeparatorChar;

        try
        {
            logger.Trace($"Configuration of {nameof(WebHost)} starting.");
            // http://zuga.net/articles/cs-how-to-determine-if-a-program-process-or-file-is-32-bit-or-64-bit/
            logger.Info($"This app runs as {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} process.\n\n");
                
            var builder = SetupBuilder(args);

            var loggingConfig = builder.Configuration.GetSection("Logging");
            builder.Logging.ClearProviders();
            // Enable NLog as logging provider for Microsoft.Extension.Logging
            builder.Logging.AddNLog(loggingConfig);
            NLogBuilder.ConfigureNLog(Path.Combine(builder.Environment.ContentRootPath, ConfigurationFolder,
                $"NLog.{builder.Environment.EnvironmentName}.config"));

            builder.WebHost.ConfigureServices(WebAppStartup.ConfigureServices);

            var app = builder.Build();

            builder.WebHost.ConfigureAppConfiguration(_ =>
            {
                WebAppStartup.Configure(app, app.Services.GetRequiredService<ILoggerFactory>());
            });
            
            await app.RunAsync();
        }
        catch (Exception e)
        {
            logger.Fatal(e, $"Application stopped after Exception. {e.Message}");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            NLog.LogManager.Shutdown();
        }
    }

    public static WebApplicationBuilder SetupBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.GetName().Name, // don't use Assembly.Fullname
            WebRootPath = "wwwroot"
            // Note: ContentRootPath is detected by the framework.
            //       If set explicitly as WebApplicationOptions, 
            //       WebApplicationFactory in unit tests does not override it.
        });

        var absoluteConfigurationPath = Path.Combine(builder.Environment.ContentRootPath,
            ConfigurationFolder);

        builder.Configuration.SetBasePath(absoluteConfigurationPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true, reloadOnChange: true)
            .AddJsonFile(@"credentials.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"credentials.{builder.Environment.EnvironmentName}.json",
                optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        if (builder.Environment.IsDevelopment())
        {
            var secretsFolder = FindParentFolder("Secrets", builder.Environment.ContentRootPath);    
            builder.Configuration.AddJsonFile(Path.Combine(secretsFolder, @"credentials.json"), false);
            builder.Configuration.AddJsonFile(Path.Combine(secretsFolder, $"credentials.{builder.Environment.EnvironmentName}.json"), false);
        }

        // Use static web assets, also from other referenced projects or packages)
        builder.WebHost.UseStaticWebAssets();

        return builder;
    }

    /// <summary>
    /// Searches the parent directories of <paramref name="startPath"/> for
    /// the first directory with name of <paramref name="directoryName"/>.
    /// </summary>
    /// <param name="directoryName">The name of the directory to search.</param>
    /// <param name="startPath">The path where the search starts.</param>
    /// <returns>The full path of the found folder.</returns>
    /// <exception cref="DirectoryNotFoundException">
    /// The <paramref name="startPath"/> does not exist, or the <paramref name="directoryName"/>
    /// was not found in the parent directories.
    /// </exception>
    /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission.</exception>
    /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    public static string FindParentFolder(string directoryName, string startPath)
    {
        if (!Directory.Exists(startPath))
        {
            throw new DirectoryNotFoundException( $"Start path '{startPath}' not found.");
        }

        var currentPath = startPath;
        var rootReached = false;

        while (!rootReached && !Directory.Exists(Path.Combine(currentPath, directoryName)))
        {
            currentPath = Directory.GetParent(currentPath)?.FullName;
            rootReached = currentPath == null;
            currentPath ??= Directory.GetDirectoryRoot(startPath);
        }
        
        var resultPath = Path.Combine(currentPath, directoryName);
        if (!Directory.Exists(resultPath)) throw new DirectoryNotFoundException( $"Folder '{directoryName}' not found in parent directories.");
        return resultPath;
    }
}
