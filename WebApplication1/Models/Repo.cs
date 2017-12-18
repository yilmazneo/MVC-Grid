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
        private static readonly int PageSize = 10;

        /// <summary>
        /// Returns all ad data from the server
        /// </summary>
        /// <returns></returns>
        private static List<Ad> GetAdsFromServer()
        {
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011, 1, 1);
            DateTime end = new DateTime(2011, 5, 1);

            return client.GetAdDataByDateRange(start, end).ToList<Ad>();
        }

        /// <summary>
        /// Returns the number of pages that is required to show given total number of records
        /// </summary>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        private static int GetNumberOfPages(int recordCount)
        {
            return (recordCount / PageSize) + ((recordCount % PageSize) > 0 ? 1 : 0);
        }

        /// <summary>
        /// Returns the model for View1
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static AdModel GetModelForAllAds(int page,string sortBy)
        {
            var ads = GetAdsFromServer();
            var dataSource = ads.OrderBy(a => GetSortColumn(sortBy,a)).Skip( (page-1)* PageSize).Take(PageSize).ToList<Ad>();
            var columns = new List<string>() { "Ad Id","Brand Id","Brand Name","Number of Pages","Position" };
            return new AdModel()
            {
                pageCount = GetNumberOfPages(ads.Count),
                currentPage = page,
                sortingEnabled = true,
                pagingEnabled = true,
                ads = dataSource,
                columnDisplayNames = columns
            };
        }

        /// <summary>
        /// Returns the model for View2
        /// </summary>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static AdModel GetModelForCoverAdsWithAtLeastHalfCoverage(int page, string sortBy)
        {                             
            decimal minimumCoverage = 0.5M;
            var ads = GetAdsFromServer();
            var filteredAds = ads.Where(a => a.Position.Equals("Cover") && a.NumPages >= minimumCoverage);
            var dataSource = filteredAds.OrderBy(a => GetSortColumn(sortBy, a)).Skip((page - 1) * PageSize).Take(PageSize).ToList<Ad>();            
            var columns = new List<string>() { "Ad Id", "Brand Id", "Brand Name", "Number of Pages", "Position" };
            return new AdModel()
            {
                pageCount = GetNumberOfPages(filteredAds.Count()),
                currentPage = page,
                sortingEnabled = true,
                pagingEnabled = true,
                ads = dataSource,
                columnDisplayNames = columns
            };
        }

        /// <summary>
        /// Returns the model for View3
        /// </summary>
        /// <returns></returns>
        public static AdModel GetModelForTop5MaxCoverageAdsByBrand()
        {
            var allAds = GetAdsFromServer();

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


            var columns = new List<string>() { "Ad Id", "Brand Id", "Brand Name", "Number of Pages", "Position" };

            return new AdModel()
            {
                pageCount = 1,
                currentPage = 1,
                sortingEnabled = false,
                pagingEnabled = false,
                ads = dataSource,
                columnDisplayNames = columns
            };
        }

        /// <summary>
        /// Returns the model for View4
        /// </summary>
        /// <returns></returns>
        public static BrandModel GetModelForTop5BrandsWithMaxSumPageCoverage()
        {
            var allAds = GetAdsFromServer();            

            var dataSource = (from ad in allAds
                                       group ad by ad.Brand.BrandId into brandAds
                                       select new Brand
                                       {
                                           BrandId = brandAds.Key,
                                           BrandName = brandAds.First().Brand.BrandName,
                                           PageCoverageSum = brandAds.Sum(b => b.NumPages)
                                       }).OrderByDescending(m => m.PageCoverageSum).ThenBy(m => m.BrandName).Take(5);


            var columns = new List<string>() { "Brand Id", "Brand Name", "Total Number of Pages" };

            return new BrandModel()
            {
                brands = dataSource.ToList<Brand>(),
                columnDisplayNames = columns
            };
        }

        /// <summary>
        /// This method is to turn column display value into its matching property value to be used in linq query's orderby        
        /// </summary>
        /// <param name="sortBy"></param>
        /// <param name="o"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns property value of a given property on the specified object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj, string propertyName)
        {
            var objType = obj.GetType();
            var prop = objType.GetProperty(propertyName);

            return prop.GetValue(obj, null);
        }


        /// <summary>
        /// Model Factory Method
        /// </summary>
        /// <param name="modelNumber"></param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public static AdModel GetModel(int modelNumber,int page,string sortBy)
        {
            switch (modelNumber)
            {
                case 1:
                    return GetModelForAllAds(page, sortBy);
                case 2:
                    return GetModelForCoverAdsWithAtLeastHalfCoverage(page, sortBy);
                default:
                    return null;
            }
        }

    }
}