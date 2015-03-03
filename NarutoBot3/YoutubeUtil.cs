using System;

namespace NarutoBot3
{
    static class YoutubeUtil
    {
        static public string getYoutubeIdFromURL(string url)
        {
            string id;
            if (url.Contains("youtu.be") && !url.Contains("&feature=youtu.be"))
            {
                id = util.getBetween(url, "youtu.be/", "?t");
            
            }
            else
            {
                if (url.Contains("?v="))
                    id = util.getBetween(url, "?v=", "&");
                else
                    id = util.getBetween(url, "&v=", "&");
            }

            return id.Split(new char[] { ' ' }, 2)[0];
        }
        static public string parseDuration(string duration)//PT#H#M#S
        {
            string temp="";
            int hours = 0, minutes = 0, seconds = 0;

            duration = duration.Replace("PT", string.Empty);

            for (int i = 0; i < duration.Length; i++)
            {
                if (duration[i] != 'H' && duration[i] != 'M' && duration[i] != 'S')
                {
                    temp = temp + duration[i];
                
                }
                else
                    switch(duration[i]){
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

            if(hours > 0)
                return hours.ToString() + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
            else
                return minutes.ToString() + ":" + seconds.ToString("00");
        }
    }
}
