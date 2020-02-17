using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PewPewSignalR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PewPewSignalR.Controllers
{
	public class AuthenticationController : Controller
	{
        

        [HttpGet]
        public IActionResult Index()
        {
            return View();
            //return Content("This will be the Authentication part of it all");
        }


        [HttpPost]
        public IActionResult Index(User user)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
    }
}
