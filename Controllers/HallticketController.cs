using Gstudent.Models;
using Gstudent.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Gstudent.ViewModels.Studentrecords;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Gstudent.Controllers
{
    public class HallticketController : Controller
    {
        private readonly IPasswordRepository passwordrepository;
        private readonly IStudentRepository iStudentRepository;

        String userid = null, type = null, code = null;
        String ip; int id;
        public HallticketController(IPasswordRepository _IPasswordRepository, IStudentRepository _IStudentRepository)
        {
            this.passwordrepository = _IPasswordRepository;
            this.iStudentRepository = _IStudentRepository;
        }
        // GET: Password
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Loginuser(FormCollection collection)
        {

            Session["user_type"] = "";
            string[] user_id = collection.GetValues("user_id");
            string[] password = collection.GetValues("password");

            Userlogin cm = await passwordrepository.getEmplogin(user_id[0], password[0]);

            if (cm != null)
            {
                Session["EMPID"] = cm.EMPID.ToString();
                Session["EMP_NAME"] = cm.EMP_NAME.ToString();
                Session["DEPT_CODE"] = cm.dept_code.ToString();
               
                    return RedirectToAction("Stdhallticket");
                
            }
            else
            {
                return Json("Password", "Index");
            }
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        public ActionResult Stdhallticket()
        {
            return View();
        }
        public ActionResult reset()
        {
            return RedirectToAction("Password", "Index");
        }


        [HttpGet]
        public async Task<ActionResult> btn_get(string regno)
        {
            Students data = new Students();
            regno = regno.Trim();
            try
            {
                data = await passwordrepository.getStudentsdata(regno);
                if (data != null)
                {


                    Session["uid"] = data.regdno;
                    Session["college_code"] = data.COLLEGE_CODE;
                    Session["campus_code"] = data.CAMPUS;
                    Session["gender"] = data.GENDER;
                    Session["degree_code"] = data.degree_code;
                    Session["branch_code"] = data.BRANCH_CODE;
                    Session["batch"] = data.batch;
                    Session["section"] = data.section;
                    Session["course_code"] = data.COURSE_CODE;
                    Session["name"] = data.name;
                    Session["Curr_sem"] = data.CURRSEM;
                    Session["dept_code"] = data.dept_code;
                    string campus = Session["campus_code"].ToString();





                    if (campus.Equals("VSP"))
                    {
                        Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_VSP";
                    }

                    else if (campus.Equals("HYD"))
                    {
                        Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_HYD";
                    }

                    else if (campus.Equals("BLR"))
                    {
                        Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_BLR";
                    }
                    if (campus.Equals("VSP"))
                    {
                        data.CAMPUS = "Visakhapatnam";
                    }
                    else if (campus.Equals("HYD"))
                    {
                        data.CAMPUS = "Hyderabad";
                    }
                    else if (campus.Equals("BLR"))
                    {
                        data.CAMPUS = "Bengaluru";
                    }

                    if (data.COLLEGE_CODE == "GIT" && campus == "VSP")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Technology";
                    }

                    else if (data.COLLEGE_CODE == "GIT" && campus == "HYD")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Technology";
                    }
                    else if (data.COLLEGE_CODE == "GST")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Technology";
                    }
                    else if (data.COLLEGE_CODE == "GIP")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Pharmacy";
                    }
                    else if (data.COLLEGE_CODE == "GSL")
                    {

                        data.COLLEGE_CODE = "GITAM School of Law";
                    }
                    else if (data.COLLEGE_CODE == "GIIB")
                    {

                        data.COLLEGE_CODE = "GITAM School of International Business";
                    }
                    else if (data.COLLEGE_CODE == "GIM")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Management";
                    }
                    else if (data.COLLEGE_CODE == "GIS")
                    {

                        data.COLLEGE_CODE = "GITAM Institute of Science";
                    }
                    else if (data.COLLEGE_CODE == "GSA")
                    {

                        data.COLLEGE_CODE = "GITAM School of Architecture";
                    }
                    else if (data.COLLEGE_CODE == "GIMSRC")
                    {

                        data.COLLEGE_CODE = "GITAM INSTITUTE OF MEDICAL SCIENCES AND RESEARCH";
                    }
                    else if (data.COLLEGE_CODE == "GIN")
                    {

                        data.COLLEGE_CODE = "GITAM School of Nursing";
                    }
                    else if (data.COLLEGE_CODE == "HBS")
                    {

                        data.COLLEGE_CODE = "HYDERABAD BUSINESS SCHOOL";
                    }
                    else if (data.COLLEGE_CODE == "SMS")
                    {

                        data.COLLEGE_CODE = "Bengaluru School of Management Studies";
                    }

                    Session["student_id"] = regno;

                    string str = "<img src=https://doeresults.gitam.edu/photo/img.aspx?id=" + regno.Trim() + " class='t-w-100 mx-auto d-block' />";

                    data.qrtext = str;
                }
                else
                {
                    data = new Students();
                    data.qrtext = "Invalid student";
                }




            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }



        [HttpGet]
        public async Task<ActionResult> btn_reset()
        {
            Students data = new Students();
            try
            {
                //data = await passwordrepository.getStudentsdata(regno);
                string student_id = Session["student_id"].ToString();
                string user_id = Session["EMPID"].ToString();
                string user_name = Session["EMP_NAME"].ToString();
                string txtNewPWD = "Gitam@123";
                string crypt = CryptSharp.Crypter.Sha256.GenerateSalt();
                String encpwd = CryptSharp.Crypter.Sha256.Crypt(txtNewPWD, crypt);

                int result = await passwordrepository.UpdateStudentPassword(student_id, user_id, txtNewPWD);
                if (result == 1)
                {
                    data.qrtext = "Reset successfull";
                }
                else
                {
                    data.qrtext = "cancelled";
                }


            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public async Task<ActionResult> GetSearchAutocompletestudent(string SearchName, string check)
        {

            IEnumerable<Students> AutoComplete = await passwordrepository.GetSearchAutocompletestudent(SearchName, check);
            List<Students> asList = AutoComplete.ToList();

            return Json(AutoComplete, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> Gethalltickets(string regno)
        {
            Hallticket chart = new Hallticket();
            Students data = new Students();
            hallticketsummary admission = new hallticketsummary();

            admission.studentdetails = await passwordrepository.getStudentsdatahallget(regno);
            if (chart != null)
            {
                Session["uid"] = admission.studentdetails.FirstOrDefault().regdno;
                Session["college_code"] = admission.studentdetails.FirstOrDefault().college_code;
                Session["campus_code"] = admission.studentdetails.FirstOrDefault().campus;
                Session["gender"] = admission.studentdetails.FirstOrDefault().GENDER;
                Session["degree_code"] = admission.studentdetails.FirstOrDefault().degree_code;
                Session["branch_code"] = admission.studentdetails.FirstOrDefault().branch_code;
                Session["batch"] = admission.studentdetails.FirstOrDefault().batch;
                Session["section"] = admission.studentdetails.FirstOrDefault().section;
                Session["course_code"] = admission.studentdetails.FirstOrDefault().course_code;
                Session["name"] = admission.studentdetails.FirstOrDefault().NAME;
                Session["Curr_sem"] = admission.studentdetails.FirstOrDefault().CURRSEM;
                Session["dept_code"] = admission.studentdetails.FirstOrDefault().dept_code;
                Session["dob"] = admission.studentdetails.FirstOrDefault().dob;
                Session["Father_Name"] = admission.studentdetails.FirstOrDefault().FATHER_NAME;
                string campus = Session["campus_code"].ToString();
                if (campus.Equals("VSP"))
                {
                    Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_VSP";
                }

                else if (campus.Equals("HYD"))
                {
                    Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_HYD";
                }

                else if (campus.Equals("BLR"))
                {
                    Session["ATTENDANCEREPORTTABLE"] = "STUDENT_ATTENDANCE_BLR";
                }
                if (campus.Equals("VSP"))
                {
                    admission.studentdetails.FirstOrDefault().campus = "Visakhapatnam";
                }
                else if (campus.Equals("HYD"))
                {
                    admission.studentdetails.FirstOrDefault().campus = "Hyderabad";
                }
                else if (campus.Equals("BLR"))
                {
                    admission.studentdetails.FirstOrDefault().campus = "Bengaluru";
                }

                if (admission.studentdetails.FirstOrDefault().college_code == "GIT" && campus == "VSP")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Technology";
                }

                else if (admission.studentdetails.FirstOrDefault().college_code == "GIT" && campus == "HYD")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Technology";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GST")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Technology";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIP")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Pharmacy";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GSL")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM School of Law";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIIB")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM School of International Business";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIM")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Management";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIS")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM Institute of Science";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GSA")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM School of Architecture";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIMSRC")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM INSTITUTE OF MEDICAL SCIENCES AND RESEARCH";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "GIN")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "GITAM School of Nursing";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "HBS")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "HYDERABAD BUSINESS SCHOOL";
                }
                else if (admission.studentdetails.FirstOrDefault().college_code == "SMS")
                {

                    admission.studentdetails.FirstOrDefault().college_code = "Bengaluru School of Management Studies";
                }

                Session["student_id"] = regno;

                string str = "<img src=https://doeresults.gitam.edu/photo/img.aspx?id=" + regno.Trim() + " class='t-w-100 mx-auto d-block' />";

                admission.studentdetails.FirstOrDefault().qrtext = str;
            }
            else
            {
                admission.studentdetails.FirstOrDefault().qrtext = "Invalid student";
            }
            IEnumerable<Hallticket> txndata = await iStudentRepository.gethallticketactive(Convert.ToString(Session["uid"]));

            if (txndata.Count() > 0)
            {
                admission.activecount = txndata.Count();
            }

            try
            {
                //throw new Exception("500");
                string regdno = Session["uid"].ToString();
                string course_code = Convert.ToString(Session["course_code"]);
                string college_code = Session["college_code"].ToString();
                string campus_code = Session["campus_code"].ToString();
                string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();

                string cur_sem = Session["Curr_sem"].ToString();
                string batch = Session["batch"].ToString();
                string[] batch1 = new string[1000];
                batch1 = batch.Split('-');
                string batch2 = batch1[0];
                Session["batchattd"] = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
                string batchattd = Session["batchattd"].ToString();
                string user_id = Session["uid"].ToString();


                if (course_code == "PHD")
                {
                    admission.notes = await iStudentRepository.gethallticketPHDdata52(regdno);
                    admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(regdno);
                    if (admission.note == null)
                        admission.note = new Hallticket();
                }

                else
                {
                    admission.notes = await iStudentRepository.gethallticketdata52(regdno);
                    admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(regdno);
                    if (admission.note == null)
                        admission.note = new Hallticket();
                }
                for (int i = 0; i < admission.notes.Count(); i++)
                {
                    Session["typesss"] = admission.notes.ElementAt(i).type1;
                    if (admission.notes.ElementAt(i).type1.Equals("R"))
                    {
                        admission.notes.ElementAt(i).type2 = "Regular";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("RE"))
                    {
                        admission.notes.ElementAt(i).type2 = "RE Registration";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("SLM"))
                    {
                        admission.notes.ElementAt(i).type2 = "SLM";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("S"))
                    {
                        admission.notes.ElementAt(i).type2 = "Supplementary";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("SP"))
                    {
                        admission.notes.ElementAt(i).type2 = "Special Exam";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("SD"))
                    {
                        admission.notes.ElementAt(i).type2 = "Special Drive";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("I"))
                    {
                        admission.notes.ElementAt(i).type2 = "Instant Exam";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("B"))
                    {
                        admission.notes.ElementAt(i).type2 = "Betterment Exam";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("BG"))
                    {
                        admission.notes.ElementAt(i).type2 = "Betterment of Grade";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("ST"))
                    {
                        admission.notes.ElementAt(i).type2 = "Summer Term";
                    }
                    else if (admission.notes.ElementAt(i).type1.Equals("CC"))
                    {
                        admission.notes.ElementAt(i).type2 = "Compressed Courses";
                    }
                    else if (admission.notes.ElementAt(i).type1.ToUpper().Equals("MID"))
                    {
                        admission.notes.ElementAt(i).type2 = "Mid Examination";
                    }
                    else
                    {
                        admission.notes.ElementAt(i).type2 = admission.notes.ElementAt(i).type1.ToUpper();
                    }
                }
            }

            catch (Exception ex)
            {
                RedirectToAction("https://login.gitam.edu");
                return Json(admission, null);
            }
            return Json(admission, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> btnsubmit_Click(string regdno, string type, string semester, string tick)
        {
            Hallticket chart = new Hallticket();
            chart.type1 = type;
            chart.id1 = semester;
            chart.tick = tick;
            hallticketsummary admission = new hallticketsummary();
            //string regdno = Session["uid"].ToString();
            chart.course_code = Convert.ToString(Session["course_code"]);
            if (chart.type1.Equals("R"))
            {
                chart.type2 = "Regular";
            }
            else if (chart.type1.Equals("RE"))
            {
                chart.type2 = "RE Registartion";
            }
            else if (chart.type1.Equals("SLM"))
            {
                chart.type2 = "SLM";
            }
            else if (chart.type1.Equals("S"))
            {
                chart.type2 = "Supplementary";
            }
            else if (chart.type1.Equals("SP"))
            {
                chart.type2 = "Special Exam";
            }
            else if (chart.type1.Equals("SD"))
            {
                chart.type2 = "Special Drive";
            }
            else if (chart.type1.Equals("I"))
            {
                chart.type2 = "Instant Exam";
            }
            else if (chart.type1.Equals("B"))
            {
                chart.type2 = "Betterment Exam";
            }
            else if (chart.type1.Equals("BG"))
            {
                chart.type2 = "Betterment of Grade";
            }
            else if (chart.type1.Equals("ST"))
            {
                chart.type2 = "Summer Term";
            }
            else if (chart.type1.Equals("CC"))
            {
                chart.type2 = "Compressed Courses";
            }
            else if (chart.type1.ToUpper().Equals("MID"))
            {
                chart.type2 = "Mid Examination";
            }
            else
            {
                chart.type2 = chart.type1.ToUpper();
            }

            if (chart.course_code.Equals("PHD"))
            {
                chart.regdno = Session["uid"].ToString();
                chart.CURRSEM = Session["Curr_sem"].ToString();
                Session["finalsem"] = semester;
                Session["type1"] = chart.type1;
                Session["type2"] = chart.type2;
                admission = await PHD_HALL_TICKET(chart);
                return View("NEW_HallTickets", admission);
            }
            else
            {
                var student = await iStudentRepository.get_student_data52(regdno);
                try
                {
                    if (student != null)
                    {
                        if (student.ElementAt(0).college_code.Equals("GIMSR"))
                        {
                            if (chart.type1 == "R" || chart.type1 == "S" || chart.type1 == "RE" || chart.type1 == "SLM")
                            {
                                if (student.ElementAt(0).status.Equals(null) || student.ElementAt(0).status.Equals("A") || student.ElementAt(0).status.Equals("") || student.ElementAt(0).status.Equals("NULL") || student.ElementAt(0).status.Equals("S") || student.ElementAt(0).status.Equals("D"))
                                {
                                    String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
                                    chart.regdno = Session["uid"].ToString();
                                    chart.CURRSEM = Session["Curr_sem"].ToString();
                                    Session["type1"] = chart.type1;
                                    Session["type2"] = chart.type2;
                                    admission = await NEW_HallTickets(chart);
                                    return View("NEW_HallTickets", admission);
                                }
                                else
                                {
                                    chart.msg = "1Please contact HOD's office..";
                                }
                            }
                        }
                        else
                        {
                            if (chart.type1 == "R" || chart.type1 == "RE" || chart.type1 == "SLM")
                            {
                                if (student.ElementAt(0).status.Equals(null) || student.ElementAt(0).status.Equals("A") || student.ElementAt(0).status.Equals("") || student.ElementAt(0).status.Equals("NULL") || student.ElementAt(0).status.Equals("S") || student.ElementAt(0).status.Equals("D"))
                                {
                                    String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
                                    chart.regdno = Session["uid"].ToString();
                                    //string semes = Session["Curr_sem"].ToString();
                                    //string[] semes1 = new string[1000];
                                    //semes1 = semes.Split('-');

                                    //string batch2 = semes1[0];
                                    //int batchInt = Convert.ToInt32(batch2);
                                    //int idInt = batchInt - 1;
                                    //string id1 = idInt.ToString();
                                    //chart.id1 = id1;
                                    Session["id1"] = chart.id1;

                                    Session["type1"] = chart.type1;
                                    Session["type2"] = chart.type2;

                                    admission = await NEW_HallTickets(chart);
                                    return View("NEW_HallTickets", admission);
                                }
                                else
                                {
                                    chart.msg = "1Please contact HOD's office..";
                                }
                            }
                            else if (chart.type1 == "SD" || chart.type1 == "B")
                            {
                                chart.type1 = type;
                                chart.id1 = semester;
                                chart.tick = tick;
                                String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
                                chart.regdno = Session["uid"].ToString();
                                chart.CURRSEM = Session["Curr_sem"].ToString();
                                Session["type1"] = chart.type1;
                                Session["type2"] = chart.type2;

                                //if (chart.tick == "Y")
                                //{
                                admission = await NEW_HallTickets(chart);
                                return View("NEW_HallTickets", admission);
                                //}
                                //else
                                //{
                                //    admission = await NEW_HallTickets(chart);
                                //    return View("hallticketinstruction", admission);

                                //}
                            }
                            else
                            {
                                String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
                                chart.regdno = Session["uid"].ToString();
                                chart.CURRSEM = Session["Curr_sem"].ToString();
                                Session["type1"] = chart.type1;
                                Session["type2"] = chart.type2;
                                admission = await NEW_HallTickets(chart);
                                return View("NEW_HallTickets", admission);
                            }
                        }
                    }
                    //}
                    else
                    {
                        chart.msg = "1Please contact HOD's office..";
                    }

                }
                catch (Exception ex)
                {
                    chart.msg = "1Invalid details please try again /Please contact HOD's office";
                }
            }
            return PartialView("Hallticket", admission);
        }

        [HttpPost]
        public async Task<hallticketsummary> NEW_HallTickets(Hallticket chart)
        {
            hallticketsummary admission = new hallticketsummary();
            IEnumerable<Hallticket> profiledata = null;
            IEnumerable<Hallticket> details = null;
            try
            {
                chart.regdno = Session["uid"].ToString();
                chart.type1 = Session["type1"].ToString();
                chart.type2 = Session["type2"].ToString();
                chart.course_code = Convert.ToString(Session["course_code"]);
                chart.branch_code = Convert.ToString(Session["branch_code"]);
                chart.degree_code = Convert.ToString(Session["degree_code"]);
                chart.college_code = Convert.ToString(Session["college_code"]);

                var student = iStudentRepository.getstudentdata(chart.regdno);
                if (student.Result.ElementAt(0).course_code == "PHD")
                {
                    if (student.Result.ElementAt(0).branch_code == "Economics")
                    {
                        chart.branch_name = "Humanities and Social Sciences";
                    }
                    else
                    {
                        chart.branch_name = student.Result.ElementAt(0).branch_code;
                    }
                }
                else
                {
                    chart.branch_name = student.Result.ElementAt(0).branch_code;
                }
                if (student.Result.ElementAt(0).course_code == "PHD")
                {
                    chart.examination = "Pre " + student.Result.ElementAt(0).course_code;
                }
                else
                {
                    chart.examination = student.Result.ElementAt(0).course_code;
                }
                chart.NAME = student.Result.ElementAt(0).NAME;
                chart.todaydate = DateTime.Now.ToString("dd-MMM-yyyy");
                chart.batch = student.Result.ElementAt(0).batch;
                chart.degree_code = student.Result.ElementAt(0).degree_code;
                var stddetails1 = iStudentRepository.Get_Student_Data(chart.regdno);
                if (chart.college_code != "GIMSR" && chart.college_code != "GIMSRC")
                {
                    if (Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2021 && chart.degree_code == "UG")
                    {

                        if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && chart.type1 == "R")
                        {
                            chart.instructn = "FinalYeardiv";
                        }
                        else
                        {
                            chart.instructn = "A21";
                        }
                    }
                }
                else if (chart.college_code == "GIMSR" || chart.college_code == "GIMSRC")
                {
                    if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && (chart.type1 == "R" || chart.type1 == "S"))//newly added this block
                    {
                        chart.instructn = "C21";
                        //chart.instructn = "FinalYeardiv";
                    }
                    else
                    {
                        chart.instructn = "C21";
                    }
                }
                else
                {
                    if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
                    {
                        chart.instructn = "FinalYeardiv";
                    }
                    else
                    {
                        chart.instructn = "B21";
                    }
                }
                if (chart.type1 != null)

                {
                    var stddetails = iStudentRepository.Get_Student_Data(chart.regdno);
                    var sem = chart.id1;
                    if (sem == "1")
                    {
                        chart.psntsem = "I";
                    }
                    else if (sem == "2")
                    {
                        chart.psntsem = "II";
                    }
                    else if (sem == "3")
                    {
                        chart.psntsem = "III";
                    }
                    else if (sem == "4")
                    {
                        chart.psntsem = "IV";
                    }
                    else if (sem == "5")
                    {
                        chart.psntsem = "V";
                    }
                    else if (sem == "6")
                    {
                        chart.psntsem = "VI";
                    }
                    else if (sem == "7")
                    {
                        chart.psntsem = "VII";
                    }
                    else if (sem == "8")
                    {
                        chart.psntsem = "VIII";
                    }
                    else if (sem == "9")
                    {
                        chart.psntsem = "IX";
                    }
                    else if (sem == "10")
                    {
                        chart.psntsem = "X";
                    }
                    else if (sem == "11")
                    {
                        chart.psntsem = "XI";
                    }
                    else if (sem == "12")
                    {
                        chart.psntsem = "XII";
                    }
                    else if (sem == "13")
                    {
                        chart.psntsem = "XIII";
                    }
                    else if (sem == "14")
                    {
                        chart.psntsem = "XIV";

                    }
                    else if (sem == "15")
                    {
                        chart.psntsem = "XV";
                    }
                    else if (sem == "91")
                    {
                        chart.psntsem = "SLM";
                    }
                    else if (sem == "92")
                    {
                        chart.psntsem = "Summer term";
                    }
                    else if (sem == "94")
                    {
                        chart.psntsem = "Re Registartion";
                    }

                    else
                    {
                        chart.psntsem = sem;
                    }

                    if (chart.type1 == "SD" || chart.type1 == "B")
                    {
                        admission.note = chart;
                        admission.notes = await iStudentRepository.getstudentspecialhalltickettheory52(chart);
                        admission.studentdetails = await iStudentRepository.getstudentspecialhallticketpractical52(chart);
                        admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(chart.regdno);
                    }
                    else
                    {
                        admission.note = chart;
                        admission.notes = await iStudentRepository.getstudenthalltickettheory52(chart);
                        admission.studentdetails = await iStudentRepository.getstudenthallticketpractical52(chart);
                        admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(chart.regdno);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Errorpage.aspx");
            }
            return (admission);
        }
        [HttpPost]
        public async Task<hallticketsummary> PHD_HALL_TICKET(Hallticket chart)
        {
            hallticketsummary admission = new hallticketsummary();
            IEnumerable<Hallticket> profiledata = null;
            IEnumerable<Hallticket> details = null;
            try
            {

                chart.regdno = Session["uid"].ToString();
                chart.type1 = Session["type1"].ToString();
                chart.type2 = Session["type2"].ToString();
                chart.course_code = Convert.ToString(Session["course_code"]);
                chart.branch_code = Convert.ToString(Session["branch_code"]);
                chart.degree_code = Convert.ToString(Session["degree_code"]);
                var student = iStudentRepository.getstudentdata(chart.regdno);
                if (student.Result.ElementAt(0).course_code == "PHD")
                {
                    if (student.Result.ElementAt(0).branch_code == "Economics")
                    {
                        chart.branch_name = "Humanities and Social Sciences";
                    }
                    else
                    {
                        chart.branch_name = student.Result.ElementAt(0).branch_code;
                    }
                }
                else
                {
                    chart.branch_name = student.Result.ElementAt(0).branch_code;
                }
                if (student.Result.ElementAt(0).course_code == "PHD")
                {
                    chart.examination = "Pre " + student.Result.ElementAt(0).course_code;
                }
                else
                {
                    chart.examination = student.Result.ElementAt(0).course_code;
                }
                chart.NAME = student.Result.ElementAt(0).NAME;
                chart.todaydate = DateTime.Now.ToString("dd-MMM-yyyy");
                chart.batch = student.Result.ElementAt(0).batch;
                var stddetails1 = iStudentRepository.Get_Student_Data(chart.regdno);

                if (Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2021 && student.Result.ElementAt(0).degree_code == "UG")
                {

                    if (stddetails1.Result.ElementAt(0).regdno.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
                    {
                        chart.instructn = "FinalYeardiv";
                    }
                    else
                    {
                        chart.instructn = "A21";
                    }
                }
                else
                {
                    if (stddetails1.Result.ElementAt(0).regdno.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
                    {
                        chart.instructn = "FinalYeardiv";
                    }
                    else
                    {
                        chart.instructn = "B21";
                    }
                }
                if (chart.type1 != null)

                {
                    var stddetails = iStudentRepository.Get_Hall_Ticket_Details(chart.regdno, chart.type1, chart.CURRSEM);
                    if (stddetails.Result.Count() > 0)
                    {
                        chart.regdno = stddetails.Result.ElementAt(0).regdno;
                        chart.branch_name = stddetails.Result.ElementAt(0).branch_name;
                        chart.examination = stddetails.Result.ElementAt(0).COURSE_NAME;
                        chart.COURSE_NAME = stddetails.Result.ElementAt(0).COURSE_NAME;
                        chart.NAME = stddetails.Result.ElementAt(0).NAME;
                        chart.batch = stddetails.Result.ElementAt(0).batch;
                        chart.Exam_Venue = stddetails.Result.ElementAt(0).Exam_Venue;
                        chart.MONTH = stddetails.Result.ElementAt(0).MONTH;
                        //chart.id1 = stddetails.Result.ElementAt(0).SEMESTER;
                        //var sem = chart.id1;
                        chart.id1 = Convert.ToString(Session["finalsem"]);
                        var sem = Convert.ToString(Session["finalsem"]);
                        if (sem == "1")
                        {
                            chart.psntsem = "I";
                        }
                        else if (sem == "2")
                        {
                            chart.psntsem = "II";
                        }
                        else if (sem == "3")
                        {
                            chart.psntsem = "III";
                        }
                        else if (sem == "4")
                        {
                            chart.psntsem = "IV";
                        }
                        else if (sem == "5")
                        {
                            chart.psntsem = "V";
                        }
                        else if (sem == "6")
                        {
                            chart.psntsem = "VI";
                        }
                        else if (sem == "7")
                        {
                            chart.psntsem = "VII";
                        }
                        else if (sem == "8")
                        {
                            chart.psntsem = "VIII";
                        }
                        else if (sem == "9")
                        {
                            chart.psntsem = "IX";
                        }
                        else if (sem == "10")
                        {
                            chart.psntsem = "X";
                        }
                        else if (sem == "11")
                        {
                            chart.psntsem = "XI";
                        }
                        else if (sem == "12")
                        {
                            chart.psntsem = "XII";
                        }
                        else if (sem == "13")
                        {
                            chart.psntsem = "XIII";
                        }
                        else if (sem == "14")
                        {
                            chart.psntsem = "XIV";

                        }
                        else if (sem == "15")
                        {
                            chart.psntsem = "XV";
                        }
                        else
                        {
                        }


                        admission.note = chart;
                        admission.notes = await iStudentRepository.getstudentPHDhalltickettheory(chart);
                        admission.studentdetails = await iStudentRepository.getstudentPHDhallticketpractical(chart);
                        admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(chart.regdno);

                    }

                    else
                    {
                        admission.note = chart;
                        admission.notes = await iStudentRepository.getstudentPHDhalltickettheory(chart);
                        admission.studentdetails = await iStudentRepository.getstudentPHDhallticketpractical(chart);
                        admission.studentdetailslist = await iStudentRepository.getstudentcoursedata(chart.regdno);


                    }

                }


            }
            catch (Exception ex)
            {
                Response.Redirect("Errorpage.aspx");
            }
            return (admission);
        }

        [HttpGet]
        public async Task<JsonResult> gethalleligiblesubjectlist(string regdno, string type, string semester, string tick)
        {
            hallticketsummary admission = new hallticketsummary();
            IEnumerable<Hallticket> chart = null;
            try
            {
                string REGDNO = Session["uid"].ToString();
                admission.eligiblesub = await iStudentRepository.gethalleligiblesubjects(regdno, type, semester);
                admission.noteligiblesub = await iStudentRepository.gethallnoteligiblesubjects(regdno, type, semester);
                admission.notes = await iStudentRepository.gethallticketdata52(regdno);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(admission, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //public async Task<ActionResult> Loginuser(FormCollection collection)
        public async Task<ActionResult> Loginuservalidations(string userid, string password)
        {
            Userlogin login = new Userlogin();
            Session["user_type"] = "";
            //string[] user_id = collection.GetValues("user_id");
            //string[] password = collection.GetValues("password");

            Userlogin cm = await passwordrepository.getEmplogin(userid, password);

            if (cm != null)
            {
                Session["EMPID"] = cm.EMPID.ToString();
                Session["EMP_NAME"] = cm.EMP_NAME.ToString();
                Session["DEPT_CODE"] = cm.dept_code.ToString();

                return Json(cm, JsonRequestBehavior.AllowGet);

            }
            else
            {
                Userlogin ul = await passwordrepository.getEmplogindetails(userid);
                if (ul != null)
                {
                    if (ul == null)
                    {
                        login.msg = "please enter valid Userid";
                    }
                    else if (ul.password != password)
                    {
                        login.msg = "please enter valid Password";
                    }
                    else
                    {
                        login.msg = "please enter valid details";
                    }
                }

                else
                {
                    login.msg = "please enter valid Userid";
                }


                return Json(login, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> Roomdetails(string regdno)

        {
            DateTime dateTime = DateTime.UtcNow.Date;



            IEnumerable<Students> data = await iStudentRepository.getexamtimetablebydate(regdno, dateTime.ToString("yyyy/MM/dd"));
            ViewBag.regdnos = regdno;

            if (data.Count() > 0)
            {
            }
            else
            {

                data = await iStudentRepository.GetDetailsstudent(regdno);
            }
            return View("Roomdetails", data);

            //return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}