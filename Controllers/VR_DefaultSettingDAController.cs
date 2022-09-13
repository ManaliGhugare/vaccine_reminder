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
    public class VR_DefaultSettingDAController : ApiController
    {
        // GET api/vr_defaultsettingda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vr_defaultsettingda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vr_defaultsettingda
        public void Post([FromBody]string value)
        {
        }

        // PUT api/vr_defaultsettingda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vr_defaultsettingda/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_DefaultSettingLsit gridview()
        {
            VR_DefaultSettingLsit ch = new VR_DefaultSettingLsit();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRRemdDefaultSetting", con);
                cmd.CommandType = CommandType.StoredProcedure;
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

        private static VR_DefaultSetting FillDataRecord(IDataReader myDataRecord)
        {
            VR_DefaultSetting h2 = new VR_DefaultSetting();

            h2.defaultvacremid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("defaultvacremid")));
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
            h2.version_no = (myDataRecord.GetString(myDataRecord.GetOrdinal("version_no")));

            return h2;
        }

        #endregion

    }
}
