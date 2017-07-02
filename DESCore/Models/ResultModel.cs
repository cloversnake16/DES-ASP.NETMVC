using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class ResultModel
    {
        public bool Status { get; set; }
        public string Command { get; set; }
        public object Result { get; set; }
    }
}
