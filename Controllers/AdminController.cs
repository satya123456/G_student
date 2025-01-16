using Gstudent.Models;
using Gstudent.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Gstudent.Controllers
{
    public class AdminController : Controller
    {
        // ADMIN 
        private readonly Iloginrepository loginrepository;

        String userid = null, type = null, code = null;
        String ip; int id;
        public AdminController(Iloginrepository _Iloginrepository)
        {
            this.loginrepository = _Iloginrepository;

        }
        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Loginuser(FormCollection collection)
        {

            Session["user_type"] = "";
            string[] user_id = collection.GetValues("user_id");
            string[] pwd = collection.GetValues("password");
          
            Userlogin cm1 = await loginrepository.getbatchdetails(user_id[0]);
            if (cm1 != null)
            {
                Session["Timetablebatch"] = "Y";
            }
            else
            {
                Session["Timetablebatch"] = "N";
            }

            Userlogin cm = await loginrepository.getloginbypass(user_id[0], pwd[0]);
            var photo = await loginrepository.getStudentmasterphoto(user_id[0]);
            if (photo != null)
            {
                if (photo.REGDNO != "0")
                {
                    Session["photoupload"] = "Y";
                }
                else
                {
                    Session["photoupload"] = "N";
                }

            }
            else
            {
                Session["photoupload"] = "N";

            }
            if (cm != null)
            {
                Session["val"] = id;
                Session["APPLICATION_NO"] = cm.APPLICATION_NO;
                Session["PREFERENCE_COURSE_1"] = cm.PREFERENCE_COURSE_1;
                Session["PREFERENCE_COURSE_5"] = cm.PREFERENCE_COURSE_5;
                Session["Courseregistrationopen"] = cm.sem15_sgpa;
                Session["Courseregistrationopentxn_flag"] = cm.txn_flag;
                Session["STUDENTstatus"] = cm.status;
                Session["row"] = cm.row;
                Session["uid"] = cm.regdno;
                Session["REGDNO"] = cm.regdno;
                Session["pwd"] = cm.password;
                Session["college_code"] = cm.college_code;
                Session["campus_code"] = cm.campus;
                Session["gender"] = cm.gender;
                Session["degree_code"] = cm.degree_code;
                Session["branch_code"] = cm.branch_code;
                Session["batch"] = cm.batch;
                Session["section"] = cm.section;
                Session["course_code"] = cm.course_code;
                Session["year_of_study"] = cm.ACADEMIC_YEAR_FROM;
                Session["studname"] = cm.name;
                Session["name"] = cm.name;
                Session["CLASS"] = cm.CLASS;
                Session["Curr_sem"] = cm.curr_sem;
                Session["fbflag"] = cm.feedback_flag;
                Session["EMAILID"] = cm.emailid;
                Session["mobile"] = cm.mobile;
                Session["user"] = type;
                Session["dept_code"] = cm.dept_code;
                Session["dept"] = cm.dept_code;
                Session["hostel_demand"] = cm.hostel_demand;
                Session["hostler"] = cm.hostler;
                Session["HOSTEL_BLOCK"] = cm.HOSTEL_BLOCK;
                Session["SF"] = cm.SF;
                Session["fine"] = cm.fine;
                Session["online"] = cm.cgpa;
                Session["PREREQ"] = cm.PREREQ;
                Session["cdlflag"] = cm.feedback;
                Session["parentmobile"] = cm.parent_mobile;
                Session["APPlno"] = cm.PINNO;
                Session["AdmissionDate"] = cm.DATE_OF_ADMISSION;
                Session["sem13_sgpa"] = cm.sem13_sgpa;
                string batch = Session["batch"].ToString();
                string[] batch1 = new string[1000];
                batch1 = batch.Split('-');
                Session["batchstart"] = batch1[0];
                string batch2 = batch1[1];
                Session["batchend"] = batch2;
                Session["capstone_flag"] = cm.capstone_flag;
                Session["Father_Name"] = cm.FATHER_NAME;
                Session["dob"] = cm.dob;
                Session["status"] = cm.status;
                string emailid = Convert.ToString(Session["EMAILID"]);
                string phone = Convert.ToString(Session["mobile"]);
                string finalstring = phone + "#" + emailid;
                Session["GATval"] = Gstudent.Models.CryptorEngine.Encrypt(finalstring, true);
                await loginrepository.insertlogincoursereg(Session["uid"].ToString(), Session["Curr_sem"].ToString());
                Userlogin coursereg = await loginrepository.getTBLSTUDENTLOGIN(Session["uid"].ToString());
                Userlogin coursereg1 = await loginrepository.getcreditscount(Session["uid"].ToString());
                Userlogin maccount = await loginrepository.getmacagainststudent(Session["uid"].ToString());
                if (maccount.regdno == "1")
                {
                    Session["maccount"] = "1";

                }
                else
                {
                    Session["maccount"] = "0";
                }

                if (coursereg1.regdno == "1")
                {
                    Session["credits"] = "1";

                }
                else
                {
                    Session["credits"] = "0";
                }


                if (coursereg.regdno == "1")
                {
                    Session["coursereg"] = "1";

                }
                else
                {
                    Session["coursereg"] = "0";
                }
                 Userlogin courseregblock = await loginrepository.getTBL_ATTENDENCE_STUDENTS(Session["uid"].ToString());

                if (Convert.ToInt32(courseregblock.regdno) >0)
                {
                    Session["courseregblock"] = "1";

                }
                else
                {
                    Session["courseregblock"] = "0";
                }

                Userlogin backattnd = await loginrepository.getTBLBacklogattendance(Session["uid"].ToString());
                if (Convert.ToInt32(backattnd.regdno) >= 1)
                {
                    Session["backattnd"] = "1";

                }
                else
                {
                    Session["backattnd"] = "0";
                }
                string emailidnew = cm.emailid;


                byte[] key = Encoding.UTF8.GetBytes("0123456789abcdef");
                byte[] iv = Encoding.UTF8.GetBytes("abcdef9876543210");

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);


                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Write all data to the stream
                                swEncrypt.Write(emailidnew);
                            }
                        }

                        byte[] encrypted = msEncrypt.ToArray();
                        string encryptedBase64 = Convert.ToBase64String(encrypted);
                        Session["stdhelpdesk"] = encryptedBase64;

                    }
                }

                var campus = Session["campus_code"].ToString();
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
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }


        }
    }
}