using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using vacrem.Models;
using vacrem.Controllers;
using System.ComponentModel;

namespace vacrem.Controllers
{
    public class generatescheapiController : ApiController
    {
        ArrayList Listpreviousduedate = new ArrayList();
        ArrayList Listpreviousenddate = new ArrayList();
        ArrayList Listpreviousgivendate = new ArrayList();
        ArrayList Listpreviousgiven = new ArrayList();
        ArrayList Listpreviousvalues = new ArrayList();
        ArrayList Listpreviousschid = new ArrayList();
        ArrayList Listprevioustbltype = new ArrayList();
        ArrayList Listpreviousvacid = new ArrayList();
        ArrayList Listpreviousdoseno = new ArrayList();

        ArrayList ListDuedates = new ArrayList();
        ArrayList ListEndDates = new ArrayList();
        ArrayList ListReminderondate = new ArrayList();
        ArrayList ListGivenVacDate = new ArrayList();
        ArrayList ListSkipdate = new ArrayList();
        ArrayList Listvacid = new ArrayList();
        ArrayList Listcatid = new ArrayList();
        ArrayList Listschid = new ArrayList();
        ArrayList Listschtype = new ArrayList();
        ArrayList ListVaccinename = new ArrayList();
        ArrayList ListVacgivenornot = new ArrayList();
        ArrayList ListVaccinedosenumber = new ArrayList();

        int noofredvaccines = 0;
        int noofgreenvaccines = 0;
        int nooforangevaccines = 0;
        int noofgrayvaccines = 0;

        DateTime firstdateduedattime;
        DateTime currentdosedate;
        string fistdatedue;
        string Curdosedate;

        // GET api/generatescheapi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/generatescheapi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/generatescheapi
        public void Post([FromBody]string value)
        {
        }

        [HttpPost]
        public finalsch generatesch(requschdata req)
        {
            List<childvacsch> childsch = new List<childvacsch>();
            finalsch fnch = new finalsch();
           
            //assign variable value
            int countryid = req.countryid;
            int stateid = req.stateid;
            int Vacyear = 2015;
            string userid = req.userid;

            ArrayList trigerschid = new ArrayList();
            ArrayList trigerdoseno = new ArrayList();

            string lblprevduedatedisplay = "";
            string lblprevenddatedisplay = "";
            string lblprevgivendatedisplay = "";
            string lblprevschids = "";
            string lblprevtbltypes = "";
            string lblduedategivenbooster = "";
            string lblDueDateGivenimgchk = "";
            string dosegivenornot = "";

            DateTime reminderdate = DateTime.Now;
            DateTime givendate = DateTime.Now;
            DateTime skipdate = DateTime.Now;

            bool nodue = false;
            bool skipflag = false;
            bool compulsoryflag = false;
            bool triggershow = true;
            bool set_as_previous = false;
            bool givenchkflag = false;

            string lblendDate = "";
            int orangedays = 0;
            string dateformat = "";
            string dosecolor = "";

            bool TriggerShowVac = false;
            bool TriggerNoDue = false;

            int givenid = 0;
            int skipid = 0;

            string bday;
            int schid = 0;
            int doseno = 0;
            int d_days = 0;
            int d_months = 0;
            int d_year = 0;
            int e_days = 0;
            int e_months = 0;
            int e_year = 0;
            string booster = "False";
            string lblboosterdoses = "";

            int childid = req.chilid;
            string sex = req.gender;
            DateTime dob =req.childbirth;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["vacrem"].ConnectionString))
            {
                con.Open();
                string custquery = "";
                bool custsch = false;

                custquery = "select customsch from vaccine_child_master where child_id='" + childid + "'";
                SqlCommand custquerycmd = new SqlCommand(custquery, con);
                custquerycmd.CommandType = CommandType.Text;
                custquerycmd.ExecuteNonQuery();
                SqlDataAdapter custad = new SqlDataAdapter(custquerycmd);
                DataTable dtcustsch = new DataTable();
                custad.Fill(dtcustsch);
                if (dtcustsch.Rows.Count != 0)
                {
                    if (dtcustsch.Rows[0].ItemArray[0].ToString() == "True")
                    {
                        custsch = true;
                    }
                }

                string query = "";
                if (custsch != true)
                {
                    if (sex.ToLower() == "boy")
                    {
                        query = "select catid from VR_Category where catid in (select catid from VR_Vaccines where vaccine_id in (select vaccine_id from VR_Schedule where archived='False' and Boys_Vaccine='True' and country_id='" + countryid + "' and stateid='" + stateid + "' and Year='" + Vacyear + "'))";
                    }
                    else
                    {
                        query = "select catid from VR_Category where catid in (select catid from VR_Vaccines where vaccine_id in (select vaccine_id from VR_Schedule where archived='False' and Girls_Vaccine='True' and country_id='" + countryid + "' and stateid='" + stateid + "' and Year='" + Vacyear + "'))";
                    }
                }
                else
                {
                    if (sex.ToLower() == "boy")
                    {
                        query = "select catid from VRC_Category where catid in (select catid from VRC_Vaccines where vaccine_id in (select vaccine_id from  VRC_Schedule where userid='" + userid + "') and  Boys_Vaccine='True')";
                    }
                    else
                    {
                        query = "select catid from VRC_Category where catid in (select catid from VRC_Vaccines where vaccine_id in (select vaccine_id from  VRC_Schedule where userid='" + userid + "') and  Girls_Vaccine='True')";
                    }
                }

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                DataTable dtcatid = new DataTable();
                ad.Fill(dtcatid);
                try
                {
                    if (dtcatid.Rows.Count != 0)
                    {
                        DateTime dob1 = DateTime.Now;
                        for (int i = 0; i < dtcatid.Rows.Count; i++)
                        {
                            nodue = false;
                            givenchkflag = false;
                            triggershow = true;
                            skipflag = false;
                            compulsoryflag = false;
                            TriggerNoDue = false;
                            TriggerShowVac = false;
                            set_as_previous = false;

                            int categoryid = Convert.ToInt32(dtcatid.Rows[i].ItemArray[0].ToString());
                            string querycatidvacid = "";
                            if (custsch != true)
                            {
                                if (sex.ToLower() == "boy")
                                {
                                    querycatidvacid = "select vaccine_id,catid,vaccine_name, 'S' as tbltype from VR_Vaccines where catid='" + categoryid + "' and Boys_Vaccine='True' and vaccine_id in (select distinct vaccine_id from VR_Schedule where archived='False' and country_id='" + countryid + "' and stateid='" + stateid + "' and Year='" + Vacyear + "')";

                                }
                                else if (sex.ToLower() == "girl")
                                {
                                    querycatidvacid = "select vaccine_id,catid,vaccine_name, 'S' as tbltype from VR_Vaccines where catid='" + categoryid + "' and Girls_Vaccine='True' and vaccine_id in (select distinct vaccine_id from VR_Schedule where archived='False' and country_id='" + countryid + "' and stateid='" + stateid + "' and Year='" + Vacyear + "')";
                                }
                            }
                            else
                            {
                                if (sex.ToLower() == "boy")
                                {
                                    querycatidvacid = "select vaccine_id,catid,vaccine_name, 'S' as tbltype from VRC_Vaccines where catid='" + categoryid + "' and Boys_Vaccine='True' and vaccine_id in (select distinct vaccine_id from VRC_Schedule where userid='" + userid + "')";

                                }
                                else if (sex.ToLower() == "girl")
                                {
                                    querycatidvacid = "select vaccine_id,catid,vaccine_name, 'S' as tbltype from VRC_Vaccines where catid='" + categoryid + "' and Girls_Vaccine='True' and vaccine_id in (select distinct vaccine_id from VRC_Schedule where userid='" + userid + "')";
                                }
                            }
                            SqlCommand cmdcatvacid = new SqlCommand(querycatidvacid, con);
                            cmdcatvacid.CommandType = CommandType.Text;
                            cmdcatvacid.ExecuteNonQuery();
                            SqlDataAdapter adcatvacid = new SqlDataAdapter(cmdcatvacid);
                            DataTable dtvacids = new DataTable();
                            adcatvacid.Fill(dtvacids);

                            #region ============= Contains greater than one vaccine id =====================

                            if (dtvacids.Rows.Count > 1)
                            {
                                for (int vid = 0; vid < dtvacids.Rows.Count; vid++)
                                {
                                    int vaccineid = Convert.ToInt32(dtvacids.Rows[vid].ItemArray[0].ToString());

                                    string queryopted = "";
                                    if (custsch != true)
                                    {
                                        queryopted = "select vaccine_id,vaccine_name from VR_Vaccines where vaccine_id in (select top 1 vaccine_id from VR_Opted where catid='" + categoryid + "' and child_id='" + childid + "' and vaccine_id ='" + vaccineid + "')";
                                    }
                                    else
                                    {
                                        queryopted = "select vaccine_id,vaccine_name from VRC_Vaccines where vaccine_id in (select top 1 vaccine_id from VRC_Opted where catid='" + categoryid + "' and child_id='" + childid + "' and vaccine_id ='" + vaccineid + "')";
                                    }
                                    // string queryopted = "select vaccine_id,vaccine_name from VR_Vaccines where vaccine_id in (select top 1 vaccine_id from VR_Opted where catid='" + categoryid + "' and child_id='" + childid + "' and vaccine_id in (select vaccine_id from VR_Schedule where country_id='" + countryid + "' and stateid='" + stateid + "' and Year='" + Vacyear + "'))";
                                    SqlCommand cmdoptvacid = new SqlCommand(queryopted, con);
                                    cmdoptvacid.CommandType = CommandType.Text;
                                    cmdoptvacid.ExecuteNonQuery();
                                    SqlDataAdapter adoptvacid = new SqlDataAdapter(cmdoptvacid);
                                    DataTable dtoptedvacid = new DataTable();
                                    adoptvacid.Fill(dtoptedvacid);

                                    #region ====== Code start for the vacid is present in VROpted ====================
                                    if (dtoptedvacid.Rows.Count != 0)
                                    {
                                        if (custsch != true)
                                        {
                                            #region ========================== Code start for normal schedule ============================
                                            VRSchedulelist vrschlist = new VRSchedulelist();
                                            vrschlist = schedulelstController.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                           // vrschlist = VRScheduleDA.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                            DataTable dtschedule = ConvertToDataTable(vrschlist);
                                            if (dtschedule.Rows.Count != 0)
                                            {

                                                trigerschid.Clear();
                                                trigerdoseno.Clear();
                                                for (int p = 0; p < dtschedule.Rows.Count; p++)
                                                {
                                                    childvacsch finalobject = new childvacsch();
                                                    set_as_previous = Convert.ToBoolean(dtschedule.Rows[p].ItemArray[12].ToString());
                                                    schid = Convert.ToInt32(dtschedule.Rows[p].ItemArray[0].ToString());
                                                    doseno = Convert.ToInt32(dtschedule.Rows[p].ItemArray[7].ToString());
                                                    d_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[4].ToString());
                                                    d_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[5].ToString());
                                                    d_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[6].ToString());
                                                    e_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[8].ToString());
                                                    e_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[9].ToString());
                                                    e_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[10].ToString());
                                                    booster = dtschedule.Rows[p].ItemArray[13].ToString();
                                                    lblboosterdoses = dtschedule.Rows[p].ItemArray[14].ToString();

                                                    #region ===================== Code start to check schedule is archieved ===================
                                                    bool archie = false;
                                                    string strarch = "select archived from VR_Schedule where  schedule_id='" + schid + "'";
                                                    SqlCommand cmdarch = new SqlCommand(strarch, con);
                                                    cmdarch.CommandType = CommandType.Text;
                                                    cmdarch.ExecuteNonQuery();
                                                    SqlDataAdapter sqladarch = new SqlDataAdapter(cmdarch);
                                                    DataTable dtarch = new DataTable();
                                                    sqladarch.Fill(dtarch);
                                                    if (dtarch.Rows.Count != 0)
                                                    {
                                                        archie = Convert.ToBoolean(dtarch.Rows[0].ItemArray[0].ToString());
                                                    }

                                                    #endregion ====================== Code end for archieved ======================

                                                    #region ========================= code start for dose details ================================
                                                    string queryorangedays = "select vacreminderid,userid,subdate,status,orangedaysetting,date_format from VaccineReminder_Settings where userid='" + userid + "' and status='True'";
                                                    SqlCommand cmdOD = new SqlCommand(queryorangedays, con);
                                                    cmdOD.CommandType = CommandType.Text;
                                                    SqlDataAdapter adOD = new SqlDataAdapter(cmdOD);
                                                    DataTable dtorangedays = new DataTable();
                                                    adOD.Fill(dtorangedays);
                                                    if (dtorangedays.Rows.Count != 0)
                                                    {
                                                        dateformat = dtorangedays.Rows[0].ItemArray[5].ToString();
                                                        orangedays = Convert.ToInt32(dtorangedays.Rows[0].ItemArray[4].ToString());
                                                    }
                                                    else
                                                    {
                                                        //take from default settings
                                                        string queryoddefault = "select defaultvacremid,subdate,status,orangedaysetting,date_format from Default_VacReminder_Setting where status='True'";
                                                        SqlCommand cmdODdefault = new SqlCommand(queryoddefault, con);
                                                        cmdODdefault.CommandType = CommandType.Text;
                                                        SqlDataAdapter adODdefault = new SqlDataAdapter(cmdODdefault);
                                                        DataTable dtdefaultorngdays = new DataTable();
                                                        adODdefault.Fill(dtdefaultorngdays);
                                                        if (dtdefaultorngdays.Rows.Count != 0)
                                                        {
                                                            dateformat = dtdefaultorngdays.Rows[0].ItemArray[4].ToString();
                                                            orangedays = Convert.ToInt32(dtdefaultorngdays.Rows[0].ItemArray[3].ToString());
                                                        }
                                                        else
                                                        {
                                                            dateformat = "dd MMM, yyyy";
                                                            orangedays = 15;
                                                        }
                                                    }
                                                    #endregion ============================ code end for dose details ===================================

                                                    #region =============== Code start for trigger schedule =====================

                                                    VR_TriggerList vrtriglist = new VR_TriggerList();
                                                    vrtriglist = triggerscheduleController.gridviewTriggerCidVacid(countryid, vaccineid);
                                                    //vrtriglist = VR_TriggerScheDA.gridviewTriggerCidVacid(countryid, vaccineid);
                                                    DataTable dttriggersch = ConvertToDataTable(vrtriglist);
                                                    #region ============ Code start for vacid present in trigger schedule =============
                                                    if (dttriggersch.Rows.Count != 0)  //vaccine id present in trigger schedule
                                                    {
                                                        TriggerShowVac = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[4].ToString());
                                                        TriggerNoDue = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[7].ToString());

                                                        string triggerquery = "select dateadd(YY,0,dateadd(M,Convert(int,'" + dttriggersch.Rows[0].ItemArray[6].ToString() + "'),dateadd(dd,0,'" + dob.ToString("yyyy-MM-dd") + "')))";
                                                        SqlCommand cmdtrig = new SqlCommand(triggerquery, con);
                                                        cmdtrig.CommandType = CommandType.Text;
                                                        SqlDataAdapter adtrig = new SqlDataAdapter(cmdtrig);
                                                        DataTable dttrigerprevgivendt = new DataTable();
                                                        adtrig.Fill(dttrigerprevgivendt);
                                                        if (TriggerShowVac == true)
                                                        {
                                                            if (TriggerNoDue == true)
                                                            {
                                                                if (doseno == 1)
                                                                {
                                                                    VR_GivenList vrgivenlist = new VR_GivenList();
                                                                    vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                    DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }

                                                                }
                                                                else if (doseno > 1)
                                                                {
                                                                    VR_GivenList vrgivenlist = new VR_GivenList();
                                                                    vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                    DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }
                                                                }
                                                            }
                                                            else if (TriggerNoDue == false)
                                                            {
                                                                if (doseno == 1)
                                                                {

                                                                }
                                                                else if (doseno > 1)
                                                                {
                                                                    VR_GivenList vrgivenlist = new VR_GivenList();
                                                                    vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                    DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (TriggerShowVac == false)
                                                        {
                                                            if (doseno == 1)
                                                            {
                                                                trigerschid.Add(schid);
                                                                trigerdoseno.Add(doseno);
                                                                triggershow = true;
                                                            }
                                                            else if (doseno > 1)
                                                            {
                                                                VR_GivenList vrgivenlist = new VR_GivenList();
                                                                vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                DataTable dttrigergiven = ConvertToDataTable(vrgivenlist);
                                                                if (dttrigergiven.Rows.Count != 0)
                                                                {
                                                                    if (Convert.ToDateTime(dttrigergiven.Rows[0].ItemArray[5].ToString()) > Convert.ToDateTime(dttrigerprevgivendt.Rows[0].ItemArray[0].ToString()))
                                                                    {
                                                                        triggershow = false;

                                                                    }
                                                                    else
                                                                    {
                                                                        triggershow = true;

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    int schidindex = trigerschid.Count - 1;
                                                                    VR_GivenList vrgivenlist1 = new VR_GivenList();
                                                                    vrgivenlist1 = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, Convert.ToInt32(trigerschid[schidindex].ToString()), Convert.ToInt32(trigerdoseno[schidindex].ToString()), "S");
                                                                    DataTable dttrigergiven1 = ConvertToDataTable(vrgivenlist1);
                                                                    if (dttrigergiven1.Rows.Count != 0)
                                                                    {
                                                                        triggershow = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        triggershow = false;
                                                                    }
                                                                }
                                                                trigerschid.Add(schid);
                                                                trigerdoseno.Add(doseno);

                                                                // triggershow = true;
                                                            }
                                                        }
                                                    }

                                                    #endregion =========== code end for vacid present in trigger schedule ============

                                                    #region =========== Code start for vacid  not present in trigger schdule ===========
                                                    else
                                                    {

                                                    }
                                                    #endregion ============= Code end for vacid not present in trigger schedule ============

                                                    #endregion ============ Code end for trigger schedule =======================

                                                    #region ======Code start for set as previous given ==============
                                                    if (doseno == 1 && archie == false)
                                                    {
                                                        Listpreviousgiven.Clear();
                                                        Listpreviousvalues.Clear();
                                                        Listpreviousschid.Clear();
                                                        Listprevioustbltype.Clear();
                                                        Listpreviousvacid.Clear();
                                                        Listpreviousduedate.Clear();
                                                        Listpreviousenddate.Clear();
                                                        Listpreviousgivendate.Clear();

                                                        VR_GivenList vrpregiven = new VR_GivenList();
                                                        vrpregiven = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                        DataTable dtpregiven = ConvertToDataTable(vrpregiven);

                                                        #region =======Code start dose no one present in vr_given =================
                                                        if (dtpregiven.Rows.Count != 0)//vaccine id dose is already given 
                                                        {
                                                            bday = dob.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgiven.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                                            givenchkflag = true;

                                                            lblduedategivenbooster = dtpregiven.Rows[0].ItemArray[5].ToString();
                                                            Listpreviousvalues.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);

                                                            Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                            firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgivendate.Add(dtpregiven.Rows[0].ItemArray[5].ToString());


                                                            //List add for database 
                                                            ListVacgivenornot.Add("Yes");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);
                                                        }
                                                        #endregion ========== code end for dose no 1 present in vr_given ================

                                                        #region ====== Code start for dose no 1 not present in vr_given ========
                                                        else
                                                        {
                                                            bday = dob.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {

                                                            }
                                                            givenchkflag = false;
                                                            Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);
                                                            lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            lblduedategivenbooster = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                            firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgivendate.Add(null);


                                                            //List add for database 
                                                            ListVacgivenornot.Add("No");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);
                                                        }
                                                        #endregion ========= Code end dose1 not present in vr_given==============
                                                    }
                                                    else if (doseno > 1 && archie == false)
                                                    {
                                                        for (int s = 0; s < Listpreviousduedate.Count; s++)
                                                        {
                                                            lblprevduedatedisplay = "";
                                                            int j = Listpreviousduedate.Count;
                                                            if (Listpreviousduedate[j - 1].ToString() != "")
                                                            {
                                                                lblprevduedatedisplay = Listpreviousduedate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }

                                                        for (int s = 0; s < Listpreviousenddate.Count; s++)
                                                        {
                                                            lblprevenddatedisplay = "";
                                                            int j = Listpreviousenddate.Count;
                                                            if (Listpreviousenddate[j - 1].ToString() != "")
                                                            {
                                                                lblprevenddatedisplay = Listpreviousenddate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listpreviousgivendate.Count; s++)
                                                        {
                                                            lblprevgivendatedisplay = "";
                                                            int j = Listpreviousgivendate.Count;
                                                            if (Listpreviousgivendate[j - 1] != null)
                                                            {
                                                                lblprevgivendatedisplay = Listpreviousgivendate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listpreviousschid.Count; s++)
                                                        {
                                                            lblprevschids = "";
                                                            //Response.Write(Listpreviousschid[i].ToString());
                                                            int j = Listpreviousschid.Count;
                                                            // Response.Write(Listpreviousschid.Count + "j= "+ j );
                                                            if (Listpreviousschid[j - 1].ToString() != "")
                                                            {
                                                                lblprevschids = Listpreviousschid[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listprevioustbltype.Count; s++)
                                                        {
                                                            lblprevtbltypes = "";
                                                            int j = Listprevioustbltype.Count;
                                                            if (Listprevioustbltype[j - 1].ToString() != "")
                                                            {
                                                                lblprevtbltypes = Listprevioustbltype[j - 1].ToString();
                                                                break;
                                                            }

                                                        }

                                                        #region ==================================================  set as previous given is true and false code starts =============================================

                                                        string currentdoseduedate = string.Empty;

                                                        if (set_as_previous == true)//set as previous given true
                                                        {
                                                            VR_GivenList vrpregiven1 = new VR_GivenList();
                                                            vrpregiven1 = VR_GivenDAController.gridviewchidschidtbltypelist(childid, Convert.ToInt32(lblprevschids), "S");
                                                            DataTable dtsgivendt1 = ConvertToDataTable(vrpregiven1);
                                                            // Response.Write("previous schid = " + lblprevschids.Text);
                                                            if (dtsgivendt1.Rows.Count != 0)
                                                            {
                                                                if (nodue == true)
                                                                {
                                                                    nodue = false;
                                                                }
                                                            }

                                                            if (lblprevgivendatedisplay != "")
                                                            {
                                                                if (Convert.ToDateTime(lblprevgivendatedisplay) <= Convert.ToDateTime(lblprevenddatedisplay))
                                                                {
                                                                    currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                    cmdbday.CommandType = CommandType.Text;
                                                                    cmdbday.ExecuteNonQuery();
                                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                    DataTable dtfirstdosedate = new DataTable();
                                                                    adbday.Fill(dtfirstdosedate);

                                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                                    {
                                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    //  Response.Write(lblprevgivendatedisplay.Text);
                                                                    currentdosedate = Convert.ToDateTime(lblprevgivendatedisplay);
                                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                    cmdbday.CommandType = CommandType.Text;
                                                                    cmdbday.ExecuteNonQuery();
                                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                    DataTable dtfirstdosedate = new DataTable();
                                                                    adbday.Fill(dtfirstdosedate);

                                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                                    {
                                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                        // Response.Write(currentdoseduedate);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                                Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                cmdbday.CommandType = CommandType.Text;
                                                                cmdbday.ExecuteNonQuery();
                                                                SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                DataTable dtfirstdosedate = new DataTable();
                                                                adbday.Fill(dtfirstdosedate);

                                                                if (dtfirstdosedate.Rows.Count != 0)
                                                                {
                                                                    currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                        }

                                                        VR_GivenList vrpregiven = new VR_GivenList();
                                                        vrpregiven = VR_GivenDAController.gridviewchidschidtbltypelist(childid, schid, "S");
                                                        DataTable dtsgivendt = ConvertToDataTable(vrpregiven);
                                                        if (dtsgivendt.Rows.Count != 0)
                                                        {
                                                            dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                            bday = dob1.ToString("yyyy-MM-dd");
                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                lblDueDateGivenimgchk = currentdoseduedate;
                                                            }
                                                            Listpreviousgiven.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                                            givenchkflag = true;

                                                            Listpreviousvalues.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);

                                                            lblduedategivenbooster = dtsgivendt.Rows[0].ItemArray[5].ToString();
                                                            Listpreviousduedate.Add(currentdoseduedate);
                                                            currentdosedate = Convert.ToDateTime(currentdoseduedate);

                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                // lblduedateaftergiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }

                                                            Listpreviousgivendate.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());

                                                            //List add for database 
                                                            ListVacgivenornot.Add("Yes");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);

                                                        }
                                                        else
                                                        {
                                                            dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                            bday = dob1.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {

                                                                Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                Listpreviousschid.Add(schid);
                                                                Listprevioustbltype.Add("S");
                                                                Listpreviousvacid.Add(vaccineid);

                                                                Listpreviousgiven.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                lblduedategivenbooster = currentdoseduedate;
                                                                lblDueDateGivenimgchk = currentdoseduedate;
                                                                Listpreviousduedate.Add(currentdoseduedate);
                                                                currentdosedate = Convert.ToDateTime(currentdoseduedate);
                                                                Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + Curdosedate + "')))";
                                                                SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                                cmdend.CommandType = CommandType.Text;
                                                                cmdend.ExecuteNonQuery();
                                                                SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                                DataTable dtduedosedate = new DataTable();
                                                                adenddate.Fill(dtduedosedate);
                                                                if (dtduedosedate.Rows.Count != 0)
                                                                {
                                                                    // lblDueDateGiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                                    Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                    lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                                }
                                                                Listpreviousgivendate.Add(null);

                                                                //List add for database 
                                                                ListVacgivenornot.Add("No");
                                                                ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                Listvacid.Add(vaccineid);
                                                                Listschid.Add(schid);
                                                                ListVaccinedosenumber.Add(doseno);
                                                                Listcatid.Add(categoryid);
                                                            }
                                                        }
                                                        #endregion =============================================== set as previous given code ends ===============================================

                                                    }

                                                    #endregion ======== Code end for set as previous given ============

                                                    #region =================Code start for Vacid present in vr_given ====================================
                                                    VR_GivenList givenlist = new VR_GivenList();
                                                    givenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                    DataTable dtgiven1 = ConvertToDataTable(givenlist);
                                                    if (dtgiven1.Rows.Count != 0)
                                                    {
                                                        givenid = Convert.ToInt32(dtgiven1.Rows[0].ItemArray[0].ToString());
                                                        givendate = Convert.ToDateTime(dtgiven1.Rows[0].ItemArray[5].ToString());
                                                        // dosegivenornot ="Yes";
                                                        givenchkflag = true;
                                                    }
                                                    else
                                                    {
                                                        givenchkflag = false;
                                                        givendate = DateTime.Now;
                                                        givenid = 0;
                                                        // dosegivenornot="No";
                                                    }
                                                    #endregion ================ Code end for Vacid present in vr_given  ==================================

                                                    #region ======================================= booster dose code starts ========================================================================================

                                                    bool showdotsbooster = true;
                                                    if (booster == "True")
                                                    {
                                                        bool chkgiven = false;
                                                        if (Convert.ToDateTime(lblduedategivenbooster) > DateTime.Now)
                                                        {
                                                            showdotsbooster = false;
                                                        }
                                                        else
                                                        {
                                                            string strdate = null;
                                                            string[] strArrdate = null;

                                                            //int count = 0;
                                                            strdate = lblboosterdoses;
                                                            char[] splitchar11 = { ',' };


                                                            strArrdate = strdate.Split(splitchar11);

                                                            for (int b = 0; b < strArrdate.Length; b++)
                                                            {
                                                                for (int s = 0; s <= b - 1; s++)
                                                                {
                                                                    VR_GivenList vrpregiven = new VR_GivenList();
                                                                    vrpregiven = VR_GivenDAController.gridviewchidschidtbltypelist(childid, Convert.ToInt32(Listpreviousschid[s]), "S");
                                                                    DataTable dtboster = ConvertToDataTable(vrpregiven);

                                                                    if (dtboster.Rows.Count != 0)
                                                                    {
                                                                        chkgiven = true;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        chkgiven = false;
                                                                    }
                                                                    if (chkgiven == true)
                                                                        break;
                                                                }
                                                            }

                                                            if (chkgiven == true)
                                                            {

                                                                showdotsbooster = false;
                                                            }
                                                            else
                                                            {

                                                                showdotsbooster = true;
                                                            }

                                                        }
                                                    }

                                                    #endregion ==================================== booster dose code ends ==========================================================================================

                                                    #region =================Code start for Not Compulsory ==========================================
                                                    VRSchedulelist vrsch = new VRSchedulelist();
                                                    vrsch = schedulelstController.gridviewvacschdoseno(schid, vaccineid, doseno, countryid, stateid, Vacyear);
                                                    DataTable dtnotcosly = ConvertToDataTable(vrsch);

                                                    if (dtnotcosly.Rows.Count != 0)
                                                    {
                                                        if (dtnotcosly.Rows[0].ItemArray[16].ToString() == "True")
                                                        {
                                                            compulsoryflag = true;
                                                        }
                                                        else
                                                        {
                                                            compulsoryflag = false;
                                                        }
                                                    }
                                                    #endregion ============================ code end for Not compulsory =============================

                                                    #region ================== code start for reminder on ================================

                                                    //Code start for reminder set on 

                                                    bool reminder1set = false;
                                                    bool reminder2set = false;
                                                    string givenreminderdate = lblDueDateGivenimgchk;

                                                    VR_ReminderChangeslist vrrchnglist = new VR_ReminderChangeslist();
                                                    vrrchnglist = VR_ReminderChngDAController.gridviewscheduleid(userid, childid, vaccineid, schid, doseno, "S");
                                                    DataTable dtchngreminder = ConvertToDataTable(vrrchnglist);

                                                    if (dtchngreminder.Rows.Count != 0)
                                                    {
                                                        if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "1")
                                                        {
                                                            //string date1 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                            ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                            reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                        }
                                                        else if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "2")
                                                        {
                                                            // string date2 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                            ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                            reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                        }
                                                    }
                                                    else
                                                    {
                                                        VR_ReminderSettingList vrremindsetting = new VR_ReminderSettingList();
                                                        vrremindsetting = VR_ReminderSettingDAController.gridviewuid(userid, "True");
                                                        DataTable dtRemindsetng = ConvertToDataTable(vrremindsetting);
                                                        if (dtRemindsetng.Rows.Count != 0)
                                                        {
                                                            if (reminder1set == false)
                                                            {
                                                                string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                ListReminderondate.Add(date1);
                                                                reminderdate = Convert.ToDateTime(date1);
                                                            }
                                                            else
                                                            {
                                                                if (dtRemindsetng.Rows[0].ItemArray[6].ToString() == "True")
                                                                {
                                                                    reminder2set = true;
                                                                    if (reminder2set == true)
                                                                    {
                                                                        string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[3].ToString())).ToShortDateString();
                                                                        ListReminderondate.Add(date2);
                                                                        reminderdate = Convert.ToDateTime(date2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    reminder2set = false;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            VR_DefaultSettingLsit vrdefalutlist = new VR_DefaultSettingLsit();
                                                            vrdefalutlist = VR_DefaultSettingDAController.gridview();
                                                            DataTable dtdefaultsetng = ConvertToDataTable(vrdefalutlist);
                                                            if (dtdefaultsetng.Rows.Count != 0)
                                                            {
                                                                if (reminder1set == false)
                                                                {
                                                                    string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[1].ToString())).ToShortDateString();
                                                                    ListReminderondate.Add(date1);
                                                                    reminderdate = Convert.ToDateTime(date1);
                                                                }
                                                                else
                                                                {
                                                                    if (dtdefaultsetng.Rows[0].ItemArray[5].ToString() == "True")
                                                                    {
                                                                        reminder2set = true;
                                                                        if (reminder2set == true)
                                                                        {
                                                                            string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                            ListReminderondate.Add(date2);
                                                                            reminderdate = Convert.ToDateTime(date2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        reminder2set = false;

                                                                    }

                                                                }

                                                            }

                                                        }

                                                    }


                                                    #endregion ================= end code for reminder on ================================

                                                    #region ===================================== code for give vaccine(Submit and remove) start ========================================================
                                                    bool archgiven = false;
                                                    VR_GivenList vrvacsubmit = new VR_GivenList();
                                                    vrvacsubmit = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                    DataTable dtschgivenchk = ConvertToDataTable(vrvacsubmit);
                                                    if (dtschgivenchk.Rows.Count != 0)
                                                    {
                                                        archgiven = true;
                                                    }
                                                    else
                                                    {
                                                        #region==================================================================skip code starts ==============================================================
                                                        VR_SkipSchlist vrskiplist = new VR_SkipSchlist();
                                                        vrskiplist = VR_SkipschDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, vaccineid, "S");
                                                        DataTable dtskip = ConvertToDataTable(vrskiplist);
                                                        if (dtskip.Rows.Count != 0)
                                                        {
                                                            skipid = Convert.ToInt32(dtskip.Rows[0].ItemArray[0].ToString());
                                                            skipdate = Convert.ToDateTime(dtskip.Rows[0].ItemArray[6].ToString());
                                                            skipflag = true;
                                                        }
                                                        else
                                                        {
                                                            skipflag = false;
                                                        }

                                                        #endregion==================================================================skip code ends ==============================================================
                                                    }
                                                    #endregion =================================== code for give vaccine(Submit and remove) end =========================================================

                                                    #region ================ Improved code start for count the doses ===================================
                                                    if (givenchkflag == true)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {
                                                                noofgreenvaccines++;
                                                                dosecolor = "Green";
                                                            }
                                                        }
                                                    }
                                                    else if (skipflag == true || compulsoryflag == true || nodue == true)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                noofgrayvaccines++;
                                                                dosecolor = "Gray";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToDateTime(lblDueDateGivenimgchk) < DateTime.Now)
                                                        {
                                                            if (showdotsbooster == false)
                                                            {
                                                                dosecolor = "";
                                                            }
                                                            else
                                                            {
                                                                if (triggershow == true)
                                                                {
                                                                    // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                    noofredvaccines++;
                                                                    dosecolor = "Red";
                                                                }
                                                                else
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                            }
                                                        }
                                                        else if (Convert.ToDateTime(lblDueDateGivenimgchk) > DateTime.Now)
                                                        {
                                                            int days = ((Convert.ToDateTime(lblDueDateGivenimgchk) - DateTime.Now).Days);
                                                            if (days < orangedays)
                                                            {
                                                                if (showdotsbooster == false)
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                                else
                                                                {
                                                                    if (triggershow == true)
                                                                    {

                                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);

                                                                        nooforangevaccines++;
                                                                        dosecolor = "Orange";
                                                                    }
                                                                    else
                                                                    {
                                                                        dosecolor = "";
                                                                    }
                                                                }

                                                            }
                                                            else
                                                            {
                                                                if (showdotsbooster == false)
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                                else
                                                                {
                                                                    if (triggershow == true)
                                                                    {

                                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                        noofgrayvaccines++;
                                                                        dosecolor = "Gray";
                                                                    }
                                                                    else
                                                                    {
                                                                        dosecolor = "";
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion ================== improved code end for count the doses =============================

                                                    // Console.WriteLine("vacid=" + vaccineid);

                                                    if (archie == true && archgiven == true)
                                                    {
                                                        finalobject.schid = schid;
                                                        finalobject.vacid = vaccineid;
                                                        finalobject.childid = childid;
                                                        finalobject.catid = categoryid;
                                                        finalobject.doseno = doseno;
                                                        finalobject.gender = sex;
                                                        finalobject.countryid = countryid;
                                                        finalobject.stateid = stateid;
                                                        finalobject.vaccineyear = Vacyear;
                                                        finalobject.DueDays = d_days;
                                                        finalobject.DueMonths = d_months;
                                                        finalobject.DueYears = d_year;
                                                        finalobject.EndDays = e_days;
                                                        finalobject.EndMonth = e_months;
                                                        finalobject.EndYear = e_year;
                                                        finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                                        finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                                        finalobject.ReminderOnDate = reminderdate;
                                                        finalobject.givenid = givenid;
                                                        finalobject.DateGiven = givendate;
                                                        finalobject.skipid = skipid;
                                                        finalobject.skipdate = skipdate;
                                                        finalobject.set_as_previous_given = set_as_previous;
                                                        finalobject.dosegivenornot = givenchkflag;
                                                        finalobject.skipornot = skipflag;
                                                        finalobject.notcompulsory = compulsoryflag;
                                                        finalobject.triggershowflag = TriggerShowVac;
                                                        finalobject.nodueflag = TriggerNoDue;
                                                        finalobject.Booster = Convert.ToBoolean(booster);
                                                        finalobject.BoosterDoses = lblboosterdoses;
                                                        finalobject.dosecolour = dosecolor;
                                                        childsch.Add(finalobject);
                                                    }
                                                    else if (archie != true)
                                                    {
                                                        finalobject.schid = schid;
                                                        finalobject.vacid = vaccineid;
                                                        finalobject.childid = childid;
                                                        finalobject.catid = categoryid;
                                                        finalobject.doseno = doseno;
                                                        finalobject.gender = sex;
                                                        finalobject.countryid = countryid;
                                                        finalobject.stateid = stateid;
                                                        finalobject.vaccineyear = Vacyear;
                                                        finalobject.DueDays = d_days;
                                                        finalobject.DueMonths = d_months;
                                                        finalobject.DueYears = d_year;
                                                        finalobject.EndDays = e_days;
                                                        finalobject.EndMonth = e_months;
                                                        finalobject.EndYear = e_year;
                                                        finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                                        finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                                        finalobject.ReminderOnDate = reminderdate;
                                                        finalobject.givenid = givenid;
                                                        finalobject.DateGiven = givendate;
                                                        finalobject.skipid = skipid;
                                                        finalobject.skipdate = skipdate;
                                                        finalobject.set_as_previous_given = set_as_previous;
                                                        finalobject.dosegivenornot = givenchkflag;
                                                        finalobject.skipornot = skipflag;
                                                        finalobject.notcompulsory = compulsoryflag;
                                                        finalobject.triggershowflag = TriggerShowVac;
                                                        finalobject.nodueflag = TriggerNoDue;
                                                        finalobject.Booster = Convert.ToBoolean(booster);
                                                        finalobject.BoosterDoses = lblboosterdoses;
                                                        finalobject.dosecolour = dosecolor;
                                                        childsch.Add(finalobject);
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                            }

                                            #endregion ========================= Code end for normal schedule ==============================
                                        }
                                        else
                                        {
                                            #region ======================== code start for custom schedule =======================
                                            //VRSchedulelist vrschlist = new VRSchedulelist();
                                            //vrschlist = VRScheduleDA.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                            string quercust = "SELECT schedule_id,ISNULL(vaccine_id,'0') as vaccine_id ,ISNULL(country_id,'0') as country_id,ISNULL(Year,'0') as Year, ISNULL(Due_Days,'0') as Due_Days, ISNULL(Due_Months,'0') as Due_Months, ISNULL(Due_Years,'0') as Due_Years," +
     "ISNULL(End_Days,'0') as End_Days, ISNULL(End_Months,'0') as End_Months, ISNULL(End_Years,'0') as  End_Years, ISNULL(Dose_No,'0') as Dose_No,ISNULL(Dose_Name,'')  as Dose_Name, ISNULL(Set_as_Previous_Given,'false')  as Set_as_Previous_Given," +
 "ISNULL(Booster,'false')  as Booster,ISNULL(Booster_Doses,'')  as Booster_Doses,ISNULL(Dose_Desc,'') as Dose_Desc, ISNULL(NotCompulsary,'false')as NotCompulsary,ISNULL(stateid,'0')  as stateid,ISNULL(No_Due_Date,'false') as No_Due_Date" +
   " from  VRC_Schedule where vaccine_id='" + vaccineid + "' and userid='" + userid + "' order by Dose_No";
                                            SqlCommand cmdcssch = new SqlCommand(quercust, con);
                                            cmdcssch.CommandType = CommandType.Text;
                                            cmdcssch.ExecuteNonQuery();
                                            SqlDataAdapter adcust = new SqlDataAdapter(cmdcssch);
                                            DataTable dtschedule = new DataTable();
                                            adcust.Fill(dtschedule);
                                            if (dtschedule.Rows.Count != 0)
                                            {
                                                trigerschid.Clear();
                                                trigerdoseno.Clear();
                                                for (int p = 0; p < dtschedule.Rows.Count; p++)
                                                {
                                                    childvacsch finalobject = new childvacsch();
                                                    set_as_previous = Convert.ToBoolean(dtschedule.Rows[p].ItemArray[12].ToString());
                                                    schid = Convert.ToInt32(dtschedule.Rows[p].ItemArray[0].ToString());
                                                    doseno = Convert.ToInt32(dtschedule.Rows[p].ItemArray[10].ToString());
                                                    d_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[4].ToString());
                                                    d_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[5].ToString());
                                                    d_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[6].ToString());
                                                    e_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[7].ToString());
                                                    e_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[8].ToString());
                                                    e_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[9].ToString());
                                                    booster = dtschedule.Rows[p].ItemArray[13].ToString();
                                                    lblboosterdoses = dtschedule.Rows[p].ItemArray[14].ToString();


                                                    #region ========================= code start for dose details ================================
                                                    string queryorangedays = "select vacreminderid,userid,subdate,status,orangedaysetting,date_format from VaccineReminder_Settings where userid='" + userid + "' and status='True'";
                                                    SqlCommand cmdOD = new SqlCommand(queryorangedays, con);
                                                    cmdOD.CommandType = CommandType.Text;
                                                    SqlDataAdapter adOD = new SqlDataAdapter(cmdOD);
                                                    DataTable dtorangedays = new DataTable();
                                                    adOD.Fill(dtorangedays);
                                                    if (dtorangedays.Rows.Count != 0)
                                                    {
                                                        dateformat = dtorangedays.Rows[0].ItemArray[5].ToString();
                                                        orangedays = Convert.ToInt32(dtorangedays.Rows[0].ItemArray[4].ToString());
                                                    }
                                                    else
                                                    {
                                                        //take from default settings
                                                        string queryoddefault = "select defaultvacremid,subdate,status,orangedaysetting,date_format from Default_VacReminder_Setting where status='True'";
                                                        SqlCommand cmdODdefault = new SqlCommand(queryoddefault, con);
                                                        cmdODdefault.CommandType = CommandType.Text;
                                                        SqlDataAdapter adODdefault = new SqlDataAdapter(cmdODdefault);
                                                        DataTable dtdefaultorngdays = new DataTable();
                                                        adODdefault.Fill(dtdefaultorngdays);
                                                        if (dtdefaultorngdays.Rows.Count != 0)
                                                        {
                                                            dateformat = dtdefaultorngdays.Rows[0].ItemArray[4].ToString();
                                                            orangedays = Convert.ToInt32(dtdefaultorngdays.Rows[0].ItemArray[3].ToString());
                                                        }
                                                        else
                                                        {
                                                            dateformat = "dd MMM, yyyy";
                                                            orangedays = 15;
                                                        }
                                                    }
                                                    #endregion ============================ code end for dose details ===================================

                                                    #region =============== Code start for trigger schedule =====================
                                                    //  VR_TriggerList vrtriglist = new VR_TriggerList();
                                                    //vrtriglist = VR_TriggerScheDA.gridviewTriggerCidVacid(countryid, vaccineid);
                                                    string quercusttrig = "SELECT triggerschid, ISNULL(vaccine_id,'0') as vaccine_id,ISNULL(country_id,'0') as country_id, ISNULL(stateid,'0') as stateid," +
      "ISNULL(show_vaccine,'false') as show_vaccine, ISNULL(end_age,'0') as end_age, ISNULL(subdate,'') as subdate, ISNULL(NoDue,'false') as NoDue" +
       " from VRC_Trigger_Schedule where userid='" + userid + "' and vaccine_id='" + vaccineid + "'";
                                                    SqlCommand cmdcsschtrig = new SqlCommand(quercusttrig, con);
                                                    cmdcsschtrig.CommandType = CommandType.Text;
                                                    cmdcsschtrig.ExecuteNonQuery();
                                                    SqlDataAdapter adcusttrig = new SqlDataAdapter(cmdcsschtrig);
                                                    DataTable dttriggersch = new DataTable();
                                                    adcusttrig.Fill(dttriggersch);

                                                    #region ============ Code start for vacid present in trigger schedule =============
                                                    if (dttriggersch.Rows.Count != 0)  //vaccine id present in trigger schedule
                                                    {
                                                        TriggerShowVac = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[4].ToString());
                                                        TriggerNoDue = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[7].ToString());

                                                        string triggerquery = "select dateadd(YY,0,dateadd(M,Convert(int,'" + dttriggersch.Rows[0].ItemArray[5].ToString() + "'),dateadd(dd,0,'" + dob.ToString("yyyy-MM-dd") + "')))";
                                                        SqlCommand cmdtrig = new SqlCommand(triggerquery, con);
                                                        cmdtrig.CommandType = CommandType.Text;
                                                        SqlDataAdapter adtrig = new SqlDataAdapter(cmdtrig);
                                                        DataTable dttrigerprevgivendt = new DataTable();
                                                        adtrig.Fill(dttrigerprevgivendt);
                                                        if (TriggerShowVac == true)
                                                        {
                                                            if (TriggerNoDue == true)
                                                            {
                                                                if (doseno == 1)
                                                                {
                                                                    // VR_GivenList vrgivenlist = new VR_GivenList();
                                                                    //vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                    string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                                    SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                                    cmdcsschgiv.CommandType = CommandType.Text;
                                                                    cmdcsschgiv.ExecuteNonQuery();
                                                                    SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                                    DataTable dtgiven = new DataTable();
                                                                    adcustgiv.Fill(dtgiven);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }

                                                                }
                                                                else if (doseno > 1)
                                                                {
                                                                    // VR_GivenList vrgivenlist = new VR_GivenList();
                                                                    //  vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                                    string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
     "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
      " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                                    SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                                    cmdcsschgiv.CommandType = CommandType.Text;
                                                                    cmdcsschgiv.ExecuteNonQuery();
                                                                    SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                                    DataTable dtgiven = new DataTable();
                                                                    adcustgiv.Fill(dtgiven);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }
                                                                }
                                                            }
                                                            else if (TriggerNoDue == false)
                                                            {
                                                                if (doseno == 1)
                                                                {

                                                                }
                                                                else if (doseno > 1)
                                                                {
                                                                    string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                                    SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                                    cmdcsschgiv.CommandType = CommandType.Text;
                                                                    cmdcsschgiv.ExecuteNonQuery();
                                                                    SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                                    DataTable dtgiven = new DataTable();
                                                                    adcustgiv.Fill(dtgiven);
                                                                    if (dtgiven.Rows.Count != 0)
                                                                    {
                                                                    }
                                                                    else
                                                                    {
                                                                        nodue = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (TriggerShowVac == false)
                                                        {
                                                            if (doseno == 1)
                                                            {
                                                                trigerschid.Add(schid);
                                                                trigerdoseno.Add(doseno);
                                                                triggershow = true;
                                                            }
                                                            else if (doseno > 1)
                                                            {
                                                                // VR_GivenList vrgivenlist = new VR_GivenList();
                                                                // vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");

                                                                string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                                SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                                cmdcsschgiv.CommandType = CommandType.Text;
                                                                cmdcsschgiv.ExecuteNonQuery();
                                                                SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                                DataTable dttrigergiven = new DataTable();
                                                                adcustgiv.Fill(dttrigergiven);

                                                                //DataTable dttrigergiven = ConvertToDataTable(vrgivenlist);
                                                                if (dttrigergiven.Rows.Count != 0)
                                                                {
                                                                    if (Convert.ToDateTime(dttrigergiven.Rows[0].ItemArray[4].ToString()) > Convert.ToDateTime(dttrigerprevgivendt.Rows[0].ItemArray[0].ToString()))
                                                                    {
                                                                        triggershow = false;

                                                                    }
                                                                    else
                                                                    {
                                                                        triggershow = true;

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    int schidindex = trigerschid.Count - 1;
                                                                    //  VR_GivenList vrgivenlist1 = new VR_GivenList();
                                                                    // vrgivenlist1 = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, Convert.ToInt32(trigerschid[schidindex].ToString()), Convert.ToInt32(trigerdoseno[schidindex].ToString()), "S");
                                                                    string quercustgiv1 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
   "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
    " from VRC_Given where schedule_id='" + trigerschid[schidindex].ToString() + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + trigerdoseno[schidindex].ToString() + "'";
                                                                    SqlCommand cmdcsschgiv1 = new SqlCommand(quercustgiv1, con);
                                                                    cmdcsschgiv1.CommandType = CommandType.Text;
                                                                    cmdcsschgiv1.ExecuteNonQuery();
                                                                    SqlDataAdapter adcustgiv1 = new SqlDataAdapter(cmdcsschgiv1);
                                                                    DataTable dttrigergiven1 = new DataTable();
                                                                    adcustgiv1.Fill(dttrigergiven1);

                                                                    // DataTable dttrigergiven1 = ConvertToDataTable(vrgivenlist1);
                                                                    if (dttrigergiven1.Rows.Count != 0)
                                                                    {
                                                                        triggershow = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        triggershow = false;
                                                                    }
                                                                }
                                                                trigerschid.Add(schid);
                                                                trigerdoseno.Add(doseno);

                                                                // triggershow = true;
                                                            }
                                                        }
                                                    }

                                                    #endregion =========== code end for vacid present in trigger schedule ============

                                                    #region =========== Code start for vacid  not present in trigger schdule ===========
                                                    else
                                                    {

                                                    }
                                                    #endregion ============= Code end for vacid not present in trigger schedule ============

                                                    #endregion ============ Code end for trigger schedule =======================

                                                    #region ======Code start for set as previous given ==============
                                                    if (doseno == 1)
                                                    {
                                                        Listpreviousgiven.Clear();
                                                        Listpreviousvalues.Clear();
                                                        Listpreviousschid.Clear();
                                                        Listprevioustbltype.Clear();
                                                        Listpreviousvacid.Clear();
                                                        Listpreviousduedate.Clear();
                                                        Listpreviousenddate.Clear();
                                                        Listpreviousgivendate.Clear();

                                                        //  VR_GivenList vrpregiven = new VR_GivenList();
                                                        // vrpregiven = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                        //  DataTable dtpregiven = ConvertToDataTable(vrpregiven);

                                                        string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                        SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                        cmdcsschgiv.CommandType = CommandType.Text;
                                                        cmdcsschgiv.ExecuteNonQuery();
                                                        SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                        DataTable dtpregiven = new DataTable();
                                                        adcustgiv.Fill(dtpregiven);

                                                        #region =======Code start dose no one present in vr_given =================
                                                        if (dtpregiven.Rows.Count != 0)//vaccine id dose is already given 
                                                        {
                                                            bday = dob.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgiven.Add(dtpregiven.Rows[0].ItemArray[4].ToString());
                                                            givenchkflag = true;

                                                            lblduedategivenbooster = dtpregiven.Rows[0].ItemArray[4].ToString();
                                                            Listpreviousvalues.Add(dtpregiven.Rows[0].ItemArray[4].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);

                                                            Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                            firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgivendate.Add(dtpregiven.Rows[0].ItemArray[4].ToString());


                                                            //List add for database 
                                                            ListVacgivenornot.Add("Yes");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);
                                                        }
                                                        #endregion ========== code end for dose no 1 present in vr_given ================

                                                        #region ====== Code start for dose no 1 not present in vr_given ========
                                                        else
                                                        {
                                                            bday = dob.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {

                                                            }
                                                            givenchkflag = false;
                                                            Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);
                                                            lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            lblduedategivenbooster = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                            firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                            Listpreviousgivendate.Add(null);


                                                            //List add for database 
                                                            ListVacgivenornot.Add("No");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);
                                                        }
                                                        #endregion ========= Code end dose1 not present in vr_given==============
                                                    }
                                                    else if (doseno > 1)
                                                    {
                                                        for (int s = 0; s < Listpreviousduedate.Count; s++)
                                                        {
                                                            lblprevduedatedisplay = "";
                                                            int j = Listpreviousduedate.Count;
                                                            if (Listpreviousduedate[j - 1].ToString() != "")
                                                            {
                                                                lblprevduedatedisplay = Listpreviousduedate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }

                                                        for (int s = 0; s < Listpreviousenddate.Count; s++)
                                                        {
                                                            lblprevenddatedisplay = "";
                                                            int j = Listpreviousenddate.Count;
                                                            if (Listpreviousenddate[j - 1].ToString() != "")
                                                            {
                                                                lblprevenddatedisplay = Listpreviousenddate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listpreviousgivendate.Count; s++)
                                                        {
                                                            lblprevgivendatedisplay = "";
                                                            int j = Listpreviousgivendate.Count;
                                                            if (Listpreviousgivendate[j - 1] != null)
                                                            {
                                                                lblprevgivendatedisplay = Listpreviousgivendate[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listpreviousschid.Count; s++)
                                                        {
                                                            lblprevschids = "";
                                                            //Response.Write(Listpreviousschid[i].ToString());
                                                            int j = Listpreviousschid.Count;
                                                            // Response.Write(Listpreviousschid.Count + "j= "+ j );
                                                            if (Listpreviousschid[j - 1].ToString() != "")
                                                            {
                                                                lblprevschids = Listpreviousschid[j - 1].ToString();
                                                                break;
                                                            }

                                                        }
                                                        for (int s = 0; s < Listprevioustbltype.Count; s++)
                                                        {
                                                            lblprevtbltypes = "";
                                                            int j = Listprevioustbltype.Count;
                                                            if (Listprevioustbltype[j - 1].ToString() != "")
                                                            {
                                                                lblprevtbltypes = Listprevioustbltype[j - 1].ToString();
                                                                break;
                                                            }

                                                        }

                                                        #region ==================================================  set as previous given is true and false code starts =============================================

                                                        string currentdoseduedate = string.Empty;

                                                        if (set_as_previous == true)//set as previous given true
                                                        {
                                                            // VR_GivenList vrpregiven1 = new VR_GivenList();
                                                            //vrpregiven1 = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(lblprevschids), "S");
                                                            //DataTable dtsgivendt1 = ConvertToDataTable(vrpregiven1);
                                                            // Response.Write("previous schid = " + lblprevschids.Text);

                                                            string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + lblprevschids + "' and child_id='" + childid + "' and from_table='S'";
                                                            SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                            cmdcsschgiv.CommandType = CommandType.Text;
                                                            cmdcsschgiv.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                            DataTable dtsgivendt1 = new DataTable();
                                                            adcustgiv.Fill(dtsgivendt1);


                                                            if (dtsgivendt1.Rows.Count != 0)
                                                            {
                                                                if (nodue == true)
                                                                {
                                                                    nodue = false;
                                                                }
                                                            }

                                                            if (lblprevgivendatedisplay != "")
                                                            {
                                                                if (Convert.ToDateTime(lblprevgivendatedisplay) <= Convert.ToDateTime(lblprevenddatedisplay))
                                                                {
                                                                    currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                    cmdbday.CommandType = CommandType.Text;
                                                                    cmdbday.ExecuteNonQuery();
                                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                    DataTable dtfirstdosedate = new DataTable();
                                                                    adbday.Fill(dtfirstdosedate);

                                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                                    {
                                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    //  Response.Write(lblprevgivendatedisplay.Text);
                                                                    currentdosedate = Convert.ToDateTime(lblprevgivendatedisplay);
                                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                    cmdbday.CommandType = CommandType.Text;
                                                                    cmdbday.ExecuteNonQuery();
                                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                    DataTable dtfirstdosedate = new DataTable();
                                                                    adbday.Fill(dtfirstdosedate);

                                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                                    {
                                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                        // Response.Write(currentdoseduedate);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                                Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                                SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                                cmdbday.CommandType = CommandType.Text;
                                                                cmdbday.ExecuteNonQuery();
                                                                SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                                DataTable dtfirstdosedate = new DataTable();
                                                                adbday.Fill(dtfirstdosedate);

                                                                if (dtfirstdosedate.Rows.Count != 0)
                                                                {
                                                                    currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }
                                                        }

                                                        // VR_GivenList vrpregiven = new VR_GivenList();
                                                        //  vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid, schid, "S");
                                                        //DataTable dtsgivendt = ConvertToDataTable(vrpregiven);

                                                        string quercustgiv1 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
   "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
    " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S'";
                                                        SqlCommand cmdcsschgiv1 = new SqlCommand(quercustgiv1, con);
                                                        cmdcsschgiv1.CommandType = CommandType.Text;
                                                        cmdcsschgiv1.ExecuteNonQuery();
                                                        SqlDataAdapter adcustgiv1 = new SqlDataAdapter(cmdcsschgiv1);
                                                        DataTable dtsgivendt = new DataTable();
                                                        adcustgiv1.Fill(dtsgivendt);

                                                        if (dtsgivendt.Rows.Count != 0)
                                                        {
                                                            dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                            bday = dob1.ToString("yyyy-MM-dd");
                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                lblDueDateGivenimgchk = currentdoseduedate;
                                                            }
                                                            Listpreviousgiven.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());
                                                            givenchkflag = true;

                                                            Listpreviousvalues.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());
                                                            Listpreviousschid.Add(schid);
                                                            Listprevioustbltype.Add("S");
                                                            Listpreviousvacid.Add(vaccineid);

                                                            lblduedategivenbooster = dtsgivendt.Rows[0].ItemArray[4].ToString();
                                                            Listpreviousduedate.Add(currentdoseduedate);
                                                            currentdosedate = Convert.ToDateTime(currentdoseduedate);

                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                            SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                            cmdend.CommandType = CommandType.Text;
                                                            cmdend.ExecuteNonQuery();
                                                            SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                            DataTable dtduedosedate = new DataTable();
                                                            adenddate.Fill(dtduedosedate);
                                                            if (dtduedosedate.Rows.Count != 0)
                                                            {
                                                                // lblduedateaftergiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                                Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                            }

                                                            Listpreviousgivendate.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());

                                                            //List add for database 
                                                            ListVacgivenornot.Add("Yes");
                                                            ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                            ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            Listvacid.Add(vaccineid);
                                                            Listschid.Add(schid);
                                                            ListVaccinedosenumber.Add(doseno);
                                                            Listcatid.Add(categoryid);

                                                        }
                                                        else
                                                        {
                                                            dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                            bday = dob1.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {

                                                                Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                Listpreviousschid.Add(schid);
                                                                Listprevioustbltype.Add("S");
                                                                Listpreviousvacid.Add(vaccineid);

                                                                Listpreviousgiven.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                lblduedategivenbooster = currentdoseduedate;
                                                                lblDueDateGivenimgchk = currentdoseduedate;
                                                                Listpreviousduedate.Add(currentdoseduedate);
                                                                currentdosedate = Convert.ToDateTime(currentdoseduedate);
                                                                Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                                string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + Curdosedate + "')))";
                                                                SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                                cmdend.CommandType = CommandType.Text;
                                                                cmdend.ExecuteNonQuery();
                                                                SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                                DataTable dtduedosedate = new DataTable();
                                                                adenddate.Fill(dtduedosedate);
                                                                if (dtduedosedate.Rows.Count != 0)
                                                                {
                                                                    // lblDueDateGiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                                    Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                    lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                                }
                                                                Listpreviousgivendate.Add(null);

                                                                //List add for database 
                                                                ListVacgivenornot.Add("No");
                                                                ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                                ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                                Listvacid.Add(vaccineid);
                                                                Listschid.Add(schid);
                                                                ListVaccinedosenumber.Add(doseno);
                                                                Listcatid.Add(categoryid);
                                                            }
                                                        }
                                                        #endregion =============================================== set as previous given code ends ===============================================

                                                    }

                                                    #endregion ======== Code end for set as previous given ============

                                                    #region =================Code start for Vacid present in vr_given ====================================
                                                    //VR_GivenList givenlist = new VR_GivenList();
                                                    //givenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                    // DataTable dtgiven1 = ConvertToDataTable(givenlist);
                                                    string quercustgiv2 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
   "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
    " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                    SqlCommand cmdcsschgiv2 = new SqlCommand(quercustgiv2, con);
                                                    cmdcsschgiv2.CommandType = CommandType.Text;
                                                    cmdcsschgiv2.ExecuteNonQuery();
                                                    SqlDataAdapter adcustgiv2 = new SqlDataAdapter(cmdcsschgiv2);
                                                    DataTable dtgiven1 = new DataTable();
                                                    adcustgiv2.Fill(dtgiven1);

                                                    if (dtgiven1.Rows.Count != 0)
                                                    {
                                                        givenid = Convert.ToInt32(dtgiven1.Rows[0].ItemArray[0].ToString());
                                                        givendate = Convert.ToDateTime(dtgiven1.Rows[0].ItemArray[4].ToString());
                                                        // dosegivenornot ="Yes";
                                                        givenchkflag = true;
                                                    }
                                                    else
                                                    {
                                                        givenchkflag = false;
                                                        givendate = DateTime.Now;
                                                        givenid = 0;
                                                        // dosegivenornot="No";
                                                    }
                                                    #endregion ================ Code end for Vacid present in vr_given  ==================================

                                                    #region ======================================= booster dose code starts ========================================================================================

                                                    bool showdotsbooster = true;
                                                    if (booster == "True")
                                                    {
                                                        bool chkgiven = false;
                                                        if (Convert.ToDateTime(lblduedategivenbooster) > DateTime.Now)
                                                        {
                                                            showdotsbooster = false;
                                                        }
                                                        else
                                                        {
                                                            string strdate = null;
                                                            string[] strArrdate = null;

                                                            //int count = 0;
                                                            strdate = lblboosterdoses;
                                                            char[] splitchar11 = { ',' };


                                                            strArrdate = strdate.Split(splitchar11);

                                                            for (int b = 0; b < strArrdate.Length; b++)
                                                            {
                                                                for (int s = 0; s <= b - 1; s++)
                                                                {
                                                                    // VR_GivenList vrpregiven = new VR_GivenList();
                                                                    //  vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(Listpreviousschid[s]), "S");
                                                                    // DataTable dtboster = ConvertToDataTable(vrpregiven);

                                                                    string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
     " from VRC_Given where schedule_id='" + Listpreviousschid[s] + "' and child_id='" + childid + "' and from_table='S'";
                                                                    SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                                    cmdcsschgiv.CommandType = CommandType.Text;
                                                                    cmdcsschgiv.ExecuteNonQuery();
                                                                    SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                                    DataTable dtboster = new DataTable();
                                                                    adcustgiv.Fill(dtboster);

                                                                    if (dtboster.Rows.Count != 0)
                                                                    {
                                                                        chkgiven = true;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        chkgiven = false;
                                                                    }
                                                                    if (chkgiven == true)
                                                                        break;
                                                                }
                                                            }

                                                            if (chkgiven == true)
                                                            {

                                                                showdotsbooster = false;
                                                            }
                                                            else
                                                            {

                                                                showdotsbooster = true;
                                                            }

                                                        }
                                                    }

                                                    #endregion ==================================== booster dose code ends ==========================================================================================

                                                    #region =================Code start for Not Compulsory ==========================================
                                                    //VRSchedulelist vrsch = new VRSchedulelist();
                                                    //vrsch = VRScheduleDA.gridviewvacschdoseno(schid, vaccineid, doseno, countryid, stateid, Vacyear);
                                                    // DataTable dtnotcosly = ConvertToDataTable(vrsch);

                                                    string quercust1 = "SELECT schedule_id,ISNULL(vaccine_id,'') as vaccine_id , ISNULL( country_id,'') as country_id, ISNULL( Year,'') as Year,ISNULL( Due_Days,'') as Due_Days," +
     "ISNULL(Due_Months,'') as Due_Months,ISNULL( Due_Years,'') as Due_Years,ISNULL( End_Days,'') as End_Days,ISNULL( End_Months,'') as End_Months,ISNULL( End_Years,'') as End_Years," +
     "ISNULL( Dose_No,'') as Dose_No,ISNULL( Dose_Name,'') as Dose_Name,ISNULL( Set_as_Previous_Given,'') as Set_as_Previous_Given,ISNULL( Booster,'') as Booster," +
     "ISNULL( Booster_Doses,'') as Booster_Doses,ISNULL( Dose_Desc,'') as Dose_Desc, ISNULL( NotCompulsary,'') as NotCompulsary,ISNULL( stateid,'') as stateid,ISNULL( No_Due_Date,'') as No_Due_Date" +
    " from VRC_Schedule where schedule_id='" + schid + "' and vaccine_id='" + vaccineid + "' and Dose_No='" + doseno + "' and userid='" + userid + "'";
                                                    SqlCommand cmdcssch1 = new SqlCommand(quercust1, con);
                                                    cmdcssch1.CommandType = CommandType.Text;
                                                    cmdcssch1.ExecuteNonQuery();
                                                    SqlDataAdapter adcust1 = new SqlDataAdapter(cmdcssch1);
                                                    DataTable dtnotcosly = new DataTable();
                                                    adcust1.Fill(dtnotcosly);

                                                    if (dtnotcosly.Rows.Count != 0)
                                                    {
                                                        if (dtnotcosly.Rows[0].ItemArray[16].ToString() == "True")
                                                        {
                                                            compulsoryflag = true;
                                                        }
                                                        else
                                                        {
                                                            compulsoryflag = false;
                                                        }
                                                    }
                                                    #endregion ============================ code end for Not compulsory =============================

                                                    #region ================== code start for reminder on ================================

                                                    //Code start for reminder set on 

                                                    bool reminder1set = false;
                                                    bool reminder2set = false;
                                                    string givenreminderdate = lblDueDateGivenimgchk;

                                                    VR_ReminderChangeslist vrrchnglist = new VR_ReminderChangeslist();
                                                    vrrchnglist = VR_ReminderChngDAController.gridviewscheduleid(userid, childid, vaccineid, schid, doseno, "S");
                                                    DataTable dtchngreminder = ConvertToDataTable(vrrchnglist);

                                                    if (dtchngreminder.Rows.Count != 0)
                                                    {
                                                        if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "1")
                                                        {
                                                            //string date1 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                            ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                            reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                        }
                                                        else if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "2")
                                                        {
                                                            // string date2 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                            ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                            reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                        }
                                                    }
                                                    else
                                                    {
                                                        VR_ReminderSettingList vrremindsetting = new VR_ReminderSettingList();
                                                        vrremindsetting = VR_ReminderSettingDAController.gridviewuid(userid, "True");
                                                        DataTable dtRemindsetng = ConvertToDataTable(vrremindsetting);
                                                        if (dtRemindsetng.Rows.Count != 0)
                                                        {
                                                            if (reminder1set == false)
                                                            {
                                                                string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                ListReminderondate.Add(date1);
                                                                reminderdate = Convert.ToDateTime(date1);
                                                            }
                                                            else
                                                            {
                                                                if (dtRemindsetng.Rows[0].ItemArray[6].ToString() == "True")
                                                                {
                                                                    reminder2set = true;
                                                                    if (reminder2set == true)
                                                                    {
                                                                        string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[3].ToString())).ToShortDateString();
                                                                        ListReminderondate.Add(date2);
                                                                        reminderdate = Convert.ToDateTime(date2);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    reminder2set = false;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            VR_DefaultSettingLsit vrdefalutlist = new VR_DefaultSettingLsit();
                                                            vrdefalutlist = VR_DefaultSettingDAController.gridview();
                                                            DataTable dtdefaultsetng = ConvertToDataTable(vrdefalutlist);
                                                            if (dtdefaultsetng.Rows.Count != 0)
                                                            {
                                                                if (reminder1set == false)
                                                                {
                                                                    string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[1].ToString())).ToShortDateString();
                                                                    ListReminderondate.Add(date1);
                                                                    reminderdate = Convert.ToDateTime(date1);
                                                                }
                                                                else
                                                                {
                                                                    if (dtdefaultsetng.Rows[0].ItemArray[5].ToString() == "True")
                                                                    {
                                                                        reminder2set = true;
                                                                        if (reminder2set == true)
                                                                        {
                                                                            string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                            ListReminderondate.Add(date2);
                                                                            reminderdate = Convert.ToDateTime(date2);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        reminder2set = false;

                                                                    }

                                                                }

                                                            }

                                                        }

                                                    }


                                                    #endregion ================= end code for reminder on ================================

                                                    #region ===================================== code for give vaccine(Submit and remove) start ========================================================

                                                    //sVR_GivenList vrvacsubmit = new VR_GivenList();
                                                    //  vrvacsubmit = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                    //   DataTable dtschgivenchk = ConvertToDataTable(vrvacsubmit);

                                                    string quercust3 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
    " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                    SqlCommand cmdcssch3 = new SqlCommand(quercust3, con);
                                                    cmdcssch3.CommandType = CommandType.Text;
                                                    cmdcssch3.ExecuteNonQuery();
                                                    SqlDataAdapter adcust3 = new SqlDataAdapter(cmdcssch3);
                                                    DataTable dtschgivenchk = new DataTable();
                                                    adcust3.Fill(dtschgivenchk);

                                                    if (dtschgivenchk.Rows.Count != 0)
                                                    {
                                                    }
                                                    else
                                                    {
                                                        #region==================================================================skip code starts ==============================================================
                                                        //VR_SkipSchlist vrskiplist = new VR_SkipSchlist();
                                                        //vrskiplist = VR_SkipschDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, vaccineid, "S");
                                                        // DataTable dtskip = ConvertToDataTable(vrskiplist);

                                                        string quercust2 = "SELECT skipid,ISNULL(schedule_id,'0')as schedule_id ,ISNULL(vaccine_id,'0') as vaccine_id,ISNULL(child_id,'0') as child_id," +
    "ISNULL(from_table,'') as  from_table,ISNULL(Dose_No,'0') as Dose_No,ISNULL(skipdate,'') as skipdate from vrc_skip " +
     "where schedule_id='" + schid + "' and vaccine_id='" + vaccineid + "' and child_id='" + childid + "' and Dose_No='" + doseno + "' and from_table='S'";
                                                        SqlCommand cmdcssch2 = new SqlCommand(quercust2, con);
                                                        cmdcssch2.CommandType = CommandType.Text;
                                                        cmdcssch2.ExecuteNonQuery();
                                                        SqlDataAdapter adcust2 = new SqlDataAdapter(cmdcssch2);
                                                        DataTable dtskip = new DataTable();
                                                        adcust2.Fill(dtskip);

                                                        if (dtskip.Rows.Count != 0)
                                                        {
                                                            skipid = Convert.ToInt32(dtskip.Rows[0].ItemArray[0].ToString());
                                                            skipdate = Convert.ToDateTime(dtskip.Rows[0].ItemArray[6].ToString());
                                                            skipflag = true;
                                                        }
                                                        else
                                                        {
                                                            skipflag = false;
                                                        }

                                                        #endregion==================================================================skip code ends ==============================================================
                                                    }
                                                    #endregion =================================== code for give vaccine(Submit and remove) end =========================================================

                                                    #region ================ Improved code start for count the doses ===================================
                                                    if (givenchkflag == true)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {
                                                                noofgreenvaccines++;
                                                                dosecolor = "Green";
                                                            }
                                                        }
                                                    }
                                                    else if (skipflag == true || compulsoryflag == true || nodue == true)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                noofgrayvaccines++;
                                                                dosecolor = "Gray";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Convert.ToDateTime(lblDueDateGivenimgchk) < DateTime.Now)
                                                        {
                                                            if (showdotsbooster == false)
                                                            {
                                                                dosecolor = "";
                                                            }
                                                            else
                                                            {
                                                                if (triggershow == true)
                                                                {
                                                                    // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                    noofredvaccines++;
                                                                    dosecolor = "Red";
                                                                }
                                                                else
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                            }
                                                        }
                                                        else if (Convert.ToDateTime(lblDueDateGivenimgchk) > DateTime.Now)
                                                        {
                                                            int days = ((Convert.ToDateTime(lblDueDateGivenimgchk) - DateTime.Now).Days);
                                                            if (days < orangedays)
                                                            {
                                                                if (showdotsbooster == false)
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                                else
                                                                {
                                                                    if (triggershow == true)
                                                                    {

                                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);

                                                                        nooforangevaccines++;
                                                                        dosecolor = "Orange";
                                                                    }
                                                                    else
                                                                    {
                                                                        dosecolor = "";
                                                                    }
                                                                }

                                                            }
                                                            else
                                                            {
                                                                if (showdotsbooster == false)
                                                                {
                                                                    dosecolor = "";
                                                                }
                                                                else
                                                                {
                                                                    if (triggershow == true)
                                                                    {

                                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                        noofgrayvaccines++;
                                                                        dosecolor = "Gray";
                                                                    }
                                                                    else
                                                                    {
                                                                        dosecolor = "";
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    #endregion ================== improved code end for count the doses =============================

                                                    Console.WriteLine("vacid=" + vaccineid);
                                                    finalobject.schid = schid;
                                                    finalobject.vacid = vaccineid;
                                                    finalobject.childid = childid;
                                                    finalobject.catid = categoryid;
                                                    finalobject.doseno = doseno;
                                                    finalobject.gender = sex;
                                                    finalobject.countryid = countryid;
                                                    finalobject.stateid = stateid;
                                                    finalobject.vaccineyear = Vacyear;
                                                    finalobject.DueDays = d_days;
                                                    finalobject.DueMonths = d_months;
                                                    finalobject.DueYears = d_year;
                                                    finalobject.EndDays = e_days;
                                                    finalobject.EndMonth = e_months;
                                                    finalobject.EndYear = e_year;
                                                    finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                                    finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                                    finalobject.ReminderOnDate = reminderdate;
                                                    finalobject.givenid = givenid;
                                                    finalobject.DateGiven = givendate;
                                                    finalobject.skipid = skipid;
                                                    finalobject.skipdate = skipdate;
                                                    finalobject.set_as_previous_given = set_as_previous;
                                                    finalobject.dosegivenornot = givenchkflag;
                                                    finalobject.skipornot = skipflag;
                                                    finalobject.notcompulsory = compulsoryflag;
                                                    finalobject.triggershowflag = TriggerShowVac;
                                                    finalobject.nodueflag = TriggerNoDue;
                                                    finalobject.Booster = Convert.ToBoolean(booster);
                                                    finalobject.BoosterDoses = lblboosterdoses;
                                                    finalobject.dosecolour = dosecolor;
                                                    childsch.Add(finalobject);
                                                }
                                            }

                                            #endregion ======================= code end for custom schedule =========================
                                        }
                                    }
                                    #endregion =========== Code end for the vacid is present in VROpted ===============

                                    #region ===== Code start for the vacid is not present in VROpted table =================
                                    else
                                    {
                                        //  int vaccineid = Convert.ToInt32(dtvacids.Rows[vid].ItemArray[0].ToString());
                                        childvacsch finalobject = new childvacsch();
                                        #region ========================comment code =========================
                                        //VRSchedulelist vrschlist = new VRSchedulelist();
                                        //vrschlist = VRScheduleDA.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                        //DataTable dtschedule = ConvertToDataTable(vrschlist);
                                        //if (dtschedule.Rows.Count != 0)
                                        //{
                                        //    for (int p = 0; p < dtschedule.Rows.Count; p++)
                                        //    {
                                        //        set_as_previous = Convert.ToBoolean(dtschedule.Rows[p].ItemArray[12].ToString());
                                        //        schid =Convert.ToInt32(dtschedule.Rows[p].ItemArray[0].ToString());
                                        //        doseno=Convert.ToInt32(dtschedule.Rows[p].ItemArray[7].ToString());
                                        //        d_days =Convert.ToInt32(dtschedule.Rows[p].ItemArray[4].ToString());
                                        //        d_months =Convert.ToInt32(dtschedule.Rows[p].ItemArray[5].ToString());
                                        //        d_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[6].ToString());
                                        //        e_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[8].ToString());
                                        //        e_months =Convert.ToInt32(dtschedule.Rows[p].ItemArray[9].ToString());
                                        //        e_year=Convert.ToInt32(dtschedule.Rows[p].ItemArray[10].ToString());
                                        //        booster = dtschedule.Rows[p].ItemArray[13].ToString();
                                        //        lblboosterdoses = dtschedule.Rows[p].ItemArray[14].ToString();


                                        //        #region ========================= code start for dose details ================================
                                        //        string queryorangedays = "select vacreminderid,userid,subdate,status,orangedaysetting,date_format from VaccineReminder_Settings where userid='"+ userid +"' and status='True'";
                                        //        SqlCommand cmdOD = new SqlCommand(queryorangedays, con);
                                        //        cmdOD.CommandType = CommandType.Text;
                                        //        SqlDataAdapter adOD = new SqlDataAdapter(cmdOD);
                                        //        DataTable dtorangedays = new DataTable();
                                        //        adOD.Fill(dtorangedays);
                                        //        if (dtorangedays.Rows.Count != 0)
                                        //        {
                                        //           dateformat = dtorangedays.Rows[0].ItemArray[5].ToString();
                                        //            orangedays = Convert.ToInt32(dtorangedays.Rows[0].ItemArray[4].ToString());
                                        //        }
                                        //        else
                                        //        {
                                        //            //take from default settings
                                        //           string queryoddefault = "select defaultvacremid,subdate,status,orangedaysetting,date_format from Default_VacReminder_Setting where status='True'";
                                        //           SqlCommand cmdODdefault = new SqlCommand(queryoddefault, con);
                                        //        cmdODdefault.CommandType = CommandType.Text;
                                        //        SqlDataAdapter adODdefault = new SqlDataAdapter(cmdODdefault);
                                        //        DataTable dtdefaultorngdays = new DataTable();
                                        //        adODdefault.Fill(dtdefaultorngdays);
                                        //            if (dtdefaultorngdays.Rows.Count != 0)
                                        //            {
                                        //               dateformat = dtdefaultorngdays.Rows[0].ItemArray[4].ToString();
                                        //                orangedays = Convert.ToInt32(dtdefaultorngdays.Rows[0].ItemArray[3].ToString());
                                        //            }
                                        //            else
                                        //            {
                                        //                dateformat= "dd MMM, yyyy";
                                        //                orangedays = 15;
                                        //            }
                                        //        }
                                        //        #endregion ============================ code end for dose details ===================================

                                        //        #region =============== Code start for trigger schedule =====================

                                        //        VR_TriggerList vrtriglist = new VR_TriggerList();
                                        //        vrtriglist = VR_TriggerScheDA.gridviewTriggerCidVacid(countryid, vaccineid);
                                        //        DataTable dttriggersch = ConvertToDataTable(vrtriglist);

                                        //        #region ============ Code start for vacid present in trigger schedule =============
                                        //        if (dttriggersch.Rows.Count != 0)  //vaccine id present in trigger schedule
                                        //        {
                                        //            TriggerShowVac = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[4].ToString());
                                        //            TriggerNoDue = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[7].ToString());
                                        //            if (TriggerShowVac == true)
                                        //            {
                                        //                if (TriggerNoDue == true)
                                        //                {
                                        //                    if (doseno == 1)
                                        //                    { 
                                        //                         VR_GivenList vrgivenlist = new VR_GivenList();
                                        //                        vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid,schid, doseno, "S");
                                        //                        DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                        //                        if (dtgiven.Rows.Count != 0)
                                        //                        {
                                        //                        }
                                        //                        else
                                        //                        {
                                        //                            nodue = true;
                                        //                        }

                                        //                    }
                                        //                    else if (doseno > 1)
                                        //                    {
                                        //                        VR_GivenList vrgivenlist = new VR_GivenList();
                                        //                        vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                        //                        DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                        //                        if (dtgiven.Rows.Count != 0)
                                        //                        {
                                        //                        }
                                        //                        else
                                        //                        {
                                        //                            nodue = true;
                                        //                        }
                                        //                    }
                                        //                }
                                        //                else if(TriggerNoDue==false)
                                        //                {
                                        //                    if (doseno == 1)
                                        //                    { 

                                        //                    }
                                        //                    else if (doseno > 1)
                                        //                    {
                                        //                        VR_GivenList vrgivenlist = new VR_GivenList();
                                        //                        vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                        //                        DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                        //                        if (dtgiven.Rows.Count != 0)
                                        //                        {
                                        //                        }
                                        //                        else
                                        //                        {
                                        //                            nodue = true;
                                        //                        }
                                        //                    }
                                        //                }
                                        //            }
                                        //            else if(TriggerShowVac==false)
                                        //            {
                                        //                int trigschid = 0;
                                        //                int trigdoseno = 0;
                                        //                if (doseno == 1)
                                        //                {
                                        //                    trigschid = schid;
                                        //                    trigdoseno = doseno;
                                        //                    triggershow = true;
                                        //                }
                                        //                else if (doseno > 1)
                                        //                {
                                        //                    VR_GivenList vrgivenlist = new VR_GivenList();
                                        //                    vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                        //                    DataTable dtgiven = ConvertToDataTable(vrgivenlist);
                                        //                    if (dtgiven.Rows.Count != 0)
                                        //                    {
                                        //                    }
                                        //                    else
                                        //                    {
                                        //                        nodue = true;
                                        //                    }
                                        //                }

                                        //            }
                                        //        }
                                        //        #endregion =========== code end for vacid present in trigger schedule ============

                                        //        #region =========== Code start for vacid  not present in trigger schdule ===========
                                        //        else
                                        //        {

                                        //        }
                                        //        #endregion ============= Code end for vacid not present in trigger schedule ============

                                        //        #endregion ============ Code end for trigger schedule =======================

                                        //        #region ======Code start for set as previous given ==============
                                        //         if (doseno == 1)
                                        //         {
                                        //                Listpreviousgiven.Clear();
                                        //                Listpreviousvalues.Clear();
                                        //                Listpreviousschid.Clear();
                                        //                Listprevioustbltype.Clear();
                                        //                Listpreviousvacid.Clear();
                                        //                Listpreviousduedate.Clear();
                                        //                Listpreviousenddate.Clear();
                                        //                Listpreviousgivendate.Clear();

                                        //                VR_GivenList vrpregiven = new VR_GivenList();
                                        //                vrpregiven = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                        //                DataTable dtpregiven = ConvertToDataTable(vrpregiven);

                                        //                #region =======Code start dose no one present in vr_given =================
                                        //                if (dtpregiven.Rows.Count != 0)//vaccine id dose is already given 
                                        //                {
                                        //                    bday = dob.ToString("yyyy-MM-dd");

                                        //                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                        //                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                    cmdbday.CommandType = CommandType.Text;
                                        //                    cmdbday.ExecuteNonQuery();
                                        //                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                    DataTable dtfirstdosedate = new DataTable();
                                        //                    adbday.Fill(dtfirstdosedate);

                                        //                    if (dtfirstdosedate.Rows.Count != 0)
                                        //                    {
                                        //                         lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                    }
                                        //                    Listpreviousgiven.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                        //                    givenchkflag = true;

                                        //                    lblduedategivenbooster = dtpregiven.Rows[0].ItemArray[5].ToString();
                                        //                    Listpreviousvalues.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                        //                    Listpreviousschid.Add(schid);
                                        //                    Listprevioustbltype.Add("S");
                                        //                    Listpreviousvacid.Add(vaccineid);

                                        //                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                        //                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                        //                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                        //                    SqlCommand cmdend = new SqlCommand(querybday, con);
                                        //                    cmdend.CommandType = CommandType.Text;
                                        //                    cmdend.ExecuteNonQuery();
                                        //                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                        //                    DataTable dtduedosedate = new DataTable();
                                        //                    adenddate.Fill(dtduedosedate);
                                        //                    if (dtduedosedate.Rows.Count != 0)
                                        //                    {
                                        //                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                        //                    }
                                        //                    Listpreviousgivendate.Add(dtpregiven.Rows[0].ItemArray[5].ToString());


                                        //                    //List add for database 
                                        //                    ListVacgivenornot.Add("Yes");
                                        //                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                    Listvacid.Add(vaccineid);
                                        //                    Listschid.Add(schid);
                                        //                    ListVaccinedosenumber.Add(doseno);
                                        //                    Listcatid.Add(categoryid);
                                        //                }
                                        //                #endregion ========== code end for dose no 1 present in vr_given ================

                                        //                #region ====== Code start for dose no 1 not present in vr_given ========
                                        //                else
                                        //                {
                                        //                    bday = dob.ToString("yyyy-MM-dd");

                                        //                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                        //                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                    cmdbday.CommandType = CommandType.Text;
                                        //                    cmdbday.ExecuteNonQuery();
                                        //                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                    DataTable dtfirstdosedate = new DataTable();
                                        //                    adbday.Fill(dtfirstdosedate);

                                        //                    if (dtfirstdosedate.Rows.Count != 0)
                                        //                    {

                                        //                    }
                                        //                    givenchkflag = false;
                                        //                    Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                    Listpreviousschid.Add(schid);
                                        //                    Listprevioustbltype.Add("S");
                                        //                    Listpreviousvacid.Add(vaccineid);
                                        //                    lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                    lblduedategivenbooster = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                        //                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                        //                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                        //                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                        //                    cmdend.CommandType = CommandType.Text;
                                        //                    cmdend.ExecuteNonQuery();
                                        //                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                        //                    DataTable dtduedosedate = new DataTable();
                                        //                    adenddate.Fill(dtduedosedate);
                                        //                    if (dtduedosedate.Rows.Count != 0)
                                        //                    {
                                        //                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                        //                    }
                                        //                    Listpreviousgivendate.Add(null);


                                        //                    //List add for database 
                                        //                    ListVacgivenornot.Add("No");
                                        //                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                    Listvacid.Add(vaccineid);
                                        //                    Listschid.Add(schid);
                                        //                    ListVaccinedosenumber.Add(doseno);
                                        //                    Listcatid.Add(categoryid);
                                        //                }
                                        //                #endregion ========= Code end dose1 not present in vr_given==============
                                        //         }
                                        //         else if(doseno>1)
                                        //         {
                                        //             for (int s = 0; s < Listpreviousduedate.Count; s++)
                                        //             {
                                        //                 int j = Listpreviousduedate.Count;
                                        //                 if (Listpreviousduedate[j - 1].ToString() != "")
                                        //                 {
                                        //                     lblprevduedatedisplay = Listpreviousduedate[j - 1].ToString();
                                        //                     break;
                                        //                 }

                                        //             }

                                        //             for (int s = 0; s < Listpreviousenddate.Count; s++)
                                        //             {
                                        //                 int j = Listpreviousenddate.Count;
                                        //                 if (Listpreviousenddate[j - 1].ToString() != "")
                                        //                 {
                                        //                     lblprevenddatedisplay = Listpreviousenddate[j - 1].ToString();
                                        //                     break;
                                        //                 }

                                        //             }
                                        //             for (int s = 0; s < Listpreviousgivendate.Count; s++)
                                        //             {
                                        //                 int j = Listpreviousgivendate.Count;
                                        //                 if (Listpreviousgivendate[j - 1] != null)
                                        //                 {
                                        //                     lblprevgivendatedisplay = Listpreviousgivendate[j - 1].ToString();
                                        //                     break;
                                        //                 }

                                        //             }
                                        //             for (int s = 0; s < Listpreviousschid.Count; s++)
                                        //             {
                                        //                 //Response.Write(Listpreviousschid[i].ToString());
                                        //                 int j = Listpreviousschid.Count;
                                        //                 // Response.Write(Listpreviousschid.Count + "j= "+ j );
                                        //                 if (Listpreviousschid[j - 1].ToString() != "")
                                        //                 {
                                        //                     lblprevschids = Listpreviousschid[j - j].ToString();
                                        //                     break;
                                        //                 }

                                        //             }
                                        //             for (int s = 0; s < Listprevioustbltype.Count; s++)
                                        //             {
                                        //                 int j = Listprevioustbltype.Count;
                                        //                 if (Listprevioustbltype[j - 1].ToString() != "")
                                        //                 {
                                        //                     lblprevtbltypes = Listprevioustbltype[j - 1].ToString();
                                        //                     break;
                                        //                 }

                                        //             }

                                        //             #region ==================================================  set as previous given is true and false code starts =============================================

                                        //             string currentdoseduedate = string.Empty;

                                        //             if (set_as_previous==true)//set as previous given true
                                        //             {
                                        //                 VR_GivenList vrpregiven1 = new VR_GivenList();
                                        //                 vrpregiven1 = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(lblprevschids), "S");
                                        //                 DataTable dtsgivendt1 = ConvertToDataTable(vrpregiven1);
                                        //                 // Response.Write("previous schid = " + lblprevschids.Text);
                                        //                 if (dtsgivendt1.Rows.Count != 0)
                                        //                 {

                                        //                     if (nodue == true)
                                        //                     {
                                        //                         nodue = false;
                                        //                     }
                                        //                 }

                                        //                 if (lblprevgivendatedisplay != "")
                                        //                 {

                                        //                     if (Convert.ToDateTime(lblprevgivendatedisplay) <= Convert.ToDateTime(lblprevenddatedisplay))
                                        //                     {
                                        //                         currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                        //                         Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                         string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                        //                         SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                         cmdbday.CommandType = CommandType.Text;
                                        //                         cmdbday.ExecuteNonQuery();
                                        //                         SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                         DataTable dtfirstdosedate = new DataTable();
                                        //                         adbday.Fill(dtfirstdosedate);

                                        //                         if (dtfirstdosedate.Rows.Count != 0)
                                        //                         {
                                        //                             currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                         }

                                        //                     }
                                        //                     else
                                        //                     {
                                        //                         //  Response.Write(lblprevgivendatedisplay.Text);
                                        //                         currentdosedate = Convert.ToDateTime(lblprevgivendatedisplay);
                                        //                         Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                         string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                        //                         SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                         cmdbday.CommandType = CommandType.Text;
                                        //                         cmdbday.ExecuteNonQuery();
                                        //                         SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                         DataTable dtfirstdosedate = new DataTable();
                                        //                         adbday.Fill(dtfirstdosedate);

                                        //                         if (dtfirstdosedate.Rows.Count != 0)
                                        //                         {
                                        //                             currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                             // Response.Write(currentdoseduedate);
                                        //                         }
                                        //                     }
                                        //                 }
                                        //                 else
                                        //                 {
                                        //                     currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                        //                     Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                     string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                        //                     SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                     cmdbday.CommandType = CommandType.Text;
                                        //                     cmdbday.ExecuteNonQuery();
                                        //                     SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                     DataTable dtfirstdosedate = new DataTable();
                                        //                     adbday.Fill(dtfirstdosedate);

                                        //                     if (dtfirstdosedate.Rows.Count != 0)
                                        //                     {
                                        //                         currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                     }
                                        //                 }
                                        //             }
                                        //             else
                                        //             {
                                        //                 currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                        //                 Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                 string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                        //                 SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                 cmdbday.CommandType = CommandType.Text;
                                        //                 cmdbday.ExecuteNonQuery();
                                        //                 SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                 DataTable dtfirstdosedate = new DataTable();
                                        //                 adbday.Fill(dtfirstdosedate);

                                        //                 if (dtfirstdosedate.Rows.Count != 0)
                                        //                 {
                                        //                     currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                        //                 }
                                        //             }

                                        //             VR_GivenList vrpregiven = new VR_GivenList();
                                        //             vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid,schid, "S");
                                        //             DataTable dtsgivendt = ConvertToDataTable(vrpregiven);
                                        //             if (dtsgivendt.Rows.Count != 0)
                                        //             {
                                        //                 dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                        //                 bday = dob1.ToString("yyyy-MM-dd");
                                        //                 string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                        //                 SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                 cmdbday.CommandType = CommandType.Text;
                                        //                 cmdbday.ExecuteNonQuery();
                                        //                 SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                 DataTable dtfirstdosedate = new DataTable();
                                        //                 adbday.Fill(dtfirstdosedate);

                                        //                 if (dtfirstdosedate.Rows.Count != 0)
                                        //                 {
                                        //                     lblDueDateGivenimgchk = currentdoseduedate;
                                        //                 }
                                        //                 Listpreviousgiven.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                        //                  givenchkflag = true;

                                        //                 Listpreviousvalues.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                        //                 Listpreviousschid.Add(schid);
                                        //                 Listprevioustbltype.Add("S");
                                        //                 Listpreviousvacid.Add(vaccineid);

                                        //                 lblduedategivenbooster = dtsgivendt.Rows[0].ItemArray[5].ToString();
                                        //                 Listpreviousduedate.Add(currentdoseduedate);
                                        //                 currentdosedate = Convert.ToDateTime(currentdoseduedate);

                                        //                 Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                 string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                        //                 SqlCommand cmdend = new SqlCommand(querybday, con);
                                        //                 cmdend.CommandType = CommandType.Text;
                                        //                 cmdend.ExecuteNonQuery();
                                        //                 SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                        //                 DataTable dtduedosedate = new DataTable();
                                        //                 adenddate.Fill(dtduedosedate);
                                        //                 if (dtduedosedate.Rows.Count != 0)
                                        //                 {
                                        //                     // lblduedateaftergiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                        //                     Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                     lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                        //                 }

                                        //                 Listpreviousgivendate.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());

                                        //                 //List add for database 
                                        //                 ListVacgivenornot.Add("Yes");
                                        //                 ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                 ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                 Listvacid.Add(vaccineid);
                                        //                 Listschid.Add(schid);
                                        //                 ListVaccinedosenumber.Add(doseno);
                                        //                 Listcatid.Add(categoryid);

                                        //             }
                                        //             else
                                        //             {
                                        //                 dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                        //                 bday = dob1.ToString("yyyy-MM-dd");

                                        //                 string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                        //                 SqlCommand cmdbday = new SqlCommand(querybday, con);
                                        //                 cmdbday.CommandType = CommandType.Text;
                                        //                 cmdbday.ExecuteNonQuery();
                                        //                 SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                        //                 DataTable dtfirstdosedate = new DataTable();
                                        //                 adbday.Fill(dtfirstdosedate);

                                        //                 if (dtfirstdosedate.Rows.Count != 0)
                                        //                 {

                                        //                     Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                     Listpreviousschid.Add(schid);
                                        //                     Listprevioustbltype.Add("S");
                                        //                     Listpreviousvacid.Add(vaccineid);

                                        //                     Listpreviousgiven.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                     lblduedategivenbooster = currentdoseduedate;
                                        //                     lblDueDateGivenimgchk = currentdoseduedate;
                                        //                     Listpreviousduedate.Add(currentdoseduedate);
                                        //                     currentdosedate = Convert.ToDateTime(currentdoseduedate);
                                        //                     Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                        //                     string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + Curdosedate + "')))";
                                        //                     SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                        //                     cmdend.CommandType = CommandType.Text;
                                        //                     cmdend.ExecuteNonQuery();
                                        //                     SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                        //                     DataTable dtduedosedate = new DataTable();
                                        //                     adenddate.Fill(dtduedosedate);
                                        //                     if (dtduedosedate.Rows.Count != 0)
                                        //                     {
                                        //                         // lblDueDateGiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                        //                         Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                     }
                                        //                     Listpreviousgivendate.Add(null);

                                        //                     //List add for database 
                                        //                     ListVacgivenornot.Add("No");
                                        //                     ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                        //                     ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                        //                     Listvacid.Add(vaccineid);
                                        //                     Listschid.Add(schid);
                                        //                     ListVaccinedosenumber.Add(doseno);
                                        //                     Listcatid.Add(categoryid);
                                        //                 }
                                        //             }
                                        //             #endregion =============================================== set as previous given code ends ===============================================

                                        //         }

                                        //        #endregion ======== Code end for set as previous given ============

                                        //        #region =================Code start for Vacid present in vr_given ====================================
                                        //        VR_GivenList givenlist = new VR_GivenList();
                                        //        givenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid,schid, doseno, "S");
                                        //        DataTable dtgiven1 = ConvertToDataTable(givenlist);
                                        //        if (dtgiven1.Rows.Count != 0)
                                        //        {
                                        //            givenid =Convert.ToInt32(dtgiven1.Rows[0].ItemArray[0].ToString());
                                        //            givendate = Convert.ToDateTime(dtgiven1.Rows[0].ItemArray[5].ToString());
                                        //           // dosegivenornot ="Yes";
                                        //            givenchkflag = true;
                                        //        }
                                        //        else
                                        //        {
                                        //            givenchkflag=false;
                                        //           // dosegivenornot="No";
                                        //        }
                                        //        #endregion ================ Code end for Vacid present in vr_given  ==================================

                                        //        #region ======================================= booster dose code starts ========================================================================================

                                        //         bool showdotsbooster = true;
                                        //         if (booster == "True")
                                        //         {
                                        //             bool chkgiven = false;
                                        //             if (Convert.ToDateTime(lblduedategivenbooster) > DateTime.Now)
                                        //             {
                                        //                 showdotsbooster = false;
                                        //             }
                                        //             else
                                        //             {
                                        //                 string strdate = null;
                                        //                 string[] strArrdate = null;

                                        //                 //int count = 0;
                                        //                 strdate = lblboosterdoses;
                                        //                 char[] splitchar11 = { ',' };


                                        //                 strArrdate = strdate.Split(splitchar11);

                                        //                 for (int b = 0; b < strArrdate.Length; b++)
                                        //                 {
                                        //                     for (int s = 0; s <= b - 1; s++)
                                        //                     {
                                        //                         VR_GivenList vrpregiven = new VR_GivenList();
                                        //                         vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(Listpreviousschid[s]), "S");
                                        //                         DataTable dtboster = ConvertToDataTable(vrpregiven);

                                        //                         if (dtboster.Rows.Count != 0)
                                        //                         {
                                        //                             chkgiven = true;
                                        //                             break;
                                        //                         }
                                        //                         else
                                        //                         {
                                        //                             chkgiven = false;
                                        //                         }
                                        //                         if (chkgiven == true)
                                        //                             break;
                                        //                     }
                                        //                 }

                                        //                 if (chkgiven == true)
                                        //                 {

                                        //                     showdotsbooster = false;
                                        //                 }
                                        //                 else
                                        //                 {

                                        //                     showdotsbooster = true;
                                        //                 }

                                        //             }
                                        //         }

                                        //         #endregion ==================================== booster dose code ends ==========================================================================================

                                        //        #region =================Code start for Not Compulsory ==========================================
                                        //         VRSchedulelist vrsch = new VRSchedulelist();
                                        //         vrsch = VRScheduleDA.gridviewvacschdoseno(schid, vaccineid,doseno, countryid, stateid, Vacyear);
                                        //         DataTable dtnotcosly = ConvertToDataTable(vrsch);

                                        //         if (dtnotcosly.Rows.Count != 0)
                                        //         {
                                        //             if (dtnotcosly.Rows[0].ItemArray[16].ToString() == "True")
                                        //             {
                                        //                 compulsoryflag = true;
                                        //             }
                                        //             else
                                        //             {
                                        //                 compulsoryflag = false;
                                        //             }
                                        //         }
                                        //         #endregion ============================ code end for Not compulsory =============================

                                        //        #region ================== code start for reminder on ================================

                                        //         //Code start for reminder set on 

                                        //         bool reminder1set = false;
                                        //         bool reminder2set = false;
                                        //         string givenreminderdate = lblDueDateGivenimgchk;

                                        //         VR_ReminderChangeslist vrrchnglist = new VR_ReminderChangeslist();
                                        //         vrrchnglist = VR_ReminderChngDA.gridviewscheduleid(userid, childid, vaccineid, schid, doseno, "S");
                                        //         DataTable dtchngreminder = ConvertToDataTable(vrrchnglist);

                                        //         if (dtchngreminder.Rows.Count != 0)
                                        //         {
                                        //             if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "1")
                                        //             {
                                        //                 //string date1 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                        //                 ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                        //             }
                                        //             else if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "2")
                                        //             {
                                        //                // string date2 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                        //                 ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                        //             }
                                        //         }
                                        //         else
                                        //         {
                                        //             VR_ReminderSettingList vrremindsetting = new VR_ReminderSettingList();
                                        //             vrremindsetting = VR_ReminderSettingDA.gridviewuid(userid, "True");
                                        //             DataTable dtRemindsetng = ConvertToDataTable(vrremindsetting);
                                        //             if (dtRemindsetng.Rows.Count != 0)
                                        //             {
                                        //                 if (reminder1set == false)
                                        //                 {
                                        //                     string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                        //                     ListReminderondate.Add(date1);
                                        //                 }
                                        //                 else
                                        //                 {
                                        //                     if (dtRemindsetng.Rows[0].ItemArray[6].ToString() == "True")
                                        //                     {
                                        //                         reminder2set = true;
                                        //                         if (reminder2set == true)
                                        //                         {
                                        //                             string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[3].ToString())).ToShortDateString();
                                        //                             ListReminderondate.Add(date2);
                                        //                         }
                                        //                     }
                                        //                     else
                                        //                     {
                                        //                         reminder2set = false;
                                        //                     } 
                                        //                 }
                                        //             }
                                        //             else
                                        //             {
                                        //                 VR_DefaultSettingLsit vrdefalutlist = new VR_DefaultSettingLsit();
                                        //                 vrdefalutlist = VR_DefaultSettingDA.gridview();
                                        //                 DataTable dtdefaultsetng = ConvertToDataTable(vrdefalutlist);
                                        //                 if (dtdefaultsetng.Rows.Count != 0)
                                        //                 {
                                        //                     if (reminder1set == false)
                                        //                     {
                                        //                         string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[1].ToString())).ToShortDateString();
                                        //                         ListReminderondate.Add(date1);
                                        //                     }
                                        //                     else
                                        //                     {
                                        //                         if (dtdefaultsetng.Rows[0].ItemArray[5].ToString() == "True")
                                        //                         {
                                        //                             reminder2set = true;
                                        //                             if (reminder2set == true)
                                        //                             {
                                        //                                 string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                        //                                 ListReminderondate.Add(date2);
                                        //                             }
                                        //                         }
                                        //                         else
                                        //                         {
                                        //                             reminder2set = false;

                                        //                         }

                                        //                     }

                                        //                 }

                                        //             }

                                        //         }


                                        //         #endregion ================= end code for reminder on ================================

                                        //        #region ===================================== code for give vaccine(Submit and remove) start ========================================================

                                        //        VR_GivenList vrvacsubmit = new VR_GivenList();
                                        //        vrvacsubmit = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                        //        DataTable dtschgivenchk = ConvertToDataTable(vrvacsubmit);
                                        //        if (dtschgivenchk.Rows.Count != 0)
                                        //        {
                                        //        }
                                        //        else
                                        //        {
                                        //            #region==================================================================skip code starts ==============================================================
                                        //             VR_SkipSchlist vrskiplist = new VR_SkipSchlist();
                                        //            vrskiplist = VR_SkipschDA.gridviewchidschiddsnotbltypelist(childid, schid,doseno, vaccineid, "S");
                                        //            DataTable dtskip = ConvertToDataTable(vrskiplist);
                                        //            if (dtskip.Rows.Count != 0)
                                        //            {
                                        //                skipid =Convert.ToInt32(dtskip.Rows[0].ItemArray[0].ToString());
                                        //                skipdate = Convert.ToDateTime(dtskip.Rows[0].ItemArray[6].ToString());
                                        //                skipflag = true;
                                        //            }
                                        //            else
                                        //            {
                                        //                 skipflag=false;
                                        //            }

                                        //            #endregion==================================================================skip code ends ==============================================================
                                        //        }
                                        //        #endregion =================================== code for give vaccine(Submit and remove) end =========================================================

                                        //        #region ================ Improved code start for count the doses ===================================
                                        //        if (givenchkflag == true)
                                        //        {

                                        //             if (showdotsbooster == false)
                                        //             {
                                        //             }
                                        //             else
                                        //             {
                                        //                 if (triggershow == true)
                                        //                 {
                                        //                      noofgreenvaccines++;
                                        //                     dosecolor ="Green";
                                        //                 }
                                        //             }
                                        //        }
                                        //        else if (skipflag == true || compulsoryflag == true || nodue == true)
                                        //        {
                                        //            if (showdotsbooster == false)
                                        //             {
                                        //             }
                                        //             else
                                        //             {
                                        //                 if (triggershow == true)
                                        //                 {
                                        //                      noofgrayvaccines++;
                                        //                     dosecolor="Gray";
                                        //                 }
                                        //            }
                                        //        }
                                        //       else
                                        //       {
                                        //            if (Convert.ToDateTime(lblDueDateGivenimgchk) < DateTime.Now)
                                        //            {
                                        //                if (showdotsbooster == false)
                                        //                    {
                                        //                    }
                                        //                else
                                        //                {
                                        //                 if (triggershow == true)
                                        //                 {
                                        //                      noofredvaccines++;
                                        //                     dosecolor="Red";
                                        //                 }
                                        //                }
                                        //            }
                                        //            else if (Convert.ToDateTime(lblDueDateGivenimgchk) > DateTime.Now)
                                        //            {
                                        //                int days = ((Convert.ToDateTime(lblDueDateGivenimgchk) - DateTime.Now).Days);
                                        //                if (days < orangedays)
                                        //                {
                                        //                    if (showdotsbooster == false)
                                        //                    {
                                        //                    }
                                        //                    else
                                        //                    {
                                        //                        if (triggershow == true)
                                        //                        {
                                        //                            nooforangevaccines++;
                                        //                            dosecolor="Orange";
                                        //                        }
                                        //                    }
                                        //                }
                                        //                else
                                        //                {
                                        //                    if (showdotsbooster == false)
                                        //                    {
                                        //                    }
                                        //                    else
                                        //                    {
                                        //                        if (triggershow == true)
                                        //                        {
                                        //                            noofgrayvaccines++;
                                        //                            dosecolor="Gray";
                                        //                        }
                                        //                    }
                                        //                }
                                        //            }
                                        //        }



                                        //        #endregion ================== improved code end for count the doses =============================


                                        //    }
                                        //}
                                        #endregion ============== comment end ===================

                                        finalobject.schid = 0;
                                        finalobject.catid = categoryid;
                                        finalobject.vacid = vaccineid;
                                        finalobject.childid = childid;
                                        finalobject.doseno = 0;
                                        finalobject.gender = sex;
                                        finalobject.countryid = countryid;
                                        finalobject.stateid = stateid;
                                        finalobject.vaccineyear = Vacyear;
                                        finalobject.DueDays = 0;
                                        finalobject.DueMonths = 0;
                                        finalobject.DueYears = 0;
                                        finalobject.EndDays = 0;
                                        finalobject.EndMonth = 0;
                                        finalobject.EndYear = 0;
                                        finalobject.DueOnDate = Convert.ToDateTime("1/1/1001");
                                        finalobject.EndDueDate = Convert.ToDateTime("1/1/1001");
                                        finalobject.ReminderOnDate = Convert.ToDateTime("1/1/1001");
                                        finalobject.givenid = 0;
                                        finalobject.DateGiven = Convert.ToDateTime("1/1/1001");
                                        finalobject.skipid = 0;
                                        finalobject.skipdate = Convert.ToDateTime("1/1/1001");
                                        finalobject.set_as_previous_given = false;
                                        finalobject.dosegivenornot = false;
                                        finalobject.skipornot = false;
                                        finalobject.notcompulsory = false;
                                        finalobject.triggershowflag = false;
                                        finalobject.nodueflag = false;
                                        finalobject.Booster = false;
                                        finalobject.BoosterDoses = "";
                                        finalobject.dosecolour = "";
                                        // childsch.Add(finalobject);

                                    }
                                    #endregion ===== Code end for the vacid is not present in VROpted table =================
                                }

                            }

                            #endregion ================== end code for  Contains greater than one vaccine id =====================

                            #region ============= Contains only one vaccine id =====================
                            else
                            {
                                if (custsch != true)
                                {
                                    #region =================== Code start for normal schedule ===================
                                    int vaccineid = Convert.ToInt32(dtvacids.Rows[0].ItemArray[0].ToString());

                                    VRSchedulelist vrschlist = new VRSchedulelist();
                                    vrschlist = schedulelstController.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                    DataTable dtschedule = ConvertToDataTable(vrschlist);


                                    if (dtschedule.Rows.Count != 0)
                                    {
                                        for (int p = 0; p < dtschedule.Rows.Count; p++)
                                        {
                                            childvacsch finalobject = new childvacsch();
                                            set_as_previous = Convert.ToBoolean(dtschedule.Rows[p].ItemArray[12].ToString());
                                            schid = Convert.ToInt32(dtschedule.Rows[p].ItemArray[0].ToString());
                                            doseno = Convert.ToInt32(dtschedule.Rows[p].ItemArray[7].ToString());
                                            d_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[4].ToString());
                                            d_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[5].ToString());
                                            d_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[6].ToString());
                                            e_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[8].ToString());
                                            e_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[9].ToString());
                                            e_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[10].ToString());
                                            booster = dtschedule.Rows[p].ItemArray[13].ToString();
                                            lblboosterdoses = dtschedule.Rows[p].ItemArray[14].ToString();

                                            #region ===================== Code start to check schedule is archieved ===================
                                            bool archie = false;
                                            string strarch = "select archived from VR_Schedule where  schedule_id='" + schid + "'";
                                            SqlCommand cmdarch = new SqlCommand(strarch, con);
                                            cmdarch.CommandType = CommandType.Text;
                                            cmdarch.ExecuteNonQuery();
                                            SqlDataAdapter sqladarch = new SqlDataAdapter(cmdarch);
                                            DataTable dtarch = new DataTable();
                                            sqladarch.Fill(dtarch);
                                            if (dtarch.Rows.Count != 0)
                                            {
                                                archie = Convert.ToBoolean(dtarch.Rows[0].ItemArray[0].ToString());
                                            }

                                            #endregion ====================== Code end for archieved ======================

                                            #region ========================= code start for dose details ================================
                                            string queryorangedays = "select vacreminderid,userid,subdate,status,orangedaysetting,date_format from VaccineReminder_Settings where userid='" + userid + "' and status='True'";
                                            SqlCommand cmdOD = new SqlCommand(queryorangedays, con);
                                            cmdOD.CommandType = CommandType.Text;
                                            SqlDataAdapter adOD = new SqlDataAdapter(cmdOD);
                                            DataTable dtorangedays = new DataTable();
                                            adOD.Fill(dtorangedays);
                                            if (dtorangedays.Rows.Count != 0)
                                            {
                                                dateformat = dtorangedays.Rows[0].ItemArray[5].ToString();
                                                orangedays = Convert.ToInt32(dtorangedays.Rows[0].ItemArray[4].ToString());
                                            }
                                            else
                                            {
                                                //take from default settings
                                                string queryoddefault = "select defaultvacremid,subdate,status,orangedaysetting,date_format from Default_VacReminder_Setting where status='True'";
                                                SqlCommand cmdODdefault = new SqlCommand(queryoddefault, con);
                                                cmdODdefault.CommandType = CommandType.Text;
                                                SqlDataAdapter adODdefault = new SqlDataAdapter(cmdODdefault);
                                                DataTable dtdefaultorngdays = new DataTable();
                                                adODdefault.Fill(dtdefaultorngdays);
                                                if (dtdefaultorngdays.Rows.Count != 0)
                                                {
                                                    dateformat = dtdefaultorngdays.Rows[0].ItemArray[4].ToString();
                                                    orangedays = Convert.ToInt32(dtdefaultorngdays.Rows[0].ItemArray[3].ToString());
                                                }
                                                else
                                                {
                                                    dateformat = "dd MMM, yyyy";
                                                    orangedays = 15;
                                                }
                                            }
                                            #endregion ============================ code end for dose details ===================================

                                            #region =============== Code start for trigger schedule =====================

                                            VR_TriggerList vrtriglist = new VR_TriggerList();
                                            vrtriglist = triggerscheduleController.gridviewTriggerCidVacid(countryid, vaccineid);
                                            DataTable dttriggersch = ConvertToDataTable(vrtriglist);

                                            #region ============ Code start for vacid present in trigger schedule =============
                                            if (dttriggersch.Rows.Count != 0)  //vaccine id present in trigger schedule
                                            {
                                                TriggerShowVac = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[4].ToString());
                                                TriggerNoDue = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[7].ToString());

                                                string triggerquery = "select dateadd(YY,0,dateadd(M,Convert(int,'" + dttriggersch.Rows[0].ItemArray[6].ToString() + "'),dateadd(dd,0,'" + dob.ToString("yyyy-MM-dd") + "')))";
                                                SqlCommand cmdtrig = new SqlCommand(triggerquery, con);
                                                cmdtrig.CommandType = CommandType.Text;
                                                SqlDataAdapter adtrig = new SqlDataAdapter(cmdtrig);
                                                DataTable dttrigerprevgivendt = new DataTable();
                                                adtrig.Fill(dttrigerprevgivendt);
                                                if (TriggerShowVac == true)
                                                {
                                                    if (TriggerNoDue == true)
                                                    {
                                                        if (doseno == 1)
                                                        {
                                                            VR_GivenList vrgivenlist = new VR_GivenList();
                                                            vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }

                                                        }
                                                        else if (doseno > 1)
                                                        {
                                                            VR_GivenList vrgivenlist = new VR_GivenList();
                                                            vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }
                                                        }
                                                    }
                                                    else if (TriggerNoDue == false)
                                                    {
                                                        if (doseno == 1)
                                                        {

                                                        }
                                                        else if (doseno > 1)
                                                        {
                                                            VR_GivenList vrgivenlist = new VR_GivenList();
                                                            vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (TriggerShowVac == false)
                                                {
                                                    if (doseno == 1)
                                                    {
                                                        trigerschid.Add(schid);
                                                        trigerdoseno.Add(doseno);
                                                        triggershow = true;
                                                    }
                                                    else if (doseno > 1)
                                                    {
                                                        VR_GivenList vrgivenlist = new VR_GivenList();
                                                        vrgivenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                        DataTable dttrigergiven = ConvertToDataTable(vrgivenlist);


                                                        if (dttrigergiven.Rows.Count != 0)
                                                        {
                                                            if (Convert.ToDateTime(dttrigergiven.Rows[0].ItemArray[5].ToString()) > Convert.ToDateTime(dttrigerprevgivendt.Rows[0].ItemArray[0].ToString()))
                                                            {
                                                                triggershow = false;

                                                            }
                                                            else
                                                            {
                                                                triggershow = true;

                                                            }
                                                        }
                                                        else
                                                        {
                                                            int schidindex = trigerschid.Count - 1;
                                                            VR_GivenList vrgivenlist1 = new VR_GivenList();
                                                            vrgivenlist1 = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, Convert.ToInt32(trigerschid[schidindex].ToString()), Convert.ToInt32(trigerdoseno[schidindex].ToString()), "S");
                                                            DataTable dttrigergiven1 = ConvertToDataTable(vrgivenlist1);

                                                            if (dttrigergiven1.Rows.Count != 0)
                                                            {
                                                                triggershow = true;
                                                            }
                                                            else
                                                            {
                                                                triggershow = false;
                                                            }
                                                        }
                                                        trigerschid.Add(schid);
                                                        trigerdoseno.Add(doseno);

                                                        // triggershow = true;
                                                    }
                                                }
                                            }

                                            #endregion =========== code end for vacid present in trigger schedule ============

                                            #region =========== Code start for vacid  not present in trigger schdule ===========
                                            else
                                            {

                                            }
                                            #endregion ============= Code end for vacid not present in trigger schedule ============

                                            #endregion ============ Code end for trigger schedule =======================

                                            #region ======Code start for set as previous given ==============
                                            if (doseno == 1 && archie == false)
                                            {
                                                Listpreviousgiven.Clear();
                                                Listpreviousvalues.Clear();
                                                Listpreviousschid.Clear();
                                                Listprevioustbltype.Clear();
                                                Listpreviousvacid.Clear();
                                                Listpreviousduedate.Clear();
                                                Listpreviousenddate.Clear();
                                                Listpreviousgivendate.Clear();

                                                VR_GivenList vrpregiven = new VR_GivenList();
                                                vrpregiven = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                DataTable dtpregiven = ConvertToDataTable(vrpregiven);

                                                #region =======Code start dose no one present in vr_given =================
                                                if (dtpregiven.Rows.Count != 0)//vaccine id dose is already given 
                                                {
                                                    bday = dob.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgiven.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                                    givenchkflag = true;

                                                    lblduedategivenbooster = dtpregiven.Rows[0].ItemArray[5].ToString();
                                                    Listpreviousvalues.Add(dtpregiven.Rows[0].ItemArray[5].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);

                                                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgivendate.Add(dtpregiven.Rows[0].ItemArray[5].ToString());


                                                    //List add for database 
                                                    ListVacgivenornot.Add("Yes");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);
                                                }
                                                #endregion ========== code end for dose no 1 present in vr_given ================

                                                #region ====== Code start for dose no 1 not present in vr_given ========
                                                else
                                                {
                                                    bday = dob.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {

                                                    }
                                                    givenchkflag = false;
                                                    Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);
                                                    lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    lblduedategivenbooster = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgivendate.Add(null);


                                                    //List add for database 
                                                    ListVacgivenornot.Add("No");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);
                                                }
                                                #endregion ========= Code end dose1 not present in vr_given==============
                                            }
                                            else if (doseno > 1 && archie == false)
                                            {
                                                for (int s = 0; s < Listpreviousduedate.Count; s++)
                                                {
                                                    lblprevduedatedisplay = "";
                                                    int j = Listpreviousduedate.Count;
                                                    if (Listpreviousduedate[j - 1].ToString() != "")
                                                    {
                                                        lblprevduedatedisplay = Listpreviousduedate[j - 1].ToString();
                                                        break;
                                                    }

                                                }

                                                for (int s = 0; s < Listpreviousenddate.Count; s++)
                                                {
                                                    lblprevenddatedisplay = "";
                                                    int j = Listpreviousenddate.Count;
                                                    if (Listpreviousenddate[j - 1].ToString() != "")
                                                    {
                                                        lblprevenddatedisplay = Listpreviousenddate[j - 1].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listpreviousgivendate.Count; s++)
                                                {
                                                    lblprevgivendatedisplay = "";
                                                    int j = Listpreviousgivendate.Count;
                                                    if (Listpreviousgivendate[j - 1] != null)
                                                    {
                                                        lblprevgivendatedisplay = Listpreviousgivendate[j - 1].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listpreviousschid.Count; s++)
                                                {
                                                    lblprevschids = "";
                                                    //Response.Write(Listpreviousschid[i].ToString());
                                                    int j = Listpreviousschid.Count;
                                                    // Response.Write(Listpreviousschid.Count + "j= "+ j );
                                                    if (Listpreviousschid[j - 1].ToString() != "")
                                                    {
                                                        lblprevschids = Listpreviousschid[j - j].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listprevioustbltype.Count; s++)
                                                {
                                                    lblprevtbltypes = "";
                                                    int j = Listprevioustbltype.Count;
                                                    if (Listprevioustbltype[j - 1].ToString() != "")
                                                    {
                                                        lblprevtbltypes = Listprevioustbltype[j - 1].ToString();
                                                        break;
                                                    }

                                                }

                                                #region ==================================================  set as previous given is true and false code starts =============================================

                                                string currentdoseduedate = string.Empty;

                                                if (set_as_previous == true)//set as previous given true
                                                {
                                                    VR_GivenList vrpregiven1 = new VR_GivenList();
                                                    vrpregiven1 = VR_GivenDAController.gridviewchidschidtbltypelist(childid, Convert.ToInt32(lblprevschids), "S");
                                                    DataTable dtsgivendt1 = ConvertToDataTable(vrpregiven1);
                                                    // Response.Write("previous schid = " + lblprevschids.Text);

                                                    if (dtsgivendt1.Rows.Count != 0)
                                                    {

                                                        if (nodue == true)
                                                        {
                                                            nodue = false;
                                                        }
                                                    }

                                                    if (lblprevgivendatedisplay != "")
                                                    {

                                                        if (Convert.ToDateTime(lblprevgivendatedisplay) <= Convert.ToDateTime(lblprevenddatedisplay))
                                                        {
                                                            currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }

                                                        }
                                                        else
                                                        {
                                                            //  Response.Write(lblprevgivendatedisplay.Text);
                                                            currentdosedate = Convert.ToDateTime(lblprevgivendatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                // Response.Write(currentdoseduedate);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                        Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                        string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                        SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                        cmdbday.CommandType = CommandType.Text;
                                                        cmdbday.ExecuteNonQuery();
                                                        SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                        DataTable dtfirstdosedate = new DataTable();
                                                        adbday.Fill(dtfirstdosedate);

                                                        if (dtfirstdosedate.Rows.Count != 0)
                                                        {
                                                            currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                }

                                                VR_GivenList vrpregiven = new VR_GivenList();
                                                vrpregiven = VR_GivenDAController.gridviewchidschidtbltypelist(childid, schid, "S");
                                                DataTable dtsgivendt = ConvertToDataTable(vrpregiven);

                                                if (dtsgivendt.Rows.Count != 0)
                                                {
                                                    dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                    bday = dob1.ToString("yyyy-MM-dd");
                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        lblDueDateGivenimgchk = currentdoseduedate;
                                                    }
                                                    Listpreviousgiven.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                                    givenchkflag = true;

                                                    Listpreviousvalues.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);

                                                    lblduedategivenbooster = dtsgivendt.Rows[0].ItemArray[5].ToString();
                                                    Listpreviousduedate.Add(currentdoseduedate);
                                                    currentdosedate = Convert.ToDateTime(currentdoseduedate);

                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        // lblduedateaftergiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }

                                                    Listpreviousgivendate.Add(dtsgivendt.Rows[0].ItemArray[5].ToString());

                                                    //List add for database 
                                                    ListVacgivenornot.Add("Yes");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);

                                                }
                                                else
                                                {
                                                    dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                    bday = dob1.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {

                                                        Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        Listpreviousschid.Add(schid);
                                                        Listprevioustbltype.Add("S");
                                                        Listpreviousvacid.Add(vaccineid);

                                                        Listpreviousgiven.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        lblduedategivenbooster = currentdoseduedate;
                                                        lblDueDateGivenimgchk = currentdoseduedate;
                                                        Listpreviousduedate.Add(currentdoseduedate);
                                                        currentdosedate = Convert.ToDateTime(currentdoseduedate);
                                                        Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                        string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + Curdosedate + "')))";
                                                        SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                        cmdend.CommandType = CommandType.Text;
                                                        cmdend.ExecuteNonQuery();
                                                        SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                        DataTable dtduedosedate = new DataTable();
                                                        adenddate.Fill(dtduedosedate);
                                                        if (dtduedosedate.Rows.Count != 0)
                                                        {
                                                            // lblDueDateGiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                            Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                        }
                                                        Listpreviousgivendate.Add(null);

                                                        //List add for database 
                                                        ListVacgivenornot.Add("No");
                                                        ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        Listvacid.Add(vaccineid);
                                                        Listschid.Add(schid);
                                                        ListVaccinedosenumber.Add(doseno);
                                                        Listcatid.Add(categoryid);
                                                    }
                                                }
                                                #endregion =============================================== set as previous given code ends ===============================================

                                            }

                                            #endregion ======== Code end for set as previous given ============

                                            #region =================Code start for Vacid present in vr_given ====================================
                                            VR_GivenList givenlist = new VR_GivenList();
                                            givenlist = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                            DataTable dtgiven1 = ConvertToDataTable(givenlist);

                                            if (dtgiven1.Rows.Count != 0)
                                            {
                                                givenid = Convert.ToInt32(dtgiven1.Rows[0].ItemArray[0].ToString());
                                                givendate = Convert.ToDateTime(dtgiven1.Rows[0].ItemArray[5].ToString());
                                                // dosegivenornot ="Yes";
                                                givenchkflag = true;
                                            }
                                            else
                                            {
                                                givenchkflag = false;
                                                givendate = DateTime.Now;
                                                givenid = 0;
                                                // dosegivenornot="No";
                                            }
                                            #endregion ================ Code end for Vacid present in vr_given  ==================================

                                            #region ======================================= booster dose code starts ========================================================================================

                                            bool showdotsbooster = true;
                                            if (booster == "True")
                                            {
                                                bool chkgiven = false;
                                                if (Convert.ToDateTime(lblduedategivenbooster) > DateTime.Now)
                                                {
                                                    showdotsbooster = false;
                                                }
                                                else
                                                {
                                                    string strdate = null;
                                                    string[] strArrdate = null;

                                                    //int count = 0;
                                                    strdate = lblboosterdoses;
                                                    char[] splitchar11 = { ',' };


                                                    strArrdate = strdate.Split(splitchar11);

                                                    for (int b = 0; b < strArrdate.Length; b++)
                                                    {
                                                        for (int s = 0; s <= b - 1; s++)
                                                        {
                                                            VR_GivenList vrpregiven = new VR_GivenList();
                                                            vrpregiven = VR_GivenDAController.gridviewchidschidtbltypelist(childid, Convert.ToInt32(Listpreviousschid[s]), "S");
                                                            DataTable dtboster = ConvertToDataTable(vrpregiven);

                                                            if (dtboster.Rows.Count != 0)
                                                            {
                                                                chkgiven = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                chkgiven = false;
                                                            }
                                                            if (chkgiven == true)
                                                                break;
                                                        }
                                                    }

                                                    if (chkgiven)
                                                    {
                                                        showdotsbooster = false;
                                                    }
                                                    else
                                                    {

                                                        showdotsbooster = true;
                                                    }

                                                }
                                            }

                                            #endregion ==================================== booster dose code ends ==========================================================================================

                                            #region =================Code start for Not Compulsory ==========================================
                                            VRSchedulelist vrsch = new VRSchedulelist();
                                            vrsch = schedulelstController.gridviewvacschdoseno(schid, vaccineid, doseno, countryid, stateid, Vacyear);
                                            DataTable dtnotcosly = ConvertToDataTable(vrsch);

                                            if (dtnotcosly.Rows.Count != 0)
                                            {
                                                if (dtnotcosly.Rows[0].ItemArray[16].ToString() == "True")
                                                {
                                                    compulsoryflag = true;
                                                }
                                                else
                                                {
                                                    compulsoryflag = false;
                                                }
                                            }
                                            #endregion ============================ code end for Not compulsory =============================

                                            #region ================== code start for reminder on ================================

                                            //Code start for reminder set on 

                                            bool reminder1set = false;
                                            bool reminder2set = false;
                                            string givenreminderdate = lblDueDateGivenimgchk;

                                            VR_ReminderChangeslist vrrchnglist = new VR_ReminderChangeslist();
                                            vrrchnglist = VR_ReminderChngDAController.gridviewscheduleid(userid, childid, vaccineid, schid, doseno, "S");
                                            DataTable dtchngreminder = ConvertToDataTable(vrrchnglist);

                                            if (dtchngreminder.Rows.Count != 0)
                                            {
                                                if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "1")
                                                {
                                                    reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                    //string date1 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                    ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                }
                                                else if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "2")
                                                {
                                                    reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                    // string date2 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                    ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                }
                                            }
                                            else
                                            {
                                                VR_ReminderSettingList vrremindsetting = new VR_ReminderSettingList();
                                                vrremindsetting = VR_ReminderSettingDAController.gridviewuid(userid, "True");
                                                DataTable dtRemindsetng = ConvertToDataTable(vrremindsetting);
                                                if (dtRemindsetng.Rows.Count != 0)
                                                {
                                                    if (reminder1set == false)
                                                    {
                                                        string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                        ListReminderondate.Add(date1);
                                                        reminderdate = Convert.ToDateTime(date1);
                                                    }
                                                    else
                                                    {
                                                        if (dtRemindsetng.Rows[0].ItemArray[6].ToString() == "True")
                                                        {
                                                            reminder2set = true;
                                                            if (reminder2set == true)
                                                            {
                                                                string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[3].ToString())).ToShortDateString();
                                                                ListReminderondate.Add(date2);
                                                                reminderdate = Convert.ToDateTime(date2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            reminder2set = false;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    VR_DefaultSettingLsit vrdefalutlist = new VR_DefaultSettingLsit();
                                                    vrdefalutlist = VR_DefaultSettingDAController.gridview();
                                                    DataTable dtdefaultsetng = ConvertToDataTable(vrdefalutlist);
                                                    if (dtdefaultsetng.Rows.Count != 0)
                                                    {
                                                        if (reminder1set == false)
                                                        {
                                                            string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[1].ToString())).ToShortDateString();
                                                            ListReminderondate.Add(date1);
                                                            reminderdate = Convert.ToDateTime(date1);
                                                        }
                                                        else
                                                        {
                                                            if (dtdefaultsetng.Rows[0].ItemArray[5].ToString() == "True")
                                                            {
                                                                reminder2set = true;
                                                                if (reminder2set == true)
                                                                {
                                                                    string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                    ListReminderondate.Add(date2);
                                                                    reminderdate = Convert.ToDateTime(date2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                reminder2set = false;

                                                            }

                                                        }

                                                    }

                                                }

                                            }


                                            #endregion ================= end code for reminder on ================================

                                            #region ===================================== code for give vaccine(Submit and remove) start ========================================================
                                            bool archgiven = false;
                                            VR_GivenList vrvacsubmit = new VR_GivenList();
                                            vrvacsubmit = VR_GivenDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                            DataTable dtschgivenchk = ConvertToDataTable(vrvacsubmit);

                                            if (dtschgivenchk.Rows.Count != 0)
                                            {
                                                archgiven = true;
                                            }
                                            else
                                            {
                                                #region==================================================================skip code starts ==============================================================
                                                VR_SkipSchlist vrskiplist = new VR_SkipSchlist();
                                                vrskiplist = VR_SkipschDAController.gridviewchidschiddsnotbltypelist(childid, schid, doseno, vaccineid, "S");
                                                DataTable dtskip = ConvertToDataTable(vrskiplist);

                                                if (dtskip.Rows.Count != 0)
                                                {
                                                    skipid = Convert.ToInt32(dtskip.Rows[0].ItemArray[0].ToString());
                                                    skipdate = Convert.ToDateTime(dtskip.Rows[0].ItemArray[6].ToString());
                                                    skipflag = true;
                                                }
                                                else
                                                {
                                                    skipflag = false;
                                                }

                                                #endregion==================================================================skip code ends ==============================================================
                                            }
                                            #endregion =================================== code for give vaccine(Submit and remove) end =========================================================

                                            #region ================ Improved code start for count the doses ===================================
                                            if (givenchkflag == true)
                                            {
                                                if (showdotsbooster == false)
                                                {
                                                    dosecolor = "";
                                                }
                                                else
                                                {
                                                    if (triggershow == true)
                                                    {
                                                        noofgreenvaccines++;
                                                        dosecolor = "Green";
                                                    }
                                                }
                                            }
                                            else if (skipflag == true || compulsoryflag == true || nodue == true)
                                            {
                                                if (showdotsbooster == false)
                                                {
                                                    dosecolor = "";
                                                }
                                                else
                                                {
                                                    if (triggershow == true)
                                                    {

                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                        noofgrayvaccines++;
                                                        dosecolor = "Gray";
                                                    }
                                                    else
                                                    {
                                                        dosecolor = "";
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (Convert.ToDateTime(lblDueDateGivenimgchk) < DateTime.Now)
                                                {
                                                    if (showdotsbooster == false)
                                                    {
                                                        dosecolor = "";
                                                    }
                                                    else
                                                    {
                                                        if (triggershow == true)
                                                        {

                                                            // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                            noofredvaccines++;
                                                            dosecolor = "Red";
                                                        }
                                                        else
                                                        {
                                                            dosecolor = "";
                                                        }
                                                    }
                                                }
                                                else if (Convert.ToDateTime(lblDueDateGivenimgchk) > DateTime.Now)
                                                {
                                                    int days = ((Convert.ToDateTime(lblDueDateGivenimgchk) - DateTime.Now).Days);
                                                    if (days < orangedays)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);

                                                                nooforangevaccines++;
                                                                dosecolor = "Orange";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                noofgrayvaccines++;
                                                                dosecolor = "Gray";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion ================== improved code end for count the doses =============================

                                            //Console.WriteLine("vacid=" + vaccineid);
                                            if (archie == true && archgiven == true)
                                            {
                                                finalobject.schid = schid;
                                                finalobject.vacid = vaccineid;
                                                finalobject.catid = categoryid;
                                                finalobject.childid = childid;
                                                finalobject.doseno = doseno;
                                                finalobject.gender = sex;
                                                finalobject.countryid = countryid;
                                                finalobject.stateid = stateid;
                                                finalobject.vaccineyear = Vacyear;
                                                finalobject.DueDays = d_days;
                                                finalobject.DueMonths = d_months;
                                                finalobject.DueYears = d_year;
                                                finalobject.EndDays = e_days;
                                                finalobject.EndMonth = e_months;
                                                finalobject.EndYear = e_year;
                                                finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                                finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                                finalobject.ReminderOnDate = reminderdate;
                                                finalobject.givenid = givenid;
                                                finalobject.DateGiven = givendate;
                                                finalobject.skipid = skipid;
                                                finalobject.skipdate = skipdate;
                                                finalobject.set_as_previous_given = set_as_previous;
                                                finalobject.dosegivenornot = givenchkflag;
                                                finalobject.skipornot = skipflag;
                                                finalobject.notcompulsory = compulsoryflag;
                                                finalobject.triggershowflag = TriggerShowVac;
                                                finalobject.nodueflag = TriggerNoDue;
                                                finalobject.Booster = Convert.ToBoolean(booster);
                                                finalobject.BoosterDoses = lblboosterdoses;
                                                finalobject.dosecolour = dosecolor;
                                                childsch.Add(finalobject);
                                            }
                                            else if (archie != true)
                                            {
                                                finalobject.schid = schid;
                                                finalobject.vacid = vaccineid;
                                                finalobject.catid = categoryid;
                                                finalobject.childid = childid;
                                                finalobject.doseno = doseno;
                                                finalobject.gender = sex;
                                                finalobject.countryid = countryid;
                                                finalobject.stateid = stateid;
                                                finalobject.vaccineyear = Vacyear;
                                                finalobject.DueDays = d_days;
                                                finalobject.DueMonths = d_months;
                                                finalobject.DueYears = d_year;
                                                finalobject.EndDays = e_days;
                                                finalobject.EndMonth = e_months;
                                                finalobject.EndYear = e_year;
                                                finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                                finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                                finalobject.ReminderOnDate = reminderdate;
                                                finalobject.givenid = givenid;
                                                finalobject.DateGiven = givendate;
                                                finalobject.skipid = skipid;
                                                finalobject.skipdate = skipdate;
                                                finalobject.set_as_previous_given = set_as_previous;
                                                finalobject.dosegivenornot = givenchkflag;
                                                finalobject.skipornot = skipflag;
                                                finalobject.notcompulsory = compulsoryflag;
                                                finalobject.triggershowflag = TriggerShowVac;
                                                finalobject.nodueflag = TriggerNoDue;
                                                finalobject.Booster = Convert.ToBoolean(booster);
                                                finalobject.BoosterDoses = lblboosterdoses;
                                                finalobject.dosecolour = dosecolor;
                                                childsch.Add(finalobject);
                                            }
                                        }
                                    }
                                    #endregion ========================= Code end for normal schedule =========================
                                }
                                else
                                {
                                    #region ============================= Code start for custom schedule =================================
                                    int vaccineid = Convert.ToInt32(dtvacids.Rows[0].ItemArray[0].ToString());

                                    //VRSchedulelist vrschlist = new VRSchedulelist();
                                    //vrschlist = VRScheduleDA.gridviewvacidcountid(vaccineid, countryid, stateid, Vacyear);
                                    // DataTable dtschedule = ConvertToDataTable(vrschlist);

                                    string quercust = "SELECT schedule_id,ISNULL(vaccine_id,'0') as vaccine_id ,ISNULL(country_id,'0') as country_id,ISNULL(Year,'0') as Year, ISNULL(Due_Days,'0') as Due_Days, ISNULL(Due_Months,'0') as Due_Months, ISNULL(Due_Years,'0') as Due_Years," +
       "ISNULL(End_Days,'0') as End_Days, ISNULL(End_Months,'0') as End_Months, ISNULL(End_Years,'0') as  End_Years, ISNULL(Dose_No,'0') as Dose_No,ISNULL(Dose_Name,'')  as Dose_Name, ISNULL(Set_as_Previous_Given,'false')  as Set_as_Previous_Given," +
    "ISNULL(Booster,'false')  as Booster,ISNULL(Booster_Doses,'')  as Booster_Doses,ISNULL(Dose_Desc,'') as Dose_Desc, ISNULL(NotCompulsary,'false')as NotCompulsary,ISNULL(stateid,'0')  as stateid,ISNULL(No_Due_Date,'false') as No_Due_Date" +
     " from  VRC_Schedule where vaccine_id='" + vaccineid + "' and userid='" + userid + "' order by Dose_No";
                                    SqlCommand cmdcssch = new SqlCommand(quercust, con);
                                    cmdcssch.CommandType = CommandType.Text;
                                    cmdcssch.ExecuteNonQuery();
                                    SqlDataAdapter adcust = new SqlDataAdapter(cmdcssch);
                                    DataTable dtschedule = new DataTable();
                                    adcust.Fill(dtschedule);

                                    if (dtschedule.Rows.Count != 0)
                                    {
                                        for (int p = 0; p < dtschedule.Rows.Count; p++)
                                        {
                                            childvacsch finalobject = new childvacsch();
                                            set_as_previous = Convert.ToBoolean(dtschedule.Rows[p].ItemArray[12].ToString());
                                            schid = Convert.ToInt32(dtschedule.Rows[p].ItemArray[0].ToString());
                                            doseno = Convert.ToInt32(dtschedule.Rows[p].ItemArray[10].ToString());
                                            d_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[4].ToString());
                                            d_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[5].ToString());
                                            d_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[6].ToString());
                                            e_days = Convert.ToInt32(dtschedule.Rows[p].ItemArray[7].ToString());
                                            e_months = Convert.ToInt32(dtschedule.Rows[p].ItemArray[8].ToString());
                                            e_year = Convert.ToInt32(dtschedule.Rows[p].ItemArray[9].ToString());
                                            booster = dtschedule.Rows[p].ItemArray[13].ToString();
                                            lblboosterdoses = dtschedule.Rows[p].ItemArray[14].ToString();


                                            #region ========================= code start for dose details ================================
                                            string queryorangedays = "select vacreminderid,userid,subdate,status,orangedaysetting,date_format from VaccineReminder_Settings where userid='" + userid + "' and status='True'";
                                            SqlCommand cmdOD = new SqlCommand(queryorangedays, con);
                                            cmdOD.CommandType = CommandType.Text;
                                            SqlDataAdapter adOD = new SqlDataAdapter(cmdOD);
                                            DataTable dtorangedays = new DataTable();
                                            adOD.Fill(dtorangedays);
                                            if (dtorangedays.Rows.Count != 0)
                                            {
                                                dateformat = dtorangedays.Rows[0].ItemArray[5].ToString();
                                                orangedays = Convert.ToInt32(dtorangedays.Rows[0].ItemArray[4].ToString());
                                            }
                                            else
                                            {
                                                //take from default settings
                                                string queryoddefault = "select defaultvacremid,subdate,status,orangedaysetting,date_format from Default_VacReminder_Setting where status='True'";
                                                SqlCommand cmdODdefault = new SqlCommand(queryoddefault, con);
                                                cmdODdefault.CommandType = CommandType.Text;
                                                SqlDataAdapter adODdefault = new SqlDataAdapter(cmdODdefault);
                                                DataTable dtdefaultorngdays = new DataTable();
                                                adODdefault.Fill(dtdefaultorngdays);
                                                if (dtdefaultorngdays.Rows.Count != 0)
                                                {
                                                    dateformat = dtdefaultorngdays.Rows[0].ItemArray[4].ToString();
                                                    orangedays = Convert.ToInt32(dtdefaultorngdays.Rows[0].ItemArray[3].ToString());
                                                }
                                                else
                                                {
                                                    dateformat = "dd MMM, yyyy";
                                                    orangedays = 15;
                                                }
                                            }
                                            #endregion ============================ code end for dose details ===================================

                                            #region =============== Code start for trigger schedule =====================

                                            //  VR_TriggerList vrtriglist = new VR_TriggerList();
                                            // vrtriglist = VR_TriggerScheDA.gridviewTriggerCidVacid(countryid, vaccineid);
                                            //  DataTable dttriggersch = ConvertToDataTable(vrtriglist);

                                            string quercusttrig = "SELECT triggerschid, ISNULL(vaccine_id,'0') as vaccine_id,ISNULL(country_id,'0') as country_id, ISNULL(stateid,'0') as stateid," +
          "ISNULL(show_vaccine,'false') as show_vaccine, ISNULL(end_age,'0') as end_age, ISNULL(subdate,'') as subdate, ISNULL(NoDue,'false') as NoDue" +
           " from VRC_Trigger_Schedule where userid='" + userid + "' and vaccine_id='" + vaccineid + "'";
                                            SqlCommand cmdcsschtrig = new SqlCommand(quercusttrig, con);
                                            cmdcsschtrig.CommandType = CommandType.Text;
                                            cmdcsschtrig.ExecuteNonQuery();
                                            SqlDataAdapter adcusttrig = new SqlDataAdapter(cmdcsschtrig);
                                            DataTable dttriggersch = new DataTable();
                                            adcusttrig.Fill(dttriggersch);


                                            #region ============ Code start for vacid present in trigger schedule =============
                                            if (dttriggersch.Rows.Count != 0)  //vaccine id present in trigger schedule
                                            {
                                                TriggerShowVac = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[4].ToString());
                                                TriggerNoDue = Convert.ToBoolean(dttriggersch.Rows[0].ItemArray[7].ToString());

                                                string triggerquery = "select dateadd(YY,0,dateadd(M,Convert(int,'" + dttriggersch.Rows[0].ItemArray[5].ToString() + "'),dateadd(dd,0,'" + dob.ToString("yyyy-MM-dd") + "')))";
                                                SqlCommand cmdtrig = new SqlCommand(triggerquery, con);
                                                cmdtrig.CommandType = CommandType.Text;
                                                SqlDataAdapter adtrig = new SqlDataAdapter(cmdtrig);
                                                DataTable dttrigerprevgivendt = new DataTable();
                                                adtrig.Fill(dttrigerprevgivendt);
                                                if (TriggerShowVac == true)
                                                {
                                                    if (TriggerNoDue == true)
                                                    {
                                                        if (doseno == 1)
                                                        {
                                                            //VR_GivenList vrgivenlist = new VR_GivenList();
                                                            // vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            //  DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                            SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                            cmdcsschgiv.CommandType = CommandType.Text;
                                                            cmdcsschgiv.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                            DataTable dtgiven = new DataTable();
                                                            adcustgiv.Fill(dtgiven);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }

                                                        }
                                                        else if (doseno > 1)
                                                        {
                                                            //VR_GivenList vrgivenlist = new VR_GivenList();
                                                            // vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            //  DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                            SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                            cmdcsschgiv.CommandType = CommandType.Text;
                                                            cmdcsschgiv.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                            DataTable dtgiven = new DataTable();
                                                            adcustgiv.Fill(dtgiven);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }
                                                        }
                                                    }
                                                    else if (TriggerNoDue == false)
                                                    {
                                                        if (doseno == 1)
                                                        {

                                                        }
                                                        else if (doseno > 1)
                                                        {
                                                            //VR_GivenList vrgivenlist = new VR_GivenList();
                                                            //vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                            //DataTable dtgiven = ConvertToDataTable(vrgivenlist);

                                                            string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                            SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                            cmdcsschgiv.CommandType = CommandType.Text;
                                                            cmdcsschgiv.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                            DataTable dtgiven = new DataTable();
                                                            adcustgiv.Fill(dtgiven);

                                                            if (dtgiven.Rows.Count != 0)
                                                            {
                                                            }
                                                            else
                                                            {
                                                                nodue = true;
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (TriggerShowVac == false)
                                                {
                                                    if (doseno == 1)
                                                    {
                                                        trigerschid.Add(schid);
                                                        trigerdoseno.Add(doseno);
                                                        triggershow = true;
                                                    }
                                                    else if (doseno > 1)
                                                    {
                                                        // VR_GivenList vrgivenlist = new VR_GivenList();
                                                        // vrgivenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                        // DataTable dttrigergiven = ConvertToDataTable(vrgivenlist);

                                                        string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                        SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                        cmdcsschgiv.CommandType = CommandType.Text;
                                                        cmdcsschgiv.ExecuteNonQuery();
                                                        SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                        DataTable dttrigergiven = new DataTable();
                                                        adcustgiv.Fill(dttrigergiven);

                                                        if (dttrigergiven.Rows.Count != 0)
                                                        {
                                                            if (Convert.ToDateTime(dttrigergiven.Rows[0].ItemArray[4].ToString()) > Convert.ToDateTime(dttrigerprevgivendt.Rows[0].ItemArray[0].ToString()))
                                                            {
                                                                triggershow = false;

                                                            }
                                                            else
                                                            {
                                                                triggershow = true;

                                                            }
                                                        }
                                                        else
                                                        {
                                                            int schidindex = trigerschid.Count - 1;
                                                            // VR_GivenList vrgivenlist1 = new VR_GivenList();
                                                            //vrgivenlist1 = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, Convert.ToInt32(trigerschid[schidindex].ToString()), Convert.ToInt32(trigerdoseno[schidindex].ToString()), "S");
                                                            //DataTable dttrigergiven1 = ConvertToDataTable(vrgivenlist1);


                                                            string quercustgiv1 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
          "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
           " from VRC_Given where schedule_id='" + trigerschid[schidindex].ToString() + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + trigerdoseno[schidindex].ToString() + "'";
                                                            SqlCommand cmdcsschgiv1 = new SqlCommand(quercustgiv1, con);
                                                            cmdcsschgiv1.CommandType = CommandType.Text;
                                                            cmdcsschgiv1.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv1 = new SqlDataAdapter(cmdcsschgiv1);
                                                            DataTable dttrigergiven1 = new DataTable();
                                                            adcustgiv1.Fill(dttrigergiven1);

                                                            if (dttrigergiven1.Rows.Count != 0)
                                                            {
                                                                triggershow = true;
                                                            }
                                                            else
                                                            {
                                                                triggershow = false;
                                                            }
                                                        }
                                                        trigerschid.Add(schid);
                                                        trigerdoseno.Add(doseno);

                                                        // triggershow = true;
                                                    }
                                                }
                                            }

                                            #endregion =========== code end for vacid present in trigger schedule ============

                                            #region =========== Code start for vacid  not present in trigger schdule ===========
                                            else
                                            {

                                            }
                                            #endregion ============= Code end for vacid not present in trigger schedule ============

                                            #endregion ============ Code end for trigger schedule =======================

                                            #region ======Code start for set as previous given ==============
                                            if (doseno == 1)
                                            {
                                                Listpreviousgiven.Clear();
                                                Listpreviousvalues.Clear();
                                                Listpreviousschid.Clear();
                                                Listprevioustbltype.Clear();
                                                Listpreviousvacid.Clear();
                                                Listpreviousduedate.Clear();
                                                Listpreviousenddate.Clear();
                                                Listpreviousgivendate.Clear();

                                                // VR_GivenList vrpregiven = new VR_GivenList();
                                                //vrpregiven = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                                //DataTable dtpregiven = ConvertToDataTable(vrpregiven);

                                                string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
     "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
      " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                                SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                cmdcsschgiv.CommandType = CommandType.Text;
                                                cmdcsschgiv.ExecuteNonQuery();
                                                SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                DataTable dtpregiven = new DataTable();
                                                adcustgiv.Fill(dtpregiven);

                                                #region =======Code start dose no one present in vr_given =================
                                                if (dtpregiven.Rows.Count != 0)//vaccine id dose is already given 
                                                {
                                                    bday = dob.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgiven.Add(dtpregiven.Rows[0].ItemArray[4].ToString());
                                                    givenchkflag = true;

                                                    lblduedategivenbooster = dtpregiven.Rows[0].ItemArray[4].ToString();
                                                    Listpreviousvalues.Add(dtpregiven.Rows[0].ItemArray[4].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);

                                                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgivendate.Add(dtpregiven.Rows[0].ItemArray[4].ToString());


                                                    //List add for database 
                                                    ListVacgivenornot.Add("Yes");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);
                                                }
                                                #endregion ========== code end for dose no 1 present in vr_given ================

                                                #region ====== Code start for dose no 1 not present in vr_given ========
                                                else
                                                {
                                                    bday = dob.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {

                                                    }
                                                    givenchkflag = false;
                                                    Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);
                                                    lblDueDateGivenimgchk = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    lblduedategivenbooster = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    Listpreviousduedate.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());

                                                    firstdateduedattime = Convert.ToDateTime(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    fistdatedue = firstdateduedattime.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                    Listpreviousgivendate.Add(null);


                                                    //List add for database 
                                                    ListVacgivenornot.Add("No");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);
                                                }
                                                #endregion ========= Code end dose1 not present in vr_given==============
                                            }
                                            else if (doseno > 1)
                                            {
                                                for (int s = 0; s < Listpreviousduedate.Count; s++)
                                                {
                                                    lblprevduedatedisplay = "";
                                                    int j = Listpreviousduedate.Count;
                                                    if (Listpreviousduedate[j - 1].ToString() != "")
                                                    {
                                                        lblprevduedatedisplay = Listpreviousduedate[j - 1].ToString();
                                                        break;
                                                    }

                                                }

                                                for (int s = 0; s < Listpreviousenddate.Count; s++)
                                                {
                                                    lblprevenddatedisplay = "";
                                                    int j = Listpreviousenddate.Count;
                                                    if (Listpreviousenddate[j - 1].ToString() != "")
                                                    {
                                                        lblprevenddatedisplay = Listpreviousenddate[j - 1].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listpreviousgivendate.Count; s++)
                                                {
                                                    lblprevgivendatedisplay = "";
                                                    int j = Listpreviousgivendate.Count;
                                                    if (Listpreviousgivendate[j - 1] != null)
                                                    {
                                                        lblprevgivendatedisplay = Listpreviousgivendate[j - 1].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listpreviousschid.Count; s++)
                                                {
                                                    lblprevschids = "";
                                                    //Response.Write(Listpreviousschid[i].ToString());
                                                    int j = Listpreviousschid.Count;
                                                    // Response.Write(Listpreviousschid.Count + "j= "+ j );
                                                    if (Listpreviousschid[j - 1].ToString() != "")
                                                    {
                                                        lblprevschids = Listpreviousschid[j - j].ToString();
                                                        break;
                                                    }

                                                }
                                                for (int s = 0; s < Listprevioustbltype.Count; s++)
                                                {
                                                    lblprevtbltypes = "";
                                                    int j = Listprevioustbltype.Count;
                                                    if (Listprevioustbltype[j - 1].ToString() != "")
                                                    {
                                                        lblprevtbltypes = Listprevioustbltype[j - 1].ToString();
                                                        break;
                                                    }

                                                }

                                                #region ==================================================  set as previous given is true and false code starts =============================================

                                                string currentdoseduedate = string.Empty;

                                                if (set_as_previous == true)//set as previous given true
                                                {
                                                    // VR_GivenList vrpregiven1 = new VR_GivenList();
                                                    //  vrpregiven1 = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(lblprevschids), "S");
                                                    //  DataTable dtsgivendt1 = ConvertToDataTable(vrpregiven1);
                                                    // Response.Write("previous schid = " + lblprevschids.Text);

                                                    string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + lblprevschids + "' and child_id='" + childid + "' and from_table='S'";
                                                    SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                    cmdcsschgiv.CommandType = CommandType.Text;
                                                    cmdcsschgiv.ExecuteNonQuery();
                                                    SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                    DataTable dtsgivendt1 = new DataTable();
                                                    adcustgiv.Fill(dtsgivendt1);


                                                    if (dtsgivendt1.Rows.Count != 0)
                                                    {

                                                        if (nodue == true)
                                                        {
                                                            nodue = false;
                                                        }
                                                    }

                                                    if (lblprevgivendatedisplay != "")
                                                    {

                                                        if (Convert.ToDateTime(lblprevgivendatedisplay) <= Convert.ToDateTime(lblprevenddatedisplay))
                                                        {
                                                            currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                            }

                                                        }
                                                        else
                                                        {
                                                            //  Response.Write(lblprevgivendatedisplay.Text);
                                                            currentdosedate = Convert.ToDateTime(lblprevgivendatedisplay);
                                                            Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                            string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                            SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                            cmdbday.CommandType = CommandType.Text;
                                                            cmdbday.ExecuteNonQuery();
                                                            SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                            DataTable dtfirstdosedate = new DataTable();
                                                            adbday.Fill(dtfirstdosedate);

                                                            if (dtfirstdosedate.Rows.Count != 0)
                                                            {
                                                                currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                                // Response.Write(currentdoseduedate);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                        Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                        string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                        SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                        cmdbday.CommandType = CommandType.Text;
                                                        cmdbday.ExecuteNonQuery();
                                                        SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                        DataTable dtfirstdosedate = new DataTable();
                                                        adbday.Fill(dtfirstdosedate);

                                                        if (dtfirstdosedate.Rows.Count != 0)
                                                        {
                                                            currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    currentdosedate = Convert.ToDateTime(lblprevduedatedisplay);
                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        currentdoseduedate = dtfirstdosedate.Rows[0].ItemArray[0].ToString();
                                                    }
                                                }

                                                //VR_GivenList vrpregiven = new VR_GivenList();
                                                //vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid, schid, "S");
                                                // DataTable dtsgivendt = ConvertToDataTable(vrpregiven);

                                                string quercustgiv1 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
     "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
      " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S'";
                                                SqlCommand cmdcsschgiv1 = new SqlCommand(quercustgiv1, con);
                                                cmdcsschgiv1.CommandType = CommandType.Text;
                                                cmdcsschgiv1.ExecuteNonQuery();
                                                SqlDataAdapter adcustgiv1 = new SqlDataAdapter(cmdcsschgiv1);
                                                DataTable dtsgivendt = new DataTable();
                                                adcustgiv1.Fill(dtsgivendt);


                                                if (dtsgivendt.Rows.Count != 0)
                                                {
                                                    dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                    bday = dob1.ToString("yyyy-MM-dd");
                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + Curdosedate + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {
                                                        lblDueDateGivenimgchk = currentdoseduedate;
                                                    }
                                                    Listpreviousgiven.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());
                                                    givenchkflag = true;

                                                    Listpreviousvalues.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());
                                                    Listpreviousschid.Add(schid);
                                                    Listprevioustbltype.Add("S");
                                                    Listpreviousvacid.Add(vaccineid);

                                                    lblduedategivenbooster = dtsgivendt.Rows[0].ItemArray[4].ToString();
                                                    Listpreviousduedate.Add(currentdoseduedate);
                                                    currentdosedate = Convert.ToDateTime(currentdoseduedate);

                                                    Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                    string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + fistdatedue + "')))";
                                                    SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                    cmdend.CommandType = CommandType.Text;
                                                    cmdend.ExecuteNonQuery();
                                                    SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                    DataTable dtduedosedate = new DataTable();
                                                    adenddate.Fill(dtduedosedate);
                                                    if (dtduedosedate.Rows.Count != 0)
                                                    {
                                                        // lblduedateaftergiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                        Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                    }

                                                    Listpreviousgivendate.Add(dtsgivendt.Rows[0].ItemArray[4].ToString());

                                                    //List add for database 
                                                    ListVacgivenornot.Add("Yes");
                                                    ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                    ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                    Listvacid.Add(vaccineid);
                                                    Listschid.Add(schid);
                                                    ListVaccinedosenumber.Add(doseno);
                                                    Listcatid.Add(categoryid);

                                                }
                                                else
                                                {
                                                    dob1 = Convert.ToDateTime(lblprevduedatedisplay);
                                                    bday = dob1.ToString("yyyy-MM-dd");

                                                    string querybday = "select dateadd(YY,Convert(int,'" + d_year + "'),dateadd(M,Convert(int,'" + d_months + "'),dateadd(dd,Convert(int,'" + d_days + "'),'" + bday + "')))";
                                                    SqlCommand cmdbday = new SqlCommand(querybday, con);
                                                    cmdbday.CommandType = CommandType.Text;
                                                    cmdbday.ExecuteNonQuery();
                                                    SqlDataAdapter adbday = new SqlDataAdapter(cmdbday);
                                                    DataTable dtfirstdosedate = new DataTable();
                                                    adbday.Fill(dtfirstdosedate);

                                                    if (dtfirstdosedate.Rows.Count != 0)
                                                    {

                                                        Listpreviousvalues.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        Listpreviousschid.Add(schid);
                                                        Listprevioustbltype.Add("S");
                                                        Listpreviousvacid.Add(vaccineid);

                                                        Listpreviousgiven.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        lblduedategivenbooster = currentdoseduedate;
                                                        lblDueDateGivenimgchk = currentdoseduedate;
                                                        Listpreviousduedate.Add(currentdoseduedate);
                                                        currentdosedate = Convert.ToDateTime(currentdoseduedate);
                                                        Curdosedate = currentdosedate.ToString("yyyy-MM-dd");

                                                        string queryenddate = "select dateadd(YY,Convert(int,'" + e_year + "'),dateadd(M,Convert(int,'" + e_months + "'),dateadd(dd,Convert(int,'" + e_days + "'),'" + Curdosedate + "')))";
                                                        SqlCommand cmdend = new SqlCommand(queryenddate, con);
                                                        cmdend.CommandType = CommandType.Text;
                                                        cmdend.ExecuteNonQuery();
                                                        SqlDataAdapter adenddate = new SqlDataAdapter(cmdend);
                                                        DataTable dtduedosedate = new DataTable();
                                                        adenddate.Fill(dtduedosedate);
                                                        if (dtduedosedate.Rows.Count != 0)
                                                        {
                                                            // lblDueDateGiven.Text = "Due on " + Convert.ToDateTime(dtduedosedate.Rows[0].ItemArray[0].ToString()).ToString(Label1.Text);
                                                            Listpreviousenddate.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                            lblendDate = dtduedosedate.Rows[0].ItemArray[0].ToString();
                                                        }
                                                        Listpreviousgivendate.Add(null);

                                                        //List add for database 
                                                        ListVacgivenornot.Add("No");
                                                        ListDuedates.Add(dtfirstdosedate.Rows[0].ItemArray[0].ToString());
                                                        ListEndDates.Add(dtduedosedate.Rows[0].ItemArray[0].ToString());
                                                        Listvacid.Add(vaccineid);
                                                        Listschid.Add(schid);
                                                        ListVaccinedosenumber.Add(doseno);
                                                        Listcatid.Add(categoryid);
                                                    }
                                                }
                                                #endregion =============================================== set as previous given code ends ===============================================

                                            }

                                            #endregion ======== Code end for set as previous given ============

                                            #region =================Code start for Vacid present in vr_given ====================================
                                            //   VR_GivenList givenlist = new VR_GivenList();
                                            //  givenlist = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                            // DataTable dtgiven1 = ConvertToDataTable(givenlist);

                                            string quercustgiv2 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
       "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
        " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                            SqlCommand cmdcsschgiv2 = new SqlCommand(quercustgiv2, con);
                                            cmdcsschgiv2.CommandType = CommandType.Text;
                                            cmdcsschgiv2.ExecuteNonQuery();
                                            SqlDataAdapter adcustgiv2 = new SqlDataAdapter(cmdcsschgiv2);
                                            DataTable dtgiven1 = new DataTable();
                                            adcustgiv2.Fill(dtgiven1);

                                            if (dtgiven1.Rows.Count != 0)
                                            {
                                                givenid = Convert.ToInt32(dtgiven1.Rows[0].ItemArray[0].ToString());
                                                givendate = Convert.ToDateTime(dtgiven1.Rows[0].ItemArray[4].ToString());
                                                // dosegivenornot ="Yes";
                                                givenchkflag = true;
                                            }
                                            else
                                            {
                                                givenchkflag = false;
                                                givendate = DateTime.Now;
                                                givenid = 0;
                                                // dosegivenornot="No";
                                            }
                                            #endregion ================ Code end for Vacid present in vr_given  ==================================

                                            #region ======================================= booster dose code starts ========================================================================================

                                            bool showdotsbooster = true;
                                            if (booster == "True")
                                            {
                                                bool chkgiven = false;
                                                if (Convert.ToDateTime(lblduedategivenbooster) > DateTime.Now)
                                                {
                                                    showdotsbooster = false;
                                                }
                                                else
                                                {
                                                    string strdate = null;
                                                    string[] strArrdate = null;

                                                    //int count = 0;
                                                    strdate = lblboosterdoses;
                                                    char[] splitchar11 = { ',' };


                                                    strArrdate = strdate.Split(splitchar11);

                                                    for (int b = 0; b < strArrdate.Length; b++)
                                                    {
                                                        for (int s = 0; s <= b - 1; s++)
                                                        {
                                                            // VR_GivenList vrpregiven = new VR_GivenList();
                                                            // vrpregiven = VR_GivenDA.gridviewchidschidtbltypelist(childid, Convert.ToInt32(Listpreviousschid[s]), "S");
                                                            // DataTable dtboster = ConvertToDataTable(vrpregiven);

                                                            string quercustgiv = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
      "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
       " from VRC_Given where schedule_id='" + Listpreviousschid[s] + "' and child_id='" + childid + "' and from_table='S'";
                                                            SqlCommand cmdcsschgiv = new SqlCommand(quercustgiv, con);
                                                            cmdcsschgiv.CommandType = CommandType.Text;
                                                            cmdcsschgiv.ExecuteNonQuery();
                                                            SqlDataAdapter adcustgiv = new SqlDataAdapter(cmdcsschgiv);
                                                            DataTable dtboster = new DataTable();
                                                            adcustgiv.Fill(dtboster);

                                                            if (dtboster.Rows.Count != 0)
                                                            {
                                                                chkgiven = true;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                chkgiven = false;
                                                            }
                                                            if (chkgiven == true)
                                                                break;
                                                        }
                                                    }

                                                    if (chkgiven)
                                                    {
                                                        showdotsbooster = false;
                                                    }
                                                    else
                                                    {

                                                        showdotsbooster = true;
                                                    }

                                                }
                                            }

                                            #endregion ==================================== booster dose code ends ==========================================================================================

                                            #region =================Code start for Not Compulsory ==========================================
                                            //VRSchedulelist vrsch = new VRSchedulelist();
                                            // vrsch = VRScheduleDA.gridviewvacschdoseno(schid, vaccineid, doseno, countryid, stateid, Vacyear);
                                            //  DataTable dtnotcosly = ConvertToDataTable(vrsch);

                                            string quercust1 = "SELECT schedule_id,ISNULL(vaccine_id,'') as vaccine_id , ISNULL( country_id,'') as country_id, ISNULL( Year,'') as Year,ISNULL( Due_Days,'') as Due_Days," +
         "ISNULL(Due_Months,'') as Due_Months,ISNULL( Due_Years,'') as Due_Years,ISNULL( End_Days,'') as End_Days,ISNULL( End_Months,'') as End_Months,ISNULL( End_Years,'') as End_Years," +
         "ISNULL( Dose_No,'') as Dose_No,ISNULL( Dose_Name,'') as Dose_Name,ISNULL( Set_as_Previous_Given,'') as Set_as_Previous_Given,ISNULL( Booster,'') as Booster," +
         "ISNULL( Booster_Doses,'') as Booster_Doses,ISNULL( Dose_Desc,'') as Dose_Desc, ISNULL( NotCompulsary,'') as NotCompulsary,ISNULL( stateid,'') as stateid,ISNULL( No_Due_Date,'') as No_Due_Date" +
        " from VRC_Schedule where schedule_id='" + schid + "' and vaccine_id='" + vaccineid + "' and Dose_No='" + doseno + "' and userid='" + userid + "'";
                                            SqlCommand cmdcssch1 = new SqlCommand(quercust1, con);
                                            cmdcssch1.CommandType = CommandType.Text;
                                            cmdcssch1.ExecuteNonQuery();
                                            SqlDataAdapter adcust1 = new SqlDataAdapter(cmdcssch1);
                                            DataTable dtnotcosly = new DataTable();
                                            adcust1.Fill(dtnotcosly);


                                            if (dtnotcosly.Rows.Count != 0)
                                            {
                                                if (dtnotcosly.Rows[0].ItemArray[16].ToString() == "True")
                                                {
                                                    compulsoryflag = true;
                                                }
                                                else
                                                {
                                                    compulsoryflag = false;
                                                }
                                            }
                                            #endregion ============================ code end for Not compulsory =============================

                                            #region ================== code start for reminder on ================================

                                            //Code start for reminder set on 

                                            bool reminder1set = false;
                                            bool reminder2set = false;
                                            string givenreminderdate = lblDueDateGivenimgchk;

                                            VR_ReminderChangeslist vrrchnglist = new VR_ReminderChangeslist();
                                            vrrchnglist = VR_ReminderChngDAController.gridviewscheduleid(userid, childid, vaccineid, schid, doseno, "S");
                                            DataTable dtchngreminder = ConvertToDataTable(vrrchnglist);

                                            if (dtchngreminder.Rows.Count != 0)
                                            {
                                                if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "1")
                                                {
                                                    reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                    //string date1 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                    ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                }
                                                else if (dtchngreminder.Rows[0].ItemArray[7].ToString() == "2")
                                                {
                                                    reminderdate = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString());
                                                    // string date2 = Convert.ToDateTime(dtchngreminder.Rows[0].ItemArray[8].ToString()).ToShortDateString();
                                                    ListReminderondate.Add(dtchngreminder.Rows[0].ItemArray[8].ToString());

                                                }
                                            }
                                            else
                                            {
                                                VR_ReminderSettingList vrremindsetting = new VR_ReminderSettingList();
                                                vrremindsetting = VR_ReminderSettingDAController.gridviewuid(userid, "True");
                                                DataTable dtRemindsetng = ConvertToDataTable(vrremindsetting);
                                                if (dtRemindsetng.Rows.Count != 0)
                                                {
                                                    if (reminder1set == false)
                                                    {
                                                        string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                        ListReminderondate.Add(date1);
                                                        reminderdate = Convert.ToDateTime(date1);
                                                    }
                                                    else
                                                    {
                                                        if (dtRemindsetng.Rows[0].ItemArray[6].ToString() == "True")
                                                        {
                                                            reminder2set = true;
                                                            if (reminder2set == true)
                                                            {
                                                                string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtRemindsetng.Rows[0].ItemArray[3].ToString())).ToShortDateString();
                                                                ListReminderondate.Add(date2);
                                                                reminderdate = Convert.ToDateTime(date2);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            reminder2set = false;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    VR_DefaultSettingLsit vrdefalutlist = new VR_DefaultSettingLsit();
                                                    vrdefalutlist = VR_DefaultSettingDAController.gridview();
                                                    DataTable dtdefaultsetng = ConvertToDataTable(vrdefalutlist);
                                                    if (dtdefaultsetng.Rows.Count != 0)
                                                    {
                                                        if (reminder1set == false)
                                                        {
                                                            string date1 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[1].ToString())).ToShortDateString();
                                                            ListReminderondate.Add(date1);
                                                            reminderdate = Convert.ToDateTime(date1);
                                                        }
                                                        else
                                                        {
                                                            if (dtdefaultsetng.Rows[0].ItemArray[5].ToString() == "True")
                                                            {
                                                                reminder2set = true;
                                                                if (reminder2set == true)
                                                                {
                                                                    string date2 = Convert.ToDateTime(lblDueDateGivenimgchk).AddDays(-Convert.ToInt32(dtdefaultsetng.Rows[0].ItemArray[2].ToString())).ToShortDateString();
                                                                    ListReminderondate.Add(date2);
                                                                    reminderdate = Convert.ToDateTime(date2);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                reminder2set = false;

                                                            }

                                                        }

                                                    }

                                                }

                                            }


                                            #endregion ================= end code for reminder on ================================

                                            #region ===================================== code for give vaccine(Submit and remove) start ========================================================

                                            // VR_GivenList vrvacsubmit = new VR_GivenList();
                                            //  vrvacsubmit = VR_GivenDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, "S");
                                            // DataTable dtschgivenchk = ConvertToDataTable(vrvacsubmit);
                                            string quercust3 = "SELECT ISNULL(givenid,'') as givenid,ISNULL(schedule_id,'') as schedule_id,ISNULL(child_id,'') as child_id," +
    "ISNULL(from_table,'') as from_table,ISNULL(given_date,'') as given_date,ISNULL(Dose_No,'') as Dose_No" +
    " from VRC_Given where schedule_id='" + schid + "' and child_id='" + childid + "' and from_table='S' and Dose_No='" + doseno + "'";
                                            SqlCommand cmdcssch3 = new SqlCommand(quercust3, con);
                                            cmdcssch3.CommandType = CommandType.Text;
                                            cmdcssch3.ExecuteNonQuery();
                                            SqlDataAdapter adcust3 = new SqlDataAdapter(cmdcssch3);
                                            DataTable dtschgivenchk = new DataTable();
                                            adcust3.Fill(dtschgivenchk);

                                            if (dtschgivenchk.Rows.Count != 0)
                                            {
                                            }
                                            else
                                            {
                                                #region==================================================================skip code starts ==============================================================
                                                // VR_SkipSchlist vrskiplist = new VR_SkipSchlist();
                                                // vrskiplist = VR_SkipschDA.gridviewchidschiddsnotbltypelist(childid, schid, doseno, vaccineid, "S");
                                                // DataTable dtskip = ConvertToDataTable(vrskiplist);

                                                string quercust2 = "SELECT skipid,ISNULL(schedule_id,'0')as schedule_id ,ISNULL(vaccine_id,'0') as vaccine_id,ISNULL(child_id,'0') as child_id," +
  "ISNULL(from_table,'') as  from_table,ISNULL(Dose_No,'0') as Dose_No,ISNULL(skipdate,'') as skipdate from vrc_skip " +
  "where schedule_id='" + schid + "' and vaccine_id='" + vaccineid + "' and child_id='" + childid + "' and Dose_No='" + doseno + "' and from_table='S'";
                                                SqlCommand cmdcssch2 = new SqlCommand(quercust2, con);
                                                cmdcssch2.CommandType = CommandType.Text;
                                                cmdcssch2.ExecuteNonQuery();
                                                SqlDataAdapter adcust2 = new SqlDataAdapter(cmdcssch2);
                                                DataTable dtskip = new DataTable();
                                                adcust2.Fill(dtskip);
                                                if (dtskip.Rows.Count != 0)
                                                {
                                                    skipid = Convert.ToInt32(dtskip.Rows[0].ItemArray[0].ToString());
                                                    skipdate = Convert.ToDateTime(dtskip.Rows[0].ItemArray[6].ToString());
                                                    skipflag = true;
                                                }
                                                else
                                                {
                                                    skipflag = false;
                                                }

                                                #endregion==================================================================skip code ends ==============================================================
                                            }
                                            #endregion =================================== code for give vaccine(Submit and remove) end =========================================================

                                            #region ================ Improved code start for count the doses ===================================
                                            if (givenchkflag == true)
                                            {
                                                if (showdotsbooster == false)
                                                {
                                                    dosecolor = "";
                                                }
                                                else
                                                {
                                                    if (triggershow == true)
                                                    {
                                                        noofgreenvaccines++;
                                                        dosecolor = "Green";
                                                    }
                                                }
                                            }
                                            else if (skipflag == true || compulsoryflag == true || nodue == true)
                                            {
                                                if (showdotsbooster == false)
                                                {
                                                    dosecolor = "";
                                                }
                                                else
                                                {
                                                    if (triggershow == true)
                                                    {

                                                        // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                        noofgrayvaccines++;
                                                        dosecolor = "Gray";
                                                    }
                                                    else
                                                    {
                                                        dosecolor = "";
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                if (Convert.ToDateTime(lblDueDateGivenimgchk) < DateTime.Now)
                                                {
                                                    if (showdotsbooster == false)
                                                    {
                                                        dosecolor = "";
                                                    }
                                                    else
                                                    {
                                                        if (triggershow == true)
                                                        {

                                                            // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                            noofredvaccines++;
                                                            dosecolor = "Red";
                                                        }
                                                        else
                                                        {
                                                            dosecolor = "";
                                                        }
                                                    }
                                                }
                                                else if (Convert.ToDateTime(lblDueDateGivenimgchk) > DateTime.Now)
                                                {
                                                    int days = ((Convert.ToDateTime(lblDueDateGivenimgchk) - DateTime.Now).Days);
                                                    if (days < orangedays)
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);

                                                                nooforangevaccines++;
                                                                dosecolor = "Orange";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }
                                                        }

                                                    }
                                                    else
                                                    {
                                                        if (showdotsbooster == false)
                                                        {
                                                            dosecolor = "";
                                                        }
                                                        else
                                                        {
                                                            if (triggershow == true)
                                                            {

                                                                // Console.Write("forskipcompulsoryflag and nodue vacid =" + vaccineid + "doseno =" + doseno + "duedate= " + lblDueDateGivenimgchk);
                                                                noofgrayvaccines++;
                                                                dosecolor = "Gray";
                                                            }
                                                            else
                                                            {
                                                                dosecolor = "";
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            #endregion ================== improved code end for count the doses =============================

                                            Console.WriteLine("vacid=" + vaccineid);
                                            finalobject.schid = schid;
                                            finalobject.vacid = vaccineid;
                                            finalobject.catid = categoryid;
                                            finalobject.childid = childid;
                                            finalobject.doseno = doseno;
                                            finalobject.gender = sex;
                                            finalobject.countryid = countryid;
                                            finalobject.stateid = stateid;
                                            finalobject.vaccineyear = Vacyear;
                                            finalobject.DueDays = d_days;
                                            finalobject.DueMonths = d_months;
                                            finalobject.DueYears = d_year;
                                            finalobject.EndDays = e_days;
                                            finalobject.EndMonth = e_months;
                                            finalobject.EndYear = e_year;
                                            finalobject.DueOnDate = Convert.ToDateTime(lblDueDateGivenimgchk);
                                            finalobject.EndDueDate = Convert.ToDateTime(lblendDate);
                                            finalobject.ReminderOnDate = reminderdate;
                                            finalobject.givenid = givenid;
                                            finalobject.DateGiven = givendate;
                                            finalobject.skipid = skipid;
                                            finalobject.skipdate = skipdate;
                                            finalobject.set_as_previous_given = set_as_previous;
                                            finalobject.dosegivenornot = givenchkflag;
                                            finalobject.skipornot = skipflag;
                                            finalobject.notcompulsory = compulsoryflag;
                                            finalobject.triggershowflag = TriggerShowVac;
                                            finalobject.nodueflag = TriggerNoDue;
                                            finalobject.Booster = Convert.ToBoolean(booster);
                                            finalobject.BoosterDoses = lblboosterdoses;
                                            finalobject.dosecolour = dosecolor;
                                            childsch.Add(finalobject);

                                        }
                                    }

                                    #endregion ===================== Code end for custom schedule ============================
                                }
                            }
                            #endregion ================== end code for  Contains only one vaccine id =====================
                        }
                    }

                    #region ============ Code start for if child chnge their country or any update details then bind the previous country vaccinde doses that the child given =============================
                    string querycountrychng = "";
                    if (custsch != true)
                    {
                        querycountrychng = "select * from VR_Given where schedule_id not in (select schedule_id from VR_Schedule where country_id='" + countryid + "' and Year='" + Vacyear + "' and stateid='" + stateid + "') and child_id='" + childid + "' and from_table='S' order by given_date";
                    }
                    else
                    {
                        querycountrychng = "select * from VRC_Given where schedule_id not in (select schedule_id from VRC_Schedule where userid='" + userid + "') and child_id='" + childid + "' and from_table='S' order by given_date";
                    }

                    SqlCommand cmdcoutnry = new SqlCommand(querycountrychng, con);
                    cmdcoutnry.CommandType = CommandType.Text;
                    cmdcoutnry.ExecuteNonQuery();
                    SqlDataAdapter adcountry = new SqlDataAdapter(cmdcoutnry);
                    DataTable dtgivencountry = new DataTable();
                    adcountry.Fill(dtgivencountry);
                    if (dtgivencountry.Rows.Count != 0)
                    {
                        for (int i = 0; i < dtgivencountry.Rows.Count; i++)
                        {
                            noofgreenvaccines++;
                        }
                    }
                    else
                    {
                    }

                    #endregion ================= Code end for  if child chnge their country or any update details then bind the previous country vaccinde doses that the child given =============================

                }
                catch (Exception ex)
                {
                    con.Close();
                    Console.Write(ex.StackTrace);
                    throw;
                }
                finally { con.Close(); }
            }

            fnch.redcount = noofredvaccines;
            fnch.greencount = noofgreenvaccines;
            fnch.graycount = noofgrayvaccines;
            fnch.orangecount = nooforangevaccines;
            fnch.schlist = childsch;
            return fnch;
        }

        // PUT api/generatescheapi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/generatescheapi/5
        public void Delete(int id)
        {
        }

        public DataTable ConvertToDataTable<T>(IList<T> data) //Convert the datalist to the datatable
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
    }
}
