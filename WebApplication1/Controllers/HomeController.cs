using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ServiceReference1;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int page=1,string sortBy="Ad Id")
        {
            AdModel ads = Models.Repo.GetAds(page,sortBy);
            return View(ads);
        }

        public ActionResult Grid(int page = 1, string sortBy = "Ad Id")
        {
            AdModel ads = Models.Repo.GetAds(page, sortBy);
            return View(ads);
        }
    }
}