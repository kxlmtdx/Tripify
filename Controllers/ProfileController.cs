﻿using Microsoft.AspNetCore.Mvc;

namespace TourFlow.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
