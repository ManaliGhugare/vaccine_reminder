using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vacrem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.WebPages.Html;

namespace vacrem.Controllers
{
    public class CounstateController : ApiController
    {
        // GET api/counstate
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/counstate/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/counstate
        public void Post([FromBody]string value)
        {
        }

        // PUT api/counstate/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/counstate/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        public countrylst getcountry()
        {
            countrylst clst = new countrylst();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                con.Open();
                
                try
                {
                    SqlCommand cmd = new SqlCommand("select * from VR_Country where active_flag='true'", con);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            DataTable dt = new DataTable();
                            dt.Load(sdr);
                          
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                country cn = new country();
                                cn.coun_id = Convert.ToInt32(dt.Rows[i].ItemArray[0].ToString());
                                cn.coun_name =(dt.Rows[i].ItemArray[1].ToString());
                                cn.active_flag = Convert.ToBoolean(dt.Rows[i].ItemArray[2].ToString());
                                clst.Add(cn);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                 
                }
                finally {
                
                    con.Close();
                }
            }
            return clst;
        }

        [HttpGet]
        public statelst getstate(int counid)
        {
            statelst clst = new statelst();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                con.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("select * from VR_Country_States where coun_id=@cid", con);
                    cmd.Parameters.AddWithValue("cid",counid.ToString());
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            DataTable dt = new DataTable();
                            dt.Load(sdr);

                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                state ste = new state();
                                ste.stateid = Convert.ToInt32(dt.Rows[i].ItemArray[0].ToString());
                                ste.state_name = (dt.Rows[i].ItemArray[2].ToString());
                                ste.coun_id = Convert.ToInt32(dt.Rows[i].ItemArray[1].ToString());
                                clst.Add(ste);
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
                finally
                {

                    con.Close();
                }
            }
            return clst;
        }

        [HttpGet]
        public List<SelectListItem> populatecount()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from VR_Country where active_flag='true'", con);
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {

                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["coun_name"].ToString(),
                                    Value = sdr["coun_id"].ToString()
                                });
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    items.Add(new SelectListItem { Text = e.Message,Value="0"}
                        );
                    
                }
                finally {
                    con.Close();
                }
            }
            return items;
        }

        [HttpGet]
        public List<SelectListItem> populatestate(int counid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("select * from VR_Country_States where coun_id=@cid", con);
                    cmd.Parameters.AddWithValue("cid", counid.ToString());
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if (sdr.HasRows)
                        {
                            while (sdr.Read())
                            {
                                items.Add(new SelectListItem
                                {
                                    Text = sdr["state_name"].ToString(),
                                    Value = sdr["stateid"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    items.Add(new SelectListItem { Text = e.Message, Value = "0" });
                }
                finally {
                    con.Close();
                }
            }

            return items;
        }

        #region ======================================private method======================================================

        public static country Filldatarecord(IDataReader myDataRecord)
        {
            country cnt = new country();

            cnt.coun_id = (myDataRecord.GetInt32(myDataRecord.GetOrdinal("coun_id")));
            cnt.coun_name = myDataRecord.GetString(myDataRecord.GetOrdinal("coun_name"));
            cnt.active_flag = myDataRecord.GetBoolean(myDataRecord.GetOrdinal("active_flag"));
            return cnt;
        }

        public static byte[] ConvertInt32ToByteArray(Int32 I32)
        {
            return BitConverter.GetBytes(I32);
        }


        #endregion

    }
}
