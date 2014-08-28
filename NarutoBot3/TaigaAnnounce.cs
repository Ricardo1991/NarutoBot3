using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{

    class TaigaAnnounce
    {
        
        string user="";

        public string User
        {
            get { return user; }
            set { user = value; }
        }
        string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        int ep=0;

        public int Ep
        {
            get { return ep; }
            set { ep = value; }
        }
        int total=0;

        public int Total
        {
            get { return total; }
            set { total = value; }
        }
        int score=0;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        string picurl = "";

        public string Picurl
        {
            get { return picurl; }
            set { picurl = value; }
        }
        string playstatus = "";

        public string Playstatus
        {
            get { return playstatus; }
            set { playstatus = value; }
        }


        //User=Ricardo1991&Name=Toradora!&Ep=1&eptotal=25&Score=10&Picurl=http://cdn.myanimelist.net/images/anime/5/22125.jpg&Playstatus=stopped
        public TaigaAnnounce(string[] bodySplit)
        {
            foreach (string s in bodySplit)
            {
                string[] split = s.Split('=');

                switch (split[0])
                { 
                    case("user"):
                        User = split[1];
                        break;
                    case ("name"):
                        Name = split[1];
                        break;
                    case ("ep"):
                        Ep = Convert.ToInt32(split[1]);
                        break;
                    case ("eptotal"):
                        Total = Convert.ToInt32(split[1]);
                        break;
                    case ("score"):
                        if(split[1]!="")
                        Score = Convert.ToInt32(split[1]);
                        break;
                    case ("picurl"):
                        Picurl = split[1];
                        break;
                    case ("playstatus"):
                        Playstatus = split[1];
                        break;

                    default:
                        break;
                
                
                }
            
            
            }
        
        }
    }
}
