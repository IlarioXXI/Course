using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DI_Service.Models;
using DI_Service.Service;
using System.Text;

namespace DI_Service.Controllers;

public class HomeController : Controller
{
    private readonly IScopedNumberService _scoped1;
    private readonly IScopedNumberService _scoped2;

    private readonly ISingletonNumberService _singleton1;
    private readonly ISingletonNumberService _singleton2;

    private readonly ITransientNumberService _transient1;
    private readonly ITransientNumberService _transient2;

    public HomeController(IScopedNumberService scoped1,
        IScopedNumberService scoped2,
        ISingletonNumberService singleton1,
        ISingletonNumberService singleton2,
        ITransientNumberService transient1,
        ITransientNumberService trasient2)
    {
        _scoped1 = scoped1;
        _scoped2 = scoped2;

        _singleton1 = singleton1;
        _singleton2 = singleton2;

        _transient1 = transient1;
        _transient2 = trasient2;


    }

    public IActionResult Index()
    {
        StringBuilder message = new StringBuilder();
        message.Append($"Transient 1 : {_transient1.GetNumber()}\n");
        message.Append($"Transient 2 : {_transient2.GetNumber()}\n\n\n\n");
        message.Append($"Scoped 1 : {_scoped1.GetNumber()}\n");
        message.Append($"Scoped 2 : {_scoped2.GetNumber()}\n\n\n\n");
        message.Append($"Singleton 1 : {_singleton1.GetNumber()}\n");
        message.Append($"Singleton 2 : {_singleton2.GetNumber()}\n\n\n\n");
        return Ok(message.ToString());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
