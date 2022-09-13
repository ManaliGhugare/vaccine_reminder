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
    public class childapiController : ApiController
    {
        // GET api/childapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/childapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/childapi
        public void Post([FromBody]string value)
        {
        }

        // PUT api/childapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/childapi/5
        public void Delete(int id)
        {
        }

        //GET api/childapi/getallchild?parid=manali@gmail.com
       [HttpGet]
        public childlist getallchild(string parid)
        {

            childlist chlst = new childlist();
            string constr = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string query = "select * from vaccine_child_master where user_id=@uid";
                using (SqlCommand cmd = new SqlCommand("VRChildGetdata"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    cmd.Parameters.Add("@userid", parid);
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            chlst.Add(Filldatarecord(sdr));
                        }
                    }
                    con.Close();
                }
            }
            return chlst;
        }

        #region ======================================private method======================================================

        public static child Filldatarecord(IDataReader myDataRecord)
        {
            child h2 = new child();

            h2.child_id = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("child_id")));
            h2.child_name = myDataRecord.GetString(myDataRecord.GetOrdinal("child_name"));
            h2.dob = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("dob"));
            h2.sex = myDataRecord.GetString(myDataRecord.GetOrdinal("sex"));
            h2.address = myDataRecord.GetString(myDataRecord.GetOrdinal("address"));
            h2.email = myDataRecord.GetString(myDataRecord.GetOrdinal("email"));
            h2.mobile = myDataRecord.GetString(myDataRecord.GetOrdinal("mobile"));
            //  h2.refering_doctor = myDataRecord.GetString(myDataRecord.GetOrdinal("refering_doctor"));
            // h2.doctor_email = myDataRecord.GetString(myDataRecord.GetOrdinal("doctor_email"));
            h2.user_id = myDataRecord.GetString(myDataRecord.GetOrdinal("user_id"));
            h2.date_added = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("date_added"));
            h2.country = myDataRecord.GetString(myDataRecord.GetOrdinal("country"));
            h2.vaccine_email = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("vaccine_email"));
            h2.child_image = myDataRecord.GetString(myDataRecord.GetOrdinal("child_image"));
            h2.verifymobcode = myDataRecord.GetString(myDataRecord.GetOrdinal("verifymobcode"));
            h2.isverifymobcode = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("isverifymobcode"));
            h2.countrycode = myDataRecord.GetString(myDataRecord.GetOrdinal("countrycode"));
            h2.email_status = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("email_status"));
            h2.sms_status = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("sms_status"));
            h2.parent_email_status = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("parent_email_status"));
            h2.parent_sms_status = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("parent_sms_status"));
            h2.schedule_year = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("schedule_year")));
            h2.stateid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("stateid")));
            h2.resendtime = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("resendtime"));
            h2.resendcount = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("resendcount")));
            h2.Verify = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("Verify"));
            h2.email_resendtime = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("email_resendtime"));
            h2.email_resendcount = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("email_resendcount")));
            h2.reminder_sent_on = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("reminder_sent_on"));
            //   h2.error_on = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("error_on"));
            //  h2.vac_schedule = myDataRecord.GetString(myDataRecord.GetOrdinal("vac_schedule"));
            //  h2.date_schedule = myDataRecord.GetString(myDataRecord.GetOrdinal("date_schedule"));
            h2.red_count = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("red_count")));
            h2.orange_count = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("orange_count")));
            h2.gray_count = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("gray_count")));
            h2.green_count = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("green_count")));
            h2.last_generate_on = myDataRecord.GetDateTime(myDataRecord.GetOrdinal("last_generate_on"));
            h2.ScheduleObject = ConvertInt32ToByteArray(myDataRecord.GetOrdinal("ScheduleObject"));

            return h2;
        }

        public static byte[] ConvertInt32ToByteArray(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }

        #endregion

    }
}
