using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class VR_ReminderSettingVO
    {

        public int vacreminderid { get; set; }
        public string userid { get; set; }
        public int rem1_daybefore { get; set; }
        public int rem2_daybefore { get; set; }
        public bool send_email { get; set; }
        public bool send_sms { get; set; }
        public bool send_rem2 { get; set; }
        public DateTime subdate { get; set; }
        public bool status { get; set; }
        public int orangedaysetting { get; set; }
        public bool parent_send_mail { get; set; }
        public bool parent_send_sms { get; set; }
        public string time_zone { get; set; }
        public string date_format { get; set; }
        public string Gmt_TimeZone { get; set; }
    }

    public class VR_ReminderSettingList : List<VR_ReminderSettingVO>
    {
        public VR_ReminderSettingList()
        {
        }
    }
}