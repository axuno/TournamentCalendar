using System.Reflection;
using System.Text;
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
    public async Task Provider_GetHttpTourneyStartPage(CollectorBase provider)
    {
        // Ensure the start page for tournaments can be loaded
        var doc = await provider.GetDocumentAsync(provider.StartPath);
        var section = doc != null ? provider.GetTournamentSection(doc) : null;

        Assert.That(doc, Is.Not.Empty);
        Assert.That(section, Is.Not.Null);
    }

    [Test]
    public async Task Providers_GetHttpGetAllTourneys()
    {
        Storage.StorageFolder = @"C:\Temp";
        await Storage.CollectAndSaveTourneys(true);
    }
    
    [TestCaseSource(nameof(GetAllProviderExpectedResults))]
    public async Task Provider_ExtractAllInfos(CollectorBase provider, int numOfLinks)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path =>
            File.ReadAllTextAsync(Path.Combine(_testDataDirectory, path.TrimStart('/')), Encoding.UTF8)!;

        var infos = await provider.GetAllTourneyInfos();
        var info = infos.FirstOrDefault();

        Assert.That(infos.Count, Is.EqualTo(numOfLinks));
        Assert.That(info?.Link, Is.Not.Empty);
        Assert.That(info?.Date, Is.Not.EqualTo(DateTime.MinValue));
        Assert.That(info?.Name, Is.Not.Empty);
        Assert.That(info?.PostalCode, Is.Not.Empty);
    }

    [TestCaseSource(nameof(GetAllProviderClassInstances))]
    public void Provider_EmptyPageShouldThrow(CollectorBase provider)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path => Task.FromResult(default(string?));

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await provider.GetAllTourneyInfos());
        Assert.That(exception != null && exception.Message.Contains("Page"));
    }

    [TestCaseSource(nameof(GetAllProviderClassInstances))]
    public void Provider_MissingTournamentSectionShouldThrow(CollectorBase provider)
    {
        provider.StartPath = $"{provider.GetType().Name}_Page1.html";
        provider.GetDocumentAsync = path => Task.FromResult("<html></html>")!;

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await provider.GetAllTourneyInfos());
        Assert.That(exception != null && exception.Message.Contains("Tournament section not found"));
    }

    public static IEnumerable<CollectorBase> GetAllProviderClassInstances()
    {
        var providerNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => typeof(CollectorBase).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(t => t.FullName!);

        foreach (var providerName in providerNames)
        {
            var provider = (CollectorBase) typeof(ICollector).Assembly.CreateInstance(providerName, false, BindingFlags.CreateInstance, null,
                new object[1], null, null)!;
            yield return provider;
        }
    }

    public static IEnumerable<object[]> GetAllProviderExpectedResults()
    {
        var numOfLinks = new Dictionary<string, int> { { nameof(CollectorA), 18 }, {nameof(CollectorB), 21} };
        
        foreach (var provider in GetAllProviderClassInstances())
        {
            yield return new object[] { provider, numOfLinks[provider.GetType().Name] };
        }
    }
}
