using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class SiteDb
    {
        public const int CountPerPage = 10;

        public static Json GetSite(string deviceId)
        {
            try
            {
                using (var db = new DESEntities())
                {

                    var site = db.Sites.Where(r => r.DeviceId == deviceId).FirstOrDefault();
                    if (site == null) return null;

                    var device = db.Devices.Where(r => r.DeviceId == site.DeviceId).FirstOrDefault();
                    if (device == null) return null;

                    return new Json
                    {
                        UserName = site.User.UserName,
                        Site = site,
                        ACMVersion = device.ACMVersion,
                        IsConnect = device.IsConnect,
                        Version = DeviceDb.GetVersion(device.ACMVersion),
                    };
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public static Json GetSite(int siteId)
        {
            try
            {
                using (var db = new DESEntities())
                {

                    var site = db.Sites.Where(r => r.Id == siteId).FirstOrDefault();
                    if (site == null) return null;

                    var device = db.Devices.Where(r => r.DeviceId == site.DeviceId).FirstOrDefault();
                    return new Json
                    {
                        UserName = site.User.UserName,
                        Site = site,
                        ACMVersion = (device == null) ? ("") : (device.ACMVersion),
                        IsConnect = (device == null) ? (false) : (device.IsConnect),
                        Version = (device == null) ? (0) : (DeviceDb.GetVersion(device.ACMVersion)),
                    };
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public static List<Site> GetAllSites()
        {
            try
            {
                using (var db = new DESEntities())
                {

                    return db.Sites.ToList();
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public IEnumerable<Json> GetSites(SiteQueryModel query)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    List<Json> listSite = new List<Json>();
                    IEnumerable<User> users = db.Users.ToArray();
                    if (query.UserModel.IsAdmin){
                        if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue))
                        {
                            SiteQueryType siteQueryType = (SiteQueryType)Enum.Parse(typeof(SiteQueryType), query.QueryKey);
                            if (siteQueryType == SiteQueryType.UserName)
                            {
                                users = users.Where(r => r.UserName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            }
                        }
                    }else users = users.Where(r => r.Id == query.UserModel.User.Id).ToArray();

                    foreach (var user in users){
                        var sites = user.Sites.ToArray();
                        if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue)){
                            SiteQueryType siteQueryType = (SiteQueryType)Enum.Parse(typeof(SiteQueryType), query.QueryKey);
                            if (siteQueryType == SiteQueryType.SiteName) sites = sites.Where(r => r.SiteName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            else if (siteQueryType == SiteQueryType.DeviceId) sites = sites.Where(r => r.DeviceId.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            else if (siteQueryType == SiteQueryType.CustomerName) sites = sites.Where(r => r.CustomerName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            else if (siteQueryType == SiteQueryType.CustomerAddress) sites = sites.Where(r => r.CustomerAddress.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                            else if (siteQueryType == SiteQueryType.CustomerPhone) sites = sites.Where(r => r.CustomerPhone.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        }

                        foreach (Site site in sites){
                            var device = db.Devices.Where(r => r.DeviceId == site.DeviceId).FirstOrDefault();
                            if (device == null){
                                listSite.Add(new Json
                                {
                                    UserName = user.UserName,
                                    Site = site,
                                    ACMVersion = "",
                                    IsConnect = false,
                                    Version = 0,
                                });
                            }else{
                                listSite.Add(new Json
                                {
                                    UserName = user.UserName,
                                    Site = site,
                                    ACMVersion = device.ACMVersion,
                                    IsConnect = device.IsConnect,
                                    Version = DeviceDb.GetVersion(device.ACMVersion),
                                });
                            }
                        }
                    }

                    return listSite;
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public ResultModel CreateSite(Site site)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Site s = db.Sites.Where(r => (r.SiteName == site.SiteName && r.UserId == site.UserId)).FirstOrDefault();
                    if (s != null) return new ResultModel { Status = false, Result = "The site name already exists." };

                    s = db.Sites.Where(r => (r.DeviceId == site.DeviceId)).FirstOrDefault();
                    if (s != null) return new ResultModel { Status = false, Result = "The device id already exists." };

                    site = db.Sites.Add(site);
                    db.SaveChanges();

                    return new ResultModel { Status = true, Result = site };
                }
            }
            catch (Exception ex) 
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public ResultModel DeleteSite(Site site)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Site s = db.Sites.Where(r => r.Id == site.Id).FirstOrDefault();
                    if (s == null) return new ResultModel { Status = false, Result = "The site doesn't exist." };

                    db.Sites.Remove(s);
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public ResultModel UpdateSite(Site site)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    Site s = db.Sites.Where(r => r.Id == site.Id).FirstOrDefault();
                    if (s == null) return new ResultModel { Status = false, Result = "The site doesn't exist." };

                    if(s.DeviceId != site.DeviceId)
                    {
                        if (db.Sites.Where(r => r.DeviceId == site.DeviceId).FirstOrDefault() != null)
                        {
                            return new ResultModel { Status = false, Result = "The device id already exists." };
                        }
                    }

                    if (site.DeviceId != null) s.DeviceId = site.DeviceId;
                    if (site.CustomerName != null) s.CustomerName = site.CustomerName;
                    if (site.CustomerAddress != null) s.CustomerAddress = site.CustomerAddress;
                    if (site.CustomerPhone != null) s.CustomerPhone = site.CustomerPhone;
                    if (site.Address2 != null) s.Address2 = site.Address2;
                    if (site.Area != null) s.Area = site.Area;
                    if (site.Town != null) s.Town = site.Town;
                    if (site.City != null) s.City = site.City;
                    if (site.Country != null) s.Country = site.Country;
                    s.DateUpdated = DateTime.Now;

                    db.Entry(s).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }
    }
}
