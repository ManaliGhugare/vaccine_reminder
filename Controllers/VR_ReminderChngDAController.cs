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
    public class VR_ReminderChngDAController : ApiController
    {
        // GET api/vr_reminderchngda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vr_reminderchngda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vr_reminderchngda
        public void Post([FromBody]string value)
        {
        }

        // PUT api/vr_reminderchngda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vr_reminderchngda/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_ReminderChangeslist gridviewscheduleid(string uid, int childid, int vacid, int schid, int dose_no, string schtype)
        {
            VR_ReminderChangeslist ch = new VR_ReminderChangeslist();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRRemdChng", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", uid);
                cmd.Parameters.Add("@child_id", childid);
                cmd.Parameters.Add("@vacid", vacid);
                cmd.Parameters.Add("@schid", schid);
                cmd.Parameters.Add("@dose_no", dose_no);
                cmd.Parameters.Add("@schtype", schtype);
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

        private static VR_ReminderChanges FillDataRecord(IDataReader myDataRecord)
        {
            VR_ReminderChanges h2 = new VR_ReminderChanges();

            h2.vacreminder_chngid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vacreminder_chngid")));
            h2.userid = (myDataRecord.GetString(myDataRecord.GetOrdinal("userid")));
            h2.child_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("child_id")));
            h2.vacid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vacid")));
            h2.schid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("schid")));
            h2.schtype = (myDataRecord.GetString(myDataRecord.GetOrdinal("schtype")));
            h2.dose_no = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("dose_no")));
            h2.reminder_number = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("reminder_number")));
            h2.reminder_date = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("reminder_date")));
            h2.subdate = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("subdate")));
            h2.status = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("status")));

            return h2;
        }

        #endregion
    }
}
