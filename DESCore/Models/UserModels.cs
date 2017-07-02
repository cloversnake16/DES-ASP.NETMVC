using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class UserModel
    {
        public User User { get; set; }
        public string FullName { get; set; }
        public string UserTypeName { get; set; }
        public bool IsAdmin { get; set; }
    }

    public enum UserQueryType { UserName, UserType, FirstName, LastName, Address, Email, PhoneNumber };
    public class UserQueryModel
    {
        public string[] QueryItems { get; set; }
        public string QueryKey { get; set; }
        public string QueryValue { get; set; }
    }
}
