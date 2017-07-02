using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DES.Models;
using DESCore.Models;
using DESCore.Database;

namespace DES.Controllers
{
    public class StaffController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                StaffModel model = new StaffModel
                {
                    ListStaffGroup = GetListStaffGroupViewModel(StaffDb.GetListStaffGroup()),
                    ListStaff = GetListStaffViewModel(StaffDb.GetListStaff()),
                };

                Session["AllSites"] = SiteDb.GetAllSites();
                return View(model);
            }
            catch { return RedirectToAction("Index", "Dashboard"); }
        }

        [HttpPost]
        public JsonResult CreateStaff(Staff staff)
        {
            try
            {
                ResultModel resultModel = StaffDb.CreateStaff(staff);
                if (resultModel.Status)
                {
                    resultModel.Result = GetStaffViewModel((Staff)resultModel.Result);
                }
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult UpdateStaff(Staff staff)
        {
            try
            {
                ResultModel resultModel = StaffDb.UpdateStaff(staff);
                if (resultModel.Status)
                {
                    resultModel.Result = GetStaffViewModel((Staff)resultModel.Result);
                }
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult RemoveStaff(Staff staff)
        {
            try
            {
                ResultModel resultModel = StaffDb.RemoveStaff(staff);
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }
        
        [HttpPost]
        public JsonResult CreateStaffGroup(StaffGroupViewModel staffGroupViewModel)
        {
            try
            {
                ResultModel resultModel = StaffDb.CreateStaffGroup(GetStaffGroupModel(staffGroupViewModel));
                if (resultModel.Status)
                {
                    resultModel.Result = GetStaffGroupViewModel((StaffGroupModel)resultModel.Result);
                }
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult UpdateStaffGroup(StaffGroupViewModel staffGroupViewModel)
        {
            try
            {
                ResultModel resultModel = StaffDb.UpdateStaffGroup(GetStaffGroupModel(staffGroupViewModel));
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult RemoveStaffGroup(int StaffGroupId)
        {
            try
            {
                ResultModel resultModel = StaffDb.RemoveStaffGroup(StaffGroupId);
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult ProgramStaffGroup()
        {
            try
            {
                ResultModel resultModel = StaffDb.ProgramStaffGroup();
                return Json(resultModel);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }); }
        }

        private List<StaffGroupViewModel> GetListStaffGroupViewModel(List<StaffGroupModel> ListStaffGroup)
        {
            try
            {
                if (ListStaffGroup == null) return null;

                List<StaffGroupViewModel> listStaffGroupViewModel = new List<StaffGroupViewModel>();
                foreach (var staffGroup in ListStaffGroup)
                {
                    listStaffGroupViewModel.Add(GetStaffGroupViewModel(staffGroup));
                }
                return listStaffGroupViewModel;
            }
            catch { return null; }           
        }

        private StaffGroupViewModel GetStaffGroupViewModel(StaffGroupModel staffGroupModel)
        {
            try
            {
                int[] tags = new int[8];
                tags[0] = staffGroupModel.StaffGroup.Tag1;
                tags[1] = staffGroupModel.StaffGroup.Tag2;
                tags[2] = staffGroupModel.StaffGroup.Tag3;
                tags[3] = staffGroupModel.StaffGroup.Tag4;
                tags[4] = staffGroupModel.StaffGroup.Tag5;
                tags[5] = staffGroupModel.StaffGroup.Tag6;
                tags[6] = staffGroupModel.StaffGroup.Tag7;
                tags[7] = staffGroupModel.StaffGroup.Tag8;

                int[] staffIds = new int[8];
                staffIds[0] = staffGroupModel.StaffGroup.StaffId1;
                staffIds[1] = staffGroupModel.StaffGroup.StaffId2;
                staffIds[2] = staffGroupModel.StaffGroup.StaffId3;
                staffIds[3] = staffGroupModel.StaffGroup.StaffId4;
                staffIds[4] = staffGroupModel.StaffGroup.StaffId5;
                staffIds[5] = staffGroupModel.StaffGroup.StaffId6;
                staffIds[6] = staffGroupModel.StaffGroup.StaffId7;
                staffIds[7] = staffGroupModel.StaffGroup.StaffId8;

                StaffGroupViewModel staffGroupViewModel = new StaffGroupViewModel                
                {
                    Id = staffGroupModel.StaffGroup.Id,
                    StaffGroupName = staffGroupModel.StaffGroup.StaffGroupName,
                    Door1 = (int)staffGroupModel.StaffGroup.Door1,
                    Door2 = (int)(staffGroupModel.StaffGroup.Door1 >> 32),
                    Door3 = (int)staffGroupModel.StaffGroup.Door2,
                    Door4 = (int)(staffGroupModel.StaffGroup.Door2 >> 32),
                    Tags = tags,
                    StaffIds = staffIds,
                    SiteIds = staffGroupModel.SiteIds==null ? "" : string.Join(",", staffGroupModel.SiteIds),
                    DateUpdated=staffGroupModel.StaffGroup.DateUpdated
                };
                
                return staffGroupViewModel;
            }
            catch { }
            return null;
        }

        private StaffGroupModel GetStaffGroupModel(StaffGroupViewModel staffGroupViewModel)
        {
            try
            {
                if (staffGroupViewModel == null) return null;

                int[] siteIds = null;
                if (staffGroupViewModel.SiteIds != null && staffGroupViewModel.SiteIds != "")
                {
                    string[] words = staffGroupViewModel.SiteIds.Split(',');
                    siteIds = new int[words.Length];
                    for (var i = 0; i < words.Length; i++) siteIds[i] = int.Parse(words[i]);
                }

                StaffGroupModel staffGroupModel = new StaffGroupModel
                {
                    StaffGroup = new StaffGroup
                    {
                        Id = staffGroupViewModel.Id,
                        StaffGroupName = staffGroupViewModel.StaffGroupName,
                        StaffId1 = staffGroupViewModel.StaffIds[0],
                        StaffId2 = staffGroupViewModel.StaffIds[1],
                        StaffId3 = staffGroupViewModel.StaffIds[2],
                        StaffId4 = staffGroupViewModel.StaffIds[3],
                        StaffId5 = staffGroupViewModel.StaffIds[4],
                        StaffId6 = staffGroupViewModel.StaffIds[5],
                        StaffId7 = staffGroupViewModel.StaffIds[6],
                        StaffId8 = staffGroupViewModel.StaffIds[7],
                        Tag1 = staffGroupViewModel.Tags[0],
                        Tag2 = staffGroupViewModel.Tags[1],
                        Tag3 = staffGroupViewModel.Tags[2],
                        Tag4 = staffGroupViewModel.Tags[3],
                        Tag5 = staffGroupViewModel.Tags[4],
                        Tag6 = staffGroupViewModel.Tags[5],
                        Tag7 = staffGroupViewModel.Tags[6],
                        Tag8 = staffGroupViewModel.Tags[7],
                    },
                    SiteIds = siteIds,
                };
                staffGroupModel.StaffGroup.Door1 = staffGroupViewModel.Door2;
                staffGroupModel.StaffGroup.Door1 <<= 32;
                staffGroupModel.StaffGroup.Door1 |= (UInt32)staffGroupViewModel.Door1;
                staffGroupModel.StaffGroup.Door2 = staffGroupViewModel.Door4;
                staffGroupModel.StaffGroup.Door2 <<= 32;
                staffGroupModel.StaffGroup.Door2 |= (UInt32)staffGroupViewModel.Door3;
                return staffGroupModel;
            }
            catch { }
            return null;
        }

        private List<StaffViewModel> GetListStaffViewModel(List<Staff> ListStaff)
        {
            try
            {
                if (ListStaff == null) return null;

                List<StaffViewModel> listStaffViewModel = new List<StaffViewModel>();
                foreach (var staff in ListStaff)
                {
                    StaffViewModel staffViewModel = GetStaffViewModel(staff);
                    if (staffViewModel != null) listStaffViewModel.Add(staffViewModel);
                }
                return listStaffViewModel;
            }
            catch { return null; }
        }

        private StaffViewModel GetStaffViewModel(Staff staff)
        {
            try
            {
                if (staff == null) return null;

                    return new StaffViewModel
                    {
                        Id = staff.Id,
                        StaffGroupId=staff.StaffGroupId,
                        FirstName = staff.FirstName,
                        LastName = staff.LastName,
                        PayrollNumber = staff.PayrollNumber,
                        DateExpire = staff.DateExpire.ToString("yyyy-MM-dd"),
                        DateUpdated = staff.DateUpdated.ToString(),
                    };
            }
            catch { return null; }
        }
    }
}