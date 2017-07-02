using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DESCore.Models;

namespace DESCore.Helpers
{
    public class ConvertHelper
    {
        private static Random random = new Random();

        public static Device GetDevice(byte[] data)
        {
            Device device = new Device
            {
                DeviceId = BitConverter.ToUInt32(data, 0).ToString("X"),
                PhoneNumber = Encoding.Default.GetString(data, 4, 11).Trim(),
                IpAddress = data[16] + "." + data[17] + "." + data[18] + "." + data[19],
            };
            return device;
        }

        public static byte[] Encoder(byte[] data)
        {
            try
            {
                List<byte> listByte = new List<byte>();
                foreach (byte b in data)
                {
                    if (b == 0xfe)
                    {
                        listByte.Add(0xfd);
                        listByte.Add(0xee);
                    }
                    else if (b == 0xfd)
                    {
                        listByte.Add(0xfd);
                        listByte.Add(0xed);
                    }
                    else listByte.Add(b);
                }
                return listByte.ToArray();
            }
            catch { }
            return data;
        }

        public static byte[] Decoder(byte[] data)
        {
            try
            {
                List<byte> listByte = new List<byte>();
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] == 0xfd && i < data.Length - 1)
                    {
                        if (data[i] == 0xed) { listByte.Add(0xfd); i++; }
                        else if (data[i] == 0xee) { listByte.Add(0xfe); i++; }
                        else listByte.Add(data[i]);
                    }
                    else if (data[i] != 0xfe) listByte.Add(data[i]);
                }
                return listByte.ToArray();
            }
            catch { }
            return data;
        }

        public static string GetString(IEnumerable<byte> data)
        {
            try
            {
                if (data == null) return "";
                List<string> listData = new List<string>();
                foreach (byte b in data) listData.Add(b.ToString("X2"));
                return string.Join(" ", listData);
            }
            catch { }
            return "";
        }

        public static string GetTagColor(int index)
        {
            if (index == 0) return "#ffffff";
            else if (index == 1) return "#000000";
            else if (index == 2) return "#000000";
            else if (index == 3) return "#000000";
            else if (index == 4) return "#000000";
            else if (index == 5) return "#ffffff";
            else if (index == 6) return "#ffffff";
            return "#ffffff";
        }

        public static string GetTagBKColor(int index)
        {
            if (index == 0) return "#0000ff";
            else if (index == 1) return "#ffffff";
            else if (index == 2) return "#ffa500";
            else if (index == 3) return "#00ff00";
            else if (index == 4) return "#ffff00";
            else if (index == 5) return "#808080";
            else if (index == 6) return "#ff0000";
            return "#000000";
        }

        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] GetBytes(string data)
        {
            string[] words = data.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[words.Length];
            for (var i = 0; i < words.Length; i++) bytes[i] = Convert.ToByte(words[i], 16);
            return bytes;
        }

        public static byte[] GetBytes(DataType dataType)
        {
            return GetBytes<object>(dataType, null);
        }

        public static byte[] GetBytes<T>(DataType dataType, T data)
        {
            try
            {
                byte[] bytes = XmlHelper.GetBytes(GetDataModel(dataType, data));
                if (bytes == null) return null;

                string strData = Convert.ToBase64String(bytes);
                return Encoding.UTF8.GetBytes("{" + strData + "}");
            }
            catch { }
            return null;
        }

        public static DataModel GetDataModel<T>(DataType dataType, T data)
        {
            try
            {
                if (data != null)
                {
                    byte[] bytes = XmlHelper.GetBytes(data);
                    if (bytes != null) return new DataModel { DataType = dataType, Data = Convert.ToBase64String(bytes), };
                }
            }
            catch { }
            return new DataModel { DataType = dataType, };
        }

        public static T GetObject<T>(DataModel dataModel)
        {
            try
            {
                if (dataModel.Data != null)
                {
                    byte[] bytes = Convert.FromBase64String(dataModel.Data);
                    if (bytes != null) return XmlHelper.GetObject<T>(bytes);
                }
            }
            catch { }
            return default(T);
        }
    }
}
