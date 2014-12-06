using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    static class YoutubeUtil
    {
        static public string getYoutubeIdFromURL(string url)
        {
            string id;
            if (url.Contains("youtu.be"))
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
    }
}
