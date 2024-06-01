namespace TournamentCalendar.Controllers;

public class ControllerBase : Controller
{
    public IWebHostEnvironment HostingEnvironment { get; set; } = null!;

    public IConfiguration Configuration { get; set; } = null!;

    public ControllerBase()
    { }

    public ControllerBase(IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : this()
    {
        HostingEnvironment = hostingEnvironment;
        Configuration = configuration;
    }
}
