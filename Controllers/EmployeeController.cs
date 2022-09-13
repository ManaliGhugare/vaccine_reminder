using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vacrem.Models;

namespace vacrem.Controllers
{
    public class EmployeeController : ApiController
    {
        string str = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;

        // GET api/employee
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/employee/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        public string Getmystring(int id)
        {
            return "value";
        }

        // POST api/employee
        public void Post([FromBody]string value)
        {
        }

        // PUT api/employee/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/employee/5
        public void Delete(int id)
        {
        }



        // api/employee/getdata/
        [HttpGet]
        public emplist getdata(string empid)
        {
            emplist lst = new emplist();
            int deptid;

            if (empid == "")
            {
                deptid = 0;
            }
            else
            {
                deptid = Convert.ToInt32(empid);
            }


            using (SqlConnection con = new SqlConnection(str))
            {
                con.Open();
                SqlTransaction tr = con.BeginTransaction();
                SqlCommand cmd = new SqlCommand("getempdata1",con,tr);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@did", deptid);
                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                lst.Add(FillDataRecord(rd));
                            }
                        }
                        rd.Close();
                    }
                }
                catch { throw; }
                finally
                {
                    tr.Commit();
                    con.Close();
                }
            }
            return lst;
        }

        #region ===============================Private Methods ==============================================

        private static Employee FillDataRecord(IDataReader myDataRecord)
        {
            Employee h2 = new Employee();
            h2.empid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("empid")));
            h2.ename = (myDataRecord.GetString(myDataRecord.GetOrdinal("ename")));
            h2.salary = myDataRecord.GetDecimal(myDataRecord.GetOrdinal("salary"));
            h2.deptname = myDataRecord.GetString(myDataRecord.GetOrdinal("deptname"));
            h2.deptid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("depid")));
            return h2;
        }

        #endregion
    }
}
