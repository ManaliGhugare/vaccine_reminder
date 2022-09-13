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
    public class schedulelstController : ApiController
    {
        // GET api/schedulelst
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/schedulelst/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/schedulelst
        public void Post([FromBody]string value)
        {
        }

        // PUT api/schedulelst/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/schedulelst/5
        public void Delete(int id)
        {
        }


        //Get api/schedulelst/gridviewvacidcountid/
        [HttpGet]
        public static VRSchedulelist gridviewvacidcountid(int vaccineid, int countryid, int stateid, int year)
        {
            VRSchedulelist vs = new VRSchedulelist();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("gridviewvacidcountid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@vaccine_id", vaccineid);
                cmd.Parameters.Add("@country_id", countryid);
                cmd.Parameters.Add("@stateid", stateid);
                cmd.Parameters.Add("@Year", year);
                con.Open();
                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                vs.Add(Filldatarecord(rd));
                            }
                        }
                        rd.Close();
                    }
                }
                catch { throw; }
                finally { con.Close(); }
            }
            return vs;
        }

        //Get api/schedulelst/gridviewvacidcountid/
        [HttpGet]
        public static VRSchedulelist gridviewvacschdoseno(int schid, int vaccineid, int doseno, int countryid, int stateid, int year)
        {
            VRSchedulelist vs = new VRSchedulelist();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("gridviewvacschdoseno", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@schedule_id", schid);
                cmd.Parameters.Add("@vaccine_id", vaccineid);
                cmd.Parameters.Add("@Dose_No", doseno);
                cmd.Parameters.Add("@country_id", countryid);
                cmd.Parameters.Add("@stateid", stateid);
                cmd.Parameters.Add("@Year", year);
                con.Open();
                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                vs.Add(Filldatarecord(rd));
                            }
                        }
                        rd.Close();
                    }
                }
                catch { throw; }
                finally { con.Close(); }
            }
            return vs;
        }

        public static schedule Filldatarecord(IDataReader myDataRecord)
        {
            schedule vr = new schedule();

            vr.schedule_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("schedule_id")));
            vr.vaccine_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("vaccine_id")));
            vr.country_id = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("country_id")));
            vr.Year = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("Year")));
            vr.Due_Days = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("Due_Days")));
            vr.Due_Months = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("Due_Months")));
            vr.Due_Years = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("Due_Years")));
            vr.End_Days = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("End_Days")));
            vr.End_Months = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("End_Months")));
            vr.End_Years = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("End_Years")));
            vr.Dose_No = Convert.ToInt32(myDataRecord.GetInt32(myDataRecord.GetOrdinal("Dose_No")));
            vr.Dose_Name = myDataRecord.GetString(myDataRecord.GetOrdinal("Dose_Name"));
            vr.Set_as_Previous_Given = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("Set_as_Previous_Given"));
            vr.Booster = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("Booster"));
            vr.Booster_Doses = myDataRecord.GetString(myDataRecord.GetOrdinal("Booster_Doses"));
            vr.Dose_Desc = myDataRecord.GetString(myDataRecord.GetOrdinal("Dose_Desc"));
            vr.NotCompulsary = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("NotCompulsary"));
            vr.stateid = Convert.ToInt32(myDataRecord.GetDecimal(myDataRecord.GetOrdinal("stateid")));
            vr.No_Due_Date = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("No_Due_Date"));

            return vr;
        }
    }
}
