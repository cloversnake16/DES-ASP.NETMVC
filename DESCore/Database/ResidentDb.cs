using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Database
{
    public class ResidentDb
    {
        public IEnumerable<ResidentModel> LoadResidents(int siteId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var residents = db.Residents.Where(r => r.SiteId == siteId).ToArray();
                    if (residents == null) return null;

                    List<ResidentModel> listResident = new List<ResidentModel>();
                    foreach (var resident in residents)
                    {
                        string siteName = null;
                        var site = db.Sites.Where(r => r.Id == siteId).FirstOrDefault();
                        if (site != null) siteName = site.SiteName;
                        listResident.Add(new ResidentModel { Resident = resident, SiteName = siteName });
                    }
                    return listResident;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public ResultModel CreateResident(Resident model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var resident = db.Residents.Where(r => r.ResidentName == model.ResidentName).FirstOrDefault();
                    if (resident != null) return new ResultModel { Status = false, Result = "The resident name already exists." };

                    resident = new Resident
                    {
                        ResidentName = model.ResidentName,
                        SiteId = model.SiteId,
                        FlatNumber = model.FlatNumber,
                        HomeTel = model.HomeTel,
                        MobileTel = model.MobileTel,
                        Email = model.Email,
                        TagIndex = model.TagIndex,
                        DateUpdated = DateTime.Now,
                    };
                    db.Residents.Add(resident);
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public ResultModel UpdateResident(Resident model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var resident = db.Residents.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (resident == null) return new ResultModel { Status = false, Result = "No the resident." };

                    resident.ResidentName = model.ResidentName;
                    resident.SiteId = model.SiteId;
                    resident.FlatNumber = model.FlatNumber;
                    resident.HomeTel = model.HomeTel;
                    resident.MobileTel = model.MobileTel;
                    resident.Email = model.Email;
                    resident.TagIndex = model.TagIndex;
                    resident.DateUpdated = DateTime.Now;

                    db.Entry(resident).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message };
            }
        }

        public ResultModel DeleteResident(int residentId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var resident = db.Residents.Where(r => r.Id == residentId).FirstOrDefault();
                    if (resident == null) return new ResultModel { Status = false, Result = "No the resident." };

                    db.Residents.Remove(resident);
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
