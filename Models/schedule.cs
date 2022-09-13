using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class schedule
    {
        public int schedule_id { get; set; }
        public int vaccine_id { get; set; }
        public int country_id { get; set; }
        public int Year { get; set; }
        public int Due_Days { get; set; }
        public int Due_Months { get; set; }
        public int Due_Years { get; set; }
        public int End_Days { get; set; }
        public int End_Months { get; set; }
        public int End_Years { get; set; }
        public int Dose_No { get; set; }
        public string Dose_Name { get; set; }
        public bool Set_as_Previous_Given { get; set; }
        public bool Booster { get; set; }
        public string Booster_Doses { get; set; }
        public string Dose_Desc { get; set; }
        public bool NotCompulsary { get; set; }
        public int stateid { get; set; }
        public bool No_Due_Date { get; set; }
    }

    public class VRSchedulelist : List<schedule>
    {
    
    }
}