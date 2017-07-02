using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace DESCore.Helpers
{
    public class LogHelper
    {
        private static string _logFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log";
        private static string lastMsg = "";
        private static object _lockForLog = new object();

        public static void Write(string msg)
        {
            try
            {
                if (msg == lastMsg) return;
                lastMsg = msg;

                StackFrame sf = new StackTrace().GetFrame(1);

                if (!Directory.Exists(_logFolder))
                {
                    Directory.CreateDirectory(_logFolder);
                }

                lock (_lockForLog)
                {
                    File.AppendAllLines(_logFolder + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt",
                        new string[] { DateTime.Now.ToString("HH:mm:ss") + " | " + msg + " | " + sf.GetMethod().Name});
                }
            }
            catch { }
        }
    }
}
