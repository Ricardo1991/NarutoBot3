using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarutoBot3
{
    public class TimeZoneAPI
    {
        public int DSTOffset { get; set; }
        public int RawOffset { get; set; }
        public string Status { get; set; }
        public string TimeZoneId { get; set; }
        public string TimeZoneName { get; set; }
    }
}
