using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using DES.Models;
using DESCore.Models;
using DESCore.Database;

namespace DES.Controllers
{
    public class SetController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (Session["UserModel"] == null || !((UserModel)Session["UserModel"]).IsAdmin) return RedirectToAction("Index", "Account");

                string[] descriptions = new string[128];
                var doorDescriptions = SettingDb.GetDoorDescriptions();
                if (doorDescriptions != null)
                {
                    foreach (var doorDescription in doorDescriptions)
                    {
                        descriptions[doorDescription.Index] = doorDescription.Contents;
                    }
                }

                Setting model = SettingDb.GetSetting();
                return View(new SetViewModel
                {
                    CheckPeriod = model.CheckPeriod,
                    MaxDataSize = model.MaxDataSize,
                    MaxEventlogSize = model.MaxEventlogSize,
                    MonthReset = model.DateReset.Month,
                    DayReset = model.DateReset.Day,
                    ReservedChannels = model.ReservedChannel,
                    DoorDescriptions = descriptions,
                });
            }
            catch { return View(); }            
        }

        [HttpPost]
        public JsonResult SaveReservedChannels(ReservedChannels ReservedChannels)
        {
            try
            {
                int reservedChannels = 0;
                for (var i = 0; i < 16; i++)
                {
                    if (ReservedChannels.Channels[i]) reservedChannels |= (1 << i);
                }
                return Json(SettingDb.SaveReservedChannels(reservedChannels));
            }
            catch (Exception ex) { return Json(new ResultModel { Status = true, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult SetAlertSetting(SetViewModel model)
        {
            try
            {
                return Json(SettingDb.SetAlertSetting(new Setting
                {
                    CheckPeriod = model.CheckPeriod,
                    MaxDataSize = model.MaxDataSize,
                    MaxEventlogSize = model.MaxEventlogSize,
                    DateReset = new DateTime(2000, model.MonthReset, model.DayReset),
                }));
            }
            catch (Exception ex) { return Json(new ResultModel { Status = true, Result = ex.Message, }); }
        }

        [HttpPost]
        public JsonResult SaveDoorDescription(Description model)
        {
            try
            {
                return Json(SettingDb.SaveDoorDescription(model));
            }
            catch (Exception ex) { return Json(new ResultModel { Status = true, Result = ex.Message, }); }
        }
    }
}
