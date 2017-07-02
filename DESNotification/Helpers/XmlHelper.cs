using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace DESNotification.Helpers
{
    public class XmlHelper
    {
        public static T GetObject<T>(byte[] bytes)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream(bytes))
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
            catch { }
            return default(T);
        }

        public static byte[] GetBytes<T>(T data)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new MemoryStream())
                {
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream))
                    {
                        serializer.Serialize(xmlWriter, data);
                        return stream.ToArray();
                    }
                }
            }
            catch { }
            return null;
        }
    }
}
