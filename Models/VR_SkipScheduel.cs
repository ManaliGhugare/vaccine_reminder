using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class VR_SkipScheduel
    {
        public int skipid{get; set;}
        public int schedule_id{get; set;}
        public int vaccine_id{get; set;}
        public int child_id{get; set;}
        public string from_table{get; set;}
        public DateTime skipdate{get; set;}
        public int Dose_No{get; set;}
    }

    public class VR_SkipSchlist : List<VR_SkipScheduel>
    {
        public VR_SkipSchlist()
        { }
    }
}