using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using RSSWebApp.Models;
using static Azure.Core.HttpHeader;

namespace RSSWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    private List<Type> GetTypes()
    {
        Type pType = typeof(Ajans);    //get derived class of parent class
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type[] types = assembly.GetTypes();

        List<Type> children = types
            .Where(t => t.IsClass && t.BaseType == pType)
            .ToList();

        return children;
    }

    public IActionResult Index()
    {
        List<Type> types = GetTypes();

        List<string> children = types
            .Select(t => t.Name)
            .ToList();

        ViewBag.Ajanslar = children; //Model yapabilirsin
        return View();
    }

    [HttpPost]
    public ActionResult GetNews(string ajans)
    {
        List<Type> types = GetTypes();

        Type ajansType = types
            .Where(t => t.Name == ajans)
            .FirstOrDefault();

        if (ajansType != null)
        {
            Ajans instance = (Ajans)Activator.CreateInstance(ajansType);

            List<News> liste = instance.GetNews();

            ViewBag.Haberler = liste;
        }
        

        return View("Views/Home/News.cshtml");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

