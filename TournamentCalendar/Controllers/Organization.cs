using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using TournamentCalendar.Views;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TournamentCalendar.Controllers;

[Route(nameof(Organization))]
public class Organization : ControllerBase
{
    private readonly string _xmlFileList = "Download-Files/TournamentDownloadfiles.xml";

    public class DownloadFile
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public bool IsProtected { get; set; }
    }

    public Organization(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(hostingEnvironment, configuration)
    {}

    [HttpGet("")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Organization.Apps), nameof(Controllers.Organization));
    }

    [HttpGet(nameof(Apps))]
    public IActionResult Apps()
    {
        ViewBag.TitleTagText = "Software zur Auswertung von Volleyballturnieren";
        return View(ViewName.Organization.Apps, GetList());
    }

    [HttpGet("download/{file}")]
    public IActionResult Download(string file)
    {
        if (GetFile(file, out var fileName) && System.IO.File.Exists(Path.Combine(HostingEnvironment.WebRootPath, fileName!.Name)))
        {
            var stream = new FileStream(Path.Combine(HostingEnvironment.WebRootPath, fileName.Name), FileMode.Open);
            return new FileStreamResult(stream, fileName.ContentType);
        }

        return NotFound();
    }


    [NonAction]
    private bool GetFile(string file, out DownloadFile? download)
    {
        try
        {
            download = GetList().FirstOrDefault(f => f.Id == file);
            return download != null;
        }
        catch (Exception)
        {
            download = null;
            return false;
        }
    }

    [NonAction]
    private IEnumerable<DownloadFile> GetList()
    {
        var xmlDoc = XDocument.Load(Path.Combine(HostingEnvironment.WebRootPath, _xmlFileList));
        return (from download in xmlDoc.Descendants("download")
            select
                new DownloadFile
                {
                    Id = download.Attribute("id")!.Value,
                    Name = download!.Element("filename")!.Value,
                    Size = new FileInfo(Path.Combine(HostingEnvironment.WebRootPath, download.Element("filename")!.Value)).Length,
                    ContentType = download.Element("contenttype")!.Value,
                    IsProtected = bool.Parse(download.Element("isprotected")!.Value)
                }).ToList();
    }
}
