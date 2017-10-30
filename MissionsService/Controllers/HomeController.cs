using MissionsService.Models;
using MissionsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MissionsService.Controllers
{
    public class HomeController : Controller
    {
        private MissionsContext db = new MissionsContext();

        public bool TestConnection()
        {
            return true;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Список задач";

            return View();
        }
    }
}
