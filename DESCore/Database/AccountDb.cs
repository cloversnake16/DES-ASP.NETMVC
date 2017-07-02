using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;
using DESCore.Helpers;

namespace DESCore.Database
{
    public class AccountDb
    {
        public IEnumerable<UserModel> GetUsers(UserQueryModel query)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    User[] users = db.Users.ToArray();
                    if (!string.IsNullOrEmpty(query.QueryKey) && !string.IsNullOrEmpty(query.QueryValue))
                    {
                        UserQueryType userQueryType = (UserQueryType)Enum.Parse(typeof(UserQueryType), query.QueryKey);

                        if (userQueryType == UserQueryType.UserName) users = users.Where(r => r.UserName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.UserType) users = users.Where(r => r.UserType.TypeName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.FirstName) users = users.Where(r => r.FirstName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.LastName) users = users.Where(r => r.LastName.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.Address) users = users.Where(r => r.Address.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.Email) users = users.Where(r => r.Email.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                        else if (userQueryType == UserQueryType.PhoneNumber) users = users.Where(r => r.ContactNumber.ToLower().Contains(query.QueryValue.Trim().ToLower())).ToArray();
                    }

                    if (users != null)
                    {
                        List<UserModel> listUser = new List<UserModel>();
                        foreach (User user in users) listUser.Add(ToUserModel(user));
                        return listUser;
                    }
                }
            }
            catch { }
            return null;
        }

        public IEnumerable<UserType> GetUserTypes()
        {
            try
            {
                using (var db = new DESEntities())
                {
                    return db.UserTypes.ToArray();
                }
            }
            catch { }
            return null;
        }

        public static UserModel ToUserModel(User user)
        {
            try
            {
                return new UserModel
                {
                    User = user,
                    FullName = AccountDb.UserFullName(user),
                    UserTypeName = user.UserType.TypeName,
                    IsAdmin = (user.UserType.TypeName == "SysAdmin"),
                };
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public static string UserFullName(User user)
        {
            try
            {
                string fullName = (user.FirstName + " " + user.LastName).Trim();
                if (string.IsNullOrEmpty(fullName)) fullName = user.UserName;
                return fullName;
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public static UserModel FindUser(string userNameOrEmail)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var user = db.Users.Where(r => r.UserName == userNameOrEmail || r.Email == userNameOrEmail).FirstOrDefault();
                    return ToUserModel(user);
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public static UserModel Login(string userName, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) return null;
                using (var db = new DESEntities())
                {
                    string encriptedPwd = SecurityHelper.Encrypt(password);

                    var user = db.Users.Where(r => (r.UserName == userName || r.Email == userName) && r.Password == encriptedPwd).FirstOrDefault();
                    if (user == null)
                    {
                        user = db.Users.Where(r => (r.UserName == userName || r.Email == userName)).FirstOrDefault();
                        if (user == null || user.Password != null || user.DefaultPassword != password) return null;
                    }
                    Notification notification = new Notification
                    {
                        UserId = user.Id,
                        Title = "Login",
                        Contents = user.UserName + " logged in successfully.",
                        DateUpdated = DateTime.Now,
                    };
                    NotificationDb.AddNotification(notification, db);

                    return ToUserModel(user);
                }
            }
            catch (Exception ex) { string err = ex.Message; }
            return null;
        }

        public ResultModel ChangePassword(ChangePasswordModel model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var user = db.Users.Where(r => r.Id == model.UserId).FirstOrDefault();
                    if (user == null) return new ResultModel { Status = false, Result = "Couldn't find The user." };

                    string encriptedPwd = SecurityHelper.Encrypt(model.OldPassword);

                    if (string.IsNullOrEmpty(user.Password) || user.Password == encriptedPwd)
                    {
                        user.Password = SecurityHelper.Encrypt(model.NewPassword);
                        db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return new ResultModel { Status = true };
                    }
                    else return new ResultModel { Status = false, Result = "Old password is invalid." };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public static ResultModel ResetPassword(ForgotModel model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var user = db.Users.Where(r => r.Id == model.UserId).FirstOrDefault();
                    if (user == null) return new ResultModel { Status = false, Result = "Couldn't find the user." };

                    user.DefaultPassword = model.DefaultPassword;
                    user.Password = SecurityHelper.Encrypt(model.DefaultPassword);

                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return new ResultModel { Status = true, Result = ToUserModel(user) };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public ResultModel UserSettings(User model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    var user = db.Users.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (user == null) return new ResultModel { Status = false, Result = "Couldn't find The user." };

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.ContactNumber = model.ContactNumber;
                    user.DateUpdated = DateTime.Now;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return new ResultModel
                    {
                        Status = true,
                        Result = ToUserModel(user),
                    };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public ResultModel CreateUser(User model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    User user = db.Users.Where(r => r.UserName == model.UserName).FirstOrDefault();
                    if (user != null) return new ResultModel { Status = false, Result = "The user already exists." };

                    user = new User
                    {
                        UserName = model.UserName,
                        UserTypeId = model.UserTypeId,
                        DefaultPassword = model.DefaultPassword,
                        Password = SecurityHelper.Encrypt(model.DefaultPassword),
                        Email = model.Email,
                        DateUpdated = DateTime.Now
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public ResultModel UpdateUser(User model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    User user = db.Users.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (user == null) return new ResultModel { Status = false, Result = "The user doesn't exist." };

                    user.UserTypeId = model.UserTypeId;
                    user.Email = model.Email;
                    user.DateUpdated = DateTime.Now;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public ResultModel DeleteUser(User model)
        {
            try
            {
                using (var db = new DESEntities())
                {
                    User user = db.Users.Where(r => r.Id == model.Id).FirstOrDefault();
                    if (user == null) return new ResultModel { Status = false, Result = "The user doesn't exist." };

                    db.Users.Remove(user);
                    db.SaveChanges();
                    return new ResultModel { Status = true };
                }
            }
            catch (Exception ex) { return new ResultModel { Status = false, Result = ex.Message }; }
        }

        public static int ValidUser(string username, string password, DESEntities db = null)
        {
            try
            {
                if (db == null)
                {
                    using (var entity = new DESEntities())
                    {
                        return ValidUser(username, password, entity);
                    }
                }
                else
                {
                    User user = db.Users.Where(r => r.UserName.ToLower() == username.ToLower() ||
                        r.Email.ToLower() == username.ToLower()).FirstOrDefault();

                    if (user == null) return 0;
                    else if (string.IsNullOrEmpty(user.Password))
                    {
                        if (user.DefaultPassword != password) return 0;
                    }
                    else if (user.Password != SecurityHelper.Encrypt(password)) return 0;

                    return user.Id;
                }
            }
            catch { }
            return 0;
        }
    }
}
