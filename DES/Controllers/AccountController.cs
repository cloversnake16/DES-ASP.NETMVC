using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using DES.Models;
using DESCore.Models;
using DESCore.Database;
using DESCore.Helpers;

namespace DES.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AccountDb.Login(model.UserName, model.Password);
                if (user != null)
                {
                    Session["UserModel"] = user;

                    new EventLogDb().SaveEvent(new EventLog
                    {
                        UserId = user.User.Id,
                        SiteId = 0,
                        Event = EventType.Login.ToString(),
                        Status = EventStatus.Successful.ToString(),
                        Description = user.User.UserName + " logged in.",
                    });
                    return RedirectToAction("Index", "Dashboard");
                }else{
                    new EventLogDb().SaveEvent(new EventLog
                    {
                        UserId = 0,
                        SiteId = 0,
                        Event = EventType.Login.ToString(),
                        Status = EventStatus.Failure.ToString(),
                        Description = model.UserName + " tried to log in.",
                    });

                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            try
            {
                UserModel userModel = (UserModel)Session["UserModel"];
                
                new EventLogDb().SaveEvent(new EventLog
                {
                    UserId = userModel.User.Id,
                    SiteId = 0,
                    Event = EventType.Logout.ToString(),
                    Status = EventStatus.Successful.ToString(),
                    Description = userModel.User.UserName + " logged out.",
                });

                Session["UserModel"] = null;
                Session["Forgot"] = null;
                Session["UsersViewModel"] = null;
                Session["DashboardViewModel"] = null;
                Session["SiteModel"] = null;
                Session["SiteDataViewModel"] = null;
            }
            catch { }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult SendCode(string username)
        {
            Session["ForgotModel"] = new ForgotModel { UserName = username };
            return Json(new ResultModel { Status = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ResetPassword()
        {
            try
            {
                ForgotModel forgotModel = (ForgotModel)Session["ForgotModel"];
                var user = AccountDb.FindUser(forgotModel.UserName);
                if (user == null || string.IsNullOrEmpty(user.User.Email))
                {
                    ModelState.AddModelError("", "Invalid username or email.");
                    return RedirectToAction("Login");
                }

                forgotModel.UserId = user.User.Id;
                forgotModel.UserName = user.User.UserName;
                forgotModel.Email = user.User.Email;
                forgotModel.Code = ConvertHelper.RandomNumber(4);
                await EmailHelper.SendEmail(forgotModel.Email, "Reset your password", "<p>Your Code is " + forgotModel.Code + ".</p>");

                return View(forgotModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ForgotModel model)
        {
            try
            {
                ForgotModel forgotModel = (ForgotModel)Session["ForgotModel"];
                model.Email = forgotModel.Email;
                if (model.Code == forgotModel.Code)
                {
                    forgotModel.DefaultPassword = ConvertHelper.RandomString(6);
                    var result = AccountDb.ResetPassword(forgotModel);
                    if (result.Status)
                    {
                        Session["UserModel"] = result.Result;
                        await EmailHelper.SendEmail(forgotModel.Email, "Reseted your account", "<p>UserName:" + forgotModel.UserName +
                            ", Password: " + forgotModel.DefaultPassword + ".</p>");
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        model.ErrorMessage = (string)result.Result;
                    }
                }
                else model.ErrorMessage = "Code doesn't match.";
            }
            catch (Exception ex) { model.ErrorMessage = ex.Message; }
            return View(model);
        }

        [HttpPost]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            if (Session["UserModel"] == null) return Json(null, JsonRequestBehavior.AllowGet);

            UserModel userModel = (UserModel)Session["UserModel"];
            model.UserId = userModel.User.Id;

            ResultModel result = new AccountDb().ChangePassword(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UserSettings(User model)
        {
            if (Session["UserModel"] == null) return Json(null, JsonRequestBehavior.AllowGet);

            UserModel userModel = (UserModel)Session["UserModel"];
            model.Id = userModel.User.Id;

            ResultModel result = new AccountDb().UserSettings(model);
            if (result.Status)
            {
                Session["UserModel"] = result.Result;
                Session["UsersViewModel"] = model;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
