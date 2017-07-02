using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;
using System.Reflection;

namespace DESCore.Database
{
    public class StaffDb
    {
        public static int[] GetSiteIds(int staffGroupId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.StaffGroupSites.Where(r => r.StaffGroupId == staffGroupId).Select(r => r.SiteId).ToArray();
                }
            }
            catch (Exception) { return null; }
        }

        public static List<Staff> GetListStaff()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.Staffs.ToList();
                }
            }
            catch (Exception) { return null; }
        }

        public static ResultModel CreateStaff(Staff model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staff = db.Staffs.Where(r => r.PayrollNumber == model.PayrollNumber).FirstOrDefault();
                    if (staff != null)
                    {
                        return new ResultModel { Status = false, Result = "The payroll already exists.", };
                    }
                    else
                    {
                        model.DateUpdated = DateTime.Now;
                        if (model.StaffGroupId == 0)
                        {
                            staff = db.Staffs.Add(model);
                            db.SaveChanges();

                            return new ResultModel { Status = true, Result = staff, };
                        }
                        else
                        {
                            string optionAttr;
                            var staffGroupFields = db.StaffGroups.Where(r => r.Id == model.StaffGroupId).FirstOrDefault();
                            if (staffGroupFields.StaffId1 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId1 = staff.Id;
                                optionAttr = "data-staff1";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command=optionAttr};
                            }
                            else if (staffGroupFields.StaffId2 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId2 = staff.Id;
                                optionAttr = "data-staff2";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId3 == 0)
                            {                               
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId3 = staff.Id;
                                optionAttr = "data-staff3";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId4 == 0)
                            {                                
                                staff = db.Staffs.Add(model);                                
                                db.SaveChanges();
                                staff.StaffGroupId = 4;

                                staffGroupFields.StaffId4 = staff.Id;
                                optionAttr = "data-staff4";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId5 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId5 = staff.Id;
                                optionAttr = "data-staff5";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId6 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId6 = staff.Id;
                                optionAttr = "data-staff6";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId7 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId7 = staff.Id;
                                optionAttr = "data-staff7";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else if (staffGroupFields.StaffId8 == 0)
                            {                                
                                staff = db.Staffs.Add(model);
                                db.SaveChanges();

                                staffGroupFields.StaffId8 = staff.Id;
                                optionAttr = "data-staff8";
                                db.SaveChanges();
                                return new ResultModel { Status = true, Result = staff, Command = optionAttr };
                            }
                            else
                            {
                                return new ResultModel { Status = false, Result = "Staff was full of Group.", };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { Status = false, Result = ex.Message, };
            };
        }

        public static ResultModel UpdateStaff(Staff model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staff = db.Staffs.Where(r => r.Id != model.Id && r.PayrollNumber == model.PayrollNumber).FirstOrDefault();
                    if (staff != null) return new ResultModel { Status = false, Result = "The payroll already exists.", };

                    staff = db.Staffs.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (staff != null)
                    {
                        // group check full or not 
                        var staffGroupFields = db.StaffGroups.Where(r => (r.StaffId1 == 0 || r.StaffId2 == 0 || r.StaffId3 == 0 || r.StaffId4 == 0 || r.StaffId5 == 0 || r.StaffId6 == 0 || r.StaffId7 == 0 || r.StaffId8 == 0) && (r.Id==model.StaffGroupId)).FirstOrDefault();
                        if (staffGroupFields == null && model.StaffGroupId > 0)
                        {
                            return new ResultModel { Status = false, Result = "Staff was full of Group.", };
                        }
                        else
                        {
                            // init
                            string optionAttr="";
                            staffGroupFields = db.StaffGroups.Where(r => r.StaffId1 == model.Id || r.StaffId2 == model.Id || r.StaffId3 == model.Id || r.StaffId4 == model.Id || r.StaffId5 == model.Id || r.StaffId6 == model.Id || r.StaffId7 == model.Id || r.StaffId8 == model.Id).FirstOrDefault();
                            if (staffGroupFields != null)
                            {
                                if (staffGroupFields.StaffId1 == model.Id)
                                {
                                    staffGroupFields.StaffId1 = 0;
                                    optionAttr = "prev:data-staff1="+Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId2 == model.Id)
                                {
                                    staffGroupFields.StaffId2 = 0;
                                    optionAttr = "prev:data-staff2=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId3 == model.Id)
                                {
                                    staffGroupFields.StaffId3 = 0;
                                    optionAttr = "prev:data-staff3=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId4 == model.Id)
                                {
                                    staffGroupFields.StaffId4 = 0;
                                    optionAttr = "prev:data-staff4=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId5 == model.Id)
                                {
                                    staffGroupFields.StaffId5 = 0;
                                    optionAttr = "prev:data-staff5=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId6 == model.Id)
                                {
                                    staffGroupFields.StaffId6 = 0;
                                    optionAttr = "prev:data-staff6=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId7 == model.Id)
                                {
                                    staffGroupFields.StaffId7 = 0;
                                    optionAttr = "prev:data-staff7=" + Convert.ToString(staffGroupFields.Id);
                                }
                                else if (staffGroupFields.StaffId8 == model.Id)
                                {
                                    staffGroupFields.StaffId8 = 0;
                                    optionAttr = "prev:data-staff8=" + Convert.ToString(staffGroupFields.Id);
                                }
                            }
                            // update                 
                            if (model.StaffGroupId > 0)
                            {
                                staffGroupFields = db.StaffGroups.Where(r => r.Id == model.StaffGroupId).FirstOrDefault();
                                if (staffGroupFields.StaffId1 == 0)
                                {
                                    staffGroupFields.StaffId1 = model.Id;
                                    optionAttr += ",cur:data-staff1";
                                }
                                else if (staffGroupFields.StaffId2 == 0)
                                {
                                    staffGroupFields.StaffId2 = model.Id;
                                    optionAttr += ",cur:data-staff2";
                                }
                                else if (staffGroupFields.StaffId3 == 0)
                                {
                                    staffGroupFields.StaffId3 = model.Id;
                                    optionAttr += ",cur:data-staff3";
                                }
                                else if (staffGroupFields.StaffId4 == 0)
                                {
                                    staffGroupFields.StaffId4 = model.Id;
                                    optionAttr += ",cur:data-staff4";
                                }
                                else if (staffGroupFields.StaffId5 == 0)
                                {
                                    staffGroupFields.StaffId5 = model.Id;
                                    optionAttr += ",cur:data-staff5";
                                }
                                else if (staffGroupFields.StaffId6 == 0)
                                {
                                    staffGroupFields.StaffId6 = model.Id;
                                    optionAttr += ",cur:data-staff6";
                                }
                                else if (staffGroupFields.StaffId7 == 0)
                                {
                                    staffGroupFields.StaffId7 = model.Id;
                                    optionAttr += ",cur:data-staff7";
                                }
                                else if (staffGroupFields.StaffId8 == 0)
                                {
                                    staffGroupFields.StaffId8 = model.Id;
                                    optionAttr += ",cur:data-staff8";
                                }
                            }

                            staff.StaffGroupId = model.StaffGroupId;
                            staff.FirstName = model.FirstName;
                            staff.LastName = model.LastName;
                            staff.PayrollNumber = model.PayrollNumber;
                            staff.DateExpire = model.DateExpire;
                            staff.DateUpdated = DateTime.Now;

                            db.Entry(staff).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            return new ResultModel { Status = true, Result = staff, Command=optionAttr};
                        }                        
                    }
                    else return new ResultModel { Status = false, Result = "The staff doesn't exist.", };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }

        public static ResultModel RemoveStaff(Staff model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staff = db.Staffs.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (staff != null)
                    {
                        // check staff group exists or not                        
                        string optionAttr = "no";
                        // init                            
                        var staffGroupFields = db.StaffGroups.Where(r => r.StaffId1 == model.Id || r.StaffId2 == model.Id || r.StaffId3 == model.Id || r.StaffId4 == model.Id || r.StaffId5 == model.Id || r.StaffId6 == model.Id || r.StaffId7 == model.Id || r.StaffId8 == model.Id).FirstOrDefault();
                        if (staffGroupFields != null)
                        {
                            if (staffGroupFields.StaffId1 == model.Id)
                            {
                                staffGroupFields.StaffId1 = 0;
                                optionAttr = "data-staff1=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId2 == model.Id)
                            {
                                staffGroupFields.StaffId2 = 0;
                                optionAttr = "data-staff2=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId3 == model.Id)
                            {
                                staffGroupFields.StaffId3 = 0;
                                optionAttr = "data-staff3=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId4 == model.Id)
                            {
                                staffGroupFields.StaffId4 = 0;
                                optionAttr = "data-staff4=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId5 == model.Id)
                            {
                                staffGroupFields.StaffId5 = 0;
                                optionAttr = "data-staff5=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId6 == model.Id)
                            {
                                staffGroupFields.StaffId6 = 0;
                                optionAttr = "data-staff6=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId7 == model.Id)
                            {
                                staffGroupFields.StaffId7 = 0;
                                optionAttr = "data-staff7=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                            else if (staffGroupFields.StaffId8 == model.Id)
                            {
                                staffGroupFields.StaffId8 = 0;
                                optionAttr = "data-staff8=" + Convert.ToString(staffGroupFields.Id + "=" + staff.Id);
                            }
                        }
                        db.Staffs.Remove(staff);
                        db.SaveChanges();
                        return new ResultModel { Status = true, Command= optionAttr, };
                    }
                    else return new ResultModel { Status = false, Result = "The staff doesn't exist.", };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }

        public static List<StaffGroupModel> GetListStaffGroup()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staffGroups = db.StaffGroups.ToArray();
                    if (staffGroups == null) return null;

                    List<StaffGroupModel> staffGroupModels = new List<StaffGroupModel>();
                    foreach (var staffGroup in staffGroups)
                    {
                        var siteIds = db.StaffGroupSites.Where(r => r.StaffGroupId == staffGroup.Id).Select(r => r.SiteId).ToArray();
                        staffGroupModels.Add(new StaffGroupModel
                        {
                            StaffGroup = staffGroup,
                            SiteIds = siteIds,
                        });
                    }
                    return staffGroupModels;
                }
            }
            catch (Exception) { return null; }
        }

        public static ResultModel CreateStaffGroup(StaffGroupModel model)
        {
            try
            {
                using (var db = new DESEntities())
                {

                    var staffGroup = db.StaffGroups.Where(r => r.StaffGroupName.ToLower() == model.StaffGroup.StaffGroupName.ToLower()).FirstOrDefault();
                    if (staffGroup == null)
                    {
                        model.StaffGroup.DateUpdated = DateTime.Now;
                        staffGroup = db.StaffGroups.Add(model.StaffGroup);
                        db.SaveChanges();
                        model.StaffGroup.Id = staffGroup.Id;

                        if (model.SiteIds != null)
                        {
                            int[] reserved = SettingDb.GetReservedChannelIndics();
                            if (reserved != null && reserved.Length > 0)
                            {
                                foreach (var siteId in model.SiteIds)
                                {
                                    var channelIndex = reserved[0];
                                    int[] channels = db.StaffGroupSites.Where(r => r.SiteId == siteId).Select(r => r.ChannelIndex).ToArray();
                                    if (channels != null && channels.Length > 0) channelIndex = reserved.Where(r => !channels.Contains(r)).FirstOrDefault();
                                    var staffGroupSite = new StaffGroupSite
                                    {
                                        StaffGroupId = staffGroup.Id,
                                        SiteId = siteId,
                                        ChannelIndex = channelIndex,
                                        DateUpdated = DateTime.Now,
                                    };
                                    db.StaffGroupSites.Add(staffGroupSite);
                                }
                                db.SaveChanges();
                            }
                        }

                        return new ResultModel { Status = true, Result = model, };
                    }
                    else return new ResultModel { Status = false, Result = model.StaffGroup.StaffGroupName + " already exists.", };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }

        public static ResultModel UpdateStaffGroup(StaffGroupModel model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staffGroup = db.StaffGroups.Where(r => r.Id == model.StaffGroup.Id).FirstOrDefault();
                    if (staffGroup != null)
                    {
                        staffGroup.StaffGroupName = model.StaffGroup.StaffGroupName;
                        staffGroup.Door1 = model.StaffGroup.Door1;
                        staffGroup.Door2 = model.StaffGroup.Door2;
                        staffGroup.StaffId1 = model.StaffGroup.StaffId1;
                        staffGroup.StaffId2 = model.StaffGroup.StaffId2;
                        staffGroup.StaffId3 = model.StaffGroup.StaffId3;
                        staffGroup.StaffId4 = model.StaffGroup.StaffId4;
                        staffGroup.StaffId5 = model.StaffGroup.StaffId5;
                        staffGroup.StaffId6 = model.StaffGroup.StaffId6;
                        staffGroup.StaffId7 = model.StaffGroup.StaffId7;
                        staffGroup.StaffId8 = model.StaffGroup.StaffId8;
                        staffGroup.Tag1 = model.StaffGroup.Tag1;
                        staffGroup.Tag2 = model.StaffGroup.Tag2;
                        staffGroup.Tag3 = model.StaffGroup.Tag3;
                        staffGroup.Tag4 = model.StaffGroup.Tag4;
                        staffGroup.Tag5 = model.StaffGroup.Tag5;
                        staffGroup.Tag6 = model.StaffGroup.Tag6;
                        staffGroup.Tag7 = model.StaffGroup.Tag7;
                        staffGroup.Tag8 = model.StaffGroup.Tag8;
                        staffGroup.DateUpdated = DateTime.Now;
                        db.Entry(staffGroup).State = System.Data.Entity.EntityState.Modified;

                        if (model.SiteIds != null)
                        {
                            var staffSites = db.StaffGroupSites.Where(r => r.StaffGroupId == staffGroup.Id && !model.SiteIds.Contains(r.SiteId)).ToArray();
                            if (staffSites != null) db.StaffGroupSites.RemoveRange(staffSites);

                            int[] reserved = SettingDb.GetReservedChannelIndics();
                            if (reserved != null && reserved.Length > 0)
                            {
                                foreach (var siteId in model.SiteIds)
                                {
                                    var staffGroupSite = db.StaffGroupSites.Where(r => r.StaffGroupId == staffGroup.Id && r.SiteId == siteId).FirstOrDefault();
                                    if (staffGroupSite == null)
                                    {
                                        var channelIndex = reserved[0];
                                        int[] channels = db.StaffGroupSites.Where(r => r.SiteId == siteId).Select(r => r.ChannelIndex).ToArray();
                                        if (channels != null && channels.Length > 0) channelIndex = reserved.Where(r => !channels.Contains(r)).FirstOrDefault();

                                        staffGroupSite = new StaffGroupSite
                                        {
                                            StaffGroupId = staffGroup.Id,
                                            SiteId = siteId,
                                            ChannelIndex = channelIndex,
                                            DateUpdated = DateTime.Now,
                                        };
                                        db.StaffGroupSites.Add(staffGroupSite);
                                    }
                                }
                            }
                        }

                        db.SaveChanges();

                        return new ResultModel { Status = true, Result = "Updated " + model.StaffGroup.StaffGroupName + " successfully." };
                    }
                    else return new ResultModel { Status = false, Result = "No existing " + model.StaffGroup.StaffGroupName + "." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }

        public static ResultModel RemoveStaffGroup(int StaffGroupId)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var staffGroup = db.StaffGroups.Where(r => r.Id == StaffGroupId).FirstOrDefault();
                    if (staffGroup != null)
                    {
                        db.StaffGroups.Remove(staffGroup);

                        var staffGroupSites = db.StaffGroupSites.Where(r => r.StaffGroupId == staffGroup.Id).ToArray();
                        db.StaffGroupSites.RemoveRange(staffGroupSites);

                        db.SaveChanges();

                        return new ResultModel { Status = true, Result = "Deleted successfully" };
                    }
                    else return new ResultModel { Status = false, Result = "No existing." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }

        public static ResultModel ProgramStaffGroup()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var siteIds = db.StaffGroupSites.Select(r => r.SiteId).Distinct().ToArray();
                    if (siteIds != null)
                    {
                        foreach (var siteId in siteIds)
                        {
                            var site = db.Sites.Where(r => r.Id == siteId).FirstOrDefault();
                            if (site != null)
                            {
                                var backTask = new BackTask
                                {
                                    UserId = site.UserId,
                                    DeviceId = site.DeviceId,
                                    WorkType = SiteDataType.ProgramStaff.ToString(),
                                    WorkStatus = WorkStatusType.Wait.ToString(),
                                    WorkItem = "",
                                    WorkIndex = 0,
                                    Description = "",
                                };
                                BackTaskDb.AddBackTask(backTask);
                            }
                        }
                    }
                    return new ResultModel { Status = true, Result = "Deleted successfully" };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message, }; };
        }
    }
}
