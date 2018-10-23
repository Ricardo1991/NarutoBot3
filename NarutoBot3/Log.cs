using System;
using System.IO;

namespace NarutoBot3
{
    internal class Log
    {
        internal static void Error(string v, Exception ex)
        {
            using (StreamWriter w = File.AppendText("ExceptionLog.txt"))
            {
                WriteLog(v, ex, w);
            }
        }

        private static void WriteLog(string logMessage, Exception ex, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("  :{0}", ex.Message);
            w.WriteLine("-------------------------------");
        }
    }
}