using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vacrem.Models
{
    public class Employee
    {
        public int empid{get;set;}
        public string ename{get;set;}
        public string address{get;set;}
        public decimal salary{get;set;}
        public string mobileno{get;set;}
        public int deptid { get; set; }
        public string deptname { get; set; }
    }

    public class emplist : List<Employee>
    {
        public emplist() { 
        
        }
    }
}