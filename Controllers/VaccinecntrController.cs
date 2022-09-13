using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using vacrem.Models;

namespace vacrem.Controllers
{
    public class VaccinecntrController : Controller
    {
        //
        // GET: /Vaccinecntr/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Vaccinecntr/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult showvaccine()
        {
            dynamic model = new ExpandoObject(); //for passing multiple model in single view
            vaccinelist lst = new vaccinelist();
            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var resp = client.GetAsync("api/vaccine/show").Result; //using arraylist
                
                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }

               JObject jobjchan = (JObject)JsonConvert.DeserializeObject(res);


               if (resp.IsSuccessStatusCode)
               {
                   if (jobjchan["errlstflg"].ToString() == "False")
                   {
                       lst.errlstflg = Convert.ToBoolean(jobjchan["errlstflg"].ToString());
                       lst.lstresultmsg = jobjchan["lstresultmsg"].ToString();

                       //TempData["errormsg"] = jobjchan["lstresultmsg"].ToString();
                       List<vaccine> lst1 = new List<vaccine>();
                       foreach (var en in jobjchan["vaclist"])
                       {
                           vaccine vr = new vaccine();
                           vr.vaccine_id = Convert.ToInt32(en["vaccine_id"].ToString());
                           vr.vaccine_name = en["vaccine_name"].ToString();
                           vr.status = Convert.ToBoolean(en["status"].ToString());
                           vr.Boys_Vaccine = Convert.ToBoolean(en["Boys_Vaccine"].ToString());
                           vr.Girls_Vaccine = Convert.ToBoolean(en["Girls_Vaccine"].ToString());
                           vr.short_vacname = (en["short_vacname"].ToString());
                           vr.catname = (en["catname"].ToString());
                           vr.catid = Convert.ToInt32(en["catid"].ToString());
                           lst1.Add(vr);
                       }
                       lst.vaclist = lst1;
                   }
                   else
                   {
                       lst.errlstflg = Convert.ToBoolean(jobjchan["errlstflg"].ToString());
                       lst.lstresultmsg = jobjchan["lstresultmsg"].ToString();

                       //  TempData["errormsg"] = jobjchan["lstresultmsg"].ToString();
                   }
               }
               else
               {
                   lst.errlstflg = false;
                   lst.lstresultmsg = "Their is no vaccine available.";
                   //  TempData["errormsg"] = "Something goes wrong";
               }
            }
            catch { 
            }

            model.vacchklst = lst;
            return View(model);
        }

        public ActionResult showvaccine1()
        {
            dynamic model = new ExpandoObject(); //for passing multiple model in single view
            List<vaccine> lst1 = new List<vaccine>();
            string res = "";
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var resp = client.GetAsync("api/vaccine/show1").Result; //using arraylist

                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }
              
                ArrayList arlst = JsonConvert.DeserializeObject<ArrayList>(res);
                bool errflg = false;
                string msg = "";
                if (arlst.Count == 3)
                {
                    errflg = Convert.ToBoolean(arlst[1].ToString());
                    msg = arlst[2].ToString();

                    JArray jsonResponse = JArray.Parse(arlst[0].ToString());

                    foreach (var en in jsonResponse)
                    {
                        vaccine vr = new vaccine();
                        vr.vaccine_id = Convert.ToInt32(en["vaccine_id"].ToString());
                        vr.vaccine_name = en["vaccine_name"].ToString();
                        vr.status = Convert.ToBoolean(en["status"].ToString());
                        vr.Boys_Vaccine = Convert.ToBoolean(en["Boys_Vaccine"].ToString());
                        vr.Girls_Vaccine = Convert.ToBoolean(en["Girls_Vaccine"].ToString());
                        vr.short_vacname = (en["short_vacname"].ToString());
                        vr.catname = (en["catname"].ToString());
                        vr.catid = Convert.ToInt32(en["catid"].ToString());
                        lst1.Add(vr);

                        //JObject jRaces = (JObject)item["races"];
                        //foreach (var rItem in jRaces)
                        //{
                        //    vaccine vr = rItem;


                        //    //string rItemKey = rItem.Key;
                        //    //JObject rItemValueJson = (JObject)rItem.Value;
                        //    //Races rowsResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Races>(rItemValueJson.ToString());
                        //}
                    }
                }
                else
                {
                    errflg = Convert.ToBoolean(arlst[0].ToString());
                    msg = arlst[1].ToString();
                }
               
            }
            catch
            {
            }

         //   vaccinelist lst = new vaccinelist();

            model.vacchklst = lst1;
            return View(model);
        }


        //
        // GET: /Vaccinecntr/Create

        public ActionResult addvaccine()
        {
            vaccine vr = new vaccine();
            vr.category = PopulateFruits();
            return View(vr);
        }

        private static List<SelectListItem> PopulateFruits()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            string constr = ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = " SELECT catid, catname FROM VR_Category";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr["catname"].ToString(),
                                Value = sdr["catid"].ToString()
                            });
                        }
                    }
                    con.Close();
                }
            }
            return items;
        }

        //  
        
        // POST: /Vaccinecntr/addvaccine

        [HttpPost]
        public ActionResult addvaccine(FormCollection collection)
        {
            try
            {
                vaccine vr = new vaccine();
             //   string va = collection["txttest"].ToString();
                string vacname = collection["vaccine_name"].ToString();
                string shortvacname = collection["short_vacname"].ToString();
                string vacstatus = collection["status"].Split(',')[0];
                string boystatus = collection["Boys_Vaccine"].Split(',')[0];
                string girlstatus = collection["Girls_Vaccine"].Split(',')[0];
                string trash = "False";
                string catid = collection["catid"].ToString();
                DateTime subdate = DateTime.Now;
                vr.vaccine_id = -1;
                vr.vaccine_name = vacname;
                vr.short_vacname = shortvacname;
                vr.status = Convert.ToBoolean(vacstatus);
                vr.Boys_Vaccine = Convert.ToBoolean(boystatus);
                vr.Girls_Vaccine = Convert.ToBoolean(girlstatus);
                vr.trash = Convert.ToBoolean(trash);
                vr.catid = Convert.ToInt32(catid);
                vr.subdate = subdate;
                // string output = JsonConvert.SerializeObject(vr); 

                bool status = true;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:50823/"); //Write url

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Create the JSON formatter.
                MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

                // Use the JSON formatter to create the content of the request body.
                HttpContent content = new ObjectContent<vaccine>(vr, jsonFormatter);

                // Send the get request.
                var resp = client.PostAsync("api/vaccine/insertvac", content).Result;
              
                //    var user =  resp.Content.ReadAsStreamAsync();
                string res = "";
                using (HttpContent content1 = resp.Content)
                {
                    Task<string> result = content1.ReadAsStringAsync();
                    res = result.Result;
                }
                JObject jobjchan = (JObject)JsonConvert.DeserializeObject(res);

                #region ================== Code comment on 25 april 2019 -============================
                //  vr = JsonConvert.DeserializeObject(res);
                //  var user = JsonConvert.DeserializeObject<vaccine>(await resp.Content.ReadAsStringAsync());
                // var client = new HttpClient();
                //  client.BaseAddress = new Uri("http://localhost:50823/api/vaccine/insertvac");
                //HTTP POST
                //  var postTask = client.PostAsJsonAsync<vaccine>("vaccine",vr);
                //var postTask = client.PostAsync("vaccine",);
                //  postTask.Wait();

                //var result = postTask.Result;
                #endregion ==================== comment code end =============================

                if (resp.IsSuccessStatusCode)
                {
                    if (jobjchan["errflg"].ToString() == "False")
                    {
                        TempData["errormsg"] = jobjchan["resultmsg"].ToString();
                    }
                    else
                    {
                        TempData["errormsg"] = jobjchan["resultmsg"].ToString();
                    }
                }
                else
                {
                    TempData["errormsg"] = "Something goes wrong";
                }
            }
            catch
            {

            } 
            vaccine vr1 = new vaccine();
            vr1.category = PopulateFruits();
            return View(vr1);
        }

        //
        // GET: /Vaccinecntr/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }


        //
        // POST: /Vaccinecntr/Edit/5

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
        // GET: /Vaccinecntr/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Vaccinecntr/Delete/5

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
    }
}
