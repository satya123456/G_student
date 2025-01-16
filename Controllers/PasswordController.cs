using Gstudent.Models;
using Gstudent.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Gstudent.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IPasswordRepository passwordrepository;

        String userid = null, type = null, code = null;
        String ip; int id;
        public PasswordController(IPasswordRepository _IPasswordRepository)
        {
            this.passwordrepository = _IPasswordRepository;

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

                return RedirectToAction("ResetPassword");
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
                    string campus = data.CAMPUS;
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

                int result = await passwordrepository.UpdateStudentPassword(student_id, user_id, encpwd);
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
    }
}