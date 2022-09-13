using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class country
    {
        public int coun_id{get;set;}
        public string coun_name{get;set;}
        public bool active_flag { get; set; }
    }

    public class countrylst : List<country>
    {
       public countrylst()
        {

        }
    }

    public class state {
        public int stateid { get; set; }
        public int coun_id { get; set; }
        public string state_name { get; set; }
    }

    public class statelst : List<state>
    {
        public statelst()
        { 
        
        }
    }

}