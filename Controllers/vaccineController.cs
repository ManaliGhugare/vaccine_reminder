using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web; 
using System.Net.Http;
using System.Web.Http;
using vacrem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;

namespace vacrem.Controllers
{
    public class vaccineController : ApiController
    {
        string str = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;

        // GET api/vaccine
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/vaccine/Get/
        public int Get(int id)
        {
            int a = 2 / id;
            return a;
        }

        // POST api/vaccine/Post
        public string Post([FromBody]string value)
        {
            return value;
        }

        // PUT api/vaccine/Put/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/vaccine/Delete/5
        public void Delete(int id)
        {
        }


        // My code start 
        //GET  api/vaccine/show
        [HttpGet]
        public vaccinelist show()
        {
            vaccinelist lst = new vaccinelist();
          
            List<vaccine> lst1 = new List<vaccine>();
            try
            {
                SqlDataSource sqlvaclst = new SqlDataSource();
                sqlvaclst.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["vacrem"].ToString();
                sqlvaclst.SelectCommand = "select vaccine_id,vaccine_name,status,Boys_Vaccine,Girls_Vaccine,short_vacname,(select catname from VR_Category as c where c.catid=v.catid) as catname,catid from VR_Vaccines as v where trash='false'";
                sqlvaclst.SelectParameters.Clear();
                DataView dvvaclst = (DataView)sqlvaclst.Select(DataSourceSelectArguments.Empty);
                DataTable dtvaclst = dvvaclst.Table;
                for (int i = 0; i < dtvaclst.Rows.Count; i++)
                {
                    vaccine vr = new vaccine();
                    vr.vaccine_id = Convert.ToInt32(dtvaclst.Rows[i].ItemArray[0].ToString());
                    vr.vaccine_name = dtvaclst.Rows[i].ItemArray[1].ToString();
                    vr.status = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[2].ToString());
                    vr.Boys_Vaccine = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[3].ToString());
                    vr.Girls_Vaccine = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[4].ToString());
                    vr.short_vacname = (dtvaclst.Rows[i].ItemArray[5].ToString());
                    vr.catname = (dtvaclst.Rows[i].ItemArray[6].ToString());
                    vr.catid = Convert.ToInt32(dtvaclst.Rows[i].ItemArray[7].ToString());
                    lst1.Add(vr);
                }
                lst.vaclist = lst1;
                lst.errlstflg = false;
                lst.lstresultmsg ="Data Bind successfully.";
            }
            catch (Exception e)
            {
                lst.errlstflg = true;
                lst.lstresultmsg = e.Message;
            }
            return lst;
        }


      //  api/vaccine/show1
        [HttpGet]
        public ArrayList show1()
        {
            ArrayList lstarr = new ArrayList();

            List<vaccine> lst1 = new List<vaccine>();
            try
            {
                SqlDataSource sqlvaclst = new SqlDataSource();
                sqlvaclst.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["vacrem"].ToString();
                sqlvaclst.SelectCommand = "select vaccine_id,vaccine_name,status,Boys_Vaccine,Girls_Vaccine,short_vacname,(select catname from VR_Category as c where c.catid=v.catid) as catname,catid from VR_Vaccines as v where trash='false'";
                sqlvaclst.SelectParameters.Clear();
                DataView dvvaclst = (DataView)sqlvaclst.Select(DataSourceSelectArguments.Empty);
                DataTable dtvaclst = dvvaclst.Table;
                for (int i = 0; i < dtvaclst.Rows.Count; i++)
                {
                    vaccine vr = new vaccine();
                    vr.vaccine_id = Convert.ToInt32(dtvaclst.Rows[i].ItemArray[0].ToString());
                    vr.vaccine_name = dtvaclst.Rows[i].ItemArray[1].ToString();
                    vr.status = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[2].ToString());
                    vr.Boys_Vaccine = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[3].ToString());
                    vr.Girls_Vaccine = Convert.ToBoolean(dtvaclst.Rows[i].ItemArray[4].ToString());
                    vr.short_vacname = (dtvaclst.Rows[i].ItemArray[5].ToString());
                    vr.catname = (dtvaclst.Rows[i].ItemArray[6].ToString());
                    vr.catid = Convert.ToInt32(dtvaclst.Rows[i].ItemArray[7].ToString());
                    lst1.Add(vr);
                }
                
                lstarr.Add(lst1);
                lstarr.Add(false);
                lstarr.Add("Data Bind successfully.");
            }
            catch (Exception e)
            {
                lstarr.Add(true);
                lstarr.Add(e.Message);
            }
            return lstarr;
        }


        // POST api/vaccine/insertvac
        [HttpPost]
        public vaccine insertvac(vaccine vr)
        {
            vaccine vrobj = new vaccine();

            SqlConnection con = new SqlConnection(str);
            SqlCommand cmd = new SqlCommand("savevac", con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (vr.vaccine_id == -1)
            {
                cmd.Parameters.AddWithValue("@vaccine_id", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@vaccine_id", vr.vaccine_id);
            }

            cmd.Parameters.AddWithValue("@catid", vr.catid);
            cmd.Parameters.AddWithValue("@vaccine_name", vr.vaccine_name);
            cmd.Parameters.AddWithValue("@subdate", vr.subdate);
            cmd.Parameters.AddWithValue("@status", true);
            cmd.Parameters.AddWithValue("@Boys_Vaccine", vr.Boys_Vaccine);
            cmd.Parameters.AddWithValue("@Girls_Vaccine", vr.Girls_Vaccine);
            cmd.Parameters.AddWithValue("@short_vacname", vr.short_vacname);
            cmd.Parameters.AddWithValue("@trash", false);
            cmd.Parameters.AddWithValue("@trashby", DBNull.Value);
            cmd.Parameters.AddWithValue("@trashed_on", DBNull.Value);

            DbParameter returnValue;
            returnValue = cmd.CreateParameter();
            returnValue.Direction = ParameterDirection.ReturnValue;
            cmd.Parameters.Add(returnValue);
            con.Open();
            int result = 0;
            try
            {
                cmd.ExecuteNonQuery();
                result = Convert.ToInt32(returnValue.Value);

                if (result != 0)
                {
                    vrobj.vaccine_id = result;
                    vrobj.errflg = false;
                    vrobj.resultmsg = "Vaccine inserted successfully";
                }
                else
                {
                    vrobj.vaccine_id = 0;
                    vrobj.errflg = true;
                    vrobj.resultmsg = "Something goes wrong while inserting data";
                }
            }
            catch (Exception e)
            {
                vrobj.vaccine_id = 0;
                vrobj.errflg = true;
                vrobj.resultmsg = e.Message;
                //TempData["errormsg"] = e.Message;
            }
            finally
            {
                con.Close();
            }
            return vrobj;
        }

        // Post api/vaccine/addvac/
        [HttpPost]
        public vaccine addvac(Stream vrobjectstr)
        {
            vaccine vrobj = new vaccine();

            //// convert Stream Data to StreamReader
            StreamReader reader = new StreamReader(vrobjectstr);
            //// Read StreamReader data as string
            //string xmlString = reader.ReadToEnd();
            //string returnValue = xmlString;
            //try
            //{
            //    JObject jobj = (JObject)JsonConvert.DeserializeObject(returnValue);
            //}
            //catch
            //{

            //}

            return vrobj;
        }

        // Post api/vaccine/testing
        [HttpPost]
        public string testing([FromBody]string value)
        {
            //vaccine vr = new vaccine();
            return value;
        }

    }
}
