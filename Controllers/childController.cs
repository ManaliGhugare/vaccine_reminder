using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using vacrem.Models;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using vacrem.Controllers;

namespace vacrem.Controllers
{
    public class childController : Controller
    {
        // GET: /child/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult childlist()
        {
            dynamic model = new ExpandoObject(); //for passing multiple model in single view
            model.child = "test";
           // model.childlist = getallchild("manali@gmail.com");  //direct access using method defin in same class

            //Call web api method
            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create the JSON formatter.
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

                // Use the JSON formatter to create the content of the request body.
            //    HttpContent content = new ObjectContent<string>("manali@gmail.com", jsonFormatter);

                var resp = client.GetAsync("api/childapi/getallchild?parid=manali@gmail.com").Result; //using arraylist

               // var result = client.PostAsync(

                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }
                childlist chlstobj = JsonConvert.DeserializeObject<childlist>(res);

                if (resp.IsSuccessStatusCode)
                {
                    model.childlist = chlstobj;
                }
                else
                {
                   
                }
            }
            catch(Exception e)
            {
                TempData["errormsg"] = e.Message;
            }
            return View(model);
        }


        public ActionResult childadd()
        {
            return View();
        }

        public ActionResult addchild1()
        {
            Session["mysec"] = "Manali";
            //dynamic model = new ExpandoObject();
            List<SelectListItem> country = new List<SelectListItem>();

            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create the JSON formatter.
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

                // Use the JSON formatter to create the content of the request body.
                //    HttpContent content = new ObjectContent<string>("manali@gmail.com", jsonFormatter);

                var resp = client.GetAsync("api/counstate/populatecount").Result; //using arraylist

                // var result = client.PostAsync(

                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }

                country = JsonConvert.DeserializeObject<List<SelectListItem>>(res);
                if (resp.IsSuccessStatusCode)
                {
                   // ViewData["CountryName"] = new SelectList(chlstobj, "Value", "Text");
                    ViewData["country"] = country;
                  //  country = ViewData["CountryName"];
                }
                else
                {

                }

            }
            catch (Exception e)
            {
                TempData["errormsg"] = e.Message;
            }

          //  return View("addchild1"); //to call another view from controller


            TempData.Keep("test");
            return View();
        }

        [HttpPost]
        public ActionResult addchild1(FormCollection col)
        {
            return View();
        }


        public ActionResult addchild()
        {
            Session["mysec"] = "Manali";
            //dynamic model = new ExpandoObject();
            List<SelectListItem> country = new List<SelectListItem>();

            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create the JSON formatter.
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

                // Use the JSON formatter to create the content of the request body.
                //    HttpContent content = new ObjectContent<string>("manali@gmail.com", jsonFormatter);

                var resp = client.GetAsync("api/counstate/populatecount").Result; //using arraylist

                // var result = client.PostAsync(

                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }

                country = JsonConvert.DeserializeObject<List<SelectListItem>>(res);
                if (resp.IsSuccessStatusCode)
                {
                    // ViewData["CountryName"] = new SelectList(chlstobj, "Value", "Text");
                    ViewData["ddlCountry"] = country;
                    //  country = ViewData["CountryName"];
                }
                else
                {

                }

            }
            catch (Exception e)
            {
                TempData["errormsg"] = e.Message;
            }

            //  return View("addchild1"); //to call another view from controller


            TempData.Keep("test");
            return View();
        }

        [HttpPost]
        public ActionResult addchild(FormCollection col)
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /child/Create

        public ActionResult Create()
        {

            return View();
        }

        // POST: /child/create
        [HttpPost]
        public ActionResult create(FormCollection col)
        {
            return View();
        }

        //
        // GET: /child/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /child/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /child/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: /child/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //my code start to get all child of particular parent
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
                    cmd.Parameters.Add("@userid", "manali@gmail.com");
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
