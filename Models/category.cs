using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class category
    {
        public int catid{get;set;}
        public string catname { get; set; }
        public bool status { get; set; }
        public bool trash { get; set; }
        public DateTime subdate { get; set; }
        public DateTime trashed_on { get; set; }
        public string trashby { get; set; }
        public bool errflg { get; set; }
        public string errmsg { get; set; }
    }
}