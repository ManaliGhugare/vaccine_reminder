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
    public class triggerscheduleController : ApiController
    {
        // GET api/triggerschedule
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/triggerschedule/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/triggerschedule
        public void Post([FromBody]string value)
        {
        }

        // PUT api/triggerschedule/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/triggerschedule/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_TriggerList gridviewTriggerCidVacid(int cid, int vacid)
        {
            VR_TriggerList ch = new VR_TriggerList();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["pediatriconcalluserConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRTriggerdataCidVacid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@vaccine_id", vacid);
                cmd.Parameters.Add("@country_id", cid);
                con.Open();
                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                ch.Add(FillDataRecord(rd));
                            }
                        }
                        rd.Close();
                    }
                }
                catch { throw; }
                finally { con.Close(); }
            }
            return ch;
        }

        #region ===============================Private Methods ==============================================

        private static triggersch FillDataRecord(IDataReader myDataRecord)
        {
            triggersch h2 = new triggersch();

            h2.triggerschid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("triggerschid")));
            h2.vaccine_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vaccine_id")));
            h2.country_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("country_id")));
            h2.stateid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("stateid")));
            h2.show_vaccine = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("show_vaccine")));
            h2.end_age = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("end_age")));
            h2.subdate = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("subdate")));
            h2.NoDue = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("NoDue")));

            return h2;
        }

        #endregion

    }
}
