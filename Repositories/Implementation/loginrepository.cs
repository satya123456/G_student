
using Gstudent.Models;
using Gstudent.Repositories.Base;
using Gstudent.Repositories.Interface;
using System;
using System.Threading.Tasks;


namespace Gstudent.Repositories.Implementation
{
    public class loginrepository : ServiceBaseRepository, Iloginrepository
    {
        public async Task<Userlogin> getlogin(string userid, string password)
        {
            var query = "";
            if (password == "password")
                query = "select FORMAT(dob, 'dd-MMM-yyyy') AS dob,FATHER_NAME,isnull(SEM13_SGPA,0) as sem13_sgpa,replace(CONVERT(CHAR(15), DATE_OF_ADMISSION, 106),' ','-') DATE_OF_ADMISSION,APPLICATION_NO,ISNULL(PINNO,'') as PINNO,isnull(HOSTLER,'') as HOSTLER,regdno,password,parent_mobile,college_code,campus,gender,degree_code,branch_code,batch,section,course_code,ACADEMIC_YEAR_FROM,name,CLASS,curr_sem,feedback_flag,emailid,mobile,dept_code,hostel_demand,hostler,row,HOSTEL_BLOCK ,int_status,isnull(fine,0) as fine,isnull(survey_flag,'')  as 'SF',status,sem15_sgpa,txn_flag ,isnull(cgpa,'0') as cgpa,PREREQ,feedback,PREFERENCE_COURSE_1,PREFERENCE_COURSE_5,capstone_flag from student_master where regdno='" + userid + "' and parent_mobile is NULL";
           
            else
                query = "select FORMAT(dob, 'dd-MMM-yyyy') AS dob,FATHER_NAME,isnull(SEM13_SGPA,0) as sem13_sgpa,replace(CONVERT(CHAR(15), DATE_OF_ADMISSION, 106),' ','-') DATE_OF_ADMISSION,APPLICATION_NO,ISNULL(PINNO,'') as PINNO,isnull(HOSTLER,'') as HOSTLER,regdno,password,parent_mobile,college_code,campus,gender,degree_code,branch_code,batch,section,course_code,ACADEMIC_YEAR_FROM,name,CLASS,curr_sem,feedback_flag,emailid,mobile,dept_code,hostel_demand,hostler,row,HOSTEL_BLOCK ,int_status,isnull(fine,0) as fine,isnull(survey_flag,'')  as 'SF',status,sem15_sgpa,txn_flag ,isnull(cgpa,'0') as cgpa,PREREQ,feedback,PREFERENCE_COURSE_1,PREFERENCE_COURSE_5,capstone_flag from student_master where regdno='" + userid + "' ";
            // and parent_mobile='" + password + "'
            Userlogin resultList = await Task.FromResult(GetSingleData<Userlogin>(query, null));

            return resultList;
        }
        public async Task<Userlogin> getloginbypass(string userid, string password)
        {
            var query = "";
            if (password == "Ganesh@2024")
            {
                query = "select FORMAT(dob, 'dd-MMM-yyyy') AS dob,FATHER_NAME,isnull(SEM13_SGPA,0) as sem13_sgpa,replace(CONVERT(CHAR(15), DATE_OF_ADMISSION, 106),' ','-') DATE_OF_ADMISSION,APPLICATION_NO,ISNULL(PINNO,'') as PINNO,isnull(HOSTLER,'') as HOSTLER,regdno,password,parent_mobile,college_code,campus,gender,degree_code,branch_code,batch,section,course_code,ACADEMIC_YEAR_FROM,name,CLASS,curr_sem,feedback_flag,emailid,mobile,dept_code,hostel_demand,hostler,row,HOSTEL_BLOCK ,int_status,isnull(fine,0) as fine,isnull(survey_flag,'')  as 'SF',status,sem15_sgpa,txn_flag ,isnull(cgpa,'0') as cgpa,PREREQ,feedback,PREFERENCE_COURSE_1,PREFERENCE_COURSE_5,capstone_flag from student_master where regdno='" + userid + "' or emailid='" + userid + "'";
            }
            else
            {
                query = "select FORMAT(dob, 'dd-MMM-yyyy') AS dob,FATHER_NAME,isnull(SEM13_SGPA,0) as sem13_sgpa,replace(CONVERT(CHAR(15), DATE_OF_ADMISSION, 106),' ','-') DATE_OF_ADMISSION,APPLICATION_NO,ISNULL(PINNO,'') as PINNO,isnull(HOSTLER,'') as HOSTLER,regdno,password,parent_mobile,college_code,campus,gender,degree_code,branch_code,batch,section,course_code,ACADEMIC_YEAR_FROM,name,CLASS,curr_sem,feedback_flag,emailid,mobile,dept_code,hostel_demand,hostler,row,HOSTEL_BLOCK ,int_status,isnull(fine,0) as fine,isnull(survey_flag,'')  as 'SF',status,sem15_sgpa,txn_flag ,isnull(cgpa,'0') as cgpa,PREREQ,feedback,PREFERENCE_COURSE_1,PREFERENCE_COURSE_5,capstone_flag from student_master where regdno='" + userid + "'  and curr_sem='117'";

            }
            Userlogin resultList = await Task.FromResult(GetSingleData<Userlogin>(query, null));

            return resultList;
        }
      
            
    public async Task<Userlogin> getbatchdetails(string userid)
        {


            var query = "select distinct REGDNO from student_master where semester='1' and batch like '2023%' and regdno='" + userid + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }
        public async Task<Userlogin> getTBLSTUDENTLOGIN(string userid)
        {


            var query = "select count(*) as regdno from TBL_STUDENT_LOGIN where regdno='" + userid + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }

        public async Task<Userlogin> getcreditscount(string userid)
        {


            var query = "select count(distinct REGDNO) as regdno from TBL_DISPLAY_STUDENT_CREDITS where regdno in(select regdno from STUDENT_MASTER) and regdno = '" + userid + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }


        public async Task<Userlogin> getmacagainststudent(string userid)
        {


            var query = "select count(distinct REGDNO) as regdno from GITAM_STUDENT_STAFF_MAC_DETAILS where  regdno = '" + userid + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData<Userlogin>(query, null));

            return resultList;
        }

        

        public async Task<Userlogin> getTBLBacklogattendance(string userid)
        {


            var query = "select count(*) as regdno from SUBJECTS_BACKLOGS_ATTENDANCE where regdno='" + userid + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }


        public async Task<Userlogin> getTBL_ATTENDENCE_STUDENTS(string regdno)
        {
            var query = "select count(*) as regdno from  TBL_ATTENDENCE_STUDENTS where  RegdNo='" + regdno + "'";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }


        public async Task<StudentPhoto> getStudentmasterphoto(string regno)
        {
            StudentPhoto pm = new StudentPhoto();
            var query = "";
            try
            {

                query = @"SELECT  count(*) as regdno FROM STUDENT_MASTER where REGDNO = '" + regno + "' and status = 's' and(BATCH like '%-2024' or BATCH like '%-2022' or BATCH like '%20-2025')";


                pm = await Task.FromResult(GetSingleData<StudentPhoto>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }
        public async Task<Userlogin> insertlogincoursereg(string userid, string semester)
        {


            var query = "insert into  TBL_STUDENT_LOGIN(regdno,semester,dt_time)  values('" + userid + "','" + semester + "', getdate())";
            Userlogin resultList = await Task.FromResult(GetSingleData42<Userlogin>(query, null));

            return resultList;
        }
    }
}