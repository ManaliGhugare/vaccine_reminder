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
    public class VR_ReminderSettingDAController : ApiController
    {
        // GET api/vr_remindersettingda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vr_remindersettingda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vr_remindersettingda
        public void Post([FromBody]string value)
        {
        }

        // PUT api/vr_remindersettingda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vr_remindersettingda/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_ReminderSettingList gridviewuid(string uid, string status)
        {
            VR_ReminderSettingList ch = new VR_ReminderSettingList();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRRemdsettinguid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@userid", uid);
                cmd.Parameters.Add("@status", status);
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

        private static VR_ReminderSettingVO FillDataRecord(IDataReader myDataRecord)
        {
            VR_ReminderSettingVO h2 = new VR_ReminderSettingVO();

            h2.vacreminderid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vacreminderid")));
            h2.userid = (myDataRecord.GetString(myDataRecord.GetOrdinal("userid")));
            h2.rem1_daybefore = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("rem1_daybefore")));
            h2.rem2_daybefore = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("rem2_daybefore")));
            h2.send_email = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("send_email")));
            h2.send_sms = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("send_sms")));
            h2.send_rem2 = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("send_rem2")));
            h2.subdate = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("subdate")));
            h2.status = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("status")));
            h2.orangedaysetting = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("orangedaysetting")));
            h2.parent_send_mail = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("parent_send_mail")));
            h2.parent_send_sms = (myDataRecord.GetBoolean(myDataRecord.GetOrdinal("parent_send_sms")));
            h2.time_zone = (myDataRecord.GetString(myDataRecord.GetOrdinal("time_zone")));
            h2.date_format = (myDataRecord.GetString(myDataRecord.GetOrdinal("date_format")));
            h2.Gmt_TimeZone = (myDataRecord.GetString(myDataRecord.GetOrdinal("Gmt_TimeZone")));

            return h2;
        }

        #endregion
    }
}
