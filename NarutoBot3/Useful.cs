using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NarutoBot3
{
    public static class Useful
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string RemoveUserMode(string user)
        {
            char[] usermodes = { '@', '+', '%', '~', '&' };

            if (usermodes.Any((s) => Convert.ToChar(user.Substring(0, 1)).Equals(s)))
                return user.Substring(1).Trim();
            else return user.Trim();
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static char GetUserMode(string user)
        {
            if (string.IsNullOrWhiteSpace(user))
            {
                return '0';
            }
            switch (user[0])
            {
                case '@':
                case '+':
                case '%':
                case '~':
                case '&':
                    return user[0];

                default:
                    return '0';
            }
        }

        public static string GetTimestamp(DateTime value)
        {
            return value.ToString("mmssffff");
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;

            if (string.IsNullOrWhiteSpace(strSource)) return string.Empty;

            if (String.IsNullOrEmpty(strEnd))
            {
                if (String.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;

                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                if (String.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End < 0) End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                if (String.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
        }

        public static string FillTags(string template, string user, string target, UserList userlist)
        {
            var regex = new Regex(Regex.Escape("<random>"));
            Random r = new Random();
            string randomTarget;
            List<User> listU = userlist.GetAllOnlineUsers();

            template = template.Replace("<TARGET>", target.ToUpper()).Replace("<USER>", user.ToUpper());
            template = template.Replace("<target>", target).Replace("<user>", user);

            while (template.Contains("<random>"))
            {
                do
                {
                    randomTarget = listU[r.Next(listU.Count)].Nick;
                } while (string.Compare(target, randomTarget, true) == 0 || listU.Count < 2);

                template = regex.Replace(template, randomTarget, 1);
            }


            return template;
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            if (box == null) return;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }

    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

    public static class GetCompilationDate
    {
        static public DateTime RetrieveLinkerTimestamp()
        {
            string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            System.IO.Stream s = null;

            try
            {
                s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                s.Read(b, 0, 2048);
            }
            finally
            {
                if (s != null)
                {
                    s.Close();
                }
            }

            int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
            int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(secondsSince1970);
            dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
            return dt;
        }
    }
}