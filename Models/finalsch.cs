using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    [Serializable()]
    public class finalsch
    {
        public int redcount;
        public int greencount;
        public int graycount;
        public int orangecount;
        public int prev_given;
        public List<childvacsch> schlist;
    }
}