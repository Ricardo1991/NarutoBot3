﻿using NarutoBot3.Properties;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace NarutoBot3
{
    internal static class YoutubeUseful
    {
        public static string GetYoutubeInfoFromID(string id)
        {
            Youtube.YoutubeVideoInfo youtubeVideo = new Youtube.YoutubeVideoInfo();

            string getString = "https://www.googleapis.com/youtube/v3/videos/" + "?key=" + Settings.Default.apikey + "&part=snippet,contentDetails,statistics" + "&id=" + id;

            var webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            webClient.Headers.Add("User-agent", Settings.Default.UserAgent);
            try
            {
                string jsonYoutube = webClient.DownloadString(getString);
                JsonConvert.PopulateObject(jsonYoutube, youtubeVideo);

                string title = WebUtility.HtmlDecode(youtubeVideo.items[0].snippet.title);
                string duration = ParseDuration(youtubeVideo.items[0].contentDetails.duration);

                return "\x02" + "\x031,0You" + "\x030,4Tube" + "\x03 Video: " + title + " [" + duration + "]\x02";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetYoutubeIdFromURL(string url)
        {
            string id;

            if (url.ToLower().Contains("youtu.be") && !url.ToLower().Contains("&feature"))
            {
                id = Useful.GetBetween(url, "youtu.be/", "?");
            }
            else
            {
                if (url.ToLower().Contains("?v="))
                    id = Useful.GetBetween(url, "?v=", "&");
                else
                    id = Useful.GetBetween(url, "&v=", "&");
            }

            return id.Split(new char[] { ' ', '\\', '"' }, 2)[0];
        }

        public static string ParseDuration(string duration)//PT#H#M#S
        {
            string temp = "";
            int hours = 0, minutes = 0, seconds = 0;

            duration = duration.Replace("PT", string.Empty);

            for (int i = 0; i < duration.Length; i++)
            {
                if (duration[i] != 'H' && duration[i] != 'M' && duration[i] != 'S')
                {
                    temp = temp + duration[i];
                }
                else
                    switch (duration[i])
                    {
                        case 'H':
                            hours = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;

                        case 'M':
                            minutes = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;

                        case 'S':
                            seconds = Convert.ToInt32(temp);
                            temp = string.Empty;
                            break;
                    }
            }

            if (hours > 0)
                return hours.ToString() + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            else
                return minutes.ToString() + ":" + seconds.ToString("00");
        }
    }
}