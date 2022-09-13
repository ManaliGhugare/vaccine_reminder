using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class VR_ReminderChanges
    {
        public int vacreminder_chngid { get; set; }
        public string userid{ get; set; }
        public int child_id{ get; set; }
        public int vacid{ get; set; }
        public int schid{ get; set; }
        public string schtype{ get; set; }
        public int dose_no{ get; set; }
        public int reminder_number{ get; set; }
        public DateTime reminder_date{ get; set; }
        public DateTime subdate{ get; set; }
        public bool status{ get; set; }
    }

    public class VR_ReminderChangeslist : List<VR_ReminderChanges>
    {
        public VR_ReminderChangeslist()
        { 
        
        }
    }
}