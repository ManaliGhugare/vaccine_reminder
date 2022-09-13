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
    public class VR_GivenDAController : ApiController
    {
        // GET api/vr_givenda
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vr_givenda/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/vr_givenda
        public void Post([FromBody]string value)
        {
        }

        // PUT api/vr_givenda/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vr_givenda/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public static VR_GivenList gridviewchidschiddsnotbltypelist(int childid, int schid, int doseno, string tbltype)
        {
            VR_GivenList ch = new VR_GivenList();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRGivenchidschiddosenotbltype", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@child_id", childid);
                cmd.Parameters.Add("@schedule_id", schid);
                cmd.Parameters.Add("@Dose_No", doseno);
                cmd.Parameters.Add("@from_table", tbltype);
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

        [HttpGet]
        public static VR_GivenList gridviewchidschidtbltypelist(int childid, int schid, string tbltype)
        {
            VR_GivenList ch = new VR_GivenList();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("VRGivenchidschidtbltype", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@child_id", childid);
                cmd.Parameters.Add("@schedule_id", schid);
                cmd.Parameters.Add("@from_table", tbltype);
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

        private static vr_given FillDataRecord(IDataReader myDataRecord)
        {
            vr_given h2 = new vr_given();

            h2.givenid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("givenid")));
            h2.schedule_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("schedule_id")));
            h2.child_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("child_id")));
            h2.from_table = (myDataRecord.GetString(myDataRecord.GetOrdinal("from_table")));
            h2.Dose_No = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("Dose_No")));
            h2.given_date = (myDataRecord.GetDateTime(myDataRecord.GetOrdinal("given_date")));
            // h2.brand_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("brand_id")));
            //h2.stock_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("stock_id")));
            return h2;
        }

        #endregion
    }
}
