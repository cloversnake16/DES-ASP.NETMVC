using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DESCore.Models;
using DESCore.Database;

namespace DES.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Account");
        }

        public ActionResult BackWork()
        {
            if (Session["UserModel"] == null) return RedirectToAction("Index", "Account");
            UserModel userModel = (UserModel)Session["UserModel"];

            return View(BackTaskDb.GetNotifications(userModel.User.Id));
        }

        public ActionResult Notification()
        {
            if (Session["UserModel"] == null) return RedirectToAction("Index", "Account");
            UserModel userModel = (UserModel)Session["UserModel"];

            return View(HomeDb.GetNotifications(userModel.User.Id));
        }

        public FileResult Download()
        {
            var FileVirtualPath = "~/App_Data/Setup.zip";
            return File(FileVirtualPath, "application/force-download", System.IO.Path.GetFileName(FileVirtualPath));
        }  

        [HttpPost]
        public JsonResult RetryTask(int Id)
        {
            try
            {
                ResultModel resultModel = BackTaskDb.RetryBackTask(Id);
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }, JsonRequestBehavior.AllowGet); }
        }

        public JsonResult CancelTask(int Id)
        {
            try
            {
                ResultModel resultModel = BackTaskDb.DeleteBackTask(Id);
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public JsonResult RemoveNotification(int Id)
        {
            try
            {
                ResultModel resultModel = HomeDb.RemoveNotification(Id);
                return Json(resultModel, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new ResultModel { Status = false, Result = ex.Message, }, JsonRequestBehavior.AllowGet); }
        }
    }
}