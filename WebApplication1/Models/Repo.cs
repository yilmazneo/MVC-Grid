using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.ServiceReference1;

namespace WebApplication1.Models
{
    public class AdModel
    {
        public List<Ad> ads { get; set; }
        public int pageCount { get; set; }
        public int currentPage { get; set; }
        public List<string> columnDisplayNames { get; set; }
        public bool sortingEnabled { get; set; }
        public bool pagingEnabled { get; set; }
    }

    public class Brand
    {
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public decimal PageCoverageSum { get; set; }
    }

    public class BrandModel
    {
        public List<Brand> brands { get; set; }
        public List<string> columnDisplayNames { get; set; }
    }

    public static class Repo
    {
        public static AdModel GetAllAds(int page,string sortBy)
        {
            int pageSize = 10;
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011,1,1);
            DateTime end = new DateTime(2011,5,1);
            AdModel model = new AdModel();
            var ads = client.GetAdDataByDateRange(start, end).ToList<Ad>();
            model.pageCount = (ads.Count/ pageSize) + ((ads.Count% pageSize)>0?1:0);
            model.currentPage = page;
            model.sortingEnabled = true;
            model.pagingEnabled = true;
            model.ads = ads.OrderBy(a => GetSortColumn(sortBy,a)).Skip( (page-1)* pageSize).Take(pageSize).ToList<Ad>();
            model.columnDisplayNames = new List<string>() { "Ad Id","Brand Id","Brand Name","Number of Pages","Position" };            
            return model;
        }

        public static AdModel GetCoverAdsWithAtLeastHalfCoverage(int page, string sortBy)
        {
            int pageSize = 10;
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011, 1, 1);
            DateTime end = new DateTime(2011, 5, 1);
            AdModel model = new AdModel();
            var ads = client.GetAdDataByDateRange(start, end).ToList<Ad>();            
            model.currentPage = page;
            model.sortingEnabled = true;
            model.pagingEnabled = true;
            decimal minimumCoverage = 0.5M;
            var filteredAds = ads.Where(a => a.Position.Equals("Cover") && a.NumPages >= minimumCoverage);
            model.ads = filteredAds.OrderBy(a => GetSortColumn(sortBy, a)).Skip((page - 1) * pageSize).Take(pageSize).ToList<Ad>();
            model.pageCount = (filteredAds.Count() / pageSize) + ((filteredAds.Count() % pageSize) > 0 ? 1 : 0);
            model.columnDisplayNames = new List<string>() { "Ad Id", "Brand Id", "Brand Name", "Number of Pages", "Position" };
            return model;
        }

        public static AdModel GetTop5MaxCoverageAdsByBrand()
        {            
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011, 1, 1);
            DateTime end = new DateTime(2011, 5, 1);
            AdModel model = new AdModel();
            var allAds = client.GetAdDataByDateRange(start, end).ToList<Ad>();
            model.sortingEnabled = false;
            model.pagingEnabled = false;

            var maxCoveragesByBrand = (from ad in allAds
                                      group ad by ad.Brand.BrandId into brandAds
                                      select new 
                                      {                                          
                                          maxCoverageAd = brandAds.First( ba => ba.NumPages == brandAds.Max(a => a.NumPages))
                                      }).OrderByDescending(m => m.maxCoverageAd.NumPages).ThenBy(m => m.maxCoverageAd.Brand.BrandName).Take(5);

            var dataSource = (from a in maxCoveragesByBrand
                    select new Ad
                    {
                        AdId= a.maxCoverageAd.AdId,
                        Brand = a.maxCoverageAd.Brand,
                        NumPages = a.maxCoverageAd.NumPages,
                        Position = a.maxCoverageAd.Position
                    }).ToList<Ad>();

            model.ads = dataSource;
            model.pageCount = 1;
            model.columnDisplayNames = new List<string>() { "Ad Id", "Brand Id", "Brand Name", "Number of Pages", "Position" };
            return model;
        }

        public static BrandModel GetTop5BrandsWithMaxSumPageCoverage()
        {            
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011, 1, 1);
            DateTime end = new DateTime(2011, 5, 1);
            BrandModel model = new BrandModel();
            var allAds = client.GetAdDataByDateRange(start, end).ToList<Ad>();            

            var dataSource = (from ad in allAds
                                       group ad by ad.Brand.BrandId into brandAds
                                       select new Brand
                                       {
                                           BrandId = brandAds.Key,
                                           BrandName = brandAds.First().Brand.BrandName,
                                           PageCoverageSum = brandAds.Sum(b => b.NumPages)
                                       }).OrderByDescending(m => m.PageCoverageSum).ThenBy(m => m.BrandName).Take(5);


            model.brands = dataSource.ToList<Brand>();
            model.columnDisplayNames = new List<string>() { "Brand Id", "Brand Name", "Total Number of Pages" };
            return model;
        }

        public static object GetSortColumn(string sortBy,object o)
        {
            switch (sortBy)
            {
                case "Ad Id":
                    return GetPropertyValue(o,"AdId");
                case "Brand Id":
                    return GetPropertyValue(GetPropertyValue(o, "Brand"), "BrandId");
                case "Brand Name":
                    return GetPropertyValue(GetPropertyValue(o, "Brand"), "BrandName");
                case "Number of Pages":
                    return GetPropertyValue(o, "NumPages");
                case "Position":
                    return GetPropertyValue(o,"Position");
                default:
                    return null;
            }
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            var objType = obj.GetType();
            var prop = objType.GetProperty(propertyName);

            return prop.GetValue(obj, null);
        }

        public static AdModel GetModel(int modelNumber,int page,string sortBy)
        {
            switch (modelNumber)
            {
                case 1:
                    return GetAllAds(page, sortBy);
                case 2:
                    return GetCoverAdsWithAtLeastHalfCoverage(page, sortBy);
                default:
                    return null;
            }
        }

    }
}