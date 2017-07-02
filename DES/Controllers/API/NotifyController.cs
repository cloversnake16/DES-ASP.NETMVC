using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DESCore.Models;
using DESCore.Database;

namespace DES.Controllers.API
{
    public class NotifyController : ApiController
    {
        [HttpPost]
        [Route("api/Notification")]
        public DataModel Notification(NotificationModel notificationModel)
        {
            return NotificationDb.Notification(notificationModel);
        }
    }
}
