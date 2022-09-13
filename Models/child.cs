using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace vacrem.Models
{
    public class child
    {
        public int child_id { get; set; }
        public string child_name { get; set; }
        public DateTime dob { get; set; }
        public string sex { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string user_id { get; set; }
        public string address { get; set; }
        public DateTime date_added { get; set; }
        public string country { get; set; }
        public bool vaccine_email { get; set; }
        public string child_image { get; set; }
        public string verifymobcode { get; set; }
        public bool isverifymobcode { get; set; }
        public string countrycode { get; set; }
        public bool email_status { get; set; }
        public bool sms_status{ get; set; }
        public bool parent_email_status { get; set; }
        public bool parent_sms_status { get; set; }
        public int schedule_year { get; set; }
        public int stateid { get; set; }
        public DateTime resendtime { get; set; }
        public int resendcount { get; set; }
        public bool Verify { get; set; }
        public DateTime email_resendtime { get; set; }
        public int email_resendcount { get; set; }
        public DateTime reminder_sent_on { get; set; }
        public int red_count { get; set; }
        public int orange_count { get; set; }
        public int gray_count { get; set; }
        public int green_count { get; set; }
        public DateTime last_generate_on { get; set; }
        public byte[] ScheduleObject { get; set; }
        public bool customsch { get; set; }
    }
}