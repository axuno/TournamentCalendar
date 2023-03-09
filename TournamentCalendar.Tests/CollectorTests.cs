using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TournamentCalendar.Collectors;

namespace TournamentCalendar.Tests;

[TestFixture]
public class CollectorTests
{
    private readonly string _testDataDirectory =
        Path.Combine(
            MiscHelpers.FindParentFolder("Data", TestContext.CurrentContext.TestDirectory),
            "Collector");

    [TestCaseSource(nameof(GetAllProviderClassInstances))]
    public async Task Provider_GetHttpTourneyStartPage(ProviderBase provider)
    {
        // Ensure the start page for tournaments can be loaded
        var doc = await provider.GetDocumentAsync(provider.StartPath);
        var section = doc != null ? provider.GetTournamentSection(doc) : null;

        Assert.That(doc, Is.Not.Empty);
        Assert.That(section, Is.Not.Null);
    }
    
    [TestCaseSource(nameof(GetAllProviderExpectedResults))]
    public async Task Provider_ExtractAllLinks(ProviderBase provider, int numOfLinks)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path =>
            File.ReadAllTextAsync(Path.Combine(_testDataDirectory, path.TrimStart('/')), Encoding.UTF8)!;

        var links = await provider.GetAllTourneyLinks();

        Assert.That(links.Count, Is.EqualTo(numOfLinks));
    }

    [TestCaseSource(nameof(GetAllProviderClassInstances))]
    public void Provider_EmptyPageShouldThrow(ProviderBase provider)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path => Task.FromResult(default(string?));

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await provider.GetAllTourneyLinks());
        Assert.That(exception != null && exception.Message.Contains("Page"));
    }

    [TestCaseSource(nameof(GetAllProviderClassInstances))]
    public void Provider_MissingTournamentSectionShouldThrow(ProviderBase provider)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path => Task.FromResult("<html></html>")!;

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await provider.GetAllTourneyLinks());
        Assert.That(exception != null && exception.Message.Contains("Tournament section not found"));
    }

    public static IEnumerable<ProviderBase> GetAllProviderClassInstances()
    {
        var providerNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => typeof(ProviderBase).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(t => t.FullName!);

        foreach (var providerName in providerNames)
        {
            var provider = (ProviderBase) typeof(IProvider).Assembly.CreateInstance(providerName, false, BindingFlags.CreateInstance, null,
                new object[1], null, null)!;
            yield return provider;
        }
    }

    public static IEnumerable<object[]> GetAllProviderExpectedResults()
    {
        var numOfLinks = new Dictionary<string, int> { { "ProviderA", 18 }, {"ProviderB", 21} };
        
        foreach (var provider in GetAllProviderClassInstances())
        {
            yield return new object[] { provider, numOfLinks[provider.GetType().Name] };
        }
    }
}
