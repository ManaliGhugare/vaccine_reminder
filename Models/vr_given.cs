using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class vr_given
    {
        public int givenid { get; set; }
        public int schedule_id { get; set; }
        public int child_id { get; set; }
        public string from_table { get; set; }
        public DateTime given_date { get; set; }
        public int Dose_No { get; set; }
        public int brand_id { get; set; }
        public int stock_id { get; set; }
    }

    public class VR_GivenList : List<vr_given>
    {
        public VR_GivenList()
        { 
        }
    }

}