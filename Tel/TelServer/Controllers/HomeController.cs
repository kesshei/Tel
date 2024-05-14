using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace TelServer.Controllers;

[Route("[controller]/[action]")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
