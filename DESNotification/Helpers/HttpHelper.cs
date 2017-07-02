using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using DESNotification.Models;

namespace DESNotification.Helpers
{
    public class HttpHelper
    {
        public static async Task<DataModel> Post<T>(string url, T data)
        {
            try
            {
                var javaScriptSerializer = new JavaScriptSerializer();
                string strData = javaScriptSerializer.Serialize(data);
                using (var stringContent = new StringContent(strData, System.Text.Encoding.UTF8, "application/json"))
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, stringContent);
                        var result = await response.Content.ReadAsStringAsync();
                        return javaScriptSerializer.Deserialize<DataModel>(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return ConvertHelper.GetDataModel(DataType.Failure, ex.Message);
            }
        }
    }
}
