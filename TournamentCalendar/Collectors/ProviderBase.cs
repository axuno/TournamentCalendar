using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace TournamentCalendar.Collectors;

public abstract class ProviderBase : IProvider
{
    protected ProviderBase(ILogger? logger)
    {
        // initial value must be set in the constructor
        GetDocumentAsync = GetHttpDocumentAsync;
        if(logger != null) Logger = logger;
    }

    protected ILogger Logger { get; set; } = NullLogger.Instance;

    public virtual int ProviderId { get; init; } = 0;
    public virtual string ProviderName { get; init; } = string.Empty;
    public virtual Uri BaseAddress { get; set; } = new("https://localhost");
    public virtual string StartPath { get; set; } = string.Empty;

    protected async Task<IHtmlDocument> ToHtmlDocument(string page)
    {
        var parser = new HtmlParser();
        return await parser.ParseDocumentAsync(page);
    }

    protected virtual HttpClient GetConfiguredHttpClient()
    {
        var httpClient = new HttpClient {
            BaseAddress = BaseAddress,
            Timeout = TimeSpan.FromSeconds(15)
        };
        httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63");

        return httpClient;
    }

    internal Func<string, Task<string?>> GetDocumentAsync;

    /// <summary>
    /// Loads the page specified as <paramref name="path"/> from the provider website.
    /// This is the default method use for <see cref="GetDocumentAsync"/>.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>The document as <see langword="string"/></returns>
    protected virtual async Task<string?> GetHttpDocumentAsync(string path)
    {
        if (string.IsNullOrEmpty(BaseAddress.AbsolutePath) || string.IsNullOrEmpty(path)) return null;

        using var httpClient = GetConfiguredHttpClient();
        return await httpClient.GetStringAsync(path);
    }

    protected internal abstract Task<IElement?> GetTournamentSection(string page);
    protected abstract string? GetPathToNextPage(IElement parentElement);
    protected abstract List<string> ExtractLinks(IElement parentElement);
    public abstract Task<List<string>> GetAllTourneyLinks();
}
