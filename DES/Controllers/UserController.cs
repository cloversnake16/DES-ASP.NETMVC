using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DES.Models;
using DESCore.Models;
using DESCore.Database;
using DESCore.Helpers;

namespace DES.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            UsersViewModel model;

            if (Session["UsersViewModel"] != null) model = (UsersViewModel)Session["UsersViewModel"];
            else
            {
                if (Session["UserModel"] == null || !((UserModel)Session["UserModel"]).IsAdmin) return RedirectToAction("Index", "Account");
                
                IEnumerable<UserType> userTypes = new AccountDb().GetUserTypes();
                if (userTypes == null) RedirectToAction("Index", "Dashboard");

                UserQueryModel query = new UserQueryModel { QueryItems = Enum.GetNames(typeof(UserQueryType)) };
                model = new UsersViewModel
                {
                    Users = new AccountDb().GetUsers(query),
                    Query = query,
                    UserTypes = userTypes,
                };

                Session["UsersViewModel"] = model;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(UserQueryModel query)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index", "User");

            UsersViewModel usersViewModel = (UsersViewModel)Session["UsersViewModel"];

            usersViewModel.Query.QueryKey = query.QueryKey;
            usersViewModel.Query.QueryValue = query.QueryValue;
            usersViewModel.Users = new AccountDb().GetUsers(usersViewModel.Query);

            return RedirectToAction("Index");
        }

        public ActionResult ClearQuery()
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index", "User");

            UsersViewModel model = (UsersViewModel)Session["UsersViewModel"];
            model.Query.QueryKey = null;
            model.Query.QueryValue = null;

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");
            UsersViewModel usersViewModel = (UsersViewModel)Session["UsersViewModel"];
            return View(new UserViewModel { UserTypes = usersViewModel.UserTypes });
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");
            UsersViewModel usersViewModel = (UsersViewModel)Session["UsersViewModel"];

            model.UserTypes = usersViewModel.UserTypes;

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Email))
            {
                model.Error = "Empty Username or Email";
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                UserTypeId = model.UserTypeId,
                DefaultPassword = ConvertHelper.RandomString(6),
                Email = model.Email,
            };

            ResultModel result = new AccountDb().CreateUser(user);
            if (result.Status)
            {
                await EmailHelper.SendEmail(model.Email, "Created your new account", "<p>UserName:" + user.UserName + ", Password: " + user.DefaultPassword + ".</p>");
                model.Info = "Created user successfully.";
                Session["UsersViewModel"] = null;
            }
            else
            {
                model.Info = "";
                model.Error = (string)result.Result;
            }

            return View(model);
        }

        public ActionResult Update(int UserId)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");
            UsersViewModel usersViewModel = (UsersViewModel)Session["UsersViewModel"];

            UserModel userModel = ((UsersViewModel)Session["UsersViewModel"]).Users.Where(r => r.User.Id == UserId).FirstOrDefault();
            Session["UserViewModel"] = userModel;

            return View(new UserViewModel 
            {
                UserName = userModel.User.UserName,
                UserTypeId = userModel.User.UserTypeId,
                Email = userModel.User.Email,
                UserTypes = usersViewModel.UserTypes,
            });
        }

        [HttpPost]
        public async Task<ActionResult> Update(UserViewModel model)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");
            UsersViewModel usersViewModel = (UsersViewModel)Session["UsersViewModel"];

            UserModel userModel = ((UserModel)Session["UserViewModel"]);

            model.UserName = userModel.User.UserName;
            model.UserTypes = usersViewModel.UserTypes;

            if (string.IsNullOrEmpty(model.Email))
            {
                model.UserTypeId = userModel.User.UserTypeId;
                model.Email = userModel.User.Email;
                model.Error = "Empty Email";
                return View(model);
            }

            var user = new User
            {
                Id = userModel.User.Id,
                UserTypeId = model.UserTypeId,
                Email = model.Email,
            };

            ResultModel result = new AccountDb().UpdateUser(user);
            if (result.Status)
            {
                if (model.Email != userModel.User.Email)
                {
                    await EmailHelper.SendEmail(model.Email, "Created your new account", "<p>UserName:" + userModel.User.UserName + ", Password: " + userModel.User.DefaultPassword + ".</p>");
                }
                Session["UsersViewModel"] = null;
                return RedirectToAction("Index");
            }

            model.Error = (string)result.Result;
            return View(model);
        }

        public ActionResult Delete(int UserId)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");

            UserModel user = ((UsersViewModel)Session["UsersViewModel"]).Users.Where(r => r.User.Id == UserId).FirstOrDefault();
            Session["UserViewModel"] = user;
            return View(new UserViewModel { UserName = user.User.UserName, Email = user.User.Email, UserType = user.UserTypeName });
        }

        [HttpPost]
        public ActionResult Delete(UserViewModel model)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");

            UserModel userModel = ((UserModel)Session["UserViewModel"]);

            model.UserName = userModel.User.UserName;
            model.UserType = userModel.UserTypeName;
            model.Email = userModel.User.Email;

            var user = new User
            {
                Id = userModel.User.Id,
            };

            ResultModel result = new AccountDb().DeleteUser(user);
            if (result.Status)
            {
                Session["UsersViewModel"] = null;
                return RedirectToAction("Index");
            }

            model.Error = (string)result.Result;
            return View(model);
        }

        public async Task<ActionResult> ResendPassword(int UserId)
        {
            if (Session["UsersViewModel"] == null) return RedirectToAction("Index");

            UserModel userModel = ((UsersViewModel)Session["UsersViewModel"]).Users.Where(r => r.User.Id == UserId).FirstOrDefault();

            await EmailHelper.SendEmail(userModel.User.Email, "Created your new account", "<p>UserName:" + userModel.User.UserName + ", Password: " + userModel.User.DefaultPassword + ".</p>");

            return RedirectToAction("Index");
        }
    }
}