using System;
using MailMergeLib;
using MailMergeLib.MessageStore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TournamentCalendar
{
    public interface IMailMergeService
    {
        IMessageStore MessageStore { get; }
        MailMergeSender Sender { get; }
        MailMergeMessage CreateDefaultMessage();
    }

    public class MailMergeServiceConfig
    {
        public Settings Settings { get; set; } = null!;
        public IMessageStore MessageStore { get; set; } = null!;
    }

    public class MailMergeService : IMailMergeService
    {
        public MailMergeService(IOptions<MailMergeServiceConfig> serviceConfig)
        {
            Settings = serviceConfig.Value.Settings;
            MessageStore = serviceConfig.Value.MessageStore;
            Sender = new MailMergeSender {Config = serviceConfig.Value.Settings.SenderConfig};
        }
        public Settings Settings { get; }
        public IMessageStore MessageStore { get; }
        public MailMergeSender Sender { get; }
        public MailMergeMessage CreateDefaultMessage()
        {
            return new MailMergeMessage {Config = Settings.MessageConfig, PlainText = string.Empty, HtmlText = string.Empty, Subject = string.Empty};
        }
    }

    /// <summary>
    /// Extension methods for adding MailMerge services to the DI container.
    /// </summary>
    public static class MailMergeServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a transient <see cref="IMailMergeService"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> the service will be added to.</param>
        /// <param name="config">The <see cref="MailMergeServiceConfig"/> which will configure the <see cref="IMailMergeService"/>.</param>
        /// <returns>The <see cref="T:IServiceCollection" /> so that additional calls can be chained.</returns>
        public static IServiceCollection AddMailMergeService(this IServiceCollection services, Action<MailMergeServiceConfig> config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            services.Configure<MailMergeServiceConfig>(config);
            services.AddTransient<IMailMergeService, MailMergeService>();
            return services;
        }
    }
}
