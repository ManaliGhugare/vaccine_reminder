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
    public class VR_SkipschDAController : ApiController
    {
        // GET api/vr_skipschda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vr_skipschda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vr_skipschda
        public void Post([FromBody]string value)
        {
        }

        // PUT api/vr_skipschda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vr_skipschda/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_SkipSchlist gridviewchidschiddsnotbltypelist(int childid, int schid, int doseno, int vacid, string tbltype)
        {
            VR_SkipSchlist ch = new VR_SkipSchlist();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                //SqlCommand cmd = new SqlCommand("VRSkipchidschiddosenotbltype", con);
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Add("@child_id", childid);
                //cmd.Parameters.Add("@schedule_id", schid);
                //cmd.Parameters.Add("@Dose_No", doseno);
                //cmd.Parameters.Add("@vaccine_id", vacid);
                //cmd.Parameters.Add("@from_table", tbltype);
                //con.Open();
                //try
                //{
                //    using (SqlDataReader rd = cmd.ExecuteReader())
                //    {
                //        if (rd.HasRows)
                //        {
                //            while (rd.Read())
                //            {
                //                ch.Add(FillDataRecord(rd));
                //            }
                //        }
                //        rd.Close();
                //    }
                //}
                //catch { throw; }
                //finally { con.Close(); }
            }
            return ch;
        }

        #region ===============================Private Methods ==============================================

        private static VR_SkipScheduel FillDataRecord(IDataReader myDataRecord)
        {
            VR_SkipScheduel h2 = new VR_SkipScheduel();

            h2.skipid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("skipid")));
            h2.schedule_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("schedule_id")));
            h2.vaccine_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vaccine_id")));
            h2.child_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("child_id")));
            h2.from_table = (myDataRecord.GetString(myDataRecord.GetOrdinal("from_table")));
            h2.Dose_No = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("Dose_No")));
            h2.skipdate = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("skipdate")));

            return h2;
        }

        #endregion 
    }
}
