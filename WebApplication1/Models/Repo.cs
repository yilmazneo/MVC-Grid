using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.ServiceReference1;

namespace WebApplication1.Models
{
    public struct AdModel
    {
        public List<Ad> ads { get; set; }
        public int pageCount { get; set; }
        public int currentPage { get; set; }
        public List<string> columnDisplayNames { get; set; }
    }

    public static class Repo
    {
        public static AdModel GetAds(int page,string sortBy)
        {
            int pageSize = 10;
            AdDataServiceClient client = new AdDataServiceClient();
            DateTime start = new DateTime(2011,1,1);
            DateTime end = new DateTime(2011,5,1);
            AdModel model = new AdModel();
            var ads = client.GetAdDataByDateRange(start, end).ToList<Ad>();
            model.pageCount = (ads.Count/ pageSize) + ((ads.Count% pageSize)>0?1:0);
            model.currentPage = page;            
                        
            model.ads = ads.OrderBy(a => GetSortColumn(sortBy,a)).Skip( (page-1)* pageSize).Take(pageSize).ToList<Ad>();
            model.columnDisplayNames = new List<string>() { "Ad Id","Brand Id","Brand Name","Number of Pages","Position" };            
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

    }
}