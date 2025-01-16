using Gstudent.Models;
using Gstudent.Repositories.Base;
using Gstudent.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Gstudent.Repositories.Implementation
{
    public class PasswordRepository : ServiceBaseRepository, IPasswordRepository
    {

        public async Task<Userlogin> getEmplogin(string userid, string password)
        {
            var query = "";
            if (password == "password")
                query = "select EMPID,EMP_NAME,DEPT_CODE from EMPLOYEE_MASTER where EMPid='" + userid + "' AND MOBILE='" + password + "'";

            else
                query = "select EMPID,EMP_NAME,DEPT_CODE from EMPLOYEE_MASTER where EMPid='" + userid + "' AND MOBILE='" + password + "'";

            Userlogin resultList = await Task.FromResult(GetSingleData<Userlogin>(query, null));

            return resultList;
        }


        public async Task<Userlogin> getEmplogindetails(string userid)
        {
            var query = "";

            query = "select EMPID,EMP_NAME,DEPT_CODE,Mobile as Password from EMPLOYEE_MASTER where EMPid='" + userid + "' ";


            Userlogin resultList = await Task.FromResult(GetSingleData<Userlogin>(query, null));

            return resultList;
        }



        public async Task<IEnumerable<Hallticket>> getStudentsdatahallget(string regno)

        {
            try
            {

                var query = "select FORMAT(dob, 'dd-MMM-yyyy') AS dob,name,FATHER_NAME,college_code,BRANCH_code,section,curr_sem as CURRSEM,regdno,course_Code,CAMPUS,* from student_master where regdno='" + regno + "'";

                return await Task.FromResult(GetAllData<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Students> getStudentsdata(string regno)
        {
            try
            {

                var query = "select name,father_name,college_code,BRANCH_code,section,curr_sem as CURRSEM,regdno,course_Code,CAMPUS,* from student_master where regdno='" + regno + "'";

                return await Task.FromResult(GetSingleData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> UpdateStudentPassword(string studentid, string userid, string password)
        {
            int j = 0;
            try
            {

                var query = @"update student_user_master set NEW_PASSWORD='" + password + "', updated_by ='" + userid + "'  where REGDNO='" + studentid + "'";
                j = await Task.FromResult(Update221gitam(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }
        public async Task<IEnumerable<Students>> GetSearchAutocompletestudent(string SearchName, string check)
        {


            string query = "";
            if (check.Equals("true"))
            {
                query = @"select top 1 regdno,NAME,feedback_flag,*
from student_master  where  (((regdno) like '%" + SearchName + "%')   or ((NAME) like '%" + SearchName + "%')) " + "  and status in ('S','D') and  COLLEGE_CODE not in ('CDL')";

            }
            else
            {
                query = @"select top 10 regdno,NAME,feedback_flag,*
from student_master  where  (((regdno) like '%" + SearchName + "%')   or ((NAME) like '%" + SearchName + "%')) " + "  and status in ('S','D') and  COLLEGE_CODE not in ('CDL')";

            }
            return await Task.FromResult(GetAllData<Students>(query, null));
        }

    }
}