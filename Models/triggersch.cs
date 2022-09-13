using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class triggersch
    {
        public int triggerschid { get; set; }
        public int vaccine_id { get; set; }
        public int country_id { get; set; }
        public int stateid { get; set; }
        public bool show_vaccine { get; set; }
        public bool NoDue { get; set; }
        public int end_age { get; set; }
        public DateTime subdate { get; set; }
    }

    public class VR_TriggerList : List<triggersch>
    { 
    
    }
}