using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Net.Http.Headers;
using System.Text;
using MailMergeLib;
using MailMergeLib.MessageStore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendar.Data;

namespace TournamentCalendar
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// The Startup class configures services and the application's request pipeline. 
        /// </summary>
        /// <param name="env"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;

            ConfigureLlblgenPro();
        }

        private void ConfigureLlblgenPro()
        {
            // strings used in ConnectionStrings.json:
            const string tournamentsConnectionToUseKeyName = "TournamentsConnectionToUse";

            var connStringKeyName = Configuration.GetValue<string>(tournamentsConnectionToUseKeyName);
            RuntimeConfiguration.AddConnectionString(Connecter.DefaultConnection, Configuration.GetConnectionString(connStringKeyName));

            if (WebHostEnvironment.IsProduction())
            {
                RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.SqlServer.SQLServerDQEConfiguration>(c => c
                    .SetTraceLevel(TraceLevel.Off)
                    .AddDbProviderFactory(typeof(System.Data.SqlClient.SqlClientFactory)));
            }
            else
            {
                RuntimeConfiguration.ConfigureDQE<SD.LLBLGen.Pro.DQE.SqlServer.SQLServerDQEConfiguration>(c => c
                    .SetTraceLevel(TraceLevel.Verbose)
                    .AddDbProviderFactory(typeof(System.Data.SqlClient.SqlClientFactory)));

                RuntimeConfiguration.Tracing.SetTraceLevel("ORMPersistenceExecution", TraceLevel.Verbose);
                RuntimeConfiguration.Tracing.SetTraceLevel("ORMPlainSQLQueryExecution", TraceLevel.Verbose);
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            #region * DataProtection service configuration *

            // Usage: 
            // private readonly IDataProtector protector;
            // public SomeController(IDataProtectionProvider provider)
            // {  protector = provider.CreateProtector("isolation purpose");}
            // public IActionResult Test(string input)
            // { var protectedPayload = protector.Protect(input);
            // var unprotectedPayload = protector.Unprotect(protectedPayload)
            // ...}
            services.AddDataProtection()
                .SetApplicationName("TournamentCalendar")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(30))
                .PersistKeysToFileSystem(
                    new DirectoryInfo(Path.Combine(WebHostEnvironment.ContentRootPath, "DataProtectionKeys")))
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                });

            #endregion

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

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMemoryCache(); // Adds a default in-memory cache implementation

            // MUST be before AddMvc!
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ".tc.sid";
            });

            services.AddLocalization(options => options.ResourcesPath = "App_GlobalResources");

            var mvcBuilder = services.AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
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
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddMvcOptions(options =>
                {
                    // Insert e.g. custom model binder providers
                });
#if DEBUG
            // Not to be added in production!
            if (WebHostEnvironment.IsDevelopment())
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }
#endif            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region *** Add CloudScribeNavigation ***

            // CloudscribeNavigation requires:
            // ~/Views/Shared/NavigationNodeChildDropdownPartial.cshtml
            // ~/Views/Shared/NavigationNodeChildTreePartial.cshtml
            // ~/Views/Shared/NavigationNodeSideNavPartial.cshtml
            // ~/Views/Shared/Components/Navigation/*.cshtml
            // ~/Views/_ViewImports.cshtml: @using cloudscribe.Web.Navigation
            services.AddCloudscribeNavigation(Configuration.GetSection("NavigationOptions")); //.Configure<NavigationOptions>(o => o.NavigationMapXmlFileName = ConfigurationFolder + @"\Navigation.xml");

            #endregion

            #region *** Cookie Authentication ***

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.LoginPath = new PathString("/auth/signin");
                        options.LogoutPath = new PathString("/auth/signoff");
                        options.AccessDeniedPath = new PathString("/auth/denied");
                        options.Cookie.Name = "Tournaments";
                    });
            
            #endregion

            #region *** MailMergeLib as a service ***

            services.AddMailMergeService(
                options =>
                {
                    options.Settings = Settings.Deserialize(
                        Path.Combine(WebHostEnvironment.ContentRootPath, Program.ConfigurationFolder,
                            $@"MailMergeLib.{WebHostEnvironment.EnvironmentName}.config"),
                        Encoding.UTF8);
                    var fms = FileMessageStore.Deserialize(Path.Combine(WebHostEnvironment.ContentRootPath, Program.ConfigurationFolder,
                        "MailMergeLibMessageStore.config"), Encoding.UTF8);
                    for (var i = 0; i < fms.SearchFolders.Length; i++)
                    {
                        // make relative paths absolute - ready to use
                        fms.SearchFolders[i] = Path.Combine(WebHostEnvironment.WebRootPath, fms.SearchFolders[i]);
                    }
                    options.MessageStore = fms;
                });

            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            #region *** Logging ***

            // Add NLog as logging provider. NLog config is set in Program.Main()
            // see: https://github.com/NLog/NLog/issues/2859
            //app.ApplicationServices.SetupNLogServiceLocator();

            // Allow TournamentManager to make use of Microsoft.Extensions.Logging
            TournamentCalendar.AppLogging.Configure(loggerFactory);

            #endregion

            #region * Supply 'Stackify Prefix' profiler with StackifyMiddleware *

            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
            }

            #endregion

            #region * Rewrite all domain names to https://volleyball-turnier.de *

            if (WebHostEnvironment.IsProduction())
            {
                using var iisUrlRewriteStreamReader = File.OpenText(Path.Combine(WebHostEnvironment.ContentRootPath, Program.ConfigurationFolder, @"IisRewrite.config"));
                var options = new RewriteOptions()
                    .AddIISUrlRewrite(iisUrlRewriteStreamReader);
                app.UseRewriter(options);
            }

            #endregion

            #region * Set error handling pages *

            if (WebHostEnvironment.IsProduction())
            {
                // The StatusCodePagesMiddleware should be one of the earliest 
                // middleware in the pipeline, as it can only modify the response 
                // of middleware that comes after it in the pipeline
                app.UseExceptionHandler("/error/500");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

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

            // UseAuthentication and UseAuthorization: after UseRouting and UseCors, but before UseEndpoints
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Calendar}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            app.UseMvc();
        }
    }
}
