using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class ForgotModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DefaultPassword { get; set; }
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
    }
}
