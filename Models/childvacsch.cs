using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class childvacsch
    {
        public string gender { get; set; }
        public int childid { get; set; }
        public int vacid { get; set; }
        public int catid { get; set; }
        public int schid { get; set; }
        public int doseno { get; set; }
        public int countryid { get; set; }
        public int stateid { get; set; }
        public int vaccineyear { get; set; }
        public int DueDays { get; set; }
        public int DueMonths { get; set; }
        public int DueYears { get; set; }
        public int EndDays { get; set; }
        public int EndMonth { get; set; }
        public int EndYear { get; set; }
        public DateTime DueOnDate { get; set; }
        public DateTime EndDueDate { get; set; }
        public DateTime ReminderOnDate { get; set; }
        public int givenid { get; set; }
        public DateTime DateGiven { get; set; }
        public int skipid { get; set; }
        public DateTime skipdate { get; set; }
        public bool set_as_previous_given { get; set; }
        public bool dosegivenornot { get; set; }
        public bool skipornot { get; set; }
        public bool notcompulsory { get; set; }
        public bool triggershowflag { get; set; }
        public bool nodueflag{ get; set; }
        public bool Booster { get; set; }
        public string BoosterDoses { get; set; }
        public string dosecolour { get; set; }
        public bool forcedgray { get; set; }
    }
}