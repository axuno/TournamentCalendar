using System.Reflection;
using System.Text;
using NUnit.Framework;
using TournamentCalendar.Collecting;

namespace TournamentCalendar.Tests;

[TestFixture]
public class CollectingTests
{
    private readonly string _testCollectorDirectory =
        Path.Combine(
            MiscHelpers.FindParentFolder("Data", TestContext.CurrentContext.TestDirectory),
            "Collector");

    [TestCaseSource(nameof(GetAllCollectorClassInstances))]
    public async Task Provider_GetHttpTourneyStartPage(CollectorBase collector)
    {
        // Ensure the start page for tournaments can be loaded
        var doc = await collector.GetDocumentAsync(collector.StartPath);
        var section = doc != null ? collector.GetTournamentSection(doc) : null;

        Assert.That(doc, Is.Not.Empty);
        Assert.That(section, Is.Not.Null);
    }

    [Test]
    public async Task Providers_GetHttpGetAllTourneys()
    {
        var tempFolder = Path.GetTempPath() + "Collect_" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(tempFolder);
        Console.WriteLine(tempFolder);

        Storage.StorageFolder = tempFolder;
        var currentTourneys = (await Collectors.CollectTourneys()).Tourneys;
        await Storage.SaveTourneysToFile(currentTourneys, DateTime.UtcNow, DateTime.UtcNow.Date.AddDays(-1), true, 10);
        
        Assert.That(Storage.GetFileNamesDescending().Count(), Is.EqualTo(1));

        // clean up
        Directory.Delete(tempFolder, true);
    }
    
    [TestCaseSource(nameof(GetAllCollectorExpectedResults))]
    public async Task Provider_ExtractAllInfos(CollectorBase collector, int numOfLinks)
    {
        collector.StartPath = $"{collector.GetType().Name}_Page1.html";
        collector.GetDocumentAsync = path =>
            File.ReadAllTextAsync(Path.Combine(_testCollectorDirectory, path.TrimStart('/')), Encoding.UTF8)!;

        var infos = await collector.GetAllTourneyInfos();
        var info = infos.FirstOrDefault();

        Assert.That(infos.Count, Is.EqualTo(numOfLinks));
        Assert.That(info?.Link, Is.Not.Empty);
        Assert.That(info?.Date, Is.Not.EqualTo(DateTime.MinValue));
        Assert.That(info?.Name, Is.Not.Empty);
        Assert.That(info?.PostalCode, Is.Not.Empty);
    }

    [TestCaseSource(nameof(GetAllCollectorClassInstances))]
    public void Provider_EmptyPageShouldThrow(CollectorBase collector)
    {
        collector.StartPath = $"{collector.GetType().Name}_Page1.html";
        collector.GetDocumentAsync = path => Task.FromResult(default(string?));

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await collector.GetAllTourneyInfos());
        Assert.That(exception != null && exception.Message.Contains("Page"));
    }

    [TestCaseSource(nameof(GetAllCollectorClassInstances))]
    public void Provider_MissingTournamentSectionShouldThrow(CollectorBase collector)
    {
        collector.StartPath = $"{collector.GetType().Name}_Page1.html";
        collector.GetDocumentAsync = path => Task.FromResult("<html></html>")!;

        var exception = Assert.ThrowsAsync<InvalidOperationException>(code: async () => await collector.GetAllTourneyInfos());
        Assert.That(exception != null && exception.Message.Contains("Tournament section not found"));
    }

    [Test]
    public void RemoveRedundantTourneyFilesFromStorage()
    {
        const int filesToKeep = 3;
        var tempFolder = Path.GetTempPath() + "Collect_" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(tempFolder);
        Console.WriteLine(tempFolder);

        Storage.StorageFolder = tempFolder;
        var tourneys = GetCollectedTourneys();
        var startDate = new DateTime(DateTime.UtcNow.Year, 6, 1);

        for (var i = 0; i < 10; i++)
        {
            Storage.SaveTourneysToFile(tourneys, startDate.AddDays(i), true);
        }

        // Act
        Storage.RemoveOldImportFiles(filesToKeep);

        var fileDates =
            Storage.ExtractDatesFromFileNames(Storage.GetFileNamesDescending());

        Assert.That(fileDates.Length, Is.EqualTo(filesToKeep));
        Assert.That(fileDates, Does.Contain(startDate.AddDays(7)));
        Assert.That(Storage.GetLastCollectionDate(fileDates, startDate.AddDays(11)), Is.EqualTo(startDate.AddDays(9)));

        // clean up
        Directory.Delete(tempFolder, true);
    }

    [Test]
    public void WriteAndReadTourneyFile()
    {
        var tempFolder = Path.GetTempPath() + "Collect_" + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(tempFolder);
        Console.WriteLine(tempFolder);

        Storage.StorageFolder = tempFolder;
        var tourneySaved = GetCollectedTourneys();
        var date = new DateTime(DateTime.UtcNow.Year, 6, 1);
        Storage.SaveTourneysToFile(tourneySaved, date, false);
        
        // Act
        var tourneyRead =
            Storage.ReadTourneysFromFile(Storage.GetFileNamesDescending().First());
        var (same, added, removed) = Collectors.CompareTourneysByUrl(tourneySaved.Tourneys, tourneyRead.Tourneys);

        Assert.That(tourneyRead.Tourneys.Count, Is.EqualTo(tourneySaved.Tourneys.Count));
        Assert.That(same.Count, Is.EqualTo(tourneySaved.Tourneys.Count));        

        // clean up
        Directory.Delete(tempFolder, true);
    }

    [Test]
    public void CompareTourneyCollectionsByUrl()
    {
        var tourneysOld = GetCollectedTourneys().Tourneys;
        tourneysOld.RemoveRange(1, 4);
        
        var tourneysLatest = GetCollectedTourneys().Tourneys;
        tourneysLatest.RemoveAt(tourneysLatest.Count - 1);

        var (same, added, removed) = Collectors.CompareTourneysByUrl(tourneysLatest, tourneysOld);

        Assert.That(same.Count, Is.EqualTo(1));
        Assert.That(added.Count, Is.EqualTo(4));
        Assert.That(removed.Count, Is.EqualTo(1));
    }

    public static CollectedTourneys GetCollectedTourneys()
    {
        var collectedDate = new DateTime(DateTime.Now.Year, 3, 1);
        var startDate = new DateTime(DateTime.Now.Year, 4, 1);

        var tourneys = new List<TourneyInfo> {
             new() { ProviderId = 1, Date = startDate.AddMonths(1), Name = "p1a", Link = "https://turnier-kalender.de/a", PostalCode = "10000", CollectedOn = collectedDate },
             new() { ProviderId = 1, Date = startDate.AddMonths(2), Name = "p1b", Link = "https://turnier-kalender.de/b", PostalCode = "10001", CollectedOn = collectedDate },
             new() { ProviderId = 1, Date = startDate.AddMonths(3), Name = "p1c", Link = "https://turnier-kalender.de/c", PostalCode = "10002", CollectedOn = collectedDate },
             new() { ProviderId = 2, Date = startDate.AddMonths(4), Name = "p2a", Link = "https://turnier-kalender.de/d", PostalCode = "10003", CollectedOn = collectedDate },
             new() { ProviderId = 2, Date = startDate.AddMonths(5), Name = "p2b", Link = "https://turnier-kalender.de/d", PostalCode = "10004", CollectedOn = collectedDate },
             new() { ProviderId = 2, Date = startDate.AddMonths(6), Name = "p2b", Link = "https://turnier-kalender.de/f", PostalCode = "10005", CollectedOn = collectedDate }
        };

        return new CollectedTourneys { Tourneys = tourneys };
    }
    
    public static IEnumerable<CollectorBase> GetAllCollectorClassInstances()
    {
        var collectorNames = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
            .Where(t => typeof(ICollector).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
            .Select(t => t.FullName!);

        foreach (var collectorName in collectorNames)
        {
            var collector = (CollectorBase) typeof(ICollector).Assembly.CreateInstance(collectorName, false, BindingFlags.CreateInstance, null,
                new object[1], null, null)!;
            yield return collector;
        }
    }

    public static IEnumerable<object[]> GetAllCollectorExpectedResults()
    {
        var numOfLinks = new Dictionary<string, int> { { nameof(CollectorA), 18 }, {nameof(CollectorB), 21} };
        
        foreach (var collector in GetAllCollectorClassInstances())
        {
            yield return new object[] { collector, numOfLinks[collector.GetType().Name] };
        }
    }
}
