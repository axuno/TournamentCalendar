using System.Diagnostics;
using Microsoft.Net.Http.Headers;
using System.Text;
using JSNLog;
using MailMergeLib;
using MailMergeLib.MessageStore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendar.Middleware;
using TournamentCalendar.Services;

namespace TournamentCalendar;

/// <summary>
/// The demo startup class to set up and configure the tournament calendar.
/// </summary>
public static class WebAppStartup
{
    /// <summary>
    /// The method gets called by <see cref="Program"/> at startup, BEFORE building the app is completed.
    /// </summary>
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var context = new WebHostBuilderContext
            { Configuration = builder.Configuration, HostingEnvironment = builder.Environment };

        #region * DataProtection service configuration *

        // required for cookies and session cookies (will throw CryptographicException without)
        // Usage: 
        // private readonly IDataProtector protector;
        // public SomeController(IDataProtectionProvider provider)
        // {  protector = provider.CreateProtector("isolation purpose");}
        // public IActionResult Test(string input)
        // { var protectedPayload = protector.Protect(input);
        // var unprotectedPayload = protector.Unprotect(protectedPayload)
        // ...}
        services.AddDataProtection()
            .SetApplicationName(context.HostingEnvironment.ApplicationName)
            .SetDefaultKeyLifetime(TimeSpan.FromDays(360))
            .PersistKeysToFileSystem(
                new DirectoryInfo(Path.Combine(context.HostingEnvironment.ContentRootPath, "DataProtectionKeys")))
            .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration() {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            });
        #endregion

        // Adds a default in-memory cache implementation
        services.AddMemoryCache();

        // Add services required for using options.
        services.AddOptions();

        // Configure form upload limits
        services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(fo =>
        {
            fo.ValueLengthLimit = int.MaxValue;
            fo.MultipartBodyLengthLimit = int.MaxValue;
        });

        services.Configure<IISOptions>(options => { });
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

        // MUST be before AddMvc!
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.HttpOnly = true;
            options.Cookie.Name = ".tc.sid";
            options.Cookie.IsEssential = true;
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICookieService, CookieService>();

        services.AddLocalization(options => options.ResourcesPath = "App_GlobalResources");

        services.AddRazorPages();

        var mvcBuilder = services.AddMvc(options =>
            {
                options.EnableEndpointRouting = true; // default
                // Add model binding messages for errors that do not reach data annotation validation
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x =>
                    string.Format(Resources.ModelBindingMessageResource.ValueMustNotBeNull));
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, val) =>
                    string.Format(Resources.ModelBindingMessageResource.AttemptedValueIsInvalid, x, val));
                options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x =>
                    string.Format(Resources.ModelBindingMessageResource.ValueIsInvalid, x));
                options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x =>
                    string.Format(Resources.ModelBindingMessageResource.ValueMustBeANumber, x));
                options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() =>
                    Resources.ModelBindingMessageResource.MissingKeyOrValue);
            })
            .AddSessionStateTempDataProvider()
            .AddDataAnnotationsLocalization()
            .AddControllersAsServices()
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = false)
            .AddMvcOptions(options =>
            {
                // Insert e.g. custom model binder providers
            });
#if DEBUG
        // Not to be added in production
        if (context.HostingEnvironment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
#endif  
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
        });

        var dbContext = new Data.DbContext();
        context.Configuration.Bind(nameof(Data.DbContext), dbContext);
        dbContext.ConnectionString = context.Configuration.GetConnectionString(dbContext.ConnectionKey)!;

        ConfigureLlblgenPro(dbContext, context.HostingEnvironment);
        
        services.TryAddSingleton<Data.IDbContext>(dbContext);
        services.AddScoped<Data.IAppDb>(s => s.GetRequiredService<Data.IDbContext>().AppDb);
        services.AddScoped<UserLocationService>();

        services.AddTransient<ClientAbortMiddleware>();

        #region *** Add CloudScribeNavigation ***

        // CloudscribeNavigation requires:
        // ~/Views/Shared/NavigationNodeChildDropdownPartial.cshtml
        // ~/Views/Shared/NavigationNodeChildTreePartial.cshtml
        // ~/Views/Shared/NavigationNodeSideNavPartial.cshtml
        // ~/Views/Shared/Components/Navigation/*.cshtml
        // ~/Views/_ViewImports.cshtml: @using cloudscribe.Web.Navigation
        services.AddCloudscribeNavigation(context.Configuration.GetSection("NavigationOptions")!); //.Configure<NavigationOptions>(o => o.NavigationMapXmlFileName = ConfigurationFolder + @"\Navigation.xml");

        #endregion

        #region *** Cookie Authentication ***

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.LoginPath = new PathString("/auth/signin");
                    options.LogoutPath = new PathString("/auth/signoff");
                    options.AccessDeniedPath = new PathString("/auth/denied");
                    options.Cookie.Name = ".TournamentsAuth";
                });
        
        #endregion

        #region *** MailMergeLib as a service ***

        services.AddMailMergeService(
            options =>
            {
                options.Settings = Settings.Deserialize(
                    Path.Combine(context.HostingEnvironment.ContentRootPath, Program.ConfigurationFolder,
                        $@"MailMergeLib.{context.HostingEnvironment.EnvironmentName}.config"),
                    Encoding.UTF8)!;
                var fms = FileMessageStore.Deserialize(Path.Combine(context.HostingEnvironment.ContentRootPath, Program.ConfigurationFolder,
                    "MailMergeLibMessageStore.config"), Encoding.UTF8)!;
                for (var i = 0; i < fms.SearchFolders.Length; i++)
                {
                    // make relative paths absolute - ready to use
                    fms.SearchFolders[i] = Path.Combine(context.HostingEnvironment.ContentRootPath, fms.SearchFolders[i]);
                }
                options.MessageStore = fms;
            });

        #endregion
    }

    private static void ConfigureLlblgenPro(Data.IDbContext dbContext, IWebHostEnvironment environment)
    {
        RuntimeConfiguration.AddConnectionString(dbContext.ConnectionKey, dbContext.ConnectionString);

        if (environment.IsProduction())
        {
            RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.SqlServer.SQLServerDQEConfiguration>(c => c
                .SetTraceLevel(TraceLevel.Off)
                .AddDbProviderFactory(typeof(Microsoft.Data.SqlClient.SqlClientFactory)));
        }
        else
        {
            RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.SqlServer.SQLServerDQEConfiguration>(static c => c
                .SetTraceLevel(TraceLevel.Verbose)
                .AddDbProviderFactory(typeof(Microsoft.Data.SqlClient.SqlClientFactory)));

            RuntimeConfiguration.Tracing.SetTraceLevel("ORMPersistenceExecution", TraceLevel.Verbose);
            RuntimeConfiguration.Tracing.SetTraceLevel("ORMPlainSQLQueryExecution", TraceLevel.Verbose);
        }
    }

    /// <summary>
    /// The method gets called by <see cref="Program"/> at startup, AFTER building the app is completed.
    /// </summary>
    public static void Configure(WebApplication app)
    {
        var env = app.Environment;
        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        Data.AppLogging.Configure(loggerFactory);

        app.UseHttpsRedirection();

        var cultureInfo = CultureInfo.GetCultureInfo("de-DE");
        CultureInfo.DefaultThreadCurrentCulture =
            CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture =
            CultureInfo.CurrentUICulture = cultureInfo;

        #region * Supply 'Stackify Prefix' profiler with StackifyMiddleware *

        if (env.IsDevelopment())
        {
            app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
        }

        #endregion

        #region * Rewrite all domain names to https://volleyball-turnier.de; redirect favicon.ico *

        if (env.IsProduction())
        {
            using var iisUrlRewriteStreamReader = File.OpenText(Path.Combine(env.ContentRootPath, Program.ConfigurationFolder, @"IisRewrite.config"));
            var options = new RewriteOptions()
                .AddIISUrlRewrite(iisUrlRewriteStreamReader);
            options.AddRedirect(@"https://volleyball-turnier\.de/favicon\.ico", "/favicon/favicon.ico", StatusCodes.Status301MovedPermanently);
            app.UseRewriter(options);
        }

        #endregion

        #region * Setup error handling *

        // Error handling must be one of the very first things to configure
        if (env.IsProduction())
        {
            // The StatusCodePagesMiddleware should be one of the earliest 
            // middleware in the pipeline, as it can only modify the response 
            // of middleware that comes after it in the pipeline
            app.UseStatusCodePagesWithReExecute($"/{nameof(TournamentCalendar.Controllers.Error)}/{{0}}");
            app.UseExceptionHandler($"/{nameof(TournamentCalendar.Controllers.Error)}/500");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
        }

        #endregion

        #region *** Javascript logging ***

        // add the JSNLog middleware before the UseStaticFiles middleware.
        var jsNLogConfiguration =
            new JsnlogConfiguration
            {
                loggers = new List<Logger>
                {
                    new() { name = "JsLogger" }
                }
            };
        app.UseJSNLog(new LoggingAdapter(loggerFactory), jsNLogConfiguration);

        #endregion

        #region *** Static files ***

        // For static files in the wwwroot folder
        app.UseDefaultFiles();

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(30)
                };
            }
        });
        // For static files using a content type provider:
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        // Make sure .webmanifest files don't cause a 404
        provider.Mappings[".webmanifest"] = "application/manifest+json";
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider,
            OnPrepareResponse = ctx =>
            {
                var headers = ctx.Context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromDays(30)
                };
            }
        });

        #endregion

        app.UseCookiePolicy();
        app.UseSession();
        app.UseRouting();
        // UseCors: before UseResponseCaching
        app.UseCors();
        // UseAuthentication and UseAuthorization: after UseRouting and UseCors, but before UseEndpoints
        app.UseAuthentication().UseAuthorization();
        // The net6.0+ top level replacement for app.UseEndpoints(...)
        // (WebApplication implements IEndpointRouteBuilder)
        app.MapControllers();
        app.MapRazorPages();

        // Suppress exceptions when the connection is closed by the client
        app.UseMiddleware<ClientAbortMiddleware>();
    }
}
