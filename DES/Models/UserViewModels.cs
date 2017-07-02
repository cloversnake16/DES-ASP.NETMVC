using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DESCore.Models;

namespace DES.Models
{
    public class UsersViewModel
    {
        public IEnumerable<UserModel> Users { get; set; }

        public UserQueryModel Query { get; set; }

        public IEnumerable<UserType> UserTypes { get; set; }
    }

    public class UserViewModel
    {
        public string UserName { get; set; }

        public int UserTypeId { get; set; }

        public string UserType { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserType> UserTypes { get; set; }

        public string Info { get; set; }

        public string Error { get; set; }
    }
}