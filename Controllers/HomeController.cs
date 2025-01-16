using GHMS.Helpers;
using Gstudent.CustomFilter;
using Gstudent.Models;
using Gstudent.Repositories.Interface;
using Gstudent.ViewModels;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ZXing;
using static Gstudent.FilterConfig;
using static Gstudent.ViewModels.Studentrecords;

namespace Gstudent.Controllers
{
    // Home
    public class HomeController : Controller
    {
        private readonly IStudentRepository iStudentRepository;

        public HomeController(IStudentRepository _IStudentRepository)
        {
            this.iStudentRepository = _IStudentRepository;


        }
        //public async Task<ActionResult> Index()
        //{
        //   // Session["uid"] = "A19BC0155009";
        //    string userid = Session["uid"].ToString();
        //    string userid1 = Encrypt(userid, true);
        //    Session["EncryUserID"] = userid1;
        //    return View();
        //}

        public async Task<ActionResult> Index()
        {
            StudentProfile profile = new StudentProfile();
            string userid = Convert.ToString(Session["uid"]);
            string userid1 = Encrypt(userid, true);
            Session["EncryUserID"] = userid1.Trim();

                   
            if (Session["status"].ToString() != "S" )
            {
                return await Gethalltickets();
            }
            
            else
            {
                if (Session["college_code"].ToString() == "CDL")
                {

                    try
                    {
                        IEnumerable<StudentProfile> profiledata = await iStudentRepository.GetCDLProfileDetails(userid);

                        profile = profiledata.OrderByDescending(X => X.dt_time).FirstOrDefault();
                        if (profile == null)
                        {
                            profile = new StudentProfile
                            {
                                regdno = userid,

                                dt_time = DateTime.Now
                            };
                        }
                    }
                    catch (HttpRequestException)
                    {
                        RedirectToAction("Index", "../loginpage");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return PartialView("Index", profile);
                }


                else
                {
                    try
                    {
                        IEnumerable<StudentProfile> profiledata = await iStudentRepository.GetDetails(userid);
                        IEnumerable<StudentProfile> statesList = await iStudentRepository.getstates();

                        ViewBag.statesList = statesList.Select(testgroup => new SelectListItem
                        {
                            Text = testgroup.STATE.ToString(),
                            Value = testgroup.STATE.ToString()
                        });
                        profile = profiledata.OrderByDescending(X => X.dt_time).FirstOrDefault();
                        if (profile == null)
                        {
                            profile = new StudentProfile
                            {
                                regdno = userid,

                                dt_time = DateTime.Now
                            };
                        }
                    }
                    catch (HttpRequestException)
                    {
                        RedirectToAction("Index", "../loginpage");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return PartialView("Index", profile);
                }
            }
        }



        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file
            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace(' ', '+'));

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }



        public ActionResult Privacy()
        {
            // Session["uid"] = "121810310050";
            Session["CLASS"] = "";
            Session["campus_code"] = "";
            Session["college_code"] = "";
            Session["dept_code"] = "";
            return View();
        }

        public async Task<ActionResult> Logout()
        {

            int i = await iStudentRepository.getlogout(Convert.ToString(Session["uid"]));
            Session.Clear();
            return Redirect("https://login.gitam.edu/Login.aspx");

        }

        public async Task<ActionResult> GetAttendance()
        {
            Attendancesummary summary = new Attendancesummary();

            string college_code = Session["college_code"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string batch = Session["batch"].ToString();
            string[] batch1 = new string[1000];
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            Session["batchattd"] = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
            string batchattd = Session["batchattd"].ToString();
            string user_id = Session["uid"].ToString();

            string campus = Session["campus_code"].ToString();
            string course_code = Session["course_code"].ToString();
            summary.flag = await iStudentRepository.Getflag(user_id, table_name, college_code, cur_sem, batchattd, campus, course_code);
            if (summary.flag.Count() > 0)
            {
                Session["Flag"] = "N";
            }
            else
            {
                Session["Flag"] = "";
            }
            string flag = Session["Flag"].ToString();
            if (flag == "N")
            {

                RedirectToAction("Index", "../loginpage");
            }
            if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
            {
                summary.notes = await iStudentRepository.Getattendance_semster_gimsr(user_id, table_name, college_code, cur_sem, batchattd);
            }
            else
            {
                summary.notes = await iStudentRepository.Getattendance_semster(user_id, table_name, college_code, cur_sem, batchattd);
            }
            //if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
            //{
            //    return PartialView("Attendance", summary);
            //}
            //return PartialView("Attendancesummerterm", summary);
            return PartialView("Attendance", summary);

        }


        public async Task<ActionResult> GetEventattendnace()
        {
            IEnumerable<EventAttendance> summary = null;


            string college_code = Session["college_code"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string batch = Session["batch"].ToString();
            string[] batch1 = new string[1000];
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            Session["batchattd"] = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
            string batchattd = Session["batchattd"].ToString();
            string user_id = Session["uid"].ToString();

            string campus = Session["campus_code"].ToString();
            string course_code = Session["course_code"].ToString();

            summary = await iStudentRepository.Getevent_attendance(user_id, cur_sem);
            return PartialView("EventAttendance", summary);
        }

        [HttpGet]
        public async Task<ActionResult> Newgleanencry()
        {
            IEnumerable<EventAttendance> summary = null;

            var result = await iStudentRepository.InsertGlearnhistory(Session["uid"].ToString(), Session["campus_code"].ToString(), Session["college_code"].ToString());
            if (result!=0)
            {
              
                string newid = Session["uid"].ToString() + "#" + "S" + "#" + result + "#" + System.DateTime.Now.ToString("ddMMMyyyyffff") + "#" + "G-Learn";
                Session["GLearnnew"] = Gstudent.Models.CryptorEngine.Encrypt(newid, true);
                var sasassa = Gstudent.Models.CryptorEngine.Encrypt(newid, true);

                var redirectUrl = $"https://glearn.gitam.edu/signon/index?val={Session["GLearnnew"].ToString()}";
                //return Redirect(redirectUrl); 


            }
            return PartialView("Newgleanencry");
        }
        
        [HttpGet]

        public async Task<ActionResult> GetAttendancedata(string category, string date)
        {
            Attendancesummary summary = new Attendancesummary();
            string college_code = Session["college_code"].ToString();
            string campus = Session["campus_code"].ToString();
            string classs = Session["CLASS"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string batchattd = Session["batchattd"].ToString();
            string user_id = Session["uid"].ToString();
            if (campus == "VSP" || campus == "HYD")
            {

                if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
                {
                    summary.notes = await iStudentRepository.Getattendance_semster_byselection_gimsr(user_id, table_name, college_code, cur_sem, batchattd, category, date);
                }
                else
                {
                    //if (classs == "I Yr")
                    //{
                    summary.notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, category, date);
                    //}
                    //else
                    //{
                    //    List<Attendance> list = new List<Attendance>();
                    //    summary.notes = list;
                    //}
                }
            }
            else
            {
                summary.notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, category, date);

            }
            summary.note = new Attendance();
            summary.note.category = category;
            return PartialView("Attendancedata", summary);
        }



        [HttpGet]

        public async Task<ActionResult> GetAttendancedatachart(string category, string date)
        {
            Attendancesummary summary = new Attendancesummary();
            string college_code = Session["college_code"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string batchattd = Session["batchattd"].ToString();
            string user_id = Session["uid"].ToString();
            if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
            {
                summary.notes = await iStudentRepository.Getattendance_semster_byselection_gimsr(user_id, table_name, college_code, cur_sem, batchattd, category, date);
            }
            else
            {
                var notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, category, date);
                summary.notes = notes.GroupBy(x => x.subjectcode).Select(x => x.First()).ToList();
            }
            summary.note = new Attendance();
            summary.note.category = category;
            return PartialView("Attendancedata", summary);
        }



        [HttpGet]
        public async Task<ActionResult> Getcourse_structure()
        {
            Attendancesummary summary = new Attendancesummary();
            // Session["batch"] = "2019-2020";
            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            //Session["college_code"] = "GIT";
            //Session["branch_code"] = "CSE";
            //Session["campus_code"] = "VSP";
            string college_code = Session["college_code"].ToString();
            string branch_code = Session["branch_code"].ToString();
            string campus_code = Session["campus_code"].ToString();

            try
            {
                summary.note = new Attendance();
                IEnumerable<Attendance> semslist = await iStudentRepository.Getsemsterlist(college_code, campus_code, branch_code, batch2);
                ViewBag.Semlist = semslist.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.SEMESTER.ToString(),
                    Value = testgroup.SEMESTER.ToString()
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Course_structure", summary);
        }

        [HttpGet]

        public async Task<JsonResult> GetSemesterdata(string semester)
        {

            Attendancesummary summary = new Attendancesummary();
            IEnumerable<Attendance> semdata = null;
            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string college_code = Session["college_code"].ToString();
            string branch_code = Session["branch_code"].ToString();
            string campus_code = Session["campus_code"].ToString();
            try
            {
                semdata = await iStudentRepository.GetSemesterdata(college_code, campus_code, branch_code, batch2, semester);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(semdata, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult Getscholarship()
        {
            return PartialView("StudentScholarship");
        }
        [HttpGet]
        public async Task<JsonResult> Getdetails(string Applicationo, string category)
        {

            //Session["REGDNO"] = "1210116117";
            string regdno = Session["REGDNO"].ToString();
            try
            {
                List<Scholarship> details = await iStudentRepository.Getscholarshipdetails(Applicationo);
                List<Scholarship> details1 = await iStudentRepository.Getscholarshipdetailsbycategory(category, regdno, Applicationo);
                var model = new studentdatamodel
                {
                    studentData = details1,
                    othersdata = details
                };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]

        public ActionResult UploadFile(HttpPostedFileBase uploadForm, string category, string applicationno)
        {
            string msg = "";
            string targetpath = "";
            if (category == "S")
            {
                targetpath = "Ecounselling_certificates\\SIBLING";
            }
            else if (category == "A")
            {
                targetpath = "Ecounselling_certificates\\ALUMNI";
            }
            else
            {

            }
            String PATH = "C:\\CATS_PROJECTS\\" + targetpath + "\\";
            String rno = null;
            try
            {


                if (uploadForm.FileName == "")
                {
                    msg = "File not found";
                }
                else
                {
                    String ext = System.IO.Path.GetExtension(uploadForm.FileName);
                    rno = applicationno;
                    if ((ext.Equals(".pdf")) || (ext.Equals(".PDF")) || (ext.Equals(".jpg")) || (ext.Equals(".JPG")))
                    {
                        int fileSize = uploadForm.ContentLength;
                        uploadForm.SaveAs(PATH + rno + ".pdf");
                        msg = "File upload was done";
                    }
                    else
                    {
                        msg = "File upload Failed";
                    }
                }
            }
            catch (Exception exp)
            {
                Response.Write(exp.Message);
            }
            return Json(msg);

        }


        [HttpPost]

        public async Task<ActionResult> CreateScholarship(HttpPostedFileBase uploadForm, Scholarship summary)
        {
            string msg = "";
            try
            {
                IEnumerable<Scholarship> data = await iStudentRepository.Getscholarcount(summary.ApplicationNumber);
                if (data.Count() > 0)
                {
                    var result = await iStudentRepository.Updatescholarship(summary);
                    if (result.flag == 1)
                    {
                        msg = "Successfully submitted";
                    }
                    else
                    {
                        msg = "Oops!!! something went wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg);
        }
        [HttpGet]

        public ActionResult Passwordchange()
        {
            //Session["user_id"] = "1210316959";
            return PartialView("Passwordchange");
        }


        [HttpPost]

        public async Task<JsonResult> getoldPassword(string Id, string pass)
        {
            dynamic showMessageString = string.Empty;
            try
            {

                Students cm = await iStudentRepository.getoldPassword(Id, Session["user_type"].ToString());
                if (cm != null)
                {
                    string pw = "";
                    if (Session["user_type"].ToString() == "P")
                    {
                        pw = cm.PARENT_PASSWORD;
                    }
                    else
                    {
                        pw = cm.NEW_PASSWORD;
                    }
                    string res = "";
                    if (CryptSharp.Crypter.CheckPassword(pass, pw))
                    {

                        res = "Match";
                    }
                    else
                    {
                        res = "Mismatch";
                    }

                    showMessageString = new
                    {
                        param1 = 200,
                        param2 = res

                    };
                }


                return Json(showMessageString, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                //  throw ex;
                showMessageString = new
                {
                    param1 = 250,
                    param2 = "Failed",
                };
                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ChangeUserPassword(FormCollection collection)
        {
            dynamic showMessageString = string.Empty;
            try
            {
                string NPW = collection["Newpassword"];
                string CPW = collection["Confirmpassword"];
                string user_id = Convert.ToString(Session["REGDNO"]);
                string user_name = Convert.ToString(Session["displayName"]);
                if (NPW == CPW)
                {
                    var password = CryptSharp.Crypter.Sha512.Crypt(NPW);
                    string res = iStudentRepository.ChangeUserPasswordAsync(password, user_id, @Session["user_type"].ToString(), NPW);

                    if (res == "1")
                    {
                        showMessageString = new
                        {
                            param1 = 200,
                            param2 = "Password reset successfull",
                        };
                        return Json(showMessageString, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        showMessageString = new
                        {
                            param1 = 100,
                            param2 = "Password reset failed",
                        };
                        return Json(showMessageString, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    showMessageString = new
                    {
                        param1 = 100,
                        param2 = "New password is not matching with confirm password",
                    };
                    return Json(showMessageString, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

                showMessageString = new
                {
                    param1 = 300,
                    param2 = "exception occured",
                };
                Response.Write(ex);
                return Json(showMessageString, JsonRequestBehavior.AllowGet);
            }

        }

        public async Task<PartialViewResult> GetStudentData()
        {

            Response.AddHeader("Access-Control-Allow-Origin", "https://login.gitam.edu");

            Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");
            Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");

            StudentProfile profile = new StudentProfile();



            string userid = Convert.ToString(Session["uid"]);
            if (userid == "")
            {
                RedirectToAction("https://login.gitam.edu");
                throw new Exception();
            }

            try
            {
                IEnumerable<StudentProfile> profiledata = await iStudentRepository.GetDetails(userid);
                IEnumerable<StudentProfile> statesList = await iStudentRepository.getstates();

                ViewBag.statesList = statesList.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.STATE.ToString(),
                    Value = testgroup.STATE.ToString()
                });
                profile = profiledata.OrderByDescending(X => X.dt_time).FirstOrDefault();
                if (profile == null)
                {
                    profile = new StudentProfile
                    {
                        regdno = userid,

                        dt_time = DateTime.Now
                    };
                }
            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Profile", profile);
        }

        public async Task<JsonResult> getstatesdynamically()
        {
            StudentProfile IPassessment = new StudentProfile();
            var id = "";
            var v = await iStudentRepository.getstates();
            return Json(v);

        }

        public async Task<ActionResult> Updateprofile(StudentProfile chart)
        {
            Studentrecordslist endo = new Studentrecordslist();
            try
            {
                //Session["uid"] = "1210315702";
                string userid = Convert.ToString(Session["uid"]);
                chart.dt_time = DateTime.Now;
                endo.record = await iStudentRepository.Updateprofileasync(chart);
                if (endo.record != null)
                {
                    endo.record.msg = "Successfully Updated";
                }
                else
                {
                    endo.record.msg = "Opps!!! something went wrong";
                }
            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception e)
            { throw e; }
            return Json(chart, JsonRequestBehavior.AllowGet);
        }




        public async Task<ActionResult> getUnderProcess()
        {
            IEnumerable<Students> data = null;
            try
            {
                data = await iStudentRepository.GetUnderProcess(Convert.ToString(Session["uid"]));
                data = data.Where(X => X.Permissiondate >= DateTime.Today);
                if (data.Count() > 0)
                {
                    foreach (var val in data)
                    {
                        if (val.fromtime != null)
                        {
                            decimal d = Convert.ToDecimal(val.fromtime);
                            string k = d.ToString("0.00");
                            val.fromtime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.fromtime = t1 + ":" + t2;
                        }
                        if (val.totime != null)
                        {
                            decimal d = Convert.ToDecimal(val.totime);
                            string k = d.ToString("0.00");
                            val.totime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.totime = t1 + ":" + t2;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> getApproved()
        {
            IEnumerable<Students> data = null;
            try
            {
                data = await iStudentRepository.GetApproved(Convert.ToString(Session["uid"]));
                data = data.Where(X => X.Permissiondate >= DateTime.Parse("01-june-2023")).ToList();
                if (data.Count() > 0)
                {
                    foreach (var val in data)
                    {
                        if (val.fromtime != null)
                        {
                            decimal d = Convert.ToDecimal(val.fromtime);
                            string k = d.ToString("0.00");
                            val.fromtime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.fromtime = t1 + ":" + t2;
                        }
                        if (val.totime != null)
                        {
                            decimal d = Convert.ToDecimal(val.totime);
                            string k = d.ToString("0.00");
                            val.totime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.totime = t1 + ":" + t2;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> getRejected()
        {
            IEnumerable<Students> data = null;
            try
            {
                data = await iStudentRepository.GetRejected(Convert.ToString(Session["uid"]));
                data = data.Where(X => X.Permissiondate >= DateTime.Parse("01-june-2023"));
                if (data.Count() > 0)
                {
                    foreach (var val in data)
                    {
                        if (val.fromtime != null)
                        {
                            decimal d = Convert.ToDecimal(val.fromtime);
                            string k = d.ToString("0.00");
                            val.fromtime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.fromtime = t1 + ":" + t2;
                        }
                        if (val.totime != null)
                        {
                            decimal d = Convert.ToDecimal(val.totime);
                            string k = d.ToString("0.00");
                            val.totime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.totime = t1 + ":" + t2;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> getHistory()
        {
            IEnumerable<Students> data = null;
            try
            {
                data = await iStudentRepository.GetHistory(Convert.ToString(Session["uid"]));
                data = data.Where(X => X.Permissiondate >= DateTime.Parse("01-june-2023"));
                if (data.Count() > 0)
                {
                    foreach (var val in data)
                    {
                        if (val.fromtime != null)
                        {
                            decimal d = Convert.ToDecimal(val.fromtime);
                            string k = d.ToString("0.00");
                            val.fromtime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.fromtime = t1 + ":" + t2;
                        }
                        if (val.totime != null)
                        {
                            decimal d = Convert.ToDecimal(val.totime);
                            string k = d.ToString("0.00");
                            val.totime = Convert.ToString(k);
                            string t1 = k.Split('.')[0];
                            string t2 = k.Split('.')[1];
                            val.totime = t1 + ":" + t2;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }




        public async Task<ActionResult> GetNoticeboard()
        {

            string college_code = Convert.ToString(Session["college_code"]);
            string campus_code = Convert.ToString(Session["campus_code"]);
            string dept_code = Convert.ToString(Session["dept_code"]);
            string CLASS = Convert.ToString(Session["CLASS"]);
            // string userid = "121910201030";
            List<Students> list = new List<Students>();
            try
            {
                list = await iStudentRepository.GetNoticeboarddata(college_code, campus_code, dept_code, CLASS);
                if (list.Count() > 0)
                {
                    list.ElementAt(0).flag = "0";
                }
                else
                {
                    Students student = new Students()
                    {
                        flag = "0"
                    };
                    list.Add(student);
                }
                IEnumerable<Students> days = await iStudentRepository.GetDistinctDates(college_code, campus_code, dept_code, CLASS);
                IEnumerable<Students> years = await iStudentRepository.GetDistinctYears(college_code, campus_code, dept_code, CLASS);


                ViewBag.list = days.Select(day => new List<Students>()
                {
                     new Students() { Month = day.Month,year = day.year},
                }).ToList();


                ViewBag.years = years.Select(year => new List<Students>()
                {
                     new Students() {year = year.year},
                }).ToList();

            }
            catch (Exception e)
            {
                throw e;
            }
            return PartialView("Noticeboard", list);

        }


        public async Task<ActionResult> getMoreNotices()
        {

            string college_code = Convert.ToString(Session["college_code"]);
            string campus_code = Convert.ToString(Session["campus_code"]);
            string dept_code = Convert.ToString(Session["dept_code"]);
            string CLASS = Convert.ToString(Session["CLASS"]);
            // string userid = "121910201030";

            List<Students> list = new List<Students>();
            IEnumerable<Students> list1 = null;
            try
            {
                list = await iStudentRepository.GetNoticeboarddataMore(college_code, campus_code, dept_code, CLASS);

                IEnumerable<Students> days = await iStudentRepository.GetDistinctDates(college_code, campus_code, dept_code, CLASS);
                IEnumerable<Students> years = await iStudentRepository.GetDistinctYears(college_code, campus_code, dept_code, CLASS);


                ViewBag.list = days.Select(day => new List<Students>()
                {
                     new Students() { Month = day.Month,year = day.year},
                }).ToList();


                ViewBag.years = years.Select(year => new List<Students>()
                {
                     new Students() {year = year.year},
                }).ToList();


            }
            catch (Exception e)
            {
                throw e;
            }
            return PartialView("NoticeBoardMore", list);

        }


        public async Task<ActionResult> GetPermissions()
        {

            permissions permodel = new permissions();

            if (Session["campus_code"].ToString() == "BLR" )
            {
                IEnumerable<Students> hostelcoordinators = await iStudentRepository.get_coordinator_dropdown(Session["campus_code"].ToString());
                ViewBag.hostelcoordinators = hostelcoordinators.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.emp_name.ToString(),
                    Value = testgroup.empid.ToString()

                });


            }
            else
            {
                permodel.warden = await iStudentRepository.get_coordinator(Session["campus_code"].ToString());
            }
            permodel.Hostlers = await iStudentRepository.gethostlerdetails(Session["uid"].ToString());

            return PartialView("permissions", permodel);

        }
        [HttpPost]
        //[HandleErrorAttribute]
        [HandleErrorAttribute]
        public async Task<ActionResult> createpermission(FormCollection collection)
        {

            string r = Session["campus_code"].ToString();
            try
            {
                string radio = collection.GetValues("radioBtn")[0];
                string fromtime = collection.GetValues("fromtimepicker")[0];
                Students hos = new Students();
                hos.regdno = collection.GetValues("regdno")[0];
                hos.AppliedDate = System.DateTime.Now;
                hos.Isapprove = "N";
                hos.CAMPUS = collection.GetValues("CAMPUS")[0];
                hos.COLLEGE_CODE = collection.GetValues("COLLEGE_CODE")[0];
                hos.dept_code = collection.GetValues("dept_code")[0];
                hos.biometric_id = collection.GetValues("biometric_id")[0];
                hos.GENDER = collection.GetValues("GENDER")[0];
                hos.AccountNo = collection.GetValues("regdno")[0];
                hos.Reason = collection.GetValues("Reason")[0];
                hos.Travelinginformation = collection.GetValues("travelinfo")[0];
                hos.Type = collection.GetValues("radioBtn")[0];
                hos.name = collection.GetValues("studentname")[0];
                hos.parent_mobile = collection.GetValues("parent_mobile")[0];
                hos.HostelCode = collection.GetValues("HOSTEL_BLOCK")[0];
                hos.hostelconame = collection.GetValues("hostelconame")[0];
                if (collection.GetValues("radioBtn")[0].Equals("V"))
                {
                    DateTime d = DateTime.Parse(collection.GetValues("fdatetimepicker")[0]);
                    string ss = d.ToString("HH:mm");
                    hos.Fromtime = Convert.ToDouble(ss.Replace(":", "."));
                    DateTime d1 = DateTime.Parse(collection.GetValues("Tdatetimepicker")[0]);
                    string ss1 = d1.ToString("HH:mm");
                    hos.Totime = Convert.ToDouble(ss1.Replace(":", "."));
                    hos.Fromdate = DateTime.Parse(collection.GetValues("FromdatePicker")[0]);
                    hos.Todate = DateTime.Parse(collection.GetValues("TodatePicker")[0]);
                    hos.Permissiondate = DateTime.Parse(collection.GetValues("FromdatePicker")[0]);
                }
                else if (collection.GetValues("radioBtn")[0].Equals("F"))
                {
                    DateTime d = DateTime.Parse(collection.GetValues("fdatetimepicker")[0]);
                    string ss = d.ToString("HH:mm");
                    hos.Fromtime = Convert.ToDouble(ss.Replace(":", "."));
                    DateTime d1 = DateTime.Parse(collection.GetValues("Tdatetimepicker")[0]);
                    string ss1 = d1.ToString("HH:mm");
                    hos.Totime = Convert.ToDouble(ss1.Replace(":", "."));
                    hos.Todate = DateTime.Parse(collection.GetValues("TodatePicker")[0]);
                    hos.Fromdate = DateTime.Parse(collection.GetValues("FromdatePicker")[0]);
                    hos.Permissiondate = DateTime.Parse(collection.GetValues("FromdatePicker")[0]);
                }
                else
                {

                    hos.Permissiondate = DateTime.Parse(collection.GetValues("datePicker")[0]);
                    DateTime d = DateTime.Parse(collection.GetValues("fromtimepicker")[0]);
                    string ss = d.ToString("HH:mm");
                    hos.Fromtime = Convert.ToDouble(ss.Replace(":", "."));
                    DateTime d1 = DateTime.Parse(collection.GetValues("Totimepicker")[0]);
                    string ss1 = d1.ToString("HH:mm");
                    hos.Totime = Convert.ToDouble(ss1.Replace(":", "."));
                    hos.Todate = DateTime.Parse(collection.GetValues("datePicker")[0]);
                    hos.Fromdate = DateTime.Parse(collection.GetValues("datePicker")[0]);
                }

                IEnumerable<Students> HostelPermissions = await iStudentRepository.gethostlerpermissions(hos.regdno);


                //if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && (we.Type == "F" || we.Type == "P") && we.Fromdate == hos.Fromdate).Count() > 0)
                //{
                //    return new HttpStatusCodeResult(404);
                //}
                if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && we.Type == "F" && we.Fromdate <= hos.Fromdate && hos.Todate < we.Todate && (we.Isapprove == "I")).Count() > 0)
                {
                    // && we.Fromdate >= hos.Fromdate && we.Todate <= hos.Todate

                    //return "You had applied a leave in between dates !";
                    return new HttpStatusCodeResult(401);
                }

                else if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && we.Type == "V" && we.Fromdate <= hos.Fromdate && hos.Todate < we.Todate && we.Isapprove == "I").Count() > 0)
                {
                    // && we.Fromdate >= hos.Fromdate && we.Todate <= hos.Todate

                    //return "You had applied a leave in between dates !";
                    return new HttpStatusCodeResult(403);
                }
                else if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && we.Type == "P" && we.Type == hos.Type && Convert.ToDateTime(we.Fromdate).ToString("dd-MMM-yyyy") == Convert.ToDateTime(hos.Fromdate).ToString("dd-MMM-yyyy") && we.Isapprove != "I").Count() > 0)
                {
                    // && we.Fromdate >= hos.Fromdate && we.Todate <= hos.Todate

                    //return "You had applied a leave in between dates !";
                    return new HttpStatusCodeResult(402);
                }
                else if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && we.Permissiondate == hos.Permissiondate && we.Type == "P" && we.Isapprove != "I").Count() > 0)
                {   //return "You had applied a permission in between dates and time !";
                    return new HttpStatusCodeResult(402);
                }
                else if (HostelPermissions.Where(we => we.regdno.Equals(hos.regdno) && we.Permissiondate == hos.Permissiondate && we.Type == "V").Count() > 0)
                {
                    // return "You had applied a permission in between dates and time !";
                    return new HttpStatusCodeResult(403);
                }
                else
                {

                    Students s = await iStudentRepository.createhostelpermision(hos);
                    return new HttpStatusCodeResult(200);
                }

            }


            catch (Exception e)
            {
                RedirectToAction("Index", "../loginpage");
            }
            return PartialView();
        }



        public async Task<PartialViewResult> GetCDLProfileupdate()
        {
            StudentProfile profile = new StudentProfile();


            // Session["uid"] = "A19BC0155009";
            string userid = Convert.ToString(Session["uid"]);

            try
            {
                IEnumerable<StudentProfile> profiledata = await iStudentRepository.GetCDLProfileDetails(userid);

                profile = profiledata.OrderByDescending(X => X.dt_time).FirstOrDefault();
                if (profile == null)
                {
                    profile = new StudentProfile
                    {
                        regdno = userid,

                        dt_time = DateTime.Now
                    };
                }
            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("CDLProfile", profile);
        }


        //[HttpPost]
        //public async Task<ActionResult> UpdateCDLProfile(StudentProfile summary)
        //{
        //    string msg = "";
        //    try
        //    {
        //        IEnumerable<StudentProfile> data = await iStudentRepository.GetCDLProfileDetails(Convert.ToString(Session["uid"]));
        //        if (data.Count() > 0)
        //        {
        //            summary.regdno = Convert.ToString(Session["uid"]);
        //            var result = await iStudentRepository.UpdateCDLProfile(summary);
        //            if (result.msg == "1")
        //            {
        //                msg = "Successfully submitted";
        //            }
        //            else
        //            {
        //                msg = "Oops!!! something went wrong";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(msg, JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]

        public async Task<ActionResult> UpdateCDLProfile(string txtdrno, string txtarea, string txtcity, string txtstate, string txtpin, string txtmobileno, string txtteleno, string txtemail)
        {
            string msg = "";
            StudentProfile summary = new StudentProfile()
            {
                TEMP_DRNO = txtdrno,
                TEMP_AREA = txtarea,
                TEMP_CITY = txtcity,
                TEMP_STATE = txtstate,
                TEMP_PIN = txtpin,
                TEMP_MOBILE = txtmobileno,
                TEMP_TELEPHONE = txtteleno,
                TEMP_EMAIL = txtemail
            };
            try
            {
                IEnumerable<StudentProfile> data = await iStudentRepository.GetCDLProfileDetails(Convert.ToString(Session["uid"]));
                if (data.Count() > 0)
                {
                    summary.regdno = Convert.ToString(Session["uid"]);
                    var result = await iStudentRepository.UpdateCDLProfile(summary);
                    if (result.msg == "1")
                    {
                        msg = "Successfully submitted";
                    }
                    else
                    {
                        msg = "Oops!!! something went wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }



        public async Task<ActionResult> GetHostelbiometric()
        {

            // Hostelbiometric permodel = new Hostelbiometric();
            //Session["campus_code"] = "VSP";
            //Session["uid"] = "VU22PHTY0100011";
            //Session["Curr_sem"] = "2";
            IEnumerable<Hostelbiometric> biometric = await iStudentRepository.gethostelbiometricsmsdetails(Session["uid"].ToString(), Session["Curr_sem"].ToString());


            return PartialView("Hostelbiometricsms", biometric);

        }

        [HttpGet]

        public async Task<ActionResult> CDL_academic_track()
        {
            CDL_academictrack note = new CDL_academictrack();
            // Session["uid"] = "C13EC1630001";
            string userid = Session["uid"].ToString();
            IEnumerable<CDL_academictrack> notes = await iStudentRepository.CDL_studentdetails(userid);
            note = notes.FirstOrDefault();
            if (note == null)
            {
                note = new CDL_academictrack();

            }
            return PartialView("CDL_Academictrack", note);
        }
        [HttpGet]

        public async Task<ActionResult> getfeedetails_cdl()
        {
            IEnumerable<CDL_academictrack> feedetails = null;
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                feedetails = await iStudentRepository.getfeedetails(userid);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(feedetails, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public async Task<ActionResult> getmaterial_dispatch_cdl()
        {
            IEnumerable<CDL_academictrack> materials = null;
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                materials = await iStudentRepository.getmaterial_dispatch_cdl(userid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(materials, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public async Task<ActionResult> Getassginments_cdl()
        {
            IEnumerable<CDL_academictrack> assignments = null;
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                assignments = await iStudentRepository.getassginments_cdl(userid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(assignments, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public async Task<ActionResult> Getresults_cdl()
        {
            IEnumerable<CDL_academictrack> results = null;
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                results = await iStudentRepository.getresults_cdl(userid);
                IEnumerable<CDL_academictrack> cgpa = await iStudentRepository.Getcgpa(userid);
                var model = new studentdatamodel
                {
                    studentData = results,
                    othersdata = cgpa
                };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]

        public async Task<ActionResult> Getcertificates_cdl()
        {
            IEnumerable<CDL_academictrack> certificates = null;
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                certificates = await iStudentRepository.getcertificates_cdl(userid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(certificates, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public async Task<ActionResult> Student_id_card()
        {
            CDL_academictrack note = new CDL_academictrack();
            // Session["uid"] = "C13EC1630001";
            string userid = Session["uid"].ToString();
            IEnumerable<CDL_academictrack> notes = await iStudentRepository.CDL_studentdetails(userid);
            note = notes.FirstOrDefault();
            if (note == null)
            {
                note = new CDL_academictrack();

            }

            return PartialView("Student_id_card", note);
        }




        [HttpGet]

        public async Task<PartialViewResult> GetAcademictrack()
        {
            trackrecords IPassessment = new trackrecords();


            //Session["uid"] = "BU21BTSC0300078";
            string userid = Convert.ToString(Session["uid"]);

            try
            {
                IEnumerable<studenttrack> semslist = await iStudentRepository.getsemesterasync(userid);
                // IEnumerable<studenttrack> semslist = await iStudentRepository.getacademicsemesterasync(userid);

                //ViewBag.semslist = semslist.Select(testgroup => new SelectListItem
                //{
                //    Text = testgroup.SEMESTER_DISPLAY.ToString(),
                //    Value = testgroup.SEMESTER.ToString()
                //}); 

                ViewBag.semslist = semslist.Select(testgroup => new SelectListItem
                {
                    Text = "Semester " + testgroup.SEMESTER.ToString(),
                    Value = testgroup.SEMESTER.ToString()
                });

            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Academictrack", IPassessment);
        }
        [HttpGet]

        public async Task<JsonResult> getcourse(string semester, string userid)
        {

            trackrecords endo = new trackrecords();
            userid = Convert.ToString(Session["uid"]);
            string coursecode = Convert.ToString(Session["course_code"]);
            string BATCH = Convert.ToString(Session["BATCH"]);
            string currentsem = Convert.ToString(Session["curr_sem"]);
            try
            {
                endo.lists = await iStudentRepository.getcourselist(semester, userid, coursecode, BATCH, currentsem);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(endo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public async Task<JsonResult> getsubjectslist(string semester)
        {

            //Session["REGDNO"] = "1210116117";
            string userid = Session["uid"].ToString();
            try
            {
                List<studenttrack> details = await iStudentRepository.getsubjects(semester, userid);
                List<studenttrack> details1 = await iStudentRepository.getperformance(semester, userid);
                var model = new trackrecords
                {
                    performance = details1,
                    lists = details
                };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        //[HttpGet]

        //public async Task<PartialViewResult> Gettimetable()
        //{
        //    timetablesummary IPassessment = new timetablesummary();

        //    string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
        //    string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
        //    string SEMESTER = Convert.ToString(Session["Curr_sem"]);
        //    string SECTION = Convert.ToString(Session["section"]);
        //    string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
        //    string userid = Convert.ToString(Session["uid"]);
        //    string COURSE_CODE = Convert.ToString(Session["course_code"]);
        //    //Session["batch"] = "2019";


        //    string[] batch1 = new string[1000];
        //    string batch = Session["batch"].ToString();
        //    batch1 = batch.Split('-');
        //    string batch2 = batch1[0];
        //    string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);

        //    //tring BATCH =  (batch2 + '-' +( batch2 + 1));
        //    IEnumerable<Timetable> semslist = null;


        //    try
        //    {

        //        IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE);


        //        semslist = await iStudentRepository.gettimetabledetails(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);

        //        ViewBag.days = new List<string>()
        //        {
        //            "Monday","Tuesday","Wednesday","Thursday","Friday"
        //        };
        //        ViewBag.list = sessionslist.Select(list => new List<Timetable>()
        //        {
        //             new Timetable() { timeslots = list.timeslots},
        //        }).ToList();

        //    }
        //    catch (HttpRequestException)
        //    {
        //        RedirectToAction("Index", "../loginpage");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return PartialView("Timetable", semslist);
        //}


        [HttpGet]

        public async Task<PartialViewResult> Gettimetable()
        {
            timetablesummarymain IPassessment = new timetablesummarymain();
            // string PREFERENCE_COURSE_5 = Session["PREFERENCE_COURSE_5"].ToString();
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Session["Curr_sem"]);
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);
            string DEGREE_CODE = Convert.ToString(Session["degree_code"]);
            //Session["batch"] = "2019";


            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);

            //tring BATCH =  (batch2 + '-' +( batch2 + 1));
            IEnumerable<Timetable> semslist = null;
            IEnumerable<Timetable> sessionslist = null;

            try
            {


                /* if (PREFERENCE_COURSE_5.ToUpper() == "FSTCSE2023" || PREFERENCE_COURSE_5.ToUpper() == "FSTNONCSE2023")
                 {
                     sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, PREFERENCE_COURSE_5, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE);
                     IPassessment.semlist = await iStudentRepository.gettimetabledetails(Session["REGDNO"].ToString(), COLLEGE_CODE, PREFERENCE_COURSE_5, SEMESTER, SECTION, CAMPUS_CODE, BATCH);
                     IPassessment.facultylists = await iStudentRepository.getFacultyDetailsTimetable_main(Session["REGDNO"].ToString(), SEMESTER,PREFERENCE_COURSE_5);


                 }
                 else if (!(BRANCH_CODE.ToUpper() == "FSTCSE2023" || BRANCH_CODE.ToUpper() == "FSTNONCSE2023") && BATCH.Split('-')[0].Equals("2023"))

                     {
                     sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE);
                     IPassessment.semlist = await iStudentRepository.gettimetabledetails(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);
                     IPassessment.facultylists = await iStudentRepository.getFacultyDetailsTimetable_main(Session["REGDNO"].ToString(), SEMESTER, BRANCH_CODE);


                 }
                 else
                 {
                     sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE);
                     IPassessment.semlist = await iStudentRepository.gettimetabledetails(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);

                     IPassessment.facultylists = await iStudentRepository.getFacultyDetailsTimetable_main(Session["REGDNO"].ToString(), SEMESTER, BRANCH_CODE);


                 }*/

                sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, DEGREE_CODE);
                IPassessment.semlist = await iStudentRepository.gettimetabledetails(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, Session["course_code"].ToString(), DEGREE_CODE);
                IPassessment.facultylists = await iStudentRepository.getFacultyDetailsTimetable_main(Session["REGDNO"].ToString(), SEMESTER, BRANCH_CODE, Session["course_code"].ToString(), Session["college_code"].ToString(), DEGREE_CODE,Session["section"].ToString());

                ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday"
                };
                ViewBag.list = sessionslist.OrderBy(x => x.timeslots).Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();

            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Timetable", IPassessment);
        }


        public ActionResult GetAccrediations()
        {
            return PartialView("Accrediations");
        }


        [HttpGet]

        public async Task<PartialViewResult> Gethostelfeedbackdetails()
        {
            Feedbacksummary fb = new Feedbacksummary();

            string userid = Convert.ToString(Session["uid"]);
            try
            {

                fb.lists = await iStudentRepository.getstudentdetailsfb(userid);

                if (fb.lists != null && fb.lists.Count() > 0)
                {
                    if (fb.list == null)
                    {
                        fb.list = new Feedback();
                        fb.list.NAME = fb.lists.FirstOrDefault().NAME;
                        fb.list.parent_mobile = fb.lists.FirstOrDefault().parent_mobile;

                    }
                    else
                    {
                        fb.list = fb.lists.Where(X => X.REGDNO == Convert.ToString(Session["uid"])).FirstOrDefault();
                    }

                }
                else
                {

                    if (fb.list == null)
                        fb.list = new Feedback();

                }


            }
            catch (Exception e)
            {
                throw e;
            }

            return PartialView("Hostelfeedbacksummary", fb);
        }

        [HttpPost]

        public async Task<ActionResult> createhostelfeedback(Feedback chart)
        {
            Feedbacksummary sumry = new Feedbacksummary();
            try
            {
                if (chart.ID == 0)
                {

                    chart.DT_TIME = DateTime.Now;

                    chart.collegecode = Convert.ToString(Session["college_code"]);
                    chart.branchcode = Convert.ToString(Session["branch_code"]);
                    chart.SEMESTER = Convert.ToString(Session["Curr_sem"]);
                    chart.SECTION = Convert.ToString(Session["section"]);
                    chart.campuscode = Convert.ToString(Session["campus_code"]);
                    chart.REGDNO = Convert.ToString(Session["uid"]);
                    chart.HOSTEL_BLOCK = Convert.ToString(Session["HOSTEL_BLOCK"]);
                    chart.gender = Convert.ToString(Session["gender"]);

                    var regdnochck = await iStudentRepository.registrationcheck(chart.REGDNO);

                    if (regdnochck.Count() > 0)
                    {
                        var mblcheck = await iStudentRepository.feedbackcheck(chart.REGDNO);

                        if (mblcheck.Count() == 0)
                        {

                            sumry.list = await iStudentRepository.inserthostelfeedback(chart);
                            if (sumry.list != null)
                            {
                                sumry.list.msg = "Successfully submitted";

                            }
                            else
                            {
                                sumry.list.msg = "Opps!!! something went wrong";

                            }
                        }
                        else
                        {
                            sumry.list = new Feedback();
                            sumry.list.msg = "Already submitted";


                        }

                    }
                    else
                    {
                        sumry.list = new Feedback();
                        sumry.list.msg = "please check registration number";

                    }

                }


            }

            catch (Exception)
            {
                RedirectToAction("Index", "../loginpage");
            }
            return Json(sumry);
        }

        public async Task<ActionResult> GetPublications()
        {
            publicationsummary summary = new publicationsummary();
            //  Session["REGDNO"] = "121965201020";
            string regdno = Session["REGDNO"].ToString();
            summary.notes = await iStudentRepository.Getpublications(regdno);
            summary.note = new Publications();
            return PartialView("Publications", summary);
        }

        [HttpPost]

        public async Task<ActionResult> CreatePublications(HttpPostedFileBase uploadForm, HttpPostedFileBase uploadForm1, Publications summary)
        {
            string msg = "";
            string absmsg = "";
            try
            {
                summary.Regdno = Session["REGDNO"].ToString();
                summary.stuname = Session["studname"].ToString(); summary.dept_code = Session["dept_code"].ToString(); summary.college_code = Session["college_code"].ToString();
                summary.campus_code = Session["campus_code"].ToString(); summary.batch = Session["batch"].ToString();
                IEnumerable<Publications> data = await iStudentRepository.Getpublicationscheck(summary.Regdno, summary.PUBLICATION_TITLE, summary.JOURNAL_NAME);
                summary.publication_abstract = uploadForm.FileName.ToString();
                summary.publication_article = uploadForm1.FileName.ToString();
                string path = @"C:\\Schloars_Publications\\";
                string path1 = @"C:\\Schloars_Publications1\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                if (!Directory.Exists(path1))
                {
                    Directory.CreateDirectory(path1);
                }
                String rno = null;
                if (uploadForm.FileName == "")
                {

                    summary.absmsg = "Abstract file not available";

                }
                else
                {
                    String ext = System.IO.Path.GetExtension(uploadForm.FileName);

                    // rno = scholar_Pid + "" + pubcnt;
                    rno = summary.Regdno + "_" + summary.JOURNAL_NAME;

                    if ((ext.Equals(".pdf")) || (ext.Equals(".PDF")))
                    {
                        int fileSize = uploadForm.ContentLength;
                        uploadForm.SaveAs(path + rno + ".pdf");

                        summary.absmsg = "Abstract file saved";

                    }
                    else
                    {
                        summary.absmsg = "Please upload The Publication Abstract file in pdf format only";
                    }

                }
                if (uploadForm1.FileName == "")
                {

                    summary.articlemsg = "  Article file not available";

                }
                else
                {
                    String ext1 = System.IO.Path.GetExtension(uploadForm1.FileName);

                    rno = summary.Regdno + "_" + summary.JOURNAL_NAME;

                    if ((ext1.Equals(".pdf")) || (ext1.Equals(".PDF")))
                    {
                        int fileSize = uploadForm1.ContentLength;
                        uploadForm1.SaveAs(path1 + rno + ".pdf");
                        summary.articlemsg = "  Article file saved";


                    }
                    else
                    {
                        summary.articlemsg = "Please upload The Publication Article file in pdf format only";

                    }
                }




                if (data.Count() <= 0)
                {
                    var result = await iStudentRepository.Insertpublication(summary);
                    if (result.flag == 1)
                    {
                        summary.msg = "Successfully submitted";
                    }
                    else
                    {
                        summary.msg = "Oops!!! something went wrong";
                    }
                }
                else
                {
                    summary.msg = "Publication record already exists";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(summary.msg);
        }

        [HttpPost]

        public ActionResult PdfGet(string PDFID)
        {
            string[] pdfpub = PDFID.Split('?');
            string pdfabt, pdfart, fileName = "";
            pdfabt = pdfpub[1];
            pdfart = pdfpub[1];
            PDFID = pdfpub[0];
            if (pdfabt == "0")
            {
                fileName = @"C:\Schloars_Publications\" + PDFID + ".pdf";
            }
            else if (pdfart == "1")
            {
                fileName = @"C:\Schloars_Publications1\" + PDFID + ".pdf";
            }

            if (fileName != "")
            {
                bool fileExists = System.IO.File.Exists(fileName);
                if (fileExists)
                {
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64 = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);

                    return Content(base64);
                    //return File(pdfByteArray, "application/pdf");
                }
            }

            return Content("PDF file not found");
        }
        [HttpPost]
        public async Task<ActionResult> DeletePublication(string regno, string pubtit, string jname)
        {
            string msg = "";
            try
            {
                string fileName = @"C:\\Schloars_Publications\\" + regno + "_" + jname + ".pdf";
                string fileName1 = @"C:\\Schloars_Publications1\\" + regno + "_" + jname + ".pdf";
                //string msg = "";
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        FileAttributes attributes = System.IO.File.GetAttributes(fileName);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes &= ~FileAttributes.ReadOnly;
                            System.IO.File.SetAttributes(fileName, attributes);
                            System.IO.File.Delete(fileName);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    // System.IO.File.Delete(fileName);
                }
                if (System.IO.File.Exists(fileName1))
                {
                    System.IO.File.SetAttributes(fileName1, FileAttributes.Normal);
                    try
                    {
                        System.IO.File.Delete(fileName1);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        FileAttributes attributes = System.IO.File.GetAttributes(fileName1);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes &= ~FileAttributes.ReadOnly;
                            System.IO.File.SetAttributes(fileName1, attributes);
                            System.IO.File.Delete(fileName1);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    // System.IO.File.Delete(fileName);
                }

                //if (System.IO.File.Exists(fileName1))
                //{
                //    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                //    System.IO.File.Delete(fileName1);
                //}
                var result = await iStudentRepository.Deletepublications(regno, pubtit, jname);
                if (result == 1)
                {
                    msg = "Record deleted successfully";
                }
                else
                {
                    msg = "Record deleted unsuccessfully";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            // Return a success response
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Getcoordinatormobile(string empid)
        {
            IEnumerable<Students> mobiledata = null;
            string campus_code = Session["campus_code"].ToString();
            try
            {
                mobiledata = await iStudentRepository.Getcoordinator_mobile(campus_code, empid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(mobiledata, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]

        public async Task<PartialViewResult> getthesis()
        {
            Studentrecordslist endo = new Studentrecordslist();
            string user_id = Session["uid"].ToString();
            try
            {
                endo.records = await iStudentRepository.getthesisasync(user_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Studentthesis", endo);
        }




        [HttpGet]

        public async Task<PartialViewResult> GetBusspassenrollment()
        {
            busspasssummary summary = new busspasssummary();
            string user_id = Session["uid"].ToString();
            string campus = Convert.ToString(Session["campus_code"]);
            try
            {
                summary.notes = await iStudentRepository.getstudentdetails(user_id);
                summary.note = summary.notes.FirstOrDefault();
                IEnumerable<Buspass> semslist = await iStudentRepository.Getbustypelist(campus);
                ViewBag.Semlist = semslist.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.bustype.ToString(),
                    Value = testgroup.bustype.ToString()
                });

            }
            catch (Exception)
            {
                RedirectToAction("Index", "../loginpage");
            }
            return PartialView("Buspassenrollment", summary.note);
        }


        public async Task<ActionResult> getroutes(string typeid)
        {

            busspasssummary endo = new busspasssummary();
            string campus_code = Convert.ToString(Session["campus_code"]);

            try
            {
                endo.notes = await iStudentRepository.getroutelist(typeid, campus_code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(endo, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> getfare(string typeid, string Board)
        {

            busspasssummary endo = new busspasssummary();
            string campus_code = Convert.ToString(Session["campus_code"]);

            try
            {
                endo.notes = await iStudentRepository.getfareasync(typeid, campus_code, Board);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(endo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public async Task<ActionResult> Insertbuspass(Buspass chart)
        {
            busspasssummary sumry = new busspasssummary();
            sumry.note = new Buspass();
            try
            {
                if (chart.REGDNO != null)
                {
                    chart.BUSPASSYEAR = System.DateTime.Now.Year.ToString();
                    chart.REGDNO = Convert.ToString(Session["uid"]);
                    chart.COLLEGE_CODE = Convert.ToString(Session["COLLEGE_CODE"]);
                    chart.BRANCH = Convert.ToString(Session["BRANCH_CODE"]);
                    chart.CAMPUS = Convert.ToString(Session["CAMPUS_CODE"]);

                    var regdnochck = await iStudentRepository.busregistrationcheck(chart.REGDNO, chart.BUSPASSYEAR);

                    if (regdnochck.Count() == 0)
                    {
                        sumry.note = await iStudentRepository.Insertbuspassasync(chart);
                        if (sumry.note != null)
                        {
                            sumry.note.msg = "Successfully submitted";

                        }

                    }
                    else
                    {

                        if (regdnochck.ElementAt(0).application_status == "SA" || regdnochck.ElementAt(0).application_status == "EA")
                        {
                            sumry.note.msg = "Already submitted";
                            sumry.note.msg1 = regdnochck.ElementAt(0).BOARDING_POINT;
                            sumry.note.msg2 = regdnochck.ElementAt(0).bustype;
                            sumry.note.msg3 = regdnochck.ElementAt(0).BUSPASSYEAR;
                        }
                        else
                        {
                            sumry.note.msg = "Opps!!! something went wrong";
                            sumry.note.msg1 = regdnochck.ElementAt(0).BOARDING_POINT;
                            sumry.note.msg2 = regdnochck.ElementAt(0).bustype;
                            sumry.note.msg3 = regdnochck.ElementAt(0).BUSPASSYEAR;
                        }


                    }

                }
                else
                {
                    sumry.note.msg = "please check registration number";

                }




            }

            catch (Exception)
            {
                RedirectToAction("Index", "../loginpage");
            }
            return Json(sumry);
        }

        public ActionResult Getemail_update_password()
        {
            Session["otp"] = "";
            return PartialView("Email_update_password");

        }

        public ActionResult Getotp(string pass, string crnpass)
        {
            string msg = "";
            try
            {

                String smsuser = "glearn", message = null;
                String smspassword = "glearn@gsms@123$";
                //    Session["mobile"] = "8121647490";
                string mobile = Session["mobile"].ToString();
                string numbers = "1234567890";
                string characters = numbers;
                int length = 5;
                string otp = string.Empty;

                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (otp.IndexOf(character) != -1);
                    otp += character;
                }

                if (Session["mobile"].ToString().Length < 10)
                {
                    msg = "Please update your mobile number in the View and update profile page";
                }
                else
                {
                    ViewBag.Mobile = Session["mobile"].ToString();
                    Session["otp"] = otp;
                    message = "Dear student,Your OTP for gitam.in password change is : " + otp + "";

                    SendSMS(smsuser, smspassword, mobile, message);
                    msg = "successfull";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        private WebProxy objProxy1 = null;

        public string SendSMS(string User, string password, string Mobile_Number, string Message)
        {
            string stringpost = null;
            stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message;
            // stringpost = "User=CDLGITAM&passwd=cdlatgitammobilenumber=9700656667&message=hi";                  
            HttpWebRequest objWebRequest = null;
            HttpWebResponse objWebResponse = null;
            StreamWriter objStreamWriter = null;
            StreamReader objStreamReader = null;

            try
            {

                string stringResult = null;

                objWebRequest = (HttpWebRequest)WebRequest.Create("http://www.smscountry.com/SMSCwebservice.asp");
                objWebRequest.Method = "POST";

                if ((objProxy1 != null))
                {
                    objWebRequest.Proxy = objProxy1;
                }
                objWebRequest.ContentType = "application/x-www-form-urlencoded";
                objStreamWriter = new StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(stringpost);
                objStreamWriter.Flush();
                objStreamWriter.Close();
                objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();
                objStreamReader.Close();
                return stringResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
                objProxy1 = null;
            }

        }

        public async Task<ActionResult> Getupdatepassword(string pass, string crnpass, string otp)
        {
            string Regdno = Session["REGDNO"].ToString();
            string stuname = Session["studname"].ToString();
            string mobile = Session["mobile"].ToString();
            string otpcorrect = Session["otp"].ToString();
            string msg = "";
            if (otp == otpcorrect)
            {
                var result = await iStudentRepository.Insertemailpassword(Regdno, stuname, pass, mobile);
                if (result == 1)
                {
                    msg = "Successfully updated";
                }
                else
                {
                    msg = "Oops!!! something went wrong";
                }
            }
            else
            {
                msg = "Please enter correct OTP!";
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> GetAccountsandpayments()
        {
            accountssummary account = new accountssummary();
            try
            {
                string campus = Session["campus_code"].ToString();
                string college = Session["college_code"].ToString();
                string Regdno = Session["REGDNO"].ToString();
                account.fee = await iStudentRepository.fillfee(Regdno);
                account.hostlefee = await iStudentRepository.fillhostelpayment(Regdno);
                account.hostledemands = await iStudentRepository.get_hosteldemand(Regdno);

                IEnumerable<Students> tutionfee = await iStudentRepository.getSeniorTutionFeeDemand(Regdno);
                double tution = 0;
                string vfee = "";
                if (tutionfee != null)
                {
                    if (tutionfee.Count() > 0)
                    {
                        vfee = (Int32.Parse(tutionfee.ElementAt(0).fee_demand1) + Int32.Parse(tutionfee.ElementAt(0).tution_fee_arrears)).ToString();
                    }
                }

                tution = Convert.ToDouble(vfee);
                string course = Session["course_code"].ToString();
                int fine = Convert.ToInt32(Session["fine"].ToString());
                account.fine = new Students()
                {
                    fine = Convert.ToString(tution + fine),
                };


                //account.fine.fine = Convert.ToString(tution + fine);


            }
            catch (Exception EX)
            {

            }
            return View("AccountsPayment", account);

        }


        public ActionResult Error()
        {

            return View();
        }
        public ActionResult Residence()
        {

            return View();
        }

        public async Task<ActionResult> Security()
        {
            Security obj = new Security();
            IEnumerable<Security> student_details = null;
            string Regdno = Session["REGDNO"].ToString();
            try
            {

                student_details = await iStudentRepository.GetSecurityReports(Regdno);
                // student_details = await _StudentRepository.GetSecurityReports(user.USER_ID);
            }
            catch (Exception ex)
            {

            }

            return View(student_details);
        }
        public async Task<ActionResult> Dining()
        {
            DiningSubscription chart = new DiningSubscription();
            IEnumerable<DiningSubscription> studentinfo = null;
            string student_id = Session["uid"].ToString();
            string CAMPUS = Session["CAMPUS_CODE"].ToString();

            studentinfo = await iStudentRepository.getstudentinfo(student_id, CAMPUS);
            if (studentinfo.Count() > 0)
            {
                Session["package_name"] = studentinfo.FirstOrDefault().package_name;
                Session["hostel_name"] = studentinfo.FirstOrDefault().hostel_name;
                Session["start_date"] = studentinfo.FirstOrDefault().start_date;
                Session["end_date"] = studentinfo.FirstOrDefault().end_date;
                chart.package_name = studentinfo.FirstOrDefault().package_name;
                chart.hostel_name = studentinfo.FirstOrDefault().hostel_name;
                chart.start_date = studentinfo.FirstOrDefault().start_date;
                chart.end_date = studentinfo.FirstOrDefault().end_date;
            }
            else
            {
                chart.package_name = "";
                chart.hostel_name = "";
                chart.start_date = "";
                chart.end_date = "";

            }
            if (chart == null)
            {
                chart = new DiningSubscription
                {
                };
            }
            return PartialView("Dining", studentinfo);
        }
        public async Task<JsonResult> Getdining(string date)
        {

            string CAMPUS = Session["CAMPUS_CODE"].ToString();
            IEnumerable<DiningSubscription> Details = null;
            if (date == null)
            {
                date = "";
            }
            if (date.Contains("to"))
            {
                string student_id = Session["uid"].ToString();
                string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
                string fromdate = dates[0];
                string todate = dates[1];
                Details = await iStudentRepository.getdininginfo(student_id, fromdate, todate, CAMPUS);
            }
            else
            {
                string student_id = Session["uid"].ToString();

                Details = await iStudentRepository.getdininginfo(student_id, date, null, CAMPUS);
            }
            return Json(Details, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Getresidence()
        {

            string Regdno = Session["REGDNO"].ToString();


            IEnumerable<residence> residencedetails = null;
            try
            {
                IEnumerable<residence> residencedetailshistory = await iStudentRepository.Getresidenceroomdetails(Regdno);
                residencedetails = await iStudentRepository.Getresidenceroomdetails19(Regdno);
                var model = new residencedatamodel
                {
                    history = residencedetailshistory,
                    maindata = residencedetails
                };
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionResult> Getresidenceattendance(string date)
        {

            string Regdno = Session["REGDNO"].ToString();

            IEnumerable<residence> residencedetails = null;
            try
            {
                residencedetails = await iStudentRepository.Getresidenceattendance(Regdno, date);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(residencedetails, JsonRequestBehavior.AllowGet);

        }




        [HttpGet]

        public async Task<PartialViewResult> Timetable_I_sem()
        {
            timetablesummary IPassessment = new timetablesummary();

            string Regdno = Session["REGDNO"].ToString();

            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Session["Curr_sem"]);
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);

            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);

            IEnumerable<Students> semslist = null;


            try
            {

                IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, Session["degree_code"].ToString());

                // DataTable timetable =  iStudentRepository.GetTimetable(Regdno, SEMESTER);




                semslist = await iStudentRepository.getFacultyDetailsTimetable(Regdno, SEMESTER);

                ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"
                };
                ViewBag.list = sessionslist.Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();

            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Timetable_I_sem", semslist);
        }



        public async Task<ActionResult> GetNewTimetable(string date)
        {

            string Regdno = Session["REGDNO"].ToString();
            string SEMESTER = Convert.ToString(Session["Curr_sem"]);
            DataTable timetable = null;
            try
            {
                timetable = iStudentRepository.GetTimetable(Regdno, SEMESTER); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(timetable);
            return Json(JSONString, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Student_displinary()
        {
            return View();
        }
        public async Task<ActionResult> GetStudent_displinary()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<student_displinary> displinary_details = await iStudentRepository.GetStudent_displinary(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<JsonResult> withdraw(string id)
        {
            int k = 0;

            string result = "";
            try
            {
                k = iStudentRepository.withdraw(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (k == 1)
            {
                result = "succesful";

            }
            else
            {
                result = "unsuccessful";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<PartialViewResult> GetSportsScholarship()
        {
            sportsscholarshipsummary fb = new sportsscholarshipsummary();
            SportsScholarship chart = new SportsScholarship();
            try
            {
                chart.RegID = Session["REGDNO"].ToString();
                fb.note = await iStudentRepository.getsportsscholarship(chart);

                IEnumerable<SportsScholarship> sportsList = await iStudentRepository.getsportslist();
                fb.notes = await iStudentRepository.getsportsscholargrid(chart.RegID);
                if (fb.notes.Count() > 0)
                {
                    fb.notes.FirstOrDefault().NAME = fb.note.NAME;
                    fb.notes.FirstOrDefault().COURSE_DISPLAY = fb.note.COURSE_DISPLAY;
                    fb.notes.FirstOrDefault().total_percent = fb.note.total_percent;
                    fb.notes.FirstOrDefault().total_exam_type = fb.note.total_exam_type;
                    fb.notes.FirstOrDefault().total_Partion_Percent = fb.note.total_Partion_Percent;
                    fb.notes.FirstOrDefault().GAT_RANK = fb.note.GAT_RANK;

                }
                else
                {

                }

                ViewBag.SportsList = sportsList.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.Sports_Name.ToString(),
                    Value = testgroup.Sport_ID.ToString()
                });
                IEnumerable<SportsScholarship> levelList = await iStudentRepository.getlevellist();

                ViewBag.LevelList = levelList.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.Level_of_Competition.ToString(),
                    Value = testgroup.Level_ID.ToString()
                });
                IEnumerable<SportsScholarship> acheivementlist = await iStudentRepository.getacheivementlist();

                ViewBag.Acheivementlist = acheivementlist.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.medal_name.ToString(),
                    Value = testgroup.medal_id.ToString()
                });
                if (fb.notes != null && fb.notes.Count() > 0)
                {
                    if (fb.note == null)
                    {
                        fb.note = new SportsScholarship();
                    }
                    else
                    {
                        fb.note = fb.notes.Where(X => X.RegID == Convert.ToString(Session["uid"])).FirstOrDefault();
                    }

                }
                else
                {

                    if (fb.note == null)
                        fb.note = new SportsScholarship();
                }
                if (fb.note.sport_date == null)
                {
                    fb.note.sport_date = Convert.ToDateTime(DateTime.Now).ToString("dd-MMM-yyyy");
                }
                else
                {
                    fb.note.sport_date = Convert.ToDateTime(fb.note.sport_date).ToString("dd-MMM-yyyy");
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return PartialView("SportsScholarshipSummary", fb);
        }

        public async Task<ActionResult> competetionnames(string levelid)
        {

            sportsscholarshipsummary endo = new sportsscholarshipsummary();

            try
            {
                endo.notes = await iStudentRepository.getcompetetionnames(levelid);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(endo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSportsScholarship(HttpPostedFileBase uploadForm, SportsScholarship summary)
        {
            string msg = "";
            string absmsg = "";
            try
            {
                summary.RegID = Session["REGDNO"].ToString();

                string current_year = DateTime.Now.Year.ToString();
                DateTime startdate;

                startdate = Convert.ToDateTime("01-APR-2023");

                string dddate = summary.sport_date.ToString();


                DateTime dt = DateTime.Parse(dddate);
                string s2 = dt.ToString("dd-MM-yyyy");
                DateTime dtnew = DateTime.ParseExact(s2, "dd-MM-yyyy", null);

                var ddd = ((startdate.Date) - (dtnew.Date)).TotalDays;

                if (Convert.ToInt32(ddd) > 0)
                {
                    summary.msg = "After 01-APR-2023 can apply";
                }
                else
                {
                    var sportchck = await iStudentRepository.getsportsschck(summary.RegID);
                    var sportidchck = await iStudentRepository.getsportsidchck(summary.RegID);
                    int i = 0;
                    if ((sportchck[0].TotalSports == "0") || (sportchck[0].TotalSports == "1"))
                    {
                        i = 0;
                    }
                    else
                    {
                        foreach (var sportId in sportidchck)
                        {
                            if (sportId.Sport_ID == summary.Sport_ID)
                            {
                                i++;
                            }
                            else
                            {
                                i = 0;
                                break;
                            }
                        }
                    }
                    //  var data = await iStudentRepository.getsportsscholarship(summary);

                    summary.publication_abstract = uploadForm.FileName.ToString();
                    string path = @"C:/SportsCertificatesNew/";

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    String rno = null; String filename = null;
                    if (uploadForm.FileName == "")
                    {

                        summary.msg = "Abstract file not available";

                    }
                    else
                    {
                        String ext = System.IO.Path.GetExtension(uploadForm.FileName);
                        rno = summary.RegID;
                        filename = "_" + summary.publication_abstract;

                        if ((ext.Equals(".pdf")) || (ext.Equals(".PDF")))
                        {
                            int fileSize = uploadForm.ContentLength;
                            uploadForm.SaveAs(path + rno + filename);
                            summary.Sport_Certificate = path + rno + filename;
                            summary.msg = "Abstract file saved";

                        }
                        else
                        {
                            summary.msg = "Please upload The Publication Abstract file in pdf format only";
                        }

                    }
                    if (i == 0 || i == 1)
                    {
                        var result = await iStudentRepository.InsertSportsScholarship(summary);



                        if (result.flag == 1)
                        {
                            summary.msg = "Inserted successfully";
                        }
                        else
                        {
                            summary.msg = "Please check sport";
                        }
                    }

                    else
                    {
                        summary.msg = "Please check Sport";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(summary);
        }
        public async Task<ActionResult> getsportsscholargrid()
        {
            string userid = Convert.ToString(Session["uid"]);

            sportsscholarshipsummary endo = new sportsscholarshipsummary();

            try
            {
                endo.notes = await iStudentRepository.getsportsscholargrid(userid);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(endo, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Updateremarks(string remarks)
        {
            string userid = Convert.ToString(Session["uid"]);
            sportsscholarshipsummary endo = new sportsscholarshipsummary();
            SportsScholarship scholar = new SportsScholarship();
            try
            {
                scholar.RegID = Convert.ToString(Session["uid"]);
                scholar.remarks = remarks;
                var result = await iStudentRepository.Updateremarks(scholar);
                if (result.flag == 1)
                {
                    scholar.msg = "Remarks Updated successfully";
                }
                else
                {
                    scholar.msg = "Remarks updated fail";
                }

            }
            catch (Exception e)
            {
            }
            return Json(scholar, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Deletesportscholarship(string slno, string regdno, string cert)
        {
            SportsScholarship scholar = new SportsScholarship();




            string fileName = cert;
            //string msg = "";
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (UnauthorizedAccessException)
                {
                    FileAttributes attributes = System.IO.File.GetAttributes(fileName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes &= ~FileAttributes.ReadOnly;
                        System.IO.File.SetAttributes(fileName, attributes);
                        System.IO.File.Delete(fileName);
                    }
                    else
                    {
                        throw;
                    }
                }
                // System.IO.File.Delete(fileName);
            }

            var result = await iStudentRepository.Deletesportscholarship(slno, regdno);
            if (result == 1)
            {
                scholar.msg = "Record deleted successfully";
            }
            else
            {
                scholar.msg = "Record deleted unsuccessfully";
            }

            // Return a success response
            return Json(scholar.msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult PdfGetscholarship(string PDFID)
        {
            string[] pdfpub = PDFID.Split('_');
            string[] pdfpub1 = PDFID.Split('/');
            string pdfabt, fileName = "";

            PDFID = pdfpub1[2];

            fileName = @"C:/SportsCertificatesNew/" + PDFID;

            if (fileName != "")
            {
                bool fileExists = System.IO.File.Exists(fileName);
                if (fileExists)
                {
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64 = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);

                    return Content(base64);
                    //return File(pdfByteArray, "application/pdf");
                }
            }

            return Content("PDF file not found");
        }
        public ActionResult Dashboard()
        {

            return View();
        }
        public ActionResult Mobiledashboard()
        {

            return View();
        }

        public ActionResult Welcome()
        {

            return View();
        }

        public async Task<JsonResult> Dashboardvisit(string id)
        {

            IEnumerable<Students> permissions = null;
            permissions = await iStudentRepository.getAllhostlerpermissions(Session["REGDNO"].ToString());
            if (permissions.Count() > 0)
            {

                permissions = permissions.OrderByDescending(X => Convert.ToDateTime(X.AppliedDate));
                ViewBag.underprocess = (permissions.Where(X => X.Isapprove.Equals("N") && X.Permissiondate >= DateTime.Today)).Count();
                ViewBag.approved = (permissions.Where(X => X.Isapprove.Equals("Y") && X.Permissiondate >= DateTime.Parse("01-june-2023"))).Count();
                ViewBag.rejected = (permissions.Where(X => X.Isapprove.Equals("I") && X.Permissiondate >= DateTime.Parse("01-june-2023"))).Count();
            }
            else
            {
                ViewBag.underprocess = 0;
                ViewBag.approved = 0;
                ViewBag.rejected = 0;

            }
            var percentage1 = "0";
            /*attendance*/
            Attendancesummary summary = new Attendancesummary();
            Attendancesummary subject = new Attendancesummary();
            accountssummary account = new accountssummary();
            newssummary NEWSLIST = new newssummary();
            Lmsdirectorysummary LMSlist = new Lmsdirectorysummary();

            string college_code = Session["college_code"].ToString();
            string campus = Session["campus_code"].ToString();
            string classs = Session["CLASS"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string hostler = Session["hostler"].ToString();
            string branch_code = Convert.ToString(Session["branch_code"]);
            string degree = Session["degree_code"].ToString();
            string batch = Session["batch"].ToString();
            string[] batch1 = new string[1000];
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string batchs = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
            string batchattd = batchs.ToString();
            string section=Session["section"].ToString();

            string course=Session["course_code"].ToString();
            string user_id = Session["uid"].ToString();
            string[] weekday = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            DateTime currentDate = DateTime.Now;
            string day = weekday[(int)currentDate.DayOfWeek];

            account.hostlefee = await iStudentRepository.filldashboardhostelpayment(user_id, Session["APPlno"].ToString());

            NEWSLIST.news = await iStudentRepository.getdashboardevents(campus);
            LMSlist.assigndata = await iStudentRepository.getassignmentpending(user_id);

            account.tutionfee = await iStudentRepository.getdashboardSeniorTutionFeeDemand(user_id, Session["APPlno"].ToString());
            //  account.timetable = await iStudentRepository.getdashboardtimetable(user_id, Session["APPlno"].ToString(), day);
        
            account.timetablenew = await iStudentRepository.gettimetabledetails(user_id, college_code, branch_code, cur_sem, section, campus, batchattd,course, degree);
            summary.course = await iStudentRepository.Getcoursestructure(college_code, campus, branch_code, batch2, cur_sem, user_id);


            if (campus == "VSP" || campus == "HYD")
            {

                if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
                {
                    summary.notes = await iStudentRepository.Getattendance_semster_byselection_gimsr(user_id, table_name, college_code, cur_sem, batchattd, "Today", DateTime.Today.ToString());
                }
                else
                {
                    if (classs == "I Yr")
                    {
                        summary.notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, "Today", DateTime.Today.ToString());
                    }
                    else
                    {
                        summary.notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, "Today", DateTime.Today.ToString());


                    }
                }
            }
            else
            {
                summary.notes = await iStudentRepository.Getattendance_semster_byselection(user_id, table_name, college_code, cur_sem, batchattd, "Today", DateTime.Today.ToString());

            }

            if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
            {
                subject.notes = await iStudentRepository.Getattendance_semster_gimsr(user_id, table_name, college_code, cur_sem, batchattd);
            }
            else
            {
                subject.notes = await iStudentRepository.Getattendance_semster(user_id, table_name, college_code, cur_sem, batchattd);
            }

            if (subject.notes.Count() > 0)
            {
                percentage1 = subject.notes.FirstOrDefault().percentage;
            }
            else
            {
                percentage1 = "0";

            }
            IEnumerable<residence> biometricdetails = await iStudentRepository.Getbiometricattendance_dashboard(user_id);
            int semester = 0;
            if (cur_sem != null)
            {
                semester = Convert.ToInt32(cur_sem) - 1;

            }
            else
            {

                semester = 0;
            }
            if (summary.notes.Count() > 0)
            {


                ViewBag.Presentcount = (summary.notes.Where(X => X.Status.Equals("Present"))).Count();
                ViewBag.Absentcount = (summary.notes.Where(X => X.Status.Equals("Absent"))).Count();

            }
            else
            {
                ViewBag.Presentcount = 0;
                ViewBag.Absentcount = 0;


            }

            List<studenttrack> details1 = await iStudentRepository.getperformancedashboard(semester.ToString(), Session["REGDNO"].ToString());
            return Json(new { presentcount = ViewBag.Presentcount, absentcount = ViewBag.Absentcount, news = NEWSLIST.news, timetable = account.timetablenew, assignlist = LMSlist.assigndata, Course = summary.course, Marks = details1, biometric = biometricdetails, hostlefee = account.hostlefee, tutionfee = account.tutionfee, percentage = percentage1, todayattendance = summary.notes.Count(), todayhostelfee = account.hostlefee.Count(), underprocess = ViewBag.underprocess, approved = ViewBag.approved, rejected = ViewBag.rejected }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public async Task<ActionResult> Getcourse_structurenew()
        {
            Attendancesummary summary = new Attendancesummary();

            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string user_id = Session["uid"].ToString();

            string college_code = Session["college_code"].ToString();
            string branch_code = Session["branch_code"].ToString();
            string campus_code = Session["campus_code"].ToString();
            string sem = Session["Curr_sem"].ToString();
            try
            {
                summary.note = new Attendance();
                summary.notes = await iStudentRepository.Getcoursestructuremenu(college_code, campus_code, branch_code, batch2, sem, user_id);
                //ViewBag.Semlist = semslist.Select(testgroup => new SelectListItem
                //{
                //    Text = testgroup.SEMESTER.ToString(),
                //    Value = testgroup.SEMESTER.ToString()
                //});
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Course_structurenew", summary);
        }


        //public ActionResult Fees()
        //{

        //    return View();
        //}

        public async Task<ActionResult> Fees()
        {
            Feesummary admission = new Feesummary();
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                string Regdno = Session["REGDNO"].ToString();
                admission.tutionfee = await iStudentRepository.gettuitionfee(Regdno);
                admission.otherfee = await iStudentRepository.getotherfee(Regdno);
                admission.evaluationfee = await iStudentRepository.getevaulfee(Regdno);
                admission.fillfee = await iStudentRepository.fillfee(Regdno);
                admission.fillhostel = await iStudentRepository.fillhostelpayment(Regdno);
                admission.payment_details = await iStudentRepository.Student_AdmPaymentdetails(APPLICATION_NO);
            }
            catch (Exception ex)
            {

            }
            return View("Fees", admission);
        }

        [HttpGet]
        public async Task<ActionResult> gettuitionfee()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.gettuitionfee(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<ActionResult> getotherfee()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.getotherfee(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        [HttpGet]
        public async Task<ActionResult> getevaulfee()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.getevaulfee(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpGet]
        public async Task<ActionResult> tutionpayment()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<Students> displinary_details = await iStudentRepository.fillfee(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpGet]
        public async Task<ActionResult> hostelpayment()
        {
            try
            {
                string Regdno = Session["REGDNO"].ToString();
                IEnumerable<Students> displinary_details = await iStudentRepository.fillhostelpaymentfees(Regdno);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        //public ActionResult Admission()
        //{

        //    return View();
        //}



        public async Task<ActionResult> Admission()
        {
            AdmissionsSummary admission = new AdmissionsSummary();
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                admission.displinary_details = await iStudentRepository.Student_Admissiondata(APPLICATION_NO);
                admission.branch_data = await iStudentRepository.Student_branchdata(APPLICATION_NO);
                admission.admission_data = await iStudentRepository.Student_Admissiondata(APPLICATION_NO);
                admission.counseling_data = await iStudentRepository.Student_counseling(APPLICATION_NO);
                admission.payment_details = await iStudentRepository.Student_AdmPaymentdetails(APPLICATION_NO);
                admission.scholoreligbility = await iStudentRepository.Student_Scholareligibility(APPLICATION_NO);
                admission.scholorallocated = await iStudentRepository.Student_Scholarallocated(APPLICATION_NO);
                admission.Coursesappliedandpayment = await iStudentRepository.Coursesappliedandpayment(APPLICATION_NO);
            }
            catch (Exception ex)
            {

            }
            return View("Admission", admission);
        }

        [HttpGet]
        public async Task<ActionResult> Student_Admissiondata()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_Admissiondata(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpGet]
        public async Task<ActionResult> Student_branchdata()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_branchdata(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<ActionResult> Student_counselingbranch()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_counseling(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<ActionResult> Student_AdmPaymentdetails()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_AdmPaymentdetails(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        [HttpGet]
        public async Task<ActionResult> Student_Scholareligibility()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_Scholareligibility(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<ActionResult> Student_Scholarallocated()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Student_Scholarallocated(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        [HttpGet]
        public async Task<ActionResult> Coursesappliedandpayment()
        {
            try
            {
                string APPLICATION_NO = Session["APPLICATION_NO"].ToString();
                IEnumerable<SportsScholarship> displinary_details = await iStudentRepository.Coursesappliedandpayment(APPLICATION_NO);
                return Json(displinary_details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<ActionResult> Contactus()
        {
            Contactussummary chartsmry = new Contactussummary();
            Contactus chart = new Contactus();
            chart.REGDNO = Session["uid"].ToString();
            chart.CAMPUS = Session["CAMPUS_CODE"].ToString();
            chart.COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            chart.DEPT_CODE = Session["dept_code"].ToString();
            chartsmry.note = await iStudentRepository.contactusasync(chart);

            if (chart == null)
            {
                chart = new Contactus
                {
                };
            }
            else
            {

                if (Session["gender"].ToString() == "M" && Session["campus_code"].ToString() == "VSP")
                {

                    chart.EMP_NAME = "  Prajith B Kurup, Residence Manager ";
                    chart.Mobile = "0891-2504074, 9100808220";
                }
                else if (Session["gender"].ToString() == "F" && Session["campus_code"].ToString() == "VSP")
                {
                    chart.EMP_NAME = "Mounika Gottipati, Residence Manager ";
                    chart.Mobile = " 0891-2840561, 9052183550";
                }
                else if (Session["gender"].ToString() == "M" && Session["campus_code"].ToString() == "HYD")
                {
                    chart.EMP_NAME = "D.Suresh Residence Manager";
                    chart.Mobile = " 08455 221212/213, 7093796641 ";
                }
                else if (Session["gender"].ToString() == "F" && Session["campus_code"].ToString() == "HYD")
                {
                    chart.EMP_NAME = "Parinita Saikia ,Residence Manager ";
                    chart.Mobile = "08455221221/222, 9007372055";
                }
                else if (Session["gender"].ToString() == "M" && Session["campus_code"].ToString() == "BLR")
                {
                    chart.EMP_NAME = "Mr. Aniesh Menon, Residence Manager ";
                    chart.Mobile = "8971199952";

                }
                else if (Session["gender"].ToString() == "F" && Session["campus_code"].ToString() == "BLR")
                {

                    chart.EMP_NAME = "Mrs. Bharathi Gadipally ,Residence Manager ";
                    chart.Mobile = "8971199921";


                }
            }
            return PartialView("ContactUs", chartsmry);
        }
        public async Task<ActionResult> Getstudent_remarksdata()
        {
            string REGDNO = Session["uid"].ToString();
            IEnumerable<Contactus> mentor_remarksdata = await iStudentRepository.getmentor_remarksdata(REGDNO);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Getstudent_results()
        {
            string REGDNO = Session["uid"].ToString();
            // IEnumerable<Contactus> mentor_remarksdata = await iStudentRepository.getmentor_remarksdata(REGDNO);
            // return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);


            IEnumerable<studenttrack> semslist = await iStudentRepository.getsemesterasync(REGDNO);

            ViewBag.semslist = semslist.Select(testgroup => new SelectListItem
            {
                Text = testgroup.SEMESTER_DISPLAY.ToString(),
                Value = testgroup.SEMESTER.ToString()
            });
            return View("Results");
        }


        [HttpGet]

        public async Task<JsonResult> Resultsview(string semester, string userid)
        {

            //trackrecords endo = new trackrecords();
            userid = Convert.ToString(Session["uid"]);
            //string coursecode = Convert.ToString(Session["course_code"]);
            //string BATCH = Convert.ToString(Session["BATCH"]);
            string currentsem = Convert.ToString(Session["curr_sem"]);
            string college_code = Convert.ToString(Session["college_code"]);
            //try
            //{
            //    endo.lists = await iStudentRepository.getcourselist(semester, userid, coursecode, BATCH, currentsem);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            IEnumerable<studenttrack> resultdata = await iStudentRepository.getresults(userid, semester, college_code);
            return Json(resultdata, JsonRequestBehavior.AllowGet);

        }


        //[HttpGet]

        //public async Task<JsonResult> getGradecard(string semester, string userid, string process)

        [HttpGet]
        public async Task<JsonResult> getGradecard(string semester, string process, string month, string year)
        {

            //Session["REGDNO"] = "1210116117";

            gradecardsummary data = new gradecardsummary();

            string userid = Session["uid"].ToString();
            Session["Gprocess"] = process;
            string QCText = "";
            string str = "";
            string campus = "";
            string college = "";
            string degree = "";
            string course = "";
            string branch = "";
            string batch = "";
            string type = "";
            if (semester == "91" || process == "RB" || semester == "94")
            {
                type = "Re Registartion";
            }
            if (semester == "92" && process == "SummerTerm 2" && month == "Jun" && year == "2024" || semester == "93" && month == "Jun" && year == "2024")
            {
                type = "Summerterm";
            }

            // month = "";
            //string year = "";
            try
            {
                List<studenttrack> records = null;
                List<studenttrack> details = null;
                if (semester == "92" && process == "SummerTerm 2" && month == "Jun" && year == "2024" || semester == "93" && month == "Jun" && year == "2024")
                {
                    records = await iStudentRepository.getStudentcarddetailsst2(semester, userid);
                    details = await iStudentRepository.getmonthst2(semester, userid, process);
                }
                else
                {
                    records = await iStudentRepository.getStudentcarddetails(semester, userid);
                    details = await iStudentRepository.getmonth(semester, userid, process);
                }

                //if (details.Count() > 0)
                //{
                //    month = details.FirstOrDefault().Month;
                //    year = details.FirstOrDefault().Year;
                //}
                month = month;
                year = year;

                if (records.Count() > 0)
                {
                    campus = records.FirstOrDefault().CAMPUS_CODE;
                    college = records.FirstOrDefault().college_code;
                    degree = records.FirstOrDefault().DEGREE_CODE;
                    course = records.FirstOrDefault().COURSE_CODE;
                    branch = records.FirstOrDefault().BRANCH_CODE;
                    batch = records.FirstOrDefault().BATCH;


                }

                int delete = await iStudentRepository.DeleteDetails(campus, college, course, branch, batch);
                //int delete = 1;

                List<studenttrack> GradeResult = null;
                if (semester == "92" && process == "SummerTerm 2" && month == "Jun" && year == "2024" || semester == "93" && month == "Jun" && year == "2024")
                {

                    GradeResult = await iStudentRepository.getGradeResultst2(semester, userid, month, year, process);

                }
                else
                {

                    GradeResult = await iStudentRepository.getGradeResult(semester, userid, month, year, process);

                }

                studenttrack detailsdata = new studenttrack()
                {
                    regdno = GradeResult.FirstOrDefault().regdno,
                    name = GradeResult.FirstOrDefault().name,
                    BRANCH = GradeResult.FirstOrDefault().BRANCH_CODE,
                    COURSE = GradeResult.FirstOrDefault().COURSE_CODE,
                    DEGREE_CODE = GradeResult.FirstOrDefault().DEGREE_CODE,
                    college_code = GradeResult.FirstOrDefault().college_code,
                    CAMPUS_CODE = GradeResult.FirstOrDefault().CAMPUS_CODE,
                    course_name = GradeResult.FirstOrDefault().course_name,
                    Branch_name = GradeResult.FirstOrDefault().Branch_name,
                    section = GradeResult.FirstOrDefault().section,
                    COURSE_TITLE = GradeResult.FirstOrDefault().COURSE_TITLE,
                    PRINT_YEAR = GradeResult.FirstOrDefault().PRINT_YEAR,
                    EXAM_NAME = GradeResult.FirstOrDefault().EXAM_NAME,
                    exam_date = GradeResult.FirstOrDefault().exam_date,
                    Month = month,
                    Year = GradeResult.FirstOrDefault().Year,
                    BATCH = GradeResult.FirstOrDefault().BATCH,
                    process_type = process,
                    sgpa = GradeResult.FirstOrDefault().sgpa,
                    cgpa = GradeResult.FirstOrDefault().cgpa,
                    TOTAL_SEM_CREDITS = GradeResult.FirstOrDefault().TOTAL_SEM_CREDITS,
                    CUM_SEM_CREDITS = GradeResult.FirstOrDefault().CUM_SEM_CREDITS,
                    SEMESTER = semester,



                };

                var insertdetails = await iStudentRepository.InsertGradedetails(detailsdata);

                List<studenttrack> detailsmore = await iStudentRepository.getGradeReports(detailsdata);
                List<studenttrack> grademarks = new List<studenttrack>();
                if (detailsmore != null)
                {
                    if (detailsmore.Count() > 0)
                    {
                        for (int i = 0; i < detailsmore.Count(); i++)
                        {
                            studenttrack grades = new studenttrack()
                            {
                                regdno = GradeResult.FirstOrDefault().regdno,
                                SUBJECT_CODE = detailsmore.ElementAt(i).SUBJECT_CODE,
                                SUBJECT_NAME = detailsmore.ElementAt(i).SUBJECT_NAME,
                                CREDITS1 = detailsmore.ElementAt(i).CREDITS1,
                                Grade = detailsmore.ElementAt(i).Grade,
                                SEMESTER = detailsmore.ElementAt(i).SEMESTER,
                                //process_type = detailsmore.ElementAt(i).process_type,
                                process_type = process,
                                Month = GradeResult.FirstOrDefault().Month,
                                Year = GradeResult.FirstOrDefault().Year,
                                CAMPUS_CODE = campus
                            };
                            grademarks.Add(grades);
                        }

                    }
                    var updatedetails = await iStudentRepository.updateGradedetails(grademarks);
                }

                if (details.Count() > 0)
                {

                    List<studenttrack> carddetails = await iStudentRepository.getcarddetails(userid);
                    carddetails = carddetails.Where(m => m.emonth == month && m.Year == year).ToList();

                    QCText = " ";
                    string TY = "QRCODE";
                    string DT_Completion = "";
                    // QCText = QCText + "(Demed to be university)  ";
                    foreach (var dtr in carddetails)
                    {
                        QCText = "";
                        TY = TY + "$" + dtr.regdno.Trim() + "$" + dtr.SEMESTER.Trim() + "$" + dtr.emonth.Trim() + "$" + dtr.Year.Trim() + "$" + dtr.process_type.Trim() + "$";

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {

                            if (dtr.sgpa == "0" && dtr.cgpa == "0")
                                DT_Completion = "-";
                            else
                                DT_Completion = Convert.ToDateTime(dtr.DT_Completion).ToString("dd-MMM-yyyy");
                            QCText = QCText + "https://onlinecertificates.gitam.edu/View_Result_Grid1.aspx?QT=" + TY + "";

                        }
                        else
                        {
                        
                            QCText = QCText + "https://gstudent.gitam.edu/Home/ViewResultGrid?QT=" + TY + "";
                        }

                        QCText = QCText + "\nRegdno: " + dtr.regdno + ",\nName: " + dtr.name + ",\nCourse:" + dtr.COURSE + ",\nBranch:" + dtr.Branch_name + ",Semester: " + dtr.SEMESTER;//,Semester : "+dtr[83].ToString()
                        QCText = QCText + "\nGITAM (Deemed to be University)\n";

                    }
                    // GenerateMyQCCode(QCText);
                    foreach (var dr in carddetails)
                    {
                        //String month = Request.Params["month"].ToString();

                        str = str + "<div class=\"wrapper\"><div class=\"abc img_hover\" style=\"margin-top:30px;\" >";
                        str = str + "<div class=\"innerdiv\">";
                        str = str + "<Label hidden id='qrdata'>" + TY + "</Label>";
                        str = str + "<table   style=\"border-radius: 10px;margin: 10px auto;width: 650px;\" cellspacing=\"0\" cellpadding=\"0\" border = \"0\" >";
                        str = str + "<tbody>";

                        str = str + "<tr>";


                        str = str + "<table width=\"200\" class='sa-width'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:right;\">";

                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";


                        str = str + "<tr>";

                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        str = str + "<table>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<td>  <img src=\"images/academiccert1.jpg\" class='logo' /></td>";


                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";




                        str = str + "<tr>";
                        //< p style =\"font-size:12px;\"</p><p style=\"font-size:12px;\"></p>
                        str = str + "<table width=\"650\" style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<center style='margin-top:10px;'><b style=\"font-size:24px;\"><span style='color:#026B5C'>GRADE CARD</span></b></center>";
                        //str = /*str + "<br/>";*/


                        //str = str + "<center style=\"font-size:18px;\"><b>" + dr[83] + " Semester, " + dr[85] + " " + dr[84] + "</b></center>";

                        if (Convert.ToString(Session["college_code"]).Equals("GPP") || dr.BRANCH == "Optometry")
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\"> " + dr.Branch_name + " - Degree Examination</b></center>";

                        }
                        else
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.COURSE + " Degree Examination</b></center>";
                        }
                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            if (dr.COURSE.Trim().ToUpper().Equals("MBA") || dr.COURSE.Trim().ToUpper().Equals("M.B.A"))
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Year, " + dr.Month + " " + dr.Year + "</b></center>";

                        }
                        else
                        {
                            if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                            {
                                str = str + "<center style=\"font-size:18px;\"><b>" + type + " " + dr.roman_semester + " Trimester, " + dr.Month + " " + dr.Year + "</b></center>";
                            }
                            else if (Convert.ToInt32(dr.SEMESTER) >= 21)
                                str = str + "<center style=\"font-size:18px;\"><b>" + type + " " + dr.roman_semester + ", " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + type + " " + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                        }
                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";

                        str = str + "</tr>";



                        str = str + "<tr>";

                        str = str + "<table class='mt-1 sa-width details' style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        //str = str + "<tr>"; str = str + "<td  width=\"650\">"; str = str + "<table width=\"550\" class='mt-1 sa-width details' style='margin-bottom:20px'>"; str = str + "<tbody>";//msr
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Regd.No</span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.regdno + "</td>";
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Name </span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.name + "</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "</tr>";
                        if (!Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>Branch</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + dr.Branch_name + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }

                        if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            string gppname = "Kautilya School of Public Policy";
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }
                        //else if(Session["phdcoursecode"].ToString().Equals("PHD"))
                        //{
                        //    string gppname = "Inistitute of Technology";
                        //    str = str + "<tr>";
                        //    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                        //    str = str + "<td> :</td>";
                        //    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "</tr>";
                        //}


                        str = str + "</tbody>";
                        str = str + "</table>";



                        str = str + "</tr>";

                        str = str + "<tr>";


                        //str = str + "<div class=\"b12121\">";
                        //height =\"350\"
                        str = str + "<div class='table-responsive'><table class=\"tab mt-2 mb-2 sa-width\">";
                        str = str + "<tbody>";
                        str = str + "<tr class=\"\" style='border:2px solid #026B5C'>";
                        str = str + "<td style=\"text-align:left;padding:4px;width:105px;\"> <span style='color:#026B5C;'><b>Course Code</b></span> </td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Name of the Course</b></span> </td>";

                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Credits</b> </span></td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Grade</b> </span></td>";

                        str = str + "</tr>";






                        if (dr.SUB1_CODE != null)
                        {
                            if (dr.SUB1_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB1_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB1_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB2_CODE != null)
                        {
                            if (dr.SUB2_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB2_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB2_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB3_CODE != null)
                        {
                            if (dr.SUB3_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB3_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB3_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB4_CODE != null)
                        {
                            if (dr.SUB4_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB4_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB4_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB5_CODE != null)
                        {
                            if (dr.SUB5_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB5_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB5_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB6_CODE != null)
                        {
                            if (dr.SUB6_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB6_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB6_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB7_CODE != null)
                        {
                            if (dr.SUB7_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB7_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB7_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB8_CODE != null)
                        {
                            if (dr.SUB8_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB8_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB8_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB9_CODE != null)
                        {
                            if (dr.SUB9_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB9_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB9_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB10_CODE != null)
                        {
                            if (dr.SUB10_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB10_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB10_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB11_CODE != null)
                        {
                            if (dr.SUB11_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB11_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB11_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB12_CODE != null)
                        {
                            if (dr.SUB12_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB12_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB12_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB13_CODE != null)
                        {
                            if (dr.SUB13_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB13_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB13_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB14_CODE != null)
                        {
                            if (dr.SUB14_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB14_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB14_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB15_CODE != null)
                        {
                            if (dr.SUB15_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB15_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB15_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }

                        str = str + "</tbody>";
                        str = str + "</table></div>";

                        str = str + "</tr>";


                        str = str + "<tr>";


                        str = str + "<div class='sgpa'><table><tbody><tr>";
                        dr.cgpa = dr.cgpa;
                        //if (Session["college_code"].ToString() == "GIM" || Session["college_code"].ToString() == "SMS" || Session["college_code"].ToString() == "HBS" || Session["college_code"].ToString() == "GIS" || Session["college_code"].ToString() == "GSS" || Session["college_code"].ToString() == "GSHS" || Session["college_code"].ToString() == "GSGS")
                        //{

                        //    if (Session["branch_code"].ToString() == "BLENDED" || Session["branch_code"].ToString() == "CHEMISTRY-PFIZER" || Session["branch_code"].ToString() == "CHEMISTRY1" || Session["branch_code"].ToString() == "Optometry" || Session["branch_code"].ToString() == "CSCS" || Session["branch_code"].ToString() == "BCA")
                        //    { }
                        //    else
                        //    {
                        //        string s = Session["Curr_sem"].ToString();
                        //        string b = Session["batch"].ToString();
                        //        if (dr.SEMESTER== "6" && b.Contains("2021-"))
                        //        {
                        //            dr.cgpa = "*";
                        //        }
                        //        else { dr.cgpa = dr.cgpa; }
                        //    }
                        //}

                        if (dr.sgpa == "0" && dr.cgpa == "0")
                        {

                            str = str + "<td> <p style =\"font-size:15px;\">SGPA </p></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <p style =\"font-size:15px;\">CGPA </p></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }
                        else
                        {

                            str = str + "<td> <span style =\"font-size:15px;\">SGPA </span></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <span style =\"font-size:15px;\">CGPA </span></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }

                        str = str + "</tr>";
                        str = str + "<div class=\"b6\">";
                        //str = str + "<span> <b>Printed On:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "</td>";
                        //str = str + "<br/></span>";

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b>Printed On&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/></span>";


                        }

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b style =\"font-size:15px;\">Printed On&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/><br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Mode of delivery&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;ODL";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of Admission&nbsp; :</b>&nbsp;" + Convert.ToString(Session["AdmissionDate"]) + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of completion :</b>&nbsp;" + DT_Completion + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Learner Support Centres :&nbsp;---</b>" + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Examination Centres&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;GITAM (deemed to beUniversity), VISAKHAPATNAM.";
                            str = str + "<br/></span>";
                        }



                        str = str + "</div>";
                        str = str + "<div class=\"b5\">";
                        str = str + "<br/>";
                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            // if(Session["Gprocess"].ToString().Trim().Equals("SRV"))
                            // {
                            //     //str = str + "<div style=\"width:600px;\"><b>''Certificate issued after the publication of supplimentary revaluation results''</b></div>";
                            // }
                            //else
                            //if (Session["Gprocess"].ToString().Trim().Contains("RV"))
                            //{
                            //    str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            //}
                            //else if (Session["Gprocess"].ToString().Trim().Contains("BG"))
                            //{
                            //str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            //}
                            if (Convert.ToString(Session["Gprocess"]).Trim().Contains("RV"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            }
                            else if (Convert.ToString(Session["Gprocess"]).Trim().Contains("BG"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            }
                        }

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            //if (Session["college_code"].ToString() == "GIM" || Session["college_code"].ToString() == "SMS" || Session["college_code"].ToString() == "HBS" || Session["college_code"].ToString() == "GIS" || Session["college_code"].ToString() == "GSS" || Session["college_code"].ToString() == "GSHS" || Session["college_code"].ToString() == "GSGS")
                            //{

                            //    if (Session["branch_code"].ToString() == "BLENDED" || Session["branch_code"].ToString() == "CHEMISTRY-PFIZER" || Session["branch_code"].ToString() == "CHEMISTRY1" || Session["branch_code"].ToString() == "Optometry" || Session["branch_code"].ToString() == "CSCS" || Session["branch_code"].ToString() == "BCA")
                            //    { }
                            //    else
                            //    {
                            //        string s = Session["Curr_sem"].ToString();
                            //        string b = Session["batch"].ToString();
                            //        if (dr.SEMESTER== "6" && b.Contains("2021-"))
                            //        {
                            //            str = str + "<p style='color:red!important;'>*CGPA will be updated based on Academic Regulations of the programme (After satisfying No. of credits in OE/UC/FC/...)</p>";
                            //        }
                            //        else { }
                            //    }
                            //}
                            str = str + "<div class='note'>";//msr
                            str = str + "<span> <b>Note:</b><br/><span><i>1.This is a digitally generated certificate. The format of this certificate may differ from the";
                            str = str + " document issued by the University<br/></i></span></span>";

                            str = str + "<span><i>2.For any clarification, please contact <a href=\"#\">controllerofexaminations@gitam.edu</a><i></span>";
                        }


                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "</td>";//msr
                            str = str + "<td style =\"width:100px;\">";
                            //str = str + "<div style=\"width:100px;height:200px\"><img src=\"images/DynamicQR.jpg\" width =\"150\" style=\"margin-left:550px\" /></div>";
                            str = str + "<div id='qrcode' class='text-center'></div>";
                            str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";

                            str = str + "<table \" style = \"width:200pxpx; height:31px;display:block;margin-left:470px;margin-top:-55px;font-family: 'Open Sans', sans - serif\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                            str = str + "<tbody>";
                            str = str + "<tr>";
                            str = str + "<td><img src='images/Sumanth-Kumar.png' alt='Snow' style=';display:block;margin-left:100px;height:30px;margin-top:-11px;'></td>";
                            str = str + "</tr>";
                            str = str + "<tr>";
                            str = str + "<td colspan ='3' align=\"right\" style = \"font-size: 15px;\"><b> <span class=\"tempfields\"  style = \"width:200px;margin-top:-2px;\">controller of examinations</span> </b></td>";
                            str = str + "</tr>";
                            str = str + "</tbody>";
                            str = str + "</table>";



                        }
                        else
                        {

                            //str = str + "<div class='sign'><div class='qr'><img src=\"images/DynamicQR.jpg\" width =\"140\" class=\"w-150\" /></div></div>";
                            str = str + "<div id='qrcode' class='text-center'></div>";
                        }
                        str = str + "</tr>";



                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</div>";
                        str = str + "</div></div><br>";
                        str = str + "<div class='break'><br></div>";




                    }
                    data.page = str;
                    data.qr = QCText;

                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public void GenerateMyQCCode(string QCText)
        {
            try
            {
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(QCText);
                string path = Server.MapPath("../images/DynamicQR.jpg");
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }



        [HttpGet]

        public async Task<JsonResult> getProcessDropDwn(string semester)
        {

            //Session["REGDNO"] = "1210116117";
            string userid = Session["uid"].ToString();
            try
            {
                List<studenttrack> details = await iStudentRepository.getprocessdropdown(semester, userid);

                return Json(details, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }








        [HttpGet]

        public async Task<JsonResult> ViewResultGrid1(string QT)
        {

            //Session["REGDNO"] = "1210116117";
            //string userid = Session["uid"].ToString();
            List<studenttrack> carddetails = new List<studenttrack>();
            string str = "";
            try
            {
                if (QT != null)
                {
                    if (QT.Split('$')[0].ToString().Equals("QRCODE"))
                    {
                        String userid = "1210215120";
                        String semester = "2";
                        String month = "";
                        String year = "";
                        String process = "";


                        if (QT.Split('$')[0] != null && QT.Split('$')[1] != null)
                        {
                            userid = QT.Split('$')[1].ToString();
                            semester = QT.Split('$')[2].ToString();
                            month = QT.Split('$')[3].ToString();
                            year = QT.Split('$')[4].ToString();
                            process = QT.Split('$')[5].ToString();
                        }
                        // carddetails = await iStudentRepository.getQrResult(reg, sem, month, process, year);
                        List<studenttrack> details = await iStudentRepository.getmonth(semester, userid, process);
                        if (details.Count() > 0)
                        {

                            carddetails = await iStudentRepository.getcarddetails(userid);
                            Session["Gprocess"] = process;
                            string QCText = "";

                            string campus = "";
                            string college = "";
                            string degree = "";
                            string course = "";
                            string branch = "";
                            string batch = "";

                            QCText = " ";
                            string TY = "QRCODE";
                            string DT_Completion = "";
                           
                            foreach (var dr in carddetails)
                            {
                                //String month = Request.Params["month"].ToString();

                                str = str + "<div class=\"wrapper\"><div class=\"abc img_hover\" style=\"margin-top:30px;\" >";
                                str = str + "<div class=\"innerdiv\">";
                                str = str + "<table   style=\"border-radius: 10px;margin: 10px auto;width: 650px;\" cellspacing=\"0\" cellpadding=\"0\" border = \"0\" >";
                                str = str + "<tbody>";

                                str = str + "<tr>";


                                str = str + "<table width=\"200\" class='sa-width'>";
                                str = str + "<tbody>";
                                str = str + "<tr style=\"text-align:right;\">";

                                str = str + "</tr>";

                                str = str + "</tbody>";
                                str = str + "</table>";


                                str = str + "</tr>";


                                str = str + "<tr>";

                                //str = str + "<br/>";
                                //str = str + "<br/>";
                                //str = str + "<br/>";
                                str = str + "<table width=\"650\">";
                                str = str + "<tbody>";
                                str = str + "<tr style=\"text-align:center;\">";
                                str = str + "<td>  <img src=\"images/academiccert1.jpg\" width =\"650\" class=\"w-700\" /></td>";


                                str = str + "</tr>";

                                str = str + "</tbody>";
                                str = str + "</table>";


                                str = str + "</tr>";




                                str = str + "<tr>";
                                //< p style =\"font-size:12px;\"</p><p style=\"font-size:12px;\"></p>
                                str = str + "<table width=\"650\" style='margin-bottom:20px'>";
                                str = str + "<tbody>";
                                str = str + "<tr style=\"text-align:center;\">";
                                str = str + "<center style='margin-top:10px;'><b style=\"font-size:24px;\"><span style='color:#026B5C'>GRADE CARD</span></b></center>";
                                //str = /*str + "<br/>";*/


                                //str = str + "<center style=\"font-size:18px;\"><b>" + dr[83] + " Semester, " + dr[85] + " " + dr[84] + "</b></center>";

                                if (Convert.ToString(Session["college_code"]).Equals("GPP") || dr.BRANCH == "Optometry")
                                {
                                    str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.Branch_name + " - Degree Examination</b></center>";

                                }
                                else
                                {
                                    str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.COURSE + " Degree Examination</b></center>";
                                }
                                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    if (dr.COURSE.Trim().ToUpper().Equals("MBA") || dr.COURSE.Trim().ToUpper().Equals("M.B.A"))
                                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                                    else
                                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Year, " + dr.Month + " " + dr.Year + "</b></center>";

                                }
                                else
                                {
                                    if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                                    {
                                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Trimester, " + dr.Month + " " + dr.Year + "</b></center>";
                                    }
                                    else if (Convert.ToInt32(dr.SEMESTER) >= 21)
                                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + ", " + dr.Month + " " + dr.Year + "</b></center>";
                                    else
                                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                                }
                                str = str + "</tr>";

                                str = str + "</tbody>";
                                str = str + "</table>";

                                str = str + "</tr>";



                                str = str + "<tr>";

                                str = str + "<table width=\"650\" class='mt-1 sa-width details' style='margin-bottom:20px'>";
                                str = str + "<tbody>";
                                //str = str + "<tr>"; str = str + "<td  width=\"650\">"; str = str + "<table width=\"550\" class='mt-1 sa-width details' style='margin-bottom:20px'>"; str = str + "<tbody>";//msr
                                str = str + "<tr>";
                                str = str + "<td style=\"width:50px;\"> <span>Regd.No</span></td>";
                                str = str + "<td>:</td>";
                                str = str + "<td style='text-align:left'> &nbsp;" + dr.regdno + "</td>";
                                str = str + "<tr>";
                                str = str + "<td style=\"width:50px;\"> <span>Name </span></td>";
                                str = str + "<td>:</td>";
                                str = str + "<td style='text-align:left'> &nbsp;" + dr.name + "</td>";
                                str = str + "<td> &nbsp;</td>";
                                str = str + "<td> &nbsp;</td>";
                                str = str + "</tr>";
                                if (!Convert.ToString(Session["college_code"]).Equals("GPP"))
                                {
                                    str = str + "<tr>";
                                    str = str + "<td style=\"width:50px;\"> <span>Branch</span></td>";
                                    str = str + "<td> :</td>";
                                    str = str + "<td style='text-align:left'> &nbsp;" + dr.Branch_name + "</td>";
                                    str = str + "<td> &nbsp;</td>";
                                    str = str + "<td> &nbsp;</td>";
                                    str = str + "</tr>";
                                }

                                if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                                {
                                    string gppname = "Kautilya School of Public Policy";
                                    str = str + "<tr>";
                                    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                                    str = str + "<td> :</td>";
                                    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                                    str = str + "<td> &nbsp;</td>";
                                    str = str + "<td> &nbsp;</td>";
                                    str = str + "</tr>";
                                }
                                //else if(Session["phdcoursecode"].ToString().Equals("PHD"))
                                //{
                                //    string gppname = "Inistitute of Technology";
                                //    str = str + "<tr>";
                                //    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                                //    str = str + "<td> :</td>";
                                //    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                                //    str = str + "<td> &nbsp;</td>";
                                //    str = str + "<td> &nbsp;</td>";
                                //    str = str + "</tr>";
                                //}


                                str = str + "</tbody>";
                                str = str + "</table>";



                                str = str + "</tr>";

                                str = str + "<tr>";


                                //str = str + "<div class=\"b12121\">";
                                //height =\"350\"
                                str = str + "<table width=\"650\" class=\"tab mt-2 mb-2 sa-width\">";
                                str = str + "<tbody>";
                                str = str + "<tr class=\"\" style='border:2px solid #026B5C'>";
                                str = str + "<td style=\"text-align:left;padding:4px;width:105px;\"> <span style='color:#026B5C;'><b>Course Code</b></span> </td>";
                                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Name of the Course</b></span> </td>";

                                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Credits</b> </span></td>";
                                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Grade</b> </span></td>";

                                str = str + "</tr>";






                                if (dr.SUB1_CODE != null)
                                {
                                    if (dr.SUB1_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB1_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB1_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB2_CODE != null)
                                {
                                    if (dr.SUB2_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB2_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB2_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB3_CODE != null)
                                {
                                    if (dr.SUB3_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB3_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB3_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB4_CODE != null)
                                {
                                    if (dr.SUB4_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB4_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB4_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB5_CODE != null)
                                {
                                    if (dr.SUB5_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB5_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB5_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB6_CODE != null)
                                {
                                    if (dr.SUB6_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB6_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB6_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB7_CODE != null)
                                {
                                    if (dr.SUB7_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB7_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB7_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB8_CODE != null)
                                {
                                    if (dr.SUB8_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB8_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB8_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB9_CODE != null)
                                {
                                    if (dr.SUB9_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB9_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB9_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB10_CODE != null)
                                {
                                    if (dr.SUB10_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB10_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB10_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB11_CODE != null)
                                {
                                    if (dr.SUB11_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB11_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB11_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB12_CODE != null)
                                {
                                    if (dr.SUB12_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB12_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB12_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB13_CODE != null)
                                {
                                    if (dr.SUB13_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB13_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB13_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB14_CODE != null)
                                {
                                    if (dr.SUB14_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB14_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB14_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }
                                if (dr.SUB15_CODE != null)
                                {
                                    if (dr.SUB15_CODE != "")
                                    {
                                        str = str + "<tr>";
                                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB15_CODE.ToString() + "</p></td>";
                                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB15_NAME.ToString() + " </p></td>";

                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_CREDITS.ToString() + " </p></td>";
                                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_GRADE.ToString() + " </p></td>";

                                        str = str + "</tr>";


                                    }
                                }

                                str = str + "</tbody>";
                                str = str + "</table>";

                                str = str + "</tr>";


                                str = str + "<tr>";


                                str = str + "<div class='sgpa'><table><tbody><tr>";

                                if (dr.sgpa == "0" && dr.cgpa == "0")
                                {
                                    //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[12] + " <br/></td></tr><tr>";
                                    //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[13] + " <br/></td>";
                                    str = str + "<td> <p style =\"font-size:15px;\">SGPA </p></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                                    str = str + "<td> <p style =\"font-size:15px;\">CGPA </p></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                                    str = str + "</tr></tbody></table></div>";
                                }
                                else
                                {
                                    //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[86] + " <br/></td></tr><tr>";
                                    //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[87] + " <br/></td>";
                                    str = str + "<td> <span style =\"font-size:15px;\">SGPA </span></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                                    str = str + "<td> <span style =\"font-size:15px;\">CGPA </span></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                                    str = str + "</tr></tbody></table></div>";
                                }

                                str = str + "</tr>";
                                str = str + "<div class=\"b6\">";
                                //str = str + "<span> <b>Printed On:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "</td>";
                                //str = str + "<br/></span>";

                                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    str = str + "<span> <b>Printed On&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                                    str = str + "<br/></span>";
                                }

                                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    str = str + "<span> <b style =\"font-size:15px;\">Printed On&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                                    str = str + "<br/><br/></span>";
                                    str = str + "<span style =\"font-size:12px;\"><b>Mode of delivery&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;ODL";
                                    str = str + "<br/></span>";
                                    str = str + "<span style =\"font-size:12px;\"><b>Date of Admission&nbsp; :</b>&nbsp;" + Convert.ToString(Session["AdmissionDate"]) + "";
                                    str = str + "<br/></span>";
                                    str = str + "<span style =\"font-size:12px;\"><b>Date of completion :</b>&nbsp;" + DT_Completion + "";
                                    str = str + "<br/></span>";
                                    str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Learner Support Centres :&nbsp;---</b>" + "";
                                    str = str + "<br/></span>";
                                    str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Examination Centres&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;GITAM (deemed to beUniversity), VISAKHAPATNAM.";
                                    str = str + "<br/></span>";
                                }



                                str = str + "</div>";
                                str = str + "<div class=\"b5\">";
                                str = str + "<br/>";
                                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    // if(Session["Gprocess"].ToString().Trim().Equals("SRV"))
                                    // {
                                    //     //str = str + "<div style=\"width:600px;\"><b>''Certificate issued after the publication of supplimentary revaluation results''</b></div>";
                                    // }
                                    //else
                                    //if (Session["Gprocess"].ToString().Trim().Contains("RV"))
                                    //{
                                    //    str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                                    //}
                                    //else if (Session["Gprocess"].ToString().Trim().Contains("BG"))
                                    //{
                                    //str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                                    //}
                                    if (Convert.ToString(Session["Gprocess"]).Trim().Contains("RV"))
                                    {
                                        str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                                    }
                                    else if (Convert.ToString(Session["Gprocess"]).Trim().Contains("BG"))
                                    {
                                        str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                                    }
                                }

                                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    str = str + "<table style =\"width:650px;margin-left: 0px;\" class=\"w-700\"><tbody><tr><td style =\"width:450px;\" class=\"w-550\">";//msr
                                    str = str + "<span style =\"width:100px;\"> <b>Note:</b><br/><span style =\"font-size:12px;\"><i>1.This is a digitally generated certificate. The format of this certificate may differ from the";
                                    str = str + " document issued by the University<br/></i></span></span>";

                                    str = str + "<span style =\"width:100px;font-size:12px;\"><i>2.For any clarification, please contact <a href=\"#\">controllerofexaminations@gitam.edu</a><i></span>";
                                }

                                // if (Session["College"].ToString().Equals("CDL"))
                                //   str = str + "<span style =\"width:100px;font-size:12px;\"><i><br/>3.Mode of Delivery ODL<i></span>";
                                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                                {
                                    str = str + "</td>";//msr
                                    str = str + "<td style =\"width:100px;\">";
                                    str = str + "<div style=\"width:100px;height:200px\"><img src=\"images/DynamicQR.jpg\" width =\"150\" style=\"margin-left:550px\" /></div>";
                                    str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";

                                    str = str + "<table \" style = \"width:200pxpx; height:31px;display:block;margin-left:470px;margin-top:-55px;font-family: 'Open Sans', sans - serif\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                                    str = str + "<tbody>";
                                    str = str + "<tr>";
                                    str = str + "<td><img src='images/Sumanth-Kumar.png' alt='Snow' style=';display:block;margin-left:100px;height:30px;margin-top:-11px;'></td>";
                                    str = str + "</tr>";
                                    str = str + "<tr>";
                                    str = str + "<td colspan ='3' align=\"right\" style = \"font-size: 15px;\"><b> <span class=\"tempfields\"  style = \"width:200px;margin-top:-2px;\">controller of examinations</span> </b></td>";
                                    str = str + "</tr>";
                                    str = str + "</tbody>";
                                    str = str + "</table>";



                                }
                                else
                                {
                                    str = str + "</td>";//msr
                                    str = str + "<td style =\"width:100px;\" class=\"w-150\">";
                                    str = str + "<div style=\"width:100px;\" class=\"w-150\"><img src=\"images/DynamicQR.jpg\" width =\"140\"  class=\"w-150\" /></div>";
                                    str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";



                                }


                                str = str + "</tr>";



                                str = str + "</tbody>";
                                str = str + "</table>";
                                str = str + "</div>";
                                str = str + "</div></div><br>";
                                str = str + "<div class='break'><br></div>";



                            }

                        }



                    }

                }
                carddetails.ElementAt(0).htmltext = str;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(str, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]

        public async Task<ActionResult> ViewResultGrid(string QT)
        {

            List<studenttrack> carddetails = new List<studenttrack>();
            gradecardsummary data = new gradecardsummary();
            try
            {
                if (QT != null)
                {
                    if (QT.Split('$')[0].ToString().Equals("QRCODE"))
                    {
                        String reg = "1210215120";
                        String sem = "2";
                        String month = "";
                        String year = "";
                        String process = "";
                        string QCText = "";
                        string str = "";
                        string campus = "";
                        string college = "";
                        string degree = "";
                        string course = "";
                        string branch = "";
                        string batch = "";

                        if (QT.Split('$')[0] != null && QT.Split('$')[1] != null)
                        {
                            reg = QT.Split('$')[1].ToString();
                            sem = QT.Split('$')[2].ToString();
                            month = QT.Split('$')[3].ToString();
                            year = QT.Split('$')[4].ToString();
                            process = QT.Split('$')[5].ToString();
                        }

                        try
                        {
                            List<studenttrack> records = await iStudentRepository.getStudentcarddetails(sem, reg);
                            List<studenttrack> details = await iStudentRepository.getmonth(sem, reg, process);
                            month = month;
                            year = year;

                            if (records.Count() > 0)
                            {
                                campus = records.FirstOrDefault().CAMPUS_CODE;
                                college = records.FirstOrDefault().college_code;
                                degree = records.FirstOrDefault().DEGREE_CODE;
                                course = records.FirstOrDefault().COURSE_CODE;
                                branch = records.FirstOrDefault().BRANCH_CODE;
                                batch = records.FirstOrDefault().BATCH;


                            }

                            int delete = await iStudentRepository.DeleteDetails(campus, college, course, branch, batch);
                            List<studenttrack> GradeResult = await iStudentRepository.getGradeResult(sem, reg, month, year, process);

                            studenttrack detailsdata = new studenttrack()
                            {
                                regdno = GradeResult.FirstOrDefault().regdno,
                                name = GradeResult.FirstOrDefault().name,
                                BRANCH = GradeResult.FirstOrDefault().BRANCH_CODE,
                                COURSE = GradeResult.FirstOrDefault().COURSE_CODE,
                                DEGREE_CODE = GradeResult.FirstOrDefault().DEGREE_CODE,
                                college_code = GradeResult.FirstOrDefault().college_code,
                                CAMPUS_CODE = GradeResult.FirstOrDefault().CAMPUS_CODE,
                                course_name = GradeResult.FirstOrDefault().course_name,
                                Branch_name = GradeResult.FirstOrDefault().Branch_name,
                                section = GradeResult.FirstOrDefault().section,
                                COURSE_TITLE = GradeResult.FirstOrDefault().COURSE_TITLE,
                                PRINT_YEAR = GradeResult.FirstOrDefault().PRINT_YEAR,
                                EXAM_NAME = GradeResult.FirstOrDefault().EXAM_NAME,
                                exam_date = GradeResult.FirstOrDefault().exam_date,
                                Month = month,
                                Year = GradeResult.FirstOrDefault().Year,
                                BATCH = GradeResult.FirstOrDefault().BATCH,
                                process_type = process,
                                sgpa = GradeResult.FirstOrDefault().sgpa,
                                cgpa = GradeResult.FirstOrDefault().cgpa,
                                TOTAL_SEM_CREDITS = GradeResult.FirstOrDefault().TOTAL_SEM_CREDITS,
                                CUM_SEM_CREDITS = GradeResult.FirstOrDefault().CUM_SEM_CREDITS,
                                SEMESTER = sem,



                            };

                            var insertdetails = await iStudentRepository.InsertGradedetails(detailsdata);

                            List<studenttrack> detailsmore = await iStudentRepository.getGradeReports(detailsdata);
                            List<studenttrack> grademarks = new List<studenttrack>();
                            if (detailsmore != null)
                            {
                                if (detailsmore.Count() > 0)
                                {
                                    for (int i = 0; i < detailsmore.Count(); i++)
                                    {
                                        studenttrack grades = new studenttrack()
                                        {
                                            regdno = GradeResult.FirstOrDefault().regdno,
                                            SUBJECT_CODE = detailsmore.ElementAt(i).SUBJECT_CODE,
                                            SUBJECT_NAME = detailsmore.ElementAt(i).SUBJECT_NAME,
                                            CREDITS1 = detailsmore.ElementAt(i).CREDITS1,
                                            Grade = detailsmore.ElementAt(i).Grade,
                                            SEMESTER = detailsmore.ElementAt(i).SEMESTER,
                                            process_type = process,
                                            Month = GradeResult.FirstOrDefault().Month,
                                            Year = GradeResult.FirstOrDefault().Year,
                                            CAMPUS_CODE = campus
                                        };
                                        grademarks.Add(grades);
                                    }

                                }
                                var updatedetails = await iStudentRepository.updateGradedetails(grademarks);
                            }

                            if (details.Count() > 0)
                            {

                                carddetails = await iStudentRepository.getcarddetails(reg);



                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("ViewResultGrid", carddetails);
        }


        //[HttpGet]

        //public async Task<ActionResult> ViewResultGrid(string QT)
        //{

        //    //Session["REGDNO"] = "1210116117";
        //    //string userid = Session["uid"].ToString();
        //    List<studenttrack> carddetails = new List<studenttrack>();
        //    try
        //    {
        //        if (QT != null)
        //        {
        //            if (QT.Split('$')[0].ToString().Equals("QRCODE"))
        //            {
        //                String reg = "1210215120";
        //                String sem = "2";
        //                String month = "";
        //                String year = "";
        //                String process = "";


        //                if (QT.Split('$')[0] != null && QT.Split('$')[1] != null)
        //                {
        //                    reg = QT.Split('$')[1].ToString();
        //                    sem = QT.Split('$')[2].ToString();
        //                    month = QT.Split('$')[3].ToString();
        //                    year = QT.Split('$')[4].ToString();
        //                    process = QT.Split('$')[5].ToString();
        //                }
        //                carddetails = await iStudentRepository.getQrResult(reg, sem, month, process, year);

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return PartialView("ViewResultGrid", carddetails);
        //}

        //public async Task<ActionResult> DegreePlan()
        //{
        //    return View("DegreePlan");
        //}

        //public async Task<ActionResult> Feedback()
        //{
        //    IEnumerable<FeedbackMaster> questionslist = null;
        //    string REGDNO = Session["uid"].ToString();
        //    string sem = Session["curr_sem"].ToString();
        //    IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem);
        //    questionslist = await iStudentRepository.getquestionlist();
        //    Session["msgfeedback"] = "";
        //    ViewBag.subjectlist = subjectlist.Select(testgroup => new SelectListItem
        //    {
        //        Text = testgroup.SUBJECT_CODE.ToString() + "#" + testgroup.SUBJECT_NAME.ToString(),
        //        Value = testgroup.GROUP_CODE.ToString()
        //    });

        //    return View("Dynamicfeedback", questionslist);
        //}

        //[HttpPost]
        //public async Task<JsonResult> Getfeedbackfaculty(string groupcode)
        //{
        //    IEnumerable<studenttrack> list = await iStudentRepository.getfeedback_facultyasync(groupcode);
        //    return Json(list);
        //}



        /*   [HttpGet]
           public async Task<ActionResult> GetFeedbackSubjects()
           {
               FeedbackCheckSummary summary = new FeedbackCheckSummary();
               List<FeedbackMaster> data = new List<FeedbackMaster>();
               List<FeedbackMaster> emp = new List<FeedbackMaster>();
               string REGDNO = Session["uid"].ToString();
               string sem = Session["curr_sem"].ToString();
               IEnumerable<FeedbackMaster> list = await iStudentRepository.getfeedback_subjects(REGDNO, sem);
               var group = "";

               IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem);

               foreach (var val in subjectlist)
               {
                   List<FeedbackMaster> newdata = new List<FeedbackMaster>();
                   IEnumerable<studenttrack> list1 = await iStudentRepository.getfeedback_facultyasync(val.GROUP_CODE, Session["campus_code"].ToString(), Session["branch_code"].ToString());


                   foreach (var val1 in list1)
                   {
                       var data1 = list.Where(m => m.empid == val1.EMPID && m.subject_code == val1.SUBJECT_CODE).FirstOrDefault();
                       if (data1 != null)
                       {
                           data.Add(data1);
                           newdata.Add(data1);
                       }


                   }
                   if (list1.Count() > 1)
                   {
                       if (list1.Count() > newdata.Count())
                       {
                           foreach (var data12 in newdata)
                           {
                               emp.Add(data12);
                           }

                       }
                   }
               }

               summary.emp = emp;
               summary.list = list.ToList();


               return Json(summary, JsonRequestBehavior.AllowGet);
           }
           [HttpPost]
           public async Task<JsonResult> Getfeedbackfaculty(string groupcode)
           {
               FeedbackCheckEmpSummary summary = new FeedbackCheckEmpSummary();
               List<FeedbackMaster> data = new List<FeedbackMaster>();
               List<FeedbackMaster> emp = new List<FeedbackMaster>();
               IEnumerable<studenttrack> listFAC = await iStudentRepository.getfeedback_facultyasync(groupcode, Session["campus_code"].ToString(), Session["branch_code"].ToString());
               string REGDNO = Session["uid"].ToString();
               string sem = Session["curr_sem"].ToString();
               foreach (var val in listFAC)
               {
                   IEnumerable<FeedbackMaster> subjectlist = await iStudentRepository.getfeedback_subjects(REGDNO, sem);
                   var data1 = subjectlist.Where(m => m.subject_code == val.SUBJECT_CODE && m.empid == val.EMPID).FirstOrDefault();

                   if (data1 != null)
                   {
                       emp.Add(data1);
                   }

               }




               summary.emp = emp;
               summary.list = listFAC.ToList();


               return Json(summary);
           }





           [HttpGet]
           public async Task<ActionResult> Getquestions(string facultyname, string subject, string faculty, string subjectname, string feedbacksession)
           {
               IEnumerable<FeedbackMaster> questionslist = null;
               string REGDNO = Session["uid"].ToString();
               string sem = Session["curr_sem"].ToString();
               string[] subjectnamesplit = new string[1000];
               subjectnamesplit = subjectname.Split('#');
               string subjectcode = subjectnamesplit[0];
               string subjectrealname = subjectnamesplit[1];
               Session["Subject1"] = subjectcode;
               Session["SubjectName1"] = subjectrealname;
               Session["faculty"] = faculty;
               Session["facultyname"] = facultyname;
               Session["feedbacksession"] = feedbacksession;
               try
               {
                   questionslist = await iStudentRepository.getquestionlist();
                   IEnumerable<FeedbackMaster> checkit = await iStudentRepository.getfeedback_check(subjectcode, faculty, feedbacksession, REGDNO, sem);
                   if (checkit.Count() > 0)
                   {
                       questionslist.FirstOrDefault().msg = "You have already given feedback to this subject";
                       Session["msgfeedback"] = "You have already given feedback to this subject";
                   }
                   else
                   {
                       questionslist.FirstOrDefault().msg = "you are eligible to give feedback";
                       Session["msgfeedback"] = "you are eligible to give feedback";
                   }
               }
               catch (Exception ex)
               {
                   throw ex;
               }
               return PartialView("Dynamicfeedbackview", questionslist);
           }


           [HttpPost]
           public async Task<ActionResult> createquestion(FormCollection collection)
           {
               dynamic showMessageString = string.Empty;
               try
               {

                   List<FeedbackMaster> list = new List<FeedbackMaster>();
                   FeedbackMaster datasingle = new FeedbackMaster();
                   string[] count = collection.GetValues("count");
                   string groupcode = "";
                   IEnumerable<FeedbackMaster> groupcodelist = await iStudentRepository.get_groupcode(Session["campus_code"].ToString(), Session["college_code"].ToString(), Session["course_code"].ToString(), Session["branch_code"].ToString(), Session["curr_sem"].ToString(), Session["Subject1"].ToString(), Session["section"].ToString());
                   if(groupcodelist.Count() > 0)
                   {
                        groupcode = groupcodelist.FirstOrDefault().group_code;
                   }

                   for (int i = 0; i <= (Convert.ToInt32(count[0]) - 1); i++)
                   {

                       string[] feedbackradio = collection.GetValues("ctl00$MainContent$ID_" + i);
                       string[] feedbackqus = collection.GetValues("NATURE_" + i);
                       string[] feedbacksino = collection.GetValues("ID_" + i);
                       string[] feedbackmaxmarks = collection.GetValues("maxmarks_" + i);
                       string[] Q_STATUS = collection.GetValues("Q_STATUS_" + i);
                       list.Add(new FeedbackMaster
                       {
                           FEEDBACK1 = feedbackradio[0],
                           NATURE = feedbacksino[0],
                           ID = feedbackqus[0],
                           maxmarks = feedbackmaxmarks[0],
                           regdno = Session["uid"].ToString(),
                           semester = Session["curr_sem"].ToString(),
                           stuname = Session["studname"].ToString(),
                           subject_code = Session["Subject1"].ToString(),
                           empid = Session["faculty"].ToString(),
                           empname = Session["facultyname"].ToString(),
                           dept_code = Session["dept_code"].ToString(),
                           college_code = Session["college_code"].ToString(),
                           subjectname = Session["SubjectName1"].ToString(),
                           section = Session["section"].ToString(),
                           batch = Session["batch"].ToString(),
                           branch_code = Session["branch_code"].ToString(),
                           campus_code = Session["campus_code"].ToString(),
                           feedbacksession = Session["feedbacksession"].ToString(),
                           course_code = Session["course_code"].ToString(),
                           Q_STATUS = Q_STATUS[0],
                           group_code = groupcode
                       });
                   }

                   var result = await iStudentRepository.Insertfeedbacknew(list);
                   if (result.FirstOrDefault().flag == "success")
                   {
                       showMessageString = new
                       {
                           param1 = 100,
                           param2 = "Successfully submitted",
                       };

                   }
                   else
                   {
                       showMessageString = new
                       {
                           param1 = 100,
                           param2 = "oops,something went wrong",
                       };
                   }


               }
               catch (Exception ex)
               {
                   throw ex;
               }

               return Json(showMessageString, JsonRequestBehavior.AllowGet);
           }

           */


        /*
                public async Task<ActionResult> Feedback()
                {
                    IEnumerable<FeedbackMaster> questionslist = null;
                    string REGDNO = Session["uid"].ToString();
                    string sem = Session["curr_sem"].ToString();
                    string sem1 = "";
                    string batch = Session["batchstart"].ToString();

                    if (batch == "2023") { sem1 = "1"; }
                    else
                    {
                        sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    }

                    IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem1);
                    questionslist = await iStudentRepository.getquestionlist();
                    Session["msgfeedback"] = "";
                    ViewBag.subjectlist = subjectlist.Select(testgroup => new SelectListItem
                    {
                        Text = testgroup.SUBJECT_CODE.ToString() + "#" + testgroup.SUBJECT_NAME.ToString(),
                        Value = testgroup.GROUP_CODE.ToString()
                    });

                    return View("Dynamicfeedback", questionslist);
                }


                [HttpGet]
                public async Task<ActionResult> GetFeedbackSubjects()
                {
                    FeedbackCheckSummary summary = new FeedbackCheckSummary();
                    List<FeedbackMaster> data = new List<FeedbackMaster>();
                    List<FeedbackMaster> emp = new List<FeedbackMaster>();
                    string REGDNO = Session["uid"].ToString();
                    string sem = Session["curr_sem"].ToString();
                    string batch = Session["batchstart"].ToString();
                    string sem1 = "";
                    if (batch == "2023") { sem1 = "1"; }
                    else
                    {
                        sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    }
                    IEnumerable<FeedbackMaster> list = await iStudentRepository.getfeedback_subjects(REGDNO, sem1);
                    var group = "";

                    IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem1);

                    foreach (var val in subjectlist)
                    {
                        List<FeedbackMaster> newdata = new List<FeedbackMaster>();
                        IEnumerable<studenttrack> list1 = await iStudentRepository.getfeedback_facultyasync(val.GROUP_CODE, Session["campus_code"].ToString(), Session["branch_code"].ToString());


                        foreach (var val1 in list1)
                        {
                            var data1 = list.Where(m => m.empid == val1.EMPID && m.subject_code == val1.SUBJECT_CODE).FirstOrDefault();
                            if (data1 != null)
                            {
                                data.Add(data1);
                                newdata.Add(data1);
                            }


                        }
                        if (list1.Count() > 1)
                        {
                            if (list1.Count() > newdata.Count())
                            {
                                foreach (var data12 in newdata)
                                {
                                    emp.Add(data12);
                                }

                            }
                        }
                    }

                    summary.emp = emp;
                    summary.list = list.ToList();


                    return Json(summary, JsonRequestBehavior.AllowGet);
                }
                [HttpPost]
                public async Task<JsonResult> Getfeedbackfaculty(string groupcode)
                {
                    FeedbackCheckEmpSummary summary = new FeedbackCheckEmpSummary();
                    List<FeedbackMaster> data = new List<FeedbackMaster>();
                    List<FeedbackMaster> emp = new List<FeedbackMaster>();
                    IEnumerable<studenttrack> listFAC = await iStudentRepository.getfeedback_facultyasync(groupcode, Session["campus_code"].ToString(), Session["branch_code"].ToString());
                    string REGDNO = Session["uid"].ToString();
                    string sem = Session["curr_sem"].ToString();
                    foreach (var val in listFAC)
                    {
                        IEnumerable<FeedbackMaster> subjectlist = await iStudentRepository.getfeedback_subjects(REGDNO, sem);
                        var data1 = subjectlist.Where(m => m.subject_code == val.SUBJECT_CODE && m.empid == val.EMPID).FirstOrDefault();

                        if (data1 != null)
                        {
                            emp.Add(data1);
                        }

                    }




                    summary.emp = emp;
                    summary.list = listFAC.ToList();


                    return Json(summary);
                }

                [HttpGet]
                public async Task<ActionResult> Getquestions(string facultyname, string subject, string faculty, string subjectname, string feedbacksession,string groupcode)
                {
                    IEnumerable<FeedbackMaster> questionslist = null;
                    string REGDNO = Session["uid"].ToString();
                    string sem = Session["curr_sem"].ToString();
                    string[] subjectnamesplit = new string[1000];
                    subjectnamesplit = subjectname.Split('#');
                    string subjectcode = subjectnamesplit[0];
                    string subjectrealname = subjectnamesplit[1];
                    Session["Subject1"] = subjectcode;
                    Session["SubjectName1"] = subjectrealname;
                    Session["faculty"] = faculty;
                    Session["facultyname"] = facultyname;
                    Session["feedbacksession"] = feedbacksession;
                    if(groupcode != null)
                    {
                        Session["groupcode"] = groupcode;
                    }
                    else
                    {
                        Session["groupcode"] = "";
                    }
                    try
                    {
                        questionslist = await iStudentRepository.getquestionlist();
                        IEnumerable<FeedbackMaster> checkit = await iStudentRepository.getfeedback_check(subjectcode, faculty, feedbacksession, REGDNO, sem);
                        if (checkit.Count() > 0)
                        {
                            questionslist.FirstOrDefault().msg = "You have already given feedback to this subject";
                            Session["msgfeedback"] = "You have already given feedback to this subject";
                        }
                        else
                        {
                            questionslist.FirstOrDefault().msg = "you are eligible to give feedback";
                            Session["msgfeedback"] = "you are eligible to give feedback";
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return PartialView("Dynamicfeedbackview", questionslist);
                }


                [HttpPost]
                public async Task<ActionResult> createquestion(FormCollection collection)
                {
                    dynamic showMessageString = string.Empty;
                    try
                    {

                        List<FeedbackMaster> list = new List<FeedbackMaster>();
                        FeedbackMaster datasingle = new FeedbackMaster();
                        string[] count = collection.GetValues("count");
                        string groupcode = "";
                        //IEnumerable<FeedbackMaster> groupcodelist = await iStudentRepository.get_groupcode(Session["campus_code"].ToString(), Session["college_code"].ToString(), Session["course_code"].ToString(), Session["branch_code"].ToString(), (Convert.ToInt32(Session["curr_sem"]) - 1).ToString(), Session["Subject1"].ToString(), Session["section"].ToString());
                        //if (groupcodelist.Count() > 0)
                        //{
                        //    groupcode = groupcodelist.FirstOrDefault().group_code;
                        //}
                        string batch = Session["batchstart"].ToString();
                        string sem1 = "";
                        if (batch == "2023") { sem1 = "1"; }
                        else
                        {
                            sem1 = (Convert.ToInt32(Session["curr_sem"].ToString()) - 1).ToString();
                        }
                        for (int i = 0; i <= (Convert.ToInt32(count[0]) - 1); i++)
                        {

                            string[] feedbackradio = collection.GetValues("ctl00$MainContent$ID_" + i);
                            string[] feedbackqus = collection.GetValues("NATURE_" + i);
                            string[] feedbacksino = collection.GetValues("ID_" + i);
                            string[] feedbackmaxmarks = collection.GetValues("maxmarks_" + i);
                            string[] Q_STATUS = collection.GetValues("Q_STATUS_" + i);
                            list.Add(new FeedbackMaster
                            {
                                FEEDBACK1 = feedbackradio[0],
                                NATURE = feedbacksino[0],
                                ID = feedbackqus[0],
                                maxmarks = feedbackmaxmarks[0],
                                regdno = Session["uid"].ToString(),


                                semester = sem1,
                                stuname = Session["studname"].ToString(),
                                subject_code = Session["Subject1"].ToString(),
                                empid = Session["faculty"].ToString(),
                                empname = Session["facultyname"].ToString(),
                                dept_code = Session["dept_code"].ToString(),
                                college_code = Session["college_code"].ToString(),
                                subjectname = Session["SubjectName1"].ToString(),
                                section = Session["section"].ToString(),
                                batch = Session["batch"].ToString(),
                                branch_code = Session["branch_code"].ToString(),
                                campus_code = Session["campus_code"].ToString(),
                                feedbacksession = Session["feedbacksession"].ToString(),
                                course_code = Session["course_code"].ToString(),
                                Q_STATUS = Q_STATUS[0],
                                group_code = Session["groupcode"].ToString()
                            });
                        }

                        var result = await iStudentRepository.Insertfeedbacknew(list);
                        if (result.FirstOrDefault().flag == "success")
                        {
                            showMessageString = new
                            {
                                param1 = 100,
                                param2 = "Successfully submitted",
                            };

                        }
                        else
                        {
                            showMessageString = new
                            {
                                param1 = 100,
                                param2 = "oops,something went wrong",
                            };
                        }


                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return Json(showMessageString, JsonRequestBehavior.AllowGet);
                }

                */

        public async Task<ActionResult> Feedback()
        {
            Session["groupcode"] = "";
            IEnumerable<FeedbackMaster> questionslist = null;
            string REGDNO = Session["uid"].ToString();
            string sem = Session["curr_sem"].ToString();
            string sem1 = "";
            string batch = Session["batchstart"].ToString();
            string branchcode = Session["branch_code"].ToString();
            if (batch == "2024" && sem=="1") { sem1 = "1"; }
            else
            {
                if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" || Session["degree_code"].ToString() == "UG"))
                {
                    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                }
                else
                {
                    sem1 = sem;
                }
                //if (Session["college_code"].ToString() == "HBS" && Session["campus_code"].ToString() == "HYD" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                //{
                //    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //}
                //else if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                //{
                //    sem1 = "33333";
                //}
                // sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //  else
                ////sem1 = sem;
            }

            IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem1);
            questionslist = await iStudentRepository.getquestionlist();
            Session["msgfeedback"] = "";
            ViewBag.subjectlist = subjectlist.Select(testgroup => new SelectListItem
            {
                Text = testgroup.SUBJECT_CODE.ToString() + "#" + testgroup.SUBJECT_NAME.ToString(),

                Value = testgroup.SUBJECT_CODE.ToString()
            });

            return View("Dynamicfeedback", questionslist);
        }
        [HttpGet]
        public async Task<ActionResult> GetFeedbackSubjects()
        {
            FeedbackCheckSummary summary = new FeedbackCheckSummary();
            List<FeedbackMaster> data = new List<FeedbackMaster>();
            List<FeedbackMaster> emp = new List<FeedbackMaster>();
            string REGDNO = Session["uid"].ToString();
            string sem = Session["curr_sem"].ToString();
            string batch = Session["batchstart"].ToString();
            string sem1 = "";

            //if (batch == "2024" && sem == "1") { sem1 = "1"; }
            //else
            //{
                //if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" || Session["degree_code"].ToString() == "UG"))
                //{
                //    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //}

                if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" ))
                {
                    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                }
                else
                {
                    sem1 = sem;
                }

                //if(Session["college_code"].ToString()=="HBS" && Session["campus_code"].ToString()=="HYD" && Session["degree_code"].ToString() =="PG" &&Session["branch_code"].ToString()=="MBA" && Session["Curr_sem"].ToString()=="3")
                //{
                ////sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //}
                //else if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                //{
                //    sem1 = "33333";
                //}
                // sem1 = (Convert.ToInt32(sem) - 1).ToString();
                // else
                ////sem1 = sem;
           // }
            IEnumerable<FeedbackMaster> list = await iStudentRepository.getfeedback_subjects(REGDNO, sem1);
            var group = "";

            IEnumerable<studenttrack> subjectlist = await iStudentRepository.getfeedback_subjectsasync(REGDNO, sem1);
            IEnumerable<studenttrack> list1 = null;
            foreach (var val in subjectlist)
            {
                List<FeedbackMaster> newdata = new List<FeedbackMaster>();

                if (sem1 == "2" || sem1 == "4" || sem1 == "6" || sem1 == "8" || sem1 == "10")
                {
                    list1 = await iStudentRepository.getfeedback_facultyasyncnew(val.GROUP_CODE, Session["campus_code"].ToString(), Session["branch_code"].ToString(), REGDNO, val.SUBJECT_CODE, sem1, Session["college_code"].ToString(), batch);


                }
                else
                {
                    //list1 = await iStudentRepository.getfeedback_facultyasync(val.GROUP_CODE, Session["campus_code"].ToString(), Session["branch_code"].ToString());
                    // getfeedback_facultyasyncnew
                    list1 = await iStudentRepository.getfeedback_facultyasyncnew(val.GROUP_CODE, Session["campus_code"].ToString(), Session["branch_code"].ToString(), REGDNO, val.SUBJECT_CODE, sem1, Session["college_code"].ToString(), batch);

                }
                foreach (var val1 in list1)
                {
                    var data1 = list.Where(m => m.empid == val1.EMPID && m.subject_code == val1.SUBJECT_CODE &&m.feedbacksession=="2").FirstOrDefault();
                    if (data1 != null)
                    {
                        data.Add(data1);
                        newdata.Add(data1);
                    }


                }
                if (list1.Count() > 1)
                {
                    if (list1.Count() > newdata.Count())
                    {
                        foreach (var data12 in newdata)
                        {
                            emp.Add(data12);
                        }

                    }
                }
            }

            summary.emp = emp;
            summary.list = list.ToList();


            return Json(summary, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<JsonResult> Getfeedbackfaculty(string subject_code)
        {
            FeedbackCheckEmpSummary summary = new FeedbackCheckEmpSummary();
            List<FeedbackMaster> data = new List<FeedbackMaster>();
            List<FeedbackMaster> emp = new List<FeedbackMaster>();
            IEnumerable<studenttrack> listFAC = null;
            string batch = Session["batchstart"].ToString();
            string REGDNO = Session["uid"].ToString();
            string sem = Session["curr_sem"].ToString();
            string sem1 = "";
            if (batch == "2024" && sem == "1") { sem1 = "1"; }
            else
            {
                if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" || Session["degree_code"].ToString() == "UG"))
                {
                    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                }
                else
                {
                    sem1 = sem;
                }


                //// sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //if (Session["college_code"].ToString() == "HBS" && Session["campus_code"].ToString() == "HYD" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                //{
                //    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //}
                //else if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                //{
                //    sem1 = "33333";

                //}
                // sem1 = (Convert.ToInt32(sem) - 1).ToString();
                //else
                ////sem1 = sem;
            }

            listFAC = await iStudentRepository.getfeedback_facultyasyncnew("", Session["campus_code"].ToString(), Session["branch_code"].ToString(), REGDNO, subject_code, sem1, Session["college_code"].ToString(), batch);
            if(listFAC.Count()==0)
            {
                listFAC = await iStudentRepository.getfeedback_facultyasyncguest("", Session["campus_code"].ToString(), Session["branch_code"].ToString(), REGDNO, subject_code, sem1, Session["college_code"].ToString(), batch);
                               
            }

            foreach (var val in listFAC)
            {
                var temp = val;
                IEnumerable<FeedbackMaster> subjectlist = await iStudentRepository.getfeedback_subjects(REGDNO, sem);
                var data1 = subjectlist.Where(m => m.subject_code == temp.SUBJECT_CODE && m.empid == temp.EMPID).FirstOrDefault();

                if (data1 != null)
                {
                    emp.Add(data1);
                }


                //else
                //{
                //    List<FeedbackMaster> empList = new List<FeedbackMaster>();
                //    FeedbackMaster emp1 = new FeedbackMaster();
                //    foreach (var val1 in listFAC)
                //    {
                  
                             
                //        emp1.subject_code = val1.SUBJECT_CODE;
                //        emp1.empid = val1.EMPID;
                //        emp1.empname = val1.emp_name;
                //        emp1.group_code = val1.GROUP_CODE;
                //        empList.Add(emp1);
                //    }



                //    summary.emp = empList;
                //    summary.list = listFAC.ToList();


                //    return Json(summary);

                //}

            }
        

            summary.emp = emp;
            summary.list = listFAC.ToList();


            return Json(summary);
        }

        [HttpGet]
        public async Task<ActionResult> Getquestions(string facultyname, string subject, string faculty, string subjectname, string feedbacksession, string groupcode)
        {
            IEnumerable<FeedbackMaster> questionslist = null;
            string REGDNO = Session["uid"].ToString();
            string sem = Session["curr_sem"].ToString();
            string[] subjectnamesplit = new string[1000];
            subjectnamesplit = subjectname.Split('#');
            string subjectcode = subjectnamesplit[0];
            string subjectrealname = subjectnamesplit[1];
            Session["Subject1"] = subjectcode;
            Session["SubjectName1"] = subjectrealname;
            Session["faculty"] = faculty;
            Session["facultyname"] = facultyname;
            Session["feedbacksession"] = feedbacksession;
            /// string sem1 = (Convert.ToInt32(sem) - 1).ToString();
            //sem=sem1;
            string sem1 = "";
            if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" || Session["degree_code"].ToString() == "UG"))
            {
                sem1 = (Convert.ToInt32(sem) - 1).ToString();
            }
            else
            {
                sem1 = sem;
            }
            try
            {
                questionslist = await iStudentRepository.getquestionlist();
                IEnumerable<FeedbackMaster> checkit = await iStudentRepository.getfeedback_check(subjectcode, faculty, feedbacksession, REGDNO, sem);

                Students getgroupcode = await iStudentRepository.getgroup_code(subjectcode, faculty, feedbacksession, REGDNO, sem);
                if (!string.IsNullOrEmpty(groupcode) && !string.IsNullOrEmpty(getgroupcode.group_code) )
                {
                    Session["groupcode"] = getgroupcode.group_code;

                }
                else
                {
                    Session["groupcode"] = "";
                }
                if (checkit.Count() > 0)
                {
                    questionslist.FirstOrDefault().msg = "You have already given feedback to this subject";
                    Session["msgfeedback"] = "You have already given feedback to this subject";
                }
                else
                {
                    questionslist.FirstOrDefault().msg = "you are eligible to give feedback";
                    Session["msgfeedback"] = "you are eligible to give feedback";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Dynamicfeedbackview", questionslist);
        }


        [HttpGet]
        public async Task<ActionResult> Finalyearfeedback()
        {
            IEnumerable<FeedbackMaster> questionslist = null;
            string REGDNO = Session["uid"].ToString();
            string sem = Session["curr_sem"].ToString();


            try
            {
                questionslist = await iStudentRepository.getfinalyearquestionlist();
                IEnumerable<FeedbackMaster> checkit = await iStudentRepository.getfeedbackfinalyear_check(REGDNO, sem);


                if (checkit.Count() > 0)
                {
                    questionslist.FirstOrDefault().msg = "You have already given feedback to this subject";
                    Session["msgfeedback"] = "You have already given feedback to this subject";
                }
                else
                {
                    questionslist.FirstOrDefault().msg = "you are eligible to give feedback";
                    Session["msgfeedback"] = "you are eligible to give feedback";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("Dynamicfinalyearfeedbackview", questionslist);
        }




        [HttpPost]
        public async Task<ActionResult> createquestion(FormCollection collection)
        {
            dynamic showMessageString = string.Empty;
            try
            {

                List<FeedbackMaster> list = new List<FeedbackMaster>();
                FeedbackMaster datasingle = new FeedbackMaster();
                string[] count = collection.GetValues("count");
                string groupcode = "";


                FeedbackMaster facultydata = await iStudentRepository.get_facultydept(Session["faculty"].ToString());


                string sem = Session["curr_sem"].ToString();
                string batch = Session["batchstart"].ToString();
                string sem1 = "";
                if (batch == "2024" && sem == "1") { sem1 = "1"; }
                else
                {
                    if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && (Session["degree_code"].ToString() == "PG" || Session["degree_code"].ToString() == "UG"))
                    {
                        sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    }
                    else
                    {
                        sem1 = sem;
                    }
                    //// sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    //if (Session["college_code"].ToString() == "HBS" && Session["campus_code"].ToString() == "HYD" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                    //{
                    //    sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    //}
                    //else if (Session["college_code"].ToString() == "SMS" && Session["campus_code"].ToString() == "BLR" && Session["degree_code"].ToString() == "PG" && Session["branch_code"].ToString() == "MBA" && Session["Curr_sem"].ToString() == "3")
                    //{
                    //    sem1 = "33333";
                    //}
                    //// sem1 = (Convert.ToInt32(sem) - 1).ToString();
                    //else
                    /// sem1 = sem;
                }
                for (int i = 0; i <= (Convert.ToInt32(count[0]) - 1); i++)
                {

                    string[] feedbackradio = collection.GetValues("ctl00$MainContent$ID_" + i);
                    string[] feedbackqus = collection.GetValues("NATURE_" + i);
                    string[] feedbacksino = collection.GetValues("ID_" + i);
                    string[] feedbackmaxmarks = collection.GetValues("maxmarks_" + i);
                    string[] Q_STATUS = collection.GetValues("Q_STATUS_" + i);
                    list.Add(new FeedbackMaster
                    {
                        FEEDBACK1 = feedbackradio[0],
                        NATURE = feedbacksino[0],
                        ID = feedbackqus[0],
                        maxmarks = feedbackmaxmarks[0],
                        regdno = Session["uid"].ToString(),


                        semester = sem1,
                        stuname = Session["studname"].ToString(),
                        subject_code = Session["Subject1"].ToString(),
                        empid = Session["faculty"].ToString(),
                        empname = Session["facultyname"].ToString(),
                        // dept_code = Session["dept_code"].ToString(),
                        dept_code = facultydata.dept_code,
                        college_code = Session["college_code"].ToString(),
                        subjectname = Session["SubjectName1"].ToString(),
                        section = Session["section"].ToString(),
                        batch = Session["batch"].ToString(),
                        branch_code = Session["branch_code"].ToString(),
                        campus_code = Session["campus_code"].ToString(),
                        feedbacksession = Session["feedbacksession"].ToString(),
                        course_code = Session["course_code"].ToString(),
                        Q_STATUS = Q_STATUS[0],
                        group_code = Session["groupcode"].ToString()
                    });
                }

                var result = await iStudentRepository.Insertfeedbacknew(list);
                if (result.FirstOrDefault().flag == "success")
                {
                    showMessageString = new
                    {
                        param1 = 100,
                        param2 = "Successfully submitted",
                    };

                }
                else
                {
                    showMessageString = new
                    {
                        param1 = 100,
                        param2 = "oops,something went wrong",
                    };
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> CDLStudycerificate()
        {

            return View("CDLStudycerificate");
        }




        [HttpGet]

        public async Task<JsonResult> getCDLStudyCertificateData()
        {

            //Session["REGDNO"] = "1210116117";
            string REGDNO = Session["uid"].ToString();
            string TY = "";
            string QCText = "";
            try
            {
                List<CDLCertificates> data = await iStudentRepository.getcdlstudycertificate(REGDNO);

                if (data.Count() > 0)
                {
                    TY = TY + "$" + REGDNO.Trim();
                   
                    QCText = QCText + "https://gstudent.gitam.edu/Home/CDL_Bonafied_Certificate?QT=" + TY + "";
                 
                    data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
                }
                GenerateMyQCCodestudy(QCText);
                string str = "<img src=\"images/StdDynamicQR.jpg\"  />";

                data.ElementAt(0).qrtext = str;

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet]

        [ValidateInput(false)]
        public async Task<ActionResult> CDL_Bonafied_Certificate(string QT)
        {

            //Session["REGDNO"] = "1210116117";
            //string userid = Session["uid"].ToString();
            List<CDLCertificates> data = new List<CDLCertificates>();
            try
            {
                if (QT != null)
                {
                    //  string reg = Session["uid"].ToString();

                    string reg = QT.Split('$')[1].ToString();
                    data = await iStudentRepository.getcdlstudycertificate(reg);
                    if (data.Count() > 0)
                    {

                        data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("CDL_Study_CertificateQR", data);
        }


        private void GenerateMyQCCodestudy(string QCText)
        {
            try
            {
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(QCText);
                string path = Server.MapPath("~/images/StdDynamicQR.jpg");
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult> getnews()
        {
            try
            {

                IEnumerable<Event> Events = await iStudentRepository.getevents(Session["campus_code"].ToString());

                return View("Newsevents", Events);
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }




        [HttpGet]
        public async Task<ActionResult> CDLTransfercerificate()
        {

            return View("CDLTransfercerificate");
        }

        [HttpGet]

        public async Task<JsonResult> getCDLTransferCertificateData()
        {

            //Session["REGDNO"] = "1210116117";
            string REGDNO = Session["uid"].ToString();
            string TY = "";
            string QCText = "";
            try
            {
                List<CDLCertificates> data = await iStudentRepository.getcdltransfercertificate(REGDNO);

                if (data.Count() > 0)
                {
                    TY = TY + "$" + REGDNO.Trim();
                    // TY = TY + "$" + dr["REGDNO"].ToString().Trim() + "$" + dr["NAME"].ToString().Trim() + "$" + dr["COURSE"].ToString().Trim() + "$";
                   
                    QCText = QCText + "https://studentlms.gitam.edu/Home/CDL_Transfer_Certificate?QT=" + TY + "";
                    //QCText = QCText + "\nRegdno: " + REGDNO + ",\nName: " + data.FirstOrDefault().NAME + ",\nCourse:" + data.FirstOrDefault().COURSE;
                    //QCText = QCText + "\nGITAM (Deemed to be University)\n";

                    data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
                    data.ElementAt(0).DOB_WORDS = NumberToWords(Convert.ToInt32(data.FirstOrDefault().DOB_WORDS), data.FirstOrDefault().DOB_WORDS);
                }
                GenerateMyQCCodeTC(QCText);
                string str = "<div class='pull-right' style='float:right;'><img src=\"images/TCDynamicQR.jpg\" width =\"175\" style=\"margin-left:-22px;margin-top:-70px\" /></div>";

                data.ElementAt(0).qrtext = str;


                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public static string NumberToWords(int number, string numm)
        {
            string words = "";
            int[] a = new int[10];
            string[] digits_words = {
            "Zero",
            "One",
            "Two",
            "Three",
            "Four",
            "Five",
            "Six",
            "Seven",
            "Eight",
            "Nine"
         };

            int next = 0;
            int num_digits = 0;
            do
            {
                next = number % 10;
                a[num_digits] = next;
                num_digits++;
                number = number / 10;
            } while (number > 0);
            num_digits--;
            for (; num_digits >= 0; num_digits--)
            {

                words = words + digits_words[a[num_digits]] + " ";
            }
            if (numm.Substring(0, 1) == "0")
            {
                words = "Zero " + words;
            }
            return words;
        }



        private void GenerateMyQCCodeTC(string QCText)
        {
            try
            {
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(QCText);
                string path = Server.MapPath("~/images/TCDynamicQR.jpg");
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message);
            }




        }



        [HttpGet]

        [ValidateInput(false)]
        public async Task<ActionResult> CDL_Transfer_Certificate(string QT)
        {

            //Session["REGDNO"] = "1210116117";
            //string userid = Session["uid"].ToString();
            List<CDLCertificates> data = new List<CDLCertificates>();
            try
            {
                if (QT != null)
                {
                    //  string reg = Session["uid"].ToString();

                    string reg = QT.Split('$')[1].ToString();
                    data = await iStudentRepository.getcdltransfercertificate(reg);
                    if (data.Count() > 0)
                    {

                        data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
                        data.ElementAt(0).DOB_WORDS = NumberToWords(Convert.ToInt32(data.FirstOrDefault().DOB_WORDS), data.FirstOrDefault().DOB_WORDS);
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("CDL_Transfer_Certificate", data);
        }


        [HttpGet]
        public async Task<ActionResult> CDLGradeCrad()
        {
            string userid = Convert.ToString(Session["uid"]);

            try
            {
                IEnumerable<studenttrack> semslist = await iStudentRepository.getCDLsemesterasync(userid);

                ViewBag.semslist = semslist.Select(testgroup => new SelectListItem
                {
                    Text = testgroup.SEMESTER.ToString(),
                    Value = testgroup.SEMESTER.ToString()
                });

            }
            catch (HttpRequestException)
            {
                RedirectToAction("Index", "../loginpage");
            }
            return View("CDLGradeCrad");
        }





        [HttpGet]

        public async Task<JsonResult> getCDLGradecard(string semester)
        {

            //Session["REGDNO"] = "1210116117";
            string userid = Session["uid"].ToString();
            string admissiondate = Convert.ToString(Session["AdmissionDate"]);
            string QCText = "";
            string str = "";
            string campus = "";
            string college = "";
            string degree = "";
            string course = "";
            string branch = "";
            string batch = "";
            string month = "";
            string year = "";
            string section = "";
            string process = "";
            string txtnote = "";
            string miscNote = "";
            try
            {
                List<studenttrack> records = await iStudentRepository.getStudentcarddetailsCDL(semester, userid);
                DateTime filedate = DateTime.Now;
                string formatfile = "ddMMyyyyfff";
                String datefile = filedate.ToString(formatfile);

                String query, query1, query2 = null;
                DataSet ds, ds1, ds4 = null;
                DataTable dt2, dt1, dt4 = null;


                // Session["College"] = college;

                //List<studenttrack> details = await iStudentRepository.getmonth(semester, userid);
                //if (details.Count() > 0)
                //{
                //    month = details.FirstOrDefault().Month;
                //    year = details.FirstOrDefault().Year;
                //}
                if (records.Count() > 0)
                {
                    campus = records.FirstOrDefault().CAMPUS_CODE;
                    college = records.FirstOrDefault().college_code;
                    degree = records.FirstOrDefault().DEGREE_CODE;
                    course = records.FirstOrDefault().COURSE_CODE;
                    branch = records.FirstOrDefault().BRANCH_CODE;
                    batch = records.FirstOrDefault().BATCH;
                    section = records.FirstOrDefault().section;
                    month = records.FirstOrDefault().Month;
                    year = records.FirstOrDefault().Year;
                    process = records.FirstOrDefault().process_type;
                }

                int delete = await iStudentRepository.DeleteDetailsCDL(campus, college, course, branch, batch);
                //int delete = 1;

                List<studenttrack> GradeResult = await iStudentRepository.getGradeResultCDL(semester, userid, month, year, process);

                studenttrack detailsdata = new studenttrack()
                {
                    regdno = GradeResult.FirstOrDefault().regdno,
                    name = GradeResult.FirstOrDefault().name,
                    BRANCH = GradeResult.FirstOrDefault().BRANCH_CODE,
                    COURSE = GradeResult.FirstOrDefault().COURSE_CODE,
                    DEGREE_CODE = GradeResult.FirstOrDefault().DEGREE_CODE,
                    college_code = GradeResult.FirstOrDefault().college_code,
                    CAMPUS_CODE = GradeResult.FirstOrDefault().CAMPUS_CODE,
                    course_name = GradeResult.FirstOrDefault().course_name,
                    Branch_name = GradeResult.FirstOrDefault().Branch_name,
                    section = GradeResult.FirstOrDefault().section,
                    COURSE_TITLE = GradeResult.FirstOrDefault().COURSE_TITLE,
                    PRINT_YEAR = GradeResult.FirstOrDefault().PRINT_YEAR,
                    EXAM_NAME = GradeResult.FirstOrDefault().EXAM_NAME,
                    exam_date = GradeResult.FirstOrDefault().exam_date,
                    Month = month,
                    Year = GradeResult.FirstOrDefault().Year,
                    BATCH = GradeResult.FirstOrDefault().BATCH,
                    process_type = process,
                    sgpa = GradeResult.FirstOrDefault().sgpa,
                    cgpa = GradeResult.FirstOrDefault().cgpa,
                    TOTAL_SEM_CREDITS = GradeResult.FirstOrDefault().TOTAL_SEM_CREDITS,
                    CUM_SEM_CREDITS = GradeResult.FirstOrDefault().CUM_SEM_CREDITS,
                    SEMESTER = semester,



                };

                var insertdetails = await iStudentRepository.InsertGradedetails(detailsdata);

                List<studenttrack> detailsmore = await iStudentRepository.getGradeReportsCDL(detailsdata);
                List<studenttrack> grademarks = new List<studenttrack>();
                if (detailsmore != null)
                {
                    if (detailsmore.Count() > 0)
                    {
                        for (int i = 0; i < detailsmore.Count(); i++)
                        {
                            studenttrack grades = new studenttrack()
                            {
                                regdno = GradeResult.FirstOrDefault().regdno,
                                SUBJECT_CODE = detailsmore.ElementAt(i).SUBJECT_CODE,
                                SUBJECT_NAME = detailsmore.ElementAt(i).SUBJECT_NAME,
                                CREDITS1 = detailsmore.ElementAt(i).CREDITS1,
                                Grade = detailsmore.ElementAt(i).Grade,
                                SEMESTER = detailsmore.ElementAt(i).SEMESTER,
                                process_type = detailsmore.ElementAt(i).process_type,
                                Month = GradeResult.FirstOrDefault().Month,
                                Year = GradeResult.FirstOrDefault().Year,
                                CAMPUS_CODE = campus,
                                DT_Completion = detailsmore.FirstOrDefault().Doc,
                            };
                            grademarks.Add(grades);
                        }

                    }
                    var updatedetails = await iStudentRepository.updateGradedetailsCDL(grademarks);
                }





                if (records.Count() > 0)
                {

                    List<studenttrack> carddetails = await iStudentRepository.getcarddetails(userid);

                    QCText = " ";
                    string TY = "QRCODE";
                    string DT_Completion = "";
                    // QCText = QCText + "(Demed to be university)  ";
                    foreach (var dtr in carddetails)
                    {
                        QCText = "";
                        // TY = TY + "$" + dtr[0].ToString().Trim() + "$" + dtr[3].ToString().Trim() + "$";
                        TY = TY + "$" + dtr.regdno.Trim() + "$" + semester + "$" + dtr.emonth.Trim() + "$" + dtr.Year.Trim() + "$" + dtr.process_type.Trim() + "$";

                        //QCText = QCText + "https:///testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT="+TY +"";
                        //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid2.aspx?QT=" + TY + "";
                        //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";
                        //  QCText = QCText + "http://testonlinecertificates.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            //  DT_Completion = Convert.ToDateTime(dtr["DT_Completion"]).ToString("dd-MMM-yyyy");
                            if (dtr.sgpa == "0" && dtr.cgpa == "0")
                                DT_Completion = "-";
                            else
                                DT_Completion = Convert.ToDateTime(dtr.date_complete).ToString("dd-MMM-yyyy");
                            // DT_Completion = Convert.ToDateTime(dtr.DT_Completion).ToString("dd-MMM-yyyy");
                            //QCText = QCText + "https://onlinecertificates.gitam.edu/View_Result_Grid1.aspx?QT=" + TY + "";
                    
                            QCText = QCText + "https://studentlms.gitam.edu/Home/ViewResultGridCDL?QT=" + TY + "";
                        }
                        else
                        {
                         
                            QCText = QCText + "https://studentlms.gitam.edu/Home/ViewResultGrid?QT=" + TY + "";
                        }

                        QCText = QCText + "\nRegdno: " + dtr.regdno + ",\nName: " + dtr.name + ",\nCourse:" + dtr.COURSE + ",\nBranch:" + dtr.Branch_name + ",Semester: " + dtr.SEMESTER;//,Semester : "+dtr[83].ToString()
                        QCText = QCText + "\nGITAM (Deemed to be University)\n";

                    }
                    GenerateMyQCCode(QCText);
                    foreach (var dr in carddetails)
                    {
                        //String month = Request.Params["month"].ToString();

                        str = str + "<div class=\"wrapper\"><div class=\"abc img_hover\" style=\"margin-top:30px;\" >";
                        str = str + "<div class=\"innerdiv\">";
                        str = str + "<Label hidden id='qrdata'>" + TY + "</Label>";
                        str = str + "<table   style=\"border-radius: 10px;margin: 10px auto;width: 650px;\" cellspacing=\"0\" cellpadding=\"0\" border = \"0\" >";
                        str = str + "<tbody>";

                        str = str + "<tr>";


                        str = str + "<table width=\"200\" class='sa-width'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:right;\">";

                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";


                        str = str + "<tr>";

                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        str = str + "<table>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<td>  <img src=\"images/academiccert1.jpg\" class='logo' /></td>";


                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";




                        str = str + "<tr>";
                        //< p style =\"font-size:12px;\"</p><p style=\"font-size:12px;\"></p>
                        str = str + "<table width=\"650\" style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<center style='margin-top:10px;'><b style=\"font-size:24px;\"><span style='color:#026B5C'>GRADE CARD</span></b></center>";
                        //str = /*str + "<br/>";*/


                        //str = str + "<center style=\"font-size:18px;\"><b>" + dr[83] + " Semester, " + dr[85] + " " + dr[84] + "</b></center>";

                        if (Convert.ToString(Session["college_code"]).Equals("GPP") || dr.BRANCH == "Optometry")
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.Branch_name + " - Degree Examination</b></center>";

                        }
                        else
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.COURSE + " Degree Examination</b></center>";
                        }
                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            if (dr.COURSE.Trim().ToUpper().Equals("MBA") || dr.COURSE.Trim().ToUpper().Equals("M.B.A"))
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Year, " + dr.Month + " " + dr.Year + "</b></center>";

                        }
                        else
                        {
                            if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                            {
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Trimester, " + dr.Month + " " + dr.Year + "</b></center>";
                            }
                            else if (Convert.ToInt32(dr.SEMESTER) >= 21)
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + ", " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                        }
                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";

                        str = str + "</tr>";



                        str = str + "<tr>";

                        str = str + "<table class='mt-1 sa-width details' style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        //str = str + "<tr>"; str = str + "<td  width=\"650\">"; str = str + "<table width=\"550\" class='mt-1 sa-width details' style='margin-bottom:20px'>"; str = str + "<tbody>";//msr
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Regd.No</span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.regdno + "</td>";
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Name </span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.name + "</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "</tr>";
                        if (!Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>Branch</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + dr.Branch_name + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }

                        if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            string gppname = "Kautilya School of Public Policy";
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }
                        //else if(Session["phdcoursecode"].ToString().Equals("PHD"))
                        //{
                        //    string gppname = "Inistitute of Technology";
                        //    str = str + "<tr>";
                        //    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                        //    str = str + "<td> :</td>";
                        //    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "</tr>";
                        //}


                        str = str + "</tbody>";
                        str = str + "</table>";



                        str = str + "</tr>";

                        str = str + "<tr>";


                        //str = str + "<div class=\"b12121\">";
                        //height =\"350\"
                        str = str + "<div class='table-responsive'><table class=\"tab mt-2 mb-2 sa-width\">";
                        str = str + "<tbody>";
                        str = str + "<tr class=\"\" style='border:2px solid #026B5C'>";
                        str = str + "<td style=\"text-align:left;padding:4px;width:105px;\"> <span style='color:#026B5C;'><b>Course Code</b></span> </td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Name of the Course</b></span> </td>";

                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Credits</b> </span></td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Grade</b> </span></td>";

                        str = str + "</tr>";






                        if (dr.SUB1_CODE != null)
                        {
                            if (dr.SUB1_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB1_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB1_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB2_CODE != null)
                        {
                            if (dr.SUB2_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB2_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB2_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB3_CODE != null)
                        {
                            if (dr.SUB3_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB3_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB3_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB4_CODE != null)
                        {
                            if (dr.SUB4_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB4_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB4_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB5_CODE != null)
                        {
                            if (dr.SUB5_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB5_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB5_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB6_CODE != null)
                        {
                            if (dr.SUB6_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB6_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB6_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB7_CODE != null)
                        {
                            if (dr.SUB7_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB7_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB7_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB8_CODE != null)
                        {
                            if (dr.SUB8_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB8_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB8_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB9_CODE != null)
                        {
                            if (dr.SUB9_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB9_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB9_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB10_CODE != null)
                        {
                            if (dr.SUB10_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB10_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB10_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB11_CODE != null)
                        {
                            if (dr.SUB11_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB11_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB11_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB12_CODE != null)
                        {
                            if (dr.SUB12_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB12_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB12_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB13_CODE != null)
                        {
                            if (dr.SUB13_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB13_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB13_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB14_CODE != null)
                        {
                            if (dr.SUB14_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB14_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB14_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB15_CODE != null)
                        {
                            if (dr.SUB15_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB15_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB15_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }

                        str = str + "</tbody>";
                        str = str + "</table></div>";

                        str = str + "</tr>";


                        str = str + "<tr>";


                        str = str + "<div class='sgpa'><table><tbody><tr>";

                        if (dr.sgpa == "0" && dr.cgpa == "0")
                        {
                            //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[12] + " <br/></td></tr><tr>";
                            //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[13] + " <br/></td>";
                            str = str + "<td> <p style =\"font-size:15px;\">SGPA </p></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <p style =\"font-size:15px;\">CGPA </p></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }
                        else
                        {
                            //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[86] + " <br/></td></tr><tr>";
                            //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[87] + " <br/></td>";
                            str = str + "<td> <span style =\"font-size:15px;\">SGPA </span></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <span style =\"font-size:15px;\">CGPA </span></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }

                        str = str + "</tr>";
                        str = str + "<div class=\"b6\">";
                        //str = str + "<span> <b>Printed On:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "</td>";
                        //str = str + "<br/></span>";

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b>Printed On&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/></span>";
                        }

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b style =\"font-size:15px;\">Printed On&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/><br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Mode of delivery&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;ODL";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of Admission&nbsp; :</b>&nbsp;" + Convert.ToString(Session["AdmissionDate"]) + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of completion :</b>&nbsp;" + DT_Completion + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Learner Support Centres :&nbsp;---</b>" + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Examination Centres&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;GITAM (deemed to beUniversity), VISAKHAPATNAM.";
                            str = str + "<br/></span>";
                        }



                        str = str + "</div>";
                        str = str + "<div class=\"b5\">";
                        str = str + "<br/>";
                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            // if(Session["Gprocess"].ToString().Trim().Equals("SRV"))
                            // {
                            //     //str = str + "<div style=\"width:600px;\"><b>''Certificate issued after the publication of supplimentary revaluation results''</b></div>";
                            // }
                            //else
                            //if (Session["Gprocess"].ToString().Trim().Contains("RV"))
                            //{
                            //    str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            //}
                            //else if (Session["Gprocess"].ToString().Trim().Contains("BG"))
                            //{
                            //str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            //}
                            if (Convert.ToString(Session["Gprocess"]).Trim().Contains("RV"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            }
                            else if (Convert.ToString(Session["Gprocess"]).Trim().Contains("BG"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            }
                        }

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<table style =\"width:650px;margin-left: 0px;\" class=\"w-700\"><tbody><tr><td style =\"width:450px;\" class=\"w-550\">";//msr
                            str = str + "<span style =\"width:100px;\"> <b>Note:</b><br/><span style =\"font-size:12px;\"><i>1.This is a digitally generated certificate. The format of this certificate may differ from the";
                            str = str + " document issued by the University<br/></i></span></span>";

                            str = str + "<span style =\"width:100px;font-size:12px;\"><i>2.For any clarification, please contact <a href=\"#\">controllerofexaminations@gitam.edu</a><i></span>";
                        }

                        // if (Session["College"].ToString().Equals("CDL"))
                        //   str = str + "<span style =\"width:100px;font-size:12px;\"><i><br/>3.Mode of Delivery ODL<i></span>";
                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            //str = str + "</td>";//msr
                            //str = str + "<td style =\"width:100px;\" class=\"w-150\">";
                            //str = str + "<div style=\"width:100px;margin-left:530px;\" class=\"w-150\"><img src=\"images/DynamicQR.jpg\" width =\"140\" class=\"w-150\" /></div>";
                            //str = str + "</td></tr>"; 
                            //str = str + "</tbody>";
                            //str = str + "</table>";

                            //str = str + "<table \" style = \"width:200pxpx;display:block;margin-left:440px;font-family: 'Open Sans', sans - serif\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                            //str = str + "<tbody>";
                            //str = str + "<tr>";
                            //str = str + "<td><img src='images/Sumanth-Kumar.png' alt='Snow' style=';display:block;margin-left:100px;height:30px;margin-top:-11px;'></td>";
                            //str = str + "</tr>";
                            //str = str + "<tr>";
                            //str = str + "<td colspan ='3' align=\"right\" style = \"font-size: 15px;\"><b> <span class=\"tempfields\"  style = \"width:200px;margin-top:-2px;\">controller of examinations</span> </b></td>";
                            //str = str + "</tr>";
                            //str = str + "</tbody>";
                            //str = str + "</table>";

                            str = str + "<div class='sign'><div class='qr'><img src=\"images/DynamicQR.jpg\" width =\"140\" class=\"w-150\" /></div><div class='text-right'><img src='images/Sumanth-Kumar.png' style='height:30px;'><div><b>controller of examinations</b></div></div></div>";

                        }
                        else
                        {
                            str = str + "</td>";//msr
                            str = str + "<td style =\"width:100px;\" class=\"w-150\">";
                            str = str + "<div style=\"width:100px;\" class=\"w-150\"><img src=\"images/DynamicQR.jpg\" width =\"140\"  class=\"w-150\" /></div>";
                            str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";



                        }


                        str = str + "</tr>";



                        str = str + "</tbody>";
                        str = str + "</table>";
                        str = str + "</div>";
                        str = str + "</div></div><br>";
                        str = str + "<div class='break'><br></div>";



                    }

                }








                return Json(str, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]

        public async Task<ActionResult> ViewResultGridCDL(string QT)
        {

            //Session["REGDNO"] = "1210116117";
            //string userid = Session["uid"].ToString();
            List<studenttrack> carddetails = new List<studenttrack>();
            try
            {
                if (QT != null)
                {
                    if (QT.Split('$')[0].ToString().Equals("QRCODE"))
                    {
                        String reg = "1210215120";
                        String sem = "2";
                        String month = "";
                        String year = "";
                        String process = "";


                        if (QT.Split('$')[0] != null && QT.Split('$')[1] != null)
                        {
                            reg = QT.Split('$')[1].ToString();
                            sem = QT.Split('$')[2].ToString();
                            month = QT.Split('$')[3].ToString();
                            year = QT.Split('$')[4].ToString();
                            process = QT.Split('$')[5].ToString();
                        }
                        carddetails = await iStudentRepository.getQrResultCDL(reg, sem, month, process, year);

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("ViewResultGrid", carddetails);
        }

        //public async Task<ActionResult> Gethalltickets()
        //{
        //    Hallticket chart = new Hallticket();

        //    hallticketsummary admission = new hallticketsummary();
        //    try
        //    {
        //        string regdno = Session["uid"].ToString();
        //        string course_code = Convert.ToString(Session["course_code"]);
        //        string college_code = Session["college_code"].ToString();



        //        string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
        //        string cur_sem = Session["Curr_sem"].ToString();
        //        string batch = Session["batch"].ToString();
        //        string[] batch1 = new string[1000];
        //        batch1 = batch.Split('-');
        //        string batch2 = batch1[0];
        //        Session["batchattd"] = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
        //        string batchattd = Session["batchattd"].ToString();
        //        string user_id = Session["uid"].ToString();

        //        if (Session["college_code"].ToString() == "GIMSR" || Session["college_code"].ToString() == "GIMSRC" || Session["college_code"].ToString() == "GIMSRH")
        //        {
        //            admission.attendancenotes = await iStudentRepository.Getattendance_semster_gimsr(user_id, table_name, college_code, cur_sem, batchattd);
        //        }
        //        else
        //        {
        //            admission.attendancenotes = await iStudentRepository.Getattendance_semster(user_id, table_name, college_code, cur_sem, batchattd);
        //        }
        //        Double percentage = 0;
        //        if (admission.attendancenotes.Count() > 0)
        //        {
        //            percentage = Convert.ToDouble(admission.attendancenotes.FirstOrDefault().percentage);
        //        }
        //        if (course_code == "PHD")
        //        {
        //            admission.notes = await iStudentRepository.gethallticketPHDdata52(regdno);
        //        }
        //        else if (college_code == "GST" || college_code == "GSHS" || college_code == "GSB" || college_code == "GSS" || college_code == "GSA" || college_code == "GSL" || college_code == "GSP")
        //        {
        //            if ((college_code == "GST" && percentage >= 75) || (college_code == "GSHS" && percentage >= 75) || (college_code == "GSB" && percentage >= 75) || (college_code == "GSS" && percentage >= 75) || (college_code == "GSA" && percentage >= 75) || (college_code == "GSL" && percentage >= 75) || (college_code == "GSP" && percentage >= 80))
        //            {
        //                admission.notes = await iStudentRepository.gethallticketdata52(regdno);
        //            }
        //            else
        //            {
        //                chart.msg = "Please contact HOD's office..";
        //            }
        //        }
        //        else
        //        {
        //            admission.notes = await iStudentRepository.gethallticketdata52(regdno);
        //        }
        //        for (int i = 0; i < admission.notes.Count(); i++)
        //        {
        //            if (admission.notes.ElementAt(i).type1.Equals("R"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Regular";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("S"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Supplementary";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("SP"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Special Exam";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("SD"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Special Drive";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("I"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Instant Exam";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("B"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Betterment Exam";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("BG"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Betterment of Grade";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("ST"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Summer Term";
        //            }
        //            else if (admission.notes.ElementAt(i).type1.Equals("CC"))
        //            {
        //                admission.notes.ElementAt(i).type2 = "Compressed Courses";
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return PartialView("Hallticket", admission);
        //}
        //[HttpGet]
        //public async Task<ActionResult> btnsubmit_Click(string regdno, string type, string semester, string tick)
        //{
        //    Hallticket chart = new Hallticket();
        //    chart.type1 = type;
        //    chart.id1 = semester;
        //    chart.tick = tick;
        //    hallticketsummary admission = new hallticketsummary();
        //    //string regdno = Session["uid"].ToString();
        //    chart.course_code = Convert.ToString(Session["course_code"]);
        //    if (chart.type1.Equals("R"))
        //    {
        //        chart.type2 = "Regular";
        //    }
        //    else if (chart.type1.Equals("S"))
        //    {
        //        chart.type2 = "Supplementary";
        //    }
        //    else if (chart.type1.Equals("SP"))
        //    {
        //        chart.type2 = "Special Exam";
        //    }
        //    else if (chart.type1.Equals("SD"))
        //    {
        //        chart.type2 = "Special Drive";
        //    }
        //    else if (chart.type1.Equals("I"))
        //    {
        //        chart.type2 = "Instant Exam";
        //    }
        //    else if (chart.type1.Equals("B"))
        //    {
        //        chart.type2 = "Betterment Exam";
        //    }
        //    else if (chart.type1.Equals("BG"))
        //    {
        //        chart.type2 = "Betterment of Grade";
        //    }
        //    else if (chart.type1.Equals("ST"))
        //    {
        //        chart.type2 = "Summer Term";
        //    }
        //    else if (chart.type1.Equals("CC"))
        //    {
        //        chart.type2 = "Compressed Courses";
        //    }
        //    if (chart.course_code.Equals("PHD"))
        //    {
        //        chart.regdno = Session["uid"].ToString();
        //        chart.CURRSEM = Session["Curr_sem"].ToString();
        //        Session["type1"] = chart.type1;
        //        Session["type2"] = chart.type2;
        //        admission = await PHD_HALL_TICKET(chart);
        //        return View("NEW_HallTickets", admission);
        //    }
        //    else
        //    {
        //        var student = await iStudentRepository.get_student_data52(regdno);
        //        try
        //        {
        //            if (student != null)
        //            {
        //                if (student.ElementAt(0).college_code.Equals("GIMSR"))
        //                {
        //                    if (chart.type1 == "R" || chart.type1 == "S")
        //                    {
        //                        if (student.ElementAt(0).status.Equals(null) || student.ElementAt(0).status.Equals("A") || student.ElementAt(0).status.Equals("") || student.ElementAt(0).status.Equals("NULL") || student.ElementAt(0).status.Equals("S"))
        //                        {
        //                            String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
        //                            chart.regdno = Session["uid"].ToString();
        //                            chart.CURRSEM = Session["Curr_sem"].ToString();
        //                            Session["type1"] = chart.type1;
        //                            Session["type2"] = chart.type2;
        //                            admission = await NEW_HallTickets(chart);
        //                            return View("NEW_HallTickets", admission);
        //                        }
        //                        else
        //                        {
        //                            chart.msg = "1Please contact HOD's office..";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    if (chart.type1 == "R")
        //                    {
        //                        if (student.ElementAt(0).status.Equals(null) || student.ElementAt(0).status.Equals("A") || student.ElementAt(0).status.Equals("") || student.ElementAt(0).status.Equals("NULL") || student.ElementAt(0).status.Equals("S"))
        //                        {
        //                            String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
        //                            chart.regdno = Session["uid"].ToString();
        //                            //string semes = Session["Curr_sem"].ToString();
        //                            //string[] semes1 = new string[1000];
        //                            //semes1 = semes.Split('-');

        //                            //string batch2 = semes1[0];
        //                            //int batchInt = Convert.ToInt32(batch2);
        //                            //int idInt = batchInt - 1;
        //                            //string id1 = idInt.ToString();
        //                            //chart.id1 = id1;
        //                            Session["id1"] = chart.id1;

        //                            Session["type1"] = chart.type1;
        //                            Session["type2"] = chart.type2;

        //                            admission = await NEW_HallTickets(chart);
        //                            return View("NEW_HallTickets", admission);
        //                        }
        //                        else
        //                        {
        //                            chart.msg = "1Please contact HOD's office..";
        //                        }
        //                    }
        //                    else if (chart.type1 == "SD" || chart.type1 == "B")
        //                    {
        //                        chart.type1 = type;
        //                        chart.id1 = semester;
        //                        chart.tick = tick;
        //                        String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
        //                        chart.regdno = Session["uid"].ToString();
        //                        chart.CURRSEM = Session["Curr_sem"].ToString();
        //                        Session["type1"] = chart.type1;
        //                        Session["type2"] = chart.type2;

        //                        //if (chart.tick == "Y")
        //                        //{
        //                            admission = await NEW_HallTickets(chart);
        //                            return View("NEW_HallTickets", admission);
        //                        //}
        //                        //else
        //                        //{
        //                        //    admission = await NEW_HallTickets(chart);
        //                        //    return View("hallticketinstruction", admission);

        //                        //}
        //                    }
        //                    else
        //                    {
        //                        String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
        //                        chart.regdno = Session["uid"].ToString();
        //                        chart.CURRSEM = Session["Curr_sem"].ToString();
        //                        Session["type1"] = chart.type1;
        //                        Session["type2"] = chart.type2;
        //                        admission = await NEW_HallTickets(chart);
        //                        return View("NEW_HallTickets", admission);
        //                    }
        //                }
        //            }
        //            //}
        //            else
        //            {
        //                chart.msg = "1Please contact HOD's office..";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            chart.msg = "1Invalid details please try again /Please contact HOD's office";
        //        }
        //    }
        //    return PartialView("Hallticket", admission);
        //}

        //[HttpPost]
        //public async Task<hallticketsummary> NEW_HallTickets(Hallticket chart)
        //{
        //    hallticketsummary admission = new hallticketsummary();
        //    IEnumerable<Hallticket> profiledata = null;
        //    IEnumerable<Hallticket> details = null;
        //    try
        //    {

        //        chart.regdno = Session["regdno"].ToString();
        //        chart.type1 = Session["type1"].ToString();
        //        chart.type2 = Session["type2"].ToString();
        //        chart.course_code = Convert.ToString(Session["course_code"]);
        //        chart.branch_code = Convert.ToString(Session["branch_code"]);
        //        chart.degree_code = Convert.ToString(Session["degree_code"]);
        //        var student = iStudentRepository.getstudentdata(chart.regdno);
        //        if (student.Result.ElementAt(0).course_code == "PHD")
        //        {
        //            if (student.Result.ElementAt(0).branch_code == "Economics")
        //            {
        //                chart.branch_name = "Humanities and Social Sciences";
        //            }
        //            else
        //            {
        //                chart.branch_name = student.Result.ElementAt(0).branch_code;
        //            }
        //        }
        //        else
        //        {
        //            chart.branch_name = student.Result.ElementAt(0).branch_code;
        //        }
        //        if (student.Result.ElementAt(0).course_code == "PHD")
        //        {
        //            chart.examination = "Pre " + student.Result.ElementAt(0).course_code;
        //        }
        //        else
        //        {
        //            chart.examination = student.Result.ElementAt(0).course_code;
        //        }
        //        chart.NAME = student.Result.ElementAt(0).NAME;
        //        chart.todaydate = DateTime.Now.ToString("dd-MMM-yyyy");
        //        chart.batch = student.Result.ElementAt(0).batch;
        //        var stddetails1 = iStudentRepository.Get_Student_Data(chart.regdno);

        //        if (Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2021 && student.Result.ElementAt(0).degree_code == "UG")
        //        {

        //            if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
        //            {
        //                chart.instructn = "FinalYeardiv";
        //            }
        //            else
        //            {
        //                chart.instructn = "A21";
        //            }
        //        }
        //        else
        //        {
        //            if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
        //            {
        //                chart.instructn = "FinalYeardiv";
        //            }
        //            else
        //            {
        //                chart.instructn = "B21";
        //            }
        //        }
        //        if (chart.type1 != null)

        //        {
        //            var stddetails = iStudentRepository.Get_Student_Data(chart.regdno);
        //            var sem = chart.id1;
        //            if (sem == "1")
        //            {
        //                chart.psntsem = "I";
        //            }
        //            else if (sem == "2")
        //            {
        //                chart.psntsem = "II";
        //            }
        //            else if (sem == "3")
        //            {
        //                chart.psntsem = "III";
        //            }
        //            else if (sem == "4")
        //            {
        //                chart.psntsem = "IV";
        //            }
        //            else if (sem == "5")
        //            {
        //                chart.psntsem = "V";
        //            }
        //            else if (sem == "6")
        //            {
        //                chart.psntsem = "VI";
        //            }
        //            else if (sem == "7")
        //            {
        //                chart.psntsem = "VII";
        //            }
        //            else if (sem == "8")
        //            {
        //                chart.psntsem = "VIII";
        //            }
        //            else if (sem == "9")
        //            {
        //                chart.psntsem = "IX";
        //            }
        //            else if (sem == "10")
        //            {
        //                chart.psntsem = "X";
        //            }
        //            else if (sem == "11")
        //            {
        //                chart.psntsem = "XI";
        //            }
        //            else if (sem == "12")
        //            {
        //                chart.psntsem = "XII";
        //            }
        //            else if (sem == "13")
        //            {
        //                chart.psntsem = "XIII";
        //            }
        //            else if (sem == "14")
        //            {
        //                chart.psntsem = "XIV";

        //            }
        //            else if (sem == "15")
        //            {
        //                chart.psntsem = "XV";
        //            }
        //            else
        //            {
        //            }

        //            if (chart.type1 == "SD" || chart.type1 == "B")
        //            {
        //                admission.note = chart;
        //                admission.notes = await iStudentRepository.getstudentspecialhallticket(chart);

        //            }
        //            else
        //            {
        //                admission.note = chart;
        //                admission.notes = await iStudentRepository.getstudenthallticket(chart);

        //            }


        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Redirect("Errorpage.aspx");
        //    }
        //    return (admission);
        //}


        //[HttpPost]
        //public async Task<hallticketsummary> PHD_HALL_TICKET(Hallticket chart)
        //{
        //    hallticketsummary admission = new hallticketsummary();
        //    IEnumerable<Hallticket> profiledata = null;
        //    IEnumerable<Hallticket> details = null;
        //    try
        //    {

        //        chart.regdno = Session["regdno"].ToString();
        //        chart.type1 = Session["type1"].ToString();
        //        chart.type2 = Session["type2"].ToString();
        //        chart.course_code = Convert.ToString(Session["course_code"]);
        //        chart.branch_code = Convert.ToString(Session["branch_code"]);
        //        chart.degree_code = Convert.ToString(Session["degree_code"]);
        //        var student = iStudentRepository.getstudentdata(chart.regdno);
        //        if (student.Result.ElementAt(0).course_code == "PHD")
        //        {
        //            if (student.Result.ElementAt(0).branch_code == "Economics")
        //            {
        //                chart.branch_name = "Humanities and Social Sciences";
        //            }
        //            else
        //            {
        //                chart.branch_name = student.Result.ElementAt(0).branch_code;
        //            }
        //        }
        //        else
        //        {
        //            chart.branch_name = student.Result.ElementAt(0).branch_code;
        //        }
        //        if (student.Result.ElementAt(0).course_code == "PHD")
        //        {
        //            chart.examination = "Pre " + student.Result.ElementAt(0).course_code;
        //        }
        //        else
        //        {
        //            chart.examination = student.Result.ElementAt(0).course_code;
        //        }
        //        chart.NAME = student.Result.ElementAt(0).NAME;
        //        chart.todaydate = DateTime.Now.ToString("dd-MMM-yyyy");
        //        chart.batch = student.Result.ElementAt(0).batch;
        //        var stddetails1 = iStudentRepository.Get_Student_Data(chart.regdno);

        //        if (Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2021 && student.Result.ElementAt(0).degree_code == "UG")
        //        {

        //            if (stddetails1.Result.ElementAt(0).regdno.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
        //            {
        //                chart.instructn = "FinalYeardiv";
        //            }
        //            else
        //            {
        //                chart.instructn = "A21";
        //            }
        //        }
        //        else
        //        {
        //            if (stddetails1.Result.ElementAt(0).regdno.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
        //            {
        //                chart.instructn = "FinalYeardiv";
        //            }
        //            else
        //            {
        //                chart.instructn = "B21";
        //            }
        //        }
        //        if (chart.type1 != null)

        //        {
        //            var stddetails = iStudentRepository.Get_Hall_Ticket_Details(chart.regdno, chart.type1, chart.CURRSEM);
        //            if (stddetails.Result.Count() > 0)
        //            {
        //                chart.regdno = stddetails.Result.ElementAt(0).regdno;
        //                chart.branch_name = stddetails.Result.ElementAt(0).branch_name;
        //                chart.examination = stddetails.Result.ElementAt(0).COURSE_NAME;
        //                chart.COURSE_NAME = stddetails.Result.ElementAt(0).COURSE_NAME;
        //                chart.NAME = stddetails.Result.ElementAt(0).NAME;
        //                chart.batch = stddetails.Result.ElementAt(0).batch;
        //                chart.Exam_Venue = stddetails.Result.ElementAt(0).Exam_Venue;
        //                chart.MONTH = stddetails.Result.ElementAt(0).MONTH;
        //                chart.id1 = stddetails.Result.ElementAt(0).SEMESTER;
        //                var sem = chart.id1;
        //                if (sem == "1")
        //                {
        //                    chart.psntsem = "I";
        //                }
        //                else if (sem == "2")
        //                {
        //                    chart.psntsem = "II";
        //                }
        //                else if (sem == "3")
        //                {
        //                    chart.psntsem = "III";
        //                }
        //                else if (sem == "4")
        //                {
        //                    chart.psntsem = "IV";
        //                }
        //                else if (sem == "5")
        //                {
        //                    chart.psntsem = "V";
        //                }
        //                else if (sem == "6")
        //                {
        //                    chart.psntsem = "VI";
        //                }
        //                else if (sem == "7")
        //                {
        //                    chart.psntsem = "VII";
        //                }
        //                else if (sem == "8")
        //                {
        //                    chart.psntsem = "VIII";
        //                }
        //                else if (sem == "9")
        //                {
        //                    chart.psntsem = "IX";
        //                }
        //                else if (sem == "10")
        //                {
        //                    chart.psntsem = "X";
        //                }
        //                else if (sem == "11")
        //                {
        //                    chart.psntsem = "XI";
        //                }
        //                else if (sem == "12")
        //                {
        //                    chart.psntsem = "XII";
        //                }
        //                else if (sem == "13")
        //                {
        //                    chart.psntsem = "XIII";
        //                }
        //                else if (sem == "14")
        //                {
        //                    chart.psntsem = "XIV";

        //                }
        //                else if (sem == "15")
        //                {
        //                    chart.psntsem = "XV";
        //                }
        //                else
        //                {
        //                }


        //                admission.note = chart;
        //                admission.notes = await iStudentRepository.getstudentPHDhallticket(chart);


        //            }

        //            else
        //            {
        //                admission.note = chart;
        //                admission.notes = await iStudentRepository.getstudentPHDhallticket(chart);

        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Redirect("Errorpage.aspx");
        //    }
        //    return (admission);
        //}
        [HttpGet]
        public async Task<ActionResult> getadmissions()
        {
            try
            {

                // IEnumerable<Event> Events = await iStudentRepository.getevents(Session["campus_code"].ToString());

                return View("Admissions");
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        [HttpGet]
        public async Task<ActionResult> CDLMigration()
        {

            return View("CDLMigration");
        }




        [HttpGet]

        public async Task<JsonResult> getCDLMigrationData()
        {

            //Session["REGDNO"] = "1210116117";
            string REGDNO = Session["uid"].ToString();
            string TY = "";
            string QCText = "";
            try
            {
                List<CDLCertificates> data = await iStudentRepository.getcdlMigration(REGDNO);
                List<CDLCertificates> data1 = await iStudentRepository.getdateofcompletion(REGDNO);
                string doc = "";
                if (data1.Count() > 0)
                {
                    doc = data1.FirstOrDefault().DOC;
                }
                if (data.Count() > 0)
                {
                    TY = TY + "$" + REGDNO.Trim();
           
                    QCText = QCText + "https://studentlms.gitam.edu/Home/CDL_Migration_Certificate?QT=" + TY + "";
                    data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");

                    string AdmissionDate = Convert.ToString(Session["AdmissionDate"]);
                    data.ElementAt(0).DOA = AdmissionDate.Trim().Equals("") ? "--" : Convert.ToDateTime(AdmissionDate).ToString("dd-MMM-yyyy");
                    data.ElementAt(0).DOC = doc;
                }
                GenerateMyQCCodeMigration(QCText);
                //data.ElementAt(0).qrtext = QCText;


                string str = "<div><img src=\"images/MGDynamicQR.jpg\" width =\"175\" style=\"margin-left:-22px;margin-top:-70px\" /></div>";

                data.ElementAt(0).qrtext = str;



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]

        [ValidateInput(false)]
        public async Task<ActionResult> CDL_Migration_Certificate(string QT)
        {

            //Session["REGDNO"] = "1210116117";
            string regnofrom = "";
            List<CDLCertificates> data = new List<CDLCertificates>();
            try
            {
                if (QT != null)
                {
                    //  string reg = Session["uid"].ToString();

                    if (QT.Split('$')[0] != null && QT.Split('$')[1] != null)
                    {
                        regnofrom = QT.Split('$')[1].ToString();

                    }
                    Session["regdnofrom"] = regnofrom;
                    string reg = QT.Split('$')[1].ToString();
                    List<CDLCertificates> data1 = await iStudentRepository.getdateofcompletion(regnofrom);
                    string doc = "";
                    if (data1.Count() > 0)
                    {
                        doc = data1.FirstOrDefault().DOC;
                    }
                    data = await iStudentRepository.getcdlMigration(regnofrom);
                    if (data.Count() > 0)
                    {
                        string AdmissionDate = Convert.ToString(Session["AdmissionDate"]);
                        data.ElementAt(0).DOA = AdmissionDate.Trim().Equals("") ? "--" : Convert.ToDateTime(AdmissionDate).ToString("dd-MMM-yyyy");
                        data.ElementAt(0).DOC = doc;
                        data.ElementAt(0).sysdate = System.DateTime.Now.ToString("dd-MMM-yyyy");
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PartialView("CDL_Migration_Certificate", data);
        }




        private void GenerateMyQCCodeMigration(string QCText)
        {
            try
            {
                var QCwriter = new BarcodeWriter();
                QCwriter.Format = BarcodeFormat.QR_CODE;
                var result = QCwriter.Write(QCText);
                string path = Server.MapPath("~/images/MGDynamicQR.jpg");
                var barcodeBitmap = new Bitmap(result);

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(path,
                       FileMode.Create, FileAccess.ReadWrite))
                    {
                        barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }

        [HttpGet]
        public async Task<ActionResult> CDLHallTicket()
        {
            CDLhallticketsummary hallticket = new CDLhallticketsummary();
            try
            {
                string REGDNO = Session["uid"].ToString();
                string course = "";
                string branch = Convert.ToString(Session["branch_code"]);

                IEnumerable<CDLHallticket> student = await iStudentRepository.getCDLRegularStudent(REGDNO);
                if (student.Count() > 0)
                {
                    hallticket.CDLRegular = await iStudentRepository.getCDLRegular(REGDNO);
                    hallticket.CDLSupply = await iStudentRepository.getCDLSupply(REGDNO);
                    hallticket.CDLbetterment = await iStudentRepository.getCDLBetterment(REGDNO);
                    IEnumerable<CDLHallticket> subjects = await iStudentRepository.getCDLRegular(REGDNO);
                    hallticket.Studentdetails = student;
                    var subbranch = await iStudentRepository.getCDLsubBranch(course, branch);


                    String branchname = subbranch.FirstOrDefault().branch_name;
                    student.FirstOrDefault().branch_name = branchname;
                    string yeardate = subjects.FirstOrDefault().exam_date.Split('-')[1] + " " + subjects.FirstOrDefault().exam_date.Split('-')[2];

                    student.FirstOrDefault().yeardate = yeardate;


                }
                else
                {
                    hallticket.CDLRegular = new List<CDLHallticket>();
                    hallticket.CDLSupply = new List<CDLHallticket>();
                    hallticket.CDLbetterment = new List<CDLHallticket>();
                    hallticket.Studentdetails = new List<CDLHallticket>();
                }



            }
            catch (Exception EX)
            {

            }

            return View("CDLHallTicket", hallticket);
        }



        [HttpGet]

        public async Task<JsonResult> getRegularhallticket(string semester)
        {


            string REGDNO = Session["uid"].ToString();
            String course = null;
            String branch = null;
            String sem = null;
            CDLhallticketsummary hallticket = new CDLhallticketsummary();

            try
            {




                IEnumerable<CDLHallticket> student = await iStudentRepository.getCDLRegularStudent(REGDNO);



                String year = Convert.ToString(student.FirstOrDefault().CURR_SEM);

                // CDLSUPP_STUDENT_SUBJECT cdl = cls.getstudentsupp(regno);
                sem = Convert.ToString(student.FirstOrDefault().CURR_SEM);
                course = student.FirstOrDefault().COURSE_CODE;
                branch = student.FirstOrDefault().BRANCH_CODE;

                //  STUDENT_SUBJECT sub = cls.getsubject(sem, course, branch);

                var subbranch = await iStudentRepository.getCDLsubBranch(course, branch);


                String branchname = subbranch.FirstOrDefault().branch_name;
                student.FirstOrDefault().branch_name = branchname;

                // lblExamcenter.Text = Convert.ToString(student.EXAM_CENTRE);
                // String sem=student.sem;

                student.FirstOrDefault().CURR_SEM = NumberToRoman(Convert.ToInt32(student.FirstOrDefault().CURR_SEM));


                // lblYear.Text = Convert.ToString(student.sem.Substring(0, 2)) + " YEAR";
                //lblName.Text = Convert.ToString(student.NAME);
                //lblparent.Text = Convert.ToString(student.FATHER_NAME);
                //// lblreg.Text =Convert.ToString();
                //lblExmn.Text = student.COURSE_CODE;
                //Label2.Text = student.REGDNO;
                // lblec.Text = get_exam_center(regno, year, student.BATCH, student.ADMISSION_TYPE);

                //  hallticket.CDLRegular = await iStudentRepository.getCDLRegular(REGDNO);
                IEnumerable<CDLHallticket> subjects = await iStudentRepository.getCDLRegular(REGDNO);
                List<CDLHallticket> subjs = new List<CDLHallticket>();
                foreach (var sb in subjects)
                {
                    CDLHallticket subj = new CDLHallticket();
                    subj.SUB_CODE = sb.subject_code;
                    subj.SUB_DATE = sb.exam_date;
                    subj.SUB_NAME = sb.subject_name;
                    subj.passcode = sb.passcode;
                    subj.passkey = sb.TIMINGS;
                    subj.exm_center = sb.EXAM_CENTRE;
                    //    Response.Write(subj.SUB_NAME);


                    subjs.Add(subj);
                    //ec = sb.EXAM_CENTRE;
                }

                student.FirstOrDefault().EXAM_CENTRE = subjects.FirstOrDefault().EXAM_CENTRE;
                if (student.FirstOrDefault().specialization_code == null)
                {
                    student.FirstOrDefault().branch1 = "";

                }
                {
                    if (student.FirstOrDefault().specialization_code.Equals("NONE"))
                    {
                        student.FirstOrDefault().branch1 = "";
                    }
                    else
                    {
                        student.FirstOrDefault().branch1 = " -" + student.FirstOrDefault().specialization_code;
                    }
                }

                string yeardate = subjects.FirstOrDefault().exam_date.Split('-')[1] + " " + subjects.FirstOrDefault().exam_date.Split('-')[2];

                student.FirstOrDefault().yeardate = yeardate;
                student.FirstOrDefault().current_date = DateTime.Now.ToString("dd/MM/yyyy");
                hallticket.Studentdetails = student;
                hallticket.CDLRegular = subjs;

            }
            catch (Exception ex)
            {

            }

            return Json(hallticket, JsonRequestBehavior.AllowGet);
        }


        public string NumberToRoman(int number)
        {


            if (number < 0 || number > 3999)
            {
                throw new ArgumentException("Value must be in the range 0 – 3,999.");
            }
            if (number == 0) return "N";

            int[] values = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] numerals = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            StringBuilder result = new StringBuilder();

            // Loop through each of the values to diminish the number
            for (int i = 0; i < 13; i++)
            {

                // If the number being converted is less than the test value, append
                // the corresponding numeral or numeral pair to the resultant string
                while (number >= values[i])
                {
                    number -= values[i];
                    result.Append(numerals[i]);
                }

            }

            return result.ToString();

        }



        [HttpGet]

        public async Task<JsonResult> getSupplyhallticket(string semester)
        {


            string REGDNO = Session["uid"].ToString();
            String course = null;
            String branch = null;
            String sem = null;
            CDLhallticketsummary hallticket = new CDLhallticketsummary();

            try
            {




                IEnumerable<CDLHallticket> student = await iStudentRepository.getCDLRegularStudent(REGDNO);



                String year = Convert.ToString(student.FirstOrDefault().CURR_SEM);

                // CDLSUPP_STUDENT_SUBJECT cdl = cls.getstudentsupp(regno);
                sem = Convert.ToString(semester);
                course = student.FirstOrDefault().COURSE_CODE;
                branch = student.FirstOrDefault().BRANCH_CODE;

                //  STUDENT_SUBJECT sub = cls.getsubject(sem, course, branch);

                var subbranch = await iStudentRepository.getCDLsubBranch(course, branch);


                String branchname = subbranch.FirstOrDefault().branch_name;
                student.FirstOrDefault().branch_name = branchname;

                // lblExamcenter.Text = Convert.ToString(student.EXAM_CENTRE);
                // String sem=student.sem;

                student.FirstOrDefault().CURR_SEM = NumberToRoman(Convert.ToInt32(semester));


                if (student.FirstOrDefault().ADMISSION_TYPE.Equals("ONLINE") || student.FirstOrDefault().ADMISSION_TYPE.Equals("OFFLINE"))
                {
                    student.FirstOrDefault().ADMISSION_TYPE = "  Exam Mode:-" + student.FirstOrDefault().ADMISSION_TYPE;
                }
                IEnumerable<CDLHallticket> subjects = await iStudentRepository.getCDLSupplySubjects(REGDNO);
                subjects = subjects.Where(m => m.CURR_SEM == semester);
                List<CDLHallticket> subjs = new List<CDLHallticket>();
                foreach (var sb in subjects)
                {
                    CDLHallticket subj = new CDLHallticket();
                    subj.SUB_CODE = sb.subject_code;
                    subj.SUB_DATE = sb.exam_date;
                    subj.SUB_NAME = sb.subject_name;
                    subj.passcode = sb.passcode;
                    subj.passkey = sb.TIMINGS;
                    subj.exm_center = sb.EXAM_CENTRE;
                    if (student.FirstOrDefault().ADMISSION_TYPE.Equals(null) || student.FirstOrDefault().ADMISSION_TYPE.Equals(""))
                    {
                        if (year.Equals("1") || year.Equals("2"))
                        {

                            var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                            subj.passkey = subtime.FirstOrDefault().TIMINGS;
                        }

                        else
                        {

                            var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                            subj.passkey = subtime.FirstOrDefault().TIMINGS;
                        }
                    }
                    else
                    {
                        if (student.FirstOrDefault().ADMISSION_TYPE.Equals("ONLINE"))
                        {
                            if (year.Equals("1") || year.Equals("2"))
                            {
                                subj.passkey = "9:00 to :11:30 AM ";
                            }

                            else
                            {

                                subj.passkey = "2:00 to 4:30 PM ";
                            }

                        }
                        else
                        {
                            if (sem.Equals("1") || sem.Equals("2"))
                            {
                                //subj.passkey = cls.gettime(sb.SUB_CODE, sb.BATCH.Split('-')[0] + "-" + (Convert.ToInt32(sb.BATCH.Split('-')[0]) + 1), sb.SEM, sb.course_code);
                                var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                                subj.passkey = subtime.FirstOrDefault().TIMINGS;
                            }

                            else
                            {

                                //subj.passkey = cls.gettime(sb.SUB_CODE, sb.BATCH.Split('-')[0] + "-" + (Convert.ToInt32(sb.BATCH.Split('-')[0]) + 1), sb.SEM, sb.course_code);
                                var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                                subj.passkey = subtime.FirstOrDefault().TIMINGS;
                            }
                        }
                    }

                    subjs.Add(subj);
                    //ec = sb.EXAM_CENTRE;
                }

                student.FirstOrDefault().EXAM_CENTRE = subjects.FirstOrDefault().EXAM_CENTRE;
                if (student.FirstOrDefault().specialization_code == null)
                {
                    student.FirstOrDefault().branch1 = "";

                }
                {
                    if (student.FirstOrDefault().specialization_code.Equals("NONE"))
                    {
                        student.FirstOrDefault().branch1 = "";
                    }
                    else
                    {
                        student.FirstOrDefault().branch1 = " -" + student.FirstOrDefault().specialization_code;
                    }
                }

                string yeardate = subjects.FirstOrDefault().exam_date.Split('-')[1] + " " + subjects.FirstOrDefault().exam_date.Split('-')[2];

                student.FirstOrDefault().yeardate = yeardate;
                student.FirstOrDefault().current_date = DateTime.Now.ToString("dd/MM/yyyy");
                hallticket.Studentdetails = student;
                hallticket.CDLRegular = subjs;

            }
            catch (Exception ex)
            {

            }

            return Json(hallticket, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public async Task<JsonResult> getBetterhallticket(string semester)
        {


            string REGDNO = Session["uid"].ToString();
            String course = null;
            String branch = null;
            String sem = null;
            CDLhallticketsummary hallticket = new CDLhallticketsummary();

            try
            {
                IEnumerable<CDLHallticket> student = await iStudentRepository.getCDLRegularStudent(REGDNO);
                String year = Convert.ToString(student.FirstOrDefault().CURR_SEM);
                sem = Convert.ToString(semester);
                course = student.FirstOrDefault().COURSE_CODE;
                branch = student.FirstOrDefault().BRANCH_CODE;
                var subbranch = await iStudentRepository.getCDLsubBranch(course, branch);
                String branchname = subbranch.FirstOrDefault().branch_name;
                student.FirstOrDefault().branch_name = branchname;
                student.FirstOrDefault().CURR_SEM = NumberToRoman(Convert.ToInt32(semester));
                if (student.FirstOrDefault().ADMISSION_TYPE.Equals("ONLINE") || student.FirstOrDefault().ADMISSION_TYPE.Equals("OFFLINE"))
                {
                    student.FirstOrDefault().ADMISSION_TYPE = "  Exam Mode:-" + student.FirstOrDefault().ADMISSION_TYPE;
                }
                IEnumerable<CDLHallticket> subjects = await iStudentRepository.getCDLBettermentSubjects(REGDNO);
                subjects = subjects.Where(m => m.CURR_SEM == semester);
                List<CDLHallticket> subjs = new List<CDLHallticket>();
                foreach (var sb in subjects)
                {
                    CDLHallticket subj = new CDLHallticket();
                    subj.SUB_CODE = sb.subject_code;
                    subj.SUB_DATE = sb.exam_date;
                    subj.SUB_NAME = sb.subject_name;
                    subj.passcode = sb.passcode;
                    subj.passkey = sb.TIMINGS;
                    subj.exm_center = sb.EXAM_CENTRE;
                    if (student.FirstOrDefault().ADMISSION_TYPE.Equals(null) || student.FirstOrDefault().ADMISSION_TYPE.Equals(""))
                    {
                        if (year.Equals("1") || year.Equals("2"))
                        {

                            var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                            subj.passkey = subtime.FirstOrDefault().TIMINGS;
                        }

                        else
                        {

                            var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                            subj.passkey = subtime.FirstOrDefault().TIMINGS;
                        }
                    }
                    else
                    {
                        if (student.FirstOrDefault().ADMISSION_TYPE.Equals("ONLINE"))
                        {
                            if (year.Equals("1") || year.Equals("2"))
                            {
                                subj.passkey = "9:00 to :11:30 AM ";
                            }

                            else
                            {

                                subj.passkey = "2:00 to 4:30 PM ";
                            }

                        }
                        else
                        {
                            if (sem.Equals("1") || sem.Equals("2"))
                            {
                                //subj.passkey = cls.gettime(sb.SUB_CODE, sb.BATCH.Split('-')[0] + "-" + (Convert.ToInt32(sb.BATCH.Split('-')[0]) + 1), sb.SEM, sb.course_code);
                                var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                                subj.passkey = subtime.FirstOrDefault().TIMINGS;
                            }

                            else
                            {

                                //subj.passkey = cls.gettime(sb.SUB_CODE, sb.BATCH.Split('-')[0] + "-" + (Convert.ToInt32(sb.BATCH.Split('-')[0]) + 1), sb.SEM, sb.course_code);
                                var subtime = await iStudentRepository.getCDLSupplySubjectstIMINGS(sb.subject_code, sb.batch.Split('-')[0] + "-" + (Convert.ToInt32(sb.batch.Split('-')[0]) + 1), sb.CURR_SEM, sb.course_code);

                                subj.passkey = subtime.FirstOrDefault().TIMINGS;
                            }
                        }
                    }

                    subjs.Add(subj);
                    //ec = sb.EXAM_CENTRE;
                }

                student.FirstOrDefault().EXAM_CENTRE = subjects.FirstOrDefault().EXAM_CENTRE;
                if (student.FirstOrDefault().specialization_code == null)
                {
                    student.FirstOrDefault().branch1 = "";

                }
                {
                    if (student.FirstOrDefault().specialization_code.Equals("NONE"))
                    {
                        student.FirstOrDefault().branch1 = "";
                    }
                    else
                    {
                        student.FirstOrDefault().branch1 = " -" + student.FirstOrDefault().specialization_code;
                    }
                }

                string yeardate = subjects.FirstOrDefault().exam_date.Split('-')[1] + " " + subjects.FirstOrDefault().exam_date.Split('-')[2];

                student.FirstOrDefault().yeardate = yeardate;
                student.FirstOrDefault().current_date = DateTime.Now.ToString("dd/MM/yyyy");
                hallticket.Studentdetails = student;
                hallticket.CDLRegular = subjs;

            }
            catch (Exception ex)
            {

            }

            return Json(hallticket, JsonRequestBehavior.AllowGet);
        }


        // certificate fee start
        public async Task<ActionResult> CertificatesFee()
        {
            certificatescollection summary = new certificatescollection();
            string WhreCond = "";
            string currentYear = DateTime.Now.Year.ToString();
            string batchcode = Session["batchend"].ToString();
            int currentYear1 = Convert.ToInt32(currentYear);
            int batchcode1 = Convert.ToInt32(batchcode);
            Session["changing"] = "Verified student details";
            string changing = Session["changing"].ToString();
            string REGDNO = Session["uid"].ToString();
            if (Session["STUDENTstatus"].ToString().Trim().ToUpper().Equals("S"))
            {
                WhreCond = "  AND  SStatus='S' ";
            }
            else if (Session["STUDENTstatus"].ToString().Trim().ToUpper().Equals("D") && (batchcode1 <= currentYear1))
            {
                WhreCond = "";
            }
            else if (Session["STUDENTstatus"].ToString().Trim().Equals("Det_Dis"))
            {
                WhreCond = "  AND  ISstatusDis='Y'  ";
            }
            else
            {
                WhreCond = "  AND  DISPLAY_FLAG='Y'  ";

            }
            summary.certificateslist = await iStudentRepository.getcertificate_labels(WhreCond, changing);
            IEnumerable<Certificatesnew> data = null;
            List<Certificatesnew> test = new List<Certificatesnew>();
            for (int k = 0; k <= (summary.certificateslist.Count()) - 1; k++)
            {
                string certificatetype = summary.certificateslist.ElementAt(k).CERTIFICATE_TYPE;

                data = await iStudentRepository.getcertificate_original_duplicate(REGDNO, certificatetype);

                string originaldistrue = "";
                string duplicatedistrue = "";
                string originalmsg = "";

                if (data.Count() > 0)
                {
                    originaldistrue = "0";
                    duplicatedistrue = "1";
                    originalmsg = "Alredy Applied... ";
                }

                else
                {
                    originaldistrue = "1";
                    duplicatedistrue = "0";
                    originalmsg = "To take Duplicate after applying the original certificate.. ";
                }
                string CMPUS_CODE = summary.certificateslist.ElementAt(k).CMPUS_CODE;
                test.Add(new Certificatesnew
                {
                    originaldistrue = originaldistrue,
                    duplicatedistrue = duplicatedistrue,
                    originalmsg = originalmsg,
                    CMPUS_CODE = CMPUS_CODE,
                    COLLEGE_CODE = summary.certificateslist.ElementAt(k).COLLEGE_CODE?.ToString(),
                    COURSE_CODE = summary.certificateslist.ElementAt(k).COURSE_CODE?.ToString(),
                    CERTIFICATE_NAME = summary.certificateslist.ElementAt(k).CERTIFICATE_NAME?.ToString(),
                    CERTIFICATE_TYPE_ORIGINAL = summary.certificateslist.ElementAt(k).CERTIFICATE_TYPE_ORIGINAL?.ToString(),
                    CERTIFICATE_TYPE_DUPLICATE = summary.certificateslist.ElementAt(k).CERTIFICATE_TYPE_DUPLICATE?.ToString(),
                    CERTIFICATE_TYPE_TRIPLECATE = summary.certificateslist.ElementAt(k).CERTIFICATE_TYPE_TRIPLECATE?.ToString(),
                    CERTIFICATE_FEE_ORIGINAL = summary.certificateslist.ElementAt(k).CERTIFICATE_FEE_ORIGINAL?.ToString(),
                    CERTIFICATE_FEE_DUPLICATE = summary.certificateslist.ElementAt(k).CERTIFICATE_FEE_DUPLICATE?.ToString(),
                    CERTIFICATE_FEE_TRIPLICATE = summary.certificateslist.ElementAt(k).CERTIFICATE_FEE_TRIPLICATE?.ToString(),
                    ORIGINAL_ADITIONAL_FEE = summary.certificateslist.ElementAt(k).ORIGINAL_ADITIONAL_FEE?.ToString(),
                    DUPLICATE_ADITIONAL_FEE = summary.certificateslist.ElementAt(k).DUPLICATE_ADITIONAL_FEE?.ToString(),
                    BY_POST_FEE = summary.certificateslist.ElementAt(k).BY_POST_FEE?.ToString(),
                    CERTIFICATE_TYPE = summary.certificateslist.ElementAt(k).CERTIFICATE_TYPE?.ToString(),
                    REQ_DESC = summary.certificateslist.ElementAt(k).REQ_DESC?.ToString(),
                    CATEGORY = summary.certificateslist.ElementAt(k).CATEGORY?.ToString(),
                    IS_ACTIVE = summary.certificateslist.ElementAt(k).IS_ACTIVE?.ToString(),
                    DISPLAY_FLAG = summary.certificateslist.ElementAt(k).DISPLAY_FLAG?.ToString(),
                    SSTATUS = summary.certificateslist.ElementAt(k).SSTATUS?.ToString(),
                    Corder = summary.certificateslist.ElementAt(k).Corder?.ToString(),
                    ISstatusDis = summary.certificateslist.ElementAt(k).ISstatusDis?.ToString()

                });
            }

            summary.certificateoridupli = test;

            IEnumerable<Certificatesnew> semesterlist = await iStudentRepository.getsemester(REGDNO);
            ViewBag.semesterlist = semesterlist.Select(testgroup => new SelectListItem
            {
                Text = testgroup.semesterdropdown.ToString(),
                Value = testgroup.semesterdropdown.ToString()
            });

            return View("CertificatesFee", summary);
        }
        [HttpGet]
        public async Task<JsonResult> semselectedoriginal(string sem, string month, string year, string addfeeOrg)
        {
            string REGDNO = Session["uid"].ToString();
            IEnumerable<Certificatesnew> data = await iStudentRepository.semselectedoriginal(sem, month, year, REGDNO);
            if (data.Count() > 0 && !string.IsNullOrEmpty(data.FirstOrDefault().date))
            {

                if (Session["COLLEGE_CODE"].ToString() == "GIMSR")
                {
                    data.FirstOrDefault().duefee = 0;
                    data.FirstOrDefault().addtextbox = true;
                }
                else
                {
                    data.FirstOrDefault().addtextbox = false;
                    string selectdate = data.FirstOrDefault().date.ToString();
                    DateTime date = DateTime.Parse(selectdate);

                    DateTime futureDate = date.AddYears(1);
                    DateTime today = DateTime.Today;
                    if (today <= futureDate)
                    {
                        data.FirstOrDefault().nofee = 0;
                    }
                    else
                    {

                        data.FirstOrDefault().duefee = data.FirstOrDefault().duefee + (Convert.ToInt32(addfeeOrg));
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> semselectedoriginalOd(string addfeeOrg)
        {
            string REGDNO = Session["uid"].ToString();
            IEnumerable<Certificatesnew> data = await iStudentRepository.semselectedoriginal("", "", "", REGDNO);
            if (data.Count() > 0 && !string.IsNullOrEmpty(data.FirstOrDefault().date))
            {
                if (Session["COLLEGE_CODE"].ToString() == "GIMSR")
                {
                    data.FirstOrDefault().duefee = 0;
                    data.FirstOrDefault().addtextbox = true;
                }
                else
                {
                    data.FirstOrDefault().addtextbox = false;
                    string selectdate = data.FirstOrDefault().date.ToString();
                    DateTime date = DateTime.Parse(selectdate);

                    DateTime futureDate = date.AddYears(1);
                    DateTime today = DateTime.Today;
                    if (today <= futureDate)
                    {
                        data.FirstOrDefault().nofee = 0;
                    }
                    else
                    {
                        int futureyear = today.Year;
                        int presentyear = date.Year;
                        int finecount = futureyear - presentyear;
                        data.FirstOrDefault().duefee = data.FirstOrDefault().duefee + (Convert.ToInt32(addfeeOrg) * finecount);
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<JsonResult> semselectedoriginalpcmg(string addfeeOrg)
        {
            string REGDNO = Session["uid"].ToString();
            IEnumerable<Certificatesnew> data = await iStudentRepository.semselectedoriginal("", "", "", REGDNO);
            if (data.Count() > 0 && !string.IsNullOrEmpty(data.FirstOrDefault().date))
            {
                if (Session["COLLEGE_CODE"].ToString() == "GIMSR")
                {
                    data.FirstOrDefault().duefee = 0;
                    data.FirstOrDefault().addtextbox = true;
                }
                else
                {
                    data.FirstOrDefault().addtextbox = false;
                    string selectdate = data.FirstOrDefault().date.ToString();
                    DateTime date = DateTime.Parse(selectdate);

                    DateTime futureDate = date.AddYears(1);
                    DateTime today = DateTime.Today;
                    if (today <= futureDate)
                    {
                        data.FirstOrDefault().nofee = 0;
                    }
                    else
                    {
                        data.FirstOrDefault().duefee = data.FirstOrDefault().duefee + (Convert.ToInt32(addfeeOrg));
                    }
                }
            }

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<JsonResult> semselectedonchange(string sem, string month, string year, string addfeeOrg, string certype)
        {
            string REGDNO = Session["uid"].ToString();
            IEnumerable<Certificatesnew> data = await iStudentRepository.semselectedonchange(sem, month, year, REGDNO, certype);
            if (data.Count() > 0)
            {

                data.FirstOrDefault().orbt = false;

            }
            else
            {
                data.FirstOrDefault().orbt = true;
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<int> insertdataCERTIFICATE(HttpPostedFileBase file, string sem, string month, string year, string category, string certificate, string type, int amt, string Certype, int bypost)
        {
            int data = 0;
            try
            {
                string regdno = Convert.ToString(Session["uid"]);
                string coursecode = Convert.ToString(Session["course_code"]);
                string BATCH = Convert.ToString(Session["BATCH"]);
                string currentsem = Convert.ToString(Session["curr_sem"]);
                string degreecode = Convert.ToString(Session["degree_code"]);
                string campus = Session["campus_code"].ToString();
                string college = Session["college_code"].ToString();
                string sessionid = Session.SessionID.ToString();
                string sname = Session["studname"].ToString();
                string branch_code = Session["branch_code"].ToString();
                int referinceid = Convert.ToInt32(Session["ref_id"]);
                var pubfilepath = "";
                pubfilepath = Upload_Certificates_pathsave(certificate, file);

                data = await iStudentRepository.insertdatacertificate(referinceid, branch_code, college, regdno, coursecode, BATCH, currentsem, certificate, type, category, sem, month, year, amt, bypost, degreecode, sessionid, pubfilepath, campus, sname);

                if (data >= 0)
                {
                    if (type.ToString().Trim().Equals("DUPLICATE") || type.ToString().Trim().Equals("TRIPLICATE"))
                        Upload_Certificates(file, certificate);
                    else if (Certype.Equals("CNS") || Certype.Equals("CNG") || Certype.Equals("OS"))
                    {
                        Upload_Certificates(file, certificate);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return data;
        }
        public async Task<int> inserttotals(string totalamount, string category)
        {
            try
            {

                string regdno = Convert.ToString(Session["uid"]);
                string refid = Session["ref_id"].ToString();
                string sname = Session["studname"].ToString();
                Session["totalamount"] = totalamount.ToString();
                int data = await iStudentRepository.inserttotals(regdno, sname, category, totalamount, refid);

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> getreferenceid()
        {
            try
            {
                int refid = await iStudentRepository.getreference_id();
                Session["ref_id"] = refid.ToString();
                return refid;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public string Upload_Certificates_pathsave(string certificateType, HttpPostedFileBase file)
        {
            string msg = "";
            string filePath = "";
            try
            {

                if (file != null && file.ContentLength > 0)
                {
                    // Map the path to save the file
                    string pathFolder = MapPathForCertificateType(certificateType);
                    string ext = Path.GetExtension(file.FileName);

                    if (ext == ".pdf" || ext == ".PDF")
                    {
                        if (file.ContentLength < 10000000)
                        {
                            // Generate a unique file name
                            string fileName = $"{certificateType.Trim()}_{Session["uid"].ToString()}_{Session["ref_id"].ToString()}{ext}";
                            filePath = Path.Combine(pathFolder, fileName);


                            // file.SaveAs(filePath);
                            msg = "success";
                            // Do any other processing related to the file
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        // Handle invalid file extension error
                    }
                }
                else
                {
                    // Handle no file uploaded error
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return filePath;
        }


        public string MapPathForCertificateType(string certificateType)
        {
            string pathFolder = "";

            switch (certificateType.Trim())
            {
                case "GRADECARD":
                    pathFolder = "GRADE_CARD";
                    break;
                case "RANK_CARD":
                    pathFolder = "RANK_CARD";
                    break;
                case "MIGRATION":
                    pathFolder = "MIGRATION";
                    break;
                case "PCMG":
                    pathFolder = "PCMG";
                    break;
                case "CORRECTION":
                    pathFolder = "CORRECTION";
                    break;
                case "CHANGE NAME":
                    pathFolder = "CHANGE_NAME";
                    break;
                case "ONLINETRANSCRIPTS":
                    pathFolder = "ONLINE_TRANSCRIPTS";
                    break;
                case "CNC":
                    pathFolder = "CONVERSION";
                    break;
                default:
                    // Handle unknown certificate type
                    break;
            }

            return Path.Combine(Server.MapPath("~/ORIGINAL_DEGREE/"), pathFolder);
        }

        [HttpPost]
        public string Upload_Certificates(HttpPostedFileBase file, string certificateType)
        {
            string filePath = "";
            try
            {


                if (file != null && file.ContentLength > 0)
                {

                    string pathFolder = MapPathForCertificateType(certificateType);
                    string ext = System.IO.Path.GetExtension(file.FileName);

                    if (ext == ".pdf" || ext == ".PDF")
                    {
                        if (file.ContentLength <= 10000000)
                        {
                            // Handle file saving
                            string fileName = $"{certificateType.Trim()}_{Session["uid"].ToString()}_{Session["ref_id"].ToString()}{ext}";
                            filePath = Path.Combine(pathFolder, fileName);


                            file.SaveAs(filePath);
                        }
                        else
                        {
                            // Handle file size exceeded error
                        }
                    }
                    else
                    {
                        // Handle invalid file extension error
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
            }
            return filePath;

        }

        public async Task<ActionResult> getcertificateconfirmation()
        {
            try
            {
                //int referinceid = await getreferenceid();
                string regdno = Convert.ToString(Session["uid"]);
                string refid = Convert.ToString(Session["ref_id"]);
                string sname = Convert.ToString(Session["studname"]);
                string sessionid = Convert.ToString(Session.SessionID);
                IEnumerable<Certificatesnew> data = await iStudentRepository.getcertificateconfirmation(regdno, refid, sname, sessionid);
                if (data.Count() > 0)
                {
                    for (int k = 0; k < data.Count(); k++)
                    {
                        Session["PROCESS_TYPE"] = data.ElementAt(k).REQUEST_DESC.ToString();
                    }

                }
                else
                {
                    Session["PROCESS_TYPE"] = "";
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult insertcertificateconfirmation()
        {
            string vregdno = Convert.ToString(Session["uid"]);
            string vfeeamount = Session["totalamount"].ToString();
            string vcategory = "CERTIFICATE";
            string vsubcategory = Session["PROCESS_TYPE"].ToString();
            string vrefid = Session["ref_id"].ToString();
            string mobile = Session["parentmobile"].ToString();
            string p = "REGDNO=" + vregdno + "&FEE=" + vfeeamount + "&CATEGORY=" + vcategory + "&SUBCATEGORY=" + vsubcategory + "&REFID=" + vrefid;
            // string p = "REGDNO=" + vregdno + "&MOBILE=" + mobile;
            // p = Encrypt(p, true);
            EnencryptEngine ee = new EnencryptEngine();
            p = ee.Encrypt1(p, true);
            // string redirectUrl = "https://onlinepay.gitam.edu/doefeepayments/doefeepayment?" + p;
            string redirectUrl = p;
            return Json(redirectUrl);
        }

        class EnencryptEngine 
        {
            /// <summary>
            /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
            /// </summary>
            /// <param name="toEncrypt">string to be encrypted</param>
            /// <param name="useHashing">use hashing? send to for extra secirity</param>
            /// <returns></returns>
            /// 

            /// <summary>
            /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
            /// </summary>
            /// <param name="cipherString">encrypted string</param>
            /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
            /// <returns></returns>
            /// 
            public string Encrypt1(string toEncrypt, bool useHashing)
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                // Get the key from config file
                //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
                //System.Windows.Forms.MessageBox.Show(key);
                string key = "xltempxltestpayguconfsandargument";
                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            public string Decrypt1(string cipherString, bool useHashing)
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(cipherString.Replace(' ', '+'));

                System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
                //Get your key from config file to open the lock!
                string key = "xltempxltestpayguconfsandargument";

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(key);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
        }

        //  certificate fee end


        [HttpGet]
        public async Task<ActionResult> Onlinegradecard()
        {
            List<studenttrack> grade = new List<studenttrack>();
            try
            {
                string REGDNO = Session["uid"].ToString();
                string course = "";
                string branch = Convert.ToString(Session["branch_code"]);

                grade = await iStudentRepository.getGradeCardDetails(REGDNO);
                if (grade == null)
                {
                    grade = new List<studenttrack>();
                }
            }
            catch (Exception EX)
            {

            }

            return View("Onlinegradecard", grade);
        }





        //[HttpGet]

        //public async Task<JsonResult> getGradecardMain(string semester, string process, string month, string year)
        //{

        //    //Session["REGDNO"] = "1210116117";
        //    string userid = Session["uid"].ToString();
        //    Session["Gprocess"] = process;
        //    string QCText = "";
        //    string str = "";
        //    string campus = "";
        //    string college = "";
        //    string degree = "";
        //    string course = "";
        //    string branch = "";
        //    string batch = "";

        //    try
        //    {
        //        List<studenttrack> records = await iStudentRepository.getStudentcarddetails(semester, userid);
        //        List<studenttrack> details = await iStudentRepository.getmonth(semester, userid);
        //        if (details.Count() > 0)
        //        {
        //            month = month;
        //            year = year;
        //        }
        //        month = month;
        //        year = year;
        //        if (records.Count() > 0)
        //        {
        //            campus = records.FirstOrDefault().CAMPUS_CODE;
        //            college = records.FirstOrDefault().college_code;
        //            degree = records.FirstOrDefault().DEGREE_CODE;
        //            course = records.FirstOrDefault().COURSE_CODE;
        //            branch = records.FirstOrDefault().BRANCH_CODE;
        //            batch = records.FirstOrDefault().BATCH;


        //        }

        //        int delete = await iStudentRepository.DeleteDetails(campus, college, course, branch, batch);
        //        //int delete = 1;

        //        List<studenttrack> GradeResult = await iStudentRepository.getGradeResult(semester, userid, month, year, process);

        //        studenttrack detailsdata = new studenttrack()
        //        {
        //            regdno = GradeResult.FirstOrDefault().regdno,
        //            name = GradeResult.FirstOrDefault().name,
        //            BRANCH = GradeResult.FirstOrDefault().BRANCH_CODE,
        //            COURSE = GradeResult.FirstOrDefault().COURSE_CODE,
        //            DEGREE_CODE = GradeResult.FirstOrDefault().DEGREE_CODE,
        //            college_code = GradeResult.FirstOrDefault().college_code,
        //            CAMPUS_CODE = GradeResult.FirstOrDefault().CAMPUS_CODE,
        //            course_name = GradeResult.FirstOrDefault().course_name,
        //            Branch_name = GradeResult.FirstOrDefault().Branch_name,
        //            section = GradeResult.FirstOrDefault().section,
        //            COURSE_TITLE = GradeResult.FirstOrDefault().COURSE_TITLE,
        //            PRINT_YEAR = GradeResult.FirstOrDefault().PRINT_YEAR,
        //            EXAM_NAME = GradeResult.FirstOrDefault().EXAM_NAME,
        //            exam_date = GradeResult.FirstOrDefault().exam_date,
        //            Month = month,
        //            Year = GradeResult.FirstOrDefault().Year,
        //            BATCH = GradeResult.FirstOrDefault().BATCH,
        //            process_type = process,
        //            sgpa = GradeResult.FirstOrDefault().sgpa,
        //            cgpa = GradeResult.FirstOrDefault().cgpa,
        //            TOTAL_SEM_CREDITS = GradeResult.FirstOrDefault().TOTAL_SEM_CREDITS,
        //            CUM_SEM_CREDITS = GradeResult.FirstOrDefault().CUM_SEM_CREDITS,
        //            SEMESTER = semester,



        //        };

        //        var insertdetails = await iStudentRepository.InsertGradedetails(detailsdata);

        //        List<studenttrack> detailsmore = await iStudentRepository.getGradeReports(detailsdata);
        //        List<studenttrack> grademarks = new List<studenttrack>();
        //        if (detailsmore != null)
        //        {
        //            if (detailsmore.Count() > 0)
        //            {
        //                for (int i = 0; i < detailsmore.Count(); i++)
        //                {
        //                    studenttrack grades = new studenttrack()
        //                    {
        //                        regdno = GradeResult.FirstOrDefault().regdno,
        //                        SUBJECT_CODE = detailsmore.ElementAt(i).SUBJECT_CODE,
        //                        SUBJECT_NAME = detailsmore.ElementAt(i).SUBJECT_NAME,
        //                        CREDITS1 = detailsmore.ElementAt(i).CREDITS1,
        //                        Grade = detailsmore.ElementAt(i).Grade,
        //                        SEMESTER = detailsmore.ElementAt(i).SEMESTER,
        //                        process_type = detailsmore.ElementAt(i).process_type,
        //                        Month = GradeResult.FirstOrDefault().Month,
        //                        Year = GradeResult.FirstOrDefault().Year,
        //                        CAMPUS_CODE = campus
        //                    };
        //                    grademarks.Add(grades);
        //                }

        //            }
        //            var updatedetails = await iStudentRepository.updateGradedetails(grademarks);
        //        }





        //        if (details.Count() > 0)
        //        {

        //            List<studenttrack> carddetails = await iStudentRepository.getcarddetails(userid);

        //            QCText = " ";
        //            string TY = "QRCODE";
        //            string DT_Completion = "";
        //            // QCText = QCText + "(Demed to be university)  ";
        //            foreach (var dtr in carddetails)
        //            {
        //                QCText = "";
        //                // TY = TY + "$" + dtr[0].ToString().Trim() + "$" + dtr[3].ToString().Trim() + "$";
        //                TY = TY + "$" + dtr.regdno.Trim() + "$" + dtr.SEMESTER.Trim() + "$" + dtr.emonth.Trim() + "$" + dtr.Year.Trim() + "$" + dtr.process_type.Trim() + "$";

        //                //QCText = QCText + "https:///testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT="+TY +"";
        //                //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid2.aspx?QT=" + TY + "";
        //                //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";
        //                //  QCText = QCText + "http://testonlinecertificates.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";

        //                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    //  DT_Completion = Convert.ToDateTime(dtr["DT_Completion"]).ToString("dd-MMM-yyyy");
        //                    if (dtr.sgpa == "0" && dtr.cgpa == "0")
        //                        DT_Completion = "-";
        //                    else
        //                        DT_Completion = Convert.ToDateTime(dtr.DT_Completion).ToString("dd-MMM-yyyy");
        //                    QCText = QCText + "https://onlinecertificates.gitam.edu/View_Result_Grid1.aspx?QT=" + TY + "";

        //                }
        //                else
        //                {
        //                    // QCText = QCText + "https://onlinecertificates.gitam.edu/View_Result_Grid2.aspx?QT=" + TY + "";
      
        //                }

        //                QCText = QCText + "\nRegdno: " + dtr.regdno + ",\nName: " + dtr.name + ",\nCourse:" + dtr.COURSE + ",\nBranch:" + dtr.Branch_name + ",Semester: " + dtr.SEMESTER;//,Semester : "+dtr[83].ToString()
        //                QCText = QCText + "\nGITAM (Deemed to be University)\n";

        //            }
        //            GenerateMyQCCode(QCText);
        //            foreach (var dr in carddetails)
        //            {
        //                //String month = Request.Params["month"].ToString();

        //                str = str + "<div class=\"wrapper\"><div class=\"abc img_hover\" style=\"margin-top:30px;\" >";
        //                str = str + "<div class=\"innerdiv\">";
        //                str = str + "<Label hidden id='qrdata'>" + TY + "</Label>";
        //                str = str + "<table   style=\"border-radius: 10px;margin: 10px auto;width: 650px;\" cellspacing=\"0\" cellpadding=\"0\" border = \"0\" >";
        //                str = str + "<tbody>";

        //                str = str + "<tr>";


        //                str = str + "<table width=\"200\" class='sa-width'>";
        //                str = str + "<tbody>";
        //                str = str + "<tr style=\"text-align:right;\">";

        //                str = str + "</tr>";

        //                str = str + "</tbody>";
        //                str = str + "</table>";


        //                str = str + "</tr>";


        //                str = str + "<tr>";

        //                //str = str + "<br/>";
        //                //str = str + "<br/>";
        //                //str = str + "<br/>";
        //                str = str + "<table width=\"650\">";
        //                str = str + "<tbody>";
        //                str = str + "<tr style=\"text-align:center;\">";
        //                str = str + "<td>  <img src=\"images/academiccert1.jpg\" width =\"650\" class=\"w-700\" /></td>";


        //                str = str + "</tr>";

        //                str = str + "</tbody>";
        //                str = str + "</table>";


        //                str = str + "</tr>";




        //                str = str + "<tr>";
        //                //< p style =\"font-size:12px;\"</p><p style=\"font-size:12px;\"></p>
        //                str = str + "<table width=\"650\" style='margin-bottom:20px'>";
        //                str = str + "<tbody>";
        //                str = str + "<tr style=\"text-align:center;\">";
        //                str = str + "<center style='margin-top:10px;'><b style=\"font-size:24px;\"><span style='color:#026B5C'>GRADE CARD</span></b></center>";
        //                //str = /*str + "<br/>";*/


        //                //str = str + "<center style=\"font-size:18px;\"><b>" + dr[83] + " Semester, " + dr[85] + " " + dr[84] + "</b></center>";

        //                if (Convert.ToString(Session["college_code"]).Equals("GPP") || dr.BRANCH == "Optometry")
        //                {
        //                    str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.Branch_name + " - Degree Examination</b></center>";

        //                }
        //                else
        //                {
        //                    str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.COURSE + " Degree Examination</b></center>";
        //                }
        //                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    if (dr.COURSE.Trim().ToUpper().Equals("MBA") || dr.COURSE.Trim().ToUpper().Equals("M.B.A"))
        //                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
        //                    else
        //                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Year, " + dr.Month + " " + dr.Year + "</b></center>";

        //                }
        //                else
        //                {
        //                    if (Convert.ToString(Session["college_code"]).Equals("GPP"))
        //                    {
        //                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Trimester, " + dr.Month + " " + dr.Year + "</b></center>";
        //                    }
        //                    else if (Convert.ToInt32(dr.SEMESTER) >= 21)
        //                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + ", " + dr.Month + " " + dr.Year + "</b></center>";
        //                    else
        //                        str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
        //                }
        //                str = str + "</tr>";

        //                str = str + "</tbody>";
        //                str = str + "</table>";

        //                str = str + "</tr>";



        //                str = str + "<tr>";

        //                str = str + "<table width=\"650\" class='mt-1 sa-width details' style='margin-bottom:20px'>";
        //                str = str + "<tbody>";
        //                //str = str + "<tr>"; str = str + "<td  width=\"650\">"; str = str + "<table width=\"550\" class='mt-1 sa-width details' style='margin-bottom:20px'>"; str = str + "<tbody>";//msr
        //                str = str + "<tr>";
        //                str = str + "<td style=\"width:50px;\"> <span>Regd.No</span></td>";
        //                str = str + "<td>:</td>";
        //                str = str + "<td style='text-align:left'> &nbsp;" + dr.regdno + "</td>";
        //                str = str + "<tr>";
        //                str = str + "<td style=\"width:50px;\"> <span>Name </span></td>";
        //                str = str + "<td>:</td>";
        //                str = str + "<td style='text-align:left'> &nbsp;" + dr.name + "</td>";
        //                str = str + "<td> &nbsp;</td>";
        //                str = str + "<td> &nbsp;</td>";
        //                str = str + "</tr>";
        //                if (!Convert.ToString(Session["college_code"]).Equals("GPP"))
        //                {
        //                    str = str + "<tr>";
        //                    str = str + "<td style=\"width:50px;\"> <span>Branch</span></td>";
        //                    str = str + "<td> :</td>";
        //                    str = str + "<td style='text-align:left'> &nbsp;" + dr.Branch_name + "</td>";
        //                    str = str + "<td> &nbsp;</td>";
        //                    str = str + "<td> &nbsp;</td>";
        //                    str = str + "</tr>";
        //                }

        //                if (Convert.ToString(Session["college_code"]).Equals("GPP"))
        //                {
        //                    string gppname = "Kautilya School of Public Policy";
        //                    str = str + "<tr>";
        //                    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
        //                    str = str + "<td> :</td>";
        //                    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
        //                    str = str + "<td> &nbsp;</td>";
        //                    str = str + "<td> &nbsp;</td>";
        //                    str = str + "</tr>";
        //                }
        //                //else if(Session["phdcoursecode"].ToString().Equals("PHD"))
        //                //{
        //                //    string gppname = "Inistitute of Technology";
        //                //    str = str + "<tr>";
        //                //    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
        //                //    str = str + "<td> :</td>";
        //                //    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
        //                //    str = str + "<td> &nbsp;</td>";
        //                //    str = str + "<td> &nbsp;</td>";
        //                //    str = str + "</tr>";
        //                //}


        //                str = str + "</tbody>";
        //                str = str + "</table>";



        //                str = str + "</tr>";

        //                str = str + "<tr>";


        //                //str = str + "<div class=\"b12121\">";
        //                //height =\"350\"
        //                str = str + "<table width=\"650\" class=\"tab mt-2 mb-2 sa-width\">";
        //                str = str + "<tbody>";
        //                str = str + "<tr class=\"\" style='border:2px solid #026B5C'>";
        //                str = str + "<td style=\"text-align:left;padding:4px;width:105px;\"> <span style='color:#026B5C;'><b>Course Code</b></span> </td>";
        //                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Name of the Course</b></span> </td>";

        //                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Credits</b> </span></td>";
        //                str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Grade</b> </span></td>";

        //                str = str + "</tr>";






        //                if (dr.SUB1_CODE != null)
        //                {
        //                    if (dr.SUB1_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB1_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB1_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB2_CODE != null)
        //                {
        //                    if (dr.SUB2_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB2_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB2_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB3_CODE != null)
        //                {
        //                    if (dr.SUB3_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB3_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB3_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB4_CODE != null)
        //                {
        //                    if (dr.SUB4_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB4_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB4_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB5_CODE != null)
        //                {
        //                    if (dr.SUB5_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB5_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB5_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB6_CODE != null)
        //                {
        //                    if (dr.SUB6_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB6_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB6_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB7_CODE != null)
        //                {
        //                    if (dr.SUB7_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB7_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB7_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB8_CODE != null)
        //                {
        //                    if (dr.SUB8_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB8_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB8_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB9_CODE != null)
        //                {
        //                    if (dr.SUB9_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB9_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB9_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB10_CODE != null)
        //                {
        //                    if (dr.SUB10_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB10_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB10_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB11_CODE != null)
        //                {
        //                    if (dr.SUB11_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB11_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB11_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB12_CODE != null)
        //                {
        //                    if (dr.SUB12_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB12_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB12_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB13_CODE != null)
        //                {
        //                    if (dr.SUB13_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB13_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB13_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB14_CODE != null)
        //                {
        //                    if (dr.SUB14_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB14_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB14_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }
        //                if (dr.SUB15_CODE != null)
        //                {
        //                    if (dr.SUB15_CODE != "")
        //                    {
        //                        str = str + "<tr>";
        //                        str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB15_CODE.ToString() + "</p></td>";
        //                        str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB15_NAME.ToString() + " </p></td>";

        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_CREDITS.ToString() + " </p></td>";
        //                        str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_GRADE.ToString() + " </p></td>";

        //                        str = str + "</tr>";


        //                    }
        //                }

        //                str = str + "</tbody>";
        //                str = str + "</table>";

        //                str = str + "</tr>";


        //                str = str + "<tr>";


        //                str = str + "<div class='sgpa'><table><tbody><tr>";

        //                if (dr.sgpa == "0" && dr.cgpa == "0")
        //                {
        //                    //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[12] + " <br/></td></tr><tr>";
        //                    //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[13] + " <br/></td>";
        //                    str = str + "<td> <p style =\"font-size:15px;\">SGPA </p></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
        //                    str = str + "<td> <p style =\"font-size:15px;\">CGPA </p></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
        //                    str = str + "</tr></tbody></table></div>";
        //                }
        //                else
        //                {
        //                    //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[86] + " <br/></td></tr><tr>";
        //                    //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[87] + " <br/></td>";
        //                    str = str + "<td> <span style =\"font-size:15px;\">SGPA </span></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
        //                    str = str + "<td> <span style =\"font-size:15px;\">CGPA </span></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
        //                    str = str + "</tr></tbody></table></div>";
        //                }

        //                str = str + "</tr>";
        //                str = str + "<div class=\"b6\">";
        //                //str = str + "<span> <b>Printed On:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "</td>";
        //                //str = str + "<br/></span>";

        //                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    str = str + "<span> <b>Printed On&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
        //                    str = str + "<br/></span>";
        //                }

        //                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    str = str + "<span> <b style =\"font-size:15px;\">Printed On&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
        //                    str = str + "<br/><br/></span>";
        //                    str = str + "<span style =\"font-size:12px;\"><b>Mode of delivery&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;ODL";
        //                    str = str + "<br/></span>";
        //                    str = str + "<span style =\"font-size:12px;\"><b>Date of Admission&nbsp; :</b>&nbsp;" + Convert.ToString(Session["AdmissionDate"]) + "";
        //                    str = str + "<br/></span>";
        //                    str = str + "<span style =\"font-size:12px;\"><b>Date of completion :</b>&nbsp;" + DT_Completion + "";
        //                    str = str + "<br/></span>";
        //                    str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Learner Support Centres :&nbsp;---</b>" + "";
        //                    str = str + "<br/></span>";
        //                    str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Examination Centres&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;GITAM (deemed to beUniversity), VISAKHAPATNAM.";
        //                    str = str + "<br/></span>";
        //                }



        //                str = str + "</div>";
        //                str = str + "<div class=\"b5\">";
        //                str = str + "<br/>";
        //                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    // if(Session["Gprocess"].ToString().Trim().Equals("SRV"))
        //                    // {
        //                    //     //str = str + "<div style=\"width:600px;\"><b>''Certificate issued after the publication of supplimentary revaluation results''</b></div>";
        //                    // }
        //                    //else
        //                    //if (Session["Gprocess"].ToString().Trim().Contains("RV"))
        //                    //{
        //                    //    str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
        //                    //}
        //                    //else if (Session["Gprocess"].ToString().Trim().Contains("BG"))
        //                    //{
        //                    //str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
        //                    //}
        //                    if (Convert.ToString(Session["Gprocess"]).Trim().Contains("RV"))
        //                    {
        //                        str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
        //                    }
        //                    else if (Convert.ToString(Session["Gprocess"]).Trim().Contains("BG"))
        //                    {
        //                        str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
        //                    }
        //                }

        //                if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    str = str + "<table style =\"width:650px;margin-left: 0px;\" class=\"w-700\"><tbody><tr><td style =\"width:450px;\" class=\"w-550\">";//msr
        //                    str = str + "<span style =\"width:100px;\"> <b>Note:</b><br/><span style =\"font-size:12px;\"><i>1.This is a digitally generated certificate. The format of this certificate may differ from the";
        //                    str = str + " document issued by the University<br/></i></span></span>";

        //                    str = str + "<span style =\"width:100px;font-size:12px;\"><i>2.For any clarification, please contact <a href=\"#\">controllerofexaminations@gitam.edu</a><i></span>";
        //                }

        //                // if (Session["College"].ToString().Equals("CDL"))
        //                //   str = str + "<span style =\"width:100px;font-size:12px;\"><i><br/>3.Mode of Delivery ODL<i></span>";
        //                if (Convert.ToString(Session["college_code"]).Equals("CDL"))
        //                {
        //                    str = str + "</td>";//msr
        //                    str = str + "<td style =\"width:100px;\">";
        //                    str = str + "<div style=\"width:100px;height:200px\"><img src=\"images/DynamicQR.jpg\" width =\"150\" style=\"margin-left:550px\" /></div>";
        //                    str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";

        //                    str = str + "<table \" style = \"width:200pxpx; height:31px;display:block;margin-left:470px;margin-top:-55px;font-family: 'Open Sans', sans - serif\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
        //                    str = str + "<tbody>";
        //                    str = str + "<tr>";
        //                    str = str + "<td><img src='images/Sumanth-Kumar.png' alt='Snow' style=';display:block;margin-left:100px;height:30px;margin-top:-11px;'></td>";
        //                    str = str + "</tr>";
        //                    str = str + "<tr>";
        //                    str = str + "<td colspan ='3' align=\"right\" style = \"font-size: 15px;\"><b> <span class=\"tempfields\"  style = \"width:200px;margin-top:-2px;\">controller of examinations</span> </b></td>";
        //                    str = str + "</tr>";
        //                    str = str + "</tbody>";
        //                    str = str + "</table>";



        //                }
        //                else
        //                {
        //                    str = str + "</td>";//msr
        //                    str = str + "<td style =\"width:100px;\" class=\"w-150\">";
        //                    str = str + "<div style=\"width:100px;\" class=\"w-150\"><img src=\"images/DynamicQR.jpg\" width =\"140\"  class=\"w-150\" /></div>";
        //                    str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";



        //                }


        //                str = str + "</tr>";



        //                str = str + "</tbody>";
        //                str = str + "</table>";
        //                str = str + "</div>";
        //                str = str + "</div></div><br>";
        //                str = str + "<div class='break'><br></div>";



        //            }

        //        }








        //        return Json(str, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        [HttpGet]

        public async Task<JsonResult> getGradecardMain(string semester, string process, string month, string year)
        {

            //Session["REGDNO"] = "1210116117";
            string userid = Session["uid"].ToString();
            Session["Gprocess"] = process;
            string QCText = "";
            string str = "";
            string campus = "";
            string college = "";
            string degree = "";
            string course = "";
            string branch = "";
            string batch = "";

            try
            {
                List<studenttrack> records = await iStudentRepository.getStudentcarddetails(semester, userid);
                List<studenttrack> details = await iStudentRepository.getmonth(semester, userid);
                if (details.Count() > 0)
                {
                    month = month;
                    year = year;
                }
                month = month;
                year = year;
                if (records.Count() > 0)
                {
                    campus = records.FirstOrDefault().CAMPUS_CODE;
                    college = records.FirstOrDefault().college_code;
                    degree = records.FirstOrDefault().DEGREE_CODE;
                    course = records.FirstOrDefault().COURSE_CODE;
                    branch = records.FirstOrDefault().BRANCH_CODE;
                    batch = records.FirstOrDefault().BATCH;


                }

                int delete = await iStudentRepository.DeleteDetails(campus, college, course, branch, batch);
                //int delete = 1;

                List<studenttrack> GradeResult = await iStudentRepository.getGradeResult(semester, userid, month, year, process);

                studenttrack detailsdata = new studenttrack()
                {
                    regdno = GradeResult.FirstOrDefault().regdno,
                    name = GradeResult.FirstOrDefault().name,
                    BRANCH = GradeResult.FirstOrDefault().BRANCH_CODE,
                    COURSE = GradeResult.FirstOrDefault().COURSE_CODE,
                    DEGREE_CODE = GradeResult.FirstOrDefault().DEGREE_CODE,
                    college_code = GradeResult.FirstOrDefault().college_code,
                    CAMPUS_CODE = GradeResult.FirstOrDefault().CAMPUS_CODE,
                    course_name = GradeResult.FirstOrDefault().course_name,
                    Branch_name = GradeResult.FirstOrDefault().Branch_name,
                    section = GradeResult.FirstOrDefault().section,
                    COURSE_TITLE = GradeResult.FirstOrDefault().COURSE_TITLE,
                    PRINT_YEAR = GradeResult.FirstOrDefault().PRINT_YEAR,
                    EXAM_NAME = GradeResult.FirstOrDefault().EXAM_NAME,
                    exam_date = GradeResult.FirstOrDefault().exam_date,
                    Month = month,
                    Year = GradeResult.FirstOrDefault().Year,
                    BATCH = GradeResult.FirstOrDefault().BATCH,
                    process_type = process,
                    sgpa = GradeResult.FirstOrDefault().sgpa,
                    cgpa = GradeResult.FirstOrDefault().cgpa,
                    TOTAL_SEM_CREDITS = GradeResult.FirstOrDefault().TOTAL_SEM_CREDITS,
                    CUM_SEM_CREDITS = GradeResult.FirstOrDefault().CUM_SEM_CREDITS,
                    SEMESTER = semester,



                };

                var insertdetails = await iStudentRepository.InsertGradedetails(detailsdata);

                List<studenttrack> detailsmore = await iStudentRepository.getGradeReports(detailsdata);
                List<studenttrack> grademarks = new List<studenttrack>();
                if (detailsmore != null)
                {
                    if (detailsmore.Count() > 0)
                    {
                        for (int i = 0; i < detailsmore.Count(); i++)
                        {
                            studenttrack grades = new studenttrack()
                            {
                                regdno = GradeResult.FirstOrDefault().regdno,
                                SUBJECT_CODE = detailsmore.ElementAt(i).SUBJECT_CODE,
                                SUBJECT_NAME = detailsmore.ElementAt(i).SUBJECT_NAME,
                                CREDITS1 = detailsmore.ElementAt(i).CREDITS1,
                                Grade = detailsmore.ElementAt(i).Grade,
                                SEMESTER = detailsmore.ElementAt(i).SEMESTER,
                                process_type = detailsmore.ElementAt(i).process_type,
                                Month = GradeResult.FirstOrDefault().Month,
                                Year = GradeResult.FirstOrDefault().Year,
                                CAMPUS_CODE = campus
                            };
                            grademarks.Add(grades);
                        }

                    }
                    var updatedetails = await iStudentRepository.updateGradedetails(grademarks);
                }





                if (details.Count() > 0)
                {

                    List<studenttrack> carddetails = await iStudentRepository.getcarddetails(userid);

                    QCText = " ";
                    string TY = "QRCODE";
                    string DT_Completion = "";
                    // QCText = QCText + "(Demed to be university)  ";
                    foreach (var dtr in carddetails)
                    {
                        QCText = "";
                        // TY = TY + "$" + dtr[0].ToString().Trim() + "$" + dtr[3].ToString().Trim() + "$";
                        TY = TY + "$" + dtr.regdno.Trim() + "$" + dtr.SEMESTER.Trim() + "$" + dtr.emonth.Trim() + "$" + dtr.Year.Trim() + "$" + dtr.process_type.Trim() + "$";

                        //QCText = QCText + "https:///testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT="+TY +"";
                        //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid2.aspx?QT=" + TY + "";
                        //QCText = QCText + "https://testmanagementevaluation.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";
                        //  QCText = QCText + "http://testonlinecertificates.gitam.edu/View_Result_Grid.aspx?QT=" + TY + "";

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            //  DT_Completion = Convert.ToDateTime(dtr["DT_Completion"]).ToString("dd-MMM-yyyy");
                            if (dtr.sgpa == "0" && dtr.cgpa == "0")
                                DT_Completion = "-";
                            else
                                DT_Completion = Convert.ToDateTime(dtr.DT_Completion).ToString("dd-MMM-yyyy");
                            QCText = QCText + "https://onlinecertificates.gitam.edu/View_Result_Grid1.aspx?QT=" + TY + "";

                        }
                        else
                        {
                          
                            QCText = QCText + "https://studentlms.gitam.edu/Home/ViewResultGrid?QT=" + TY + "";
                        }

                        QCText = QCText + "\nRegdno: " + dtr.regdno + ",\nName: " + dtr.name + ",\nCourse:" + dtr.COURSE + ",\nBranch:" + dtr.Branch_name + ",Semester: " + dtr.SEMESTER;//,Semester : "+dtr[83].ToString()
                        QCText = QCText + "\nGITAM (Deemed to be University)\n";

                    }
                    GenerateMyQCCode(QCText);
                    foreach (var dr in carddetails)
                    {
                        //String month = Request.Params["month"].ToString();

                        str = str + "<div class=\"wrapper\"><div class=\"abc img_hover\" style=\"margin-top:30px;\" >";
                        str = str + "<div class=\"innerdiv\">";
                        str = str + "<Label hidden id='qrdata'>" + TY + "</Label>";
                        str = str + "<table   style=\"border-radius: 10px;margin: 10px auto;width: 650px;\" cellspacing=\"0\" cellpadding=\"0\" border = \"0\" >";
                        str = str + "<tbody>";

                        str = str + "<tr>";


                        str = str + "<table width=\"200\" class='sa-width'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:right;\">";

                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";


                        str = str + "<tr>";

                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        //str = str + "<br/>";
                        str = str + "<table>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<td>  <img src=\"images/academiccert1.jpg\" class='logo' /></td>";


                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";


                        str = str + "</tr>";




                        str = str + "<tr>";
                        //< p style =\"font-size:12px;\"</p><p style=\"font-size:12px;\"></p>
                        str = str + "<table width=\"650\" style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        str = str + "<tr style=\"text-align:center;\">";
                        str = str + "<center style='margin-top:10px;'><b style=\"font-size:24px;\"><span style='color:#026B5C'>GRADE CARD</span></b></center>";
                        //str = /*str + "<br/>";*/


                        //str = str + "<center style=\"font-size:18px;\"><b>" + dr[83] + " Semester, " + dr[85] + " " + dr[84] + "</b></center>";

                        if (Convert.ToString(Session["college_code"]).Equals("GPP") || dr.BRANCH == "Optometry")
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.Branch_name + " - Degree Examination</b></center>";

                        }
                        else
                        {
                            str = str + "<center style='margin-top:10px;margin-left:5px;'><b style=\"font-size:19px;margin-top:10px;\">" + dr.COURSE + " Degree Examination</b></center>";
                        }
                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            if (dr.COURSE.Trim().ToUpper().Equals("MBA") || dr.COURSE.Trim().ToUpper().Equals("M.B.A"))
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Year, " + dr.Month + " " + dr.Year + "</b></center>";

                        }
                        else
                        {
                            if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                            {
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Trimester, " + dr.Month + " " + dr.Year + "</b></center>";
                            }
                            else if (Convert.ToInt32(dr.SEMESTER) >= 21)
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + ", " + dr.Month + " " + dr.Year + "</b></center>";
                            else
                                str = str + "<center style=\"font-size:18px;\"><b>" + dr.roman_semester + " Semester, " + dr.Month + " " + dr.Year + "</b></center>";
                        }
                        str = str + "</tr>";

                        str = str + "</tbody>";
                        str = str + "</table>";

                        str = str + "</tr>";



                        str = str + "<tr>";

                        str = str + "<table class='mt-1 sa-width details' style='margin-bottom:20px'>";
                        str = str + "<tbody>";
                        //str = str + "<tr>"; str = str + "<td  width=\"650\">"; str = str + "<table width=\"550\" class='mt-1 sa-width details' style='margin-bottom:20px'>"; str = str + "<tbody>";//msr
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Regd.No</span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.regdno + "</td>";
                        str = str + "<tr>";
                        str = str + "<td style=\"width:50px;\"> <span>Name </span></td>";
                        str = str + "<td>:</td>";
                        str = str + "<td style='text-align:left'> &nbsp;" + dr.name + "</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "<td> &nbsp;</td>";
                        str = str + "</tr>";
                        if (!Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>Branch</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + dr.Branch_name + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }

                        if (Convert.ToString(Session["college_code"]).Equals("GPP"))
                        {
                            string gppname = "Kautilya School of Public Policy";
                            str = str + "<tr>";
                            str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                            str = str + "<td> :</td>";
                            str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "<td> &nbsp;</td>";
                            str = str + "</tr>";
                        }
                        //else if(Session["phdcoursecode"].ToString().Equals("PHD"))
                        //{
                        //    string gppname = "Inistitute of Technology";
                        //    str = str + "<tr>";
                        //    str = str + "<td style=\"width:50px;\"> <span>School</span></td>";
                        //    str = str + "<td> :</td>";
                        //    str = str + "<td style='text-align:left'> &nbsp;" + gppname + "</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "<td> &nbsp;</td>";
                        //    str = str + "</tr>";
                        //}


                        str = str + "</tbody>";
                        str = str + "</table>";



                        str = str + "</tr>";

                        str = str + "<tr>";


                        //str = str + "<div class=\"b12121\">";
                        //height =\"350\"
                        str = str + "<div class='table-responsive'><table class=\"tab mt-2 mb-2 sa-width\">";
                        str = str + "<tbody>";
                        str = str + "<tr class=\"\" style='border:2px solid #026B5C'>";
                        str = str + "<td style=\"text-align:left;padding:4px;width:105px;\"> <span style='color:#026B5C;'><b>Course Code</b></span> </td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Name of the Course</b></span> </td>";

                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Credits</b> </span></td>";
                        str = str + "<td style=\"text-align:center;\"> <span style='color:#026B5C'><b>Grade</b> </span></td>";

                        str = str + "</tr>";






                        if (dr.SUB1_CODE != null)
                        {
                            if (dr.SUB1_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB1_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB1_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB1_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB2_CODE != null)
                        {
                            if (dr.SUB2_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB2_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB2_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB2_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB3_CODE != null)
                        {
                            if (dr.SUB3_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB3_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB3_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB3_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB4_CODE != null)
                        {
                            if (dr.SUB4_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB4_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB4_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB4_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB5_CODE != null)
                        {
                            if (dr.SUB5_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB5_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB5_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB5_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB6_CODE != null)
                        {
                            if (dr.SUB6_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB6_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB6_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB6_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB7_CODE != null)
                        {
                            if (dr.SUB7_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB7_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB7_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB7_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB8_CODE != null)
                        {
                            if (dr.SUB8_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB8_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB8_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB8_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB9_CODE != null)
                        {
                            if (dr.SUB9_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB9_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB9_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB9_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB10_CODE != null)
                        {
                            if (dr.SUB10_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB10_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB10_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB10_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB11_CODE != null)
                        {
                            if (dr.SUB11_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB11_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB11_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB11_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB12_CODE != null)
                        {
                            if (dr.SUB12_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB12_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB12_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB12_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB13_CODE != null)
                        {
                            if (dr.SUB13_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB13_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB13_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB13_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB14_CODE != null)
                        {
                            if (dr.SUB14_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB14_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB14_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB14_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }
                        if (dr.SUB15_CODE != null)
                        {
                            if (dr.SUB15_CODE != "")
                            {
                                str = str + "<tr>";
                                str = str + "<td><p style=\"margin-left:10px;text-align:left;\">" + dr.SUB15_CODE.ToString() + "</p></td>";
                                str = str + "<td><p style=\"margin-left:10px; text-align:left;\">" + dr.SUB15_NAME.ToString() + " </p></td>";

                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_CREDITS.ToString() + " </p></td>";
                                str = str + "<td style=\"text-align:center;\"><p>" + dr.SUB15_GRADE.ToString() + " </p></td>";

                                str = str + "</tr>";


                            }
                        }

                        str = str + "</tbody>";
                        str = str + "</table></div>";

                        str = str + "</tr>";


                        str = str + "<tr>";


                        str = str + "<div class='sgpa'><table><tbody><tr>";

                        //if (Session["college_code"].ToString() == "GIM" || Session["college_code"].ToString() == "SMS" || Session["college_code"].ToString() == "HBS" || Session["college_code"].ToString() == "GIS" || Session["college_code"].ToString() == "GSS" || Session["college_code"].ToString() == "GSHS" || Session["college_code"].ToString() == "GSGS")
                        //{

                        //    if (Session["branch_code"].ToString() == "BLENDED" || Session["branch_code"].ToString() == "CHEMISTRY-PFIZER" || Session["branch_code"].ToString() == "CHEMISTRY1" || Session["branch_code"].ToString() == "Optometry" || Session["branch_code"].ToString() == "CSCS"|| Session["branch_code"].ToString() == "BCA")
                        //    {
                        //    }
                        //    else
                        //    {
                        //        string s = Session["Curr_sem"].ToString();
                        //        string b = Session["batch"].ToString();

                        //        if (s == "6" && b.Contains("2021-"))
                        //        {
                        //            dr.cgpa = "<span style='color:red!important;'>*</span>";

                        //        }

                        //    }


                        //}

                        if (dr.sgpa == "0" && dr.cgpa == "0")
                        {
                            //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[12] + " <br/></td></tr><tr>";
                            //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[13] + " <br/></td>";
                            str = str + "<td> <p style =\"font-size:15px;\">SGPA </p></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <p style =\"font-size:15px;\">CGPA </p></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }
                        else
                        {
                            //str = str + "<td> <b style =\"font-size:15px;\">SGPA </b></td><td>:</td><td>" + dr[86] + " <br/></td></tr><tr>";
                            //str = str + "<td> <b style =\"font-size:15px;\">CGPA </b></td><td>:</td><td>" + dr[87] + " <br/></td>";
                            str = str + "<td> <span style =\"font-size:15px;\">SGPA </span></td><td>:</td><td>" + dr.sgpa + " <br/></td></tr><tr>";
                            str = str + "<td> <span style =\"font-size:15px;\">CGPA </span></td><td>:</td><td>" + dr.cgpa + " <br/></td>";
                            str = str + "</tr></tbody></table></div>";
                        }

                        str = str + "</tr>";
                        str = str + "<div class=\"b6\">";
                        //str = str + "<span> <b>Printed On:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "</td>";
                        //str = str + "<br/></span>";

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b>Printed On&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/></span>";
                        }

                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<span> <b style =\"font-size:15px;\">Printed On&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:</b>" + System.DateTime.Now.ToString("dd-MMM-yyyy") + "";
                            str = str + "<br/><br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Mode of delivery&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;ODL";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of Admission&nbsp; :</b>&nbsp;" + Convert.ToString(Session["AdmissionDate"]) + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Date of completion :</b>&nbsp;" + DT_Completion + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Learner Support Centres :&nbsp;---</b>" + "";
                            str = str + "<br/></span>";
                            str = str + "<span style =\"font-size:12px;\"><b>Name and address of all Examination Centres&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :</b>" + "&nbsp;GITAM (deemed to beUniversity), VISAKHAPATNAM.";
                            str = str + "<br/></span>";
                        }



                        str = str + "</div>";
                        str = str + "<div class=\"b5\">";
                        str = str + "<br/>";
                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            // if(Session["Gprocess"].ToString().Trim().Equals("SRV"))
                            // {
                            //     //str = str + "<div style=\"width:600px;\"><b>''Certificate issued after the publication of supplimentary revaluation results''</b></div>";
                            // }
                            //else
                            //if (Session["Gprocess"].ToString().Trim().Contains("RV"))
                            //{
                            //    str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            //}
                            //else if (Session["Gprocess"].ToString().Trim().Contains("BG"))
                            //{
                            //str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            //}
                            if (Convert.ToString(Session["Gprocess"]).Trim().Contains("RV"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of revaluation results.</b></div>";
                            }
                            else if (Convert.ToString(Session["Gprocess"]).Trim().Contains("BG"))
                            {
                                str = str + "<div style=\"width:600px;font-size:13px;\"><b>Certificate issued after the publication of Betterment of Grade results.</b></div>";
                            }
                        }

                        if (!Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "<div class='note'>";//msr
                            str = str + "<span> <b>Note:</b><br/><span><i>1.This is a digitally generated certificate. The format of this certificate may differ from the";
                            str = str + " document issued by the University<br/></i></span></span>";

                            str = str + "<span><i>2.For any clarification, please contact <a href=\"#\">controllerofexaminations@gitam.edu</a><i></span>";
                        }

                        // if (Session["College"].ToString().Equals("CDL"))
                        //   str = str + "<span style =\"width:100px;font-size:12px;\"><i><br/>3.Mode of Delivery ODL<i></span>";
                        if (Convert.ToString(Session["college_code"]).Equals("CDL"))
                        {
                            str = str + "</td>";//msr
                            str = str + "<td style =\"width:100px;\">";
                            str = str + "<div style=\"width:100px;height:200px\"><img src=\"images/DynamicQR.jpg\" width =\"150\" style=\"margin-left:550px\" /></div>";
                            str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";

                            str = str + "<table \" style = \"width:200pxpx; height:31px;display:block;margin-left:470px;margin-top:-55px;font-family: 'Open Sans', sans - serif\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">";
                            str = str + "<tbody>";
                            str = str + "<tr>";
                            str = str + "<td><img src='images/Sumanth-Kumar.png' alt='Snow' style=';display:block;margin-left:100px;height:30px;margin-top:-11px;'></td>";
                            str = str + "</tr>";
                            str = str + "<tr>";
                            str = str + "<td colspan ='3' align=\"right\" style = \"font-size: 15px;\"><b> <span class=\"tempfields\"  style = \"width:200px;margin-top:-2px;\">controller of examinations</span> </b></td>";
                            str = str + "</tr>";
                            str = str + "</tbody>";
                            str = str + "</table>";



                        }
                        else
                        {
                            //str = str + "</td>";//msr
                            //str = str + "<td style =\"width:100px;\" class=\"w-150\">";
                            //str = str + "<div style=\"width:100px;\" class=\"w-150\"><img src=\"images/DynamicQR.jpg\" width =\"140\"  class=\"w-150\" /></div>";
                            //str = str + "</td></tr>"; str = str + "</tbody>"; str = str + "</table>";

                            str = str + "<div class='sign'><div class='qr'><img src=\"images/DynamicQR.jpg\" width =\"140\" class=\"w-150\" /></div></div>";
                            //<div class='text-right'><img src='images/Sumanth-Kumar.png' style='height:30px;'><div><b>controller of examinations</b></div></div>
                        }


                        str = str + "</tr>";



                        str = str + "</tbody>";
                        str = str + "</table>";
                        str = str + "</div>";
                        if (Session["college_code"].ToString() == "GIM" || Session["college_code"].ToString() == "SMS" || Session["college_code"].ToString() == "HBS" || Session["college_code"].ToString() == "GIS" || Session["college_code"].ToString() == "GSS" || Session["college_code"].ToString() == "GSHS" || Session["college_code"].ToString() == "GSGS")
                        {

                            //if (Session["branch_code"].ToString() == "BLENDED" || Session["branch_code"].ToString() == "CHEMISTRY-PFIZER" || Session["branch_code"].ToString() == "CHEMISTRY1" || Session["branch_code"].ToString() == "Optometry" || Session["branch_code"].ToString() == "CSCS" || Session["branch_code"].ToString() == "BCA")
                            //{ 
                            //}
                            //else
                            //{
                            //    string s = Session["Curr_sem"].ToString();
                            //    string b = Session["batch"].ToString();

                            //    if(s=="6" && b.Contains("2021-"))
                            //    {
                            //        str = str + "<p style='color:red!important'>*CGPA will be updated based on Academic Regulations of the programme (After satisfying No. of credits in OE/UC/FC/...)</p>";

                            //    }
                            //}


                        }
                        str = str + "</div></div><br>";
                        str = str + "<div class='break'><br></div>";



                    }

                }








                return Json(str, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ActionResult> getphdbiometric()
        {

            List<int> Years = new List<int>();
            DateTime startYear = DateTime.Now;
            while (startYear.Year - 1 <= DateTime.Now.Year)
            {
                Years.Add(startYear.Year - 1);
                startYear = startYear.AddYears(1);
            }
            ViewBag.Years = Years;
            return PartialView("phdbiometric_report");
        }


        public async Task<ActionResult> getcdlstudymaterial()
        {

            return PartialView("Cdlmaterail");
        }
        public async Task<JsonResult> getphdbiometricreport(string month, string year)
        {

            List<Phdbiometric> grade = new List<Phdbiometric>();
            string REGDNO = Session["uid"].ToString();

            grade = await iStudentRepository.getphdbiometricreport(REGDNO, month, year);

            //return PartialView("phdbiometric_report");
            return Json(grade, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Mycredits()
        {
            Degreeplansummary plan = new Degreeplansummary();
            Degreeplan chart = new Degreeplan();
            Attendancesummary summary = new Attendancesummary();
            IEnumerable<Degreeplan> dp = null;
            try
            {
                

                string[] batch1 = new string[1000];
                string batch = Session["batch"].ToString();
                batch1 = batch.Split('-');
                string batch2 = batch1[0];
                string user_id = Session["uid"].ToString();

                string college_code = Session["college_code"].ToString();
                string branch_code = Session["branch_code"].ToString();
                string campus_code = Session["campus_code"].ToString();
                string sem = Session["Curr_sem"].ToString();
                try
                {
                    summary.note = new Attendance();
                    summary.notes = await iStudentRepository.Getcoursestructuremenu(college_code, campus_code, branch_code, batch2, sem, user_id);
               
                }
                catch (Exception ex)
                {
                    throw ex;
                }
               

                dp = await iStudentRepository.getremainingcredits(Convert.ToString(Session["uid"]));
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            summary.degreeplan = dp;
            return View("Mycredits", summary);
        }
        //public async Task<ActionResult> DegreePlan()
        //{
        //    Degreeplansummary plan = new Degreeplansummary();
        //    Degreeplan chart = new Degreeplan();
        //    try
        //    {
        //        chart.REGDNO = Convert.ToString(Session["uid"]);
        //        chart.COURSE = Convert.ToString(Session["course_code"]);
        //        chart.BATCH = Convert.ToString(Session["BATCH"]);
        //        string sem = Convert.ToString(Session["curr_sem"]);
        //        chart.DEGREE = Convert.ToString(Session["degree_code"]);
        //        chart.CAMPUS = Session["campus_code"].ToString();
        //        chart.COLLEGE = Session["college_code"].ToString();
        //        chart.NAME = Session["studname"].ToString();
        //        chart.BRANCH = Session["branch_code"].ToString();
        //        chart.DEPARTMENT = Session["dept_code"].ToString();
        //        string[] batch1 = new string[1000];
        //        batch1 = sem.Split('-');
        //        string batch2 = batch1[0];
        //        Session["SEMESTER"] = (Convert.ToInt32(batch2) - 1);
        //        chart.SEMESTER = Convert.ToString(Session["SEMESTER"]);
        //        plan.note = await iStudentRepository.getcreditperformance(chart);
        //        chart = plan.note;
        //        plan.required = await iStudentRepository.getremainingcredits(chart);
        //        plan.selected = await iStudentRepository.getselectingcredits(chart);
        //        plan.deg = await iStudentRepository.getsubjects(chart);
        //        plan.open = await iStudentRepository.getsubjectsOE(chart);
        //        plan.faculty = await iStudentRepository.getsubjectsFC(chart);
        //        plan.program = await iStudentRepository.getsubjectsPC(chart);
        //        plan.programelective = await iStudentRepository.getsubjectsPE(chart);
        //        plan.MICS = await iStudentRepository.getsubjectsMIC(chart);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return View("DegreePlan", plan);
        //}
        public async Task<ActionResult> getstudentmedicalappointment(string id)
        {
            Degreeplansummary plan = new Degreeplansummary();
            Degreeplan chart = new Degreeplan();
            try
            {
                chart.REGDNO = Convert.ToString(Session["uid"]);
                chart.COURSE = Convert.ToString(Session["course_code"]);
                chart.BATCH = Convert.ToString(Session["BATCH"]);
                //plan.required = await iStudentRepository.getmedicalappointment(chart);
                //plan.medical = await iStudentRepository.getmedicallists(chart);

                plan.required = await iStudentRepository.getmedicalappointment(chart);
                plan.medical = await iStudentRepository.getmedicallists(chart);


            }

            catch (Exception ex)
            {
                throw ex;
            }

            return View("MedicalAppointment", plan);
        }
        public async Task<JsonResult> getmedicalappointment(string id)
        {
            Degreeplansummary plan = new Degreeplansummary();
            Degreeplan chart = new Degreeplan();
            try
            {



                chart.REGDNO = Convert.ToString(Session["uid"]);
                chart.COURSE = Convert.ToString(Session["course_code"]);
                chart.BATCH = Convert.ToString(Session["BATCH"]);
                plan.required = await iStudentRepository.getmedicalappointment(chart);

                plan.required.FirstOrDefault().flag3 = "succ";

            }

            catch (Exception ex)
            {
                throw ex;
            }

            return Json(plan, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> bookappointment(string id, string name, string venue, string date)
        {
            Degreeplansummary plan = new Degreeplansummary();
            Degreeplan chart = new Degreeplan();
            try
            {
                chart.REGDNO = Convert.ToString(Session["uid"]);
                chart.AppointmentID = id;
                chart.ENAME = name;
                chart.EVENUE = venue;
                chart.EDATE = date;
                //chart.datee = Convert.ToDateTime(chart.EDATE).ToString("dd-MMM-yyyy");
                var result = await iStudentRepository.checkappointment(chart.AppointmentID);
                if (result.Count() >= 1)
                {
                    int a = Convert.ToInt32(result.FirstOrDefault().MAXCOUNT.ToString());
                    int b = Convert.ToInt32(result.FirstOrDefault().ALLOTEDCOUNT.ToString());
                    if (a == b)
                    {
                        chart.msg = "Appointment were filled";
                    }
                    else if (b > a)
                    {

                    }
                    else if (a > b)
                    {
                        var updateflag = await iStudentRepository.updateflagappointment(chart);
                        {
                            var update = await iStudentRepository.updateappointment(chart);
                            if (update.flag > 0)
                            {
                                plan.note = await iStudentRepository.insertappointment(chart);
                                if (plan.note != null)
                                {
                                    chart.msg = "Appointment successful";
                                    chart.flag2 = "succ";
                                }
                                else
                                {
                                    chart.msg = "Appointment insertion failed count increased";
                                }
                            }

                        }



                    }
                    else
                    {

                    }
                }

                else
                {
                    chart.msg = "Appointment doesnot exist";
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(chart, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<ActionResult> CDLEvaluationReceipts()
        {
            List<CDLEvaluationReceipts> grade = new List<CDLEvaluationReceipts>();
            CDLSupplimentaryFeesSummary data = new CDLSupplimentaryFeesSummary();
            try
            {
                string REGDNO = Session["uid"].ToString();
                string course = "";
                string branch = Convert.ToString(Session["branch_code"]);

                data.data = await iStudentRepository.getEvaluationStudentDetails(REGDNO);
                if (data.data.Count() > 0)
                {
                    Session["TYPE"] = data.data.FirstOrDefault().additional_info;
                }
                else
                {
                    Session["TYPE"] = "";
                }
                if (data.data == null)
                {
                    data.data = new List<CDLEvaluationReceipts>();
                }
                data.subjects = new List<CDLEvaluationReceipts>();
                data.projectresult = new List<CDLEvaluationReceipts>();
                data.projectresult = new List<CDLEvaluationReceipts>();
                data.exam_centers = new List<CDLEvaluationReceipts>();
            }
            catch (Exception EX)
            {

            }

            return View("CDLEvaluationReceipts", data);
        }



        public async Task<ActionResult> getCDLFeeHistory()
        {
            IEnumerable<CDLEvaluationReceipts> data = null;
            CDLEvaluatoionFeesSummary summary = new CDLEvaluatoionFeesSummary();
            try
            {
                summary.fee = await iStudentRepository.getCDLFeeHistory(Convert.ToString(Session["uid"]));
                summary.eval_fee = await iStudentRepository.getCDLEvalFeeHistory(Convert.ToString(Session["uid"]));

            }
            catch (Exception ex)
            {

            }
            return Json(summary, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> getSupplimentaryDetails(string type)
        {
            List<CDLEvaluationReceipts> data = new List<CDLEvaluationReceipts>();

            String branch = Session["branch_code"].ToString();
            String course = Session["course_code"].ToString();
            String degree = Session["degree_code"].ToString();
            String college = Session["college_code"].ToString();
            String campus = Session["campus_code"].ToString();
            string processtype = type;
            try
            {
                data = await iStudentRepository.getCDLSupplyDetails(processtype, degree, course, campus, college);

                if (data.Count() > 0)
                {
                    if (degree == "PG" && type == "Revaluation")
                    {
                        data.ElementAt(0).Reason = "Not applicable";
                    }

                }


            }
            catch (Exception ex)
            {

            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> ProceedPayment(string type)
        {
            IEnumerable<CDLEvaluationReceipts> data = null;
            CDLEvaluationReceipts data1 = new CDLEvaluationReceipts();
            string REGDNO = Session["uid"].ToString();
            String branch = Session["branch_code"].ToString();
            String course = Session["course_code"].ToString();
            String degree = Session["degree_code"].ToString();
            String college = Session["college_code"].ToString();
            String campus = Session["campus_code"].ToString();
            String CLASS = Session["CLASS"].ToString();
            String BATCH = Session["batch"].ToString();
            String STUDENT_TYPE = Session["TYPE"].ToString();
            String TYPE = type;


            try
            {
                String VSEMTYPE = "ODD";
                data = await iStudentRepository.getCDLEvaluationSem(college);
                int rcnt = data.Count();

                foreach (var val in data)
                {
                    VSEMTYPE = val.SEMTYPE;
                }

                Session["VSEMTYPE"] = VSEMTYPE;

                SqlDataAdapter getexceptionalrs, getamountrs = null;
                SqlCommand getexceptionalcmd, getamountcmd = null;
                DataSet getexceptionalds, getamountds = null;
                DataTable getexceptionaldt, getamountdt = null;
                String getexceptionalqry, getamountqry = "";
                int getexceptionalrcount1, getamountrcount1 = 0;
                String EXCEPTIONALprocesstype = "";

                if (type.Equals("Supplementary"))
                {
                    EXCEPTIONALprocesstype = "SUPPLEMENTARY";
                }
                else if (type.Equals("Revaluation"))
                {
                    EXCEPTIONALprocesstype = "REVALUATION";
                }
                else if (type.Equals("Re_totaling"))
                {
                    EXCEPTIONALprocesstype = "RETOTALING";
                }

                else if (type.Equals("Betterment"))
                {
                    EXCEPTIONALprocesstype = "BETTERMENT";
                }

                Session["PROCESS_TYPE"] = EXCEPTIONALprocesstype;

                if (type.Equals("Supplementary"))
                {

                    String getdate = "";
                    String lastdate = "";
                    String processtype = "SUPPLEMENTARY";
                    SqlDataAdapter getlastdaters = null;
                    SqlCommand getlastdatecmd = null;
                    DataSet getlastdateds = null;
                    DataTable getlastdatedt = null;
                    String getlastdateqry = "";
                    int getlastdatercount1, rcount1 = 0;
                    String getlastdate = "";
                    String getlastdate1 = "";
                    String presyear = "";
                    String month = "";
                    DateTime finallastdate;
                    DateTime finallastdate1;
                    DateTime gettodaydate;
                    DateTime firstlastdate;
                    int presentyear;
                    String readmit = "N";
                    String[] batch1 = Session["BATCH"].ToString().Split('-');
                    int batch = Convert.ToInt32(batch1[0]);
                    int batch2 = Convert.ToInt32(batch1[1]);
                    int batch3 = batch2 - batch;

                    try
                    {

                        //sqlcon = g.getConnection();
                        //sqlcon.Open();

                        var getlastdatecmddata = await iStudentRepository.getCDLlastdate(processtype, degree, course, campus, college);
                        //getlastdateqry = "select convert(varchar,LASTDATE,105) as LASTDATE,lastdateamount,
                        //convert(varchar,LASTDATE1,105) as LASTDATE1,lastdateamount1,convert(varchar,getdate(),105) as getdate
                        //,convert(varchar,LASTDATE2,105) as LASTDATE2,lastdateamount2,month,year from CDL_DOE_DATES_INFO where process='" + processtype + "' and degree='" + degree + "' and course='" + course + "' and campus='" + campus + "' and college='" + college + "' and year = '2023'";
                        // and month=substring(CONVERT(VARCHAR(11), getdate(),106),4,3) and year=substring(CONVERT(VARCHAR(11), getdate(), 106),8,4)
                        // Response.Write(getlastdateqry);
                        // getlastdatecmd = new SqlCommand(getlastdateqry, sqlcon);
                        // getlastdaters = new SqlDataAdapter();
                        // getlastdaters.SelectCommand = getlastdatecmd;
                        // getlastdateds = new DataSet();
                        //getlastdateds.Tables.Clear();
                        // getlastdaters.Fill(getlastdateds);
                        // getlastdatedt = getlastdateds.Tables[0];
                        getlastdatercount1 = getlastdatecmddata.Count();
                        // Response.Write(getlastdatercount1);
                        if (getlastdatercount1 > 0)
                        {
                            String latefeeamount = "";
                            String latefeeamount1 = "";
                            String latefee = "";
                            foreach (var val in getlastdatecmddata)
                            {

                                lastdate = val.LASTDATE.ToString();
                                latefee = val.lastdateamount.ToString();
                                getlastdate = val.LASTDATE1.ToString();
                                latefeeamount = val.LASTDATEAMOUNT1.ToString();
                                getdate = val.getdate.ToString();
                                getlastdate1 = val.LASTDATE2.ToString();
                                latefeeamount1 = val.LASTDATEAMOUNT2.ToString();
                                month = val.MONTH.ToString();
                                presyear = val.YEAR.ToString();
                            }


                            gettodaydate = Convert.ToDateTime(getdate);
                            firstlastdate = Convert.ToDateTime(lastdate);
                            finallastdate = Convert.ToDateTime(getlastdate);
                            finallastdate1 = Convert.ToDateTime(getlastdate1);
                            presentyear = Convert.ToInt32(presyear);

                            DataSet ds1 = null;
                            DataTable dt1 = null;


                            var qry = await iStudentRepository.getCDLfeechallan(REGDNO);

                            // qry = "select * from fee_challan_master where regdno = '" + lblregdno.Text + "' and others1_desc like '%reg%' and college= 'CDL'";
                            //  Response.Write(qry);

                            rcount1 = qry.Count();
                            //   int rcount2 =(2 * batch3 + batch);

                            if (STUDENT_TYPE.Equals("ACADEMIC"))
                            {
                                if ((presentyear > (2 * batch3 + batch)) && (rcount1 == 0))
                                {
                                    readmit = "Y";
                                }
                                else if ((presentyear == (2 * batch3 + batch)) && (rcount1 == 0))
                                {
                                    if (month.Equals("Jun"))//if (month.Equals("mAY"))
                                    {
                                        readmit = "N";
                                    }
                                    else
                                    {
                                        readmit = "Y";

                                    }
                                }

                                else
                                {
                                    readmit = "N";
                                }
                            }
                            else if (STUDENT_TYPE.Equals("CALENDAR"))
                            {
                                if ((presentyear >= (2 * batch3 + batch)) && (rcount1 == 0))
                                {
                                    //if (month.Equals("Dec"))
                                    //{
                                    readmit = "Y";
                                    //}
                                    //else
                                    //{
                                    //    readmit = "Y";
                                    //}
                                }
                                else
                                {
                                    readmit = "N";
                                }
                            }

                            if (STUDENT_TYPE.Contains("DIPLOMA"))
                            {

                                if ((presentyear >= 2 + batch) && (rcount1 == 0))
                                {
                                    readmit = "Y";
                                }
                                else if ((presentyear == 2 + batch) && (rcount1 == 0))
                                {
                                    if (month.Equals("Jun")) //if (month.Equals("May"))
                                    {
                                        readmit = "N";
                                    }
                                    else
                                    {
                                        readmit = "Y";

                                    }
                                }
                                else
                                {
                                    readmit = "N";
                                }

                            }


                            readmit = "N";
                            if ((course.ToUpper().Equals("MBA") || course.ToUpper().Equals("MBAEXE") || course.ToUpper().Equals("MBAExe")) && (batch >= 2013))
                            {


                                if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                                {


                                    // Response.Redirect("MBA1_Supplimentry.aspx?latefee=" + latefeeamount + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount;
                                    data1.readmit = readmit;

                                }

                                else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                                {

                                    //  Response.Redirect("MBA1_Supplimentry.aspx?latefee=" + latefeeamount1 + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= firstlastdate)
                                {

                                    // Response.Redirect("MBA1_Supplimentry.aspx?latefee=" + latefee + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefee;
                                    data1.readmit = readmit;
                                }
                                else
                                {
                                    //Label1.Visible = true;
                                    data1.status = "Applications Closed";
                                }

                            }

                            else if ((course.ToUpper().Equals("MBA")) && batch >= 2013)
                            {


                                if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                                {

                                    //Response.Redirect("MBA_Supplimentry.aspx?latefee=" + latefeeamount + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                                {

                                    //Response.Redirect("MBA_Supplimentry.aspx?latefee=" + latefeeamount1 + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= firstlastdate)
                                {

                                    // Response.Redirect("MBA_Supplimentry.aspx?latefee=" + latefee + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }
                                else
                                {
                                    //Label1.Visible = true;
                                    data1.status = "Applications Closed";
                                }

                            }

                            else if (degree.Equals("PG") && course != "MBA" && course != "MBAExe" && course != "MCA")
                            {

                                if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                                {


                                    // Response.Redirect("PG_Supplimentry.aspx?latefee=" + latefeeamount + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                                {

                                    //Response.Redirect("PG_Supplimentry.aspx?latefee=" + latefeeamount1 + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= firstlastdate)
                                {

                                    //  Response.Redirect("PG_Supplimentry.aspx?latefee=" + latefee + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }
                                else
                                {
                                    //Label1.Visible = true;
                                    data1.status = "Applications Closed";
                                }

                            }
                            else if (degree.Equals("PG") && course.Equals("MCA"))
                            {
                                if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                                {


                                    // Response.Redirect("MCA_Supplimentry.aspx?latefee=" + latefeeamount + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                                {

                                    //Response.Redirect("MCA_Supplimentry.aspx?latefee=" + latefeeamount1 + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= firstlastdate)
                                {

                                    //Response.Redirect("MCA_Supplimentry.aspx?latefee=" + latefee + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }
                                else
                                {
                                    //Label1.Visible = true;
                                    data1.status = "Applications Closed";
                                }

                            }

                            else if (degree.Equals("UG"))
                            {
                                if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                                {


                                    //  Response.Redirect("UG_Supplimentry.aspx?latefee=" + latefeeamount + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                                {

                                    //Response.Redirect("UG_Supplimentry.aspx?latefee=" + latefeeamount1 + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }

                                else if (gettodaydate <= firstlastdate)
                                {

                                    //Response.Redirect("UG_Supplimentry.aspx?latefee=" + latefee + "&readmit=" + readmit + "");
                                    data1.status = "MBA1_Supplimentry";
                                    data1.latefeeamount = latefeeamount1;
                                    data1.readmit = readmit;
                                }
                                else
                                {
                                    //Label1.Visible = true;
                                    data1.status = "Applications Closed";
                                }

                            }
                        }

                        else
                        {
                            //Label1.Visible = true;
                            data1.status = "No Details Found.";
                        }

                    }
                    catch (Exception e1)
                    {
                        // Label1.Visible = true;
                        data1.status = e1.Message;
                    }
                    finally
                    {
                        // sqlcon.Close();

                    }
                }
                else if (type.Equals("Revaluation") && degree.Equals("UG"))
                {

                    String getdate = "";
                    String lastdate = "";
                    String processtype = "REVALUATION";
                    SqlDataAdapter getlastdaters = null;
                    SqlCommand getlastdatecmd = null;
                    DataSet getlastdateds = null;
                    DataTable getlastdatedt = null;
                    String getlastdateqry = "";
                    int getlastdatercount1 = 0;
                    String getlastdate = "";
                    String getlastdate1 = "";
                    DateTime finallastdate;
                    DateTime finallastdate1;
                    DateTime gettodaydate;
                    DateTime firstlastdate;
                    String month = null;
                    int year = 0;
                    //  String getlastdate1 = "";
                    //   DateTime finallastdate1;
                    try
                    {

                        var getlastdatecmddata = await iStudentRepository.getCDLlastdate(processtype, degree, course, campus, college);

                        getlastdatercount1 = getlastdatecmddata.Count();
                        // Response.Write(getlastdatercount1);
                        if (getlastdatercount1 > 0)
                        {
                            String latefeeamount = "";
                            String latefeeamount1 = "";
                            String latefee = "";
                            foreach (var val in getlastdatecmddata)
                            {

                                lastdate = val.LASTDATE.ToString();
                                latefee = val.lastdateamount.ToString();
                                getlastdate = val.LASTDATE1.ToString();
                                latefeeamount = val.LASTDATEAMOUNT1.ToString();
                                getdate = val.getdate.ToString();
                                getlastdate1 = val.LASTDATE2.ToString();
                                latefeeamount1 = val.LASTDATEAMOUNT2.ToString();
                                month = val.MONTH.ToString();
                                year = Convert.ToInt32(val.YEAR);
                            }



                            gettodaydate = Convert.ToDateTime(getdate);
                            firstlastdate = Convert.ToDateTime(lastdate);
                            finallastdate = Convert.ToDateTime(getlastdate);
                            finallastdate1 = Convert.ToDateTime(getlastdate1);



                            if (gettodaydate <= finallastdate && gettodaydate > firstlastdate)
                            {
                                //Response.Redirect("Revaluation.aspx?latefee=" + latefeeamount + "&month1=" + month + "&year1= " + year + "");
                                data1.status = "Revaluation";
                                data1.latefeeamount = latefeeamount1;
                                data1.MONTH = month;
                                data1.YEAR = Convert.ToString(year);
                            }

                            else if (gettodaydate <= finallastdate1 && gettodaydate > finallastdate)
                            {
                                //Response.Redirect("Revaluation.aspx?latefee=" + latefeeamount1 + "&month1=" + month + "&year1= " + year + "");
                                data1.status = "Revaluation";
                                data1.latefeeamount = latefeeamount1;
                                data1.MONTH = month;
                                data1.YEAR = Convert.ToString(year);
                            }

                            else if (gettodaydate <= firstlastdate)
                            {
                                // Response.Redirect("Revaluation.aspx?latefee=" + latefee + "&month1=" + month + "&year1= " + year + "");
                                data1.status = "Revaluation";
                                data1.latefeeamount = latefeeamount1;
                                data1.MONTH = month;
                                data1.YEAR = Convert.ToString(year);
                            }
                            else
                            {
                                data1.status = "Applications Closed";
                            }
                        }
                        else
                        {
                            data1.status = "No Details Found";

                        }

                    }

                    catch (Exception e1)
                    {

                    }



                }

            }
            catch (Exception ex)
            {

            }
            return Json(data1, JsonRequestBehavior.AllowGet);
        }





        public async Task<ActionResult> MBA1_Supplimentry(string latefee, string readmit)
        {
            //IEnumerable<CDLEvaluationReceipts> data = null;
            CDLSupplimentaryFeesSummary data = new CDLSupplimentaryFeesSummary();
            try
            {
                data.exam_centers = await iStudentRepository.getCDLExamCentre();
                String vsemlist = "in (1,2,3,4,5,6) ";
                String vcollegecode = Session["COLLEGE_CODE"].ToString().Trim();
                string REGDNO = Session["uid"].ToString();

                //lblsem1.text = semester;


                data.subjects = await iStudentRepository.getCDLSupplySubjects(REGDNO, vsemlist);
                data.projectresult = await iStudentRepository.getCDLProjectResults(REGDNO, vsemlist);
                CDLEvaluationReceipts d = new CDLEvaluationReceipts()
                {
                    latefeeamount = latefee,
                    readmit = readmit
                };
                data.data = new List<CDLEvaluationReceipts>();
                data.data.Add(d);

            }
            catch (Exception ex)
            {

            }

            return PartialView("SupplimentryFee", data);
        }
        [HttpPost]
        public async Task<ActionResult> ProceedPay(FormCollection collection)
        {
            List<CDLEvaluationReceipts> list = new List<CDLEvaluationReceipts>();
            CDLSupplimentaryFeesSummary data = new CDLSupplimentaryFeesSummary();
            CDLEvaluationReceipts status = new CDLEvaluationReceipts();

            string REGDNO = Session["uid"].ToString();
            String branch = Session["branch_code"].ToString();
            String course = Session["course_code"].ToString();
            String degree = Session["degree_code"].ToString();
            String college = Session["college_code"].ToString();
            String campus = Session["campus_code"].ToString();
            String CLASS = Session["CLASS"].ToString();
            String batch = Session["batch"].ToString();
            String STUDENT_TYPE = Session["TYPE"].ToString();


            String name = Session["NAME"].ToString();

            String sessionid = Session.SessionID.ToString();
            int refid = await getCDLreferenceid();
            Session["REFID"] = refid;
            string type = Session["PROCESS_TYPE"].ToString();
            try
            {
                string[] count = collection.GetValues("count");
                string[] projectresult = collection.GetValues("projectresult");
                string[] totalfee = collection.GetValues("totalfee");
                string[] examcenter = collection.GetValues("examcenter");
                for (int i = 1; i <= 6; i++)
                {

                    string[] checkbox = collection.GetValues("checkbox_" + i);
                    string[] subjectcode = collection.GetValues("subjectcode_" + i);
                    string[] subjectname = collection.GetValues("subjectname_" + i);
                    string[] semester = collection.GetValues("semester_" + i);
                    string[] totalsem = collection.GetValues("totalsem_" + i);




                    if (checkbox != null)
                    {
                        for (int j = 0; j < subjectcode.Length; j++)
                        {
                            for (int k = 0; k < checkbox.Length; k++)
                            {
                                if (subjectcode[j] == checkbox[k])
                                {
                                    list.Add(new CDLEvaluationReceipts
                                    {
                                        SUBJECT_CODE = subjectcode[j],
                                        SUBJECT_NAME = subjectname[j],
                                        SEMESTER = semester[j],
                                        Total_fee = totalfee[0],
                                        REGDNO = REGDNO,
                                        BRANCH_CODE = branch,
                                        COURSE_CODE = course,
                                        DEGREE_CODE = degree,
                                        COLLEGE_CODE = college,
                                        CAMPUS = campus,
                                        CLASS = CLASS,
                                        batch = batch,
                                        Refid = Convert.ToString(refid),
                                        sessionid = sessionid,
                                        exam_centre = examcenter[0],
                                        Type = type,
                                        NAME = name

                                    });

                                }
                            }

                        }


                    }





                }
                if (projectresult != null)
                {
                    if (projectresult[0] == "Y")
                    {
                        list.Add(new CDLEvaluationReceipts
                        {
                            SUBJECT_CODE = "Project & Viva",
                            SUBJECT_NAME = "Project & Viva",
                            SEMESTER = "",
                            Total_fee = totalfee[0],
                            REGDNO = REGDNO,
                            BRANCH_CODE = branch,
                            COURSE_CODE = course,
                            DEGREE_CODE = degree,
                            COLLEGE_CODE = college,
                            CAMPUS = campus,
                            CLASS = CLASS,
                            batch = batch,
                            Refid = Convert.ToString(refid),
                            sessionid = sessionid,
                            exam_centre = examcenter[0],
                            Type = type,
                            NAME = name

                        });
                    }
                }
                if (list.Count() > 0)
                {

                    var EVALUATION_REQUESTS = await iStudentRepository.InsertEvaluationRequest(list);

                    Session["totalamount"] = totalfee[0];

                    String FEETYPE = "EXAMINATION";
                    Session["CATEGORY"] = "EXAMINATION";
                    var EVALUATION_feetotal = await iStudentRepository.InsertTotalFee(REGDNO, name, totalfee[0], Convert.ToString(refid), FEETYPE);

                    if (EVALUATION_feetotal == 1)
                    {
                        return RedirectToAction("Receiptconfirmation");
                    }
                    else
                    {
                        status.Reason = "Cancelled";
                    }

                }
                else
                {
                    status.Reason = "Cancelled";
                }
            }
            catch (Exception ex)
            {

            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }




        public async Task<int> getCDLreferenceid()
        {


            int refid = 0;
            try
            {

                var data = await iStudentRepository.getCDLRefid();

                if (data.Count() > 0)
                {
                    refid = Convert.ToInt32(data.FirstOrDefault().Refid);
                }


                refid = refid + 1;
                int result = await iStudentRepository.updateCDLRefid(refid);





            }
            catch (Exception ex)
            {
                // lblerror.Text = ex.Message;
            }

            return refid;

        }


        public async Task<ActionResult> Receiptconfirmation()
        {
            string REGDNO = Session["uid"].ToString();

            string name = Session["NAME"].ToString();
            String sessionid = Session.SessionID.ToString();
            List<CDLEvaluationReceipts> data = new List<CDLEvaluationReceipts>();
            try
            {
                data = await iStudentRepository.getReceiptconfirmation(REGDNO, sessionid);

                var update = await iStudentRepository.UpdateCDLevaluationRequest(sessionid);

                data.ElementAt(0).PROCESS_TYPE = Convert.ToString(Session["PROCESS_TYPE"]);
                data.ElementAt(0).Category = "EXAMINATION";
                data.ElementAt(0).Refid = Convert.ToString(Session["REFID"]);



            }
            catch (Exception ex)
            {

            }
            return Json(new { data = Helper.RenderViewToString(this.ControllerContext, "ReceiptConfirm", data) }, JsonRequestBehavior.AllowGet);

            //return PartialView("ReceiptConfirm", data);
        }





        public async Task<ActionResult> Revaluation(string latefee, string month, string year)
        {
            //IEnumerable<CDLEvaluationReceipts> data = null;
            CDLSupplimentaryFeesSummary data = new CDLSupplimentaryFeesSummary();
            try
            {



                String vevenodd = "ODD";

                String vsemlist = "in (1,3,5,7,9,11,13,15) ";

                if (vevenodd.Trim().Equals("EVEN"))
                    vsemlist = "in (2,4,6,8,10,12,14) ";

                String vcollegecode = Session["COLLEGE_CODE"].ToString().Trim();


                if (vcollegecode.Equals("GIM") || vcollegecode.Equals("HBS") || vcollegecode.Equals("SMS"))
                    if (vevenodd.Trim().Equals("ODD"))

                        vsemlist = "in (1,3,4,5,7,3,6,9) ";
                    else

                        vsemlist = "in (2,4,5,7,3,6,9,12) ";

                string REGDNO = Session["uid"].ToString();
                String branch = Session["branch_code"].ToString();
                String course = Session["course_code"].ToString();
                String degree = Session["degree_code"].ToString();
                String college = Session["college_code"].ToString();
                String campus = Session["campus_code"].ToString();
                String CLASS = Session["CLASS"].ToString();
                String batch = Session["batch"].ToString();
                String STUDENT_TYPE = Session["TYPE"].ToString();
                String name = Session["NAME"].ToString();


                SqlDataAdapter rs1 = null;
                // query = "  SELECT (SUBJECT_CODE+'-'+SUBJECT_NAME),SEMESTER,subject_code,subject_name FROM CDL_EVALUATION_RESULTS WHERE REGDNO = '" + lblregdno.Text + "' AND SUB_TYPE = 'T' and semester " + vsemlist + " and month ='" + month + "' and year = " + year + "";

                String VSEM = "";


                data.exam_centers = await iStudentRepository.getCDLExamCentre();




                data.subjects = await iStudentRepository.getCDLRevalutionResult(REGDNO, vsemlist, month, year);
                data.projectresult = await iStudentRepository.getCDLProjectResults(REGDNO, vsemlist);
                CDLEvaluationReceipts d = new CDLEvaluationReceipts()
                {
                    latefeeamount = latefee,
                    //readmit = readmit
                };
                data.data = new List<CDLEvaluationReceipts>();
                data.data.Add(d);

            }
            catch (Exception ex)
            {

            }

            return PartialView("SupplimentryFee", data);
        }




        [HttpPost]
        public async Task<ActionResult> ProceedPayCertificate(FormCollection collection)
        {
            List<CDLEvaluationReceipts> list = new List<CDLEvaluationReceipts>();
            CDLSupplimentaryFeesSummary data = new CDLSupplimentaryFeesSummary();
            CDLEvaluationReceipts status = new CDLEvaluationReceipts();

            string REGDNO = Session["uid"].ToString();
            String branch = Session["branch_code"].ToString();
            String course = Session["course_code"].ToString();
            String degree = Session["degree_code"].ToString();
            String college = Session["college_code"].ToString();
            String campus = Session["campus_code"].ToString();
            String CLASS = Session["CLASS"].ToString();
            String batch = Session["batch"].ToString();
            String STUDENT_TYPE = Session["TYPE"].ToString();
            string certificate = "";
            string category = "";

            String name = Session["NAME"].ToString();

            String sessionid = Session.SessionID.ToString();
            int refid = await getCDLreferenceid();
            Session["REFID"] = refid;
            //string type = Session["PROCESS_TYPE"].ToString();
            try
            {


                int amt = 0;


                string[] India = collection.GetValues("India");
                string[] IndiaAbroadTextbox = collection.GetValues("IndiaAbroadTextbox");
                string[] checkboxTrascript = collection.GetValues("checkboxTrascript");
                string[] counterval = collection.GetValues("counterval");
                string[] lbltotamount300 = collection.GetValues("lbltotamount300");
                string[] TextBox300 = collection.GetValues("TextBox300");
                string[] lbltotamount500 = collection.GetValues("lbltotamount500");
                string[] TextBox500 = collection.GetValues("TextBox500");
                string[] lbltotamount1700 = collection.GetValues("lbltotamount1700");
                string[] TextBox1700 = collection.GetValues("TextBox1700");
                string[] lbltotamount = collection.GetValues("lbltotamount");

                if (India != null)
                {
                    certificate = "CREDENTIAL";
                    category = "CERTIFICATE";
                    status = new CDLEvaluationReceipts()
                    {

                        REGDNO = REGDNO,
                        BRANCH_CODE = branch,
                        COURSE_CODE = course,
                        DEGREE_CODE = degree,
                        COLLEGE_CODE = college,
                        CAMPUS = campus,
                        CLASS = CLASS,
                        batch = batch,
                        Refid = Convert.ToString(refid),
                        sessionid = sessionid,
                        certificate = certificate,
                        Category = category,
                        Type = India[0],
                        amount = IndiaAbroadTextbox[0],
                        bypost = "0",
                        SEMESTER = "0",
                        NAME = name

                    };
                    var EVALUATION_feetotal = await iStudentRepository.InsertCertificatedata(status);

                }
                if (checkboxTrascript != null)
                {
                    certificate = "TRANSCRIPTS";
                    category = "CERTIFICATE";
                    status = new CDLEvaluationReceipts()
                    {

                        REGDNO = REGDNO,
                        BRANCH_CODE = branch,
                        COURSE_CODE = course,
                        DEGREE_CODE = degree,
                        COLLEGE_CODE = college,
                        CAMPUS = campus,
                        CLASS = CLASS,
                        batch = batch,
                        Refid = Convert.ToString(refid),
                        sessionid = sessionid,
                        certificate = certificate,
                        Category = category,
                        Type = "ORIGINAL",
                        amount = counterval[0],
                        bypost = "0",
                        SEMESTER = "0",
                        NAME = name

                    };
                    var EVALUATION_feetotal = await iStudentRepository.InsertCertificatedata(status);

                }
                if (lbltotamount300 != null)
                {
                    certificate = "WES";
                    category = "CERTIFICATE";
                    int amt1 = Convert.ToInt32(TextBox300[0]) + 300;
                    status = new CDLEvaluationReceipts()
                    {

                        REGDNO = REGDNO,
                        BRANCH_CODE = branch,
                        COURSE_CODE = course,
                        DEGREE_CODE = degree,
                        COLLEGE_CODE = college,
                        CAMPUS = campus,
                        CLASS = CLASS,
                        batch = batch,
                        Refid = Convert.ToString(refid),
                        sessionid = sessionid,
                        certificate = certificate,
                        Category = category,
                        Type = "ORIGINAL",
                        amount = Convert.ToString(amt1),
                        bypost = "0",
                        SEMESTER = "0",
                        NAME = name

                    };
                    var EVALUATION_feetotal = await iStudentRepository.InsertCertificatedata(status);

                }
                if (lbltotamount500 != null)
                {
                    certificate = "E-Transfer";
                    category = "CERTIFICATE";
                    int amt1 = Convert.ToInt32(TextBox500[0]) + 500;
                    status = new CDLEvaluationReceipts()
                    {

                        REGDNO = REGDNO,
                        BRANCH_CODE = branch,
                        COURSE_CODE = course,
                        DEGREE_CODE = degree,
                        COLLEGE_CODE = college,
                        CAMPUS = campus,
                        CLASS = CLASS,
                        batch = batch,
                        Refid = Convert.ToString(refid),
                        sessionid = sessionid,
                        certificate = certificate,
                        Category = category,
                        Type = "ORIGINAL",
                        amount = Convert.ToString(amt1),
                        bypost = "0",
                        SEMESTER = "0",
                        NAME = name

                    };
                    var EVALUATION_feetotal = await iStudentRepository.InsertCertificatedata(status);

                }
                if (lbltotamount1700 != null)
                {
                    certificate = "OD";
                    category = "CERTIFICATE";
                    int amt1 = Convert.ToInt32(TextBox1700[0]) + 1700;
                    status = new CDLEvaluationReceipts()
                    {

                        REGDNO = REGDNO,
                        BRANCH_CODE = branch,
                        COURSE_CODE = course,
                        DEGREE_CODE = degree,
                        COLLEGE_CODE = college,
                        CAMPUS = campus,
                        CLASS = CLASS,
                        batch = batch,
                        Refid = Convert.ToString(refid),
                        sessionid = sessionid,
                        certificate = certificate,
                        Category = category,
                        Type = "ORIGINAL",
                        amount = Convert.ToString(amt1),
                        bypost = "0",
                        SEMESTER = "0",
                        NAME = name

                    };
                    var EVALUATION_feetotal = await iStudentRepository.InsertCertificatedata(status);

                }


                Session["totalamount"] = lbltotamount[0];
                Session["CATEGORY"] = "CERTIFICATE";
                Session["PROCESS_TYPE"] = certificate;
                String FEETYPE = "CERTIFICATE";
                status = new CDLEvaluationReceipts()
                {

                    REGDNO = REGDNO,
                    BRANCH_CODE = branch,
                    COURSE_CODE = course,
                    DEGREE_CODE = degree,
                    COLLEGE_CODE = college,
                    CAMPUS = campus,
                    CLASS = CLASS,
                    batch = batch,
                    Refid = Convert.ToString(refid),
                    sessionid = sessionid,
                    certificate = certificate,
                    Category = category,
                    Type = "CERTIFICATE",
                    Total_fee = Convert.ToString(lbltotamount[0]),
                    bypost = "0",
                    SEMESTER = "0",
                    NAME = name

                };
                var feetotal = await iStudentRepository.InsertCertificatedataTotal(status);

                if (feetotal == 1)
                {
                    return RedirectToAction("Receiptconfirmationcertifictes");
                }
                else
                {
                    status.Reason = "Cancelled";
                }






                //}
                //if (projectresult != null)
                //{
                //    if (projectresult[0] == "Y")
                //    {
                //        list.Add(new CDLEvaluationReceipts
                //        {
                //            SUBJECT_CODE = "Project & Viva",
                //            SUBJECT_NAME = "Project & Viva",
                //            SEMESTER = "",
                //            Total_fee = totalfee[0],
                //            REGDNO = REGDNO,
                //            BRANCH_CODE = branch,
                //            COURSE_CODE = course,
                //            DEGREE_CODE = degree,
                //            COLLEGE_CODE = college,
                //            CAMPUS = campus,
                //            CLASS = CLASS,
                //            batch = batch,
                //            Refid = Convert.ToString(refid),
                //            sessionid = sessionid,
                //            exam_centre = examcenter[0],
                //            Type = type

                //        });
                //    }
                //}
                //if (list.Count() > 0)
                //{

                //    var EVALUATION_REQUESTS = await iStudentRepository.InsertEvaluationRequest(list);

                //    Session["totalamount"] = totalfee[0];

                //    String FEETYPE = "EXAMINATION";
                //    Session["CATEGORY"] = "EXAMINATION";
                //    var EVALUATION_feetotal = await iStudentRepository.InsertTotalFee(REGDNO, name, totalfee[0], Convert.ToString(refid), FEETYPE);

                //    if (EVALUATION_feetotal == 1)
                //    {
                //        return RedirectToAction("Receiptconfirmation");
                //    }
                //    else
                //    {
                //        status.Reason = "Cancelled";
                //    }

                //}
                //else
                //{
                //    status.Reason = "Cancelled";
                //}
            }
            catch (Exception ex)
            {

            }

            return Json(status, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Receiptconfirmationcertifictes()
        {
            string REGDNO = Session["uid"].ToString();

            string name = Session["NAME"].ToString();
            String sessionid = Session.SessionID.ToString();
            List<CDLEvaluationReceipts> data = new List<CDLEvaluationReceipts>();
            try
            {
                data = await iStudentRepository.getReceiptconfirmationCertificate(REGDNO, sessionid);

                var update = await iStudentRepository.UpdateCDLevaluationRequest(sessionid);

                data.ElementAt(0).PROCESS_TYPE = Convert.ToString(Session["PROCESS_TYPE"]);
                data.ElementAt(0).Category = "CERTIFICATE";
                data.ElementAt(0).Refid = Convert.ToString(Session["REFID"]);
                String request_list = "";

                foreach (var val in data)
                {
                    request_list = val.REQUEST_DESC + "--" + request_list;

                }
                data.ElementAt(0).request_list = request_list;
                data.ElementAt(0).Total_fee = Session["totalamount"].ToString();

            }
            catch (Exception ex)
            {

            }
            return Json(new { data = Helper.RenderViewToString(this.ControllerContext, "CertificateConfirm", data) }, JsonRequestBehavior.AllowGet);

            //return PartialView("ReceiptConfirm", data);
        }

        [HttpGet]
        public async Task<ActionResult> CourseRegistration(string set)
        {
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Convert.ToInt32(Session["Curr_sem"]));
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);
            Courseregistartion record = new Courseregistartion();

            record.feepaid = await iStudentRepository.getfeepaid(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            var vpf = record.feepaid.vpf;
            record.feepaid = await iStudentRepository.getlatefee(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            var latefee = record.feepaid.latefee;
            record.feepaid = await iStudentRepository.getotherfee(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            string lbltotalamount = (Convert.ToDecimal(record.feepaid.vafee) + Convert.ToDecimal(record.feepaid.vsemfee1) - Convert.ToDecimal(vpf) + Convert.ToDecimal(latefee) - Convert.ToDecimal(record.feepaid.vscholorship)).ToString();

            record.lbltotalamount = lbltotalamount;


            IEnumerable<Students> accessdata = await iStudentRepository.getaccessdata(Convert.ToString(Session["uid"]));
            record.accesscount = 0;
            if (accessdata.Count() > 0)
            {
                record.accesscount = accessdata.Count();
            }


            IEnumerable<Students> txndata = await iStudentRepository.getTxn_flag(Convert.ToString(Session["uid"]), SEMESTER);
            record.txn_flag = 0;
            if (txndata.Count() > 0)
            {
                record.txn_flag = txndata.Count();
            }



            IEnumerable<Students> sem13_flag = await iStudentRepository.getSEM13_SGPA(Convert.ToString(Session["uid"]), SEMESTER);
            record.sem13_sgpa = 0;
            if (sem13_flag.Count() > 0)
            {
                record.sem13_sgpa = sem13_flag.Count();
            }
            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);



            record.semcredits = await iStudentRepository.getcredits_currsem(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            List<Students> newsemcredits = new List<Students>();
            //= record.semcredits;

            foreach (var scredit in record.semcredits)
            {
                string fileName = @"C:/CATS_PROJECTS/Coursesyllabus/" + scredit.SUBJECT_CODE + ".pdf";

                if (fileName != "")
                {
                    bool fileExists = System.IO.File.Exists(fileName);
                    if (fileExists)
                    {
                        scredit.file_flag = "1";

                        newsemcredits.Add(scredit);
                    }
                    else
                    {
                        newsemcredits.Add(scredit);
                    }
                }
            }

            record.semcredits = newsemcredits;

            record.credits = await iStudentRepository.getplan_currsem(Convert.ToString(Session["uid"]), SEMESTER);
            record.sets = await iStudentRepository.getsets(Session["campus_code"].ToString(), Session["branch_code"].ToString(), batch2, SEMESTER);

            IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, Session["degree_code"].ToString());

            if (set == null)
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, batch2, "SET1");
            }
            else
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, set);
            }
            ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday"
                };
            ViewBag.list = sessionslist.OrderBy(x => x.timeslots).Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();
            record.upcomingtimetable = await iStudentRepository.getupcomingtimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);
            return View("CourseRegistration", record);
        }



        [HttpGet]
        public async Task<ActionResult> CourseRegistrationnewodd(string set)
        {
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Convert.ToInt32(Session["Curr_sem"]));
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);
            Courseregistartion record = new Courseregistartion();

            IEnumerable<Students> txndata = await iStudentRepository.getarrears(Convert.ToString(Session["uid"]), SEMESTER);
            IEnumerable<Students> txndatahostel = await iStudentRepository.getarrearshostel(Convert.ToString(Session["uid"]), SEMESTER);
            IEnumerable<Students> txndatahostelbalance = await iStudentRepository.getarrearshostelbalanacedata(Convert.ToString(Session["uid"]), SEMESTER);
            record.arrears_flag = 0;

            if (txndata.Count() > 0)
            {
                record.arrears_flag = txndata.Count();
                record.status_flag = Convert.ToInt32(txndata.FirstOrDefault().statusflag);

            }
            else
            {
                record.arrears_flag = 0;
                record.status_flag = 0;
            }

            if (txndatahostel.Count() > 0)
            {
                //record.hostel_arrears_flag = txndatahostel.Count();
                int hostel_arreas_value = 0;
                foreach (var srm in txndatahostel)
                {
                    hostel_arreas_value = Convert.ToInt32(srm.hostel_arrears);

                }

                record.hostel_arrears_flag = hostel_arreas_value;

            }
            else
            {
                record.hostel_arrears_flag = 0;

            }


            if (txndatahostelbalance.Count() > 0)
            {
                int hostel_value = 0;
                foreach (var srm in txndatahostelbalance)
                {
                    hostel_value = Convert.ToInt32(srm.total_fee);

                }

                record.hostel_balance_flag = hostel_value;


            }
            else
            {
                record.hostel_balance_flag = 0;

            }
            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);

            record.semcredits = await iStudentRepository.getcredits_currsem(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            List<Students> newsemcredits = new List<Students>();

            IEnumerable<Students> accessdata = await iStudentRepository.getaccessdata(Convert.ToString(Session["uid"]));
            record.accesscount = 0;
            if (accessdata.Count() > 0)
            {
                record.accesscount = accessdata.Count();
            }

            foreach (var scredit in record.semcredits)
            {
                string fileName = @"C:/CATS_PROJECTS/Coursesyllabus/" + scredit.SUBJECT_CODE + ".pdf";

                if (fileName != "")
                {
                    bool fileExists = System.IO.File.Exists(fileName);
                    if (fileExists)
                    {
                        scredit.file_flag = "1";

                        newsemcredits.Add(scredit);
                    }
                    else
                    {
                        newsemcredits.Add(scredit);
                    }
                }
            }

            record.semcredits = newsemcredits;

            record.credits = await iStudentRepository.getplan_currsem(Convert.ToString(Session["uid"]), SEMESTER);
            record.sets = await iStudentRepository.getsets(Session["campus_code"].ToString(), Session["branch_code"].ToString(), batch2, SEMESTER);

            IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, Session["degree_code"].ToString());

            if (set == null)
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, batch2, "SET1");
            }
            else
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, set);
            }
            ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday"
                };
            ViewBag.list = sessionslist.Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();
            record.upcomingtimetable = await iStudentRepository.getupcomingtimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);
            return View("CourseRegistrationnew", record);

        }


        [HttpGet]
        public async Task<ActionResult> CourseRegistrationneweven(string set)
        {
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Convert.ToInt32(Session["Curr_sem"]));
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);
            Courseregistartion record = new Courseregistartion();


            IEnumerable<Students> txndatatutionbalance = await iStudentRepository.gettxndatatutionbalance(Convert.ToString(Session["uid"]), SEMESTER);

            IEnumerable<Students> txndatastudattnd= await iStudentRepository.getTBL_ALLOW_STUDENTS(Convert.ToString(Session["uid"]), SEMESTER);


        
            record.arrears_flag = 0;
            record.status_flag = 0;
           if (txndatatutionbalance.Count() > 0)
            {
                record.arrears_flag = 1;
            }
            else
            {
                record.arrears_flag = 0;              
            }
            if (txndatastudattnd.Count() > 0)
            {
                record.status_flag = 1;
            }
            else
            {
                record.status_flag = 0;
            }



            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);

            record.semcredits = await iStudentRepository.getcredits_currsem(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            List<Students> newsemcredits = new List<Students>();

            IEnumerable<Students> accessdata = await iStudentRepository.getaccessdata(Convert.ToString(Session["uid"]));
            record.accesscount = 0;
            if (accessdata.Count() > 0)
            {
                record.accesscount = accessdata.Count();
            }

            foreach (var scredit in record.semcredits)
            {
                string fileName = @"C:/CATS_PROJECTS/Coursesyllabus/" + scredit.SUBJECT_CODE + ".pdf";

                if (fileName != "")
                {
                    bool fileExists = System.IO.File.Exists(fileName);
                    if (fileExists)
                    {
                        scredit.file_flag = "1";

                        newsemcredits.Add(scredit);
                    }
                    else
                    {
                        newsemcredits.Add(scredit);
                    }
                }
            }

            record.semcredits = newsemcredits;

            record.credits = await iStudentRepository.getplan_currsem(Convert.ToString(Session["uid"]), SEMESTER);
            record.sets = await iStudentRepository.getsets(Session["campus_code"].ToString(), Session["branch_code"].ToString(), batch2, SEMESTER);

            IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, Session["degree_code"].ToString());

            if (set == null)
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, batch2, "SET1");
            }
            else
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, set);
            }
            ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday"
                };
            ViewBag.list = sessionslist.Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();
            record.upcomingtimetable = await iStudentRepository.getupcomingtimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH);
            return View("CourseRegistrationneweven", record);

        }

        [HttpGet]
        public async Task<ActionResult> getsetcourses(string set)
        {
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Convert.ToInt32(Session["Curr_sem"]) + 1);
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);



            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);


            Courseregistartion record = new Courseregistartion();
            record.semcredits = await iStudentRepository.getcredits_currsem(Convert.ToString(Session["uid"]), Convert.ToString(Session["Curr_sem"]));
            record.credits = await iStudentRepository.getplan_currsem(Convert.ToString(Session["uid"]), SEMESTER);
            record.sets = await iStudentRepository.getsets(Session["campus_code"].ToString(), Session["branch_code"].ToString(), batch2, SEMESTER);

            IEnumerable<Timetable> sessionslist = await iStudentRepository.getslots(COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, COURSE_CODE, Session["degree_code"].ToString());

            if (set == null)
            {
                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, "SET1");
            }
            else
            {

                //set = Regex.Replace(set, @"[^0-9]", "");
                set = set.Replace("-", string.Empty);


                record.semlist = await iStudentRepository.getsettimetable(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, set);
            }
            ViewBag.days = new List<string>()
                {
                    "Monday","Tuesday","Wednesday","Thursday","Friday"
                };
            ViewBag.list = sessionslist.Select(list => new List<Timetable>()
                {
                     new Timetable() { timeslots = list.timeslots},
                }).ToList();

            return View("Courseview", record);
        }


        [HttpGet]
        public async Task<JsonResult> Getpopupfaculty(string set, string subjectcode, string sec)
        {
            string COLLEGE_CODE = Convert.ToString(Session["college_code"]);
            string BRANCH_CODE = Convert.ToString(Session["branch_code"]);
            string SEMESTER = Convert.ToString(Convert.ToInt32(Session["Curr_sem"]) + 1);
            string SECTION = Convert.ToString(Session["section"]);
            string CAMPUS_CODE = Convert.ToString(Session["campus_code"]);
            string userid = Convert.ToString(Session["uid"]);
            string COURSE_CODE = Convert.ToString(Session["course_code"]);

            string[] batch1 = new string[1000];
            string batch = Session["batch"].ToString();
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            string BATCH = batch2 + '-' + Convert.ToString(Convert.ToUInt32(batch2) + 1);
            set = set.Replace("-", string.Empty);
            IEnumerable<Timetable> summary = await iStudentRepository.getpopupfaculty(Session["REGDNO"].ToString(), COLLEGE_CODE, BRANCH_CODE, SEMESTER, SECTION, CAMPUS_CODE, BATCH, set, subjectcode, sec);

            return Json(summary, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult PdfGetcourse(string PDFID)
        {
            string[] pdfpub = PDFID.Split('_');
            string[] pdfpub1 = PDFID.Split('/');
            string pdfabt, fileName = "";

            //PDFID = pdfpub1[2];

            fileName = @"C:/CATS_PROJECTS/Coursesyllabus/" + PDFID + ".pdf";

            if (fileName != "")
            {
                bool fileExists = System.IO.File.Exists(fileName);
                if (fileExists)
                {
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64 = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);

                    return Content(base64);
                    //return File(pdfByteArray, "application/pdf");
                }
            }

            return Content("PDF file not found");
        }
        public async Task<ActionResult> Gethalltickets()
        {
            Hallticket chart = new Hallticket();

            hallticketsummary admission = new hallticketsummary();
            admission.studentrooms = await iStudentRepository.getexamtimetable(Session["Regdno"].ToString());


            IEnumerable<Hallticket> txndata = await iStudentRepository.gethallticketactive(Convert.ToString(Session["uid"]));

            if (txndata.Count() == 0)
            {
                admission.activecount = txndata.Count();
            }
            else
            {
                admission.activecount = txndata.Count();


            }

            try
            {
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
                        admission.notes.ElementAt(i).type2 = "Re-Registration";
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
            }
            if (Session["status"].ToString() != "S")
            {
                return PartialView("Hallticketskip", admission);
            }
            else
            {
                return PartialView("Hallticket", admission);
            }
        }



        public async Task<ActionResult> Gethallticketsreleived()
        {
            Hallticket chart = new Hallticket();

            hallticketsummary admission = new hallticketsummary();
            admission.studentrooms = await iStudentRepository.getexamtimetable(Session["Regdno"].ToString());


            IEnumerable<Hallticket> txndata = await iStudentRepository.gethallticketactive(Convert.ToString(Session["uid"]));

            if (txndata.Count() == 0)
            {
                admission.activecount = txndata.Count();
            }
            else
            {
                admission.activecount = txndata.Count();


            }

            try
            {
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
                        admission.notes.ElementAt(i).type2 = "Re-Registration";
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
            }

            return PartialView("Hallticket", admission);

        }

        [HttpGet]
        public async Task<ActionResult> btnsubmit_Click(string regdno, string type, string semester, string tick)
        {
            Hallticket chart = new Hallticket();
            chart.type1 = type;
            chart.id1 = semester;
            chart.tick = tick;
            string imgurl = "";
            hallticketsummary admission = new hallticketsummary();
            //string regdno = Session["uid"].ToString();
            chart.course_code = Convert.ToString(Session["course_code"]);

            //await SaveImage(regdno);

            if (chart.type1.Equals("R"))
            {

                chart.type2 = "Regular";
            }
            else if (chart.type1.Equals("RE"))
            {
                chart.type2 = "Re-Registration";
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
            else if (chart.type1.Equals("MID"))
            {
                chart.type2 = "Mid Examination";
            }
            else
            {

                chart.type2 = chart.type1.ToUpper();
            }
            //System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
            //imgBarCode.Height = 150;
            //imgBarCode.Width = 150;
            //using (Bitmap bitMap = new Bitmap("E:\\STUDENT\\Students\\" + regdno + ".jpg"))
            //{
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //        byte[] byteImage = ms.ToArray();
            //        imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
            //    }
            //    if (imgBarCode.ImageUrl == null)
            //    {
            //        imgurl = "";

            //    }
            //    else
            //    {
            //        imgurl = imgBarCode.ImageUrl;
            //    }

            //}
            if (chart.course_code.Equals("PHD"))
            {
                chart.regdno = Session["uid"].ToString();
                chart.id1 = Session["Curr_sem"].ToString();
                Session["type1"] = chart.type1;
                Session["type2"] = chart.type2;
                Session["finalsem"] = semester;
                chart.id1 = semester;
                admission = await PHD_HALL_TICKET(chart);
                //admission.ImageUrl = imgurl;
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
                                    // admission.ImageUrl = imgurl;
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
                                    // admission.ImageUrl = imgurl;
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


                                admission = await NEW_HallTickets(chart);
                                // admission.ImageUrl = imgurl;
                                return View("NEW_HallTickets", admission);

                            }
                            else
                            {
                                String sem = Convert.ToString(student.ElementAt(0).CURRSEM);
                                chart.regdno = Session["uid"].ToString();
                                chart.CURRSEM = Session["Curr_sem"].ToString();
                                Session["type1"] = chart.type1;
                                Session["type2"] = chart.type2;
                                admission = await NEW_HallTickets(chart);
                                // admission.ImageUrl = imgurl;
                                return View("NEW_HallTickets", admission);
                            }
                        }
                    }

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
            //admission.ImageUrl = imgurl;
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

                chart.regdno = Session["regdno"].ToString();
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

                        if (stddetails1.Result.ElementAt(0).Passedout.Trim().Equals("Y") && chart.type1 == "R")//newly added this block
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
                        //chart.instructn = "FinalYeardiv";
                        chart.instructn = "C21";
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

                chart.regdno = Session["regdno"].ToString();
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


        [HttpGet]
        public async Task<JsonResult> getresource_additional(string group_code)
        {
            try
            {
                string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();
                IEnumerable<LmsResource> data = await iStudentRepository.Getresource_additionaldata(group_code, campus_code, COLLEGE_CODE, USER_ID, DEPT_CODE);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public async Task<JsonResult> gettypebysubject(string subcode, string subname, string groupcode)
        {
            IEnumerable<LMSsubject> lmssubject = null;
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string DEPT_CODE = Session["dept_code"].ToString(); string BRANCH_CODE = Session["branch_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString();

            string userid = Convert.ToString(Session["uid"]);
            lmssubject = await iStudentRepository.getlmssubjects(userid, Session["Curr_sem"].ToString(), campus_code, COLLEGE_CODE, DEPT_CODE, BRANCH_CODE, curr_sem);
            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            ViewBag.groupcode = groupcode;
            return Json(lmssubject, JsonRequestBehavior.AllowGet);

        }
        public async Task<ActionResult> getlmssubjects()
        {
            Lmsdirectorysummary sum = new Lmsdirectorysummary();

            IEnumerable<LMSsubject> lmssubject = null;
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string DEPT_CODE = Session["dept_code"].ToString(); string BRANCH_CODE = Session["branch_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString();

            string userid = Convert.ToString(Session["uid"]);
            sum.directorydata = await iStudentRepository.getlmssubjects(userid, Session["Curr_sem"].ToString(), campus_code, COLLEGE_CODE, DEPT_CODE, BRANCH_CODE, curr_sem);

            sum.data = new LMSsubject();
            List<LMSsubject> list = new List<LMSsubject>();
            foreach (var data in sum.directorydata.Select(m => new { m.subject_code, m.group_code }).Distinct())
            {
                string data1 = data.subject_code;
                string data2 = data.group_code;
                sum.data = new LMSsubject();
                if (data1 != "")
                {
                    sum.data = await iStudentRepository.getlmssubjectsmodeulepercentage(data1, data2);

                }
                if (sum.data != null && data1 != "")
                {
                    list.Add(new LMSsubject()
                    {
                        subject_code = data.subject_code,
                        group_code = data.group_code,
                        count = sum.data.count
                    });

                }

            }

            sum.reportscount = list;
            if (sum.data == null)
            {
                sum.data = new LMSsubject();

            }

            return View("Lmshome", sum);
        }
        public async Task<ActionResult> lmsgetdirectory(string subcode, string subname, string groupcode, string radiovalue)
        {
            Lmsdirectorysummary sum = new Lmsdirectorysummary();
            string userid = Convert.ToString(Session["uid"]);
            sum.directorydata = await iStudentRepository.getfilemasterfolders(groupcode);
            if (sum.directorydata.Count() == 0)
            {
                sum.directorydata = await iStudentRepository.insertfilemaster(groupcode);

            }

            sum.directoryfiles = await iStudentRepository.getfilemasterfoldersDATA(groupcode);

            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            ViewBag.radiovalue = radiovalue;
            return View("Lmsdirectoryview", sum);
        }

        //public async Task<ActionResult> lmsgetassignments(string subcode, string subname, string groupcode)
        //{
        //    Lmsdirectorysummary sum = new Lmsdirectorysummary();
        //    string userid = Convert.ToString(Session["uid"]);
        //    string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string DEPT_CODE = Session["dept_code"].ToString(); string BRANCH_CODE = Session["branch_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString();
        //    string course_code = Session["course_code"].ToString();
        //    sum.directorydata = await iStudentRepository.getassignmentmasterfolders(subcode, groupcode, Convert.ToString(Session["uid"]), campus_code, COLLEGE_CODE, DEPT_CODE, BRANCH_CODE, course_code);
        //    sum.directoryfiles = await iStudentRepository.getstudentassignments(Convert.ToString(Session["uid"]), groupcode);
        //    LMSsubject data = await iStudentRepository.getsubjectname(Convert.ToString(Session["uid"]), groupcode);
        //    ViewBag.subjectcode = subcode;
        //    ViewBag.subjectname = data.subject_name;
        //    ViewBag.groupcode = groupcode;
        //    return View("Lmsassignments", sum);
        //}


        public async Task<ActionResult> lmsgetassignments(string subcode, string subname, string groupcode)
        {
            Lmsdirectorysummary sum = new Lmsdirectorysummary();
            string userid = Convert.ToString(Session["uid"]);
            if (userid != "" && userid != null)
            {
                Session["assign_sub_code"] = subcode;
                Session["assign_sub_name"] = subname;
                Session["assign_group_code"] = groupcode;


                string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string DEPT_CODE = Session["dept_code"].ToString(); string BRANCH_CODE = Session["branch_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString();
                string course_code = Session["course_code"].ToString();
                sum.directorydata = await iStudentRepository.getassignmentmasterfolders(subcode, groupcode, Convert.ToString(Session["uid"]), campus_code, COLLEGE_CODE, DEPT_CODE, BRANCH_CODE, course_code);
                sum.directoryfiles = await iStudentRepository.getstudentassignments(Convert.ToString(Session["uid"]), groupcode);
                LMSsubject data = await iStudentRepository.getsubjectname(Convert.ToString(Session["uid"]), groupcode);

                sum.marksdata = new LMSsubject();
                List<LMSsubject> list = new List<LMSsubject>();
                foreach (var marksdata in sum.directorydata.Select(m => new { m.subject_code, m.group_code, m.assignmentid, m.final_marks, m.ASSIG_FLAG }).Distinct())
                {
                    string data1 = marksdata.subject_code;
                    string data2 = marksdata.group_code;
                    string data3 = marksdata.assignmentid;
                    string data4 = Convert.ToString(Session["uid"]);
                    sum.marksdata = new LMSsubject();
                    if (data3 != "")
                    {
                        sum.marksdata = await iStudentRepository.getassignmentmarksdisplay(data1, data2, data3, data4);

                    }
                    if (sum.marksdata != null && data1 != "")
                    {
                        list.Add(new LMSsubject()
                        {
                            subject_code = sum.marksdata.subject_code,
                            group_code = sum.marksdata.group_code,
                            final_marks = sum.marksdata.final_marks,
                            ASSIG_FLAG = sum.marksdata.ASSIG_FLAG
                        });

                    }

                }

                sum.markscount = list;
                if (sum.marksdata == null)
                {
                    sum.marksdata = new LMSsubject();

                }

                ViewBag.subjectcode = subcode;
                ViewBag.subjectname = data.subject_name;
                ViewBag.groupcode = groupcode;
                return View("Lmsassignments", sum);
            }
            else
            {
                return Redirect("https://login.gitam.edu/Login.aspx");
            }
        }


        public async Task<ActionResult> lmsgetanouncements(string subcode, string subname, string groupcode)
        {
            Lmsdirectorysummary sum = new Lmsdirectorysummary();
            string userid = Convert.ToString(Session["uid"]);
            sum.directorydata = await iStudentRepository.getstudentanouncments(groupcode, userid);


            // sum.directoryfiles = await iStudentRepository.getstudentassignments(Convert.ToString(Session["uid"]), groupcode);

            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            return View("Lmsannoncements", sum);
        }
        [HttpPost]
        public ActionResult getfiles(string type, string name)
        {


            string fileName = @"E:/CATS_PROJECTS/VSP/GIT/VSP#GIT#BTECH#CIVIL#2021-2022#5#A#CSEN1031/Course material" + name + ".pdf";
            //string fileName = @"E:/VSP/GIT/VSP#GIT#BTECH#CIVIL#2021-2022#5#A#CSEN1031/Course material/" + name + ".pdf";

            if (fileName != "")
            {
                bool fileExists = System.IO.File.Exists(fileName);
                if (fileExists)
                {
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64 = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);

                    return Content(base64);
                    //return File(pdfByteArray, "application/pdf");
                }
            }

            return Content("file not found");
        }


        [HttpGet]
        public ActionResult PdfGet_assign(string filename, string filetype, string group_code, string folderid, string assignmentid)
        {
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();
            var path = filename + filetype;
            //string[] pdfpub = PDFID.Split('?');
            string fileName = "";
            //fileName = @"C:\\" + groupcode + "\\\\" + foldername + "\\" + path + "";
            fileName = @"E:\\ASSIGNMENTS\\" + campus_code + "\\" + COLLEGE_CODE + "\\" + group_code + "\\" + assignmentid + "\\" + path + "";
            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }

            return Content("PDF file not found");
        }


        [HttpGet]
        public ActionResult PdfGet_announcement(string filename, string group_code, string folderid, string assignmentid)
        {
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();
            var path = filename;
            //string[] pdfpub = PDFID.Split('?');E:\CATS_PROJECTS\UPLOADS\announcement\VSP\GIT\1
            string fileName = "";
            //fileName = @"C:\\" + groupcode + "\\\\" + foldername + "\\" + path + "";
            fileName = "E:\\CATS_PROJECTS\\UPLOADS\\announcement\\" + campus_code + "\\" + COLLEGE_CODE + "\\" + folderid + "\\" + path;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }

            return Content("PDF file not found");
        }
        [HttpGet]
        public async Task<JsonResult> add_assidn_data(string folderid, string grp, string assignid)
        {
            try
            {
                Session["group_code_assign"] = grp;
                Session["assignmentid_assign"] = assignid;
                string regdno = Session["uid"].ToString();
                var Details = iStudentRepository.getassignmentmasterfoldersASSIGN(grp, assignid, folderid, regdno);
                //var Details = Json(Details);
                return Json(Details, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return null;
            }
        }




        [HttpPost, ValidateInput(false)]
        public async Task<JsonResult> AddassignmentINSERT(string assignid, string PName, string ResultComment)
        {
            LMSsubject sub = new LMSsubject();
            string Regdno = Session["REGDNO"].ToString();
            if (Regdno != "" && Regdno != null)
            {
                string college_code = Session["college_code"].ToString();
                string branch_code = Session["branch_code"].ToString();
                string campus_code = Session["campus_code"].ToString();
                string group_code = Session["group_code_assign"].ToString();
                string assignmentid = Session["assignmentid_assign"].ToString();
                string USER_ID = Session["uid"].ToString();
                if (ResultComment == "")
                {
                    string[] Pdf2 = new string[1000];
                    Pdf2 = PName.Split('\\');
                    string PDFfold = Pdf2[0];
                    string PDFfold1 = Pdf2[1];
                    string PDFfold2 = Pdf2[2];
                    string[] extn = new string[1000];
                    string filefull = System.IO.Path.GetFileNameWithoutExtension(PDFfold2);
                    string fileExtension = System.IO.Path.GetExtension(PDFfold2);
                    var path = "";
                    string fileName = "";
                    fileName = @"E:\\STUDENT\\ASSIGNMENTS\\" + USER_ID + "\\" + assignid + "\\" + PDFfold2 + "";
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (System.IO.File.Exists(fileName))
                        {
                            var result2 = await iStudentRepository.Insertassignuploaddata(filefull, Regdno, campus_code, college_code, assignid, fileExtension, group_code);
                            if (result2 >= 1)
                            {
                                sub.msg = "Successfully updated";
                            }
                            else
                            {
                                sub.msg = "Oops!!! something went wrong";
                            }
                        }
                        else
                        {
                            sub.msg = "Oops!!! something went wrong";
                        }
                    }
                }
                else if (PName == "")
                {
                    var result = await iStudentRepository.Insertassigneditordata(ResultComment, Regdno, campus_code, college_code, assignid);
                    if (result >= 1)
                    {
                        sub.msg = "Successfully updated";
                    }
                    else
                    {
                        sub.msg = "Oops!!! something went wrong";
                    }
                }
                else
                {
                    var path = "";
                    string fileName = "";


                    string[] Pdf2 = new string[1000];
                    Pdf2 = PName.Split('\\');
                    string PDFfold = Pdf2[0];
                    string PDFfold1 = Pdf2[1];
                    string PDFfold2 = Pdf2[2];
                    string[] extn = new string[1000];
                    string filefull = System.IO.Path.GetFileNameWithoutExtension(PDFfold2);
                    string fileExtension = System.IO.Path.GetExtension(PDFfold2);


                    fileName = @"E:\\STUDENT\\ASSIGNMENTS\\" + USER_ID + "\\" + assignid + "\\" + PDFfold2 + "";
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (System.IO.File.Exists(fileName))
                        {
                            var result = await iStudentRepository.Insertassigneditordata(ResultComment, Regdno, campus_code, college_code, assignid);
                            if (result >= 1)
                            {
                                var result2 = await iStudentRepository.Insertassignuploaddata(filefull, Regdno, campus_code, college_code, assignid, fileExtension, group_code);
                                if (result2 >= 1)
                                {
                                    sub.msg = "Successfully updated";
                                }
                                else
                                {
                                    sub.msg = "Oops!!! something went wrong";
                                }
                            }
                            else
                            {
                                sub.msg = "Oops!!! something went wrong";
                            }
                        }
                        else
                        {
                            sub.msg = "Oops!!! something went wrong";
                        }
                    }
                }
                return Json(sub.msg);
            }
            else
            {

                sub.msg = "session expired";
                return Json(sub.msg);
                //return Redirect("https://login.gitam.edu/Login.aspx");
            }
        }



        [HttpPost, ValidateInput(false)]
        public ActionResult Addassignmentupload(string Pdf)
        {
            LMSsubject summary = new LMSsubject();
            string userid = Convert.ToString(Session["uid"]);
            string Regdno = Session["REGDNO"].ToString();
            if (userid != "" && userid != null)
            {
                string college_code = Session["college_code"].ToString();
                string branch_code = Session["branch_code"].ToString();
                string campus_code = Session["campus_code"].ToString();
                string group_code = Session["group_code_assign"].ToString();
                string assignmentid = Session["assignmentid_assign"].ToString();



                if (Request.Files.Count > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string fname;
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                                //var path = @"E:\\ASSIGNMENTS\\" + campus_code + "\\" + college_code + "\\" + userid + "\\" + assignmentid + "\\";
                                var path = @"E:\\STUDENT\\ASSIGNMENTS\\" + userid + "\\" + assignmentid + "\\";

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                            }
                            else
                            {

                                string[] Pdf2 = new string[1000];
                                Pdf2 = Pdf.Split('\\');
                                string PDFfold = Pdf2[0];
                                string PDFfold1 = Pdf2[1];
                                string PDFfold2 = Pdf2[2];
                                fname = PDFfold2;
                                //var path = @"E:\\ASSIGNMENTS\\" + campus_code + "\\" + college_code + "\\" + userid + "\\" + assignmentid + "\\";
                                var path = @"E:\\STUDENT\\ASSIGNMENTS\\" + userid + "\\" + assignmentid + "\\";

                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                file.SaveAs(path + fname);
                            }

                        }
                        return Json("1");
                    }
                    catch (Exception ex)
                    {
                        return Json("Error occurred. Error details: " + ex.Message);
                    }
                }

                else
                {
                    return Json("0");
                }
            }
            else
            {
                return Json("0");
            }

        }




        public async Task<ActionResult> getCEtemplate(string subcode, string subname, string groupcode)
        {
            Lmsdirectorysummary sum = new Lmsdirectorysummary();
            try
            {
                string userid = Convert.ToString(Session["uid"]);

                if (userid != null)
                {
                    string campus_code = Session["campus_code"].ToString();
                    string sem = Session["Curr_sem"].ToString();
                    string year = "2023";
                    sum.Totalall = await iStudentRepository.getstudentCEList(Convert.ToString(Session["uid"]), subcode, year, campus_code, sem);

                    sum.Assignmentmarks = await iStudentRepository.getstudentAssignmentmarks(Convert.ToString(Session["uid"]), groupcode, subcode);
                    sum.Quizmarks = await iStudentRepository.getstudentsectionlist(subcode, year, campus_code, sem);
                    sum.finalmarks = await iStudentRepository.getassignmentmarksfinaldisplay(subcode, groupcode, Convert.ToString(Session["uid"]));

                    ViewBag.subjectcode = subcode;
                    ViewBag.subjectname = subname;
                    ViewBag.groupcode = groupcode;

                }
                else
                {
                    RedirectToAction("Index", "../loginpage");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("lmscetemplate", sum);
        }

        //public async Task<ActionResult> getCEtemplate(string subcode, string subname, string groupcode)
        //{
        //    Lmsdirectorysummary sum = new Lmsdirectorysummary();
        //    try
        //    {
        //        string userid = Convert.ToString(Session["uid"]);

        //        if (userid != null)
        //        {
        //            string campus_code = Session["campus_code"].ToString();
        //            string sem = Session["Curr_sem"].ToString();
        //            string year = "2023";
        //            sum.Totalall = await iStudentRepository.getstudentCEList(subcode, year, campus_code, sem);

        //            sum.Assignmentmarks = await iStudentRepository.getstudentAssignmentmarks(Convert.ToString(Session["uid"]), groupcode, subcode);
        //            sum.Quizmarks = await iStudentRepository.getstudentsectionlist(subcode, year, campus_code, sem);

        //            ViewBag.subjectcode = subcode;
        //            ViewBag.subjectname = subname;
        //            ViewBag.groupcode = groupcode;

        //        }
        //        else
        //        {
        //            RedirectToAction("Index", "../loginpage");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return View("lmscetemplate", sum);




        //}

        public async Task<ActionResult> getAssessments(string subcode, string subname, string groupcode)
        {
            //Lmsdirectorysummary sum = new Lmsdirectorysummary();
            //try
            //{

            //    IEnumerable<LMSsubject> lmssubject = null;
            //    string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string DEPT_CODE = Session["dept_code"].ToString(); string BRANCH_CODE = Session["branch_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString();

            //    string userid = Convert.ToString(Session["uid"]);
            //    sum.directorydata = await iStudentRepository.getlmssubjects(userid, Session["Curr_sem"].ToString(), campus_code, COLLEGE_CODE, DEPT_CODE, BRANCH_CODE, curr_sem);

            //    sum.data = new LMSsubject();
            //    List<LMSsubject> list = new List<LMSsubject>();
            //    foreach (var data in sum.directorydata.Select(m => new { m.subject_code, m.group_code }).Distinct())
            //    {
            //        string data1 = data.subject_code;
            //        string data2 = data.group_code;
            //        sum.data = new LMSsubject();
            //        if (data1 != "")
            //        {
            //            sum.data = await iStudentRepository.getlmssubjectsmodeulepercentage(data1, data2);

            //        }
            //        if (sum.data != null && data1 != "")
            //        {
            //            list.Add(new LMSsubject()
            //            {
            //                subject_code = data.subject_code,
            //                group_code = data.group_code,
            //                count = sum.data.count
            //            });

            //        }

            //    }

            //    sum.reportscount = list;
            //    if (sum.data == null)
            //    {
            //        sum.data = new LMSsubject();

            //    }


            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return View("LmsAssessments");
        }
        [HttpPost]
        public async Task<ActionResult> Deleteassignmntfile(string assignid, string regdno, string folderid, string fileid, string filetype, string filename, string group_code)
        {
            LMSsubject scholar = new LMSsubject();

            string campus_code = Session["campus_code"].ToString(); string college_code = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();

            //var path = @"E:\\ASSIGNMENTS\\" + campus_code + "\\" + college_code + "\\" + USER_ID + "\\" + assignid + "\\";
            var path = @"E:\\STUDENT\\ASSIGNMENTS\\" + USER_ID + " \\" + assignid + "\\";


            string fileName = path + filename + filetype;
            //string msg = "";
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (UnauthorizedAccessException)
                {
                    FileAttributes attributes = System.IO.File.GetAttributes(fileName);
                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes &= ~FileAttributes.ReadOnly;
                        System.IO.File.SetAttributes(fileName, attributes);
                        System.IO.File.Delete(fileName);
                    }
                    else
                    {
                        throw;
                    }
                }
                // System.IO.File.Delete(fileName);
            }

            var result = await iStudentRepository.Deleteassignmentfileupload(assignid, regdno, group_code, fileid, filename);
            if (result == 1)
            {
                scholar.msg = "Record deleted successfully";
            }
            else
            {
                scholar.msg = "Record deleted unsuccessfully";
            }

            // Return a success response
            return Json(scholar.msg, JsonRequestBehavior.AllowGet);
        }



        //[HttpGet]
        //public async Task<JsonResult> getresource_unit(string sub_code,string facultyid)
        //{
        //    try
        //    {
        //        IEnumerable<LmsResource> data = await iStudentRepository.Getresource_unitwisedata(sub_code, Session["campus_code"].ToString(),facultyid);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        [HttpGet]
        public async Task<JsonResult> getresource_unit(string sub_code, string facultyid)
        {
            try
            {
                string COLLEGE_CODE = Session["college_code"].ToString(); string curr_sem = Session["Curr_sem"].ToString(); string regdno = Session["uid"].ToString();
                IEnumerable<LmsResource> data = await iStudentRepository.Getresource_unitwisedata(sub_code, Session["campus_code"].ToString(), regdno, COLLEGE_CODE, curr_sem);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Getfolderpdf_img_mdl(string folder_id, string group_code, string folder_name, string year)
        {

            string curr_sem = Session["Curr_sem"].ToString(); string regdno = Session["uid"].ToString();

            Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");

            folder_name = folder_name.TrimEnd('.');

            string GalleryPath = Path.Combine(@"E:\Module", year, group_code, illegalInFileName.Replace(folder_name, ""));
            GalleryPath = @"\\?\" + GalleryPath;

            string GalleryImagePath;
            DirectoryInfo dir = new DirectoryInfo(GalleryPath);
            DataTable dtLoadGallery = new DataTable();
            dtLoadGallery.Columns.Add("GalleryImagePath");
            dtLoadGallery.Columns.Add("Img_Len");
            dtLoadGallery.Columns.Add("Date");
            dtLoadGallery.Columns.Add("access");
            dtLoadGallery.Columns.Add("Id");
            dtLoadGallery.Columns.Add("Generated_by");
            FileInfo[] file = null;
            if (dir.Exists)
            {
                file = dir.GetFiles();

                var i = 0;
                foreach (FileInfo image in file)
                {

                    string sLen = image.Length.ToString();
                    if (image.Length >= (1 << 30))
                        sLen = string.Format("{0}Gb", image.Length >> 30);
                    else
                        if (image.Length >= (1 << 20))
                        sLen = string.Format("{0}Mb", image.Length >> 20);
                    else
                        if (image.Length >= (1 << 10))
                        sLen = string.Format("{0}Kb", image.Length >> 10);
                    string name = Path.GetFileNameWithoutExtension(image.Name); //getting file name without extension  
                    GalleryImagePath = name + System.IO.Path.GetExtension(image.Name);
                    //var data1 = iStudentRepository.getcrseresrctrns1(folder_id, group_code, GalleryImagePath);
                    var data1 = iStudentRepository.getcrseresrctrns1(folder_id, group_code, GalleryImagePath, regdno, curr_sem);
                    if (data1 != null)
                    {
                        if (!string.IsNullOrEmpty(data1.id))
                        {
                            dtLoadGallery.Rows.Add(GalleryImagePath, sLen, Convert.ToDateTime(image.CreationTime).ToString("dd-MMM-yyyy HH:mm tt"), data1.action_name, data1.id, data1.postedby);
                            i++;
                        }
                    }


                }
            }

            var data = dtLoadGallery.Rows.OfType<DataRow>()
                 .Select(row => dtLoadGallery.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));

            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(data);
            return jsonData;

        }

        public string Getfolderpdf_imgrepo(string folder_id, string group_code, string folder_name)
        {
            //string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            //foreach (char c in invalidChars)
            //{
            //    folder_name = folder_name.Replace(c.ToString(), "");
            //}
            folder_name = folder_name.Replace(":", "");
            string GalleryPath = "E:\\" + group_code + "\\\\" + folder_name + "\\";
            string GalleryImagePath;
            DirectoryInfo dir = new DirectoryInfo(GalleryPath);
            DataTable dtLoadGallery = new DataTable();
            dtLoadGallery.Columns.Add("GalleryImagePath");
            dtLoadGallery.Columns.Add("Img_Len");
            dtLoadGallery.Columns.Add("Date");
            dtLoadGallery.Columns.Add("access");
            dtLoadGallery.Columns.Add("Id");
            dtLoadGallery.Columns.Add("Generated_by");
            FileInfo[] file = null;
            if (dir.Exists)
            {
                file = dir.GetFiles();

                var i = 0;
                foreach (FileInfo image in file)
                {

                    string sLen = image.Length.ToString();
                    if (image.Length >= (1 << 30))
                        sLen = string.Format("{0}Gb", image.Length >> 30);
                    else
                        if (image.Length >= (1 << 20))
                        sLen = string.Format("{0}Mb", image.Length >> 20);
                    else
                        if (image.Length >= (1 << 10))
                        sLen = string.Format("{0}Kb", image.Length >> 10);
                    string name = Path.GetFileNameWithoutExtension(image.Name); //getting file name without extension  
                    GalleryImagePath = name + System.IO.Path.GetExtension(image.Name);
                    var data1 = iStudentRepository.getcrseresrctrns2(folder_id, group_code, GalleryImagePath);
                    if (data1 != null)
                    {
                        if (!string.IsNullOrEmpty(data1.id))
                        {
                            dtLoadGallery.Rows.Add(GalleryImagePath, sLen, Convert.ToDateTime(image.CreationTime).ToString("dd-MMM-yyyy HH:mm tt"), data1.action_name, data1.id, data1.postedby);
                            i++;
                        }
                    }


                }
            }


            var data = dtLoadGallery.Rows.OfType<DataRow>()
                 .Select(row => dtLoadGallery.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(data);
            return jsonData;
        }
        public async Task<JsonResult> getcrseresrctrns(string folder_id, string group_code, string type)
        {
            try
            {

                var data1 = await iStudentRepository.getcrseresrctrns(folder_id, group_code);
                return Json(data1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Getmodulepdf_img(string folder_id, string group_code, string folder_name)
        {

            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();

            Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");
            folder_name = folder_name.TrimEnd('.');
            string GalleryPath = Path.Combine(@"E:\\", group_code.Split('#')[0], group_code.Split('#')[1], group_code, illegalInFileName.Replace(folder_name, ""));
            GalleryPath = @"\\?\" + GalleryPath;
            string GalleryImagePath;
            DirectoryInfo dir = new DirectoryInfo(GalleryPath);
            DataTable dtLoadGallery = new DataTable();
            dtLoadGallery.Columns.Add("GalleryImagePath");
            dtLoadGallery.Columns.Add("Img_Len");
            dtLoadGallery.Columns.Add("Date");
            dtLoadGallery.Columns.Add("Id");
            FileInfo[] file = null;
            if (dir.Exists)
            {
                file = dir.GetFiles();

                var i = 0;
                foreach (FileInfo image in file)
                {

                    string sLen = image.Length.ToString();
                    if (image.Length >= (1 << 30))
                        sLen = string.Format("{0}Gb", image.Length >> 30);
                    else
                        if (image.Length >= (1 << 20))
                        sLen = string.Format("{0}Mb", image.Length >> 20);
                    else
                        if (image.Length >= (1 << 10))
                        sLen = string.Format("{0}Kb", image.Length >> 10);
                    string name = Path.GetFileNameWithoutExtension(image.Name); //getting file name without extension  
                    GalleryImagePath = name + System.IO.Path.GetExtension(image.Name);
                    var id = iStudentRepository.getcrseresrctrns_additional(folder_id, group_code, GalleryImagePath);
                    if (id != null)
                    {
                        dtLoadGallery.Rows.Add(GalleryImagePath, sLen, Convert.ToDateTime(image.CreationTime).ToString("dd-MMM-yyyy HH:mm tt"), id);
                        i++;
                    }

                }
            }


            var data = dtLoadGallery.Rows.OfType<DataRow>()
                 .Select(row => dtLoadGallery.Columns.OfType<DataColumn>()
                     .ToDictionary(col => col.ColumnName, c => row[c]));
            var serializer = new JavaScriptSerializer();
            var jsonData = serializer.Serialize(data);
            return jsonData;
        }
        public async Task<JsonResult> getcrseresrcrepo(string group_code, string type)
        {
            try
            {
                IEnumerable<LmsResource> data1 = await iStudentRepository.getcrseresrcrepo(group_code, type);
                return Json(data1, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public JsonResult getcrseresrctrnsrepo(string folder_id, string group_code)
        {
            try
            {

                var id = iStudentRepository.getcrseresrctrnsrepo(folder_id, group_code);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult PdfGet1(string PDFID, string path, string folderid, string foldername, string year, string groupcode, string extension)
        {
            string[] pdfpub = PDFID.Split('?');
            string fileName = "";
            Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");
            foldername = foldername.TrimEnd('.');
            fileName = @"\\?\" + Path.Combine(@"E:\Module", year, groupcode, illegalInFileName.Replace(foldername, ""), path);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }

            return Content("PDF file not found");
        }

        [HttpGet]
        public ActionResult PdfGet_additional(string PDFID, string path, string foldername, string groupcode)
        {
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();

            string[] pdfpub = PDFID.Split('?');
            string fileName = "";

            //fileName = @"E:\\" + groupcode.Split('#')[0] + "\\" + groupcode.Split('#')[1] + "\\" + groupcode + "\\\\" + foldername + "\\" + path + "";
            Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");
            foldername = foldername.TrimEnd('.');
            fileName = @"\\?\" + Path.Combine(@"E:\", groupcode.Split('#')[0], groupcode.Split('#')[1], groupcode, illegalInFileName.Replace(foldername, ""), path);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }
            return Content("PDF file not found");
        }
        public string GetMimeType(string fileExtension)
        {
            string mimeType = "application/octet-stream"; // Default MIME type

            switch (fileExtension.ToLower())
            {
                case ".pdf":
                    mimeType = "application/pdf";
                    break;
                case ".doc":
                case ".docx":
                    mimeType = "application/msword";
                    break;

                case ".xls":
                    mimeType = "application/vnd.ms-excel";
                    break;
                case ".xlsx":
                    mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".jpeg":
                case ".jpg":
                    mimeType = "image/jpeg";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".ppt":
                case ".pptx":
                    mimeType = "application/vnd.ms-powerpoint";
                    break;
                case ".csv":
                    mimeType = "text/csv";
                    break;
                case ".webp":
                    mimeType = "image/webp";
                    break;
                case ".rar":
                    mimeType = "application/x-rar-compressed";
                    break;
                case ".ods":
                    mimeType = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case ".txt":
                    mimeType = "text/plain";
                    break;



                case ".mp4":
                    mimeType = "video/mp4";
                    break;
                case ".avi":
                    mimeType = "video/x-msvideo";
                    break;
                case ".wmv":
                    mimeType = "video/x-ms-wmv";
                    break;
                case ".mov":
                    mimeType = "video/quicktime";
                    break;
                case ".mkv":
                    mimeType = "video/x-matroska";
                    break;
                default:
                    break;
            }

            return mimeType;
        }

        [HttpGet]
        public ActionResult PdfGet_repo(string PDFID, string path, string foldername, string groupcode)
        {
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();

            string[] pdfpub = PDFID.Split('?');
            string fileName = "";
            //string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            //foreach (char c in invalidChars)
            //{
            //    foldername = foldername.Replace(c.ToString(), "");
            //}
            foldername = foldername.Replace(":", "");
            fileName = @"E:\\" + groupcode + "\\\\" + foldername + "\\" + path + "";
            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }

            return Content("PDF file not found");
        }



        public async Task<ActionResult> getTextbooks(string subcode, string subname, string groupcode, string radiovalue)
        {
            lmstextbooks data = new lmstextbooks();
            string campus = Session["campus_code"].ToString();

            // data.subcode = subcode; data.subname = subname; data.groupcode = groupcode; data.radiovalue = radiovalue;
            data.textbooksummary = await iStudentRepository.gettextbooks(subcode, campus);
            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            return View("LmsTextbooks", data);
        }
        public ActionResult lmsgetlessonplan(string subcode, string subname, string groupcode, string radiovalue)
        {
            LmsResource data = new LmsResource();
            data.subcode = subcode; data.subname = subname; data.groupcode = groupcode; data.radiovalue = radiovalue;
            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            return View("lmslessonplan", data);
        }
        [HttpGet]
        public async Task<JsonResult> getresource_unittopicwise(string sub_code, string module_id)
        {
            try
            {
                IEnumerable<LmsResource> data = await iStudentRepository.Getresource_topicwisedata(sub_code, module_id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpGet]
        public ActionResult PdfGetdownload_assign(string assignid, string regdno, string folderid, string fileid, string filetype, string filename, string group_code, string file_status)
        {
            string campus_code = Session["campus_code"].ToString(); string COLLEGE_CODE = Session["college_code"].ToString(); string USER_ID = Session["uid"].ToString(); string DEPT_CODE = Session["dept_code"].ToString();
            var path = filename + filetype;

            string fileName = "";
            fileName = @"E:\\STUDENT\\ASSIGNMENTS\\" + USER_ID + "\\" + assignid + "\\" + path + "";
            if (!string.IsNullOrEmpty(fileName))
            {
                if (System.IO.File.Exists(fileName))
                {
                    byte[] fileByteArray = System.IO.File.ReadAllBytes(fileName);
                    string base64Content = Convert.ToBase64String(fileByteArray, 0, fileByteArray.Length);
                    string fileExtension = System.IO.Path.GetExtension(fileName);
                    string fileType = GetMimeType(fileExtension);
                    //return Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    JsonResult result = Json(new { Base64Content = base64Content, FileType = fileType }, JsonRequestBehavior.AllowGet);
                    result.MaxJsonLength = int.MaxValue;
                    return result;
                }
            }

            return Content("PDF file not found");
        }
        public async Task<ActionResult> LmsResourceview(string subcode, string subname, string groupcode, string radiovalue, string faculty_id)
        {

            lmstextbooks data = new lmstextbooks();
            data.textbook = new LmsResource();
            string campus = Session["campus_code"].ToString();

            data.textbook.subcode = subcode; data.textbook.subname = subname; data.textbook.groupcode = groupcode; data.textbook.radiovalue = radiovalue; data.textbook.faculty_id = faculty_id;
            data.textbooksummary = await iStudentRepository.gettextbooks(subcode, campus);
            ViewBag.subjectcode = subcode;
            ViewBag.subjectname = subname;
            ViewBag.faculty_id = faculty_id;
            return View("LmsResourceview", data);
        }
        public ActionResult BellTheCats()
        {

            return View();
        }
        public async Task<ActionResult> Getgevents()
        {
            IEnumerable<Students> data = await iStudentRepository.eventsdecider(Session["Regdno"].ToString());

            return View("Gevents", data);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Response.Headers.Remove("Referrer-Policy");
            Response.AddHeader("Access-Control-Allow-Origin", "*");


            Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");


            Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");

            base.OnActionExecuting(filterContext);
        }

        protected override void OnException(ExceptionContext filterContext)
        {

            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "https://login.gitam.edu/Login.aspx");
            //filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Home/Error"
            };
        }




        [HttpGet]
        public async Task<ActionResult> PhotoUpload()
        {
            StudentPhoto data = new StudentPhoto();
            try
            {
                string userid = Convert.ToString(Session["uid"]);
                data = await iStudentRepository.getStudentapplication(userid);
                if (data == null)
                {

                    data = await iStudentRepository.getStudentmasterphoto(userid);
                    if (data != null)
                    {
                        data.MONTH = "April";
                        data.YEAR = "2024";
                        data.page_status = "0";

                    }
                    else
                    {
                        data = new StudentPhoto();


                    }

                }
                else
                {
                    data.page_status = "1";


                    if (System.IO.File.Exists("C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\" + userid + ".jpg"))
                    {

                        data.img_status = "1";

                    }
                    else
                    {
                        data.img_status = "0";
                    }



                    System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                    imgBarCode.Height = 150;
                    imgBarCode.Width = 150;
                    using (Bitmap bitMap = new Bitmap("C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\" + userid + ".jpg"))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            byte[] byteImage = ms.ToArray();
                            imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                        }
                        // plBarCode.Controls.Add(imgBarCode);
                        data.ImageUrl = imgBarCode.ImageUrl;
                    }

                }


            }
            catch (Exception ex)
            {

            }

            return PartialView("PhotoUpload", data);
        }





        //[HttpPost]
        //public async Task<ActionResult> CreateStudentPhoto(HttpPostedFileBase uploadForm, StudentPhoto summary)
        //{
        //    StudentPhoto data = null;
        //    try
        //    {


        //        string batch = Session["batch"].ToString();
        //        summary.batch = batch;
        //        //summary.date = DateTime.Now;


        //        string userid = Convert.ToString(Session["uid"]);
        //        data = await iStudentRepository.getStudentapplication(userid);

        //        if (data != null)
        //        {
        //            try
        //            {

        //                var update = await iStudentRepository.UpdateStudentapplication(summary);
        //                if (update.msg == "1")
        //                {
        //                    if (uploadForm != null)
        //                    {
        //                        if (uploadForm.FileName != "")
        //                        {



        //                            if (uploadForm.FileName != "")
        //                            {
        //                                String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\";
        //                                int fileSize = uploadForm.ContentLength;
        //                                String ext1 = uploadForm.FileName.Trim().Split('.')[1].ToString();


        //                                uploadForm.SaveAs(PATH4 + summary.REGDNO + ".jpg");


        //                            }

        //                            summary.msg = "Successfully Uploaded!";




        //                        }
        //                    }
        //                    summary.msg = "Successfully Uploaded!";
        //                }


        //            }
        //            catch (Exception ex)
        //            {

        //            }
        //        }
        //        else
        //        {

        //            var insert = await iStudentRepository.InsertStudentapplication(summary);
        //            if (insert.msg == "1")
        //            {

        //                String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\";
        //                int fileSize = uploadForm.ContentLength;
        //                String ext1 = uploadForm.FileName.Split('.')[1].ToString();

        //                uploadForm.SaveAs(PATH4 + summary.REGDNO + ".jpg");


        //            }

        //            summary.msg = "Successfully Uploaded!";

        //        }



        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Json(summary, JsonRequestBehavior.AllowGet);

        //}




        [HttpPost]
        public async Task<ActionResult> CreateStudentPhoto(HttpPostedFileBase uploadForm, StudentPhoto summary)
        {
            StudentPhoto data = null;
            try
            {


                string batch = Session["batch"].ToString();
                summary.batch = batch;
                //summary.date = DateTime.Now;


                string userid = Convert.ToString(Session["uid"]);
                data = await iStudentRepository.getStudentapplication(userid);






                if (data != null)
                {
                    try
                    {

                        var update = await iStudentRepository.UpdateStudentapplication(summary);
                        if (update.msg == "1")
                        {
                            if (uploadForm != null)
                            {
                                if (uploadForm.FileName != "")
                                {



                                    if (uploadForm.FileName != "")
                                    {
                                        String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\";
                                        int fileSize = uploadForm.ContentLength;
                                        String ext1 = uploadForm.FileName.Trim().Split('.')[1].ToString();


                                        uploadForm.SaveAs(PATH4 + summary.REGDNO + ".jpg");


                                    }

                                    summary.msg = "Successfully Uploaded!";




                                }
                            }
                            else
                            {

                                using (HttpClient httpClient = new HttpClient())
                                {
                                    try
                                    {
                                        var imageUrl = "https://doeresults.gitam.edu/photo/img.aspx?id=" + summary.REGDNO;
                                        // Download the image bytes
                                        byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                                        // Save the image to a local file in JPG format
                                        String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\" + summary.REGDNO + ".jpg";
                                        //string localFilePath = Server.MapPath(PATH4);
                                        System.IO.File.WriteAllBytes(PATH4, imageBytes);

                                        ViewBag.Message = "Image saved successfully!";
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        ViewBag.Message = $"Error downloading the image: {ex.Message}";
                                    }
                                }

                            }
                            summary.msg = "Successfully Uploaded!";
                        }


                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                {

                    var insert = await iStudentRepository.InsertStudentapplication(summary);
                    if (insert.msg == "1")
                    {

                        if (uploadForm != null)
                        {

                            String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\";
                            int fileSize = uploadForm.ContentLength;
                            String ext1 = uploadForm.FileName.Split('.')[1].ToString();

                            uploadForm.SaveAs(PATH4 + summary.REGDNO + ".jpg");
                        }
                        else
                        {
                            using (HttpClient httpClient = new HttpClient())
                            {
                                try
                                {
                                    var imageUrl = "https://doeresults.gitam.edu/photo/img.aspx?id=" + summary.REGDNO;
                                    // Download the image bytes
                                    byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

                                    // Save the image to a local file in JPG format
                                    String PATH4 = "C:\\CATS_PROJECTS\\FINAL_YEAR_2024\\" + summary.REGDNO + ".jpg";
                                    //string localFilePath = Server.MapPath(PATH4);
                                    System.IO.File.WriteAllBytes(PATH4, imageBytes);


                                }
                                catch (HttpRequestException ex)
                                {

                                }
                            }
                        }


                    }

                    summary.msg = "Successfully Uploaded!";

                }



            }
            catch (Exception ex)
            {

            }
            return Json(summary, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult WhatsNew()
        {

            return View();
        }
        public ActionResult studenthelpdesk()
        {

            return View();
        }


        public async Task<ActionResult> studenthelpdesknew()
        {
        
            Userlogin summary = null;

            summary = await iStudentRepository.get74user(Session["uid"].ToString());
            if (summary != null)
            {

           
               Session["helpdesk_user"] = summary.id;
                Session["helpdesk_password"] = summary.password;
      

            }
            return PartialView("studenthelpdesknew");
        }

        public async Task<bool> SaveImage(string regdno)
        {
            string imageUrl = "https://doeresults.gitam.edu/photo/img.aspx?id=" + regdno;
            string fileName = $"{regdno}.jpg";

            string saveLocation = @"E:\\STUDENT\\Students\\" + fileName;

            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
            WebResponse imageResponse = imageRequest.GetResponse();

            Stream responseStream = imageResponse.GetResponseStream();

            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(500000000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();

            FileStream fs = new FileStream(saveLocation, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(imageBytes);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
            return true;
        }


        [HttpPost]
        public async Task<ActionResult> DeleteImage(string regno)
        {
            string msg = "";
            try
            {
                string fName = $"{regno}.jpg";
                string fileName = @"E:\\STUDENT\\Students\\" + fName;

                //string msg = "";
                if (System.IO.File.Exists(fileName))
                {
                    System.IO.File.SetAttributes(fileName, FileAttributes.Normal);
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        FileAttributes attributes = System.IO.File.GetAttributes(fileName);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            attributes &= ~FileAttributes.ReadOnly;
                            System.IO.File.SetAttributes(fileName, attributes);
                            System.IO.File.Delete(fileName);
                        }
                        else
                        {
                            throw;
                        }
                    }
                    // System.IO.File.Delete(fileName);
                }



                msg = "Record deleted successfully";

            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }


            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> createfinalyearquestion(FormCollection collection)
        {
            dynamic showMessageString = string.Empty;
            try
            {
                List<FeedbackMaster> list = new List<FeedbackMaster>();
                FeedbackMaster datasingle = new FeedbackMaster();
                string[] count = collection.GetValues("count");


                //FeedbackMaster facultydata = await iStudentRepository.get_facultydept(Session["faculty"].ToString());
                string sem = Session["curr_sem"].ToString();
                string batch = Session["batchstart"].ToString();
                string sem1 = "";
                sem1 = sem;
                for (int i = 0; i <= (Convert.ToInt32(count[0]) - 1); i++)
                {

                    string[] feedbackradio = collection.GetValues("ctl00$MainContent$ID_" + i);
                    string[] feedbackqus = collection.GetValues("NATURE_" + i);
                    string[] feedbacksino = collection.GetValues("ID_" + i);
                    string[] feedbackmaxmarks = collection.GetValues("maxmarks_" + i);
                    string[] Q_STATUS = collection.GetValues("Q_STATUS_" + i);
                    list.Add(new FeedbackMaster
                    {
                        FEEDBACK1 = feedbackradio[0],
                        NATURE = feedbacksino[0],
                        ID = feedbackqus[0],
                        maxmarks = feedbackmaxmarks[0],
                        regdno = Session["uid"].ToString(),
                        semester = sem1,
                        stuname = Session["studname"].ToString(),
                        college_code = Session["college_code"].ToString(),
                        section = Session["section"].ToString(),
                        batch = Session["batch"].ToString(),
                        branch_code = Session["branch_code"].ToString(),
                        campus_code = Session["campus_code"].ToString(),
                        feedbacksession = "1",
                        course_code = Session["course_code"].ToString(),
                        Q_STATUS = Q_STATUS[0],

                    });
                }

                var result = await iStudentRepository.InsertGITAM_STUDENT_OUTGOINGfeedbacknew(list);
                if (result.FirstOrDefault().flag == "success")
                {
                    showMessageString = new
                    {
                        param1 = 100,
                        param2 = "Successfully submitted",
                    };

                }
                else
                {
                    showMessageString = new
                    {
                        param1 = 100,
                        param2 = "oops,something went wrong",
                    };
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public async Task<JsonResult> moodlequizjson(string mid)
        {

            string firstUrl = "https://apiserver.gitam.edu/moodle/login.php?username=" + Session["REGDNO"] + "&mid=" + mid;


            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(firstUrl);
            string secondUrlNode = doc.ParsedText;
            //string subst = secondUrlNode.Split('=')[1];

            ViewBag.url = secondUrlNode;
            // return PartialView("moodlequiz");

            return Json(secondUrlNode, JsonRequestBehavior.AllowGet);
        }

        //public async Task<ActionResult> Mac_details()
        //{
        //    Mac_details data = new Mac_details();
        //    data.Regdno = Session["uid"].ToString(); data.Name = Session["name"].ToString();
        //    data = await iStudentRepository.getmac_details(data.Regdno);
        //    if (data == null)
        //    {
        //        data = new Mac_details();
        //    }
        //    data.Regdno = Session["uid"].ToString(); data.Name = Session["name"].ToString();
        //    Session["otp_mac"] = "";
        //    return View("Mac_details", data);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Createmac_details(Mac_details summary)
        //{
        //    try
        //    {
        //        summary.Regdno = Session["uid"].ToString(); summary.Name = Session["name"].ToString(); summary.Campus = Session["campus_code"].ToString(); summary.Gender = Session["gender"].ToString();
        //        // string otp_mac = Session["otp_mac"].ToString();
        //        //if (otp_mac == summary.otp)
        //        //{
        //        var outcome = "";
        //        string id1 = "";
        //        string id2 = "";
        //        for (int i = 0; i < 2; i++)
        //        {
        //            string endpointName = "Allowed-List";
        //            string macAddress = "";
        //            var url = "https://172.22.12.45/ers/config/endpoint";
        //            var username = "ERSAdmin";
        //            var password = "pd2_wZpL_s3d";
        //            string description = "test endpoint";
        //            string groupid = "5da9fe60-f991-11ee-a229-624ce54b8381";
        //            bool staticProfileAssignment = true;
        //            string attrStr = "aaa";
        //            string attrInt = "111";
        //            if (i == 0) { endpointName = summary.MAC1; macAddress = summary.MAC1; }
        //            if (i == 1) { macAddress = summary.MAC2; macAddress = summary.MAC2; }

        //            var payload = $@"{{
        //    ""ERSEndPoint"": {{
        //        ""name"": ""{endpointName}"",
        //        ""description"": ""{description}"",
        //        ""mac"": ""{macAddress}"",
        //        ""staticProfileAssignment"": {staticProfileAssignment.ToString().ToLower()},
        //        ""staticGroupAssignment"": {staticProfileAssignment.ToString().ToLower()},
        //            ""groupId"": ""{groupid}"",
        //        ""customAttributes"": {{
        //            ""customAttributes"": {{
        //                ""attr_str"": ""{attrStr}"",
        //                ""attr_int"": ""{attrInt}""
        //            }}
        //        }}
        //    }}
        //}}";
        //            HttpClientHandler handler = new HttpClientHandler();
        //            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

        //            using (var client = new HttpClient(handler))
        //            {
        //                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        //                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        //                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                var content = new StringContent(payload, Encoding.UTF8, "application/json");

        //                var response = client.PostAsync(url, content).GetAwaiter().GetResult();

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //                    Console.WriteLine("Success!");
        //                    Console.WriteLine(responseContent);
        //                    outcome = responseContent;

        //                }
        //                else
        //                {
        //                    var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); ;
        //                    Console.WriteLine("Error:");
        //                    Console.WriteLine(responseContent);
        //                    outcome = responseContent;
        //                }
        //            }
        //            HttpClientHandler handler1 = new HttpClientHandler();
        //            handler1.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
        //            using (var client1 = new HttpClient(handler1))
        //            {
        //                var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
        //                client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        //                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                var content = new StringContent(payload, Encoding.UTF8, "application/json");


        //                var response1 = client1.GetAsync("https://172.22.12.45/ers/config/endpoint/name/" + macAddress).GetAwaiter().GetResult();
        //                if (response1.IsSuccessStatusCode)
        //                {
        //                    var responseContent1 = response1.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //                    Response1 finres = JsonConvert.DeserializeObject<Response1>(responseContent1.ToString());
        //                    if (i == 0) { id1 = finres.ERSEndPoint.id; }
        //                    if (i == 1) { id2 = finres.ERSEndPoint.id; }
        //                }

        //            }

        //        }

        //        if (summary.id == 0 && Response.StatusCode == 200)
        //        {
        //            var result = await iStudentRepository.InsertMac_details(summary, id1, id2);
        //            if (result == 1)
        //            {
        //                summary.msg = "Successfully submitted";
        //            }
        //            else
        //            {
        //                summary.msg = "Oops!!! something went wrong";
        //            }
        //        }
        //        else
        //        {
        //            var result1 = await iStudentRepository.updateMac_details(summary);
        //            if (result1 == 1)
        //            {
        //                summary.msg = "Successfully updated";
        //            }
        //            else
        //            {
        //                summary.msg = "Oops!!! something went wrong";
        //            }
        //        }

        //        //}
        //        //else
        //        //{
        //        //    summary.msg = "Please enter valid otp";
        //        //}


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Json(summary.msg);
        //}
        public ActionResult getotp_mac()
        {
            string msg = "";
            try
            {

                String smsuser = "glearn", message = null;
                String smspassword = "glearn@gsms@123$";
                //   Session["mobile"] = "8121647490";
                //  string mobile = Session["mobile"].ToString();
                string mobile = "6281621459";
                string numbers = "1234567890";
                string characters = numbers;
                int length = 5;
                string otp = string.Empty;

                for (int i = 0; i < length; i++)
                {
                    string character = string.Empty;
                    do
                    {
                        int index = new Random().Next(0, characters.Length);
                        character = characters.ToCharArray()[index].ToString();
                    } while (otp.IndexOf(character) != -1);
                    otp += character;
                }

                if (Session["mobile"].ToString().Length < 10)
                {
                    msg = "Please update your mobile number in the View and update profile page";
                }
                else
                {
                    ViewBag.Mobile = Session["mobile"].ToString();
                    ViewBag.Otp_mac = otp;
                    Session["otp_mac"] = otp;
                    message = "Dear student,Your OTP for gitam.in mac update is : " + otp + "";

                    SendSMS(smsuser, smspassword, mobile, message);
                    msg = "successfull";
                }
                return Json(new { otp, msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //public async Task<ActionResult> macdetailscheck(string macid)
        //{
        //    try
        //    {
        //        string Regdno = Session["uid"].ToString();
        //        int msg = await iStudentRepository.getmacdetails_check(macid, Regdno);
        //        return Json(msg, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<ActionResult> DailyLog()
        {
            IEnumerable<Students> data = null;
            try
            {
                string Regdnos = Session["uid"].ToString();
                data = await iStudentRepository.getdailylog(Regdnos);
            }
            catch (Exception e)
            {
                throw e;
            }
            return View("DailyLog", data);
        }
        [HttpPost]
        public async Task<JsonResult> SubmittingDailyLog(string Arrivaltimedata, string departtimedata, string orgnamedata, string dptnamedata, string mtrnamedata, string lernactiondata)
        {
            if (Session["uid"] == null)
            {
                return Json(new { success = false, message = "Session variables are not set." });
            }
            else
            {
                var uid = Session["uid"].ToString();
                //var id = Session["id"].ToString();
                var semester = Session["Curr_sem"].ToString();
                var collegeCode = Session["college_code"].ToString();
                var branchCode = Session["branch_code"].ToString();
                var campusCode = Session["campus_code"].ToString();
                // var dtTime = Session["dt_time"].ToString();

                try
                {
                    //DateTime arrivalTime = DateTime.Parse(Arrivaltimedata);
                    //DateTime departTime = DateTime.Parse(departtimedata);

                    var result = await iStudentRepository.SaveDailyLog(Arrivaltimedata, departtimedata, orgnamedata, dptnamedata, mtrnamedata, lernactiondata, uid, semester, collegeCode, branchCode, campusCode);

                    return Json(new { success = true, message = "Data inserted successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while inserting the data." });
                }

            }

        }
        public async Task<ActionResult> Weeklyactivitylog()
        {
            IEnumerable<Students> data = null;
            try
            {
                string Regdnos = Session["uid"].ToString();
                data = await iStudentRepository.getweeklyactivities(Regdnos);
            }
            catch (Exception ex)
            {

            }
            return View("Weeklyactivitylog", data);

        }

        [HttpPost]
        public async Task<JsonResult> submittingweeklyactivitylog(string cmpnamedata, string cmpmentordata, string stfguidedata, string actperformeddata, string keyprfindicatordata, string challengesdata, string achievementsdata, string learningpointsdata)
        {
            if (Session["uid"] == null)
            {
                return Json(new { success = false, message = "Session variables are not set." });
            }
            else
            {
                var uid = Session["uid"].ToString();
                //var id = Session["id"].ToString();
                var semester = Session["Curr_sem"].ToString();
                var collegeCode = Session["college_code"].ToString();
                var branchCode = Session["branch_code"].ToString();
                var campusCode = Session["campus_code"].ToString();
                // var dtTime = Session["dt_time"].ToString();

                try
                {

                    var result = await iStudentRepository.Insertweeklyactivity(cmpnamedata, cmpmentordata, stfguidedata, actperformeddata, keyprfindicatordata, challengesdata, achievementsdata, learningpointsdata, uid, semester, collegeCode, branchCode, campusCode);

                    return Json(new { success = true, message = "Data inserted successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while inserting the data." });
                }

            }

        }
        public ActionResult summerintprjct()
        {

            return View();
        }

        public async Task<ActionResult> fldwrkprpsl()
        {
            IEnumerable<Students> data = null;
            try
            {
                string Regdnos = Session["uid"].ToString();
                data = await iStudentRepository.getfieldworkproposal(Regdnos);
            }
            catch (Exception e)
            {
                throw e;
            }
            return View("fldwrkprpsl", data);
        }
        [HttpPost]
        public async Task<JsonResult> submittingfieldworkproposal(string fwtitledata, string rprtdatedata, string rwdomaindata, string rwksubdomainddata, string rpmbackgrounddata, string prbdefinitiondata, string stdmethoddata, string expoutcomedata)
        {
            if (Session["uid"] == null)
            {
                return Json(new { success = false, message = "Session variables are not set." });
            }
            else
            {
                var uid = Session["uid"].ToString();
                //var id = Session["id"].ToString();
                var semester = Session["Curr_sem"].ToString();
                var collegeCode = Session["college_code"].ToString();
                var branchCode = Session["branch_code"].ToString();
                var campusCode = Session["campus_code"].ToString();
                // var dtTime = Session["dt_time"].ToString();

                try
                {

                    var result = await iStudentRepository.Insertfieldworkproposal(fwtitledata, rprtdatedata, rwdomaindata, rwksubdomainddata, rpmbackgrounddata, prbdefinitiondata, stdmethoddata, expoutcomedata, uid, semester, collegeCode, branchCode, campusCode);

                    return Json(new { success = true, message = "Data inserted successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while inserting the data." });
                }
            }

        }
        public async Task<ActionResult> GetAttendanceSummerTerm()
        {
            Attendancesummary summary = new Attendancesummary();

            string college_code = Session["college_code"].ToString();
            string table_name = Session["ATTENDANCEREPORTTABLE"].ToString();
            string cur_sem = Session["Curr_sem"].ToString();
            string batch = Session["batch"].ToString();
            string[] batch1 = new string[1000];
            batch1 = batch.Split('-');
            string batch2 = batch1[0];
            Session["batchattd"] = batch2 + "-" + (Convert.ToInt32(batch2) + 1);
            string batchattd = Session["batchattd"].ToString();
            string user_id = Session["uid"].ToString();

            string campus = Session["campus_code"].ToString();
            string course_code = Session["course_code"].ToString();

            summary.notes = await iStudentRepository.Getattendance_semsterissummerterm(user_id, table_name, college_code, batchattd);
            summary.notessecond = await iStudentRepository.Getattendance_semster_byselectionissummerterm(user_id, table_name, college_code, batchattd);

            return PartialView("GetAttendancedataforsummerterm", summary);
        }
        //public async Task<ActionResult> Projectslist()
        //{
        //    Softwareprojectsummary summary = new Softwareprojectsummary();
        //    string Regdno = Session["REGDNO"].ToString();
        //    if (Regdno == "")
        //    {
        //        RedirectToAction("https://login.gitam.edu");
        //        throw new Exception();
        //    }
        //    else
        //    {
        //        summary.notes = await iStudentRepository.selectedprojects(Regdno);
        //        Session["project_count"] = summary.notes.FirstOrDefault().count;
        //        return PartialView("Projectslist", summary);
        //    }

        //}
        //public async Task<ActionResult> Getsoftwareprojectslist()
        //{

        //    string campus = Session["campus_code"].ToString();
        //    IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getsoftwareprojectslist_async(campus);
        //    return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        //}

        //public async Task<ActionResult> Getdomainslist_multi()
        //{

        //    string campus = Session["campus_code"].ToString();
        //    IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getprojectdomain(campus);
        //    return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public async Task<ActionResult> GetProjectsList(string selectedValues)
        //{

        //    var selectedValuesArray = JsonConvert.DeserializeObject<string[]>(selectedValues);
        //    string campus = Session["campus_code"].ToString();
        //    IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getprojectslist(campus, selectedValues);
        //    return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        //}
        //public async Task<ActionResult> btninsert(string selectedValues)
        //{
        //    string Regdno = Session["REGDNO"].ToString();
        //    string stuname = Session["studname"].ToString();
        //    string campus = Session["campus_code"].ToString();
        //    string college = Session["college_code"].ToString();
        //    string mobile = Session["mobile"].ToString();
        //    string dept = Session["dept_code"].ToString();
        //    string msg = "";
        //    string[] selectedValuesArray;
        //    if (Regdno == "")
        //    {
        //        RedirectToAction("https://login.gitam.edu");
        //        throw new Exception();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            System.Diagnostics.Debug.WriteLine("selectedValues: " + selectedValues);
        //            selectedValuesArray = selectedValues.Split(',')
        //                                                 .Select(value => value.Trim())
        //                                                 .ToArray();
        //            if (selectedValuesArray.Length > 0)
        //            {
        //                var result = await iStudentRepository.insertprojectselection(Regdno, stuname, campus, college, mobile, dept, selectedValues);
        //                if (result == 3)
        //                {
        //                    msg = "Successfully updated";
        //                }
        //                else if (result == 2)
        //                {
        //                    msg = "Already selected";
        //                }
        //                else if (result == 1)
        //                {
        //                    msg = "Max Exceeds";
        //                }
        //                else
        //                {
        //                    msg = "Already selection process completed";
        //                }
        //            }
        //            else
        //            {
        //                msg = "NO data";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return Json(msg, JsonRequestBehavior.AllowGet);
        //}
        //public async Task<ActionResult> Getselectedsoftwareprojectslist()
        //{
        //    string Regdno = Session["REGDNO"].ToString();
        //    IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getselectedsoftwareprojectslist_async(Regdno);
        //    return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        //}
        public async Task<ActionResult> Projectslist()

        {
            Softwareprojectsummary summary = new Softwareprojectsummary();
            string Regdno = Session["REGDNO"].ToString();
            if (Regdno == "")
            {
                RedirectToAction("https://login.gitam.edu");
                throw new Exception();
            }
            else
            {
                summary.notes = await iStudentRepository.selectedprojects(Regdno);
                Session["project_count"] = summary.notes.FirstOrDefault().count;
                return PartialView("Projectslist", summary);
            }

        }
        public async Task<ActionResult> Getsoftwareprojectslist()
        {

            string campus = Session["campus_code"].ToString();
            IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getsoftwareprojectslist_async(campus);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Getdomainslist_multi()
        {

            string campus = Session["campus_code"].ToString();
            IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getprojectdomain(campus);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetProjectsList(string selectedValues)
        {

            var selectedValuesArray = JsonConvert.DeserializeObject<string[]>(selectedValues);
            string campus = Session["campus_code"].ToString();
            IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getprojectslist(campus, selectedValues);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> btninsert(string selectedValues)
        {
            string Regdno = Session["REGDNO"].ToString();
            string stuname = Session["studname"].ToString();
            string campus = Session["campus_code"].ToString();
            string college = Session["college_code"].ToString();
            string mobile = Session["mobile"].ToString();
            string dept = Session["dept_code"].ToString();
            string msg = "";
            string[] selectedValuesArray;
            if (Regdno == "")
            {
                RedirectToAction("https://login.gitam.edu");
                throw new Exception();
            }
            else
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("selectedValues: " + selectedValues);
                    selectedValuesArray = selectedValues.Split(',')
                                                         .Select(value => value.Trim())
                                                         .ToArray();
                    if (selectedValuesArray.Length > 0)
                    {
                        var result = await iStudentRepository.insertprojectselection(Regdno, stuname, campus, college, mobile, dept, selectedValues);
                        if (result == 3)
                        {
                            msg = "Successfully updated";
                        }
                        else if (result == 2)
                        {
                            msg = "Already selected";
                        }
                        else if (result == 1)
                        {
                            msg = "Max Exceeds";
                        }
                        else
                        {
                            msg = "Already selection process completed";
                        }
                    }
                    else
                    {
                        msg = "NO data";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Getselectedsoftwareprojectslist()
        {
            string Regdno = Session["REGDNO"].ToString();
            IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getselectedsoftwareprojectslist_async(Regdno);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> GetProjectsListchange(string domain)
        {
            string campus = Session["campus_code"].ToString();
            IEnumerable<Softwareproject> mentor_remarksdata = await iStudentRepository.Getprojectslistchange(campus, domain);
            return Json(mentor_remarksdata, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> btnchangeproject(string selectedValues, string selectedtitleVal, string old_title_id, string oldtitle, string oldcount, string domain)
        {
            string Regdno = Session["REGDNO"].ToString();
            string stuname = Session["studname"].ToString();
            string campus = Session["campus_code"].ToString();
            string college = Session["college_code"].ToString();
            string mobile = Session["mobile"].ToString();
            string dept = Session["dept_code"].ToString();
            string msg = "";
            string[] selectedValuesArray;
            if (Regdno == "")
            {
                RedirectToAction("https://login.gitam.edu");
                throw new Exception();
            }
            else
            {
                try
                {
                    if (selectedValues.Length > 0)
                    {
                        var result = await iStudentRepository.insertprojectchange(Regdno, stuname, campus, college, mobile, dept, selectedValues, old_title_id, oldtitle, oldcount, domain, selectedtitleVal);
                        if (result == 1)
                        {
                            msg = "Successfully updated";
                        }
                        else if (result == 4)
                        {
                            msg = "Already selected";
                        }
                        else if (result == 3)
                        {
                            msg = "Max Exceeds";
                        }
                        else
                        {
                            msg = "Already selection process completed";
                        }
                    }
                    else
                    {
                        msg = "NO data";
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Mac_details()
        {
            Mac_details data = new Mac_details();
            data.Regdno = Session["uid"].ToString(); data.Name = Session["name"].ToString();
            data = await iStudentRepository.getmac_details(data.Regdno);
            if (data == null)
            {
                data = new Mac_details();
            }
            data.Regdno = Session["uid"].ToString(); data.Name = Session["name"].ToString();
            Session["otp_mac"] = "";
            return View("Mac_details", data);
        }
        public async Task<ActionResult> EnableEdit(string regdno)
        {
            Mac_details data = new Mac_details();
            // var model = GetStudentByRegdno(regdno); // Fetch the student data by registration number
            data = await iStudentRepository.getmac_details(Session["uid"].ToString());
            ViewBag.IsEditMode = true; // Set edit mode to true
            return PartialView("_MACDetailsEdit", data); // Return the partial view for editing MAC details
        }

        [HttpPost]
        public async Task<JsonResult> UpdateMacs(string MAC11, string MAC22)
        {
            Mac_details summary = new Mac_details();

            summary.Regdno = Session["uid"].ToString();
            summary.Name = Session["name"].ToString();
            summary.Campus = Session["campus_code"].ToString();
            summary.Gender = Session["gender"].ToString();
            int result = 0;
            var outcome = "";
            MAC11 = MAC11?.ToLower();
            MAC22 = MAC22?.ToLower();
            // Get the IP address
            string IPAddress = GetIP();
            if (summary.id == 0 && Response.StatusCode == 200)
            {
                result = await iStudentRepository.updatemac_details(summary, MAC11, MAC22, IPAddress);

                if (result == 1)
                {
                    summary.msg = "success";
                }
                else
                {
                    summary.msg = "Oops!!! something went wrong";
                }
            }
            if (result == 1)
            {
                return Json(summary.msg);
            }
            return Json(new { success = false });
        }


        public async Task<ActionResult> Createmac_details(Mac_details summary)
        {
            try
            {
                summary.Regdno = Session["uid"].ToString();
                summary.Name = Session["name"].ToString();
                summary.Campus = Session["campus_code"].ToString();
                summary.Gender = Session["gender"].ToString();
                summary.MAC1 = summary.MAC1?.ToLower();
                summary.MAC2 = summary.MAC2?.ToLower();
                var outcome = "";
                string id1 = "";
                string id2 = "";
                // Get the IP address
                string IPAddress = GetIP();
                if (summary.id == 0 && Response.StatusCode == 200)
                {
                    var result = await iStudentRepository.InsertMac_details(summary, id1, id2, IPAddress);
                    var newResult = await iStudentRepository.InsertNewData_MYSQL(summary, id1, id2, IPAddress);
                    if (newResult != 0)
                    {
                        summary.msg = "Successfully submitted";
                    }
                    else
                    {
                        summary.msg = "Oops!!! something went wrong";
                    }
                }
                else
                {
                    var result1 = await iStudentRepository.updateMac_details(summary, IPAddress);
                    var newResult1 = await iStudentRepository.UpdateNewData_MYSQL(summary, IPAddress);
                    if (result1 == 1)
                    {
                        summary.msg = "Successfully updated";
                    }
                    else
                    {
                        summary.msg = "Oops!!! something went wrong";
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(summary.msg);
        }

        private string GetIP() //for ip
        {
            string localIP = string.Empty;

            // Get the host name of the machine
            string hostName = Dns.GetHostName();

            // Get all IP addresses associated with the machine
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            // Loop through the IP addresses and find the IPv4 address
            foreach (var ip in hostEntry.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) // IPv4 address
                {
                    localIP = ip.ToString();
                    //break;
                }
            }
            return localIP;
        }



        public async Task<ActionResult> macdetailscheck(string macid)
        {
            try
            {
                // Remove ':' and '-' from the input MAC address
                string cleanedMacId = macid.Replace(":", "").Replace("-", "");

                // Get the registration number from the session
                string Regdno = Session["uid"].ToString();

                // Send the cleaned MAC ID to the repository for comparison
                int msg = await iStudentRepository.getmacdetails_check(cleanedMacId, Regdno);

                // Return the result as JSON
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }

}