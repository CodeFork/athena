﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Athena.Models;
using Microsoft.AspNetCore.Authorization;

namespace Athena.Controllers
{
    [Route("")]
    [AllowAnonymous]
    public class HomeController : AthenaControllerBase
    {
        [HttpGet("Home")]
        [HttpGet]
        public IActionResult Home()
        {
            return View();
        }

        [HttpGet("Add_Classes")]
        public IActionResult Add_Classes()
        {
            return View();
        }

        [HttpGet("Schedule_Page")]
        public IActionResult Schedule_Page()
        {
            return View();
        }

        [HttpGet("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
