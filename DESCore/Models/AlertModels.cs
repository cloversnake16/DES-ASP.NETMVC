using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESCore.Models
{
    public class ListAlertModel
    {
        public List<AlertModel> AlertModels { get; set; }
        public int CheckPeroid { get; set; }
    }

    public class AlertModel
    {
        public string Email { get; set; }
        public string AlertMsg { get; set; }
    }
}
