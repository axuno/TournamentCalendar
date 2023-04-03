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

[Route("orga")]
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
    [HttpGet("Index")]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Organization.Evaluation), nameof(Controllers.Organization));
    }

    [HttpGet("auswertung")]
    public IActionResult Evaluation()
    {
        ViewBag.TitleTagText = "Software zur Auswertung von Volleyballturnieren";
        return View(ViewName.Orga.Auswertung, GetList());
    }

    [HttpGet("download/{id?}")]
    public IActionResult Download(string id)
    {
        if (GetFile(id, out var file) && System.IO.File.Exists(Path.Combine(HostingEnvironment.WebRootPath, file!.Name)))
        {
            var stream = new FileStream(Path.Combine(HostingEnvironment.WebRootPath, file.Name), FileMode.Open);
            return new FileStreamResult(stream, file.ContentType);
        }

        return NotFound();
    }


    [NonAction]
    private bool GetFile(string id, out DownloadFile? download)
    {
        try
        {
            download = GetList().FirstOrDefault(f => f.Id == id);
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
