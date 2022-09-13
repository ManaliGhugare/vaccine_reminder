using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vacrem.Models
{
    public class vaccine
    {
        public int vaccine_id { get; set; }
        public int catid { get; set; }
        public string catname { get; set; }
        public string vaccine_name { get; set; }
        public DateTime subdate { get; set; }
        public bool status { get; set; }
        public bool Boys_Vaccine { get; set; }
        public bool Girls_Vaccine { get; set; }
        public string short_vacname { get; set; }
        public string trashby { get; set; }
        public DateTime trashed_on { get; set; }
        public bool trash { get; set; }
        public bool errflg { get; set; }
        public string resultmsg { get; set; }

        public List<SelectListItem> category { get; set; }
    }

    public class vaccinelist 
    {
        public List<vaccine> vaclist { get; set; }
        public bool errlstflg { get; set; }
        public string lstresultmsg { get; set; }
    }

    public class vaclistclass : List<vaccine>
    {
    }
}