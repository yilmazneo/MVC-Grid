using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ServiceReference1;

namespace WebApplication1.Controllers
{
    public class AdsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(int page = 1, string sortBy = "Brand Name",int modelNumber = 1)
        {
            AdModel ads = Models.Repo.GetModel(modelNumber,page,sortBy);
            return View(ads);
        }

        public ActionResult View1(int page = 1, string sortBy = "Brand Name")
        {
            AdModel ads = Models.Repo.GetModelForAllAds(page, sortBy);
            return View(ads);
        }

        public ActionResult View2(int page = 1, string sortBy = "Brand Name")
        {
            AdModel ads = Models.Repo.GetModelForCoverAdsWithAtLeastHalfCoverage(page, sortBy);
            return View(ads);
        }

        public ActionResult View3()
        {
            AdModel ads = Models.Repo.GetModelForTop5MaxCoverageAdsByBrand();
            return View(ads);
        }

        public ActionResult View4()
        {
            BrandModel ads = Models.Repo.GetModelForTop5BrandsWithMaxSumPageCoverage();
            return View(ads);
        }

    }
}