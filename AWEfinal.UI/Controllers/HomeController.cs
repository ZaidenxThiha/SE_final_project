using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AWEfinal.UI.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IWebHostEnvironment environment, ILogger<HomeController> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var spaIndexPath = Path.Combine(_environment.WebRootPath, "app", "index.html");
        if (!System.IO.File.Exists(spaIndexPath))
        {
            _logger.LogWarning("SPA build not found at {Path}. Run `npm install && npm run build` inside ClientApp.", spaIndexPath);
            return Content("Client build not found. Run `npm install && npm run build` inside AWEfinal.UI/ClientApp.", "text/plain");
        }

        return PhysicalFile(spaIndexPath, "text/html");
    }
}
