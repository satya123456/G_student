using Gstudent.Models;
using Gstudent.Repositories.Base;
using Gstudent.Repositories.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using MySqlConnector;

namespace Gstudent.Repositories.Implementation
{
    public class StudentRepository : ServiceBaseRepository, IStudentRepository
    {
        private readonly string _dapperconnection42 = ConfigurationManager.ConnectionStrings["studentconn42"].ConnectionString;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["studentconnMYSQL"].ConnectionString;

        //public async Task<IEnumerable<Students>> GetDetails(string userid)
        //{
        //    try
        //    {

        //        var query = @"select name,regdno,SUBSTRING((CONVERT(VARCHAR(11), dob, 106)), 0, 12),BLOOD_GROUP,MOBILE,EMAILID,GENDER,nationality,RELIGION,CATEGORY,DOORNO,LOCATION,CITY,STATE,COUNTRY,PINCODE,FATHER_NAME,MOTHERNAME,CAMPUS,COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,BATCH,CLASS,SECTION,curr_sem,parent_mobile,GUARDIANNAME,GUARDIAN_CONTACT,PARENT_EMAIL_ID,AADHAR_NO,Fee_reimbursement,Parental_income,CATEGORY,OCCUPATION_PARENT from student_master where regdno='" + userid + "' ";
        //        return await Task.FromResult(GetAllData<Students>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<IEnumerable<Attendance>> Getattendance_semster(string user_id, string table_name, string college_code, string cur_sem, string batchattd)
        {
            try
            {
                //var query = @"select(cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  and batch = '" + batchattd + "'  and Regdno = s.Regdno and Semester = s.Semester and Status = 'P') * 100.0) / (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  and batch = '" + batchattd + "'  and Regdno = s.Regdno and Semester = s.Semester)),2 ) as decimal(5, 2))) as percentage from " + table_name + " s where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  and batch = '" + batchattd + "'  group by regdno,Semester";
                /*for batch removal*/
                //// var query = @"select(cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  and Regdno = s.Regdno and Semester = s.Semester and Status = 'P') * 100.0) / (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'   and Regdno = s.Regdno and Semester = s.Semester)),2 ) as decimal(5, 2))) as percentage from " + table_name + " s where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  group by regdno,Semester";

                var query = @"select(cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  and Regdno = s.Regdno and Semester = s.Semester and Status = 'P' and  subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester = '" + cur_sem + "') ) * 100.0) / (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'   and Regdno = s.Regdno and Semester = s.Semester and subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester = '" + cur_sem + "'))),2 ) as decimal(5, 2))) as percentage from " + table_name + " s where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  group by regdno,Semester";

                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Attendance>> Getattendance_semster_gimsr(string user_id, string table_name, string college_code, string cur_sem, string batchattd)
        {
            try
            {
                var query = @"select(cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'    and Regdno = s.Regdno and Semester = s.Semester and Status = 'P') * 100.0) / (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'   and Regdno = s.Regdno and Semester = s.Semester)),2 ) as decimal(5, 2))) as percentage from " + table_name + " s where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'   group by regdno,Semester";
                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Attendance>> Getflag(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string campus, string course_code)
        {
            try
            {
                var query = @"select * from GLEARN_ATTENDANCE_DISPLAY  where display_flag='N' and semester='" + cur_sem + "' and college='" + college_code + "' and campus='" + campus + "' and COURSE_code='" + course_code + "'";
                return await Task.FromResult(GetAllData<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Attendance>> Getattendance_semster_byselection(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string category, string date)
        {
            var query = "";

            try
            {
                if (category == "Today")
                {
                    DateTime nextdate1;
                    nextdate1 = System.DateTime.Now.Date;
                    query = query + "";
                    query = query + "select convert(varchar,Attdate,105)+' '+substring(convert(varchar,Fromtime,108),1,5)as [sessiondate],substring(convert(varchar,totime,108),1,5)as [To_time],SubjectName as [subjectname],PostedBy as [Faculty_ID],postedname as [facultyname],  case Status when 'P' then 'Present' else 'Absent' end as STATUS from " + table_name + " where regdno='" + user_id + "' and ";
                    query = query + " CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "' ";
                    query = query + " and year(Attdate)= '" + nextdate1.Year + "' and month(Attdate)= '" + nextdate1.Month + "' and day(Attdate)= '" + nextdate1.Day + "'";

                }
                else if (category == "Yesterday")
                {
                    DateTime nextdate1;
                    nextdate1 = System.DateTime.Now.AddDays(-1);
                    query = query + "";
                    query = query + "select regdno,convert(varchar,Attdate,105)+' '+substring(convert(varchar,Fromtime,108),1,5)as [sessiondate],substring(convert(varchar,totime,108),1,5)as [To_time],SubjectName as [subjectname],PostedBy as [Faculty_ID],postedname as [facultyname],  case Status when 'P' then 'Present' else 'Absent' end as STATUS from " + table_name + " where regdno='" + user_id + "' and ";
                    // query = query + " CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'";
                    query = query + "  Semester = '" + cur_sem + "'";
                    query = query + " and year(Attdate)= '" + nextdate1.Year + "' and month(Attdate)= '" + nextdate1.Month + "' and day(Attdate)= '" + nextdate1.Day + "'";

                }

                else if (category == "subject")
                {
                    if (cur_sem == "1")
                    {
                        //query = query + "select Regdno as regno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P'   and SubjectCode = s.SubjectCode and Semester = '" + cur_sem + "')*100.0)/";
                        //query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "')) ,2 ) as decimal(5,2))) as percentage";
                        //query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "' group by regdno,SubjectCode,subjectname,status";

                        /*backup*/
                        //query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )*100.0)/";
                        //query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "' )) ,2 ) as decimal(5,2))) as percentage";
                        //query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'  and Semester = '" + cur_sem + "'  and subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester= '" + cur_sem + "' ) group by regdno,SubjectCode,subjectname,status";


                        /*college code removal*/
                        query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )*100.0)/";
                        query = query + " (select count(*) from " + table_name + " where  Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "' )) ,2 ) as decimal(5,2))) as percentage";
                        query = query + " from " + table_name + " s  where Regdno = '" + user_id + "'   and Semester = '" + cur_sem + "'  and subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester= '" + cur_sem + "' ) group by regdno,SubjectCode,subjectname,status";


                    }
                    else
                    {
                        //query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )*100.0)/";
                        //query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "' )) ,2 ) as decimal(5,2))) as percentage";
                        //query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'  and Semester = '" + cur_sem + "'   group by regdno,SubjectCode,subjectname,status";

                        /*backup*/
                        //query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )*100.0)/";
                        //query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "' )) ,2 ) as decimal(5,2))) as percentage";
                        //query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'  and Semester = '" + cur_sem + "'  and subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester= '" + cur_sem + "' ) group by regdno,SubjectCode,subjectname,status";

                        /*college code removal*/
                        query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where  Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )*100.0)/";
                        query = query + " (select count(*) from " + table_name + " where  Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "' )) ,2 ) as decimal(5,2))) as percentage";
                        query = query + " from " + table_name + " s  where Regdno = '" + user_id + "'  and Semester = '" + cur_sem + "'  and subjectcode COLLATE SQL_Latin1_General_CP1_CI_AS in  (SELECT distinct   b.Subject_Code FROM " + table_name + " a INNER JOIN LinkedServer_192_168_64_42.GITAMEVALUATION.dbo.cbcs_student_subject_assign b ON a.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS = b.Regdno COLLATE SQL_Latin1_General_CP1_CI_AS and a.SubjectCode COLLATE SQL_Latin1_General_CP1_CI_AS = b.subject_code COLLATE SQL_Latin1_General_CP1_CI_AS where b.Regdno = '" + user_id + "' and b.semester= '" + cur_sem + "' ) group by regdno,SubjectCode,subjectname,status";

                    }
                }
                else if (category == "summerterm")
                {
                    string fromdate = "";
                    string todate = "";
                    if (date == null)
                    {
                        date = "";
                    }
                    if (date.Contains("to"))
                    {
                        string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
                        fromdate = dates[0];
                        todate = dates[1];

                    }

                    //string[] datesplit = fromdate.Split('-');
                    //var fromday = datesplit[0];
                    //var frommon = datesplit[1];
                    //var fromyear = datesplit[2];
                    //string[] datesplitto = todate.Split('-');
                    //var today = datesplitto[0];
                    //var tomon = datesplitto[1];
                    //var toyear = datesplitto[2];
                    query = query + "SELECT regdno, format(Attdate,'yyyy-MM-dd') +' ' +SUBSTRING(CONVERT(varchar, Fromtime, 108), 1, 5) AS [sessiondate], ";
                    query = query + "SUBSTRING(CONVERT(varchar, totime, 108), 1, 5) AS [To_time], SubjectName AS [subjectname], ";
                    query = query + "PostedBy AS [Faculty_ID], postedname AS [facultyname], CASE Status WHEN 'P' THEN 'Present' ELSE 'Absent' END AS STATUS ";
                    query = query + "FROM " + table_name + " ";
                    query = query + "WHERE regdno = '" + user_id + "' ";
                    //  query = query + "AND CollegeCode = '" + college_code + "' ";
                    query = query + "AND Semester = '92' ";
                    //query = query + "AND batch = '" + batchattd + "' ";
                    //query = query + "AND (Attdate BETWEEN CONVERT(DATETIME, '" + fromyear + "-" + frommon + "-" + fromday + "', 102) ";
                    //query = query + "AND CONVERT(DATETIME, '" + toyear + "-" + tomon + "-" + today + "', 102)) order by Format(Attdate,'yyyy-MM-dd') desc";

                }
                else
                {
                    string fromdate = "";
                    string todate = "";
                    if (date == null)
                    {
                        date = "";
                    }
                    if (date.Contains("to"))
                    {
                        string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
                        fromdate = dates[0];
                        todate = dates[1];

                    }

                    string[] datesplit = fromdate.Split('-');
                    var fromday = datesplit[0];
                    var frommon = datesplit[1];
                    var fromyear = datesplit[2];
                    string[] datesplitto = todate.Split('-');
                    var today = datesplitto[0];
                    var tomon = datesplitto[1];
                    var toyear = datesplitto[2];
                    query = query + "SELECT regdno, format(Attdate,'yyyy-MM-dd') +' ' +SUBSTRING(CONVERT(varchar, Fromtime, 108), 1, 5) AS [sessiondate], ";
                    query = query + "SUBSTRING(CONVERT(varchar, totime, 108), 1, 5) AS [To_time], SubjectName AS [subjectname], ";
                    query = query + "PostedBy AS [Faculty_ID], postedname AS [facultyname], CASE Status WHEN 'P' THEN 'Present' ELSE 'Absent' END AS STATUS ";
                    query = query + "FROM " + table_name + " ";
                    query = query + "WHERE regdno = '" + user_id + "' ";
                    query = query + "AND CollegeCode = '" + college_code + "' ";
                    query = query + "AND Semester = '" + cur_sem + "' ";
                    //query = query + "AND batch = '" + batchattd + "' ";
                    query = query + "AND (Attdate BETWEEN CONVERT(DATETIME, '" + fromyear + "-" + frommon + "-" + fromday + "', 102) ";
                    query = query + "AND CONVERT(DATETIME, '" + toyear + "-" + tomon + "-" + today + "', 102)) order by Format(Attdate,'yyyy-MM-dd') desc";

                }

                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Attendance>> Getattendance_semster_byselection_gimsr(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string category, string date)
        {
            var query = "";

            try
            {
                if (category == "Today")
                {
                    DateTime nextdate1;
                    nextdate1 = System.DateTime.Now.Date;
                    query = query + "";
                    query = query + "select convert(varchar,Attdate,105)+' '+substring(convert(varchar,Fromtime,108),1,5)as [sessiondate],substring(convert(varchar,totime,108),1,5)as [To_time],SubjectName as [subjectname],PostedBy as [Faculty_ID],postedname as [facultyname],  case Status when 'P' then 'Present' else 'Absent' end as STATUS from " + table_name + " where regdno='" + user_id + "' and ";
                    query = query + " CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'  ";
                    query = query + " and year(Attdate)= '" + nextdate1.Year + "' and month(Attdate)= '" + nextdate1.Month + "' and day(Attdate)= '" + nextdate1.Day + "'";

                }
                else if (category == "Yesterday")
                {
                    DateTime nextdate1;
                    nextdate1 = System.DateTime.Now.AddDays(-1);
                    query = query + "";
                    query = query + "select regdno,convert(varchar,Attdate,105)+' '+substring(convert(varchar,Fromtime,108),1,5)as [sessiondate],substring(convert(varchar,totime,108),1,5)as [To_time],SubjectName as [subjectname],PostedBy as [Faculty_ID],postedname as [facultyname],  case Status when 'P' then 'Present' else 'Absent' end as STATUS from " + table_name + " where regdno='" + user_id + "' and ";
                    query = query + " CollegeCode = '" + college_code + "' and Semester = '" + cur_sem + "'    ";
                    query = query + " and year(Attdate)= '" + nextdate1.Year + "' and month(Attdate)= '" + nextdate1.Month + "' and day(Attdate)= '" + nextdate1.Day + "'";

                }

                else if (category == "subject")
                {
                    if (cur_sem == "1")
                    {
                        query = query + "select Regdno as regno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P'   and SubjectCode = s.SubjectCode and Semester = '" + cur_sem + "')*100.0)/";
                        query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode    and Semester = '" + cur_sem + "')) ,2 ) as decimal(5,2))) as percentage";
                        query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'   and Semester = '" + cur_sem + "' group by regdno,SubjectCode,subjectname";
                    }
                    else
                    {
                        query = query + "select Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'  )*100.0)/";
                        query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '" + cur_sem + "'   )) ,2 ) as decimal(5,2))) as percentage";
                        query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'  and Semester = '" + cur_sem + "'   group by regdno,SubjectCode,subjectname";
                    }
                }

                else
                {
                    string fromdate = "";
                    string todate = "";
                    if (date == null)
                    {
                        date = "";
                    }
                    if (date.Contains("to"))
                    {
                        string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
                        fromdate = dates[0];
                        todate = dates[1];

                    }

                    string[] datesplit = fromdate.Split('-');
                    var fromday = datesplit[0];
                    var frommon = datesplit[1];
                    var fromyear = datesplit[2];
                    string[] datesplitto = todate.Split('-');
                    var today = datesplitto[0];
                    var tomon = datesplitto[1];
                    var toyear = datesplitto[2];
                    query = query + "SELECT regdno, CONVERT(varchar, Attdate, 105) + ' ' + SUBSTRING(CONVERT(varchar, Fromtime, 108), 1, 5) AS [sessiondate], ";
                    query = query + "SUBSTRING(CONVERT(varchar, totime, 108), 1, 5) AS [To_time], SubjectName AS [subjectname], ";
                    query = query + "PostedBy AS [Faculty_ID], postedname AS [facultyname], CASE Status WHEN 'P' THEN 'Present' ELSE 'Absent' END AS STATUS ";
                    query = query + "FROM " + table_name + " ";
                    query = query + "WHERE regdno = '" + user_id + "' ";
                    query = query + "AND CollegeCode = '" + college_code + "' ";
                    query = query + "AND Semester = '" + cur_sem + "' ";
                    //query = query + "AND batch = '" + batchattd + "' ";
                    query = query + "AND (Attdate BETWEEN CONVERT(DATETIME, '" + fromyear + "-" + frommon + "-" + fromday + "', 102) ";
                    query = query + "AND CONVERT(DATETIME, '" + toyear + "-" + tomon + "-" + today + "', 102))";
                    //query = query + " and (Attdate between '"+fromdate+"' and '"+todate+"')";
                }
                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Attendance>> Getsemsterlist(string college_code, string campus_code, string branch_code, string batch)
        {
            try
            {
                var query = "select  distinct SEMESTER from SUBJECT_MASTER  where  COLLEGE_CODE='" + college_code + "' and BRANCH_CODE='" + branch_code + "' and CAMPUS_CODE='" + campus_code + "' and BATCH like '%" + batch + '-' + "%' order by Semester";
                return await Task.FromResult(GetAllData50<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<Attendance>> GetSemesterdata(string college_code, string campus_code, string branch_code, string batch, string semester)
        {
            try
            {
                var query = "select SUBJECT_CODE as 'subjectcode',SUBJECT_NAME as 'subjectname',SUB_CATEGORY +'/'+  Subject_type as 'Subject_type' ,CREDITS as credits from SUBJECT_MASTER  where  COLLEGE_CODE='" + college_code + "' and BRANCH_CODE='" + branch_code + "' and CAMPUS_CODE='" + campus_code + "' and BATCH like '%" + batch + '-' + "%' and semester='" + semester + "'   order by SUBJECT_ORDER ";
                return await Task.FromResult(GetAllData50<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<List<Scholarship>> Getscholarshipdetails(string applicationno)
        {
            try
            {
                var query = "select r.ApplicationNumber, r.StudentName, r.StudentMobile, r.StudentEmailId,(select  campusname from tblCampusMaster where CampusCode = r.campuscode) as campusname ,(select  collegename from tblCollegeMaster where CampusCode = r.campuscode and CollegeCode = r.CollegeCode) as collegename,r.DegreeCode,r.counselling_coursename from tblregistrationmaster r INNER JOIN tblscholarship s ON CAST(s.ApplicationNumber AS VARCHAR) = r.ApplicationNumber where r.applicationnumber = '" + applicationno + "' and r.allotment_flag='Y' and r.ApplicationNumber like '2024%'  and r.CollegeCode not in('GPP')";
                return await Task.FromResult(GetAllData60<Scholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<IEnumerable<Scholarship>> Getscholarcount(string applicationno)
        {
            try
            {
                var query = "select r.ApplicationNumber, r.StudentName, r.StudentMobile, r.StudentEmailId,(select  campusname from tblCampusMaster where CampusCode = r.campuscode) as campusname ,(select  collegename from tblCollegeMaster where CampusCode = r.campuscode and CollegeCode = r.CollegeCode) as collegename,r.DegreeCode,r.counselling_coursename from tblregistrationmaster r,tblscholarship s where s.applicationnumber=r.applicationnumber and r.applicationnumber = '" + applicationno + "' and r.allotment_flag='Y'";
                return await Task.FromResult(GetAllData60<Scholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<List<Scholarship>> Getscholarshipdetailsbycategory(string category, string regdno, string Applicationo)
        {
            var query = "";
            try
            {
                if (category == "S")
                {
                    query = "select regdno as regdno1,APPLICATION_NO as ApplicationNumber, name as name1,CAMPUS as CAMPUS1,COLLEGE_CODE as COLLEGE_CODE1,DEGREE_CODE as DEGREE_CODE1 ,BRANCH_CODE as BRANCH_CODE1,COURSE_CODE as COURSE_CODE1,batch as batch1 from STUDENT_MASTER where regdno='" + regdno + "' and APPLICATION_NO='" + Applicationo + "' and STATUS='S'";

                }
                else if (category == "A")
                {
                    query = "select regdno as regdno1,APPLICATION_NO as ApplicationNumber, name as  name1,CAMPUS as CAMPUS1,COLLEGE_CODE as COLLEGE_CODE1,DEGREE_CODE as DEGREE_CODE1,BRANCH_CODE as BRANCH_CODE1,COURSE_CODE as COURSE_CODE1,batch as batch1 from STUDENT_MASTER where regdno='" + regdno + "' and APPLICATION_NO='" + Applicationo + "' and batch like '____-202%'";
                }
                else
                {

                }
                return await Task.FromResult(GetAllData<Scholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public async Task<Scholarship> Updatescholarship(Scholarship chart)
        {
            try
            {
                int j = 0;
                string Query = "";
                if (chart.category1 == "S")
                {
                    Query = "update TBLSCHOLARSHIP set sibling_confirmation_flag='N',sibling_flag='N',sibling_name='" + chart.name1 + "',sibling_regno='" + chart.regdno1 + "',sibling_campus='" + chart.CAMPUS1 + "',sibling_college='" + chart.COLLEGE_CODE1 + "',sibling_branch='" + chart.BRANCH_CODE1 + "',sibling_course='" + chart.COURSE_CODE1 + "',sibling_batch='" + chart.batch1 + "',sibling_degree='" + chart.DEGREE_CODE1 + "',sibling_scholarpercent='10' where APPLICATIONNUMBER='" + chart.ApplicationNumber + "'";
                }

                else if (chart.category1 == "A")
                {
                    Query = "update TBLSCHOLARSHIP set Alumni_confirmation_flag='N',Alumni_flag='N',Alumni_name='" + chart.name1 + "',Alumni_regno='" + chart.regdno1 + "',Alumni_campus='" + chart.CAMPUS1 + "',Alumni_college='" + chart.COLLEGE_CODE1 + "',Alumni_branch='" + chart.BRANCH_CODE1 + "',Alumni_course='" + chart.COURSE_CODE1 + "',Alumni_batch='" + chart.batch1 + "',Alumni_degree='" + chart.DEGREE_CODE1 + "',Alumni_scholarpercent='10' where APPLICATIONNUMBER='" + chart.ApplicationNumber + "'";
                }
                else
                {

                }
                j = await Task.FromResult(Update60(Query, null));


                chart.flag = j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }

        public async Task<IEnumerable<StudentProfile>> GetDetails(string userid)
        {
            try
            {
                var query = @"select name,regdno,SUBSTRING((CONVERT(VARCHAR(11), dob, 106)), 0, 12) as dob,BLOOD_GROUP,MOBILE,EMAILID,GENDER,nationality,RELIGION,CATEGORY,DOORNO,LOCATION,CITY,STATE,COUNTRY,PINCODE,FATHER_NAME,MOTHERNAME,CAMPUS,COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,BATCH,CLASS,SECTION,curr_sem,parent_mobile,GUARDIANNAME,GUARDIAN_CONTACT,PARENT_EMAIL_ID,AADHAR_NO,Fee_reimbursement,Parental_income,CATEGORY,OCCUPATION_PARENT from student_master where regdno='" + userid + "' ";
                return await Task.FromResult(GetAllData<StudentProfile>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<StudentProfile>> getstates()
        {
            List<StudentProfile> pm = null;
            try
            {

                var query = "select distinct state_name as STATE from tblState where Country_ID='101'";
                pm = await Task.FromResult(GetAllData60<StudentProfile>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }


        public async Task<StudentProfile> Updateprofileasync(StudentProfile _Parameter)
        {
            bool retnVal = false;

            try
            {
                var query1 = "insert into nirf_student_master_changes(regdno, name, CATEGORY, DOORNO, LOCATION, CITY, STATE, COUNTRY, PINCODE, Fee_reimbursement, OCCUPATION_PARENT, Parental_income, NRIF_flag, AADHAR_NO, MOBILE, BLOOD_GROUP, dt_time) select regdno, name, CATEGORY, DOORNO, LOCATION, CITY, STATE, COUNTRY, PINCODE, Fee_reimbursement, OCCUPATION_PARENT, Parental_income, NRIF_flag, AADHAR_NO, MOBILE, BLOOD_GROUP, getdate() from STUDENT_MASTER where regdno = '" + _Parameter.regdno + "'";
                int k = await Task.FromResult(InsertData(query1, null));

                var query = "update student_master   set Fee_reimbursement ='" + _Parameter.Fee_reimbursement + "', Parental_income ='" + _Parameter.Parental_income + "', NRIF_flag ='Y',  CATEGORY ='" + _Parameter.CATEGORY + "', OCCUPATION_PARENT='" + _Parameter.OCCUPATION_PARENT + "',  DOORNO= '" + _Parameter.DOORNO + "',LOCATION= '" + _Parameter.LOCATION + "',CITY= '" + _Parameter.CITY + "',STATE= '" + _Parameter.STATE + "',COUNTRY='" + _Parameter.COUNTRY + "',PINCODE='" + _Parameter.PINCODE + "', mobile= '" + _Parameter.MOBILE + "',AADHAR_NO='" + _Parameter.AADHAR_NO + "',BLOOD_GROUP='" + _Parameter.BLOOD_GROUP + "', dob='" + _Parameter.dob + "'  where regdno='" + _Parameter.regdno + "'  ";
                int j = await Task.FromResult(Update(query, null));
                if (j > 0)
                {
                    retnVal = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Parameter;
        }
        public async Task<int> getlogout(string regdno)
        {
            var query1 = "delete from TBL_STUDENT_LOGIN where regdno = '" + regdno + "'";
            int k = await Task.FromResult(Deleteorinsert(query1, null));

            return k;

        }



        public async Task<IEnumerable<Students>> GetUnderProcess(string userid)
        {
            try
            {

                var query = @"select Permission_date as Permissiondate,id,Convert(varchar(12),Fromdate,105) as From_date,Convert(varchar(12),Todate,105) as To_date,Reason,Type,Convert(varchar(12),Permission_date,105) as Date, isnull(fromtime,'') as fromtime,isnull(totime,'') as totime from  HostelPermission   where regdno='" + userid + "' and Isapprove='N' order by  Convert(varchar(12),Fromdate,105) desc";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<Students>> GetApproved(string userid)
        {
            try
            {

                var query = @"select hm.Permission_date as Permissiondate,REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') as ApprovedDate,ISNULL(e.FIRST_NAME,'') + '' + ISNULL(e.LAST_NAME,'') as emp_name,Reason,Type,Approvedby,REPLACE(CONVERT(VARCHAR(11),Fromdate, 106), ' ', '-') as Fromdate,REPLACE(CONVERT(VARCHAR(11),Todate, 106), ' ', '-') as Todate,isnull(Fromtime,'') as fromtime,isnull(Totime,'') as totime from HostelPermission hm 
                              left join EMPLOYEE_MASTER e on e.EMPID= hm.Approvedby 
                               where regdno='" + userid + "' and Isapprove='Y' order by REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') desc ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Students>> GetRejected(string userid)
        {
            try
            {

                var query = @"select hm.Permission_date as Permissiondate,REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') as ApprovedDate,ISNULL(e.FIRST_NAME,'') + '' + ISNULL(e.LAST_NAME,'') as emp_name,Reason,Type,Approvedby,REPLACE(CONVERT(VARCHAR(11),Fromdate, 106), ' ', '-') as Fromdate,REPLACE(CONVERT(VARCHAR(11),Todate, 106), ' ', '-') as Todate,isnull(Fromtime,'') as fromtime,isnull(Totime,'') as totime from HostelPermission hm left join EMPLOYEE_MASTER e on e.EMPID= hm.Approvedby where regdno='" + userid + "' and Isapprove='D' order by REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') desc ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Students>> GetHistory(string userid)
        {
            try
            {

                var query = @"select hm.Permission_date as Permissiondate,REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') as ApprovedDate,ISNULL(e.FIRST_NAME,'') + '' + ISNULL(e.LAST_NAME,'') as emp_name,Reason,Type,Approvedby,REPLACE(CONVERT(VARCHAR(11),Fromdate, 106), ' ', '-') as Fromdate,REPLACE(CONVERT(VARCHAR(11),Todate, 106), ' ', '-') as Todate,isnull(Fromtime,'') as fromtime,isnull(Totime,'') as totime from HostelPermission hm 
                              left join EMPLOYEE_MASTER e on e.EMPID= hm.Approvedby 
                               where regdno='" + userid + "' and Isapprove='I' order by REPLACE(CONVERT(VARCHAR(11),ApprovedDate, 106), ' ', '-') desc";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<Students>> GetNoticeboarddata(string college_code, string campus_code, string dept_code, string CLASS)
        {
            try
            {

                var query = @"select  top 8 Month(START_DATE) AS Month,year(START_DATE) AS year,TITLE, CONTENT_DATA,CONTENT_URL,SEARCH_ID,CONVERT(varchar(20), START_DATE,106) as Date ,
							   content_type from NOTICE_CONTENT_MASTER  where COLLEGE_CODE in('" + college_code + "','All') and  CAMPUS in('" + campus_code + "', 'All') and DEPT_CODE in('" + dept_code + "', 'All') and student = 'Y' and  class in('" + CLASS + "','ALL')  order by SEARCH_ID DESC ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Students>> GetNoticeboarddataMore(string college_code, string campus_code, string dept_code, string CLASS)
        {
            try
            {

                var query = @"   select ROW_NUMBER() OVER(ORDER BY search_id DESC) AS row,  Convert(varchar,CONVERT(varchar(20), 
							 START_DATE,106))+'  :   '+Convert(varchar(100), CONTENT_DATA) as title,Month(START_DATE) AS Month,year(START_DATE) AS year,TITLE,CONTENT_DATA,CONTENT_URL,SEARCH_ID,CONVERT(varchar(20), START_DATE,106) as Date ,content_type 
								from NOTICE_CONTENT_MASTER  where COLLEGE_CODE in('" + college_code + "','All') and  CAMPUS in('" + campus_code + "', 'All') and DEPT_CODE in('" + dept_code + "', 'All') and student = 'Y' and  class in('" + CLASS + "','ALL')  order by SEARCH_ID DESC ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public async Task<IEnumerable<Students>> GetDistinctDates(string college_code, string campus_code, string dept_code, string CLASS)
        {
            try
            {

                var query = @"   select distinct Month(START_DATE) AS Month,year(START_DATE) AS year
								from NOTICE_CONTENT_MASTER  where COLLEGE_CODE in('" + college_code + "','All') and  CAMPUS in('" + campus_code + "', 'All') and DEPT_CODE in('" + dept_code + "', 'All') and student = 'Y' and  class in('" + CLASS + "','ALL') order by month asc";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Students>> get_coordinator(string campus)
        {
            try
            {

                string query = "select empid,emp_name,mobile from GHP_COORDINATORS  where hostel='1'  and level='Level-1' and campus='" + campus + "'";
                return await Task.FromResult(GetAllData<Students>(query, null));




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Students>> gethostlerdetails(string uid)
        {
            try
            {

                // string query = "select GENDER,biometric_id,regdno,name,CURR_SEM,dept_code,ACADEMIC_YEAR_TO,BRANCH_CODE,COURSE_CODE,COLLEGE_CODE,CAMPUS,section,degree_code,batch,parent_mobile,GENDER,hostler,HOSTEL_BLOCK from student_master where regdno='" + uid + "' and Status='s'";

                string query = "select GENDER,biometric_id,regdno,name,CURR_SEM,dept_code,ACADEMIC_YEAR_TO,BRANCH_CODE,COURSE_CODE,COLLEGE_CODE,CAMPUS,section,degree_code,batch,parent_mobile,GENDER,hostler,isnull(HOSTEL_BLOCK,'') as HOSTEL_BLOCK from student_master where regdno='" + uid + "' and Status in ('s','Det_Dis','D')";

                return await Task.FromResult(GetAllData<Students>(query, null));




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Students>> gethostlerpermissions(string uid)
        {
            try
            {

                string query = "select regdno,AccountNo,Fromdate,Todate,Reason,Isapprove,Approvedby,ParentMobile,ApprovedDate,AppliedDate,Deptcode,Collegecode,CampusCode,HostelCode,Type,Fromtime,Totime,biometricid,refno,Gender,Name,Permission_date as Permissiondate,hostler,Travelby,Destination,remarks,emp_name,fromdatetime,todatetime,Traveling_information,Warden_remarks,service_flag,level_status,confirmed_by,sms_status,genarated_by from HostelPermission where regdno='" + uid + "' and Isapprove !='D'";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Students> createhostelpermision(Students student)
        {
            bool retnVal = false;

            try
            {
                var hostelpermission = "Insert into  HostelPermission(Regdno,AccountNo,Fromdate,Todate,Reason,Isapprove,Approvedby,parentmobile,ApprovedDate,AppliedDate,deptcode," +
                       "Collegecode,CampusCode,HostelCode,Type,Fromtime,Totime,GENDER,Name,Permission_date,hostler,Travelby,fromdatetime,todatetime,Traveling_information,coordinator_Id" +
                       ") values('" + student.regdno + "', '" + student.AccountNo + "','" + student.Fromdate.ToString("yyyy-MM-dd") + "', '" + student.Todate.ToString("yyyy-MM-dd") + "', '" + student.Reason + "','" + student.Isapprove + "','" + student.Approvedby + "','" + student.parent_mobile + "','','" + student.AppliedDate.ToString("yyyy-MM-dd") + "','" + student.dept_code + "'" +
                       ",'" + student.COLLEGE_CODE + "','" + student.CAMPUS + "','" + student.HostelCode + "','" + student.Type + "','" + student.Fromtime + "','" + student.Totime + "','" + student.GENDER + "','" + student.name + "','" + student.Permissiondate.ToString("yyyy-MM-dd") + "','" + student.hostler + "','" + student.Travelby + "','" + student.fromdatetime + "','" + student.todatetime + "','" + student.Travelinginformation + "','" + student.hostelconame + "')";
                int j = await Task.FromResult(InsertData(hostelpermission, null));
                if (j != 0)
                {
                    retnVal = true;
                    var hosteltrans = "Insert into  Hostel_Permission_Transactions(Regdno,AccountNo,Fromdate,Todate,Reason,Isapprove,Approvedby,parentmobile,ApprovedDate,AppliedDate,deptcode," +
                     "Collegecode,CampusCode,HostelCode,Type,Fromtime,Totime,GENDER,Permission_date,coordinator_Id) values('" + student.regdno + "', '" + student.AccountNo + "','" + student.Fromdate.ToString("yyyy-MM-dd") + "', '" + student.Todate.ToString("yyyy-MM-dd") + "', '" + student.Reason + "','" + student.Isapprove + "','" + student.Approvedby + "','" + student.parent_mobile + "','','" + student.AppliedDate.ToString("yyyy-MM-dd") + "','" + student.dept_code + "'" +
                     ",'" + student.COLLEGE_CODE + "','" + student.CAMPUS + "','" + student.HostelCode + "','" + student.Type + "','" + student.Fromtime + "','" + student.Totime + "','" + student.GENDER + "','" + student.Permissiondate.ToString("yyyy-MM-dd") + "','" + student.hostelconame + "')";
                    int k = await Task.FromResult(InsertData(hosteltrans, null));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return student;
        }

        public async Task<IEnumerable<StudentProfile>> GetCDLProfileDetails(string userid)
        {
            try
            {
                var query = @"select NAME,FATHER_NAME,convert(varchar,DOB,105)as DOB,COURSE_CODE,BRANCH_CODE,MEDIUM,SECOND_LANGUAGE,
                       STUDY_CENTRE_OPTED,TEMP_DRNO,TEMP_AREA,TEMP_CITY,TEMP_STATE,TEMP_PIN,TEMP_MOBILE,TEMP_TELEPHONE,TEMP_EMAIL,
                     GENDER,nationality,MARITAL_STATUS,SOCIAL_STATUS,AREA,ORGANISATION1,DESIGNATION1,SALARY,
                    TOTAL_SERVICE,SSC_YEAR,SSC_REGDNO,SSC_PERCENTAGE,SSC_CLASS,SSC_BOARD,SSC_ELECTIVE,
                      INTER_YEAR,INTER_REGDNO,INTER_PERCENTAGE,INTER_CLASS,INTER_BOARD,INTER_ELECTIVE,
                    DEGREE_YEAR,GRADUATION_REGDNO,DEGREE_PERCENTAGE,UG_CLASS,DEGREE_BOARD,GRADUATION_ELECTIVE,
                      PG_YEAR,PG_REGDNO,PG_PERCENTAGE,PG_CLASS,PG_BOARD,PG_ELECTIVE,
                   ENTRANCE_YEAR,ENTRANCE_REGDNO,ENTRANCE_MARKS,ENTRANCE_RANK,ENTRANCE_BOARD,ENTRANCE_ELECTIVE,CLASS,
                   DOORNO,LOCATION,CITY,STATE,PINCODE,MOBILE,PHONE,EMAILID,specialization_code
                       from student_master where regdno='" + userid + "'";
                return await Task.FromResult(GetAllData19hf<StudentProfile>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<StudentProfile> UpdateCDLProfile(StudentProfile chart)
        {
            try
            {
                int j = 0;
                int k = 0;
                string Query = "";

                Query = @"update student_master set TEMP_DRNO='" + chart.TEMP_DRNO + "',TEMP_AREA='" + chart.TEMP_AREA + "',TEMP_CITY='" + chart.TEMP_CITY + "',TEMP_STATE='" + chart.TEMP_STATE + "',TEMP_PIN='" + chart.TEMP_PIN + "',TEMP_MOBILE='" + chart.TEMP_MOBILE + "',TEMP_TELEPHONE='" + chart.TEMP_TELEPHONE + "',TEMP_EMAIL='" + chart.TEMP_EMAIL + "' where regdno='" + chart.regdno + "' AND COLLEGE_CODE='CDL'";



                j = await Task.FromResult(Update19hf(Query, null));

                if (j > 0)
                {
                    var portalid = "STUDENT";
                    var portalname = "CDL STUDENTPORTAL";
                    var txntype = "CDL PROFILE DATA UPDATE";
                    var ip = GetUser_IP();
                    var query1 = @"insert into GITAM_TRANSACTION_HISTORY(USERID,TXN_QUERY,TXN_TIME,PORTAL_ID,PORTAL_NAME,IP,TXN_TYPE) values('" + chart.regdno + "','" + Query + "',getdate(),'" + portalid + "','" + portalname + "','" + ip + "','" + txntype + "')";
                    k = await Task.FromResult(InsertData19gitamdb(Query, null));
                }

                if (k > 0)
                {
                    chart.msg = "1";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }

        public String GetUser_IP()
        {
            string VisitorsIPAddr = string.Empty;
            try
            {

                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    VisitorsIPAddr = HttpContext.Current.Request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {

            }
            return VisitorsIPAddr;
        }


        public async Task<IEnumerable<Hostelbiometric>> gethostelbiometricsmsdetails(string regno, string curr_sem)
        {
            try
            {

                string query = "select student_RegNo as Regdno,studentName as Name,mobileNo as Mobile,convert(date,sentDate) as Date,HostelName ,ROOMNO  from tr_StudentSMSSent where student_RegNo = '" + regno + "' and SEMESTER='" + curr_sem + "' order by convert(date,sentDate) desc";
                return await Task.FromResult(GetData64_156<Hostelbiometric>(query, null));




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDL_academictrack>> CDL_studentdetails(string user_id)
        {
            try
            {
                var query = "select regdno,name,CURR_SEM,ACADEMIC_YEAR_FROM,ACADEMIC_YEAR_TO,BRANCH_CODE,COURSE_CODE,COLLEGE_CODE,CAMPUS,degree_code  from student_master where regdno='" + user_id + "'";
                return await Task.FromResult(GetAllData<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CDL_academictrack>> getfeedetails(string user_id)
        {
            try
            {
                // var query = "select challan_no as[Challan_No], TOTAL_FEE As Amount,PAYMENT_STATUS As [Payment_Status],payment_date as[Payment_Date],SEMESTER as[sem_year],isnull(ddno,'') as ddno,isnull(ddamount,'') as ddamount,isnull(dddate,'') as dddate,isnull(ddbank,'') as ddbank from fee_challan_master where regdno='" + user_id + "' and PAYMENT_STATUS='Y' order by challan_date";
                var query = "select challan_no as[Challan_No], TOTAL_FEE As Amount,PAYMENT_STATUS As [Payment_Status],convert(varchar,payment_date,105) as[Payment_Date],SEMESTER as[sem_year],isnull(ddno,'') as ddno,isnull(ddamount,'') as ddamount,isnull(dddate,'') as dddate,isnull(ddbank,'') as ddbank from fee_challan_master where regdno='" + user_id + "' and PAYMENT_STATUS='Y' order by challan_date";

                return await Task.FromResult(GetAllData<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDL_academictrack>> getmaterial_dispatch_cdl(string user_id)
        {
            try
            {
                // var query = "select Regdno as [regdno],SUBJECT_CODE as [Subject_Code],SUBJECT_NAME as[Subject_Name],ISSUE_DATE as[Issue_Date], Year as[sem_year],case(IS_ISSUED) when 'Y' THEN 'Dispatched' else 'Not Dispatched' end as Status  from STUDENT_ISSUE_MASTER  where REGDNO='" + user_id + "'";
                var query = "select Regdno as [regdno],SUBJECT_CODE as [Subject_Code],SUBJECT_NAME as[Subject_Name],convert(varchar,ISSUE_DATE,105) as [Issue_Date], Year as[sem_year],case(IS_ISSUED) when 'Y' THEN 'Dispatched' else 'Not Dispatched' end as Status  from STUDENT_ISSUE_MASTER  where REGDNO='" + user_id + "'";

                return await Task.FromResult(GetAllData60cdl<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDL_academictrack>> getassginments_cdl(string user_id)
        {
            try
            {
                //var query = "select row_number() over (order by regdno) as slno,Regdno as [regdno],SUBJECT_CODE as Subject_Code,SUBJECT_NAME as Subject_Name ,convert(varchar,DATE_OF_SUBMISSION,105) as DATE_OF_SUBMISSION,YEAR,ASSIGNMENT_STATUS,isnull(finalmarks,'') as finalmarks from GITAM_ASSIGNMENT_MASTER  WHERE REGDNO='" + user_id + "' and college='CDL' order by substring(subject_code,6,3)";
                var query = "select row_number() over (order by regdno) as slno,Regdno as [regdno],SUBJECT_CODE as Subject_Code,SUBJECT_NAME as Subject_Name ,DATE_OF_SUBMISSION = case when isnull(DATE_OF_SUBMISSION,'') = '1900-01-01 00:00:00.000' then '-' else convert(varchar,DATE_OF_SUBMISSION,105) end,YEAR,ASSIGNMENT_STATUS,isnull(finalmarks,'') as finalmarks from GITAM_ASSIGNMENT_MASTER  WHERE REGDNO='" + user_id + "' and college='CDL' order by substring(subject_code,6,3)";

                return await Task.FromResult(GetAllData<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDL_academictrack>> getresults_cdl(string user_id)
        {
            try
            {
                var query = "select s.Regdno as [regdno],s.subject_code as Subject_Code,s.subject_name as Subject_Name,s.subject_type,s.semester as sem_year,s.subject_credits,s.subject_grade from NEW_RESULTS_SUBJECT s,new_results_student ss  where ss.regdno = s.regdno and s.semester = ss.semester and s.REGDNO='" + user_id + "' and ss.status = 'A' order by s.semester,s.subject_order";
                return await Task.FromResult(GetAllData52<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDL_academictrack>> Getcgpa(string user_id)
        {
            try
            {
                var query = "SELECT TOP 1 REGDNO,CGPA FROM NEW_RESULTS_STUDENT WHERE REGDNO='" + user_id + "' and status ='A' ORDER BY SEMESTER DESC";
                return await Task.FromResult(GetAllData52<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDL_academictrack>> getcertificates_cdl(string user_id)
        {
            try
            {
                // var query = "select Regdno as [regdno],OTHERS4_DESC as [Certificate],isnull(TALLY_STATUS,'') as [Status],txn_date as  [Issue_Date],ddno,total_fee as Amount,isnull(remarks,'') as [Postal_TrackID] from CDL_FEE_CHALLAN_MASTER   where  others2_desc='CDLEXAMS' and regdno='" + user_id + "'";
                var query = "select Regdno as [regdno],OTHERS4_DESC as [Certificate],isnull(TALLY_STATUS,'') as [Status], convert(varchar,txn_date,105) as  [Issue_Date],ddno,total_fee as Amount,isnull(remarks,'') as [Postal_TrackID] from CDL_FEE_CHALLAN_MASTER   where  others2_desc='CDLEXAMS' and regdno='" + user_id + "'";

                return await Task.FromResult(GetAllData60cdl<CDL_academictrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







        public async Task<IEnumerable<studenttrack>> getsemesterasync(string userid)
        {
            try
            {
                //   var query = @"select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER <= (select curr_sem from student_master where regdno = '" + userid + "') union select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER >= '21'";

                var query = @"select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER 
<= (select curr_sem from student_master where regdno = '" + userid + "')union select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER >= '21'";

                return await Task.FromResult(GetAllData<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getsubjects(string semester, string userid)
        {
            try
            {
                var query = @"select s.subject_name as Subject,s.subject_grade ,s.BATCH  as Grade from NEW_RESULTS_SUBJECT1 s,new_results_student1 sm where  sm.REGDNO=s.regdno  and s.REGDNO='" + userid + "' and s.semester='" + semester + "' and  sm.status='A' and s.semester=sm.semester and s.branch_code=sm.branch_code and s.course_code=sm.course_code  and s.batch= sm.batch   order by s.semester,s.subject_order";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getperformance(string semester, string userid)
        {
            try
            {
                // var query = @"select cgpa,sgpa,sem_credits from NEW_RESULTS_STUDENT where regdno='" + userid + "' and semester='" + semester + "'";
                //var query = @"SELECT cgpa,sgpa,sem_credits,CONVERT(DATETIME, MONTH + ' ' + YEAR) AS combined_date FROM new_results_student WHERE regdno = '"+ userid + "' and semester='" + semester + "' ORDER BY combined_date DESC;";
                var query = @"SELECT semester,cgpa,sgpa,sem_credits,CONVERT(DATETIME, MONTH + ' ' + YEAR) AS combined_date FROM new_results_student1 WHERE regdno = '" + userid + "' and semester='" + semester + "' and STATUS NOT IN ('Dt','Dc','Exp') and process_type not in ('G','GV')order by semester desc";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<studenttrack>> getperformancedashboard(string semester, string userid)
        {
            try
            {
                // var query = @"select cgpa,sgpa,sem_credits from NEW_RESULTS_STUDENT where regdno='" + userid + "' and semester='" + semester + "'";
                //var query = @"SELECT cgpa,sgpa,sem_credits,CONVERT(DATETIME, MONTH + ' ' + YEAR) AS combined_date FROM new_results_student WHERE regdno = '"+ userid + "' and semester='" + semester + "' ORDER BY combined_date DESC;";
                var query = @"SELECT top 1 semester,cgpa,sgpa,sem_credits,CONVERT(DATETIME, MONTH + ' ' + YEAR) AS combined_date FROM new_results_student WHERE regdno = '" + userid + "'  and STATUS NOT IN ('Dt','Dc','Exp') and process_type not in ('G','GV') and len(semester)<=1 order by semester desc";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<studenttrack>> getcourselist(string semester, string userid, string coursecode, string BATCH, string currentsem)
        {
            try
            {

                if (semester.Equals("1") && coursecode.Equals("BTECH") && BATCH.Contains("2019-"))
                {
                    var query = "";
                    query = "";
                    query = query + "select c.SUBJECT_NAME ,c.SUBJECT_CODE  ,MARKS   from DOE_MARKS_MASTER d, CBCS_SUBJECT_MASTER c where REGDNO = '" + userid + "' and d.SEMESTER = '" + semester + "' ";
                    query = query + " and d.CAMPUS = c.CAMPUS_CODE and d.COLLEGE = c.COLLEGE_CODE";
                    query = query + " and d.DEGREE = c.DEGREE_CODE and d.COURSE = c.COURSE_CODE";
                    query = query + " and d.BRANCH = c.BRANCH_CODE and d.SEMESTER = c.SEMESTER";
                    query = query + " and d.BATCH = c.BATCH and d.SUB_NAME = c.SUBJECT_NAME and";
                    query = query + " d.SUB_CODE = c.SUBJECT_CODE   order by c.SUBJECT_ORDER,c.SUBJECT_CODE";
                    return await Task.FromResult(GetAllData42<studenttrack>(query, null));
                }
                else
                {
                    string query1 = "select SUBJECT_NAME ,SUBJECT_CODE , int_mark  from STUDENT_EVALUATION_RESULTS  where  REGDNO='" + userid + "'  and SEMESTER='" + semester + "'  and SEMESTER!= '" + currentsem + "' order by subject_order ";
                    return await Task.FromResult(GetAllData19<studenttrack>(query1, null));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public async Task<IEnumerable<Timetable>> gettimetabledetails(string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH)
        //{
        //    try
        //    {
        //        var query = @"select WEEKDAY ,SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) as timeslots from TIME_TABLE_MASTER where  CAMPUS_CODE='" + CAMPUS_CODE + "' and COLLEGE_CODE='" + COLLEGE_CODE + "' and BRANCH_CODE='" + BRANCH_CODE + "' and SEMESTER='" + SEMESTER + "' and BATCH='" + BATCH + "' and SECTION='" + SECTION + "' ";
        //        return await Task.FromResult(GetAllData42<Timetable>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //       public async Task<IEnumerable<Timetable>> gettimetabledetails(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH)
        //       {
        //           try
        //           {
        //               //var query = @"select WEEKDAY ,SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
        //               //as timeslots from TIME_TABLE_MASTER where  CAMPUS_CODE='" + CAMPUS_CODE + "' and COLLEGE_CODE='" + COLLEGE_CODE + "' 
        //               //  and BRANCH_CODE='" + BRANCH_CODE + "' and SEMESTER='" + SEMESTER + "' and BATCH='" + BATCH + "' and SECTION='" + SECTION + "' ";
        //               string query = "";
        //               if (CAMPUS_CODE.ToUpper() == "BLR")
        //               {
        //                   query = @"select WEEKDAY ,t.SUBJECT_CODE + '#' + t.buildingname + '#' +t.ROOM_NO as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
        //as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
        //t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
        //h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
        //and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
        //and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '"+ regdno + "' ";
        //               }
        //               else if (CAMPUS_CODE.ToUpper() == "VSP")
        //               {
        //                   query = @"select WEEKDAY ,t.SUBJECT_CODE + '#' + t.buildingname + '#' +t.ROOM_NO as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
        //as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
        //t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
        //h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
        //and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
        //and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '"+ regdno + "' ";

        //               }
        //               else
        //               {

        //                   query = @"select WEEKDAY ,t.SUBJECT_CODE + '#' + t.buildingname + '#' +t.ROOM_NO as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
        //as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
        //t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
        //h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
        //and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
        //and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '"+ regdno + "' ";
        //               }

        //               return await Task.FromResult(GetAllData42<Timetable>(query, null));
        //           }
        //           catch (Exception ex)
        //           {
        //               throw ex;
        //           }
        //       }


        public async Task<IEnumerable<Timetable>> gettimetabledetails(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string course_code, string degreecode)
        {
            try
            {
                //var query = @"select WEEKDAY ,SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
                //as timeslots from TIME_TABLE_MASTER where  CAMPUS_CODE='" + CAMPUS_CODE + "' and COLLEGE_CODE='" + COLLEGE_CODE + "' 
                //  and BRANCH_CODE='" + BRANCH_CODE + "' and SEMESTER='" + SEMESTER + "' and BATCH='" + BATCH + "' and SECTION='" + SECTION + "' ";
                string query = "";

                /*      if(BRANCH_CODE.ToUpper()== "FSTCSE2023" || BRANCH_CODE.ToUpper() == "FSTNONCSE2023")
                      {
                          string branch_campus = "";
                          if (CAMPUS_CODE == "VSP")
                          {branch_campus = "VSPOE"; }
                          else if(CAMPUS_CODE=="BLR") { branch_campus = "BLROE"; } else if (CAMPUS_CODE == "HYD") { branch_campus = "HYDOE"; }
                          query = @"
        select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h  , STUDENT_MASTER sm where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION  and(t.branch_code='"+ branch_campus + "' " +
       "or t.branch_code='"+BRANCH_CODE+"') and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno+"' " +
       "and h.REGDNO=sm.REGDNO and sm.SEMESTER=h.SEMESTER and t.SEMESTER=sm.SEMESTER ";
                      }

                     else if (!(BRANCH_CODE.ToUpper() == "FSTCSE2023" || BRANCH_CODE.ToUpper() == "FSTNONCSE2023") && BATCH.Split('-')[0].Equals("2023"))
                      {
                          string branch_campus = "ECO";

                          query = @"
        select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h  , STUDENT_MASTER sm where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION  and(t.branch_code='" + branch_campus + "' " +
       "or t.branch_code='" + BRANCH_CODE + "') and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' " +
       "and h.REGDNO=sm.REGDNO and sm.SEMESTER=h.SEMESTER and t.SEMESTER=sm.SEMESTER ";
                      }
                      else
                      {
                          query = @"select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
       and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' ";
                      }

       */
                if (degreecode == "UG")
                {
                    //                    query = @"SELECT Weekdays,[08:00] as [08:00 to 08:50], [09:00] as [09:00 to 09:50] ,[10:00] as [10:00 to 10:50] ,
                    //[11:00] as [11:00 to 11:50] , [12:00] as [12:00 to 12:50] ,[13:00] as [13:00 to 13:50],[14:00] as 
                    //[14:00 to 14:50],[15:00] as [15:00  to 15:50],[16:00 ] as [16:00 to [16:50 ] FROM
                    //(
                    //  select WEEK_DAYS as Weekdays , substring( convert(varchar, FROM_TIME, 108),1,5)  as sub,subject_code  
                    //  from STUDENT_SLOT_MASTER_" + SEMESTER + "sem_backup_main  where regdno='" + regdno + "' and SEMESTER='" + SEMESTER + "')t PIVOT (max(subject_code) for sub IN([08:00], [09:00],[10:00], [11:00], [12:00], [13:00], [14:00], [15:00], [16:00 ])) AS pvt  ORDER BY  CASE  WHEN Weekdays = 'Monday' THEN 2 WHEN Weekdays = 'Tuesday' THEN 3 WHEN Weekdays = 'Wednesday' THEN 4 WHEN Weekdays = 'Thursday' THEN 5 WHEN Weekdays = 'Friday' THEN 6  WHEN Weekdays = 'Saturday' THEN 7 END ASC";
                    query = @"  select WEEK_DAYS as WEEKDAY ,subject_code,substring( convert(varchar, FROM_TIME, 108),1,5)  +' to '+ substring( convert(varchar, TO_TIME, 108),1,5) 
       as timeslots   from STUDENT_SLOT_MASTER_" + SEMESTER + "sem_backup_main  where regdno='" + regdno + "'  and SEMESTER='" + SEMESTER + "'";
                    IEnumerable<Timetable> data = await Task.FromResult(GetAllData42<Timetable>(query, null));

                    if (data.Count() == 0)
                    {
                        query = @"select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
       and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' and h.SEMESTER='" + SEMESTER + "'";
                    }
                    if (COLLEGE_CODE == "GSL")
                    {
                        query = @"select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
       and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' and h.SEMESTER='" + SEMESTER + "'";
                    }
                }
                else
                {
                    if (degreecode == "PG" && BRANCH_CODE == "MBA") { query = @"select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION 
       and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' and h.SEMESTER='" + SEMESTER + "'"; }
                    else
                    {
                        query = @"select WEEKDAY ,t.SUBJECT_CODE as subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
       as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
       t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
       h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
       and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
       and h.COURSE_CODE = t.COURSE_CODE  and h.REGDNO = '" + regdno + "' and h.SEMESTER='" + SEMESTER + "'";
                    }
                }
                return await Task.FromResult(GetAllData42<Timetable>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Timetable>> getslots(string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string COURSE_CODE, string degreecode)
        {
            var query = "";
            try
            {
                if (degreecode == "UG")
                {


                    query = @"select DISTINCT substring(convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) as timeslots   from TIME_TABLE_MASTER  where  COLLEGE_CODE='" + COLLEGE_CODE + "'  and CAMPUS_CODE='" + CAMPUS_CODE + "' and  SEMESTER='" + SEMESTER + "'  and BATCH='" + BATCH + "'  ";
                }

                else
                {
                    query = @"select DISTINCT substring(convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) as timeslots   from TIME_TABLE_MASTER  where  COLLEGE_CODE='" + COLLEGE_CODE + "'  and CAMPUS_CODE='" + CAMPUS_CODE + "' and  SEMESTER='" + SEMESTER + "'  and BRANCH_CODE='" + BRANCH_CODE + "' and BATCH='" + BATCH + "' and COURSE_CODE='" + COURSE_CODE + "' ";
                }

                return await Task.FromResult(GetAllData42<Timetable>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Timetable>> getsessions(string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string COURSE_CODE)
        {
            try
            {
                var query = @"select DISTINCT substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5)  
  from TIME_TABLE_MASTER where  COLLEGE_CODE='" + COLLEGE_CODE + "'  and CAMPUS_CODE='" + CAMPUS_CODE + "' and  SEMESTER='" + SEMESTER + "'  and BRANCH_CODE='" + BRANCH_CODE + "' and BATCH='" + BATCH + "' and COURSE_CODE='" + COURSE_CODE + "' ";
                return await Task.FromResult(GetAllData42<Timetable>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Feedback> inserthostelfeedback(Feedback _Parameter)
        {
            bool retnVal = false;
            try
            {
                int k = 0;
                for (int i = 0; i < 12; i++)
                {
                    k = i + 1;

                    _Parameter.QuestionID = Convert.ToString(k);
                    if (k == 1)
                    {
                        _Parameter.Status = _Parameter.Status1;
                        _Parameter.Remarks = _Parameter.Remarks1;
                    }
                    else if (k == 2)
                    {
                        _Parameter.Status = _Parameter.Status2;
                        _Parameter.Remarks = "";
                    }
                    else if (k == 3)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks3;
                    }
                    else if (k == 4)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks4;
                    }
                    else if (k == 5)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks5;
                    }
                    else if (k == 6)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks6;
                    }
                    else if (k == 7)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks7;
                    }
                    else if (k == 8)
                    {
                        _Parameter.Status = _Parameter.Status3;
                        _Parameter.Remarks = _Parameter.Remarks8;
                    }
                    else if (k == 9)
                    {
                        _Parameter.Status = _Parameter.Status4;
                        _Parameter.Remarks = _Parameter.Remarks9;
                    }
                    else if (k == 10)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks10;
                    }
                    else if (k == 11)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks11;
                    }
                    else if (k == 12)
                    {
                        _Parameter.Status = "";
                        _Parameter.Remarks = _Parameter.Remarks12;
                    }
                    var query = @"INSERT INTO Hostel_Feedback(REGDNO,NAME, QuestionID, Status, Remarks, campuscode, collegecode, gender, mobile, HOSTEL_BLOCK, DT_TIME) VALUES('" + _Parameter.REGDNO + "', '" + _Parameter.NAME + "', '" + _Parameter.QuestionID + "', '" + _Parameter.Status + "', '" + _Parameter.Remarks + "', '" + _Parameter.campuscode + "', '" + _Parameter.collegecode + "', '" + _Parameter.gender + "','" + _Parameter.mobile + "', '" + _Parameter.HOSTEL_BLOCK + "','" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.sss") + "')";
                    int j = await Task.FromResult(InsertData19hf(query, null));
                    if (j > 0)
                    {
                        retnVal = true;

                    }
                }





            }

            catch (Exception ex)
            {
                throw ex;
            }
            return _Parameter;
        }
        public async Task<List<Feedback>> registrationcheck(string REGDNO)
        {
            try
            {
                var query = @"Select * from student_master where regdno='" + REGDNO + "' and  HOSTLER='Y' and STATUS='S'";
                return await Task.FromResult(GetAllData<Feedback>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Feedback>> feedbackcheck(string REGDNO)
        {
            try
            {
                var query = @"Select * from Hostel_Feedback where regdno='" + REGDNO + "'";
                return await Task.FromResult(GetAllData<Feedback>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Publications>> Getpublications(string regdno)
        {
            try
            {
                var query = @"select Regdno,PUBLICATION_TITLE,JOURNAL_NAME,Approved_Status from GITAM_PUBLICATIONS_Scholar_MASTER where Regdno='" + regdno + "' and  Approved_Status in('A','N','D') ";
                return await Task.FromResult(GetAllData19hf<Publications>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Publications>> Getpublicationscheck(string regdno, string publication_number, string journal_title)
        {
            try
            {
                var query = "select * from GITAM_PUBLICATIONS_Scholar_MASTER where regdno='" + regdno + "' and (PUBLICATION_TITLE='" + publication_number + "' or JOURNAL_NAME='" + journal_title + "')";
                return await Task.FromResult(GetAllData19hf<Publications>(query, null));
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public async Task<Publications> Insertpublication(Publications chart)
        {
            try
            {
                int j = 0;
                string Query = "";

                Query = "insert into GITAM_PUBLICATIONS_Scholar_MASTER  (Regdno,SCHOLAR_NAME,DEPT_CODE,COLLEGE_CODE,CAMPUS_CODE,ACADEMIC_YEAR,DT_TIME,PUBLICATION_TITLE,JOURNAL_NAME,ISSUE_NO,ISSUE_YEAR,VOLUME,PUBLICATION_ABSTRACT,PUBLICATION_ARTICLE,DOI,hyperlink,indexed_by,citation_index,IMPACT_FACTOR,PG_NO,author_type,Approved_Status)" +
                  " values('" + chart.Regdno + "','" + chart.stuname + "','" + chart.dept_code + "','" + chart.college_code + "','" + chart.campus_code + "','" + chart.batch + "',GETDATE(),'" + chart.PUBLICATION_TITLE + "','" + chart.JOURNAL_NAME + "','" + chart.Issue_number + "','" + chart.Issue_Year + "','" + chart.Volume + "','" + chart.publication_abstract + "','" + chart.publication_article + "','" + chart.DOI_number + "','" + chart.Hyperlink + "','" + chart.Indexed_by + "','" + chart.Citation_index + "','" + chart.Impact_factor + "','" + chart.Page_numbers + "','" + chart.Author_type + "','N')";

                j = await Task.FromResult(InsertData19hf(Query, null));


                chart.flag = j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;


        }
        public async Task<int> Deletepublications(string regno, string pubtit, string jname)
        {
            try
            {
                int j = 0;
                string Query = "";
                Query = "delete GITAM_PUBLICATIONS_Scholar_MASTER where Regdno='" + regno + "' and JOURNAL_NAME='" + jname + "' and PUBLICATION_TITLE='" + pubtit + "'";
                j = await Task.FromResult(Delete19hf(Query, null));
                return j;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<StudentProfile>> Getstudentdetails(string username, string password)
        {
            string query = @"SELECT HOSTLER,REGDNO,parent_mobile,PINNO,ROLLNO,NAME,FATHER_NAME,DOB,GENDER,CATEGORY,ACADEMIC_YEAR_FROM,ACADEMIC_YEAR_TO,CURR_SEM,CGPA,ACADEMIC_BACKLOGS,DOORNO,LOCATION,CITY,STATE,COUNTRY,PINCODE,PHONE,MOBILE,EMAILID FROM STUDENT_MASTER where REGDNO='" + username + "' and parent_mobile ='" + password + "' ";
            return await Task.FromResult(GetAllData<StudentProfile>(query, new { }));


        }

        public async Task<IEnumerable<Students>> get_coordinator_dropdown(string campus)
        {
            try
            {

                // string query = "select empid,emp_name,mobile  from GHP_COORDINATORS  where campus='BLR'and empid in('93030','700434','700635','93310','700255')";
                string query = "select empid,emp_name,mobile from GHP_COORDINATORS  where hostel='1'  and level='Level-1' and campus='" + campus + "'";
                return await Task.FromResult(GetAllData<Students>(query, null));




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Students>> Getcoordinator_mobile(string campus, string empid)
        {
            try
            {

                string query = "select empid,emp_name,mobile from GHP_COORDINATORS  where  campus='" + campus + "' and empid='" + empid + "'";
                return await Task.FromResult(GetAllData<Students>(query, null));




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<StudentProfile>> getthesisasync(string user_id)
        {
            string query = @"select top 1 Regdno,NAME,THESIS_STATUS,CONVERT(varchar, DT_TIME, 106)  as dt_time from THESIS_STATUS_TRANSACTIONS where  regdno='" + user_id + "' order by id desc  ";
            return await Task.FromResult(GetAllData<StudentProfile>(query, new { }));
        }



        public async Task<IEnumerable<Buspass>> getstudentdetails(string user_id)
        {
            try
            {
                var query = @"select CAMPUS,COLLEGE_CODE,COURSE_CODE,BRANCH_CODE,regdno,NAME,CLASS,CURR_SEM,MOBILE as STUDENT_MOBILE,parent_mobile from student_master where regdno='" + user_id + "' ";
                return await Query<Buspass>(query, new { ID = user_id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Buspass>> Getbustypelist(string campus)
        {
            try
            {
                var query = @"select distinct bustype from ROUTE_FARE_MASTER where CAMPUS='" + campus + "' ";
                return await Task.FromResult(GetAllData60trans<Buspass>(query, new { ID = campus }));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Buspass>> getroutelist(string typeid, string campus_code)
        {
            try
            {

                var query = "select distinct BOARDING_POINT from ROUTE_FARE_MASTER where CAMPUS='" + campus_code + "' and bustype='" + typeid + "'";
                return await Task.FromResult(GetAllData60trans<Buspass>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Buspass>> getfareasync(string typeid, string campus_code, string Board)
        {
            try
            {

                var query = "select distinct FARE as buspassfee from ROUTE_FARE_MASTER where CAMPUS = '" + campus_code + "' and bustype = '" + typeid + "' and BOARDING_POINT='" + Board + "'";
                return await Task.FromResult(GetAllData60trans<Buspass>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Buspass> Insertbuspassasync(Buspass chart)
        {
            try
            {
                int j = 0;
                string Query = "";

                Query = "insert into PASSENGER_MASTER(REGDNO,NAME,ROUTE_ID,BOARDING_POINT,BRANCH,CAMPUS,CLASS,STUDENT_MOBILE,PARENT_MOBILE,type,application_status,applied_on_date,approved_by_empid,approved_on_date,BUSPASSYEAR,buspassfee,bustype,COLLEGE_CODE)" +
                            "values('" + chart.REGDNO + "','" + chart.NAME + "','" + chart.ROUTE_ID + "','" + chart.BOARDING_POINT + "','" + chart.BRANCH + "','" + chart.CAMPUS + "','" + chart.CLASS + "','" + chart.STUDENT_MOBILE + "','" + chart.PARENT_MOBILE + "','','SA',getdate(),'','','" + chart.BUSPASSYEAR + "','" + chart.buspassfee + "','" + chart.bustype + "','" + chart.COLLEGE_CODE + "')";

                j = await Task.FromResult(InsertData60trans(Query, null));



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;


        }
        public async Task<List<Buspass>> busregistrationcheck(string REGDNO, string BUSPASSYEAR)
        {
            try
            {
                var query = @"select REGDNO,NAME,CAMPUS,application_status,BUSTYPE,BUSPASSFEE,BOARDING_POINT from passenger_master where regdno='" + REGDNO + "' and BUSPASSYEAR='" + BUSPASSYEAR + "'";
                return await Task.FromResult(GetAllData60trans<Buspass>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Buspass>> busrapplicationstatuscheck(string user_id)
        {
            try
            {
                var query = @"select CAMPUS,COLLEGE_CODE,COURSE_CODE,BRANCH_CODE,regdno,NAME,CLASS,CURR_SEM,MOBILE as STUDENT_MOBILE,parent_mobile from student_master where regdno='" + user_id + "' ";
                return await Query<Buspass>(query, new { ID = user_id });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Insertemailpassword(string regdno, string name, string password, string mobile)
        {
            int j = 0;
            string Query = "";
            try
            {
                Query = "insert into email_password(regdno,name,password,mobileno) values('" + regdno + "','" + name + "','" + password + "','" + mobile + "')";
                j = await Task.FromResult(InsertData19gitamdb(Query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }


        public async Task<IEnumerable<Students>> fillfee(string regdno)
        {
            try
            {
                var query2 = "select Convert(VARCHAR(10),payment_date,103) as Date,total_fee  from fee_challan_master where regdno='" + regdno + "' and PAYMENT_STATUS='Y'  and FEE_TYPE='R' order by challan_date";

                return await Query<Students>(query2, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public async Task<IEnumerable<Students>> get_hosteldemand(string regdno)
        {
            double t1 = 0, t2 = 0, t3 = 0, t4 = 0;
            var query = "select isnull(sum(isnull(TOTAL_FEE, 0)),0) as total_fee from HOSTEL_FEE_CHALLAN_MASTER    where regdno = '" + regdno + "'  and    payment_date > '1-apr-2020' and PAYMENT_STATUS = 'y'";
            var dt1 = await Query<Students>(query, new { });


            if (dt1.Count() > 0)
            {

                t1 = Convert.ToDouble(dt1.ElementAt(0).total_fee);

            }
            var query1 = " select isnull(sum(isnull(ARREARS, 0) + isnull(Advanced_payment, 0)),0) as advanced_payment from HOSTEL_MESS_OPENING_balance  where regdno = '" + regdno + "'  and FIN_YEAR = '2020-2021'";
            var dt2 = await Query<Students>(query, new { });
            if (dt2.Count() > 0)
            {

                t2 = Convert.ToDouble(dt2.ElementAt(0).advanced_payment);


            }

            var query3 = "select isnull(sum(TOTAL_MESS_BILL),0) from HOSTEL_MESS_BILL  where regdno = '" + regdno + "' and FIN_YEAR = '2020-2021' and status = 's'";
            var dt3 = await Query<Students>(query, new { });
            if (dt3.Count() > 0)
            {

                t3 = Convert.ToDouble(dt2.ElementAt(0).TOTAL_MESS_BILL);


            }
            t4 = t2 + t3 - t1;
            if (t4 > 0)
            {
                dt2.ElementAt(0).TOTAL_MESS_BILL = Convert.ToString(t4);
            }
            else
            {
                dt2.ElementAt(0).TOTAL_MESS_BILL = "0";
            }
            return dt2;
        }



        public async Task<IEnumerable<Students>> getSeniorTutionFeeDemand(string regdno)
        {
            try
            {
                var query = "select isnull(fee_demand1,0) as fee_demand1,isnull(tution_fee_arrears,0) as tution_fee_arrears From STUDENT_MASTER where  REGDNO ='" + regdno + "'";

                return await Query<Students>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Security>> GetSecurityReports(string userid)
        {
            try
            {

                //var query = @"select max(p.REGDNO) as REGDNO, max(p.Name) as Name,REPLACE(CONVERT(VARCHAR(11),Permission_date, 106), ' ', '-') as Permission_date,p.Isapprove, max( CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEIN, 108) AS TIME), 100)) as TIMEIN,max( CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEOUT, 108) AS TIME), 100)) as TIMEOUT from STUDENT_INOUT_DETAILS s ,HostelPermission p  where  p.Regdno = s.REGDNO and s.Regdno='" + userid + "'group by s.REGDNO,p.Permission_date,p.Isapprove order by max(s.TIMEIN) desc";
                //var query = "select REPLACE(CONVERT(VARCHAR(11),DT_TIME, 106), ' ', '-') as Permission_date, max(CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEIN, 108) AS TIME), 100)) as TIMEIN,max(CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEOUT, 108) AS TIME), 100)) as TIMEOUT from STUDENT_INOUT_DETAILS s where s.Regdno = '" + userid + "' group by s.TIMEIN,s.TIMEOUT,DT_TIME order by max(s.TIMEIN) desc";
                var query = @"select distinct s.TIMEOUT as outtime,REPLACE(CONVERT(VARCHAR(11),s.TIMEIN, 106), ' ', '-') as 
CheckIn_date,max(p.REGDNO) as REGDNO, max(p.Name) as Name,REPLACE(CONVERT(VARCHAR(11),s.TIMEOUT, 106), ' ', '-') as 
Checkout_date,max( CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEOUT, 108) AS TIME), 100)) as TIMEOUT,
max( CONVERT(varchar(15), CAST(Convert(varchar(5), s.TIMEIN, 108) AS TIME), 100)) as TIMEIN from 
STUDENT_INOUT_DETAILS s ,HostelPermission p  where  p.Regdno = s.REGDNO and s.Regdno='" + userid + "' and Isapprove in ('L','I') group by s.REGDNO,p.Permission_date,s.TIMEOUT,TIMEIN order by s.TIMEOUT desc ";
                return await Query<Security>(query, new { });


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<DiningSubscription>> getstudentinfo(string student_id, string CAMPUS)
        {
            try
            {

                if (CAMPUS == "VSP")
                {
                    var query = @"select package_name,hostel_name,FORMAT (cast(start_date as date),'dd-MMM-yyyy') as start_date,FORMAT (cast(end_date as date),'dd-MMM-yyyy') as end_date from subscriptions where user_id='" + student_id + "'";
                    return await Task.FromResult(GetAllData74vsp<DiningSubscription>(query, null));

                }
                else if (CAMPUS == "BLR")
                {
                    var query = @"select package_name,hostel_name,FORMAT (cast(start_date as date),'dd-MMM-yyyy') as start_date,FORMAT (cast(end_date as date),'dd-MMM-yyyy') as end_date from subscriptions where user_id='" + student_id + "'";
                    return await Task.FromResult(GetAllData74blr<DiningSubscription>(query, null));

                }
                else
                {
                    var query = @"select package_name,hostel_name,FORMAT (cast(start_date as date),'dd-MMM-yyyy') as start_date,FORMAT (cast(end_date as date),'dd-MMM-yyyy') as end_date from subscriptions where user_id='" + student_id + "'";
                    return await Task.FromResult(GetAllData74<DiningSubscription>(query, null));

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<DiningSubscription>> getdininginfo(string user_id, string fromdate, string todate, string CAMPUS)
        {



            try
            {

                if (todate == null)
                {
                    todate = "";
                }
                //if (todate == "" && fromdate != "")
                //{
                //    fromdate = DateTime.Today.ToString("dd-MMM-yyyy");
                //}
                if (CAMPUS == "VSP")
                {

                    string sqlQuery174 = "";
                    if (todate == "" && fromdate == "")
                    {
                        // sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date>='" + fromdate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN DateAdd(DD,-6,GETDATE() ) and GETDATE()) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else if (todate == "" && fromdate != "")
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND DateAdd(DD,+1,'" + fromdate + "' )) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND '" + todate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    return await Task.FromResult(GetAllData74vsp<DiningSubscription>(sqlQuery174, null));

                }
                else if (CAMPUS == "BLR")
                {
                    string sqlQuery174 = "";
                    if (todate == "" && fromdate == "")
                    {
                        // sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date>='" + fromdate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN DateAdd(DD,-6,GETDATE() ) and GETDATE()) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else if (todate == "" && fromdate != "")
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND DateAdd(DD,+1,'" + fromdate + "' )) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND '" + todate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    return await Task.FromResult(GetAllData74blr<DiningSubscription>(sqlQuery174, null));

                }
                else
                {
                    string sqlQuery174 = "";
                    if (todate == "" && fromdate == "")
                    {
                        // sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date>='" + fromdate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN DateAdd(DD,-6,GETDATE() ) and GETDATE()) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else if (todate == "" && fromdate != "")
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND DateAdd(DD,+1,'" + fromdate + "' )) t PIVOT(count(user_id) FOR meal_type IN(breakfast, Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    else
                    {
                        sqlQuery174 = @"SELECT FORMAT (cast(used_date as date),'dd-MMM-yyyy') as used_date,max(breakfast) as breakfast ,max(lunch) as lunch ,max(snacks) as snacks,max(dinner) as dinner FROM (SELECT user_id,used_date,meal_type FROM subscription_usage p  where  user_id='" + user_id + "'  and used_date BETWEEN '" + fromdate + "' AND '" + todate + "') t PIVOT(count(user_id) FOR meal_type IN (breakfast , Lunch, Snacks, Dinner)) AS pivot_table group by cast(used_date as date)";

                    }
                    return await Task.FromResult(GetAllData74<DiningSubscription>(sqlQuery174, null));

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<residence>> Getresidenceroomdetails(string regdno)
        {
            try
            {
                var query = "select regdno,ROOM_NO,HOSTEL_BLOCK,FLOOR,CONVERT(varchar(19), date, 120) as date from HOSTEL_ROOM_TRANSFER where Regdno='" + regdno + "'  order by CONVERT(varchar(19), date, 120) desc";

                return await Query<residence>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<residence>> Getresidenceroomdetails19(string regdno)
        {
            try
            {
                var query = "select regdno,HOSTEL_BLOCK,HOSTEL_FLOOR as FLOOR ,HOSTEL_ROOMNO as ROOM_NO from STUDENT_MASTER where  hostler='Y' and Regdno='" + regdno + "' ";

                return await Query<residence>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<residence>> Getresidenceattendance(string regdno, string date)
        {
            var query = "";
            string fromdate = "";
            string todate = "";
            if (date == "Present")
            {
                DateTime nextdate1;
                nextdate1 = System.DateTime.Now.Date;

            }
            else if (date.Contains("to"))
            {
                string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
                fromdate = Convert.ToDateTime(dates[0]).ToString("yyyy-MM-dd");
                todate = Convert.ToDateTime(dates[1]).ToString("yyyy-MM-dd");
            }
            try
            {
                if (date == "Present")
                {
                    DateTime loaddate1;
                    DateTime loaddate2;
                    loaddate1 = System.DateTime.Now.Date;
                    loaddate2 = System.DateTime.Now.Date.AddDays(-6);
                    string nextdate1 = Convert.ToDateTime(loaddate1).ToString("yyyy-MM-dd");
                    string nextdate2 = Convert.ToDateTime(loaddate2).ToString("yyyy-MM-dd");

                    query = query + "";
                    //query = "select  EventDateTime ,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date,CAST(EventDateTime AS VARCHAR),103)= '" + Convert.ToDateTime(nextdate1).ToString("MM-dd-yyyy") + "'  order by CONVERT(date,CAST(EventDateTime AS VARCHAR),103) desc";
                    query = "select CONVERT(varchar, CONVERT(datetime, EventDateTime, 103), 105) + ' ' + RIGHT(EventDateTime, 8) AS EventDateTime ,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103) between '" + Convert.ToDateTime(nextdate2).ToString("MM-dd-yyyy") + "' and '" + Convert.ToDateTime(nextdate1).ToString("MM-dd-yyyy") + "'  order by CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103) desc";
                }
                else if (fromdate == "" && todate == "" && date != "Present")
                {
                    query = query + "";
                    query = "select  CONVERT(varchar, CONVERT(datetime, EventDateTime, 103), 105) + ' ' + RIGHT(EventDateTime, 8) AS EventDateTime,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103)= '" + Convert.ToDateTime(date).ToString("MM-dd-yyyy") + "'  order by CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103) desc";

                }
                else
                {
                    query = "select  CONVERT(varchar, CONVERT(datetime, EventDateTime, 103), 105) + ' ' + RIGHT(EventDateTime, 8) AS EventDateTime,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103) between '" + Convert.ToDateTime(fromdate).ToString("MM-dd-yyyy") + "' and '" + Convert.ToDateTime(todate).ToString("MM-dd-yyyy") + "' order by CONVERT(date, CONVERT(datetime, EventDateTime, 103), 103) desc";
                }
                return await Task.FromResult(GetAllData81<residence>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public async Task<IEnumerable<residence>> Getresidenceattendance(string regdno, string date)
        //{
        //    var query = "";
        //    string fromdate = "";
        //    string todate = "";
        //    if (date == "Present")
        //    {
        //        DateTime nextdate1;
        //        nextdate1 = System.DateTime.Now.Date;

        //    }
        //    else if (date.Contains("to"))
        //    {
        //        string[] dates = date.Split(new string[] { "to" }, StringSplitOptions.None);
        //        fromdate = Convert.ToDateTime(dates[0]).ToString("yyyy-MM-dd");
        //        todate = Convert.ToDateTime(dates[1]).ToString("yyyy-MM-dd");
        //    }
        //    try
        //    {
        //        if (date == "Present")
        //        {
        //            DateTime nextdate1;
        //            nextdate1 = System.DateTime.Now.Date;
        //            query = query + "";
        //            query = "select  EventDateTime ,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date,CAST(EventDateTime AS VARCHAR),103)= '" + Convert.ToDateTime(nextdate1).ToString("MM-dd-yyyy") + "'  order by CONVERT(date,CAST(EventDateTime AS VARCHAR),103) desc";
        //        }
        //        else if (fromdate == "" && todate == "" && date != "Present")
        //        {
        //            query = query + "";
        //            query = "select  EventDateTime ,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date,CAST(EventDateTime AS VARCHAR),103)= '" + Convert.ToDateTime(date).ToString("MM-dd-yyyy") + "'  order by CONVERT(date,CAST(EventDateTime AS VARCHAR),103) desc";

        //        }
        //        else
        //        {
        //            query = "select  EventDateTime ,device_name as Location  from Mx_VEW_APIUserAttendanceEvents where UserID='" + regdno + "' and CONVERT(date,CAST(EventDateTime AS VARCHAR),103) between '" + Convert.ToDateTime(fromdate).ToString("MM-dd-yyyy") + "' and '" + Convert.ToDateTime(todate).ToString("MM-dd-yyyy") + "' order by CONVERT(date,CAST(EventDateTime AS VARCHAR),103) desc";
        //        }
        //        return await Task.FromResult(GetAllData81<residence>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<Students> getoldPassword(string username, string type)
        {
            try
            {
                string query = @"SELECT PARENT_PASSWORD,NEW_PASSWORD FROM STUDENT_USER_MASTER where REGDNO='" + username + "' ";
                return await Task.FromResult(GetSingleData221gitam<Students>(query, new { }));
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public string ChangeUserPasswordAsync(string NPW, string USER_ID, string type, string plainpwd)
        {
            try
            {
                string query1 = "";
                if (type == "P")
                {
                    query1 = @"update student_user_master set PARENT_PASSWORD=@NPW where regdno=@USER_ID ";
                }
                else
                {
                    query1 = @"update student_user_master set new_password=@NPW where regdno=@USER_ID ";

                }
                var op = Update221gitam(query1, new { NPW = NPW, USER_ID = USER_ID });

                if (op > 0)
                {

                    string queryoldap = @"UPDATE OPENLDAP SET PASSWORD=EncryptByPassPhrase('cats',@NPW) where userid=@USER_ID";
                    var op1 = Update221gitam(queryoldap, new { NPW = NPW, USER_ID = USER_ID });


                    String newqry = removequotes("update student_master set new_password=" + plainpwd + " where regdno=" + USER_ID + "", "'", '#');

                    string IPADDRESS = GetIp();
                    string qry12 = "insert into GITAM_TRANSACTION_HISTORY(USERID,TXN_QUERY,TXN_TIME,PORTAL_ID,PORTAL_NAME,IP,TXN_TYPE) values('" + USER_ID + "','" + newqry + "',getdate(),'STUDENT','STUDENTPORTAL','" + IPADDRESS + "','CHANGE PASSWORD')";
                    int k = InsertData19gitamdb(qry12, null);


                    return "1";
                }
                else
                {
                    return "0";
                }

            }
            catch (Exception ex)
            {
                return "50";
            }


        }

        public string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }
        public string removequotes(string target, string samples, char replaceWith)
        {

            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(samples))
                return target;


            var tar = target.ToCharArray();

            for (var i = 0; i < tar.Length; i++)
            {
                for (var j = 0; j < samples.Length; j++)
                {

                    if (tar[i] == samples[j])
                    {
                        tar[i] = replaceWith;
                        break;
                    }
                }
            }

            return new string(tar);
        }


        public async Task<IEnumerable<Students>> getDetails(string regno)
        {
            try
            {
                var query = @"select regdno, password, college_code, campus, gender, degree_code, branch_code, batch, section, course_code, ACADEMIC_YEAR_FROM, name, CLASS, curr_sem, feedback_flag, emailid, mobile, dept_code, hostel_demand, hostler, row, HOSTEL_BLOCK, int_status, isnull(fine, 0) as fine,isnull(survey_flag, '') as 'SF',status,sem15_sgpa,txn_flag ,isnull(cgpa, '0') as cgpa,PREREQ,feedback,PREFERENCE_COURSE_1,PREFERENCE_COURSE_2,isnull(JOURNAL_DATE,'1-nov-2014') as JOURNAL_DATE  from student_master where regdno='" + regno + "' and status='S'";
                return await Task.FromResult(GetAllData42<Students>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public DataTable GetTimetable(string reg_no, string curr_sem)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("getstudenttimetable1semnew");
                using (SqlConnection con = new SqlConnection(_dapperconnection42))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.StoredProcedure;
                        sda.SelectCommand = cmd;
                        cmd.Parameters.Add("@regdno", SqlDbType.VarChar).Value = reg_no;
                        cmd.Parameters.Add("@semester", SqlDbType.VarChar).Value = curr_sem;


                        using (DataTable dt = new DataTable())
                        {
                            try
                            {
                                con.Open();
                                sda.Fill(dt);
                                return dt;
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                if (con.State == ConnectionState.Open)
                                    con.Close();
                            }




                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<Students>> getFacultyDetailsTimetable(string reg_no, string SEMESTER)
        {
            try
            {
                var query = @"select distinct REGDNO,NAME,SUBJECT_CODE,SUBJECT_NAME,SUBJECT_TYPE,employee_section,BRANCH_CODE,DEPT_CODE,COURSE_CODE,DEGREE_CODE,COLLEGE_CODE,CAMPUS_CODE,SEMESTER,max_int,MAX_EXT,BATCH,GROUP_CODE_HOD,CREDITS,EMPID,building_name,room_no,CBCS_CATEGORY,EMPNAME from  TBL_COURSE_SELECTED_CART_DATA_1sem  where regdno = '" + reg_no + "' and semester='" + SEMESTER + "' and registration_flag='Y' ";
                return await Task.FromResult(GetAllData42<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Feedback>> getstudentdetailsfb(string user_id)
        {
            try
            {
                var query = @"select sm.NAME,sm.parent_mobile,hf.REGDNO,hf.QuestionID,hf. Status, hf.Remarks, hf.campuscode, collegecode, hf.gender, hf.mobile, hf.HOSTEL_BLOCK,hf. DT_TIME from STUDENT_MASTER sm left join Hostel_Feedback hf on hf.REGDNO=sm.regdno where sm.regdno='" + user_id + "' ";
                return await Task.FromResult(GetAllData<Feedback>(query, new { ID = user_id }));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<student_displinary>> GetStudent_displinary(string regdno)
        {
            try
            {
                string query = "select ROW_NUMBER() OVER(ORDER BY REGDNO) Slno,COMMENT as [Comment],EMPID as [Employee_ID],EMPNAME as [Employee_name],convert(varchar, DTTIME, 105) as  date from STUDENT_COMMENTS where regdno = '" + regdno + "' ORDER BY REGDNO,DTTIME desc";
                return await Query<student_displinary>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int withdraw(string id)
        {

            try
            {
                string query = @"delete from hostelpermission where id='" + id + "'";
                return Delete19hf(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<IEnumerable<Students>> GetDistinctYears(string college_code, string campus_code, string dept_code, string CLASS)
        {
            try
            {

                var query = @"   select distinct year(START_DATE) AS year
								from NOTICE_CONTENT_MASTER  where COLLEGE_CODE in('" + college_code + "','All') and  CAMPUS in('" + campus_code + "', 'All') and DEPT_CODE in('" + dept_code + "', 'All') and student = 'Y' and  class in('" + CLASS + "','ALL') order by YEAR DESC";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<SportsScholarship>> getsportslist()
        {
            try
            {
                var query = "Select  ID as Sport_ID ,Sports_Name as Sports_Name from tbl_Sports_Master";
                return await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<SportsScholarship>> getlevellist()
        {
            try
            {
                var query = "Select distinct  ID as Level_ID ,Level_of_Competition  from tbl_Level_Competition where IsActive=1";
                return await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> getcompetetionnames(string levelid)
        {
            try
            {
                var query = "Select  ID  as Competition_ID,Name_of_Competition as Name_of_Competition from tbl_Competition_Names where LevelID='" + levelid + "'";
                return await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> getacheivementlist()
        {
            try
            {
                var query = "select Id as medal_id , Medal_name as medal_name from tbl_Medal_list";
                return await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<SportsScholarship>> semcheck(string REGDNO)
        {
            try
            {
                var query = @"select CURR_SEM as CurrSem,NAME as StudentName,EMAILID as StudentEmail,MOBILE as StudentMobile,CAMPUS from STUDENT_MASTER where REGDNO='" + REGDNO + "'";
                return await Task.FromResult(GetAllData<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<SportsScholarship> InsertSportsScholarship(SportsScholarship chart)
        {
            try
            {
                int j = 0;
                string Query = "";
                var semchck = @"select CURR_SEM as CURR_SEM,NAME as NAME,EMAILID as EMAILID,MOBILE as MOBILE,CAMPUS from STUDENT_MASTER where REGDNO='" + chart.RegID + "'";
                var data19 = await Task.FromResult(GetAllData<SportsScholarship>(semchck, null));
                if (data19.Count() > 0)
                {
                    chart.CURR_SEM = data19[0].CURR_SEM;
                    chart.NAME = data19[0].NAME;
                    chart.CAMPUS = data19[0].CAMPUS;
                    chart.MOBILE = data19[0].MOBILE;
                    chart.EMAILID = data19[0].EMAILID;


                    int pre_sem = 0;

                    if (chart.CURR_SEM == 1)
                    {
                        pre_sem = 1;
                    }

                    else
                    {
                        pre_sem = chart.CURR_SEM - 1;
                    }

                    var attend = @"select (cast(round((((select count(*) from STUDENT_ATTENDANCE_VSP where Regdno = s.Regdno and Semester=s.Semester and Status = 'P')*100.0)/ (select count(*) from STUDENT_ATTENDANCE_VSP where Regdno = s.Regdno and Semester=s.Semester)),2 ) as decimal(5,2))) as percentage from STUDENT_ATTENDANCE_VSP s where Regdno = '" + chart.RegID + "' and semester='" + pre_sem + "' group by regdno,Semester union select (cast(round((((select count(*) from STUDENT_ATTENDANCE_HYD where Regdno = s.Regdno and Semester=s.Semester and Status = 'P')*100.0)/ (select count(*) from STUDENT_ATTENDANCE_HYD where Regdno = s.Regdno and Semester=s.Semester)),2 ) as decimal(5,2))) as percentage from STUDENT_ATTENDANCE_HYD s where Regdno = '" + chart.RegID + "' and semester='" + pre_sem + "' group by regdno,Semester union select (cast(round((((select count(*) from STUDENT_ATTENDANCE_BLR where Regdno = s.Regdno and Semester=s.Semester and Status = 'P')*100.0)/ (select count(*) from STUDENT_ATTENDANCE_BLR where Regdno = s.Regdno and Semester=s.Semester)),2 ) as decimal(5,2))) as percentage from STUDENT_ATTENDANCE_BLR s where Regdno = '" + chart.RegID + "' and semester='" + pre_sem + "' group by regdno,Semester";
                    var data240 = await Task.FromResult(GetAllData240<SportsScholarship>(attend, null));
                    if (data240.Count() > 0)
                    {
                        chart.attendance = data240[0].Percentage;
                    }
                    else
                    {

                    }
                    var cgpa = @"select CGPA as CGPA from new_results_student where regdno='" + chart.RegID + "' and semester=" + pre_sem + "";
                    var data52 = await Task.FromResult(GetAllData52<SportsScholarship>(cgpa, null));
                    if (data52.Count() > 0)
                    {
                        chart.CGPA = data52[0].CGPA;
                    }
                    else
                    {

                    }
                    if ((chart.Sport == "12") && (chart.Competition_ID == "15") || (chart.Competition_ID == "16") || (chart.Competition_ID == "17") || (chart.Competition_ID == "18") || (chart.Competition_ID == "19") || (chart.Competition_ID == "20"))
                    {
                        var sql = "";

                        int bcount = 0;

                        if (chart.CURR_SEM == 1)
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,sports_name,Competition_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','0','0','" + chart.Sport + "','" + chart.Name_of_Competition + "')";

                            bcount = await Task.FromResult(InsertData99(sql, null));
                        }
                        else
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,Competition_name,sports_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','" + chart.attendance + "','" + chart.CGPA + "','" + chart.Name_of_Competition + "','" + chart.Sport + "')";

                            bcount = await Task.FromResult(InsertData99(sql, null));
                        }
                        chart.flag = bcount;
                    }
                    else if ((chart.Sport == "12") || ((chart.Competition_ID == "15") || (chart.Competition_ID == "16") || (chart.Competition_ID == "17") || (chart.Competition_ID == "18") || (chart.Competition_ID == "19") || (chart.Competition_ID == "20")))
                    {

                        var sql = "";
                        int bcount = 0;

                        if (chart.CURR_SEM == 1)
                        {

                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,Competition_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','0','0','" + chart.Name_of_Competition + "')";
                            bcount = await Task.FromResult(InsertData99(sql, null));

                        }
                        else
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,Competition_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','" + chart.attendance + "','" + chart.CGPA + "','" + chart.Name_of_Competition + "')";
                            bcount = await Task.FromResult(InsertData99(sql, null));

                        }

                        chart.flag = bcount;
                    }

                    else if ((chart.Sport == "12") || ((chart.Competition_ID != "15") || (chart.Competition_ID != "16") || (chart.Competition_ID != "17") || (chart.Competition_ID != "18") || (chart.Competition_ID != "19") || (chart.Competition_ID != "20")))
                    {

                        var sql = "";
                        int bcount = 0;

                        if (chart.CURR_SEM == 1)
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,sports_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','0','0','" + chart.Sport + "')";
                            bcount = await Task.FromResult(InsertData99(sql, null));


                        }
                        else
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA,sports_name)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','" + chart.attendance + "','" + chart.CGPA + "','" + chart.Sport + "')";
                            bcount = await Task.FromResult(InsertData99(sql, null));

                        }
                        chart.flag = bcount;
                    }


                    else
                    {


                        var sql = "";
                        int bcount = 0;

                        if (chart.CURR_SEM == 1)
                        {
                            sql = "";
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','0','0')";
                            bcount = await Task.FromResult(InsertData99(sql, null));

                        }
                        else
                        {
                            sql = "insert into tbl_Student_Sports_Data(Redgno,RegID,Sport_ID,Level_ID,Competition_ID,Medal,Sport_Certificate,sport_date,Trans_date,StudentName,Emailid,MobileNo,Attendance,CGPA)" +
                                "values('" + chart.RegID + "','" + chart.RegID + "','" + chart.Sport_ID + "','" + chart.Level_ID + "','" + chart.Competition_ID + "','" + chart.medal_id + "','" + chart.Sport_Certificate + "',CONVERT(DATE, '" + chart.sport_date + "', 105),getdate(),'" + chart.NAME + "','" + chart.EMAILID + "','" + chart.MOBILE + "','" + chart.attendance + "','" + chart.CGPA + "')";
                            bcount = await Task.FromResult(InsertData99(sql, null));

                        }
                        chart.flag = bcount;
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;



        }
        public async Task<IEnumerable<SportsScholarship>> getsportsscholargrid(string userid)
        {
            try
            {
                var query = "select  ROW_NUMBER() OVER(ORDER BY td.ID desc) as slno,td.ID,[Redgno],[Appno],[RegID],[Sport_ID],[Level_ID],[Competition_ID],[Medal],[Sport_Certificate],[Marks],[Trans_date],Convert(varchar(12),sport_date,105) AS sport_date,[status],[Level_Other_Remarks],[Competition_Other_Remarks],[StudentName],[Emailid],[MobileNo],[Attendance],[CGPA],[GAT_RANK],p.Sports_name as Sport,L.[Level_of_Competition] as Level_of_Competition,C.Name_of_Competition as Name_of_Competition,M.Medal_Name as Medal_Name,[remarks] ,td.sports_name,Competition_name,[sports_status] from tbl_Student_Sports_Data td left join tbl_Sports_Master p on p.ID=td.[Sport_ID]  left join tbl_Level_Competition L on L.ID=td.Level_ID left join tbl_Competition_Names C on C.ID=td.Competition_ID left join tbl_Medal_list M on M.ID=td.[Medal]where  Redgno='" + userid + "'";

                return await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<SportsScholarship> Updateremarks(SportsScholarship chart)
        {
            try
            {
                int j = 0;
                string Query = "";
                var semchck = @"SELECT top 1 ID FROM tbl_Student_Sports_Data where Redgno = '" + chart.RegID + "' order by Trans_date desc";
                var data226 = await Task.FromResult(GetAllData99<SportsScholarship>(semchck, null));
                if (data226.Count() > 0)
                {
                    chart.ID = data226[0].ID;
                }
                else
                {
                    chart.ID = "";
                }
                var remupdate = @"update tbl_Student_Sports_Data set remarks='" + chart.remarks + "' where Redgno = '" + chart.RegID + "' and ID='" + chart.ID + "'";

                j = await Task.FromResult(Update99(remupdate, null));
                if (j > 0)
                {
                    chart.flag = j;
                }

                else
                {
                    chart.flag = 0;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }

        public async Task<SportsScholarship> getsportsscholarship(SportsScholarship chart)
        {
            try
            {
                var query = "select ROW_NUMBER() OVER(ORDER BY t.Redgno desc) as slno,t.ID as ID,t.Sport_ID, t.Redgno as Redgno,case when m.Sports_Name = 'Others' then m.Sports_Name + '-' + t.sports_name when m.Sports_Name != 'Others' then m.Sports_Name END AS[Sports_Name]," +
                    "case when n.Name_of_Competition = 'Others' then n.Name_of_Competition + '-' + t.Competition_name when n.Name_of_Competition != 'Others' then n.Name_of_Competition END AS[Name_of_Competition],c.Level_of_Competition as [Level_of_Competition],n.Name_of_Competition AS[name Competition],l.Medal_Name as [medal_name],t.sport_date as [sport_date],t.Sport_Certificate as Sport_Certificate," +
                    "t.Trans_date as [Trans_date] from tbl_Student_Sports_Data t,STUDENT_MASTER s, tbl_Sports_Master m,tbl_Level_Competition c, tbl_Competition_Names n,tbl_Medal_list l where t.Redgno = s.REGDNO and t.Redgno = '" + chart.RegID + "' and m.ID = t.Sport_ID and t.Level_ID = c.ID and t.Competition_ID = n.ID and l.ID = t.Medal";
                var data52 = await Task.FromResult(GetAllData99<SportsScholarship>(query, null));
                if (data52.Count() > 0)
                {
                    chart.Sport_ID = data52[0].Sport_ID;

                    chart.slno = data52[0].slno;
                    chart.ID = data52[0].ID;
                    chart.Redgno = data52[0].Redgno;
                    chart.Sports_Name = data52[0].Sports_Name;
                    chart.Name_of_Competition = data52[0].Name_of_Competition;
                    chart.Level_of_Competition = data52[0].Level_of_Competition;
                    chart.medal_name = data52[0].medal_name;
                    chart.sport_date = data52[0].sport_date;
                    chart.Sport_Certificate = data52[0].Sport_Certificate;
                    chart.Trans_date = data52[0].Trans_date;
                }

                var query1 = "select APPLICATION_NO as APPLICATION_NO from GITAM.dbo.STUDENT_MASTER where REGDNO='" + chart.RegID + "'";
                var data19 = await Task.FromResult(GetAllData<SportsScholarship>(query1, null));
                if (data19.Count() > 0)
                {
                    chart.APPLICATION_NO = data19[0].APPLICATION_NO;
                }
                else
                {
                    chart.APPLICATION_NO = "";
                }
                var query2 = "select ROW_NUMBER() OVER(ORDER BY APPLICATIONNUMBER desc) as slno,NAME  as NAME,COURSE_DISPLAY as COURSE_DISPLAY,GAT_RANK as [GAT_RANK],total_percent as [total_percent],total_exam_type as [total_exam_type],total_Partion_Percent as [total_Partion_Percent] from TBLSCHOLARSHIP where APPLICATIONNUMBER='" + chart.APPLICATION_NO + "'";
                var data60 = await Task.FromResult(GetAllData60<SportsScholarship>(query2, null));

                if (data60.Count() > 0)
                {
                    chart.slno = data60[0].slno;
                    chart.NAME = data60[0].NAME;
                    chart.COURSE_DISPLAY = data60[0].COURSE_DISPLAY;
                    chart.GAT_RANK = data60[0].GAT_RANK;
                    chart.total_percent = data60[0].total_percent;
                    chart.total_exam_type = data60[0].total_exam_type;
                    chart.total_Partion_Percent = data60[0].total_Partion_Percent;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return chart;

        }
        public async Task<List<SportsScholarship>> getsportsschck(string REGDNO)
        {
            try
            {
                string qq1 = "select count(Sport_ID) as TotalSports from tbl_Student_Sports_Data where Redgno = '" + REGDNO + "'";
                return await Task.FromResult(GetAllData99<SportsScholarship>(qq1, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<SportsScholarship>> getsportsidchck(string REGDNO)
        {
            try
            {
                string qq1 = "select a.Sport_ID  from tbl_Student_Sports_Data a,tbl_Student_Sports_Data b where a.Redgno = b.Redgno and a.Sport_ID = b.Sport_ID and a.Redgno = '" + REGDNO + "' group by a.Sport_ID";
                return await Task.FromResult(GetAllData99<SportsScholarship>(qq1, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> Deletesportscholarship(string slno, string regdno)
        {
            try
            {
                int j = 0;
                string Query = "";
                Query = "delete tbl_Student_Sports_Data where Redgno='" + regdno + "' and ID='" + slno + "' ";
                j = await Task.FromResult(Delete99(Query, null));
                return j;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Students>> getAllhostlerpermissions(string uid)
        {
            try
            {

                string query = "select regdno,AccountNo,Fromdate,Todate,Reason,Isapprove,Approvedby,ParentMobile,ApprovedDate,AppliedDate,Deptcode,Collegecode,CampusCode,HostelCode,Type,Fromtime,Totime,biometricid,refno,Gender,Name,Permission_date as Permissiondate,hostler,Travelby,Destination,remarks,emp_name,fromdatetime,todatetime,Traveling_information,Warden_remarks,service_flag,level_status,confirmed_by,sms_status,genarated_by from HostelPermission where regdno='" + uid + "' ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Students>> filldashboardhostelpayment(string regdno, string pinno)
        {
            try
            {
                var query = "select top 5 row_number() over (order by payment_date desc) as slno,isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee  from HOSTEL_FEE_CHALLAN_MASTER where payment_status='Y' and regdno='" + regdno + "' and  PAYMENT_DATE>='1-apr-2018' order by payment_date desc";
                var data19 = await Task.FromResult(GetAllData<Students>(query, null));
                if (data19.Count() > 0)
                {
                    return await Query<Students>(query, new { });
                }
                else
                {
                    var query1 = "select top 5 row_number() over (order by payment_date desc) as slno,isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee  from hostel_tblFeeChallanMaster where payment_status='Y' and regdno='" + pinno + "' and  PAYMENT_DATE>='1-apr-2018' order by payment_date desc";
                    return await Query60<Students>(query1, new { });

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Students>> getdashboardSeniorTutionFeeDemand(string regdno, string pinno)
        {
            try
            {
                var query = "select top 5 row_number() over (order by challan_date desc) as slno,isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee  from fee_challan_master where regdno='" + regdno + "' and PAYMENT_STATUS='Y'  and FEE_TYPE='R' order by challan_date desc ";

                var data19 = await Task.FromResult(GetAllData<Students>(query, null));
                if (data19.Count() > 0)
                {
                    return await Query<Students>(query, new { });
                }
                else
                {
                    var query1 = "select top 5 row_number() over(order by challan_date desc) as slno,isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee from fee_challan_master where regdno = '" + regdno + "' and PAYMENT_STATUS = 'Y'  and FEE_TYPE = 'R' order by challan_date desc";
                    return await Query60<Students>(query1, new { });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<residence>> Getbiometricattendance_dashboard(string regdno)
        {
            var query = "";
            try
            {

                query = " SELECT DISTINCT TOP 5 CONVERT(date, CAST(EventDateTime AS VARCHAR), 103) AS EventDateTime, device_name AS Location FROM Mx_VEW_APIUserAttendanceEvents WHERE UserID = '" + regdno + "' ORDER BY EventDateTime DESC";
                //query = " SELECT DISTINCT TOP 5 CONVERT(date, CAST(EventDateTime AS VARCHAR), 103) AS EventDate, EventDateTime as EventDateTime, device_name AS Location FROM Mx_VEW_APIUserAttendanceEvents WHERE UserID = '" + regdno + "' ORDER BY EventDate DESC";
                return await Task.FromResult(GetAllData81<residence>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<Attendance>> Getcoursestructure(string college_code, string campus_code, string branch_code, string batch, string sem, string regdno)
        {
            try
            {
                var query1 = @"select  c.semester,c.SUBJECT_CODE as course_code,c.SUBJECT_NAME as subject,isnull(c.SUBJECT_type,'T') as type,section ,
isnull( convert(varchar, c.NEW_CREDITS) ,'-')  as sem_credits
from CBCS_STUDENT_SUBJECT_ASSIGn c 
where REGDNO='" + regdno + "' and c.SEMESTER='" + sem + "'  order by c.SUBJECT_CODE";
                return await Task.FromResult(GetAllData42<Attendance>(query1, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<Attendance>> Getcoursestructuremenu(string college_code, string campus_code, string branch_code, string batch, string sem, string regdno)
        {
            try
            {
                var query1 = @"select  c.semester,c.SUBJECT_CODE as course_code,c.SUBJECT_NAME as subject,isnull(c.SUBJECT_type,'T') as Subject_type,section ,c.CBCS_CATEGORY as type,isnull(c.passfail,'N') as passfail,
isnull( convert(varchar, c.NEW_CREDITS) ,'-')  as credits
from CBCS_STUDENT_SUBJECT_ASSIGn c 
where REGDNO='" + regdno + "'  order by c.SUBJECT_CODE";
                return await Task.FromResult(GetAllData42<Attendance>(query1, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<Students>> fillhostelpayment(string regdno)
        {
            try
            {
                //var query = "select Convert(VARCHAR(10),payment_date,103) as Date,total_fee,CHALLAN_NO  from HOSTEL_FEE_CHALLAN_MASTER where payment_status='Y' and regdno='" + regdno + "' and  PAYMENT_DATE>='1-apr-2018' order by payment_date desc";
                var query = "select isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee,CHALLAN_NO  from HOSTEL_FEE_CHALLAN_MASTER where payment_status='Y' and regdno='" + regdno + "' and  PAYMENT_DATE>='1-apr-2018' order by payment_date desc";

                return await Query<Students>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<SportsScholarship>> gettuitionfee(string userid)
        {
            try
            {
                var query = "Select isnull(REGDNO,'') as REGDNO,isnull(NAME,'') as name,isnull(CHALLAN_NO,'') as challan_no,isnull(REPLACE(CONVERT(NVARCHAR,PAYMENT_DATE, 106), ' ', '-'),'') as date,isnull(TOTAL_FEE,'') as total_fee, isnull(txn_id,'') as txn_id  from FEE_CHALLAN_MASTER  where FEE_TYPE='R' and PAYMENT_STATUS='Y' and REGDNO='" + userid + "' ORDER BY PAYMENT_DATE DESC";

                return await Task.FromResult(GetAllData19hf<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<SportsScholarship>> getotherfee(string userid)
        {
            try
            {
                var query = "select isnull(REGDNO,'') as REGDNO,isnull(NAME,'') as name,isnull(CHALLAN_NO,'') as challan_no,isnull(REPLACE(CONVERT(NVARCHAR,PAYMENT_DATE, 106), ' ', '-'),'') as date,isnull(TOTAL_FEE,'') as total_fee, isnull(txn_id,'') as txn_id,isnull(OTHERS1_DESC,'') as description  from FEE_CHALLAN_MASTER where  FEE_TYPE='MISC-FEE'  and PAYMENT_STATUS='Y' and REGDNO='" + userid + "' ORDER BY PAYMENT_DATE DESC";

                return await Task.FromResult(GetAllData19hf<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<SportsScholarship>> getevaulfee(string userid)
        {
            try
            {
                var query = "select  isnull(REGDNO,'') as REGDNO,isnull(NAME,'') as name,isnull(CHALLAN_NO,'') as challan_no,isnull(REPLACE(CONVERT(NVARCHAR,PAYMENT_DATE, 106), ' ', '-'),'') as date,isnull(TOTAL_FEE,'') as total_fee, isnull(txn_id,'') as txn_id,isnull(OTHERS3_DESC,'') as description  from  EXAMINATIONDB.dbo.FEE_CHALLAN_MASTER_DOE  where   REGDNO='" + userid + "' and PAYMENT_STATUS='Y' ORDER BY PAYMENT_DATE DESC";
                return await Task.FromResult(GetAllData19hf<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        public async Task<IEnumerable<SportsScholarship>> Student_Admissiondata(string APPLICATION_NO)
        {
            try
            {
                var query = "select ApplicationNumber as APPLICATION_NO,isnull(CONVERT(VARCHAR,allotment_DTtime, 105),'') as allotmentdate,isnull(admission_mode,'')as type,isnull(GAT_Rank,'') as GAT_RANK,CampusCode as CAMPUS,BranchCode as branch,DegreeCode as degree_code from tblRegistrationMaster where ApplicationNumber='" + APPLICATION_NO + "'";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Student_branchdata(string APPLICATION_NO)
        {
            try
            {
                var query = "select  REGISTRATION_NUMBER as REGDNO ,Campus1 as CAMPUS,course1 as COURSE_DISPLAY,isnull(CONVERT(VARCHAR,allocated_dt_time, 105),'') as allotmentdate from TBL_BRANCH_SELECTED  where REGISTRATION_NUMBER like '" + APPLICATION_NO + "%' order by DT_TIME";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Student_counseling(string APPLICATION_NO)
        {
            try
            {
                var query = "select REGISTRATION_NUMBER as REGDNO,Campus1  as CAMPUS,course1 as COURSE_DISPLAY,isnull(CONVERT(VARCHAR,allocated_dt_time, 105),'') as allotmentdate from TBL_BRANCH_SELECTED where REGISTRATION_NUMBER like '" + APPLICATION_NO + "%' and CAMPUS2='Y'";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Student_AdmPaymentdetails(string APPLICATION_NO)
        {
            try
            {
                var query = @"select Regdno as REGDNO,TOTAL_FEE as amount,TXN_ID as txn_id,PAYMENT_STATUS as status,CONVERT(VARCHAR,PAYMENT_DATE, 105) as paymenT_DATE from tblFeeChallanMaster where regdno like '" + APPLICATION_NO + "%' and PAYMENT_STATUs='Y' order by Regdno";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Student_Scholareligibility(string APPLICATION_NO)
        {
            try
            {
                var query = @"select Appno as APPLICATION_NO, Exam_Type as type,Scholar_Percentage as Percentage,isnull(Scholarshipflag, '') as PostedFlag from Scholorship_Que_Details where AppNo like '" + APPLICATION_NO + "%' order by Scholarshipflag";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Student_Scholarallocated(string APPLICATION_NO)
        {
            try
            {
                var query = @"select APPLICATIONNUMBER as applicantno,total_exam_type as type,total_Partion_Percent as percentage,Scholartype1,ScholarPer1,ScholarDeductAmount1,Scholartype2,ScholarPer2,ScholarDeductAmount2,Scholartype3,ScholarPer3,ScholarDeductAmount3,Scholartype4,ScholarPer4,ScholarDeductAmount4,Scholartype5,ScholarPer5,ScholarDeductAmount5,Scholartype6,ScholarPer6,ScholarDeductAmount6 from TBLSCHOLARSHIP where ApplicationNumber like '" + APPLICATION_NO + "%'";
                return await Task.FromResult(GetAllData60<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<SportsScholarship>> Coursesappliedandpayment(string APPLICATION_NO)
        {
            try
            {
                var query = @"select UserID as REGDNO,AppID as applicantno,u.EmailID as EMAILID,CourseID as COURSE_DISPLAY,ProgramName as NAME,MobileNo as MOBILE ,ispaymentdone as Status from tbl_CourseApplications tca join tbl_FetchCourses fc on tca.CourseID = fc.ID join tbl_Users u on u.Id=UserID and IsDeleted=0 and ispaymentdone in ('1','2') where AppID like '" + APPLICATION_NO + "%'";
                return await Task.FromResult(GetAllData35<SportsScholarship>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<Students>> fillhostelpaymentfees(string regdno)
        {
            try
            {
                var query = "select isnull(REPLACE(CONVERT(NVARCHAR,payment_date, 106), ' ', '-'),'') as Date,total_fee,CHALLAN_NO  from HOSTEL_FEE_CHALLAN_MASTER where payment_status='Y' and regdno='" + regdno + "' and  PAYMENT_DATE>='1-apr-2018' order by payment_date desc";

                return await Query<Students>(query, new { });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Contactus> contactusasync(Contactus chart)
        {
            try
            {
                var query = "select EMP_NAME as hod_EMP_NAME, MOBILE as hod_Mobile, EMAILID as hod_EMAILID,DESIGNATION from EMPLOYEE_MASTER where CAMPUS = '" + chart.CAMPUS + "' and COLLEGE_CODE ='" + chart.COLLEGE_CODE + "' and DEPT_CODE = '" + chart.DEPT_CODE + "'  and EMP_STATUS = 'A'  and DESIGNATION like '%-hod%'";

                var data19 = await Task.FromResult(GetAllData19hf<Contactus>(query, null));
                if (data19.Count() > 0)
                {
                    chart.hod_EMP_NAME = data19[0].hod_EMP_NAME;
                    chart.hod_Mobile = data19[0].hod_Mobile;
                    chart.hod_EMAILID = data19[0].hod_EMAILID;
                    chart.DESIGNATION = data19[0].DESIGNATION;
                }
                else
                {
                    chart.hod_EMP_NAME = "";
                    chart.hod_Mobile = "";
                    chart.hod_EMAILID = "";
                }
                var query2 = "select e.EMP_NAME as mentor_EMP_NAME,isnull(MOBILE,'') as mentor_Mobile,emailid  as mentor_EMAILID from Mentor_master m inner join employee_master e on e.EMPID=m.Emp_id  where  REGDNO='" + chart.REGDNO + "'";

                var data102 = await Task.FromResult(GetAllData102<Contactus>(query2, null));
                if (data102.Count() > 0)
                {
                    chart.mentor_EMP_NAME = data102[0].mentor_EMP_NAME;
                    chart.mentor_Mobile = data102[0].mentor_Mobile;
                    chart.mentor_EMAILID = data102[0].mentor_EMAILID;


                }
                else
                {
                    chart.mentor_EMP_NAME = "";
                    chart.mentor_Mobile = "";
                    chart.mentor_EMAILID = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return chart;
        }


        public async Task<IEnumerable<Students>> getdashboardtimetable(string regdno, string pinno, string day)
        {
            try
            {
                var sqlQuery = @"select   distinct t.CAMPUS_CODE,t.COLLEGE_CODE,t.SEMESTER,  h.REGDNO, CASE WHEN Weekday = 'Monday' THEN 2 WHEN Weekday = 'Tuesday' THEN 3
                WHEN Weekday = 'Wednesday' THEN 4 WHEN Weekday = 'Thursday' THEN 5 WHEN Weekday = 'Friday' THEN 6 WHEN Weekday = 'Saturday' 
                THEN 7 END,Convert(varchar(5), t.FROMTIME, 108),t.COLLEGE_CODE,t.SEMESTER, h.REGDNO as regdno,t.weekday as weekday,Convert(varchar(5), t.FROMTIME, 108) as FROM_TIME ,Convert(varchar(5), t.TOTIME, 108) as TO_TIME,
                t.SUBJECT_CODE as SUBJECT_CODE,t.SUBJECT_NAME as SUBJECT_NAME,
                ( h.SUBJECT_CODE COLLATE Latin1_General_CI_AS + ' :: ' + h.SECTION + ' :: ' + convert(varchar, h.SEMESTER) + ' :: ' + h.BRANCH_CODE +  h.SUBJECT_NAME + '') as description
                from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where 
                 t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and
                h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE
                and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.GROUP_SECTION and h.GROUP_BRANCH = t.BRANCH_CODE 
                and h.COURSE_CODE = t.COURSE_CODE and t.SEMESTER in(1, 3, 5, 7, 9) and h.REGDNO = '" + regdno + "'";
                if (day != null)
                {
                    sqlQuery += "  and WEEKDAY='" + day + "' ";
                }

                sqlQuery += " order by  h.REGDNO, CASE WHEN Weekday = 'Monday' THEN 2 WHEN Weekday = 'Tuesday' THEN 3 WHEN Weekday = 'Wednesday' THEN 4 WHEN Weekday = 'Thursday' THEN 5 WHEN Weekday = 'Friday' THEN 6 WHEN Weekday = 'Saturday' THEN 7 END,Convert(varchar(5), t.FROMTIME, 108) ";
                return await Task.FromResult(GetAllData42<Students>(sqlQuery, null));


                return await Task.FromResult(GetAllData42<Students>(sqlQuery, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Contactus>> getmentor_remarksdata(string regdno)
        {
            try
            {
                var query = "select id,regdno as REGDNO,remarks as mentor_remarks,format(Generated_time,'dd-MMM-yyyy hh:mm tt') as time,user_id,user_name from AMC_COUNSELLOR_MASTER where Regdno='" + regdno + "' order by format(Generated_time,'dd-MMM-yyyy hh:mm tt') desc ";
                return await Task.FromResult(GetAllData102<Contactus>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<studenttrack>> getresults(string reg, string sem, string college_code)
        {
            try
            {
                // var query = "select id,regdno as REGDNO,remarks as mentor_remarks,format(Generated_time,'dd-MMM-yyyy hh:mm tt') as time,user_id,user_name from AMC_COUNSELLOR_MASTER where Regdno='" + regdno + "' order by format(Generated_time,'dd-MMM-yyyy hh:mm tt') desc ";
                string sql = "";

                if (college_code.Equals("CDL"))
                {
                    sql = "SELECT SS.college_code,S.STATUS AS STATUS,S.REGDNO as REGDNO,S.NAME AS NAME,S.BRANCH_NAME AS BRANCH_NAME,S.COURSE_NAME AS COURSE_NAME,'SEMESTER'=case s.semester when 1 then 'I' when 2 then 'II' ";
                    sql = sql + "when 3 then 'III' when 4 then 'IV'  when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 ";
                    sql = sql + "then 'VIII'   when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII'   when 13 ";
                    sql = sql + " then 'XIII' when 14 then 'XIV' when 15 then 'XV' END ";
                    sql = sql + " ,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER='" + sem + "') AS SEM_CREDITS,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER <= '" + sem + "') AS CUM_CREDITS,S.SGPA AS SGPA,S.CGPA AS CGPA, ";
                    sql = sql + " SS.SUBJECT_CODE AS SUBJECT_CODE,'SUBJECT_NAME' = CASE WHEN SS.SUBJECT_CATEGORY = 'ELE' THEN 'ELECTIVE: '+SS.SUBJECT_NAME ELSE ";
                    sql = sql + " SS.SUBJECT_NAME END,SS.SUBJECT_CREDITS AS CREDITS,SS.SUBJECT_GRADE AS GRADE,'SUBJECT_TYPE' = CASE WHEN SS.SUBJECT_TYPE = 'T' THEN ";
                    sql = sql + " 'THEORY:' ELSE  'PRACTICALS:' END,SS.SUBJECT_CATEGORY,S.MONTH AS MONTH,S.YEAR AS YEAR ";
                    sql = sql + " FROM NEW_RESULTS_STUDENT S,NEW_RESULTS_SUBJECT SS WHERE S.REGDNO = SS.REGDNO AND S.SEMESTER = SS.SEMESTER AND ";
                    sql = sql + " S.REGDNO = '" + reg + "' AND SS.SEMESTER='" + sem + "' and s.status = 'A' AND S.COLLEGE_CODE = 'CDL' ORDER BY SS.SUBJECT_ORDER";
                }
                else
                {
                    sql = "SELECT SS.college_code,S.STATUS AS STATUS,S.REGDNO as REGDNO,S.NAME AS NAME,S.BRANCH_NAME AS BRANCH_NAME,S.COURSE_NAME AS COURSE_NAME,'SEMESTER'=case s.semester when 1 then 'I' when 2 then 'II' ";
                    sql = sql + "when 3 then 'III' when 4 then 'IV'  when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 ";
                    sql = sql + "then 'VIII'   when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII'   when 13 ";
                    sql = sql + " then 'XIII' when 14 then 'XIV' when 15 then 'XV' END ";
                    sql = sql + " ,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER='" + sem + "') AS SEM_CREDITS,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER <= '" + sem + "') AS CUM_CREDITS,S.SGPA AS SGPA,S.CGPA AS CGPA, ";
                    sql = sql + " SS.SUBJECT_CODE AS SUBJECT_CODE,'SUBJECT_NAME' = CASE WHEN SS.SUBJECT_CATEGORY = 'ELE' THEN 'ELECTIVE: '+SS.SUBJECT_NAME ELSE ";
                    sql = sql + " SS.SUBJECT_NAME END,SS.SUBJECT_CREDITS AS CREDITS,SS.SUBJECT_GRADE AS GRADE,'SUBJECT_TYPE' = CASE WHEN SS.SUBJECT_TYPE = 'T' THEN ";
                    sql = sql + " 'THEORY:' ELSE  'PRACTICALS:' END,SS.SUBJECT_CATEGORY,S.MONTH AS MONTH,S.YEAR AS YEAR ";
                    sql = sql + " FROM NEW_RESULTS_STUDENT S,NEW_RESULTS_SUBJECT SS WHERE S.REGDNO = SS.REGDNO AND S.SEMESTER = SS.SEMESTER AND ";
                    sql = sql + " S.REGDNO = '" + reg + "' AND SS.SEMESTER='" + sem + "' and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  ORDER BY SS.SUBJECT_ORDER";

                }
                return await Task.FromResult(GetAllData52<studenttrack>(sql, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<List<studenttrack>> getmonth(string semester, string userid, string process)
        {
            try
            {
                var query = @"select distinct month as Month,year as Year from new_results_student1 where regdno = '" + userid + "' and semester = '" + semester + "' and Process_type='" + process + "'and  STATUS NOT IN ('Dt','Dc','Exp') order by month";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getmonthst2(string semester, string userid, string process)
        {
            try
            {
                //var query = @"select distinct month as Month,year as Year from new_results_student1 where regdno = '" + userid + "' and semester = '" + semester + "' and Process_type='" + process + "'and  STATUS NOT IN ('Dt','Dc','Exp') order by month";
                var query = @"select distinct month as Month,year as Year from [NEW_RESULTS_STUDENT1] where regdno = '" + userid + "'  and month='Jun' and year='2024' order by year desc ";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getcarddetails(string userid)
        {
            try
            {
                //var query = @"select REGDNO,NAME,SECTION,SEMESTER,BRANCH,case when BRANCH_NAME='Optometry' then 'B.Optometry' else  BRANCH_NAME end BRANCH_NAME,COURSE = case course when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSC' THEN 'B.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)' when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'  when 'BCOM ' THEN 'B.Com.(Hons.)' when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else course end, COURSE_NAME, COURSE_TITLE, DEGREE, COLLEGE, CAMPUS, SGPA , CGPA , EXAM_DATE, PRINT_YEAR, EXAM_NAME, TOTAL_SEM_CREDITS, CUM_SEM_CREDITS, SUB1_CODE, SUB2_CODE, SUB3_CODE, SUB4_CODE, SUB5_CODE, SUB6_CODE, SUB7_CODE, SUB8_CODE, SUB9_CODE, SUB10_CODE,SUB11_CODE, SUB12_CODE, SUB13_CODE, SUB14_CODE, SUB15_CODE, SUB16_CODE, SUB1_GRADE = case SUB1_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB1_GRADE end, SUB2_GRADE = case SUB2_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB2_GRADE end, SUB3_GRADE = case SUB3_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB3_GRADE end, SUB4_GRADE = case SUB4_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB4_GRADE end, SUB5_GRADE = case SUB5_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else  SUB5_GRADE end, SUB6_GRADE = case SUB6_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB6_GRADE end, SUB7_GRADE = case SUB7_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB7_GRADE end, SUB8_GRADE = case SUB8_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB8_GRADE end, SUB9_GRADE  = case SUB9_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB9_GRADE end, SUB10_GRADE = case SUB10_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB10_GRADE end, SUB11_GRADE =case SUB11_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB11_GRADE end, SUB12_GRADE =case SUB12_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB12_GRADE end, SUB13_GRADE =case SUB13_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB13_GRADE end, SUB14_GRADE =case SUB14_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB14_GRADE end, SUB15_GRADE =case SUB15_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB15_GRADE end, SUB16_GRADE =case SUB16_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB16_GRADE end, SUB1_NAME, SUB2_NAME,SUB3_NAME, SUB4_NAME, SUB5_NAME, SUB6_NAME, SUB7_NAME, SUB8_NAME, SUB9_NAME, SUB10_NAME,SUB11_NAME, SUB12_NAME, SUB13_NAME, SUB14_NAME, SUB15_NAME, SUB16_NAME, SUB1_CREDITS, SUB2_CREDITS,SUB3_CREDITS, SUB4_CREDITS, SUB5_CREDITS, SUB6_CREDITS, SUB7_CREDITS, SUB8_CREDITS, SUB9_CREDITS,SUB10_CREDITS, SUB11_CREDITS, SUB12_CREDITS, SUB13_CREDITS, SUB14_CREDITS, SUB15_CREDITS,SUB16_CREDITS, roman_semester, year, month =case month when 'jan' THEN 'January' when 'feb' THEN 'February' when 'mar' THEN 'March' when 'Apr' THEN 'April' when 'may' THEN 'May' when 'jun' THEN 'June' when 'jul' THEN 'July' when 'aug' THEN 'August' when 'sep' THEN 'September' when 'oct' THEN 'October' when 'nov' THEN 'November' when 'dec' THEN 'December' else month end,CAST(SGPA AS DECIMAL(10,2))sgpa1,CAST(CGPA AS DECIMAL(10,2)) cgpa1,month as month,process_type,DT_Completion FROM NEW_GRADE_REPORT  where regdno = '" + userid + "' ";
                var query = @"select REGDNO,NAME,SECTION,SEMESTER,BRANCH,case when BRANCH_NAME='Optometry' then 'B.Optometry' else  BRANCH_NAME end AS Branch_name,COURSE = case course when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSC' THEN 'B.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)' when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'  when 'BCOM ' THEN 'B.Com.(Hons.)' when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else course end, COURSE_NAME, COURSE_TITLE, DEGREE, COLLEGE, CAMPUS, SGPA , CGPA , EXAM_DATE, PRINT_YEAR, EXAM_NAME, TOTAL_SEM_CREDITS, CUM_SEM_CREDITS, SUB1_CODE, SUB2_CODE, SUB3_CODE, SUB4_CODE, SUB5_CODE, SUB6_CODE, SUB7_CODE, SUB8_CODE, SUB9_CODE, SUB10_CODE,SUB11_CODE, SUB12_CODE, SUB13_CODE, SUB14_CODE, SUB15_CODE, SUB16_CODE, SUB1_GRADE = case SUB1_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB1_GRADE end, SUB2_GRADE = case SUB2_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB2_GRADE end, SUB3_GRADE = case SUB3_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB3_GRADE end, SUB4_GRADE = case SUB4_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB4_GRADE end, SUB5_GRADE = case SUB5_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else  SUB5_GRADE end, SUB6_GRADE = case SUB6_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB6_GRADE end, SUB7_GRADE = case SUB7_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB7_GRADE end, SUB8_GRADE = case SUB8_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB8_GRADE end, SUB9_GRADE  = case SUB9_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB9_GRADE end, SUB10_GRADE = case SUB10_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB10_GRADE end, SUB11_GRADE =case SUB11_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB11_GRADE end, SUB12_GRADE =case SUB12_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB12_GRADE end, SUB13_GRADE =case SUB13_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB13_GRADE end, SUB14_GRADE =case SUB14_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB14_GRADE end, SUB15_GRADE =case SUB15_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB15_GRADE end, SUB16_GRADE =case SUB16_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB16_GRADE end, SUB1_NAME, SUB2_NAME,SUB3_NAME, SUB4_NAME, SUB5_NAME, SUB6_NAME, SUB7_NAME, SUB8_NAME, SUB9_NAME, SUB10_NAME,SUB11_NAME, SUB12_NAME, SUB13_NAME, SUB14_NAME, SUB15_NAME, SUB16_NAME, SUB1_CREDITS, SUB2_CREDITS,SUB3_CREDITS, SUB4_CREDITS, SUB5_CREDITS, SUB6_CREDITS, SUB7_CREDITS, SUB8_CREDITS, SUB9_CREDITS,SUB10_CREDITS, SUB11_CREDITS, SUB12_CREDITS, SUB13_CREDITS, SUB14_CREDITS, SUB15_CREDITS,SUB16_CREDITS, roman_semester, year AS Year, month =case month when 'jan' THEN 'January' when 'feb' THEN 'February' when 'mar' THEN 'March' when 'Apr' THEN 'April' when 'may' THEN 'May' when 'jun' THEN 'June' when 'jul' THEN 'July' when 'aug' THEN 'August' when 'sep' THEN 'September' when 'oct' THEN 'October' when 'nov' THEN 'November' when 'dec' THEN 'December' else month end,CAST(SGPA AS DECIMAL(10,2))sgpa1,CAST(CGPA AS DECIMAL(10,2)) cgpa1,month as emonth,process_type,DT_Completion as date_complete FROM NEW_GRADE_REPORT  where regdno = '" + userid + "' ";
                return await Task.FromResult(GetAllData25<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<studenttrack>> getStudentcarddetails(string semester, string userid)
        {
            try
            {
                //var query = @"select REGDNO,NAME,SECTION,SEMESTER,BRANCH,case when BRANCH_NAME='Optometry' then 'B.Optometry' else  BRANCH_NAME end BRANCH_NAME,COURSE = case course when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSC' THEN 'B.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)' when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'  when 'BCOM ' THEN 'B.Com.(Hons.)' when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else course end, COURSE_NAME, COURSE_TITLE, DEGREE, COLLEGE, CAMPUS, SGPA , CGPA , EXAM_DATE, PRINT_YEAR, EXAM_NAME, TOTAL_SEM_CREDITS, CUM_SEM_CREDITS, SUB1_CODE, SUB2_CODE, SUB3_CODE, SUB4_CODE, SUB5_CODE, SUB6_CODE, SUB7_CODE, SUB8_CODE, SUB9_CODE, SUB10_CODE,SUB11_CODE, SUB12_CODE, SUB13_CODE, SUB14_CODE, SUB15_CODE, SUB16_CODE, SUB1_GRADE = case SUB1_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB1_GRADE end, SUB2_GRADE = case SUB2_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB2_GRADE end, SUB3_GRADE = case SUB3_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB3_GRADE end, SUB4_GRADE = case SUB4_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB4_GRADE end, SUB5_GRADE = case SUB5_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else  SUB5_GRADE end, SUB6_GRADE = case SUB6_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB6_GRADE end, SUB7_GRADE = case SUB7_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB7_GRADE end, SUB8_GRADE = case SUB8_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB8_GRADE end, SUB9_GRADE  = case SUB9_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB9_GRADE end, SUB10_GRADE = case SUB10_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB10_GRADE end, SUB11_GRADE =case SUB11_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB11_GRADE end, SUB12_GRADE =case SUB12_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB12_GRADE end, SUB13_GRADE =case SUB13_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB13_GRADE end, SUB14_GRADE =case SUB14_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB14_GRADE end, SUB15_GRADE =case SUB15_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB15_GRADE end, SUB16_GRADE =case SUB16_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB16_GRADE end, SUB1_NAME, SUB2_NAME,SUB3_NAME, SUB4_NAME, SUB5_NAME, SUB6_NAME, SUB7_NAME, SUB8_NAME, SUB9_NAME, SUB10_NAME,SUB11_NAME, SUB12_NAME, SUB13_NAME, SUB14_NAME, SUB15_NAME, SUB16_NAME, SUB1_CREDITS, SUB2_CREDITS,SUB3_CREDITS, SUB4_CREDITS, SUB5_CREDITS, SUB6_CREDITS, SUB7_CREDITS, SUB8_CREDITS, SUB9_CREDITS,SUB10_CREDITS, SUB11_CREDITS, SUB12_CREDITS, SUB13_CREDITS, SUB14_CREDITS, SUB15_CREDITS,SUB16_CREDITS, roman_semester, year, month =case month when 'jan' THEN 'January' when 'feb' THEN 'February' when 'mar' THEN 'March' when 'Apr' THEN 'April' when 'may' THEN 'May' when 'jun' THEN 'June' when 'jul' THEN 'July' when 'aug' THEN 'August' when 'sep' THEN 'September' when 'oct' THEN 'October' when 'nov' THEN 'November' when 'dec' THEN 'December' else month end,CAST(SGPA AS DECIMAL(10,2))sgpa1,CAST(CGPA AS DECIMAL(10,2)) cgpa1,month as month,process_type,DT_Completion FROM NEW_GRADE_REPORT  where regdno = '" + userid + "' ";
                var query = @"select CAMPUS_CODE,COLLEGE_CODE as college_code,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,SEMESTER,BATCH,SECTION,MONTH,YEAR from [NEW_RESULTS_STUDENT1] where regdno = '" + userid + "' and semester='" + semester + "'  order by year desc ";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getStudentcarddetailsst2(string semester, string userid)
        {
            try
            {
                //var query = @"select REGDNO,NAME,SECTION,SEMESTER,BRANCH,case when BRANCH_NAME='Optometry' then 'B.Optometry' else  BRANCH_NAME end BRANCH_NAME,COURSE = case course when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSC' THEN 'B.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)' when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'  when 'BCOM ' THEN 'B.Com.(Hons.)' when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else course end, COURSE_NAME, COURSE_TITLE, DEGREE, COLLEGE, CAMPUS, SGPA , CGPA , EXAM_DATE, PRINT_YEAR, EXAM_NAME, TOTAL_SEM_CREDITS, CUM_SEM_CREDITS, SUB1_CODE, SUB2_CODE, SUB3_CODE, SUB4_CODE, SUB5_CODE, SUB6_CODE, SUB7_CODE, SUB8_CODE, SUB9_CODE, SUB10_CODE,SUB11_CODE, SUB12_CODE, SUB13_CODE, SUB14_CODE, SUB15_CODE, SUB16_CODE, SUB1_GRADE = case SUB1_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB1_GRADE end, SUB2_GRADE = case SUB2_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB2_GRADE end, SUB3_GRADE = case SUB3_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB3_GRADE end, SUB4_GRADE = case SUB4_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB4_GRADE end, SUB5_GRADE = case SUB5_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else  SUB5_GRADE end, SUB6_GRADE = case SUB6_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB6_GRADE end, SUB7_GRADE = case SUB7_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB7_GRADE end, SUB8_GRADE = case SUB8_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB8_GRADE end, SUB9_GRADE  = case SUB9_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB9_GRADE end, SUB10_GRADE = case SUB10_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB10_GRADE end, SUB11_GRADE =case SUB11_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB11_GRADE end, SUB12_GRADE =case SUB12_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB12_GRADE end, SUB13_GRADE =case SUB13_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB13_GRADE end, SUB14_GRADE =case SUB14_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB14_GRADE end, SUB15_GRADE =case SUB15_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB15_GRADE end, SUB16_GRADE =case SUB16_GRADE when '*F' THEN 'F' when 'F' THEN 'F' else SUB16_GRADE end, SUB1_NAME, SUB2_NAME,SUB3_NAME, SUB4_NAME, SUB5_NAME, SUB6_NAME, SUB7_NAME, SUB8_NAME, SUB9_NAME, SUB10_NAME,SUB11_NAME, SUB12_NAME, SUB13_NAME, SUB14_NAME, SUB15_NAME, SUB16_NAME, SUB1_CREDITS, SUB2_CREDITS,SUB3_CREDITS, SUB4_CREDITS, SUB5_CREDITS, SUB6_CREDITS, SUB7_CREDITS, SUB8_CREDITS, SUB9_CREDITS,SUB10_CREDITS, SUB11_CREDITS, SUB12_CREDITS, SUB13_CREDITS, SUB14_CREDITS, SUB15_CREDITS,SUB16_CREDITS, roman_semester, year, month =case month when 'jan' THEN 'January' when 'feb' THEN 'February' when 'mar' THEN 'March' when 'Apr' THEN 'April' when 'may' THEN 'May' when 'jun' THEN 'June' when 'jul' THEN 'July' when 'aug' THEN 'August' when 'sep' THEN 'September' when 'oct' THEN 'October' when 'nov' THEN 'November' when 'dec' THEN 'December' else month end,CAST(SGPA AS DECIMAL(10,2))sgpa1,CAST(CGPA AS DECIMAL(10,2)) cgpa1,month as month,process_type,DT_Completion FROM NEW_GRADE_REPORT  where regdno = '" + userid + "' ";
                // var query = @"select CAMPUS_CODE,COLLEGE_CODE as college_code,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,SEMESTER,BATCH,SECTION,MONTH,YEAR from [NEW_RESULTS_STUDENT1] where regdno = '" + userid + "' and semester='" + semester + "'  order by year desc ";
                var query = @"select CAMPUS_CODE,COLLEGE_CODE as college_code,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,SEMESTER,BATCH,SECTION,MONTH,YEAR
from [NEW_RESULTS_STUDENT1] where regdno = '" + userid + "' and month='Jun' and year='2024'order by year desc ";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<int> DeleteDetails(string campus, string college, string course, string branch, string batch)
        {
            int j = 0;
            try
            {
                var query = @"DELETE FROM NEW_GRADE_REPORT WHERE CAMPUS = '" + campus + "' and COLLEGE = '" + college + "' AND DEGREE in('UG','PG') AND COURSE = '" + course + "'  AND BRANCH = '" + branch + "' and batch = '" + batch + "' ";
                j = await Task.FromResult(Delete25(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }




        public async Task<List<studenttrack>> getprocessdropdown(string semester, string userid)
        {
            try
            {
                var query = @"select distinct process_type from new_results_student1 where regdno = '" + userid + "' and semester = '" + semester + "' and STATUS NOT IN ('Dt','Dc','Exp') and process_type not in ('G','GA','SG','SGA') order by process_type";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getGradeResult(string semester, string userid, string month, string year, string process)
        {
            try
            {


                var query1 = "select distinct s.regdno,s.name,s.branch_code,s.course_code,s.degree_code,s.college_code as college_code,s.campus_code ,S.course_name,S.branch_name,s.section,";
                query1 = query1 + "S.course_name as course_title,s.batch,S.SGPA ,S.CGPA ,S.SEM_CREDITS,S.CUM_CREDITS,S.MONTH,S.YEAR as Year";
                query1 = query1 + "  from  [dbo].[NEW_RESULTS_STUDENT1] S,[dbo].[NEW_RESULTS_SUBJECT1] SB where S.REGDNO='" + userid + "' AND S.SEMESTER = " + semester + " and S.Year=" + year + " and S.Month='" + month + "'";
                query1 = query1 + " and S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  and s.process_type = sb.process_type  ";
                //   Response.Write(query1);
                if (process.Equals("RV") || process.Equals("RRRV") || process.Equals("SRV"))
                {
                    query1 = query1 + " and  s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  ORDER BY s.regdno";
                }
                if (process.Equals("CRV"))
                {
                    query1 = query1 + " and  s.process_type IN ('CRV')  ORDER BY s.regdno";
                }
                else if (process.Equals("R"))
                {
                    query1 = query1 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY s.regdno";
                }
                else if (process.Equals("I"))
                {
                    query1 = query1 + " and s.process_type IN ('I')  ORDER BY s.regdno";
                }
                else if (process.Equals("B"))
                {
                    query1 = query1 + " and s.process_type IN ('B') ORDER BY s.regdno";
                }
                else if (process.Equals("BG"))
                {
                    query1 = query1 + " and  s.process_type IN ('BG') ORDER BY s.regdno";
                }
                else if (process.Equals("SP"))
                {
                    query1 = query1 + "and  s.process_type IN ('SP')  ORDER BY s.regdno";
                }
                else if (process.Equals("SD"))
                {
                    query1 = query1 + " and  s.process_type IN ('SD') ORDER BY s.regdno";
                }
                else if (process.Equals("RT"))
                {
                    query1 = query1 + " and s.process_type IN ('RT','SRT') ORDER BY s.regdno";
                }
                else if (process.Equals("S"))
                {
                    query1 = query1 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY s.regdno";
                }
                else if (process.Equals("RR"))
                {
                    query1 = query1 + " and s.process_type IN ('RR','RRGA','RRG','R','GA','G') ORDER BY s.regdno";
                }
                else if (process.Equals("ST"))
                {
                    query1 = query1 + " and s.process_type IN ('ST') ORDER BY s.regdno";
                }
                else if (process.Equals("CC"))
                {
                    query1 = query1 + " and s.process_type IN ('CC') ORDER BY s.regdno";
                }
                else if (process.Equals("CCRV"))
                {
                    query1 = query1 + " and s.process_type IN ('CCRV') ORDER BY s.regdno";
                }
                else if (process.Equals("RB"))
                {

                    query1 = query1 + " and s.process_type IN ('RB') ORDER BY s.regdno";
                }
                return await Task.FromResult(GetAllData52<studenttrack>(query1, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Students>> getarrears(string regdno, string sem)
        {
            var query = "select isnull(statusflag,0) as statusflag,regdno from STUDENT_BALANCE_DATA  where status='1'  and FeeAmount>10000  and RegdNo='" + regdno + "'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }

        public async Task<IEnumerable<Students>> getarrearshostel(string regdno, string sem)
        {
            var query = "select isnull(hostel_arrears,0) as hostel_arrears  from STUDENT_MASTER  where REGDNO = '" + regdno + "'  and HOSTEL_ARREARS> 10000  and STATUS = 's'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }

        public async Task<IEnumerable<Students>> getarrearshostelbalanacedata(string regdno, string sem)
        {
            var query = "select isnull(sum(total_fee),0) as total_fee  from hostel_fee_challan_master where payment_status = 'y'  and PAYMENT_DATE>= '1-apr-2024'  and REGDNO = '" + regdno + "'";

            return await Task.FromResult(GetAllData<Students>(query, null));
        }

        public async Task<IEnumerable<Students>> gettxndatatutionbalance(string regdno, string sem)
        {
            var query = "select * from FEE_CHALLAN_MASTER_Selection where PaymentStatus = 'y'  and FEE_TYPE in('Semester', 'Annual')  and Payment_date>= '24-may-2024'  and RegdNo='" + regdno + "'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }
        public async Task<IEnumerable<Students>> getTBL_ALLOW_STUDENTS(string regdno, string sem)
        {
            var query = "select * from student_master where fin_flag = '1' and RegdNo ='" + regdno + "'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }


        public async Task<List<studenttrack>> getGradeResultst2(string semester, string userid, string month, string year, string process)
        {
            try
            {


                var query1 = "select distinct s.regdno,s.name,s.branch_code,s.course_code,s.degree_code,s.college_code as college_code, s.campus_code ,S.course_name,S.branch_name,s.section,S.course_name as course_title,s.batch,S.SGPA ,S.CGPA , S.SEM_CREDITS,S.CUM_CREDITS,S.MONTH,S.YEAR as Year  from[dbo].[NEW_RESULTS_STUDENT1] S,[dbo].[NEW_RESULTS_SUBJECT1] SB where S.REGDNO = '" + userid + "' and S.Year = 2024 and S.Month = 'Jun' and S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  and s.process_type = sb.process_type  order by  sem_credits desc ";

                return await Task.FromResult(GetAllData52<studenttrack>(query1, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<studenttrack> InsertGradedetails(studenttrack _Parameter)
        {
            bool retnVal = false;

            try
            {
                var query2 = "insert into New_Grade_report (regdno,name,branch,course,degree,college,campus,COURSE_NAME,BRANCH_NAME" +
                    ",section,COURSE_TITLE,PRINT_YEAR,EXAM_NAME,NOTE,Misc_Note,exam_date,month,year,batch,process_type,SGPA,CGPA,TOTAL_SEM_CREDITS,CUM_SEM_CREDITS) VALUES ";
                query2 = query2 + "('" + _Parameter.regdno + "','" + _Parameter.name + "','" + _Parameter.BRANCH + "','" + _Parameter.COURSE + "','" + _Parameter.DEGREE_CODE + "','" + _Parameter.college_code + "','" + _Parameter.CAMPUS_CODE + "','" + _Parameter.course_name + "'" +
                    ",'" + _Parameter.Branch_name + "','" + _Parameter.section + "','" + _Parameter.COURSE_TITLE + "','" + _Parameter.PRINT_YEAR + "','" + _Parameter.EXAM_NAME + "','','','" + _Parameter.exam_date + "','" + _Parameter.Month + "'," + _Parameter.Year + ",'" + _Parameter.BATCH + "','" + _Parameter.process_type + "','" + _Parameter.sgpa + "','" + _Parameter.cgpa + "','" + _Parameter.TOTAL_SEM_CREDITS + "','" + _Parameter.CUM_SEM_CREDITS + "')";


                int k = await Task.FromResult(InsertData25(query2, null));


                if (k > 0)
                {
                    retnVal = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Parameter;
        }


        public async Task<List<studenttrack>> getGradeReports(studenttrack _Parameter)
        {
            var query3 = "";
            try
            {
                if (_Parameter.process_type.Equals("BG"))
                {
                    query3 = @"WITH RankedResults AS (SELECT   SB.subject_code,SB.subject_name, SB.SUBJECT_CREDITS AS CREDITS1, SB.sUBJECT_GRADE AS GRADE,SB.semester, SB.SUBJECT_ORDER,SB.process_type,ROW_NUMBER() OVER(PARTITION BY SB.subject_code ORDER BY SB.SUBJECT_ORDER) AS RowNum  FROM   [dbo].[NEW_RESULTS_SUBJECT1] SB   JOIN   NEW_RESULTS_STUDENT1 S ON S.REGDNO = SB.REGDNO  AND S.SEMESTER = SB.SEMESTER  AND S.month = SB.month AND S.year = SB.year WHERE S.status = 'A'  AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "   AND S.process_type IN('R', 'GA', 'G', 'S', 'SGA', 'SG', 'BG')  AND S.process_type = SB.process_type) SELECT subject_code,  subject_name,   CREDITS1,   GRADE,    semester,    process_type FROM    RankedResults WHERE    RowNum = 1 ORDER BY  SUBJECT_ORDER;";
                }

                else if (_Parameter.SEMESTER == "92" && _Parameter.process_type == "SummerTerm 2" && _Parameter.Month == "Jun" && _Parameter.Year == "2024" || _Parameter.SEMESTER == "93" && _Parameter.Month == "Jun" && _Parameter.Year == "2024")
                {
                    query3 = @" select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE, SB.semester,SB.process_type  from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where  S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year   and   s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "'  and s.month = 'Jun' and s.year = '2024'  ";
                }
                else if (_Parameter.process_type.Equals("RB") || _Parameter.process_type.Equals("RBRV"))
                {

                    query3 = @"  select distinct SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type  from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S  where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and   s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER =" + _Parameter.SEMESTER + "and s.month ='" + _Parameter.Month + "' and s.year ='" + _Parameter.Year + "'  and sB.process_type='" + _Parameter.process_type + "' and s.process_type =sb.process_type";

                }
                else
                {
                    query3 = " select  distinct Sb.SUBJECT_ORDER,SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type ";
                    query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                    query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'  and s.process_type =sb.process_type";

                    if (_Parameter.process_type.Equals("RV") || _Parameter.process_type.Equals("RRRV") || _Parameter.process_type.Equals("SRV"))
                    {
                        //query3 = query3 + " and s.process_type IN ('RV','SRV','RRV','SRRV')  ORDER BY Sb.SUBJECT_ORDER";
                        query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER ";
                        query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'  ";//and s.process_type =sb.process_type


                        query3 = query3 + " and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  and sb.process_type not in('CC','G','GA') and s.process_type not in('CC','G','GA')  ";
                        query3 = query3 + @" AND SUBJECT_CODE NOT IN ( select SB.subject_code  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  and s.process_type =sb.process_type )";

                        query3 = query3 + @"  UNION ALL select SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";

                    }
                    else if (_Parameter.process_type.Equals("CRV"))
                    {

                        query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER ";
                        query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'  ";//and s.process_type =sb.process_type


                        query3 = query3 + " and s.process_type IN ('CRV') and s.process_type =sb.process_type ";
                        query3 = query3 + @" AND SUBJECT_CODE NOT IN ( select SB.subject_code  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('CRV')  and s.process_type =sb.process_type )";

                        query3 = query3 + @"  UNION ALL select SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('CRV')  and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";

                    }
                    else if (_Parameter.process_type.Equals("R"))
                    {
                        query3 = query3 + " and s.process_type IN ('R','GA','G','S','SGA','SG') and s.process_type =sb.process_type  ORDER BY Sb.subject_code";//and s.process_type =sb.process_type
                    }
                    else if (_Parameter.process_type.Equals("I"))
                    {
                        query3 = query3 + " and s.process_type IN ('I') and s.process_type =sb.process_type  ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("B"))
                    {
                        query3 = query3 + " and s.process_type IN ('B') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("BG"))
                    {

                        query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER ";
                        query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "   ";//and s.process_type =sb.process_type


                        query3 = query3 + " and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV') ";
                        query3 = query3 + @" AND SUBJECT_CODE NOT IN ( select SB.subject_code  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "    and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  and s.process_type =sb.process_type )";

                        query3 = query3 + @"  UNION ALL select SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "    and s.process_type IN ('RV','SRV','RRV','SRRV','RRRV')  and s.process_type =sb.process_type ";

                        query3 = " select *  from  (" + query3 + " ) temp ";


                        query3 = query3 + @"  where subject_code not in(
                                   select SB.subject_code from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S
                                where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year
                                and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + @"' AND S.SEMESTER = " + _Parameter.SEMESTER + @"    and s.process_type IN ('BG')) UNION ALL
                                select SB.subject_code, sb.subject_name, sb.SUBJECT_CREDITS as CREDITS1, SB.sUBJECT_GRADE as GRADE, SB.semester, SB.process_type, sb.SUBJECT_ORDER from[dbo].[NEW_RESULTS_SUBJECT1] SB, NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year
                                and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + @"   and s.process_type IN ('BG')  and s.process_type =sb.process_type ORDER BY SUBJECT_ORDER    ";




                        //query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER ";
                        //query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        //query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + dr3[0] + "' AND S.SEMESTER = " + semester + "  ";//  and s.month = '" + month + "' and s.year = '" + year + "' and s.process_type =sb.process_type


                        //query3 = query3 + " and s.process_type IN ('BG','R') ";
                        //query3 = query3 + @" AND SUBJECT_CODE NOT IN ( select SB.subject_code  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                        //                and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + dr3[0] + "' AND S.SEMESTER = " + semester + "    and s.process_type IN ('BG')   )";//and s.process_type =sb.process_type and s.month = '" + month + "' and s.year = '" + year + "'

                        //query3 = query3 + @"  UNION ALL select SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                        //                and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + dr3[0] + "' AND S.SEMESTER = " + semester + "   and s.process_type IN ('BG')  and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";//and s.month = '" + month + "' and s.year = '" + year + "' 



                        //query3 = query3 + " and s.process_type IN ('BG') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("SP"))
                    {
                        query3 = query3 + " and s.process_type IN ('SP') and s.process_type =sb.process_type  ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("SD"))
                    {
                        query3 = query3 + " and s.process_type IN ('SD') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("RT"))
                    {
                        query3 = query3 + "and  s.process_type IN ('RT','SRT') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("S"))
                    {
                        query3 = query3 + " and s.process_type IN ('R','GA','G','S','SGA','SG') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }
                    else if (_Parameter.process_type.Equals("RR"))
                    {
                        query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type ,sb.SUBJECT_ORDER ";
                        query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "  and s.process_type =sb.process_type ";//and s.month = '" + month + "' and s.year = '" + year + "' and s.process_type =sb.process_type

                        query3 = query3 + " and s.process_type IN ('RR','RRGA','RRG') and s.process_type =sb.process_type ";

                        query3 = query3 + @"  UNION ALL select SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + "   and s.process_type IN ('R','GA','G')  and s.process_type =sb.process_type and sb.subject_code not in ( select subject_code  from  new_results_subject1  where regdno='" + _Parameter.regdno + "' and SEMESTER = " + _Parameter.SEMESTER + "   and process_type IN ('RR','RRGA','RRG','BG')) ORDER BY Sb.SUBJECT_ORDER";//and s.month = '" + month + "' and s.year = '" + year + "' 

                    }
                    else if (_Parameter.process_type.Equals("ST"))
                    {
                        query3 = query3 + " and s.process_type IN ('ST') and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";
                    }

                    if (_Parameter.process_type.Equals("CC") || _Parameter.process_type.Equals("CCRV"))
                    {
                        //query3 = query3 + " and s.process_type IN ('RV','SRV','RRV','SRRV')  ORDER BY Sb.SUBJECT_ORDER";
                        query3 = " select  distinct SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,null as process_type,sb.SUBJECT_ORDER ";
                        query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year  and ";
                        query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE != 'CDL' and s.process_type=sb.PROCESS_TYPE  AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'  ";//and s.process_type =sb.process_type


                        query3 = query3 + " and s.process_type IN ('CC','CCRV') ";
                        query3 = query3 + @" AND SUBJECT_CODE NOT IN ( select SB.subject_code  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('CCRV')  and s.process_type =sb.process_type )";

                        query3 = query3 + @"  UNION ALL select distinct SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,null as process_type,sb.SUBJECT_ORDER  from[dbo].[NEW_RESULTS_SUBJECT1] SB,NEW_RESULTS_STUDENT1 S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER AND s.month = sb.month and s.year = sb.year 
                                        and s.status = 'A' AND S.COLLEGE_CODE != 'CDL'  AND S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'   and s.process_type IN ('CCRV')  and s.process_type =sb.process_type ORDER BY Sb.SUBJECT_ORDER";


                    }

                }
                return await Task.FromResult(GetAllData52<studenttrack>(query3, null));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<studenttrack>> updateGradedetails(List<studenttrack> _Parameter)
        {
            bool retnVal = false;
            var query4 = "";
            int scount = 1;
            int k = 0;
            try
            {
                for (int i = 0; i < _Parameter.Count; i++)
                {
                    if (_Parameter[i].process_type.ToString().Trim().Equals("S") || _Parameter[i].process_type.ToString().Trim().Equals("SG") || _Parameter[i].process_type.ToString().Trim().Equals("SGA") || _Parameter[i].process_type.ToString().Trim().Equals("SRV") || _Parameter[i].process_type.ToString().Trim().Equals("SP") || _Parameter[i].process_type.ToString().Trim().Equals("SPG") || _Parameter[i].process_type.ToString().Trim().Equals("SPGA"))
                    {
                        // query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + dr1[0] + "' ,sub" + scount + "_name= '" + dr1[1] + "',sub" + scount + "_credits='" + dr1[2] + "',sub" + scount + "_grade = '*'+'" + dr1[3] + "',semester =  " + dr1[4] + " ,roman_semester = case  " + dr1[4] + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV' end  where regdno = '" + dr3[0] + "' AND MONTH = '" + month + "' AND YEAR = '" + year + "' AND PROCESS_TYPE = '"+ process + "' and campus = '" + campus + "'";
                        query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + _Parameter[i].SUBJECT_CODE + "' ,sub" + scount + "_name= '" + _Parameter[i].SUBJECT_NAME + "',sub" + scount + "_credits='" + _Parameter[i].CREDITS1 + "',sub" + scount + "_grade ='" + _Parameter[i].Grade + "',semester =  " + _Parameter[i].SEMESTER + " ,roman_semester = case  " + _Parameter[i].SEMESTER + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV' when 21 then 'Summer Term' when  41 then 'Summer Term' end  where regdno = '" + _Parameter[i].regdno + "' AND MONTH = '" + _Parameter[i].Month + "' AND YEAR = '" + _Parameter[i].Year + "' AND PROCESS_TYPE = '" + _Parameter[i].process_type + "' and campus = '" + _Parameter[i].CAMPUS_CODE + "'";

                        // Response.Write(query4);
                    }

                    else

                    {
                        query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + _Parameter[i].SUBJECT_CODE + "' ,sub" + scount + "_name= '" + _Parameter[i].SUBJECT_NAME + "',sub" + scount + "_credits='" + _Parameter[i].CREDITS1 + "',sub" + scount + "_grade = '" + _Parameter[i].Grade + "',semester =  " + _Parameter[i].SEMESTER + " ,roman_semester = case  " + _Parameter[i].SEMESTER + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV'  when 21 then 'Summer Term' when  41 then 'Summer Term' end  where regdno = '" + _Parameter[i].regdno + "' AND MONTH = '" + _Parameter[i].Month + "' AND YEAR = '" + _Parameter[i].Year + "' AND PROCESS_TYPE = '" + _Parameter[i].process_type + "' and campus = '" + _Parameter[i].CAMPUS_CODE + "'";
                    }
                    scount = scount + 1;
                    k = await Task.FromResult(Update25(query4, null));
                }







                if (k > 0)
                {
                    retnVal = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Parameter;
        }




        public async Task<List<studenttrack>> getQrResult(string reg, string sem, string month, string process, string year)
        {
            string sql = "";
            try
            {


                sql = "SELECT S.STATUS AS STATUS,S.REGDNO as REGDNO,S.NAME AS NAME,S.BRANCH_NAME AS BRANCH_NAME,S.COURSE_NAME AS COURSE_NAME,'SEMESTER'=case s.semester when 1 then 'I' when 2 then 'II' ";
                sql = sql + "when 3 then 'III' when 4 then 'IV'  when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 ";
                sql = sql + "then 'VIII'   when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII'   when 13 ";
                sql = sql + " then 'XIII' when 14 then 'XIV' when 15 then 'XV' END ";
                sql = sql + " ,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT1 WHERE REGDNO ='" + reg + "' AND SEMESTER='" + sem + "') AS SEM_CREDITS,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT1 WHERE REGDNO ='" + reg + "' AND SEMESTER <= '" + sem + "') AS CUM_CREDITS,S.SGPA AS SGPA,S.CGPA AS CGPA, ";
                sql = sql + " SS.SUBJECT_CODE AS SUBJECT_CODE,'SUBJECT_NAME' = CASE WHEN SS.SUBJECT_CATEGORY = 'ELE' THEN 'ELECTIVE: '+SS.SUBJECT_NAME ELSE ";
                sql = sql + " SS.SUBJECT_NAME END,SS.SUBJECT_CREDITS AS CREDITS,SS.SUBJECT_GRADE AS GRADE,'SUBJECT_TYPE' = CASE WHEN SS.SUBJECT_TYPE = 'T' THEN ";
                sql = sql + " 'THEORY:' ELSE  'PRACTICALS:' END,SS.SUBJECT_CATEGORY,S.MONTH AS MONTH,S.YEAR AS YEAR ";
                sql = sql + " FROM NEW_RESULTS_STUDENT1 S,NEW_RESULTS_SUBJECT1 SS WHERE S.REGDNO = SS.REGDNO AND S.SEMESTER = SS.SEMESTER AND s.month = ss.month and s.year =ss.year   AND ";
                sql = sql + " S.REGDNO = '" + reg + "' AND SS.SEMESTER='" + sem + "' and s.month = '" + month + "' and s.year = '" + year + "'  and s.status = 'A' AND S.COLLEGE_CODE != 'CDL' and s.process_type =ss.process_type  and  ";

                if (process.Equals("RV"))
                {
                    sql = sql + " ss.process_type IN ('RV','SRV','RRV','SRRV') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("R") || process.Equals("S"))
                {
                    sql = sql + " ss.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("I"))
                {
                    sql = sql + " ss.process_type IN ('I') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("B"))
                {
                    sql = sql + " ss.process_type IN ('B') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("BG"))
                {
                    sql = sql + " ss.process_type IN ('BG') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("RR"))
                {
                    sql = sql + " ss.process_type IN ('RR') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("SP"))
                {
                    sql = sql + " ss.process_type IN ('SP','SPG','SPGA') AND s.process_type=ss.process_type   ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("SD"))
                {
                    sql = sql + " ss.process_type IN ('SD') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                else if (process.Equals("RT"))
                {
                    sql = sql + " ss.process_type IN ('RT','SRT') AND s.process_type=ss.process_type  ORDER BY SS.SUBJECT_ORDER";
                }
                return await Task.FromResult(GetAllData52<studenttrack>(sql, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
        public async Task<IEnumerable<studenttrack>> getfeedback_subjectsasync(string userid, string sem)
        {
            try
            {
                int sem1 = 0;
                sem1 = Convert.ToInt32(sem) - 1;
                var query = @"select distinct SUBJECT_CODE,SUBJECT_NAME,GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where regdno = '" + userid + "' and SEMESTER = '" + sem1 + "' and GROUP_CODE is not null and SUBJECT_CODE  not like '%p' and  SUBJECT_TYPE in('tp'  ,'t') ";
                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<studenttrack>> getfeedback_facultyasync(string groupcode, string campus, string dept_code)
        {
            try
            {
                var query = @"select top 1 GROUP_CODE,EMPID, emp_name,dept_code,subject_code from HOD_STAFF_SUBJECTS_MASTER where DACTIVE_FLAG = 'A'   and GROUP_CODE = '" + groupcode + "'  and  CAMPUS='" + campus + "' and sms is null";
                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        public async Task<IEnumerable<FeedbackMaster>> getquestionlist()
        {
            try
            {
                var query = @"select NATURE,Q_STATUS,FEEDBACK1,FEEDBACK2,FEEDBACK3,FEEDBACK4,FEEDBACK5,FEEDBACK1_MARK,FEEDBACK2_MARK,FEEDBACK3_MARK,FEEDBACK4_MARK,FEEDBACK5_MARK,ID,maxmarks from gitam_feedback_new_master   order by ID";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<FeedbackMaster>> getfeedback_check(string subjectname, string faculty, string feedbacksession, string REGDNO, string sem)
        {
            try
            {
                var query = @"select feedback_Status from  feedback_student where regdno='" + REGDNO + "' and subject='" + subjectname + "'  and semester ='" + sem + "' and feedback_status='y' and feedback_session='" + feedbacksession + "' and empid='" + faculty + "'";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

   

        public async Task<IEnumerable<FeedbackMaster>> Insertfeedbacknew(List<FeedbackMaster> chart)
        {
            try
            {
                int j = 0;
                string updqry = null;
                string query = "";
                int k = 0;
                bool retnval = false;
                int maxmarkstotal = 0;
                int count1 = chart.Count-1;
                for (int p = 0; p < count1; p++)
                {
                    maxmarkstotal = maxmarkstotal + Convert.ToInt32(chart[p].maxmarks);
                }
                for (int i = 0; i < chart.Count; i++)
                {

                    if (chart[i].Q_STATUS.Equals("O"))
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        updqry = "insert into GITAM_STUDENT_FEEDBACK(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME, FEEDBACK_TITLE,";
                        updqry = updqry + "FEEDBACK_MARK,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,FEEDBACK_ID,section,batch,campus_code,branch_code,COMMENTS,semester,FEEDBACK_SESSION)";
                        updqry = updqry + " values('" + chart[i].group_code + "','" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].subject_code + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].empid + "','" + chart[i].empname + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','"+chart[i].maxmarks+"','" + chart[i].subjectname + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','','" + chart[i].semester + "','" + chart[i].feedbacksession + "')";
                    }
                    else
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        chart[i].FEEDBACK1 = chart[i].FEEDBACK1.Replace("'", "''");

                        updqry = "insert into GITAM_STUDENT_FEEDBACK(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME, FEEDBACK_TITLE,";
                        updqry = updqry + "COMMENTS,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,FEEDBACK_ID,section,batch,campus_code,branch_code,semester,FEEDBACK_SESSION)";

                        updqry = updqry + " values('" + chart[i].group_code + "','" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].subject_code + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].empid + "','" + chart[i].empname + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','"+chart[i].maxmarks+"','" + chart[i].subjectname + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','" + chart[i].semester + "','" + chart[i].feedbacksession + "')";


                    }
                    k = await Task.FromResult(InsertData60feedback(updqry, null));
                }

                if (k > 0)
                {
                    query = "insert into feedback_student(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME,";
                    query = query + "FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,section,campus_code,branch_code,semester,FEEDBACK_SESSION,feedback_status)";

                    query = query + " values('" + chart[0].group_code + "','" + chart[0].regdno + "','" + chart[0].stuname + "','" + chart[0].subject_code + "','" + chart[0].course_code + "','" + chart[0].dept_code + "','" + chart[0].empid + "','" + chart[0].empname + "',getdate(),'" + chart[0].college_code + "','"+ maxmarkstotal + "','" + chart[0].subjectname + "','" + chart[0].section + "','" + chart[0].campus_code + "','" + chart[0].branch_code + "','" + chart[0].semester + "','" + chart[0].feedbacksession + "','Y')";
                    j = await Task.FromResult(InsertData60feedback(query, null));
                    chart[0].flag = "success";
                }
                return chart;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        */

        public async Task<List<CDLCertificates>> getcdlstudycertificate(string userid)
        {
            try
            {
                var query = @"select SLNO,REGDNO,NAME,FATHER_NAME,CONVERT(varchar(10),DOB,105) DOB,NATIONALITY,RELIGION,CONVERT(varchar(10),DATE_OF_ADMISSION,105) DOA,CONVERT(varchar(10),DATE_OF_LEAVING,105) DOL,MEDIUM,COURSE = case COURSE_CODE when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)'  when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'   when 'BCOM ' THEN 'B.Com.'   when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else COURSE_CODE end, STATUS, course_name, BRANCH_CODE as branch, Batch from STUDENT_MASTER where REGDNO = '" + userid + "'";
                return await Task.FromResult(GetAllData<CDLCertificates>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Event>> getevents(string campus)
        {
            try
            {
                var query = @"select EventDescription,Event_dates,eventid,url,eventday as eventday,Eventmonth,case  campus_code WHEN 'VSP' then 'Visakhapatnam' when 'BLR' then 'Bengaluru' else 'Hyderabad' end as campus,ConfirmFlag,Convert(Date, EventDate) ,eventid,EventName,substring(eventlocation,1,25) as EventLocation,EventDate ,convert(varchar,start_date,107) as Date,End_date,Uploadtype,url,Contact_person,Contact_number,Contact_email,IMAGE_PATH,organizedby from GITAM_EVENTS_INSERT where Convert(Date, EventDate) >=Convert(Date, getdate()) and campus_code='" + campus + "'   and ConfirmFlag='Y' order by start_date";
                return await Task.FromResult(GetAllData60website<Event>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<CDLCertificates>> getcdltransfercertificate(string userid)
        {
            try
            {
                var query = @" select  SLNO,REGDNO,NAME,FATHER_NAME,CONVERT(varchar(10),DOB,105) DOB ,replace(CONVERT(varchar(10),DOB,105),'-','') as DOB_WORDS,NATIONALITY,RELIGION,CONVERT(varchar(10),DATE_OF_ADMISSION,105) DOA
                                 , CONVERT(varchar(10), DATE_OF_LEAVING, 105) DOL,'Completed' as STUDY_STATUS,MEDIUM as MEDIUM,1 as COUNT,
                                 COURSE = case COURSE_CODE when 'BTECH' THEN 'B.Tech' when 'MTECH' THEN 'M.Tech' when 'MSC' THEN 'M.Sc.' when 'BSCHONS' THEN 'B.Sc.(Hons)'
                                 when 'BPHARMACY ' THEN 'B.Pharmacy' when 'MPHARMACY ' THEN 'M.Pharmacy' when 'BARCH ' THEN 'B.Architecture' when 'MARCH ' THEN 'M.Architecture'   when 'BCOM ' THEN 'B.Com.'
                                 when 'FINTECH ' THEN 'FinTech'  when 'IMSC ' THEN  'Integrated M.Sc. (B.Sc.+M.Sc.)' else COURSE_CODE end,'ISSUED' as STATUS
                                 ,(select course_name from  COURSE_MASTER where COURSE_CODE = s.course_code and CAMPUS_CODE = s.CAMPUS and COLLEGE_CODE = s.COLLEGE_CODE and DEGREE_CODE = s.DEGREE_CODE) as course_name
                                 ,BRANCH_CODE,BATCH as AC_YEAR,
                                 case when s.COURSE_CODE in('BA', 'B.A') then
                                  (select BRANCH_NAME  from BRANCH_MASTER b where b.BRANCH_CODE = s.BRANCH_CODE and b.CAMPUS_CODE = s.CAMPUS and b.COLLEGE_CODE = s.COLLEGE_CODE and b.DEGREE_CODE = s.DEGREE_CODE and b.COURSE_CODE = s.COURSE_CODE)
                                 else '-' end sp_name , CASE WHEN S.BRANCH_CODE LIKE 'GEN%' THEN 'GENERAL' ELSE S.BRANCH_CODE END SP_STATUS from STUDENT_MASTER s where REGDNO = '" + userid + "' ";
                return await Task.FromResult(GetAllData<CDLCertificates>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<studenttrack>> getCDLsemesterasync(string userid)
        {
            try
            {
                //   var query = @"select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER <= (select curr_sem from student_master where regdno = '" + userid + "') union select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER >= '21'";

                var query = @"select distinct(semester) from new_results_student where regdno = '" + userid + "' and STATUS NOT IN ('Dt','Dc','Exp') AND ( (YEAR =2021 and MONTH='Aug') or (YEAR >=2022) ) order by semester";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<List<studenttrack>> getStudentcarddetailsCDL(string semester, string userid)
        {
            try
            {
                var query = @"select CAMPUS_CODE,COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,SEMESTER,BATCH,SECTION as section,MONTH as Month,YEAR as Year,PROCESS_TYPE as process_type from [NEW_RESULTS_STUDENT] where regdno = '" + userid + "' and semester='" + semester + "' AND ((YEAR=2021 and MONTH='Aug') or (YEAR >=2022)) ";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<int> DeleteDetailsCDL(string campus, string college, string course, string branch, string batch)
        {
            int j = 0;
            try
            {
                var query = @"DELETE FROM NEW_GRADE_REPORT WHERE CAMPUS = '" + campus + "' and COLLEGE = '" + college + "' AND DEGREE in('UG','PG') AND COURSE = '" + course + "'  AND BRANCH = '" + branch + "' and batch = '" + batch + "' ";
                j = await Task.FromResult(Delete25(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }


        public async Task<List<studenttrack>> getGradeResultCDL(string semester, string userid, string month, string year, string process)
        {
            try
            {


                var query1 = "select distinct s.regdno,s.name,s.branch_code,s.course_code,s.degree_code,s.college_code,s.campus_code ,S.course_name,S.branch_name,s.section,";
                query1 = query1 + "S.course_name as course_title,s.batch,S.SGPA ,S.CGPA ,S.SEM_CREDITS,S.CUM_CREDITS,S.MONTH,S.YEAR";
                query1 = query1 + "  from  [dbo].[NEW_RESULTS_STUDENT] S,[dbo].[NEW_RESULTS_SUBJECT] SB where S.REGDNO='" + userid + "' AND S.SEMESTER = " + semester + " and S.Year=" + year + " and S.Month='" + month + "'";
                query1 = query1 + " and S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER  and s.status = 'A' AND S.COLLEGE_CODE = 'CDL'   ";//AND s.month = sb.month and s.year = sb.year  and s.process_type = sb.process_type

                if (process.Equals("RV"))
                {
                    query1 = query1 + " and  s.process_type IN ('RV','SRV','RRV','SRRV')  ORDER BY s.regdno";
                }
                else if (process.Equals("R"))
                {
                    query1 = query1 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY s.regdno";
                }
                else if (process.Equals("I"))
                {
                    query1 = query1 + " and s.process_type IN ('I')  ORDER BY s.regdno";
                }
                else if (process.Equals("B"))
                {
                    query1 = query1 + " and s.process_type IN ('B') ORDER BY s.regdno";
                }
                else if (process.Equals("BG"))
                {
                    query1 = query1 + " and  s.process_type IN ('BG') ORDER BY s.regdno";
                }
                else if (process.Equals("SP"))
                {
                    query1 = query1 + "and  s.process_type IN ('SP')  ORDER BY s.regdno";
                }
                else if (process.Equals("SD"))
                {
                    query1 = query1 + " and  s.process_type IN ('SD') ORDER BY s.regdno";
                }
                else if (process.Equals("RT"))
                {
                    query1 = query1 + " and s.process_type IN ('RT','SRT') ORDER BY s.regdno";
                }
                else if (process.Equals("S"))
                {
                    query1 = query1 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY s.regdno";
                }
                else if (process.Equals("LSP"))
                {
                    query1 = query1 + " and s.process_type IN ('LSP') ORDER BY s.regdno";
                }
                return await Task.FromResult(GetAllData52<studenttrack>(query1, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<studenttrack>> getGradeReportsCDL(studenttrack _Parameter)
        {
            try
            {

                string process = _Parameter.process_type;
                var query3 = " select  SB.subject_code,sb.subject_name,sb.SUBJECT_CREDITS as CREDITS1,SB.sUBJECT_GRADE as GRADE,SB.semester,SB.process_type ";
                query3 = query3 + ",(select max(ss.dt_time) from [NEW_RESULTS_SUBJECT] ss where ss.regdno=sb.regdno and s.regdno=s.regdno AND  SS.REGDNO = '" + _Parameter.regdno + "' AND Ss.SEMESTER =" + _Parameter.SEMESTER + " ) Doc,convert(varchar(10),sb.dt_time,121) Doc1 ";
                query3 = query3 + " from [dbo].[NEW_RESULTS_SUBJECT] SB,NEW_RESULTS_STUDENT S where S.REGDNO = Sb.REGDNO AND S.SEMESTER = Sb.SEMESTER   and ";//AND s.month = sb.month and s.year = sb.year
                query3 = query3 + "  s.status = 'A' AND S.COLLEGE_CODE = 'CDL'   AND  S.REGDNO = '" + _Parameter.regdno + "' AND S.SEMESTER = " + _Parameter.SEMESTER + " and s.month = '" + _Parameter.Month + "' and s.year = '" + _Parameter.Year + "'  ";//and s.process_type =sb.process_type

                if (process.Equals("RV"))
                {
                    query3 = query3 + " and s.process_type IN ('RV','SRV','RRV','SRRV')  ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("R"))
                {
                    query3 = query3 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("I"))
                {
                    query3 = query3 + " and s.process_type IN ('I')  ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("B"))
                {
                    query3 = query3 + " and s.process_type IN ('B') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("BG"))
                {
                    query3 = query3 + " and s.process_type IN ('BG') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("SP"))
                {
                    query3 = query3 + " and s.process_type IN ('SP')  ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("SD"))
                {
                    query3 = query3 + " and s.process_type IN ('SD') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("RT"))
                {
                    query3 = query3 + "and  s.process_type IN ('RT','SRT') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("S"))
                {
                    query3 = query3 + " and s.process_type IN ('R','GA','G','S','SGA','SG') ORDER BY Sb.SUBJECT_ORDER";
                }
                else if (process.Equals("LSP"))
                {
                    query3 = query3 + " and s.process_type IN ('LSP') ORDER BY Sb.SUBJECT_ORDER";
                }

                return await Task.FromResult(GetAllData52<studenttrack>(query3, null));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<List<studenttrack>> updateGradedetailsCDL(List<studenttrack> _Parameter)
        {
            bool retnVal = false;
            var query4 = "";
            int scount = 1;
            int k = 0;
            try
            {
                for (int i = 0; i < _Parameter.Count; i++)
                {
                    if (_Parameter[i].process_type.ToString().Trim().Equals("S") || _Parameter[i].process_type.ToString().Trim().Equals("SG") || _Parameter[i].process_type.ToString().Trim().Equals("SGA") || _Parameter[i].process_type.ToString().Trim().Equals("SRV") || _Parameter[i].process_type.ToString().Trim().Equals("SP") || _Parameter[i].process_type.ToString().Trim().Equals("SPG") || _Parameter[i].process_type.ToString().Trim().Equals("SPGA"))
                    {
                        // query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + dr1[0] + "' ,sub" + scount + "_name= '" + dr1[1] + "',sub" + scount + "_credits='" + dr1[2] + "',sub" + scount + "_grade = '*'+'" + dr1[3] + "',semester =  " + dr1[4] + " ,roman_semester = case  " + dr1[4] + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV' end  where regdno = '" + dr3[0] + "' AND MONTH = '" + month + "' AND YEAR = '" + year + "' AND PROCESS_TYPE = '"+ process + "' and campus = '" + campus + "'";
                        query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + _Parameter[i].SUBJECT_CODE + "' ,sub" + scount + "_name= '" + _Parameter[i].SUBJECT_NAME + "',sub" + scount + "_credits='" + _Parameter[i].CREDITS1 + "',sub" + scount + "_grade ='" + _Parameter[i].Grade + "',semester =  " + _Parameter[i].SEMESTER + " ,roman_semester = case  " + _Parameter[i].SEMESTER + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV' when 21 then 'Summer Term' end, DT_Completion='" + _Parameter[i].DT_Completion + "'  where regdno = '" + _Parameter[i].regdno + "' AND MONTH = '" + _Parameter[i].Month + "' AND YEAR = '" + _Parameter[i].Year + "' AND PROCESS_TYPE = '" + _Parameter[i].process_type + "' and campus = '" + _Parameter[i].CAMPUS_CODE + "'";

                        // Response.Write(query4);
                    }

                    else

                    {
                        query4 = "update New_Grade_REPORT set sub" + scount + "_code = '" + _Parameter[i].SUBJECT_CODE + "' ,sub" + scount + "_name= '" + _Parameter[i].SUBJECT_NAME + "',sub" + scount + "_credits='" + _Parameter[i].CREDITS1 + "',sub" + scount + "_grade = '" + _Parameter[i].Grade + "',semester =  " + _Parameter[i].SEMESTER + " ,roman_semester = case  " + _Parameter[i].SEMESTER + " when 1 then 'I' when 2 then 'II' when 3 then 'III' when 4 then 'IV' when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 then 'VIII' when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII' when 13 then 'XIII' when 14 then 'XIV' when 15 then 'XV'  when 21 then 'Summer Term' end, DT_Completion='" + _Parameter[i].DT_Completion + "'  where regdno = '" + _Parameter[i].regdno + "' AND MONTH = '" + _Parameter[i].Month + "' AND YEAR = '" + _Parameter[i].Year + "' AND PROCESS_TYPE = '" + _Parameter[i].process_type + "' and campus = '" + _Parameter[i].CAMPUS_CODE + "'";
                    }
                    scount = scount + 1;
                    k = await Task.FromResult(Update25(query4, null));
                }







                if (k > 0)
                {
                    retnVal = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _Parameter;
        }


        public async Task<List<studenttrack>> getQrResultCDL(string reg, string sem, string month, string process, string year)
        {
            string sql = "";
            try
            {


                sql = "SELECT S.STATUS AS STATUS,S.REGDNO as REGDNO,S.NAME AS NAME,S.BRANCH_NAME AS BRANCH_NAME,S.COURSE_NAME AS COURSE_NAME,'SEMESTER'=case s.semester when 1 then 'I' when 2 then 'II' ";
                sql = sql + "when 3 then 'III' when 4 then 'IV'  when 5 then 'V' when 6 then 'VI' when 7 then 'VII' when 8 ";
                sql = sql + "then 'VIII'   when 9 then 'IX' when 10 then 'X' when 11 then 'XI' when 12 then 'XII'   when 13 ";
                sql = sql + " then 'XIII' when 14 then 'XIV' when 15 then 'XV' END ";
                sql = sql + " ,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER='" + sem + "') AS SEM_CREDITS,(SELECT SUM(SUBJECT_CREDITS) FROM NEW_RESULTS_SUBJECT WHERE REGDNO ='" + reg + "' AND SEMESTER <= '" + sem + "') AS CUM_CREDITS,S.SGPA AS SGPA,S.CGPA AS CGPA, ";
                sql = sql + " SS.SUBJECT_CODE AS SUBJECT_CODE,'SUBJECT_NAME' = CASE WHEN SS.SUBJECT_CATEGORY = 'ELE' THEN 'ELECTIVE: '+SS.SUBJECT_NAME ELSE ";
                sql = sql + " SS.SUBJECT_NAME END,SS.SUBJECT_CREDITS AS CREDITS,SS.SUBJECT_GRADE AS GRADE,'SUBJECT_TYPE' = CASE WHEN SS.SUBJECT_TYPE = 'T' THEN ";
                sql = sql + " 'THEORY:' ELSE  'PRACTICALS:' END,SS.SUBJECT_CATEGORY,S.MONTH AS Month,S.YEAR AS Year ";
                sql = sql + " FROM NEW_RESULTS_STUDENT S,NEW_RESULTS_SUBJECT SS WHERE S.REGDNO = SS.REGDNO AND S.SEMESTER = SS.SEMESTER AND ";
                sql = sql + " S.REGDNO = '" + reg + "' AND SS.SEMESTER='" + sem + "' and s.status = 'A' AND S.COLLEGE_CODE = 'CDL' ORDER BY SS.SUBJECT_ORDER";

                return await Task.FromResult(GetAllData52<studenttrack>(sql, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<List<CDLCertificates>> getcdlMigration(string userid)
        {
            try
            {
                var query = @"select Regdno,name,course_Code,Branch_code,Semester,Batch,College_Code,Campus_Code,Degree_Code,
                               (select course_name  from course_master where course_code=s.course_Code) course_name,
                               (select distinct MONTH+', '+convert(varchar(10),YEAR)  from FINAL_GRADES fg where fg.regdno=s.REGDNO and DT_TIME=(select max(dt_time)  from  FINAL_GRADES where regdno='" + userid + "' ) and year=(select max(YEAR)  from  FINAL_GRADES where regdno='" + userid + @"')) Month_Year,convert(varchar(10),DOJ,105) DOA ,
                               (select convert(varchar(10),max(DT_TIME),105) from FINAL_GRADES f where f.REGDNO=s.REGDNO ) Doc                                 
                               from  STUDENT_MASTER s  where regdno='" + userid + "'";
                return await Task.FromResult(GetAllData61<CDLCertificates>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<CDLCertificates>> getdateofcompletion(string userid)
        {
            try
            {
                var query = @"select replace(CONVERT(CHAR(15), max(DT_TIME), 106),' ','-') DOC from new_results_subject s where regdno = '" + userid + "'";
                return await Task.FromResult(GetAllData52<CDLCertificates>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CDLHallticket>> getCDLRegular(string userid)
        {
            try
            {
                var query = @" SELECT * FROM CDL_HALLTICKET_DETAILs where REGDNO= '" + userid + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDLHallticket>> getCDLRegularStudent(string userid)
        {
            try
            {
                //var query = @"select * from student_master where regdno='" + userid + "' and (TALLY_FLAG ='' OR TALLY_FLAG='A' OR TALLY_FLAG = NULL)";
                var query = @"select * from student_master where regdno='" + userid + "'";
                return await Task.FromResult(GetAllData<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLHallticket>> getCDLSupply(string userid)
        {
            try
            {
                var query = @"select sem as CURR_SEM,Type,* from CDL_SUPPLY_STUDENTs where supply_flag = 'Y' and Type='Supply' and REGDNO='" + userid + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDLHallticket>> getCDLBetterment(string userid)
        {
            try
            {
                var query = @"select Type,* from CDL_SUPPLY_STUDENTs where supply_flag = 'Y' and Type='Betterment' and REGDNO='" + userid + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLHallticket>> getCDLsubBranch(string colgcode, string branch)
        {
            try
            {
                var query = @"SELECT * FROM BRANCH_MASTER where COLLEGE_CODE='CDL' and BRANCH_CODE='" + branch + "'";
                return await Task.FromResult(GetAllData<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLHallticket>> getCDLSupplySubjects(string userid)
        {
            try
            {
                var query = @"select SUB_NAME AS subject_name,SUB_CODE AS subject_code,PASSKEY AS TIMINGS,sem as CURR_SEM,sub_date as exam_date,* from CDL_SUPPLY_SUBJECTs where supply_flag = 'Y' and Type='Supply' and REGDNO='" + userid + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<CDLHallticket>> getCDLBettermentSubjects(string userid)
        {
            try
            {
                var query = @"select SUB_NAME AS subject_name,SUB_CODE AS subject_code,PASSKEY AS TIMINGS,sem as CURR_SEM,sub_date as exam_date,* from CDL_SUPPLY_SUBJECTs where supply_flag = 'Y' and Type='Betterment' and REGDNO='" + userid + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLHallticket>> getCDLSupplySubjectstIMINGS(string sub_code, string batch, string semester, string course)
        {
            try
            {
                var query = @"SELECT TIMINGS,SUBJECT_CODE,SUBJECT_NAME FROM CDL_SUBJECT_MASTER WHERE SUBJECT_CODE='" + sub_code + "' AND BATCH='" + batch + "' AND  SEMESTER='" + semester + "' AND COURSE_CODE='" + course + "'";
                return await Task.FromResult(GetAllData60CDL<CDLHallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Certificatesnew>> getcertificate_labels(string WhreCond, string changing)
        {
            try
            {
                string Query = "";
                if (changing == "Please apply the below certificates for name changing")
                {
                    Query = @"SELECT Distinct  * FROM  CERTIFICATES_FEE_STRUCTURE WHERE CERTIFICATE_TYPE in('CNS','CNG') and   IS_ACTIVE='Y' " + WhreCond + " order by Corder asc";
                }

                else
                {
                    Query = @"SELECT Distinct  * FROM  CERTIFICATES_FEE_STRUCTURE WHERE IS_ACTIVE='Y' " + WhreCond + " order by Corder asc";
                }

                return await Task.FromResult(GetAllData19<Certificatesnew>(Query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Certificatesnew>> getsemester(string regdno)
        {
            List<Certificatesnew> pm = null;
            try
            {
                var query = "select  distinct (Convert(varchar,semester)+' '+'Semester'+' '+month+' '+year) as semesterdropdown from new_results_student1 where regdno='" + regdno + "'";
                pm = await Task.FromResult(GetAllData52<Certificatesnew>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }
        public async Task<IEnumerable<Certificatesnew>> semselectedoriginal(string sem, string month, string year, string regdno)
        {
            List<Certificatesnew> pm = null;
            try
            {
                var query = "select max(distinct convert(datetime, ('01' + '/' + month + '/' + year) )) as date from new_results_student1 where regdno = '" + regdno + "'";
                pm = await Task.FromResult(GetAllData52<Certificatesnew>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }

        public async Task<IEnumerable<Certificatesnew>> getcertificate_original_duplicate(string regdno, string certificatetype)
        {
            try
            {
                string Query2 = "";

                Query2 = @"  select distinct  a.REQUEST_DESC,c.CERTIFICATE_TYPE ,a.REQUEST_TYPE from STUDENT_EVALUATION_REQUESTS a, FEE_CHALLAN_MASTER_DOE b,CERTIFICATES_FEE_STRUCTURE c
                                 where a.REF_ID = b.REFID and a.CATEGORY = 'CERTIFICATE' and a.REQUEST_TYPE = 'ORIGINAL' and a.REGDNO = b.REGDNO and b.PAYMENT_STATUS = 'Y'  and c.REQ_DESC = a.REQUEST_DESC
                                and a.REGDNO = '" + regdno + "' and c.CERTIFICATE_TYPE = '" + certificatetype + "' and c.CERTIFICATE_TYPE not in( 'OS') ";

                return await Task.FromResult(GetAllData19<Certificatesnew>(Query2, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Certificatesnew>> semselectedonchange(string sem, string month, string year, string regdno, string certype)
        {
            List<Certificatesnew> pm = null;
            try
            {
                var query = "select distinct  a.REQUEST_DESC,c.CERTIFICATE_TYPE ,a.REQUEST_TYPE,a.month,a.year,a.SEMESTER from STUDENT_EVALUATION_REQUESTS a, FEE_CHALLAN_MASTER_DOE b,CERTIFICATES_FEE_STRUCTURE c where a.REF_ID = b.REFID and a.CATEGORY = 'CERTIFICATE' and a.REQUEST_TYPE = 'ORIGINAL' and a.REGDNO = b.REGDNO and b.PAYMENT_STATUS = 'Y'  and c.REQ_DESC = a.REQUEST_DESC and a.REGDNO = '" + regdno + "' and c.CERTIFICATE_TYPE = '" + certype + "' and a.month = '" + month + "' and a.year = '" + year + "' and a.semester = '" + sem + "'";
                return await Task.FromResult(GetAllData19<Certificatesnew>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }
        public async Task<int> getreference_id()
        {
            List<Certificatesnew> pm = null;
            int refid = 0;
            try
            {
                var query = "SELECT REF_ID FROM STUDENT_EVALUATION_REF_COUNTER";
                pm = await Task.FromResult(GetAllData19<Certificatesnew>(query, null));
                if (pm.Count() > 0)
                {
                    pm[0].REF_IDint = Convert.ToInt32(pm[0].REF_ID);
                    int refid1 = (pm[0].REF_IDint) + 1;
                    refid = refid1;
                }
                var query1 = "update STUDENT_EVALUATION_REF_COUNTER  set ref_id = " + pm[0].REF_IDint + "";
                int j = await Task.FromResult(Update19(query1, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return refid;
        }
        public async Task<int> inserttotals(string regdno, string sname, string category, string totalamount, string refid)
        {
            List<Certificatesnew> pm = null;

            try
            {
                var query = "insert into STUDENT_EVALUATION_FEE_TOTALS (REGDNO,NAME,TOTAL_FEE,DT_TIME,ref_id,feetype) VALUES ";
                query = query + "('" + regdno + "','" + sname + "','" + totalamount + "',GETDATE(),'" + refid + "','" + category + "')";

                int j = await Task.FromResult(InsertData24(query, null));
                return j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<int> insertdatacertificate(int referinceid, string branch_code, string college, string regdno, string coursecode, string BATCH, string currentsem, string certificate, string type, string category, string sem, string month, string year, int amt, int bypost, string degreecode, string sessionid, string pubfilepath, string campus, string sname)
        {
            string query = ""; string insertqry = ""; string deleteqry = "";
            int k = 0;
            List<Certificatesnew> fee = null;
            try
            {
                if (certificate.Trim().ToUpper().Equals("GRADECARD"))
                {
                    query = "SELECT s.regdno,s.name,S.REQUEST_DESC FROM STUDENT_EVALUATION_REQUESTS s, FEE_CHALLAN_MASTER_DOE F WHERE S.REGDNO = F.REGDNO  AND S.REF_ID = F.REFID AND S.REGDNO = '" + regdno + "' and S.REQUEST_DESC ='" + certificate + "'  and F.semester = '" + currentsem + "' AND F.PAYMENT_STATUS = 'Y'  and REQUEST_TYPE='" + type + "' ";
                    fee = await Task.FromResult(GetAllData19<Certificatesnew>(query, null));
                    if (fee.Count() <= 0)
                    {
                        deleteqry = "delete from STUDENT_EVALUATION_REQUESTS where REGDNO='" + regdno + "' and CATEGORY='" + category + "' and REQUEST_DESC='" + certificate + "' and CAMPUS_CODE='" + campus + "' and DEGREE_CODE='" + degreecode + "'  and convert(varchar(10), REQUEST_DATE,105)='" + System.DateTime.Now.ToString("dd-MM-yyyy") + "' and SESSIONID='" + sessionid + "' and REQUEST_TYPE='" + type + "' and semester=" + sem + " and month='" + month + "' and year='" + year + "' ";
                        int p = await Task.FromResult(Delete19(query, null));

                    }
                    insertqry = "insert into STUDENT_EVALUATION_REQUESTS(REGDNO, NAME, CATEGORY, REQUEST_DESC, REQUEST_TYPE, FEE, REQUEST_DATE, CAMPUS_CODE, COLLEGE_CODE, DEGREE_CODE, COURSE_CODE, BRANCH_CODE, semester, SESSIONID, BYPOST, REF_ID, UploadedPath, Batch, month, year) VALUES ";
                    insertqry = insertqry + "('" + regdno + "','" + sname + "','" + category + "','" + certificate + "','" + type + "','" + amt + "',getdate(),'" + campus + "','" + college + "','" + degreecode + "','" + coursecode + "','" + branch_code + "','" + currentsem + "','" + sessionid + "','" + bypost + "','" + referinceid + "','" + pubfilepath + "','" + BATCH + "','" + month + "','" + year + "')";
                    k = await Task.FromResult(InsertData19(insertqry, null));
                }
                else
                {
                    if (certificate.Trim().ToUpper().Equals("CREDENTIAL VERIFICATION"))
                    {
                        type = type.ToUpper().Trim().Equals("DUPLICATE") ? "Abroad" : "India";
                    }
                    query = "SELECT s.regdno,s.name,S.REQUEST_DESC FROM STUDENT_EVALUATION_REQUESTS s, FEE_CHALLAN_MASTER_DOE F WHERE S.REGDNO = F.REGDNO  AND S.REF_ID = F.REFID AND S.REGDNO = '" + regdno + "' and S.REQUEST_DESC ='" + certificate + "'  and F.semester = '" + currentsem + "' AND F.PAYMENT_STATUS = 'Y'  and REQUEST_TYPE='" + type + "' ";
                    fee = await Task.FromResult(GetAllData19<Certificatesnew>(query, null));
                    if (fee.Count() <= 0)
                    {
                        deleteqry = "delete from STUDENT_EVALUATION_REQUESTS where REGDNO='" + regdno + "' and CATEGORY='" + category + "' and REQUEST_DESC='" + certificate + "' and CAMPUS_CODE='" + campus + "' and DEGREE_CODE='" + degreecode + "'  and convert(varchar(10), REQUEST_DATE,105)='" + System.DateTime.Now.ToString("dd-MM-yyyy") + "' and SESSIONID='" + sessionid + "' and REQUEST_TYPE='" + type + "'  ";
                        int p = await Task.FromResult(Delete19(query, null));

                    }
                    insertqry = "insert into STUDENT_EVALUATION_REQUESTS(REGDNO, NAME, CATEGORY, REQUEST_DESC, REQUEST_TYPE, FEE, REQUEST_DATE, CAMPUS_CODE, COLLEGE_CODE, DEGREE_CODE, COURSE_CODE, BRANCH_CODE, semester, SESSIONID, BYPOST, REF_ID, UploadedPath, Batch) VALUES ";
                    insertqry = insertqry + "('" + regdno + "','" + sname + "','" + category + "','" + certificate + "','" + type + "','" + amt + "',getdate(),'" + campus + "','" + college + "','" + degreecode + "','" + coursecode + "','" + branch_code + "','" + currentsem + "','" + sessionid + "','" + bypost + "','" + referinceid + "','" + pubfilepath + "','" + BATCH + "')";
                    k = await Task.FromResult(InsertData19(insertqry, null));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }
        public async Task<IEnumerable<Certificatesnew>> getcertificateconfirmation(string regdno, string refid, string stname, string sessionid)
        {
            List<Certificatesnew> pm = null;
            try
            {

                var query1 = "update STUDENT_EVALUATION_REQUESTS set DISPLAY_FLAG='Y' WHERE SESSIONID = '" + sessionid + "'";
                int j = await Task.FromResult(Update19(query1, null));
                var query = "SELECT (case when  (REQUEST_DESC='GRADECARD')then( REQUEST_DESC+'_'+Convert(varchar,semester)+'_'+Month+'_'+Year) else  REQUEST_DESC end )as 'REQUEST_DESCRIPTION',REQUEST_DESC,request_type as 'REQUEST_TYPE',fee AS 'FEE',BYPOST,REQUEST_DATE AS 'DATE',COURSE_CODE AS 'COURSE',BRANCH_CODE AS 'BRANCH' FROM STUDENT_EVALUATION_REQUESTS WHERE REGDNO = '" + regdno + "' AND CATEGORY='CERTIFICATE' and  REF_ID = '" + refid + "'";

                return await Task.FromResult(GetAllData19<Certificatesnew>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<studenttrack>> getGradeCardDetails(string userid)
        {
            try
            {
                //  var query = @"select distinct year as Year ,month as Month,process_type,regdno, branch_code, semester as SEMESTER from new_results_student1 where regdno = '" + userid + "' and STATUS NOT IN ('Dt','Dc','Exp') and process_type not in ('G','GV')order by semester desc";


                var query = @"
	WITH AggregatedRecords AS (
    SELECT 
        year AS Year, 
        month AS Month,
        CASE 
            WHEN process_type IN ('ST', 'RBRR') AND month = 'Jun' AND year = 2024 THEN 'SummerTerm 2' 
            ELSE process_type 
        END AS process_type, 
        regdno, 
        branch_code, 
        MAX(semester) AS SEMESTER
    FROM  
        new_results_student1  WHERE regdno = '" + userid + "'   AND STATUS NOT IN ('Dt', 'Dc', 'Exp') AND process_type NOT IN ('G', 'GV')  GROUP BY year,  month,semester ,     CASE WHEN process_type IN ('ST', 'RBRR') AND month = 'Jun' AND year = 2024 THEN 'SummerTerm 2'  ELSE process_type END, regdno, branch_code) SELECT * FROM AggregatedRecords ORDER BY SEMESTER DESC";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<studenttrack>> getmonth(string semester, string userid)
        {
            try
            {
                var query = @"select distinct month as Month,year as Year from new_results_student1 where regdno = '" + userid + "' and semester = '" + semester + "' and  STATUS NOT IN ('Dt','Dc','Exp') order by month";
                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Event>> getdashboardevents(string campus)
        {
            try
            {
                var query = @"select top 5 EventDescription,Event_dates,eventid,url,eventday as eventday,Eventmonth,case  campus_code WHEN 'VSP' then 'Visakhapatnam' when 'BLR' then 'Bengaluru' else 'Hyderabad' end as campus,ConfirmFlag,Convert(Date, EventDate) as EventDate ,eventid,EventName,substring(eventlocation,1,25) as EventLocation,EventDate ,convert(varchar,start_date,107) as Date,End_date,Uploadtype,url,Contact_person,Contact_number,Contact_email,IMAGE_PATH,organizedby from GITAM_EVENTS_INSERT where Convert(Date, EventDate) >=Convert(Date, getdate()) and campus_code='" + campus + "'  and ConfirmFlag='Y'  order by start_date";
                return await Task.FromResult(GetAllData60website<Event>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Phdbiometric>> getphdbiometricreport(string userid, string month, string year)
        {
            var qry = "";
            try
            {
                qry = qry + " select ROW_NUMBER() OVER(order by userid, convert(varchar(12), eventdate, 109)) AS Row, convert(varchar(12), eventdate, 109) as Date , (select phdregdno from PHD_VS_USERID where status = 'S' and phduserid=h.userid)as  [Regdno],(select name from PHD_VS_USERID where status = 'S' and phduserid=h.userid)as  [Name],convert(char(5), min(eventtime), 108) as CheckIN, convert(char(5), max(eventtime), 108) as CheckOUT,";
                qry = qry + " convert(varchar(10), DateDiff(s, MIn(eventtime), Max(eventtime)) / 3600) + ':' + convert(varchar(10), DateDiff(s, MIn(eventtime), Max(eventtime)) % 3600 / 60) as [Duration]";
                qry = qry + " from history h where";
                qry = qry + " userid in(select phduserid from PHD_VS_USERID where status = 'S' and phdregdno = '" + userid + "')";
                qry = qry + " and month(eventdate) = '" + month + "' and year(eventdate) = '" + year + "'";
                qry = qry + " group by userid, convert(varchar(12), eventdate, 109)  order by userid, convert(varchar(12), eventdate, 109)";


                return await Task.FromResult(GetAllData228<Phdbiometric>(qry, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<Degreeplan> getcreditperformance(Degreeplan chart)
        {
            try
            {
                var query = "select cgpa as CGPA,sgpa as SGPA,sem_credits as SEM_CREDITS,semester as SEMESTER,CUM_CREDITS from NEW_RESULTS_STUDENT  where regdno='" + chart.REGDNO + "' and semester ='" + chart.SEMESTER + "' and campus_code='" + chart.CAMPUS + "' and branch_code='" + chart.BRANCH + "'";
                var data52 = await Task.FromResult(GetAllData52<Degreeplan>(query, null));
                if (data52.Count > 0)
                {
                    chart.CGPA = data52[0].CGPA;
                    chart.SGPA = data52[0].SGPA;
                    chart.SEM_CREDITS = data52[0].SEM_CREDITS;
                    chart.SEMESTER = data52[0].SEMESTER;
                    chart.CUM_CREDITS = data52[0].CUM_CREDITS;
                }
                else
                {
                    chart.CGPA = "";
                    chart.SGPA = "";
                    chart.SEM_CREDITS = "";
                    chart.SEMESTER = "";
                    chart.CUM_CREDITS = "";
                }

                var query1 = "select UC as UC,FC as FC,PC as PC,PE as PE,OE as OE,MIC as MIC,total as totalcreditssum,* from TBL_DISPLAY_CREDITS_DETAILS where REGDNO='" + chart.REGDNO + "' and DISPLAY_TYPE='total'";
                var data42 = await Task.FromResult(GetAllData42<Degreeplan>(query1, null));
                if (data42.Count > 0)
                {
                    chart.UC = data42[0].UC;
                    chart.FC = data42[0].FC;
                    chart.PC = data42[0].PC;
                    chart.PE = data42[0].PE;
                    chart.OE = data42[0].OE;
                    chart.MIC = data42[0].MIC;
                    chart.totalcreditssum = data42[0].totalcreditssum;
                }
                else
                {
                    chart.UC = "";
                    chart.FC = "";
                    chart.PC = "";
                    chart.PE = "";
                    chart.OE = "";
                    chart.MIC = "";
                    chart.totalcreditssum = "";
                }
                var query2 = "select UC as UC_COUNT,FC as FC_COUNT,PC as PC_COUNT,PE as PE_COUNT,OE as OE_COUNT,MIC as MIC_COUNT,total as totalcreditssum2  from TBL_DISPLAY_CREDITS_DETAILS where REGDNO='" + chart.REGDNO + "' and DISPLAY_TYPE='Registered till last semester'";
                var data421 = await Task.FromResult(GetAllData42<Degreeplan>(query2, null));
                if (data421.Count > 0)
                {
                    chart.UC_COUNT = data421[0].UC_COUNT;
                    chart.FC_COUNT = data421[0].FC_COUNT;
                    chart.PC_COUNT = data421[0].PC_COUNT;
                    chart.PE_COUNT = data421[0].PE_COUNT;
                    chart.OE_COUNT = data421[0].OE_COUNT;
                    chart.MIC_COUNT = data421[0].MIC_COUNT;
                    chart.totalcreditssum2 = data42[0].totalcreditssum2;
                }
                else
                {
                    chart.UC_COUNT = "";
                    chart.FC_COUNT = "";
                    chart.PC_COUNT = "";
                    chart.PE_COUNT = "";
                    chart.OE_COUNT = "";
                    chart.MIC_COUNT = "";
                    chart.totalcreditssum2 = "";
                }
            }



            catch (Exception ex)
            {
                throw ex;
            }
            return chart;

        }
        public async Task<IEnumerable<Degreeplan>> getsubjects(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'UC'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Degreeplan>> getsubjectsOE(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'OE'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Degreeplan>> getsubjectsFC(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'FC'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Degreeplan>> getsubjectsPC(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'PC'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Degreeplan>> getsubjectsPE(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'PE'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Degreeplan>> getsubjectsMIC(Degreeplan chart)
        {
            string query = "";
            try
            {

                query = ("select  cs.CBCS_CATEGORY,cs.SUBJECT_CODE,cs.SUBJECT_NAME from CBCS_SUBJECT_MASTER sub,CBCS_STUDENT_SUBJECT_ASSIGN cs where cs.SUBJECT_CODE COLLATE Latin1_General_CI_AS = sub.SUBJECT_CODE  and cs.SEMESTER = sub.SEMESTER and cs.semester = '" + chart.SEMESTER + "' and cs.BRANCH_CODE = '" + chart.BRANCH + "' and cs.CAMPUS_CODE = sub.CAMPUS_CODE and cs.BRANCH_CODE = sub.BRANCH_CODE and cs.COLLEGE_CODE = sub.COLLEGE_CODE and cs.DEPT_CODE = sub.dept_code and cs.campus_code = '" + chart.CAMPUS + "' and cs.REGDNO = '" + chart.REGDNO + "' and cs.CBCS_CATEGORY = 'MOC'");
                return await Task.FromResult(GetAllData42<Degreeplan>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Degreeplan>> getremainingcredits(String regdno)
        {
            string query = "";
            try
            {

                //  var query2 = "select UC as UC_remain,FC as FC_remain,PC as PC_remain,PE as PE_remain,OE as OE_remain,MIC as MIC_remain  from TBL_DISPLAY_CREDITS_DETAILS where REGDNO='" + chart.REGDNO + "' and DISPLAY_TYPE='Remaining credits'";
                var query2 = "select DISPLAY_TYPE AS Category,UC,FC,PC,PE,OE,MIC,TOTAL as [Total] from TBL_DISPLAY_STUDENT_CREDITS where REGDNO ='" + regdno + "'";
                return await Task.FromResult(GetAllData42<Degreeplan>(query2, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Degreeplan>> getselectingcredits(Degreeplan chart)
        {
            string query = "";
            try
            {

                var query2 = "select UC as UC_select,FC as FC_select,PC as PC_select,PE as PE_select,OE as OE_select,MIC as MIC_select from TBL_DISPLAY_CREDITS_DETAILS where REGDNO='" + chart.REGDNO + "' and DISPLAY_TYPE='Credits selected for this semester'";
                return await Task.FromResult(GetAllData42<Degreeplan>(query2, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        //public async Task<IEnumerable<FeedbackMaster>> getfeedback_subjects(string userid, string sem)
        //{
        //    try
        //    {
        //        var query = @"select EMPID,CAMPUS_CODE,COLLEGE_CODE,BRANCH_CODE,SUBJECT as subject_code, SUBJECT_NAME as subjectname,Semester from feedback_student where  regdno = '" + userid + "' and SEMESTER = '" + sem + "' and SUBJECT  not like '%p  ' and  feedback_status='Y'";/*and  SUBJECT_TYPE in('tp'  ,'t','p')*/
        //        return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public async Task<IEnumerable<Degreeplan>> getmedicalappointment(Degreeplan chart)
        {
            string query = "";
            try
            {

                var query2 = "select ROW_NUMBER() OVER(order by a.DTTIME desc) AS row,M.ID as [ID], ENAME, EVENUE,CONVERT(varchar, EDATE,105) as [EDATE],SLOTFROMTIME as [FROMTIME],SLOTTOTIME as [TOTIME],slot from TBL_MEDICAL_APPOINTMENT_SCHEDULING_NEW M, TBL_STUDENT_MEDICAL_APPOINTMENT A where m.ID = a.AppointmentID and a.REGDNO = '" + chart.REGDNO + "' order by a.DTTIME desc";
                return await Task.FromResult(GetAllData19medical<Degreeplan>(query2, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Degreeplan>> getmedicallists(Degreeplan chart)
        {
            string query = "";
            try
            {
                var query2 = "SELECT ROW_NUMBER() OVER(order by DTTIME desc) AS row,ID, ENAME,appointment_flag, EVENUE,CONVERT(varchar, EDATE,105) as  [EDATE],SLOTFROMTIME as [FROMTIME],SLOTTOTIME as [TOTIME],slot, DTTIME, USERID, MAXCOUNT,(CONVERT(int, MAXCOUNT) - CONVERT(int, ALLOTEDCOUNT)) as ALLOTEDCOUNT FROM TBL_MEDICAL_APPOINTMENT_SCHEDULING_NEW  M where id not in (select distinct AppointmentID from TBL_STUDENT_MEDICAL_APPOINTMENT where REGDNO = '" + chart.REGDNO + "'and AppointmentID = m.ID) and  ename not in (SELECT EENAME from TBL_STUDENT_MEDICAL_APPOINTMENT where REGDNO = '" + chart.REGDNO + "' and eename=m.ename and eedate=m.edate) and MAXCOUNT != ALLOTEDCOUNT  order by DTTIME desc";
                return await Task.FromResult(GetAllData19medical<Degreeplan>(query2, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Degreeplan>> checkappointment(string id)
        {
            try
            {
                var query = @"SELECT ID,ENAME,EVENUE,EDATE,SLOTFROMTIME as [FROMTIME],SLOTTOTIME as [TOTIME],slot,DTTIME,USERID,MAXCOUNT,ALLOTEDCOUNT FROM TBL_MEDICAL_APPOINTMENT_SCHEDULING_NEW where ID='" + id + "'";
                return await Task.FromResult(GetAllData19medical<Degreeplan>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Degreeplan> updateflagappointment(Degreeplan chart)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo("en-US"); // Specify the appropriate culture
                DateTime dateTime = DateTime.ParseExact(chart.EDATE, "dd-MM-yyyy", cultureInfo);

                int j = 0;
                string Query = "";

                Query = "update TBL_MEDICAL_APPOINTMENT_SCHEDULING_NEW set appointment_flag='Y' where EVENUE='" + chart.EVENUE + "' and EDATE='" + dateTime + "' and ENAME='" + chart.ENAME + "' and  id='" + chart.AppointmentID + "'";

                j = await Task.FromResult(Update19medical(Query, null));


                chart.flag = j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }
        public async Task<Degreeplan> updateappointment(Degreeplan chart)
        {
            try
            {
                int j = 0;
                string Query = "";

                Query = "update TBL_MEDICAL_APPOINTMENT_SCHEDULING_NEW set ALLOTEDCOUNT=ALLOTEDCOUNT+1 where id='" + chart.AppointmentID + "'";

                j = await Task.FromResult(Update19medical(Query, null));


                chart.flag = j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }
        public async Task<Degreeplan> insertappointment(Degreeplan chart)
        {
            try
            {
                CultureInfo cultureInfo = new CultureInfo("en-US"); // Specify the appropriate culture
                DateTime dateTime = DateTime.ParseExact(chart.EDATE, "dd-MM-yyyy", cultureInfo);

                int j = 0;
                string Query = "";

                Query = "INSERT INTO TBL_STUDENT_MEDICAL_APPOINTMENT(regdno, appointmentid, dttime, EENAME, EEVENUE, EEDATE)VALUES('" + chart.REGDNO + "', '" + chart.AppointmentID + "', getdate(), '" + chart.ENAME + "', '" + chart.EVENUE + "', '" + dateTime + "')";

                j = await Task.FromResult(InsertData19medical(Query, null));


                chart.flag = j;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;


        }



        public async Task<List<CDLEvaluationReceipts>> getEvaluationStudentDetails(string userid)
        {
            try
            {
                var query = @"SELECT CAMPUS,COLLEGE_CODE,BRANCH_CODE,DEGREE_CODE,COURSE_CODE,FATHER_NAME,NAME,REGDNO,CLASS,CURR_SEM,batch,additional_info FROM STUDENT_MASTER WHERE REGDNO = '" + userid + "' AND COLLEGE_CODE  IN ('CDL')";
                return await Task.FromResult(GetAllData<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLEvaluationReceipts>> getCDLFeeHistory(string userid)
        {
            try
            {

                var query = @"select Fee_Type as Type,Challan_no,Txn_ID,PaymentMode,College as COLLEGE_CODE,Campus,Refid,others2_desc as Exam_Type,others3_desc as Category,Payment_Date AS PAYMENT_DATE,Payment_Status,Total_fee from CDL_fee_challan_master where regdno='" + userid + "' and payment_status='Y' order by payment_date";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<CDLEvaluationReceipts>> getCDLEvalFeeHistory(string userid)
        {
            try
            {

                var query = @"SELECT S.REGDNO,ISNULL(S.SUB_CODE,'') as SUBJECT_CODE,ISNULL(S.SUB_NAME,'') AS SUBJECT_NAME,ISNULL(S.SEMESTER,'') AS SEMESTER,S.REQUEST_DESC AS PROCESS_TYPE,SM.PAYMENT_DATE,SM.REFID FROM CDL_STUDENT_EVALUATION_REQUESTS S,CDL_fee_challan_master SM where S.REGDNO = SM.REGDNO AND S.REF_ID = SM.REFID AND SM.PAYMENT_STATUS = 'Y' AND  S.REGDNO = '" + userid + "' ORDER BY SM.PAYMENT_DATE";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<CDLEvaluationReceipts>> getCDLSupplyDetails(string processtype, string degree, string course, string campus, string college)
        {
            try
            {

                var query = @"select PROCESS as PROCESS_TYPE,YEAR,MONTH,convert(varchar,LASTDATE,105) as LASTDATE,convert(varchar,LASTDATE1,105) as LASTDATE1,LASTDATEAMOUNT1,convert(varchar,LASTDATE2,105) as LASTDATE2,LASTDATEAMOUNT2 from CDL_DOE_DATES_INFO where process='" + processtype + "' and degree='" + degree + "' and course='" + course + "' and campus='" + campus + "' and college='" + college + "' and year = '2023'";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLEvaluationReceipts>> getCDLEvaluationSem(string college)
        {
            try
            {

                var query = @"select ISNULL(SEMTYPE,'ODD') as SEMTYPE from STUDENT_EVALUATION_SEMESTERS where COLLEGE_CODE = '" + college + "' AND ( MONTH1 = MONTH(GETDATE()) OR MONTH2 = MONTH(GETDATE()) OR MONTH3 = MONTH(GETDATE()) OR MONTH4 = MONTH(GETDATE()) OR MONTH5 = MONTH(GETDATE()) OR MONTH6 = MONTH(GETDATE()))";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLEvaluationReceipts>> getCDLfeechallan(string userid)
        {
            try
            {

                var query = @"select * from fee_challan_master where regdno = '" + userid + "' and others1_desc like '%reg%' and college= 'CDL'";
                return await Task.FromResult(GetAllData<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<CDLEvaluationReceipts>> getCDLlastdate(string processtype, string degree, string course, string campus, string college)
        {
            try
            {

                var query = @"select convert(varchar,LASTDATE,105) as LASTDATE,lastdateamount as lastdateamount,
                        convert(varchar,LASTDATE1,105) as LASTDATE1,lastdateamount1 as LASTDATEAMOUNT1,convert(varchar,getdate(),105) as getdate
                        ,convert(varchar,LASTDATE2,105) as LASTDATE2,lastdateamount2 as LASTDATEAMOUNT2,month as MONTH,year AS YEAR from CDL_DOE_DATES_INFO where process='" + processtype + "' and degree='" + degree + "' and course='" + course + "' and campus='" + campus + "' and college='" + college + "' and year = '2023'";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<CDLEvaluationReceipts>> getCDLExamCentre()
        {
            try
            {

                var query = @"select distinct(exam_centre) as exam_centre from exam_centre_master where exam_centre is not NULL AND exam_centre != ''";
                return await Task.FromResult(GetAllData<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CDLEvaluationReceipts>> getCDLSupplySubjects(string userid, string vsemlist)
        {
            try
            {

                var query = @"SELECT (SUBJECT_CODE+'-'+SUBJECT_NAME),SEMESTER,subject_code,subject_name FROM CDL_EVALUATION_RESULTS WHERE REGDNO = '" + userid + "' and GRADE in ('F','Ab')  and  subject_name not like  '%viva%' and subject_name not like  '%Project Work%' and SUB_TYPE in('T','TP') and semester  " + vsemlist + "";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<CDLEvaluationReceipts>> getCDLProjectResults(string userid, string vsemlist)
        {
            try
            {

                var query = @"SELECT * FROM CDL_EVALUATION_RESULTS WHERE REGDNO = '" + userid + "' and GRADE in ('F','Ab') and subject_name  like  '%project%' and semester " + vsemlist + "";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<CDLEvaluationReceipts>> getCDLRefid()
        {
            try
            {

                var query = @"SELECT REF_ID as Refid FROM CDL_STUDENT_EVALUATION_REF_COUNTER";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> updateCDLRefid(int refid)
        {
            int j = 0;
            try
            {

                var query = @"update CDL_STUDENT_EVALUATION_REF_COUNTER  set ref_id = " + refid + "";
                j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }

        public async Task<int> InsertEvaluationRequest(List<CDLEvaluationReceipts> list)
        {
            int j = 0;
            var query = "";
            string vsubcode = "";
            try
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].SUBJECT_CODE == "Project & Viva")
                    {
                        vsubcode = list[i].SUBJECT_CODE;
                    }
                    else
                    {
                        vsubcode = list[i].SUBJECT_CODE + "-" + list[i].SUBJECT_NAME;
                    }

                    list[i].Category = "EXAMINATION";
                    // string requestdesc = "SUPPLEMENTARY";
                    string requestdesc = list[i].Type;
                    string requesttype = "ALL";
                    query = "insert into CDL_STUDENT_EVALUATION_REQUESTS (REGDNO,NAME,CATEGORY,REQUEST_DESC,REQUEST_TYPE,FEE,REQUEST_DATE,CAMPUS_CODE," +
                    "COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,SUBJECT_CODE,SESSIONID, REF_ID,semester,sub_code,sub_name,batch,Exam_centre) VALUES ";
                    query = query + "('" + list[i].REGDNO + "','" + list[i].NAME + "','" + list[i].Category + "','" + requestdesc + "','" + requesttype + "'," +
                        "'" + list[i].Total_fee + "',getdate(),'" + list[i].CAMPUS + "','" + list[i].COLLEGE_CODE + "','" + list[i].DEGREE_CODE + "','" + list[i].COURSE_CODE + "','" + list[i].BRANCH_CODE + "','"
                        + vsubcode + "','" + list[i].sessionid + "','" + list[i].Refid + "','" + list[i].SEMESTER + "','" + list[i].SUBJECT_CODE + "','" + list[i].SUBJECT_NAME + "','" + list[i].batch + "', '" + list[i].exam_centre + "')";

                    j = await Task.FromResult(Insert19(query, null));
                }

                //j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }



        public async Task<int> InsertTotalFee(String regdno, String name, String totalfee, String refid, String feetype)
        {
            int j = 0;
            var query = "";
            string vsubcode = "";
            try
            {

                query = "insert into CDL_STUDENT_EVALUATION_FEE_TOTALS (REGDNO,NAME,TOTAL_FEE,DT_TIME,ref_id,feetype) VALUES ";
                query = query + "('" + regdno + "','" + name + "','" + totalfee + "',GETDATE(),'" + refid + "','" + feetype + "')";

                j = await Task.FromResult(Insert19(query, null));

                //j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }


        public async Task<List<CDLEvaluationReceipts>> getReceiptconfirmation(string regno, string SessionID)
        {
            try
            {

                var query = @"SELECT REGDNO,NAME,REQUEST_DESC,SUB_CODE as SUBJECT_CODE,SUB_NAME as SUBJECT_NAME,SEMESTER,EXAM_CENTRE as exam_centre,COURSE_CODE,BRANCH_CODE,FEE as Total_fee,REQUEST_DESC,REQUEST_DATE from CDL_STUDENT_EVALUATION_REQUESTS WHERE REGDNO = '" + regno + "' AND CATEGORY='EXAMINATION' AND SESSIONID = '" + SessionID + "' AND DISPLAY_FLAG IS NULL";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<int> UpdateCDLevaluationRequest(string sessionid)
        {
            int j = 0;
            try
            {

                var query = @"update CDL_STUDENT_EVALUATION_REQUESTS set DISPLAY_FLAG='Y' WHERE SESSIONID = '" + sessionid + "'";
                j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }



        public async Task<List<CDLEvaluationReceipts>> getCDLRevalutionResult(string userid, string vsemlist, string month, string year)
        {
            try
            {

                var query = @"SELECT (SUBJECT_CODE+'-'+SUBJECT_NAME),SEMESTER,subject_code,subject_name FROM CDL_EVALUATION_RESULTS WHERE REGDNO = '" + userid + "' AND SUB_TYPE = 'T' and semester " + vsemlist + " and month ='" + month + "' and year = " + year + "";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<int> InsertCertificatedata(CDLEvaluationReceipts data)
        {
            int j = 0;
            var query = "";
            string vsubcode = "";
            try
            {

                query = "insert into CDL_STUDENT_EVALUATION_REQUESTS (REGDNO,NAME,CATEGORY,REQUEST_DESC,REQUEST_TYPE,FEE,REQUEST_DATE,CAMPUS_CODE,COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,semester,SESSIONID,BYPOST,REF_ID) VALUES ";
                query = query + "('" + data.REGDNO + "','" + data.NAME + "','" + data.Category + "','" + data.certificate + "','" + data.Type + "','" + data.amount + "',getdate(),'" + data.CAMPUS + "','" + data.COLLEGE_CODE + "','" + data.DEGREE_CODE + "','" + data.COURSE_CODE + "','" + data.BRANCH_CODE + "','" + data.SEMESTER + "','" + data.sessionid + "','" + data.bypost + "','" + data.Refid + "' )";

                j = await Task.FromResult(Insert19(query, null));

                //j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }


        public async Task<int> InsertCertificatedataTotal(CDLEvaluationReceipts data)
        {
            int j = 0;
            var query = "";
            string vsubcode = "";
            try
            {

                query = "insert into CDL_STUDENT_EVALUATION_FEE_TOTALS (REGDNO,NAME,TOTAL_FEE,DT_TIME,ref_id,feetype) VALUES ";
                query = query + "('" + data.REGDNO + "','" + data.NAME + "','" + data.Total_fee + "',GETDATE(),'" + data.Refid + "','" + data.Type + "')";

                j = await Task.FromResult(Insert19(query, null));

                //j = await Task.FromResult(Update19(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }


        public async Task<List<CDLEvaluationReceipts>> getReceiptconfirmationCertificate(string regno, string SessionID)
        {
            try
            {

                //var query = @"SELECT REGDNO,NAME,REQUEST_DESC,SUB_CODE as SUBJECT_CODE,SUB_NAME as SUBJECT_NAME,SEMESTER,EXAM_CENTRE as exam_centre,COURSE_CODE,BRANCH_CODE,FEE as Total_fee,REQUEST_DESC,REQUEST_DATE from CDL_STUDENT_EVALUATION_REQUESTS WHERE REGDNO = '" + regno + "' AND CATEGORY='EXAMINATION' AND SESSIONID = '" + SessionID + "' AND DISPLAY_FLAG IS NULL";
                var query = @" SELECT REGDNO, NAME, category, REQUEST_DESC, request_type, fee, BYPOST, REQUEST_DATE, COURSE_CODE, BRANCH_CODE FROM CDL_STUDENT_EVALUATION_REQUESTS WHERE REGDNO = '" + regno + "' AND CATEGORY = 'CERTIFICATE' and SESSIONID = '" + SessionID + "'   ";
                return await Task.FromResult(GetAllData19<CDLEvaluationReceipts>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<FeedbackMaster>> get_groupcode(string campus, string college, string course, string branch, string curr_sem, string subject, string section)
        {
            var query = "select GROUP_CODE as group_code FROM HOD_STAFF_SUBJECTS_MASTER where CAMPUS='" + campus + "' AND COLLEGE='" + college + "' AND COURSE_CODE='" + course + "' AND BRANCH_CODE='" + branch + "' AND SEMESTER='" + curr_sem + "' AND SUBJECT_CODE='" + subject + "' AND SECTION='" + section + "' and DACTIVE_FLAG ='R' order by ID desc";
            return await Task.FromResult(GetAllData42<FeedbackMaster>(query, null));
        }

        public async Task<IEnumerable<Students>> getcredits_currsem(string regdno, string sem)
        {
            string query = "";
            try
            {
                int semadd = Convert.ToInt32(sem) + 1;
                query = "select subject_code, SUBJECT_NAME, subject_type, NEW_CREDITS as credits,CBCS_CATEGORY as CBCS_CATEGORY,SEMESTER as CURRSEM   from CBCS_STUDENT_SUBJECT_ASSIGN where REGDNO = '" + regdno + "' and semester='" + semadd + "'";
                return await Task.FromResult(GetAllData42<Students>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Students>> getplan_currsem(string regdno, string sem)
        {
            string query = "";
            try
            {

                query = "select isnull(regdno,'0') as regdno , isnull(UC,'0') as UC, isnull(FC,'0') as FC, isnull(PC,'0') as PC, isnull(PE,'0') as PE,isnull(OE,'0') as OE, isnull(MIC,'0') as MIC, isnull(TOTAL,'0') as TOTAL,isnull(DISPLAY_TYPE,'0') as DISPLAY_TYPE from TBL_DISPLAY_CREDITS_DETAILS20230915 where REGDNO = '" + regdno + "'";
                return await Task.FromResult(GetAllData42<Students>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



        public async Task<IEnumerable<Timetable>> getpopupfaculty(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string set, string subjectcode, string sec)
        {
            string query = "";
            try
            {

                /*query = @"select distinct h.empid,h.emp_name,h.SUBJECT_NAME,h.campus as campus_code,h.Building_name,h.roomno
from HOD_STAFF_SUBJECTS_MASTER h, TIME_TABLE_MASTER t where t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE
and h.SECTION COLLATE Latin1_General_CI_AS = t.section and h.SEMESTER = t.SEMESTER
and h.batch COLLATE Latin1_General_CI_AS = t.batch and h.CAMPUS COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE
and h.BRANCH_CODE COLLATE Latin1_General_CI_AS = t.BRANCH_CODE and h.SECTION COLLATE Latin1_General_CI_AS = t.SECTION
and t.CAMPUS_CODE = '" + CAMPUS_CODE + "' and h.branch_code = '" + BRANCH_CODE + "'  and h.SEMESTER = '"+SEMESTER+"' and h.SMS is null " +
"and h.class='" + set + "' and h.SUBJECT_CODE='" + subjectcode + "' and h.section= '" + sec + "'";*/

                query = @"select distinct h.empid,h.emp_name,h.SUBJECT_NAME,h.campus as campus_code,h.Building_name,h.roomno
from HOD_STAFF_SUBJECTS_MASTER h, TIME_TABLE_MASTER t where t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE
and h.SECTION COLLATE Latin1_General_CI_AS = t.section and h.SEMESTER = t.SEMESTER
and h.batch COLLATE Latin1_General_CI_AS = t.batch and h.CAMPUS COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE
and h.BRANCH_CODE COLLATE Latin1_General_CI_AS = t.BRANCH_CODE and h.SECTION COLLATE Latin1_General_CI_AS = t.SECTION
and t.CAMPUS_CODE = '" + CAMPUS_CODE + "' and h.branch_code = '" + BRANCH_CODE + "'  and h.SEMESTER = '" + SEMESTER + "' and h.SMS is null " +
"and h.class='" + set + "' and h.SUBJECT_CODE='" + subjectcode + "'";
                return await Task.FromResult(GetAllData42<Timetable>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Students>> getsets(string campus, string branch, string batch, string sem)
        {
            try
            {
                var query = @"select distinct class as class1 from HOD_STAFF_SUBJECTS_MASTER where SMS is null and campus='" + campus + "' and branch_code='" + branch + "' and batch like '" + batch + "%' and SEMESTER='" + sem + "' and class !=''";
                return await Task.FromResult(GetAllData42<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<Timetable>> getsettimetable(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string set)
        {
            try
            {
                string query = "";


                query = @"select distinct h.class as class1,h.emp_name,t.section,h.subject_code,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
 as timeslots ,WEEKDAY from HOD_STAFF_SUBJECTS_MASTER h ,TIME_TABLE_MASTER t where t.SUBJECT_CODE COLLATE Latin1_General_CI_AS=h.SUBJECT_CODE
and h.SECTION COLLATE Latin1_General_CI_AS=t.section and h.SEMESTER =t.SEMESTER 
and h.batch COLLATE Latin1_General_CI_AS=t.batch and h.CAMPUS COLLATE Latin1_General_CI_AS=t.CAMPUS_CODE 
and h.BRANCH_CODE COLLATE Latin1_General_CI_AS=t.BRANCH_CODE and h.SECTION COLLATE Latin1_General_CI_AS=t.SECTION
and t.CAMPUS_CODE='" + CAMPUS_CODE + "' and h.branch_code='" + BRANCH_CODE + "' and h.batch like '" + BATCH + "%' and h.SEMESTER='" + SEMESTER + "' and h.SMS is null and h.class='" + set + "' ";


                return await Task.FromResult(GetAllData42<Timetable>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Students> getfeepaid(string regdno, string sem)
        {
            string query = "";
            try
            {

                query = "select isnull(sum(total_fee),'0') as vpf from fee_challan_master where REGDNO = '" + regdno + "' and fee_type = 'R' and payment_status = 'Y' and challan_date>= '01-jun-2023'";
                return await QueryFirstOrDefault<Students>(query, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<Students> getlatefee(string regdno, string sem)
        {
            string query = "";
            try
            {

                query = "select isnull(sum(CAST(fine as float )),0) as latefee from GPAY_FINE_POSTING_LIVE where regdno='" + regdno + "' and status = 'y'";
                return await QueryFirstOrDefault<Students>(query, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<Students> getotherfee(string regdno, string sem)
        {
            string query = "";
            try
            {

                query =
                    @"select isnull(tution_fee_arrears,0) as vafee,fee_demand2 as vsemfee1,regdno,SCHOLARSHIP as vscholorship,name,isnull(fee_demand1, '0') as fee_demand1,campus,college_code,course_code,branch_code,class,mobile,parent_mobile,batch,section,isnull(curr_sem,'0'),
degree_code,isnull(PART_PAYMENT_FLAG,'N'),GENDER,isnull(fine,'0'),hostler,PAYMENT_STATUS,STATUS,LATEFEE,SCHOLARSHIP,fee_demand5,FEE_SELECTION,challan_number,hostel_arrears,summerterm,isnull(EXCESS_FEE,'N')  from student_master where regdno = '" + regdno + "'";
                return await QueryFirstOrDefault<Students>(query, null);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Students>> getSEM13_SGPA(string regdno, string sem)
        {
            var query = "SELECT * FROM student_master  where REGDNO='" + regdno + "' and SEM13_SGPA =1  and STATUS='S'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }


        public async Task<IEnumerable<Students>> getaccessdata(string regdno)
        {
            var query = "SELECT * FROM TBL_STUDENT_COURSEREGISTRATION_ACCESS  where REGDNO='" + regdno + "'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }

        public async Task<IEnumerable<Students>> getTxn_flag(string regdno, string sem)
        {
            var query = "SELECT * FROM student_master  where REGDNO='" + regdno + "' and Txn_flag ='Y'  and STATUS='S'";
            return await Task.FromResult(GetAllData<Students>(query, null));
        }

        public async Task<IEnumerable<Timetable>> getupcomingtimetable(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH)
        {
            try
            {
                string query = "";
                if (CAMPUS_CODE.ToUpper() == "BLR")
                {
                    query = @"select WEEKDAY ,t.SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
   as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
   and h.COURSE_CODE = t.COURSE_CODE and t.SEMESTER='" + SEMESTER + "' and h.REGDNO = '" + regdno + "'  ";
                }
                else if (CAMPUS_CODE.ToUpper() == "VSP")
                {
                    query = @"select WEEKDAY ,t.SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
   as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
   and h.COURSE_CODE = t.COURSE_CODE and t.SEMESTER='" + SEMESTER + "' and h.REGDNO = '" + regdno + "'  ";

                }
                else
                {

                    query = @"select WEEKDAY ,t.SUBJECT_CODE,substring( convert(varchar, FROMTIME, 108),1,5)  +' to '+ substring( convert(varchar, TOTIME, 108),1,5) 
   as timeslots  from TIME_TABLE_MASTER t, CBCS_STUDENT_SUBJECT_ASSIGN h where                  
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE 
   and h.COURSE_CODE = t.COURSE_CODE and t.SEMESTER='" + SEMESTER + "' and h.REGDNO = '" + regdno + "'  ";
                }


                return await Task.FromResult(GetAllData42<Timetable>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Hallticket>> getstudenthallticket(Hallticket chart)
        {
            string query = "";
            try
            {
                if (chart.type1 == "R")
                {
                    if (chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                    {
                        if (chart.course_code == "PHD")
                        {
                            query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester=ss.curr_sem   and s.sub_type ='TP' and s.supp_exam_flag in('R','S')   order by CONVERT(DATETIME, s.exam_date, 103)  ");
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH  and s.sub_type ='TP' and s.supp_exam_flag='" + chart.type1 + "'  order by CONVERT(DATETIME, s.exam_date, 103)  ");//and s.semester=ss.curr_sem 
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                        }


                    }
                    else
                    {
                        query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' and s.semester=ss.curr_sem order by (select subject_order from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus   and s.sub_type ='TP' and batch = s.batch)  ");
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }


                }
                else if (chart.type1 == "SP")
                {
                    query = "select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus  = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "'  and s.supp_exam_flag='SP'  and degree_code=s.degree_code  and s.sub_type ='TP'  order by S.SEMESTER   ";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "B")
                {
                    query = "select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus  = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point ,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "'   and s.sub_type ='TP'  and s.supp_exam_flag='B' order by S.SEMESTER ";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "SD" || chart.type1 == "ST")
                {
                    if (chart.type1 == "ST")
                    {
                        query = "select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus  = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "'  and s.sub_type ='TP'  and s.supp_exam_flag='ST'  order by CONVERT(DATETIME, s.exam_date, 103) ";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                    else
                    {
                        query = "select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus  = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "'   and s.sub_type ='TP' and s.supp_exam_flag='SD' order by S.SEMESTER ";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                }
                else if (chart.type1 == "I")
                {
                    query = "select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select DISTINCT(subject_name) from subject_master where subject_code = s.sub_code and SEMESTER = s.SEMESTER and COURSE_CODE = s.course and branch_code = s.branch and degree_code = s.degree_code and campus = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s, student_master ss where s.regdno = ss.regdno and s.regdno = '" + chart.regdno + "'  and s.sub_type ='TP'  and s.supp_exam_flag = 'I' order by S.SEMESTER ";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
                else if (chart.type1 == "S" || chart.type1 == "BG" || chart.type1 == "CC")
                {
                    if (chart.type1.Trim() == "CC")
                    {
                        query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='TP' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by CONVERT(DATETIME, s.exam_date, 103)  ");
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                    else
                    {

                        if (chart.batch.Trim().Equals("2013-2014") || chart.batch.Trim().Equals("2014-2015") || chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                        {
                            query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='TP' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by CONVERT(DATETIME, s.exam_date, 103)  ");
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point ,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' and s.sub_type ='P' order by (select subject_order from subject_master where subject_code=s.sub_code   and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch  )  ");
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }

                    }

                }
                else
                {
                    query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' and s.semester=ss.curr_sem order by (select subject_order from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER  and s.sub_type ='TP' and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch)  ");
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> getstudenthallticketpractical(Hallticket chart)
        {
            string query = "";
            try
            {
                if (chart.type1 == "R")
                {
                    if (chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                    {
                        if (chart.course_code == "PHD")
                        {
                            query = @"select  s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester=ss.curr_sem   and s.sub_type ='P' and s.supp_exam_flag in('R','S')  ";
                            query = query + @"UNION SELECT  s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno, (SELECT DISTINCT subject_name FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course AND branch_code = s.branch AND degree_code = s.degree_code AND college_code = s.college_code AND campus = s.campus   AND batch = s.batch) AS subject_name,  CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -' END AS MONTH,  s.grade_point, s.exam_date,   s.exam_time FROM students_marks s JOIN student_master ss  ON s.regdno = ss.regdno  WHERE s.regdno = 'YourRegdNo'   AND s.BATCH = ss.BATCH   AND s.COLLEGE_CODE = ss.COLLEGE_CODE AND s.CAMPUS = ss.CAMPUS AND s.COURSE = ss.COURSE AND s.BRANCH = ss.BRANCH AND s.semester = ss.curr_sem AND s.sub_type = 'TP'  AND s.sub_code LIKE '%P'  AND s.supp_exam_flag IN ('R', 'S')) AS combined_result ORDER BY CONVERT(DATETIME, exam_date, 103);";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = @"SELECT subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time FROM (SELECT s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno,(SELECT TOP 1 subject_name FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course AND branch_code = s.branch   AND degree_code = s.degree_code AND college_code = s.college_code   AND campus = s.campus AND batch = s.batch) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -' WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time  FROM students_marks s, student_master ss  WHERE s.regdno = ss.regdno  AND s.regdno = '" + chart.regdno + "' AND S.BATCH = SS.BATCH  AND S.COLLEGE_CODE = SS.COLLEGE_CODE  AND S.CAMPUS = SS.CAMPUS  AND S.COURSE = SS.COURSE  AND S.BRANCH = SS.BRANCH  AND s.sub_type = 'P'  AND s.supp_exam_flag = '" + chart.type1 + "' ";
                            query = query + @" union   SELECT s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno,(SELECT TOP 1 subject_name FROM subject_master  WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course AND branch_code = s.branch  AND degree_code = s.degree_code AND college_code = s.college_code  AND campus = s.campus AND batch = s.batch) AS subject_name,   CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -'    WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -' END AS MONTH,   s.grade_point, s.exam_date, s.exam_time  FROM students_marks s, student_master ss WHERE s.regdno = ss.regdno  AND s.regdno =  '" + chart.regdno + "'  AND S.BATCH = SS.BATCH  AND S.COLLEGE_CODE = SS.COLLEGE_CODE  AND S.CAMPUS = SS.CAMPUS  AND S.COURSE = SS.COURSE  AND S.BRANCH = SS.BRANCH  AND s.sub_type = 'TP' AND s.sub_code LIKE '%P' AND s.supp_exam_flag = '" + chart.type1 + "') AS combined_result ORDER BY CONVERT(DATETIME, exam_date, 103);";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                        }


                    }
                    else
                    {


                        query = @"SELECT s.sub_type AS sub_type,s.regdno,  (SELECT DISTINCT subject_name FROM subject_master  WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch) AS subject_name,  CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -'  END AS MONTH,s.grade_point FROM students_marks sJOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "' AND s.semester = ss.curr_sem AND s.sub_type = 'P' AND s.batch = ss.batch ";
                        query = query + @" union SELECT s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno, (SELECT DISTINCT subject_name  FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course  AND branch_code = s.branch   AND degree_code = s.degree_code   AND campus = s.campus  AND batch = s.batch) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'    WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'    WHEN 'Jun' THEN 'June -'   WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'     WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'    WHEN 'Dec' THEN 'December -'   END AS MONTH,s.grade_point FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno ='" + chart.regdno + "' AND s.semester = ss.curr_sem AND s.sub_type = 'TP' AND s.sub_code LIKE '%P' AND s.batch = ss.batch ORDER BY subject_code, sub_type, regdno";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }


                }
                else if (chart.type1 == "SP")
                {
                    query = "SELECT   s.sub_type AS sub_type,  s.regdno, (SELECT subject_name   FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course    AND branch_code = s.branch   AND degree_code = s.degree_code   AND campus = s.campus  AND batch = s.batch   AND college_code = s.college_code) AS subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'    WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'    WHEN 'Nov' THEN 'November -'    WHEN 'Dec' THEN 'December -'   END AS MONTH,  s.grade_point,  S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.supp_exam_flag = 'SP'  AND degree_code = s.degree_code  AND s.sub_type = 'P' ";
                    query = query + @" union SELECT  s.sub_code AS subject_code,  s.sub_type AS sub_type,  s.regdno, (SELECT subject_name   FROM subject_master   WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course  AND branch_code = s.branch   AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code) AS subject_name, CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'    WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point,   S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno ='" + chart.regdno + "'  AND s.supp_exam_flag = 'SP'    AND degree_code = s.degree_code  AND s.sub_type = 'TP'   AND s.sub_code LIKE '%P'ORDER BY SEMESTER";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "B")
                {


                    query = "SELECT   s.sub_type AS sub_type, s.regdno, (SELECT subject_name  FROM subject_master   WHERE subject_code = s.sub_code  AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course  AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time, S.SEMESTER FROM students_marks s JOIN student_master ss  ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'P'  AND s.supp_exam_flag = 'B' ";
                    query = query + @" union SELECT   s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, (SELECT subject_name  FROM subject_master   WHERE subject_code = s.sub_code  AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course   AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus    AND batch = s.batch  AND college_code = s.college_code) AS subject_name,  CASE s.result    WHEN 'Jan' THEN 'January -'   WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'    WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -'    WHEN 'Jul' THEN 'July -'    WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'    WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time, S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'   AND s.sub_type = 'TP'   AND s.sub_code LIKE '%P'  AND s.supp_exam_flag = 'B' ORDER BY SEMESTER";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "SD" || chart.type1 == "ST")
                {
                    if (chart.type1 == "ST")
                    {
                        query = "SELECT s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, ( SELECT DISTINCT subject_name  FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course AND branch_code = s.branch AND degree_code = s.degree_code AND campus = s.campus  AND batch = s.batch AND college_code = s.college_code) AS subject_name,  MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'   WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -' END,  s.grade_point,  s.exam_date,  s.exam_time  FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno  WHERE  s.regdno = '" + chart.regdno + "' AND s.sub_type = 'P' AND s.supp_exam_flag = 'ST' ";
                        query = query + @" union SELECT s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno,  ( SELECT DISTINCT subject_name  FROM subject_master WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course    AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code ) AS subject_name,  MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -' END,    s.grade_point, s.exam_date,  s.exam_time FROM students_marks s   JOIN student_master ss ON s.regdno = ss.regdno WHERE   s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'TP'  AND s.sub_code LIKE '%P' AND s.supp_exam_flag = 'ST' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                    }
                    else
                    {
                        query = "SELECT s.sub_type AS sub_type,  s.regdno,( SELECT subject_name  FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course  AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus AND batch = s.batch  AND college_code = s.college_code ) AS subject_name, MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -'  END,   s.grade_point, s.exam_date, s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'P' AND s.supp_exam_flag = 'SD' ";
                        query = query + @" union SELECT  s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, ( SELECT subject_name  FROM subject_master WHERE  subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course  AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch AND college_code = s.college_code ) AS subject_name,   MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -'  END,  s.grade_point, s.exam_date, s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE   s.regdno = '" + chart.regdno + "' AND s.sub_type = 'TP'   AND s.sub_code LIKE '%P' AND s.supp_exam_flag = 'SD' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                }
                else if (chart.type1 == "I")
                {

                    query = "select s.sub_type as sub_type, s.regdno,(select DISTINCT(subject_name) from subject_master where subject_code = s.sub_code and SEMESTER = s.SEMESTER and COURSE_CODE = s.course and branch_code = s.branch and degree_code = s.degree_code and campus = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s, student_master ss where s.regdno = ss.regdno and s.regdno = '" + chart.regdno + "'  and s.sub_type ='P'  and s.supp_exam_flag = 'I' ";
                    query = query + @" union select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select DISTINCT(subject_name) from subject_master where subject_code = s.sub_code and SEMESTER = s.SEMESTER and COURSE_CODE = s.course and branch_code = s.branch and degree_code = s.degree_code and campus = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s, student_master ss where s.regdno = ss.regdno and s.regdno = '" + chart.regdno + "'  and s.sub_type ='TP'  and s.sub_code like'%P' and s.supp_exam_flag = 'I' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point;";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
                else if (chart.type1 == "S" || chart.type1 == "BG" || chart.type1 == "CC")
                {
                    if (chart.type1.Trim() == "CC")
                    {
                        query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='P' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'");
                        query = query + @" union select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='TP' and s.sub_code like'%P'  AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by  s.sub_code ";

                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                    else
                    {

                        if (chart.batch.Trim().Equals("2013-2014") || chart.batch.Trim().Equals("2014-2015") || chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                        {
                            query = ("select  s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='P' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'");
                            query = query + @" union select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='TP' and s.sub_code like'%P'   AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by  s.sub_code ";

                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = ("SELECT  s.sub_type AS sub_type,  s.regdno, sm.subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'    WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -' WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -' WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point, s.exam_date,  s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno JOIN subject_master sm ON sm.subject_code = s.sub_code  AND sm.SEMESTER = s.SEMESTER AND sm.COURSE_CODE = s.course AND sm.branch_code = s.branch AND sm.degree_code = s.degree_code AND sm.campus = s.campus  AND sm.batch = s.batch WHERE s.regdno = '" + chart.regdno + "' AND s.semester = '" + chart.id1 + "'  AND s.sub_type = 'P' AND s.supp_exam_flag = '" + chart.type1 + "'");
                            query = query + @" union SELECT DISTINCT  s.sub_code AS subject_code,  s.sub_type AS sub_type,  s.regdno, sm.subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'    WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -' WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -' WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point, s.exam_date,  s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno JOIN subject_master sm ON sm.subject_code = s.sub_code  AND sm.SEMESTER = s.SEMESTER AND sm.COURSE_CODE = s.course AND sm.branch_code = s.branch AND sm.degree_code = s.degree_code AND sm.campus = s.campus  AND sm.batch = s.batch WHERE s.regdno = '" + chart.regdno + "' AND s.semester = '" + chart.id1 + "'  AND s.sub_type = 'TP'  and s.sub_code like'%P' AND s.supp_exam_flag = '" + chart.type1 + "'   order by  s.sub_code ";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }

                    }

                }
                else
                {
                    query = ("select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result  when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -'  when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno= '" + chart.regdno + "' and s.semester=ss.curr_sem  and s.sub_type='P'  union select s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result  when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno= '" + chart.regdno + "' and s.semester=ss.curr_sem  and s.sub_type='TP' and s.sub_code like'%P' ");
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<IEnumerable<Hallticket>> getstudenthalltickettheory(Hallticket chart)
        {
            string query = "";
            try
            {
                if (chart.type1 == "R")
                {
                    if (chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                    {
                        if (chart.course_code == "PHD")
                        {
                            query = @"select ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester=ss.curr_sem   and s.sub_type ='T' and s.supp_exam_flag in('R','S')  ";
                            query = query + @"UNION SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row, s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno, (SELECT DISTINCT subject_name FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course AND branch_code = s.branch AND degree_code = s.degree_code AND college_code = s.college_code AND campus = s.campus   AND batch = s.batch) AS subject_name,  CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -' END AS MONTH,  s.grade_point, s.exam_date,   s.exam_time FROM students_marks s JOIN student_master ss  ON s.regdno = ss.regdno  WHERE s.regdno = 'YourRegdNo'   AND s.BATCH = ss.BATCH   AND s.COLLEGE_CODE = ss.COLLEGE_CODE AND s.CAMPUS = ss.CAMPUS AND s.COURSE = ss.COURSE AND s.BRANCH = ss.BRANCH AND s.semester = ss.curr_sem AND s.sub_type = 'TP'  AND s.sub_code not LIKE '%P'  AND s.supp_exam_flag IN ('R', 'S')) AS combined_result ORDER BY CONVERT(DATETIME, exam_date, 103);";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = @"SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time FROM (SELECT s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno,(SELECT TOP 1 subject_name FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course AND branch_code = s.branch   AND degree_code = s.degree_code AND college_code = s.college_code   AND campus = s.campus AND batch = s.batch) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -' WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time  FROM students_marks s, student_master ss  WHERE s.regdno = ss.regdno  AND s.regdno = '" + chart.regdno + "' AND S.BATCH = SS.BATCH  AND S.COLLEGE_CODE = SS.COLLEGE_CODE  AND S.CAMPUS = SS.CAMPUS  AND S.COURSE = SS.COURSE  AND S.BRANCH = SS.BRANCH  AND s.sub_type = 'T'  AND s.supp_exam_flag = '" + chart.type1 + "' ";
                            query = query + @" union   SELECT  s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno,(SELECT TOP 1 subject_name FROM subject_master  WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course AND branch_code = s.branch  AND degree_code = s.degree_code AND college_code = s.college_code  AND campus = s.campus AND batch = s.batch) AS subject_name,   CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -'    WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -' END AS MONTH,   s.grade_point, s.exam_date, s.exam_time  FROM students_marks s, student_master ss WHERE s.regdno = ss.regdno  AND s.regdno =  '" + chart.regdno + "'  AND S.BATCH = SS.BATCH  AND S.COLLEGE_CODE = SS.COLLEGE_CODE  AND S.CAMPUS = SS.CAMPUS  AND S.COURSE = SS.COURSE  AND S.BRANCH = SS.BRANCH  AND s.sub_type = 'TP' AND s.sub_code  not LIKE '%P' AND s.supp_exam_flag = '" + chart.type1 + "') AS combined_result ORDER BY CONVERT(DATETIME, exam_date, 103);";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                        }


                    }
                    else
                    {


                        query = @"SELECT ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row, s.sub_code AS subject_code,s.sub_type AS sub_type,s.regdno,  (SELECT DISTINCT subject_name FROM subject_master  WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch) AS subject_name,  CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'  WHEN 'Dec' THEN 'December -'  END AS MONTH,s.grade_point FROM students_marks sJOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "' AND s.semester = ss.curr_sem AND s.sub_type = 'T' AND s.batch = ss.batch ";
                        query = query + @" union SELECT ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row,s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno, (SELECT DISTINCT subject_name  FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course  AND branch_code = s.branch   AND degree_code = s.degree_code   AND campus = s.campus  AND batch = s.batch) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'    WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'    WHEN 'Jun' THEN 'June -'   WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'     WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'    WHEN 'Dec' THEN 'December -'   END AS MONTH,s.grade_point FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno ='" + chart.regdno + "' AND s.semester = ss.curr_sem AND s.sub_type = 'TP' AND s.sub_code not LIKE '%P' AND s.batch = ss.batch ORDER BY subject_code, sub_type, regdno";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }


                }
                else if (chart.type1 == "SP")
                {
                    query = "SELECT  ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row, s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno, (SELECT subject_name   FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course    AND branch_code = s.branch   AND degree_code = s.degree_code   AND campus = s.campus  AND batch = s.batch   AND college_code = s.college_code) AS subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'   WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'    WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'    WHEN 'Nov' THEN 'November -'    WHEN 'Dec' THEN 'December -'   END AS MONTH,  s.grade_point,  S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.supp_exam_flag = 'SP'  AND degree_code = s.degree_code  AND s.sub_type = 'T' ";
                    query = query + @" union SELECT   ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row,s.sub_code AS subject_code,  s.sub_type AS sub_type,  s.regdno, (SELECT subject_name   FROM subject_master   WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course  AND branch_code = s.branch   AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code) AS subject_name, CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'   WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -'  WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'    WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point,   S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno ='" + chart.regdno + "'  AND s.supp_exam_flag = 'SP'    AND degree_code = s.degree_code  AND s.sub_type = 'TP'   AND s.sub_code not LIKE '%P'ORDER BY SEMESTER";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "B")
                {


                    query = "SELECT  ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row, s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, (SELECT subject_name  FROM subject_master   WHERE subject_code = s.sub_code  AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course  AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code) AS subject_name,  CASE s.result   WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'   WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time, S.SEMESTER FROM students_marks s JOIN student_master ss  ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'T'  AND s.supp_exam_flag = 'B' ";
                    query = query + @" union SELECT   ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row, s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, (SELECT subject_name  FROM subject_master   WHERE subject_code = s.sub_code  AND SEMESTER = s.SEMESTER   AND COURSE_CODE = s.course   AND branch_code = s.branch  AND degree_code = s.degree_code  AND campus = s.campus    AND batch = s.batch  AND college_code = s.college_code) AS subject_name,  CASE s.result    WHEN 'Jan' THEN 'January -'   WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'    WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -'   WHEN 'Jun' THEN 'June -'    WHEN 'Jul' THEN 'July -'    WHEN 'Aug' THEN 'August -'    WHEN 'Sep' THEN 'September -'    WHEN 'Oct' THEN 'October -'   WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'   END AS MONTH, s.grade_point, s.exam_date, s.exam_time, S.SEMESTER FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'   AND s.sub_type = 'TP'   AND s.sub_code not LIKE '%P'  AND s.supp_exam_flag = 'B' ORDER BY SEMESTER";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                }
                else if (chart.type1 == "SD" || chart.type1 == "ST")
                {
                    if (chart.type1 == "ST")
                    {
                        query = "SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, ( SELECT DISTINCT subject_name  FROM subject_master WHERE subject_code = s.sub_code AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course AND branch_code = s.branch AND degree_code = s.degree_code AND campus = s.campus  AND batch = s.batch AND college_code = s.college_code) AS subject_name,  MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'   WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'  WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -' END,  s.grade_point,  s.exam_date,  s.exam_time  FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno  WHERE  s.regdno = '" + chart.regdno + "' AND s.sub_type = 'T' AND s.supp_exam_flag = 'ST' ";
                        query = query + @" union SELECT ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno,  ( SELECT DISTINCT subject_name  FROM subject_master WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course    AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch  AND college_code = s.college_code ) AS subject_name,  MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -'   WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -' END,    s.grade_point, s.exam_date,  s.exam_time FROM students_marks s   JOIN student_master ss ON s.regdno = ss.regdno WHERE   s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'TP'  AND s.sub_code not LIKE '%P' AND s.supp_exam_flag = 'ST' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
                    }
                    else
                    {
                        query = "SELECT ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row,s.sub_code AS subject_code, s.sub_type AS sub_type,  s.regdno,( SELECT subject_name  FROM subject_master  WHERE subject_code = s.sub_code   AND SEMESTER = s.SEMESTER AND COURSE_CODE = s.course  AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus AND batch = s.batch  AND college_code = s.college_code ) AS subject_name, MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -'  END,   s.grade_point, s.exam_date, s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE s.regdno = '" + chart.regdno + "'  AND s.sub_type = 'T' AND s.supp_exam_flag = 'SD' ";
                        query = query + @" union SELECT ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row, s.sub_code AS subject_code, s.sub_type AS sub_type, s.regdno, ( SELECT subject_name  FROM subject_master WHERE  subject_code = s.sub_code AND SEMESTER = s.SEMESTER  AND COURSE_CODE = s.course  AND branch_code = s.branch AND degree_code = s.degree_code  AND campus = s.campus  AND batch = s.batch AND college_code = s.college_code ) AS subject_name,   MONTH = CASE s.result  WHEN 'Jan' THEN 'January -'  WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -'  WHEN 'Apr' THEN 'April -'  WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -'  WHEN 'Jul' THEN 'July -'  WHEN 'Aug' THEN 'August -'  WHEN 'Sep' THEN 'September -'   WHEN 'Oct' THEN 'October -'  WHEN 'Nov' THEN 'November -' WHEN 'Dec' THEN 'December -'  END,  s.grade_point, s.exam_date, s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno WHERE   s.regdno = '" + chart.regdno + "' AND s.sub_type = 'TP'   AND s.sub_code not LIKE '%P' AND s.supp_exam_flag = 'SD' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point, exam_date, exam_time";
                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                }
                else if (chart.type1 == "I")
                {

                    query = "select ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select DISTINCT(subject_name) from subject_master where subject_code = s.sub_code and SEMESTER = s.SEMESTER and COURSE_CODE = s.course and branch_code = s.branch and degree_code = s.degree_code and campus = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s, student_master ss where s.regdno = ss.regdno and s.regdno = '" + chart.regdno + "'  and s.sub_type ='T'  and s.supp_exam_flag = 'I' ";
                    query = query + @" union select ROW_NUMBER() OVER(ORDER BY S.SEMESTER asc) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select DISTINCT(subject_name) from subject_master where subject_code = s.sub_code and SEMESTER = s.SEMESTER and COURSE_CODE = s.course and branch_code = s.branch and degree_code = s.degree_code and campus = s.campus and batch = s.batch and college_code = s.college_code) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s, student_master ss where s.regdno = ss.regdno and s.regdno = '" + chart.regdno + "'  and s.sub_type ='TP'  and s.sub_code not like'%P' and s.supp_exam_flag = 'I' ORDER BY subject_code, sub_type, regdno, subject_name, MONTH, grade_point;";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
                else if (chart.type1 == "S" || chart.type1 == "BG" || chart.type1 == "CC")
                {
                    if (chart.type1.Trim() == "CC")
                    {
                        query = ("select ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='T' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'");
                        query = query + @" union select ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type  ='TP' and s.sub_code not like'%P'  AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by  s.sub_code ";

                        return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                    }
                    else
                    {

                        if (chart.batch.Trim().Equals("2013-2014") || chart.batch.Trim().Equals("2014-2015") || chart.batch.Trim().Equals("2015-2016") || chart.batch.Trim().Equals("2016-2017") || chart.batch.Trim().Equals("2017-2018") || chart.batch.Trim().Equals("2018-2019") || Convert.ToInt32(chart.batch.Trim().Split('-')[0]) >= 2019)
                        {
                            query = ("select ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='T' AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'");
                            query = query + @" union select ROW_NUMBER() OVER(ORDER BY CONVERT(DATETIME, exam_date, 103)) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + chart.regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE  and s.sub_type ='TP' and s.sub_code not like'%P'   AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH and s.semester='" + chart.id1 + "' and s.supp_exam_flag='" + chart.type1 + "' AND s.Degree_Code='" + chart.degree_code + "'   order by  s.sub_code ";

                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }
                        else
                        {
                            query = ("SELECT ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row,  s.sub_code AS subject_code,  s.sub_type AS sub_type,  s.regdno, sm.subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'    WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -' WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -' WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point, s.exam_date,  s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno JOIN subject_master sm ON sm.subject_code = s.sub_code  AND sm.SEMESTER = s.SEMESTER AND sm.COURSE_CODE = s.course AND sm.branch_code = s.branch AND sm.degree_code = s.degree_code AND sm.campus = s.campus  AND sm.batch = s.batch WHERE s.regdno = '" + chart.regdno + "' AND s.semester = '" + chart.id1 + "'  AND s.sub_type = 'T' AND s.supp_exam_flag = '" + chart.type1 + "'");
                            query = query + @" union SELECT ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row,  s.sub_code AS subject_code,  s.sub_type AS sub_type,  s.regdno, sm.subject_name,   CASE s.result   WHEN 'Jan' THEN 'January -'    WHEN 'Feb' THEN 'February -'  WHEN 'Mar' THEN 'March -' WHEN 'Apr' THEN 'April -'   WHEN 'May' THEN 'May -' WHEN 'Jun' THEN 'June -' WHEN 'Jul' THEN 'July -' WHEN 'Aug' THEN 'August -' WHEN 'Sep' THEN 'September -' WHEN 'Oct' THEN 'October -' WHEN 'Nov' THEN 'November -'   WHEN 'Dec' THEN 'December -'  END AS MONTH,  s.grade_point, s.exam_date,  s.exam_time FROM students_marks s JOIN student_master ss ON s.regdno = ss.regdno JOIN subject_master sm ON sm.subject_code = s.sub_code  AND sm.SEMESTER = s.SEMESTER AND sm.COURSE_CODE = s.course AND sm.branch_code = s.branch AND sm.degree_code = s.degree_code AND sm.campus = s.campus  AND sm.batch = s.batch WHERE s.regdno = '" + chart.regdno + "' AND s.semester = '" + chart.id1 + "'  AND s.sub_type = 'TP'  and s.sub_code not like'%P' AND s.supp_exam_flag = '" + chart.type1 + "'   order by  s.sub_code ";
                            return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                        }

                    }

                }
                else
                {
                    query = ("select ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result  when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -'  when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno= '" + chart.regdno + "' and s.semester=ss.curr_sem  and s.sub_type='T'  union select ROW_NUMBER() OVER(ORDER BY subject_code asc) AS row,s.sub_code as subject_code,s.sub_type as sub_type, s.regdno,(select distinct(subject_name) from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result  when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno= '" + chart.regdno + "' and s.semester=ss.curr_sem  and s.sub_type='TP' and s.sub_code not like'%P' ");
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> getstudentspecialhallticket(Hallticket chart)
        {
            string query = "";
            int k = 0;
            DateTime nextdate1;
            string nextdate;
            nextdate1 = System.DateTime.Now.Date;
            nextdate = Convert.ToDateTime(nextdate1).ToString("MMM dd yyyy hh:mmtt");
            try
            {
                if (chart.type1 == "SD")
                {

                    query = @"select *  from  STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and Exam_Type='" + chart.type1 + "' and status='Y'";

                }
                else if (chart.type1 == "B")
                {
                    query = @"select *  from  STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and Exam_Type='" + chart.type1 + "' and status='Y'";

                }
                var sdt = await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                if (sdt.Count > 0)
                {
                    chart.status_flag = "Accept";
                    query = @"insert into HALL_TICKET_DOWNLOAD_STATUS values('" + chart.regdno + "'," + chart.CURRSEM + " , '" + chart.type1 + "', '" + chart.status_flag + "','" + nextdate1 + "')";
                    k = await Task.FromResult(InsertData52hall(query, null));

                }
                return sdt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public async Task<IEnumerable<Hallticket>> gethalleligiblesubjects(string regdno, string type1, string semester)
        {
            string query = "";
            try
            {
                query = @"select subject_code ,subject_name,status, *  from STUDENT_HALL_TICKETS_DATA where regdno='" + regdno + "' and ATTENDANCE_STATUS ='Y'  and EXAM_TYPE ='" + type1 + "' and SEMESTER='" + semester + "' ";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> gethallnoteligiblesubjects(string regdno, string type1, string semester)
        {
            string query = "";
            try
            {
                query = @"select subject_code ,subject_name,status, *  from STUDENT_HALL_TICKETS_DATA where regdno='" + regdno + "' and ATTENDANCE_STATUS ='N'  and EXAM_TYPE ='" + type1 + "' and SEMESTER='" + semester + "'  ";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }





        //psnthallticket

        public async Task<IEnumerable<Hallticket>> gethallticketactive(string regno)
        {
            var query2 = "";
            try
            {
                query2 = "select * from student_hall_tickets_data  where regdno='" + regno + "' and status='Y'";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query2, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<IEnumerable<Hallticket>> gethallticketPHDdata52(string regdno)
        {
            try
            {
                var query = "select EXAM_TYPE as type1,SEMESTER as CURRSEM,isnull(attendance_status, '') as attendance_status,isnull(status, '') as status,exam_type,sub_type,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type from STUDENT_HALL_TICKETS_DATA where regdno = '" + regdno + "' and YEAR = '2024' ";

                //var query = "select subject_code,subject_name, MONTH+'-' as MONTH ,year as grade_point,s.exam_date,s.exam_time,s.course_name,s.branch_name as branch_name,s.semester as currsem,s.exam_type as type1,s.degree_code,s.college_code,s.course_code,s.branch_code,s.campus_code,regdno, name from HALL_TICKETS_DATA s  where s.regdno = '" + regdno + "'  and status = 'Y' order by exam_date  ";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<IEnumerable<Hallticket>> getstudentcoursedata(string regdno)
        //{
        //    try
        //    {
        //        var query = "select top 1 isnull(c.COURSE_NAME, '')  as COURSE_NAME,isnull(b.branch_name, '') as branch_name,case when s.CAMPUS='vsp' then 'Visakhapatnam' else case when s.CAMPUS='hyd' then 'Hyderabad'  else 'Bangalore'  end end as Exam_Venue , s.NAME from student_master s, branch_master b, course_master c where b.BRANCH_CODE=s.BRANCH_CODE and c.COURSE_CODE=s.COURSE_CODE and c.COLLEGE_CODE=s.COLLEGE_CODE and b.COLLEGE_CODE=s.COLLEGE_CODE and c.CAMPUS_CODE=s.CAMPUS and b.CAMPUS_CODE=s.campus  and s.regdno='" + regdno + "'";

        //        //var query = "select subject_code,subject_name, MONTH+'-' as MONTH ,year as grade_point,s.exam_date,s.exam_time,s.course_name,s.branch_name as branch_name,s.semester as currsem,s.exam_type as type1,s.degree_code,s.college_code,s.course_code,s.branch_code,s.campus_code,regdno, name from HALL_TICKETS_DATA s  where s.regdno = '" + regdno + "'  and status = 'Y' order by exam_date  ";
        //        return await Task.FromResult(GetAllData<Hallticket>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        public async Task<IEnumerable<Hallticket>> getstudentcoursedata(string regdno)
        {
            try
            {
                //var query = "select top 1 isnull(c.COURSE_NAME, '')  as COURSE_NAME,isnull(b.branch_name, '') as branch_name,case when s.CAMPUS='vsp' then 'Visakhapatnam' else case when s.CAMPUS='hyd' then 'Hyderabad'  else 'Bangalore'  end end as Exam_Venue , s.NAME from student_master s, branch_master b, course_master c where b.BRANCH_CODE=s.BRANCH_CODE and c.COURSE_CODE=s.COURSE_CODE and c.COLLEGE_CODE=s.COLLEGE_CODE and b.COLLEGE_CODE=s.COLLEGE_CODE and c.CAMPUS_CODE=s.CAMPUS and b.CAMPUS_CODE=s.campus  and s.regdno='" + regdno + "'";

                //var query = "select subject_code,subject_name, MONTH+'-' as MONTH ,year as grade_point,s.exam_date,s.exam_time,s.course_name,s.branch_name as branch_name,s.semester as currsem,s.exam_type as type1,s.degree_code,s.college_code,s.course_code,s.branch_code,s.campus_code,regdno, name from HALL_TICKETS_DATA s  where s.regdno = '" + regdno + "'  and status = 'Y' order by exam_date  ";
                var query = @"select top 1  isnull(s.COURSE_NAME, '')  as COURSE_NAME,isnull(s.branch_name, '') as branch_name,case when st.CAMPUS='VSP' then 'Visakhapatnam' else case when st.CAMPUS='HYD' then 'Hyderabad'  else 'Bengaluru' end end as Exam_Venue , s.NAME from STUDENT_HALL_TICKETS_DATA s left join LinkedServer_192_168_23_19.GITAM.dbo.student_master st on s.regdno=st.regdno where s.regdno='" + regdno + "'";
                //  return await Task.FromResult(GetAllData<Hallticket>(query, null));
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> gethallticketdata52(string regdno)
        {
            try
            {
                var query = "select isnull(EXAM_TYPE,'') as type1,isnull(SEMESTER,'') as CURRSEM,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type from STUDENT_HALL_TICKETS_DATA where regdno = '" + regdno + "' and  status not in ('D','N') ";
                //var query = "select s.sub_code as subject_code,s.regdno as regdno,s.course as course_code,s.branch as branch_name,s.sub_code as subject_code,s.sub_type,s.supp_exam_flag as type1,s.semester as currsem,s.degree_code,s.campus as campus_code,s.college_code,(select top 1 subject_name from subject_master where subject_code=s.sub_code and SEMESTER=s.SEMESTER and COURSE_CODE=s.course and branch_code=s.branch and degree_code=s.degree_code and college_code = s.college_code and campus = s.campus and batch = s.batch) as subject_name, MONTH = case s.result when 'Jan' then 'January -' when 'Feb' then 'February -' when 'Mar' then 'March -' when 'Apr' then 'April -' when 'May' then 'May -' when 'Jun' then 'June -' when 'Jul' then 'July -' when 'Aug' then 'August -' when 'Sep' then 'September -' when 'Oct' then 'October -'  when 'Nov' then 'November -' when 'Dec' then 'December -' end,s.grade_point,s.exam_date,s.exam_time from students_marks s,student_master ss where s.regdno=ss.regdno and s.regdno='" + regdno + "' AND S.BATCH= SS.BATCH AND S.COLLEGE_CODE = SS.COLLEGE_CODE AND S.CAMPUS = SS.CAMPUS AND  S.COURSE= SS.COURSE AND S.BRANCH= SS.BRANCH  order by CONVERT(DATETIME, s.exam_date, 103)  ";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> get_student_data52(string regdno)
        {
            try
            {
                var query = "select NAME,regdno,GENDER,FATHER_NAME,CATEGORY,DOORNO,campus,status,batch,college_code  from  student_master where regdno='" + regdno + "'";
                return await Task.FromResult(GetAllData<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> getstudentdata(string regdno)
        {
            try
            {
                var query = "select NAME,regdno,GENDER,FATHER_NAME,CATEGORY,DOORNO,campus,college_code,degree_code,course_code,status,batch,branch_code  from  student_master where regdno='" + regdno + "'";
                return await Task.FromResult(GetAllData<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Hallticket>> Get_Student_Data(string REGDNO)
        {
            try
            {
                string qq1 = "select regdno,batch,CURR_SEM as CURRSEM,case when(year(getdate()) - cast(substring(batch, 0, 5) as int)) >= (cast(substring(batch, 6, len(batch)) as int) - cast(substring(batch, 0, 5) as int)) then 'Y' else 'N'  end Passedout from student_master where regdno = '" + REGDNO + "'";
                return await Task.FromResult(GetAllData<Hallticket>(qq1, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> getstudentspecialhalltickettheory52(Hallticket chart)
        {
            string query = "";
            int k = 0;
            DateTime nextdate1;
            string nextdate;
            nextdate1 = System.DateTime.Now.Date;
            nextdate = Convert.ToDateTime(nextdate1).ToString("MMM dd yyyy hh:mmtt");
            try
            {
                query = @"select isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,EXAM_VENUE,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type from  STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and Exam_Type='" + chart.type1 + "'  and sub_type ='T' and ATTENDANCE_STATUS='Y' union " +
                    "" +
                    "select  COURSE_NAME as course_name_last,BRANCH_NAME as branch_name_last,EXAM_VENUE,REGDNO,NAME,isnull(SUBJECT_CODE, '') as SUBJECT_CODE,isnull(SUBJECT_NAME, '') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME, '') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME, '') as BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE, '') as EXAM_TYPE ,isnull(MONTH, '') as MONTH ,isnull(YEAR, '') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE, '') as EXAM_VENUE,isnull(STATUS, '') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS, '') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "'  and sub_type ='TP' and subject_code not like'%P'  and EXAM_TYPE ='" + chart.type1 + "' and ATTENDANCE_STATUS='Y' order by EXAM_DATE asc";
                var sdt = await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                if (sdt.Count > 0)
                {
                    chart.status_flag = "Accept";
                    query = @"insert into HALL_TICKET_DOWNLOAD_STATUS values('" + chart.regdno + "'," + chart.CURRSEM + " , '" + chart.type1 + "', '" + chart.status_flag + "','" + nextdate1 + "')";
                    k = await Task.FromResult(InsertData52hall(query, null));

                }
                return sdt;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> getstudentspecialhallticketpractical52(Hallticket chart)
        {
            string query = "";
            int k = 0;
            DateTime nextdate1;
            string nextdate;
            nextdate1 = System.DateTime.Now.Date;
            nextdate = Convert.ToDateTime(nextdate1).ToString("MMM dd yyyy hh:mmtt");
            try
            {
                query = @"select isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,EXAM_VENUE,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from  STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and Exam_Type='" + chart.type1 + "' and sub_type ='P' and ATTENDANCE_STATUS='Y' union select isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,EXAM_VENUE,REGDNO,NAME,isnull(SUBJECT_CODE, '') as SUBJECT_CODE,isnull(SUBJECT_NAME, '') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME, '') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME, '') as BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE, '') as EXAM_TYPE ,isnull(MONTH, '') as MONTH ,isnull(YEAR, '') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE, '') as EXAM_VENUE,isnull(STATUS, '') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS, '') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "'  and sub_type ='TP' and subject_code like'%P'  and EXAM_TYPE ='" + chart.type1 + "' and ATTENDANCE_STATUS='Y' order by EXAM_DATE asc";
                var sdt = await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                if (sdt.Count > 0)
                {
                    chart.status_flag = "Accept";
                    query = @"insert into HALL_TICKET_DOWNLOAD_STATUS values('" + chart.regdno + "'," + chart.CURRSEM + " , '" + chart.type1 + "', '" + chart.status_flag + "','" + nextdate1 + "')";
                    k = await Task.FromResult(InsertData52hall(query, null));

                }
                return sdt;

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> getstudenthalltickettheory52(Hallticket chart)
        {
            string query = "";
            try
            {
                if (chart.type1 == "S")
                {
                    query = @"select  isnull(TIME_DUARATION,'') as timings,isnull(EXAM_PAPER,'') as papers,isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and STATUS='Y' and status!='D' and  sub_type ='T'  " +
                        " union select  isnull(TIME_DUARATION,'') as timings,isnull(EXAM_PAPER,'') as papers,isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and STATUS='Y' and status!='D' and sub_type ='TP' and subject_code not like'%P' order by final_exam_date asc";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }
                else
                {
                    query = @"select  isnull(TIME_DUARATION,'') as timings,isnull(EXAM_PAPER,'') as papers,isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and sub_type ='T'  and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and ATTENDANCE_STATUS='Y' and status!='D' " +
                  "union select  isnull(TIME_DUARATION,'') as timings,isnull(EXAM_PAPER,'') as papers,isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "'  and sub_type ='TP' and subject_code not like'%P'  and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and ATTENDANCE_STATUS='Y' and status!='D' order by final_exam_date asc";
                    return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<IEnumerable<Hallticket>> getstudenthallticketpractical52(Hallticket chart)
        {
            string query = "";
            try
            {
                query = @"select  isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "' and sub_type ='P'  and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and ATTENDANCE_STATUS='Y' and status!='D' " +
                    "union select isnull(COURSE_NAME,'') as course_name_last,isnull(BRANCH_NAME,'') as branch_name_last,REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type  from STUDENT_HALL_TICKETS_DATA where regdno='" + chart.regdno + "'  and sub_type ='TP' and subject_code like'%P'  and EXAM_TYPE ='" + chart.type1 + "' and semester ='" + chart.id1 + "' and ATTENDANCE_STATUS='Y' and status!='D' order by EXAM_DATE asc";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<List<Hallticket>> Get_Hall_Ticket_Details(string REGDNO, string type1, string currsem)
        {
            try
            {
                string sqry = @"SELECT ID, REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type
                  FROM  STUDENT_HALL_TICKETS_DATA where regdno='" + REGDNO + "' and Exam_Type='" + type1 + "'order by exam_date";
                return await Task.FromResult(GetAllData52hall<Hallticket>(sqry, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> getstudentPHDhalltickettheory(Hallticket chart)
        {
            string query = "";
            try
            {
                query = @" select REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,s.EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type, MONTH+'-' as MONTH ,year as grade_point,s.exam_date,s.exam_time,course_name,branch_name,regdno,name,COURSE_NAME as course_name_last,BRANCH_NAME as branch_name_last,EXAM_VENUE,SEMESTER as psntsem,year as year  from STUDENT_HALL_TICKETS_DATA s  where  s.regdno='" + chart.regdno + "' and exam_type='" + chart.type1 + "' and ATTENDANCE_STATUS='Y' and sub_type='T'  and semester ='" + chart.id1 + "' order by s.exam_date";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Hallticket>> getstudentPHDhallticketpractical(Hallticket chart)
        {
            string query = "";
            try
            {
                query = @" select REGDNO,NAME,isnull(SUBJECT_CODE,'') as SUBJECT_CODE,isnull(SUBJECT_NAME,'') as SUBJECT_NAME,CAMPUS_CODE,COLLEGE_CODE,COURSE_CODE ,isnull(COURSE_NAME,'') as COURSE_NAME ,BRANCH_CODE , isnull(BRANCH_NAME,'') as  BRANCH_NAME,DEGREE_CODE ,SEMESTER ,isnull(EXAM_TYPE,'') as EXAM_TYPE ,isnull(MONTH,'') as MONTH ,isnull(YEAR,'') as YEAR ,s.EXAM_DATE ,EXAM_TIME ,isnull(EXAM_VENUE,'') as EXAM_VENUE,isnull(STATUS,'') as STATUS ,TRANSFERRED_BY ,BATCH,PAYMENT_STATUS,isnull(ATTENDANCE_STATUS,'') as ATTENDANCE_STATUS,BLOCKED_BY,REMARKS,FINAL_EXAM_DATE,TRANS_DATE,sub_type, MONTH+'-' as MONTH ,year as grade_point,s.exam_date,s.exam_time,course_name,branch_name,regdno,name,COURSE_NAME as course_name_last,BRANCH_NAME as branch_name_last,EXAM_VENUE,SEMESTER as psntsem,year as year from STUDENT_HALL_TICKETS_DATA s  where  s.regdno='" + chart.regdno + "' and exam_type='" + chart.type1 + "' and ATTENDANCE_STATUS='Y' and sub_type='P' and semester ='" + chart.id1 + "' order by s.exam_date";
                return await Task.FromResult(GetAllData52hall<Hallticket>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getstudentanouncments(string groupcode, string regdno)
        {
            string query = "";
            try
            {
                query = @"select  content_data,am.id,am.group_code as group_code,af.createdby,format(am.dt_time,'dd-MMM-yyyy') as dt_time,af.FILE_NAME as FILE_NAME1,af.FILE_PATH as FILE_PATH1,af.DISPLAY_FLAG ,af.id,af.ANNOUNCE_ID as folderid from ANNOUNCE_MASTER  am
left join ANNOUNCEMENT_FILES af ON af.ANNOUNCE_ID = am.id 
where am.group_code='" + groupcode + "' and am.display_flag = 'Y' order by dt_time desc  ";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<LMSsubject>> getlmssubjects(string regdno, string sem, string campus_code, string COLLEGE_CODE, string DEPT_CODE, string BRANCH_CODE, string curr_sem)
        {
            string query = "";
            try
            {

                //                query = @"SELECT distinct H.roomno,H.building_name,H.subject_type,C.FACULTY_ID,
                //C.REGDNO,C.SUBJECT_CODE,C.SUBJECT_NAME,C.GROUP_CODE,C.VIRTUAL_SECTION,
                //C.SECTION,C.BATCH, C.BRANCH_CODE,C.semester,
                // C.CAMPUS_CODE,C.COLLEGE_CODE FROM CBCS_STUDENT_SUBJECT_ASSIGN C
                //left join HOD_STAFF_SUBJECTS_MASTER H on C.GROUP_CODE = H.GROUP_CODE
                //where C.REGDNO ='" + regdno + "' and H.SMS is null and H.subject_type in('P', 'T', 'TP','J') " +
                //" and  C.SEMESTER = '" + curr_sem + "'";

                query = @"SELECT distinct H.subject_type,C.FACULTY_ID,
C.REGDNO,C.SUBJECT_CODE,C.SUBJECT_NAME,C.GROUP_CODE,C.VIRTUAL_SECTION,
C.SECTION,C.BATCH, C.BRANCH_CODE,C.semester,
 C.CAMPUS_CODE,C.COLLEGE_CODE FROM CBCS_STUDENT_SUBJECT_ASSIGN C
left join HOD_STAFF_SUBJECTS_MASTER H on C.GROUP_CODE = H.GROUP_CODE
where C.REGDNO ='" + regdno + "' and H.subject_type in('P', 'T', 'TP','J') " +
" and  C.SEMESTER in( '" + curr_sem + "','92')";

                return await Task.FromResult(GetAllData42<LMSsubject>(query, null));

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<LMSsubject> getlmssubjectsmodeulepercentage(string sub_code, string group_code)
        {
            try
            {

                string query = @"with t(id,Topic_name,tid) as (select t.Id,t.Topic_name,
                                 (select max(id) from EMP_MODULE_TRANSACTION where Topic_id=t.Id and Status='Completed' and Group_code='" + group_code + "') from MODULE_MASTER m,MODULE_TRANSACTION t  " +
                                " where m.Id=t.Module_id and Course_code='" + sub_code + "' and m.Acd_year='2023')   select isnull((sum(case when tid is null then 0 else 1 end)*100)/count(*),'') as count  from t";
                var dis = await Task.FromResult(GetSingleData102<LMSsubject>(query, null));
                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getfilemasterfolders(string groupcode)
        {
            string query = "";
            try
            {
                query = @"select id, foldername, groupcode as group_code, Folderorder, Accessrights from
    file_master where groupcode = '" + groupcode + "' AND Accessrights = 'Y' order by folderorder";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getassignmentmasterfolders(string groupcode)
        {
            string query = "";
            try
            {
                query = @"select isnull(am.Title,'') as title,sa.regdno,sa.Status, am.Id as id,af.Assginmentid as assignmentid, ISNULL(am.subjectcode,'') as subject_code,ISNULL(am.groupcode,'') as group_code, ISNULL(ce.format_id,'') as formatid,ISNULL(format_name,'') as name,
                            ISNULL(section_name,'') as section,ISNULL(ce.section_id,'') as sectionid,ISNULL(section_type,'') as section_type,ISNULL(section_sub_name,'') as section_group,ISNULL(section_sub_id,'') as section_sub_id,
                            ISNULL(section_sub_marks,'') as section_sub_marks,ISNULL(ce.section_marks,'') as marks,ISNULL(ce.batch,'') as batch,ISNULL(am.STATUS,'') as status,FORMAT(last_date,'dd-MMM-yyyy') as date,
                            ISNULL(am.max_marks,'') as max_marks,am.dttime as dt_time, ISNULL(am.Description,'') as Description,am.Id as flag,ISNULL(am.mode,'') as mode,am.publish_flag,ce.SECTION_ID,ce.id ,
                               af.filename,af.filetype,af.uploadedby,af.uploadeddate,af.id as folderid ,    
(Case when am.mode = 'Online' THEN '(Online)' else '(Offline)' end) as  mode1 from ASSIGNMENT_MASTER am 
                                left join ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE and ce.SECTION_NAME='Assignment' and DISPLAY_FLAG='y' and am.Id=ce.ASSIGN_FLAG
  left join ASSIGNMENT_FILES af ON am.Id = af.Assginmentid
    left join student_assignments sa ON sa.assignmentid = af.Assginmentid where am.groupcode='" + groupcode + "'";
                //query = @"SELECT id,max_marks, TITLE, dESCRIPTION, GROUPCODE, dttime as date,id,(Case when Mode = 'ON' THEN '(Online)' else '(Offline)' end) as  type FROM ASSIGNMENT_MASTER where groupcode='" + groupcode + "' order by dttime desc";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getfilemasterfoldersDATA(string groupcode)
        {
            string query = "";
            try
            {
                query = " select *  from FILE_TRANSACTION where  groupcode='" + groupcode + "'";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> insertfilemaster(string groupcode)
        {
            int j = 0;
            IEnumerable<LMSsubject> lms = null;
            var qry = "";
            try
            {
                qry = "INSERT INTO FILE_MASTER(Foldername, Folderorder, GroupCode, Accessrights, collegecode, campuscode)";
                qry = qry + " VALUES ('Course objectives / outcomes', 1, '" + groupcode + "', 'Y', '" + groupcode.Split('#')[1] + "','" + groupcode.Split('#')[0] + "'),('Course syllabus', 2, '" + groupcode + "', 'Y', '" + groupcode.Split('#')[1] + "','" + groupcode.Split('#')[0] + "'),('Teaching plan', 3, '" + groupcode + "', 'Y', '" + groupcode.Split('#')[1] + "','" + groupcode.Split('#')[0] + "'),('Course material', 4, '" + groupcode + "', 'Y', '" + groupcode.ToString().Split('#')[1] + "','" + groupcode.Split('#')[0] + "'),('Old question papers', 5, '" + groupcode + "', 'Y', '" + groupcode.Split('#')[1] + "','" + groupcode.Split('#')[0] + "')";

                int k = await Task.FromResult(InsertData102(qry, null));
                if (k > 0)
                {
                    //retnVal = true;
                    lms = await getfilemasterfolders(groupcode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lms;
        }

        public async Task<IEnumerable<LMSsubject>> getassignmentmasterfoldersASSIGN(string groupcode, string assignid)
        {
            string query = "";
            try
            {
                query = @"select isnull(am.Title,'') as title, am.Id as id,am.Idas assignmentid, ISNULL(am.subjectcode,'') as subject_code,ISNULL(am.groupcode,'') as group_code, ISNULL(ce.format_id,'') as formatid,ISNULL(format_name,'') as name,
                            ISNULL(section_name,'') as section,ISNULL(ce.section_id,'') as sectionid,ISNULL(section_type,'') as section_type,ISNULL(section_sub_name,'') as section_group,ISNULL(section_sub_id,'') as section_sub_id,
                            ISNULL(section_sub_marks,'') as section_sub_marks,ISNULL(ce.section_marks,'') as marks,ISNULL(ce.batch,'') as batch,ISNULL(am.STATUS,'') as status,FORMAT(last_date,'dd-MMM-yyyy') as date,
                            ISNULL(am.max_marks,'') as max_marks,am.dttime as dt_time, ISNULL(am.Description,'') as Description,am.Id as flag,ISNULL(am.mode,'') as mode,am.publish_flag,ce.SECTION_ID,ce.id ,
                               af.filename,af.filetype,af.uploadedby,af.uploadeddate,af.id as folderid ,    
(Case when am.mode = 'Online' THEN '(Online)' else '(Offline)' end) as  mode1 from ASSIGNMENT_MASTER am 
                                left join ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE and ce.SECTION_NAME='Assignment' and DISPLAY_FLAG='y' and am.Id=ce.ASSIGN_FLAG
  left join ASSIGNMENT_FILES af ON am.Id = af.Assginmentid
                                where groupcode='" + groupcode + "' and af.Assginmentid='" + assignid + "' ";
                //query = @"SELECT id,max_marks, TITLE, dESCRIPTION, GROUPCODE, dttime as date,id,(Case when Mode = 'ON' THEN '(Online)' else '(Offline)' end) as  type FROM ASSIGNMENT_MASTER where groupcode='" + groupcode + "' order by dttime desc";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<LMSsubject>> getstudentAssignmentmarks(string regdno, string groupcode, string subcode)
        {
            string query = "";
            try
            {
                //query = @"select [ID] ,[REGDNO] ,[NAME] ,[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],isnull([MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,isnull([UPDATED_BY],'1') as  UPDATED_BY ,[UPDATED_DATE] ,[BRANCH_CODE],[DEPT_CODE],[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,[FORMAT_ID],	[GROUP_CODE] ,[STUDENT_FLAG] ,[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],[AC_YEAR] from CE_MARKS_MASTER where regdno='" + regdno + "' and SUB_SECTION_NAME like'Assignment%' and group_code='" + groupcode + "' ";
                query = @"select cea.mid,cea.STATUS,ce.ID,[REGDNO] ,[NAME] ,ce.[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],
isnull(ce.[MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,[UPDATED_BY] ,[UPDATED_DATE] ,[BRANCH_CODE],
[DEPT_CODE],ce.[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,ce.[FORMAT_ID],	
ce.[GROUP_CODE] ,[STUDENT_FLAG] ,ce.[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],ce.[AC_YEAR] from CE_MARKS_MASTER ce
left join CE_EMPLOYEE_ASSIGN cea on cea.GROUP_CODE = ce.GROUP_CODE and cea.SECTION_SUB_ID = ce.SUB_SECTION_ID where regdno='" + regdno + "' and ce.GROUP_CODE='" + groupcode + "' and DISPLAY_FLAG='Y'";

                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getstudentsectionlist(string subject_code, string year, string campus_code, string sem)
        {
            string query = "";
            try
            {
                query = @"select distinct csm.section_name as section from CE_FORMAT_MASTER cef
inner join CE_SECTION_MASTER csm on csm.FORMAT_ID=cef.id
inner join CE_SUB_SECTION_MASTER cssm on csm.ID=cssm.SECTION_ID 
where cef.subject_code='" + subject_code + "' and cef.ac_year='" + year + "' and cef.CAMPUS_CODE in('All','" + campus_code + "')  and cef.semester in ('92','" + sem + "') order by csm.section_name";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getstudentQuizmarks(string regdno, string groupcode)
        {
            string query = "";
            try
            {
                // query = @"select [ID] ,[REGDNO] ,[NAME] ,[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],isnull([MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,isnull([UPDATED_BY],'1') as  UPDATED_BY ,[UPDATED_DATE] ,[BRANCH_CODE],[DEPT_CODE],[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,[FORMAT_ID],	[GROUP_CODE] ,[STUDENT_FLAG] ,[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],[AC_YEAR] from CE_MARKS_MASTER where regdno='" + regdno + "' and SUB_SECTION_NAME like'Quiz%' and group_code='" + groupcode + "' ";
                query = @"select cea.STATUS,ce.ID,[REGDNO] ,[NAME] ,ce.[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],
isnull(ce.[MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,[UPDATED_BY] ,[UPDATED_DATE] ,[BRANCH_CODE],
[DEPT_CODE],ce.[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,ce.[FORMAT_ID],	
ce.[GROUP_CODE] ,[STUDENT_FLAG] ,ce.[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],ce.[AC_YEAR] from CE_MARKS_MASTER ce
left join CE_EMPLOYEE_ASSIGN cea on cea.GROUP_CODE = ce.GROUP_CODE and cea.SECTION_SUB_ID = ce.SUB_SECTION_ID where regdno='" + regdno + "' and SUB_SECTION_NAME like 'Quiz%' and ce.group_code='" + groupcode + "'";

                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getstudentMidmarks(string regdno, string groupcode)
        {
            string query = "";
            try
            {
                //  query = @"select [ID] ,[REGDNO] ,[NAME] ,[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],isnull([MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,isnull([UPDATED_BY],'1') as  UPDATED_BY ,[UPDATED_DATE] ,[BRANCH_CODE],[DEPT_CODE],[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,[FORMAT_ID],	[GROUP_CODE] ,[STUDENT_FLAG] ,[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],[AC_YEAR] from CE_MARKS_MASTER where regdno='" + regdno + "' and SUB_SECTION_NAME like'Mid%' and group_code='" + groupcode + "' ";
                query = @"select cea.STATUS,ce.ID,[REGDNO] ,[NAME] ,ce.[SUBJECT_CODE] ,isnull([MARKS] ,'0') as MARKS,[SUB_SECTION_NAME],
isnull(ce.[MAX_MARKS] ,'0') as MAX_MARKS,[USER_ID] ,[DT_TIME] ,[UPDATED_BY] ,[UPDATED_DATE] ,[BRANCH_CODE],
[DEPT_CODE],ce.[BATCH] ,[SECTION],[SEMESTER],[COLLEGE_CODE] ,[CAMPUS_CODE] ,	[TRANSFER_FLAG] ,ce.[FORMAT_ID],	
ce.[GROUP_CODE] ,[STUDENT_FLAG] ,ce.[SECTION_ID] ,[SUB_SECTION_ID],[ASSIG_FLAG],ce.[AC_YEAR] from CE_MARKS_MASTER ce
left join CE_EMPLOYEE_ASSIGN cea on cea.GROUP_CODE = ce.GROUP_CODE and cea.SECTION_SUB_ID = ce.SUB_SECTION_ID where regdno='" + regdno + "' and SUB_SECTION_NAME like'Mid%' and ce.group_code='" + groupcode + "' ";

                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Deleteassignmentfileupload(string assignid, string regdno, string group_code, string fileid, string filename)
        {
            try
            {
                int j = 0;
                string Query = "";
                Query = "delete STUDENT_FILES where regdno='" + regdno + "' and id='" + fileid + "' and Assginmentid='" + assignid + "' and groupcode='" + group_code + "' and Filename='" + filename + "' ";
                j = await Task.FromResult(Delete102(Query, null));
                return j;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> add_assidn_data(string folderid)
        {
            var sqlQuery = @" select top 1 dESCRIPTION as description,format(lastdate,'dd-MMM-yyyy') as lastdate,mode as Mode,max_marks,af.id as id,af.Assginmentid as Assginmentid,af.Filename as Filename,af.filetype as filetype,af.uploadedby,format(af.uploadeddate,'dd-MMM-yyyy') as uploadeddate,title as Title from ASSIGNMENT_MASTER am 
                          left join ASSIGNMENT_FILES af on af.Assginmentid=am.id where am.id='" + folderid + "'";

            return await Task.FromResult(GetAllData102<LMSsubject>(sqlQuery, null));

        }
        public async Task<int> Insertassigneditordata(string result, string regdno, string campus_code, string college_code, string assignid)
        {
            int j = 0;
            string Query = "";
            try
            {
                Query = "update STUDENT_ASSIGNMENTS set description='" + result + "',status='S' where regdno='" + regdno + "'and campus_code='" + campus_code + "' and college_code='" + college_code + "' and assignmentid='" + assignid + "'  ";
                j = await Task.FromResult(InsertData102(Query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }
        public async Task<IEnumerable<LMSsubject>> getassignmentmasterfoldersASSIGN(string groupcode, string assignid, string folderid, string regdno)
        {
            string query = "";
            try
            {
                if (folderid != "")
                {
                    query = @"
select isnull(am.Title,'') as title,sa.regdno,sa.description as stdescription,sa.Status, am.Id as id, am.Id as assignmentid, ISNULL(am.subjectcode,'') as subject_code,ISNULL(am.groupcode,'') as group_code, ISNULL(ce.format_id,'') as formatid,ISNULL(format_name,'') as name,
                            ISNULL(section_name,'') as section,ISNULL(ce.section_id,'') as sectionid,ISNULL(section_type,'') as section_type,ISNULL(section_sub_name,'') as section_group,ISNULL(section_sub_id,'') as section_sub_id,
                            ISNULL(section_sub_marks,'') as section_sub_marks,ISNULL(ce.section_marks,'') as marks,ISNULL(ce.batch,'') as batch,ISNULL(am.STATUS,'') as status,FORMAT(am.lastdate,'dd-MMM-yyyy') as date,
                            ISNULL(am.max_marks,'') as max_marks,am.dttime as dt_time, ISNULL(am.Description,'') as Description,am.Id as flag,ISNULL(am.mode,'') as mode,am.publish_flag,ce.SECTION_ID,ce.id ,
                               af.filename,af.filetype,af.uploadedby,af.uploadeddate,af.id as folderid ,sf.filename as sffilename, sf.id as sffileid,sf.filetype as sffiletype,sa.lastdate,sa.status,
(Case when am.mode = 'Online' THEN '(Online)' else '(Offline)' end) as  mode1 from ASSIGNMENT_MASTER am 
                                left join ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE and ce.SECTION_NAME='Assignment' and DISPLAY_FLAG='y' and am.Id=ce.ASSIGN_FLAG
  left join ASSIGNMENT_FILES af ON am.Id = af.Assginmentid
    left join student_assignments sa ON am.id=sa.assignmentid 
	    left join STUDENT_FILES sf ON am.id= sf.Assginmentid   and sf.regdno= sa.regdno                            

where am.groupcode='" + groupcode + "' and am.id='" + assignid + "' and sa.regdno='" + regdno + "' and af.id='" + folderid + "' ";
                }
                else
                {
                    query = @"
select isnull(am.Title,'') as title,sa.regdno,sa.description as stdescription,sa.Status, am.Id as id, am.Id  as assignmentid, ISNULL(am.subjectcode,'') as subject_code,ISNULL(am.groupcode,'') as group_code, ISNULL(ce.format_id,'') as formatid,ISNULL(format_name,'') as name,
                            ISNULL(section_name,'') as section,ISNULL(ce.section_id,'') as sectionid,ISNULL(section_type,'') as section_type,ISNULL(section_sub_name,'') as section_group,ISNULL(section_sub_id,'') as section_sub_id,
                            ISNULL(section_sub_marks,'') as section_sub_marks,ISNULL(ce.section_marks,'') as marks,ISNULL(ce.batch,'') as batch,ISNULL(am.STATUS,'') as status,FORMAT(am.lastdate,'dd-MMM-yyyy') as date,
                            ISNULL(am.max_marks,'') as max_marks,am.dttime as dt_time, ISNULL(am.Description,'') as Description,am.Id as flag,ISNULL(am.mode,'') as mode,am.publish_flag,ce.SECTION_ID,ce.id ,
                               af.filename,af.filetype,af.uploadedby,af.uploadeddate,af.id as folderid ,sf.filename as sffilename, sf.id as sffileid,sf.filetype as sffiletype,sa.lastdate,sa.status,
(Case when am.mode = 'Online' THEN '(Online)' else '(Offline)' end) as  mode1 from ASSIGNMENT_MASTER am 
                                left join ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE and ce.SECTION_NAME='Assignment' and DISPLAY_FLAG='y' and am.Id=ce.ASSIGN_FLAG
  left join ASSIGNMENT_FILES af ON am.Id = af.Assginmentid
    left join student_assignments sa ON am.id=sa.assignmentid 
	    left join STUDENT_FILES sf ON am.id= sf.Assginmentid   and sf.regdno= sa.regdno                            

where am.groupcode='" + groupcode + "' and am.id='" + assignid + "' and sa.regdno='" + regdno + "'";
                }
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Insertassignuploaddata(string fileNameonly, string Regdno, string campus_code, string college_code, string assignid, string ext, string group_code)
        {
            int j = 0;
            string Query = "";
            try
            {
                var query1 = @"insert into STUDENT_FILES(Assginmentid, Filename, filetype, uploadedby, uploadeddate, regdno, groupcode) values('" + assignid + "','" + fileNameonly + "','" + ext + "','" + Regdno + "',getdate(),'" + Regdno + "','" + group_code + "')";

                int k = await Task.FromResult(InsertData102(query1, null));
                if (k > 0)
                {
                    Query = "update STUDENT_ASSIGNMENTS set status='S' where regdno='" + Regdno + "'and campus_code='" + campus_code + "' and college_code='" + college_code + "' and assignmentid='" + assignid + "'  ";
                    j = await Task.FromResult(Update102(Query, null));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }
        public async Task<IEnumerable<LMSsubject>> getassignmentmasterfolders(string subcode, string groupcode, string uid, string campus_code, string COLLEGE_CODE, string DEPT_CODE, string BRANCH_CODE, string COURSE_CODE)
        {
            string query = "";
            try
            {
                query = @"select isnull(am.Title,'') as title,isnull(ce.SECTION_SUB_NAME ,'') as sub_section_name,isnull(sa.remarks,'') as remarks,sa.regdno,sa.Status, am.Id as id,am.Id as assignmentid, ISNULL(am.subjectcode,'') as subject_code,ISNULL(am.groupcode,'') as group_code, ISNULL(ce.format_id,'') as formatid,ISNULL(format_name,'') as name,
                            ISNULL(section_name,'') as section,ISNULL(ce.section_id,'') as sectionid,ISNULL(section_type,'') as section_type,ISNULL(section_sub_name,'') as section_group,ISNULL(section_sub_id,'') as section_sub_id,
                            ISNULL(section_sub_marks,'') as section_sub_marks,ISNULL(ce.section_marks,'') as marks,ISNULL(ce.batch,'') as batch,ISNULL(am.STATUS,'') as status,sa.lastdate as date,
                            ISNULL(am.max_marks,'') as max_marks,am.dttime as dt_time, ISNULL(am.Description,'') as Description,am.Id as flag,ISNULL(am.mode,'') as mode,am.publish_flag,ce.SECTION_ID,ce.id ,
                               af.filename,af.filetype,af.uploadedby,af.uploadeddate,af.id as folderid ,FORMAT(sa.lastdate,'dd-MMM-yyyy') as assign_last_date,FORMAT(sa.lastdate,'hh:mm:ss tt') as assign_last_time,sa.status,    
(Case when am.mode = 'Online' THEN '(Online)' else '(Offline)' end) as  mode1 from ASSIGNMENT_MASTER am 
                                left join ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE and ce.SECTION_NAME='Assignment' and DISPLAY_FLAG='y' and am.Id=ce.ASSIGN_FLAG
  left join ASSIGNMENT_FILES af ON am.Id = af.Assginmentid 
    left join student_assignments sa ON sa.assignmentid = am.Id 
where am.groupcode='" + groupcode + "' and sa.regdno='" + uid + "' ";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getstudentassignments(string regdno, string groupcode)
        {
            string query = "";
            try
            {
                query = @"select id,assignmentid,regdno,Status,marks,campus_code,college_code,
isnull(lastdate, isnull( (select lastdate from ASSIGNMENT_MASTER where id=s.assignmentid) ,getdate())) as lastdate 
from STUDENT_ASSIGNMENTS s  where regdno='" + regdno + "' and groupcode='" + groupcode + "' ";
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LMSsubject> getsubjectname(string regdno, string groupcode)
        {
            string query = "";
            try
            {
                query = @"select distinct top 1 subject_name from CBCS_STUDENT_SUBJECT_ASSIGN  where regdno='" + regdno + "' and group_code='" + groupcode + "' ";
                return await Task.FromResult(GetSingleData42<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<IEnumerable<LmsResource>> Getresource_unitwisedata(string subject_code, string campus,string facultyid)
        //{
        //    try
        //    {
        //        var facultyid1 = facultyid.Trim();
        //        string query = @"select module_unit+' - '+ module_name as name,id as order_id,acd_year as year,Module_duration  from MODULE_MASTER where Campus_code in('All','" + campus + "') and course_code='" + subject_code + "' and acd_year='2023' and generated_by ='"+facultyid1+"'  ";
        //        return await Task.FromResult(GetAllData102<LmsResource>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<IEnumerable<LmsResource>> Getresource_unitwisedata(string subject_code, string campus, string regdno, string college_code, string curr_sem)
        {
            try
            {


                string query = @"select module_unit+' - '+ module_name as name,id as order_id,acd_year as year,Module_duration  from MODULE_MASTER where Campus_code in ('All','" + campus + "') and course_code='" + subject_code + "' and acd_year='2023' and  Semester in ('" + curr_sem + "' ,'92')";
                return await Task.FromResult(GetAllData102<LmsResource>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public LmsResource getcrseresrctrns1(string folder_id, string group_code, string image, string regdno, string curr_sem)
        {
            try
            {
                var facultyid1 = "";
                string query1 = "SELECT distinct H.EMPID as faculty_id FROM CBCS_STUDENT_SUBJECT_ASSIGN C left join HOD_STAFF_SUBJECTS_MASTER H on C.GROUP_CODE = H.GROUP_CODE where C.REGDNO = '" + regdno + "' and  H.subject_type in('P', 'T', 'TP')  and C.SEMESTER in ('" + curr_sem + "' ,'92') and C.SUBJECT_CODE = '" + group_code + "'  and DACTIVE_FLAG='A'";
                List<LmsResource> data = GetAllData42<LmsResource>(query1, null);
                for (int i = 0; i < data.Count(); i++)
                {
                    if (i == 0)
                    {
                        facultyid1 = "'" + data[i].faculty_id + "'";
                    }
                    else
                    {
                        facultyid1 += " , '" + data[i].faculty_id + "'";
                    }

                }
                string query = @"select id,Accessrights as action_name,Uploadedby as postedby from FILE_TRANSACTION where FolderId='" + folder_id + "' and groupcode='" + group_code + "' and Filename='" + image + "' and Filetype!='DATA'  and Accessrights in ('S','Y') and uploadedby in (" + facultyid1 + ")";
                var dis = GetSingleData102<LmsResource>(query, null);
                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public LmsResource getcrseresrctrns2(string folder_id, string group_code, string image)
        {
            try
            {

                string query = @"select id,Accessrights as action_name,Uploadedby as postedby from FILE_TRANSACTION where FolderId='" + folder_id + "' and groupcode='" + group_code + "' and Filename='" + image + "' and Filetype!='DATA'  and Accessrights in ('S','Y')";
                var dis = GetSingleData102<LmsResource>(query, null);
                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public LmsResource getcrseresrctrns_additional(string folder_id, string group_code, string image)
        {
            try
            {

                string query = @"select id,Accessrights as action_name,Uploadedby as postedby from FILE_TRANSACTION where FolderId='" + folder_id + "' and groupcode='" + group_code + "' and Filename='" + image + "' and Filetype!='DATA' and Accessrights ='Y'";
                var dis = GetSingleData102<LmsResource>(query, null);
                if (dis != null)
                {
                    return dis;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public LmsResource getcrseresrctrnsrepo(string folder_id, string group_code)
        {
            try
            {

                string query = @"select data as name,Accessrights as action_name,Id as id  from FILE_TRANSACTION where FolderId='" + folder_id + "' and groupcode='" + group_code + "' and Filetype='DATA'  and Accessrights in ('S','Y')";
                var dis = GetSingleData102<LmsResource>(query, null);
                if (dis != null)
                {
                    return dis;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<LmsResource>> Getresource_additionaldata(string group_code, string college, string campus, string userid, string dept)
        {
            try
            {

                string sqlQuery4 = @"select isnull(isactive,'0') as status,id,foldername as name,groupcode as grp_code,Folderorder as order_id,Accessrights as action_name,format(date_time,'dd-MMM-yyyy HH:mm tt') as time from file_master where groupcode='" + group_code + "' and type='I' and  Accessrights='Y' order by folderorder";
                var dis = await Task.FromResult(GetAllData102<LmsResource>(sqlQuery4, null));
                return dis;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<LmsResource> getcrseresrctrns(string folder_id, string group_code)
        {
            try
            {

                string query = @"select data as name,Accessrights as action_name,Id as id from FILE_TRANSACTION where FolderId='" + folder_id + "' and groupcode='" + group_code + "' and Filetype='DATA' and Accessrights ='Y' ";
                var dis = await Task.FromResult(GetSingleData102<LmsResource>(query, null));
                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IEnumerable<LmsResource>> getcrseresrcrepo(string group_code, string type)
        {
            try
            {

                string query = @"select isnull(isactive,'0') as status,id,foldername as name,groupcode as grp_code,Folderorder as order_id,Accessrights as action_name,format(date_time,'dd-MMM-yyyy HH:mm tt') as time,generated_by as postedby  from file_master where groupcode='" + group_code + "' and type='" + type + "'  and Accessrights in ('S','Y')  order by folderorder";
                var dis = await Task.FromResult(GetAllData102<LmsResource>(query, null));
                return dis;


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<LmsResource>> Getresource_topicwisedata(string subject_code, string module_id)
        {
            try
            {
                string query = @"select Topic_name,Delivery_method,Periods,format(date,'dd-MMM-yyyy') as date,Id as order_id,Module_id,acd_year as year,Type as TYPE from MODULE_TRANSACTION where subject_code='" + subject_code + "' and Module_id ='" + module_id + "' and acd_year='2023'  order by format(date,'dd-MMM-yyyy') desc";
                return await Task.FromResult(GetAllData102<LmsResource>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LmsResource>> gettextbooks(string subject_code, string campus)
        {
            try
            {
                string query = @"select GENERATED_DATE,TYPE,TEXTBOOK_NAME,UNIT_MAPPING,AUTHOR,YEAR as year,EDITION,PUBLICATION,PUBLICATION_PLACE,ISBN,SUBJECT_TYPE,SUBJECT_NAME  from CE_TEXTBOOK_MASTER where SUBJECT_CODE='" + subject_code + "'  and campus_code='" + campus + "' order by GENERATED_DATE desc";
                return await Task.FromResult(GetAllData102<LmsResource>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<studenttrack>> getacademicsemesterasync(string userid)
        {
            try
            {
                //   var query = @"select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER <= (select curr_sem from student_master where regdno = '" + userid + "') union select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER >= '21'";

                //                var query = @"select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER 
                //<= (select curr_sem from student_master where regdno = '" + userid + "')union select SEMESTER, SEMESTER_DISPLAY from SEMESTER_DISPLAY_TABLE where SEMESTER >= '21'";

                var query = @"select SEMESTER from new_results_student where regdno = '" + userid + "' AND process_type NOT in ('S')";

                return await Task.FromResult(GetAllData52<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<LMSsubject>> getstudentCEList(string regdno, string subject_code, string year, string campus_code, string sem)
        {
            string query = "";
            try
            {

                query = "select count(*) as mid from CBCS_STUDENT_SUBJECT_ASSIGN where regdno='" + regdno + "' and SEMESTER='92'";

                IEnumerable<LMSsubject> countcnt = await Task.FromResult(GetAllData42<LMSsubject>(query, null));

                if (Convert.ToInt32(countcnt.FirstOrDefault().mid) >= 1)
                {

                    query = @"select cssm.id as id,cef.semester,cef.CAMPUS_CODE,isnull(cef.subject_code,'') as subjectcode, isnull(cef.id,'') as formatid,
isnull(format_name,'') as name,isnull(section_name,'') as section,isnull(cssm.section_id,'') as sectionid,isnull(section_type,'') as section_type,isnull(BESTOF_COUNT,'') as best,
isnull(section_sub_name,'') as SUB_SECTION_NAME,isnull(cssm.id,'') as SUB_SECTION_ID,isnull(cssm.MARKS,'') as MAX_MARKS,isnull(cssm.MARKS,'') as maxxx_marks,isnull(csm.BESTOF_COUNT,'') as BESTOF_COUNT,isnull(csm.TOTAL_COUNT,'') as TOTAL_COUNT,
isnull(csm.TOTAL_MARKS,'') as marks,isnull(format(cssm.last_date,'dd-MMM-yyyy'),'') as date,isnull(cef.max_marks,'')
as max_marks1,isnull(format(cssm.FROM_DATE,'dd-MMM-yyyy'),'') as from_date,isnull(format(cssm.TO_DATE,'dd-MMM-yyyy'),'') as todate from CE_FORMAT_MASTER cef
inner join CE_SECTION_MASTER csm on csm.FORMAT_ID=cef.id
inner join CE_SUB_SECTION_MASTER cssm on csm.ID=cssm.SECTION_ID 
where cef.subject_code='" + subject_code + "' and cef.ac_year='" + year + "' and cef.CAMPUS_CODE in('All','" + campus_code + "') and cef.semester in ('92','" + sem + "') order by cssm.id";

                }
                else
                {
                    query = @"select cssm.id as id,cef.semester,cef.CAMPUS_CODE,isnull(cef.subject_code,'') as subjectcode, isnull(cef.id,'') as formatid,
isnull(format_name,'') as name,isnull(section_name,'') as section,isnull(cssm.section_id,'') as sectionid,isnull(section_type,'') as section_type,isnull(BESTOF_COUNT,'') as best,
isnull(section_sub_name,'') as SUB_SECTION_NAME,isnull(cssm.id,'') as SUB_SECTION_ID,isnull(cssm.MARKS,'') as MAX_MARKS,isnull(cssm.MARKS,'') as maxxx_marks,isnull(csm.BESTOF_COUNT,'') as BESTOF_COUNT,isnull(csm.TOTAL_COUNT,'') as TOTAL_COUNT,
isnull(csm.TOTAL_MARKS,'') as marks,isnull(format(cssm.last_date,'dd-MMM-yyyy'),'') as date,isnull(cef.max_marks,'')
as max_marks1,isnull(format(cssm.FROM_DATE,'dd-MMM-yyyy'),'') as from_date,isnull(format(cssm.TO_DATE,'dd-MMM-yyyy'),'') as todate from CE_FORMAT_MASTER cef
inner join CE_SECTION_MASTER csm on csm.FORMAT_ID=cef.id
inner join CE_SUB_SECTION_MASTER cssm on csm.ID=cssm.SECTION_ID 
where cef.subject_code='" + subject_code + "' and cef.ac_year='" + year + "' and cef.CAMPUS_CODE in('All','" + campus_code + "') and cef.semester in ('" + sem + "') order by cssm.id";


                }
                return await Task.FromResult(GetAllData102<LMSsubject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<LMSsubject>> getassignmentpending(string regdno)
        {
            try
            {
                //                var query1 = @"SELECT DISTINCT am.id,ISNULL(am.Title, '') as title,ISNULL(am.Title, '') + ' ' + ISNULL(ce.SECTION_SUB_NAME, '') as sub_section_name, sa.regdno,  sa.Status, am.Id as assignmentid, ISNULL(am.subjectcode, '') as subject_code,  ISNULL(am.groupcode, '') as group_code,  ISNULL(ce.format_id, '') as formatid, ISNULL(format_name, '') as name, ISNULL(section_name, '') as section,  ISNULL(ce.section_id, '') as sectionid,  ISNULL(section_type, '') as section_type, ISNULL(section_sub_name, '') as section_group, ISNULL(section_sub_id, '') as section_sub_id, ISNULL(section_sub_marks, '') as section_sub_marks, ISNULL(ce.section_marks, '') as marks, ISNULL(ce.batch, '') as batch, ISNULL(am.STATUS, '') as status, sa.lastdate as date, ISNULL(am.max_marks, '') as max_marks, am.dttime as dt_time, ISNULL(am.Description, '') as Description, am.Id as flag, ISNULL(am.mode, '') as mode, am.publish_flag, ce.SECTION_ID, ce.id, af.filename, af.filetype, af.uploadedby, af.uploadeddate, af.id as folderid, FORMAT(sa.lastdate, 'dd-MMM-yyyy') as assign_last_date, FORMAT(sa.lastdate, 'hh:mm:ss tt') as assign_last_time, sa.status, (CASE WHEN am.mode = 'Online' THEN '(Online)' ELSE '(Offline)' END) as mode1, am.lastdate as assignment_lastdate -- Alias for am.lastdate
                //FROM ASSIGNMENT_MASTER am  LEFT JOIN ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE AND ce.SECTION_NAME = 'Assignment' AND DISPLAY_FLAG = 'y' AND ce.ASSIGN_FLAG = am.Id
                //    LEFT JOIN ASSIGNMENT_FILES af ON am.Id = af.Assginmentid LEFT JOIN student_assignments sa ON sa.assignmentid = am.Id  LEFT JOIN CE_Marks_master cm ON cm.ASSIG_FLAG = am.Id
                //WHERE sa.regdno = '" + regdno + "' AND sa.status = 'N'  AND am.mode = 'Online'  AND am.lastdate BETWEEN GETDATE() AND am.lastdate ORDER BY assignment_lastdate,  am.id DESC";
                var query1 = @"SELECT DISTINCT am.id,ISNULL(am.Title, '') as title,
                                sa.Status, am.Id as assignmentid, ISNULL(am.subjectcode, '') as subject_code, ISNULL(AM.Subjectname,'') AS subject_name,  ISNULL(am.groupcode, '') as group_code, 
                                ISNULL(am.Description, '') as Description, am.Id as flag, 
                                FORMAT(sa.lastdate, 'dd-MMM-yyyy') as assign_last_date, FORMAT(sa.lastdate, 'hh:mm:ss tt') as assign_last_time,
                                sa.status 
                                FROM ASSIGNMENT_MASTER am  LEFT JOIN ce_employee_assign ce ON am.GROUPCODE = ce.GROUP_CODE AND ce.SECTION_NAME = 'Assignment' 
                                AND DISPLAY_FLAG = 'y' AND ce.ASSIGN_FLAG = am.Id    
                                LEFT JOIN student_assignments sa ON sa.assignmentid = am.Id 
                                WHERE sa.regdno = '" + regdno + "' AND sa.status In ('N','R')  AND am.mode = 'Online'  AND am.lastdate BETWEEN GETDATE() AND am.lastdate ORDER BY am.id DESC";

                return await Task.FromResult(GetAllData102<LMSsubject>(query1, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<IEnumerable<FeedbackMaster>> getfeedback_subjects(string userid, string sem)
        {

            // int sem1 = Convert.ToInt32(sem) - 1;
            try
            {
                var query = @"select EMPID,CAMPUS_CODE,COLLEGE_CODE,BRANCH_CODE,SUBJECT as subject_code, SUBJECT_NAME as subjectname,Semester,FEEDBACK_SESSION as feedbacksession from feedback_student where  regdno = '" + userid + "' and SEMESTER = '" + sem + "' and  feedback_status='Y'";/*and  SUBJECT_TYPE in('tp'  ,'t','p')*/
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<studenttrack>> getfeedback_subjectsasync(string userid, string sem)
        {
            string query = "";
            try
            {

                //if (sem == "3" || sem == "5" || sem == "7")
                if (sem == "2" || sem == "4" || sem == "6" || sem == "8" || sem == "10")
                {
                    // query = @"select distinct SUBJECT_CODE,SUBJECT_NAME,GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where regdno = '" + userid + "' and SEMESTER = '" + sem + "' and GROUP_CODE is not null and  faculty_id not in ('N' ,'No faculty') and  Emp_name <>'NULL' ";

                    query = @"select distinct SUBJECT_CODE,SUBJECT_NAME,GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where regdno = '" + userid + "' and SEMESTER = '" + sem + "' and GROUP_CODE is not null  and (FACULTY_ID !='No faculty' or FACULTY_ID is NULL)  and  (remarks not like ('%mooc%') or remarks is NULL)";/*and  SUBJECT_TYPE in('tp'  ,'t','p')*/

                }
                else
                {
                    query = @"select distinct SUBJECT_CODE,SUBJECT_NAME,GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where regdno = '" + userid + "' and SEMESTER = '" + sem + "' and GROUP_CODE is not null  and (FACULTY_ID !='No faculty' or FACULTY_ID is NULL)  and  (remarks not like ('%mooc%') or remarks is NULL)";/*and  SUBJECT_TYPE in('tp'  ,'t','p')*/

                }
                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<studenttrack>> getfeedback_facultyasync(string groupcode, string campus, string dept_code)
        {
            try
            {
                var query = @"select top 1 GROUP_CODE,EMPID, emp_name,dept_code,subject_code from HOD_STAFF_SUBJECTS_MASTER where  GROUP_CODE = '" + groupcode + "'  and  CAMPUS='" + campus + "' and sms is null";
                // var query = @"select top 1 GROUP_CODE,FACULTY_ID as empid, NAME,dept_code,subject_code from CBCS_STUDENT_SUBJECT_ASSIGN where   regdno='VU22CSEN0300169' and subject_code='ACCN1001'";
                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //public async Task<IEnumerable<studenttrack>> getfeedback_facultyasyncnew(string groupcode, string campus, string branch_code, string regdno, string subject_code, string sem,string college_code)
        //{
        //    try
        //    {
        //                      var query = "";
        //        if (college_code== "PTY" && branch_code == "Physiotherapy" && (sem == "3"|| sem == "5"))
        //        {

        //            query = @"select emp_name as Emp_name,subject_code as GROUP_CODE,EMPID as empid, dept_code,subject_code from STAFF_SUBJECTS_MASTER where subject_code='"+subject_code+"'";
        //        }
        //        else
        //        {
        //            query = @"select top 1 Emp_name,GROUP_CODE,FACULTY_ID as empid, NAME,dept_code,subject_code from CBCS_STUDENT_SUBJECT_ASSIGN where   regdno='" + regdno + "' and subject_code='" + subject_code + "' and faculty_id not in ('N' ,'No faculty') ";
        //        }

        //        return await Task.FromResult(GetAllData42<studenttrack>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public async Task<IEnumerable<studenttrack>> getfeedback_facultyasyncnew(string groupcode, string campus, string branch_code, string regdno, string subject_code, string sem, string college_code, string batch)
        {
            try
            {
                //var query = @"select top 1 GROUP_CODE,EMPID, emp_name,dept_code,subject_code from HOD_STAFF_SUBJECTS_MASTER where  GROUP_CODE = '" + groupcode + "'  and  CAMPUS='" + campus + "' and sms is null";
                var query = "";
                if (college_code == "GSA" && (branch_code == "BARCH" || branch_code == "MARCH"))
                //if(false)
                {
                    query = @"select distinct hod.empid,emp.Emp_name,subject_code,hod.dept_code,GROUP_CODE from HOD_STAFF_SUBJECTS_MASTER hod ,employee_master emp where SUBJECT_CODE='" + subject_code + "'  and hod.campus='" + campus + "' and SEMESTER='" + sem + "' " +
                        "and BATCH like '%" + batch + "%' and hod.EMPID COLLATE Latin1_General_CI_AS = emp.EMPID and GLEARN_GROUP_CODE COLLATE Latin1_General_CI_AS in (select GLEARN_GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where SUBJECT_CODE='" + subject_code + "' " +
                        "and regdno= '" + regdno + "' and campus_code= '" + campus + "' and SEMESTER= '" + sem + "' and BATCH like '%" + batch + "%')";


                    ////}
                    //query = @"select top 1 c.faculty_id as empid,emp.Emp_name,c.subject_code,c.dept_code,c.GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN c, employee_master emp  where   c.regdno='" + regdno + "' and c.subject_code='" + subject_code + "' and c.faculty_id not in ('N' ,'No faculty') and LTRIM(RTRIM(c.FACULTY_ID))=emp.EMPID";


                }


                else if (college_code == "PTY" && branch_code == "Physiotherapy" && (sem == "3" || sem == "5"))
                {

                    query = @"select emp_name as Emp_name,subject_code as GROUP_CODE,EMPID as empid, dept_code,subject_code from STAFF_SUBJECTS_MASTER where subject_code='" + subject_code + "'";
                }
                /*else
                {
                    query = @"select top 1 Emp_name,GROUP_CODE,FACULTY_ID as empid, NAME,dept_code,subject_code from CBCS_STUDENT_SUBJECT_ASSIGN where   regdno='" + regdno + "' and subject_code='" + subject_code + "' and faculty_id not in ('N' ,'No faculty') ";
                }*/
                else if (college_code == "SMS" && branch_code == "MBA" && (sem == "33333"))
                {

                    //query = @"select top 1 Emp_name,GROUP_CODE,FACULTY_ID as empid, NAME,dept_code,subject_code from CBCS_STUDENT_SUBJECT_ASSIGN where   regdno='" + regdno + "' and subject_code='" + subject_code + "' and faculty_id not in ('N' ,'No faculty') ";

                    // query = @"select top 1 empid,Emp_name,subject_code,dept_code,GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where   regdno='" + regdno + "' and subject_code='" + subject_code + "' and faculty_id not in ('N' ,'No faculty')";
                    query = @"select top 1 c.faculty_id as empid,emp.Emp_name,c.subject_code,c.dept_code,c.GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN c, employee_master emp  where   c.regdno='" + regdno + "' and c.subject_code='" + subject_code + "' and c.faculty_id not in ('N' ,'No faculty') and LTRIM(RTRIM(c.FACULTY_ID))=emp.EMPID";
                }
                else
                {
                    query = @"select distinct hod.empid,emp.Emp_name,subject_code,hod.dept_code,GROUP_CODE from HOD_STAFF_SUBJECTS_MASTER hod ,employee_master emp where SUBJECT_CODE='" + subject_code + "'  and hod.campus='" + campus + "' and SEMESTER='" + sem + "' " +
                        "and BATCH like '%" + batch + "%' and hod.EMPID COLLATE Latin1_General_CI_AS = emp.EMPID and GLEARN_GROUP_CODE COLLATE Latin1_General_CI_AS in (select GLEARN_GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN where SUBJECT_CODE='" + subject_code + "' " +
                        "and regdno= '" + regdno + "' and campus_code= '" + campus + "' and SEMESTER= '" + sem + "' and BATCH like '%" + batch + "%')";

                }

                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<studenttrack>> getfeedback_facultyasyncguest(string groupcode, string campus, string branch_code, string regdno, string subject_code, string sem, string college_code, string batch)
        {
            try
            {
                var query = "select top 1 c.faculty_id as empid,emp.Emp_name,c.subject_code,c.dept_code,c.GROUP_CODE from CBCS_STUDENT_SUBJECT_ASSIGN c, GLEARN_GUEST_STAFF_MASTER_new emp where c.regdno = '" + regdno + "' and c.subject_code = '" + subject_code + "' and c.faculty_id not in ('N', 'No faculty') and LTRIM(RTRIM(c.FACULTY_ID))= emp.EMPID";
                return await Task.FromResult(GetAllData42<studenttrack>(query, null));
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<FeedbackMaster>> getquestionlist()
        {
            try
            {
                var query = @"select NATURE,Q_STATUS,FEEDBACK1,FEEDBACK2,FEEDBACK3,FEEDBACK4,FEEDBACK5,FEEDBACK1_MARK,FEEDBACK2_MARK,FEEDBACK3_MARK,FEEDBACK4_MARK,FEEDBACK5_MARK,ID,maxmarks from gitam_feedback_new_master   order by ID";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<FeedbackMaster>> getfinalyearquestionlist()
        {
            try
            {
                var query = @"select NATURE, Q_STATUS, FEEDBACK1, FEEDBACK2, FEEDBACK3, FEEDBACK4, FEEDBACK5, FEEDBACK1_MARK, FEEDBACK2_MARK, FEEDBACK3_MARK, FEEDBACK4_MARK, FEEDBACK5_MARK, ID, maxmarks from gitam_feedback_outgoing_master   order by ID";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<FeedbackMaster>> getfeedback_check(string subjectname, string faculty, string feedbacksession, string REGDNO, string sem)
        {
            try
            {
                var query = @"select feedback_Status from  feedback_student where regdno='" + REGDNO + "' and subject='" + subjectname + "'  and semester ='" + sem + "' and feedback_status='y' and feedback_session='" + feedbacksession + "' and empid='" + faculty + "'";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<FeedbackMaster>> getfeedbackfinalyear_check(string REGDNO, string sem)
        {
            try
            {
                var query = @"select feedback_Status from  GITAM_STUDENT_OUTGOING where regdno='" + REGDNO + "'  and semester ='" + sem + "' and feedback_status='y' ";
                return await Task.FromResult(GetAllData60feedback<FeedbackMaster>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<FeedbackMaster>> Insertfeedbacknew(List<FeedbackMaster> chart)
        {
            try
            {
                int j = 0;
                string updqry = null;
                string query = "";
                int k = 0;
                bool retnval = false;
                int maxmarkstotal = 0;
                int count1 = chart.Count - 1;
                for (int p = 0; p < count1; p++)
                {
                    maxmarkstotal = maxmarkstotal + Convert.ToInt32(chart[p].maxmarks);
                }
                for (int i = 0; i < chart.Count; i++)
                {

                    if (chart[i].Q_STATUS.Equals("O"))
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        updqry = "insert into GITAM_STUDENT_FEEDBACK(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME, FEEDBACK_TITLE,";
                        updqry = updqry + "FEEDBACK_MARK,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,FEEDBACK_ID,section,batch,campus_code,branch_code,COMMENTS,semester,FEEDBACK_SESSION)";
                        updqry = updqry + " values('" + chart[i].group_code + "','" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].subject_code + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].empid + "','" + chart[i].empname + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','" + chart[i].maxmarks + "','" + chart[i].subjectname + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','','" + chart[i].semester + "','" + chart[i].feedbacksession + "')";
                    }
                    else
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        chart[i].FEEDBACK1 = chart[i].FEEDBACK1.Replace("'", "''");

                        updqry = "insert into GITAM_STUDENT_FEEDBACK(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME, FEEDBACK_TITLE,";
                        updqry = updqry + "COMMENTS,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,FEEDBACK_ID,section,batch,campus_code,branch_code,semester,FEEDBACK_SESSION)";

                        updqry = updqry + " values('" + chart[i].group_code + "','" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].subject_code + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].empid + "','" + chart[i].empname + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','" + chart[i].maxmarks + "','" + chart[i].subjectname + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','" + chart[i].semester + "','" + chart[i].feedbacksession + "')";


                    }
                    k = await Task.FromResult(InsertData60feedback(updqry, null));
                }

                if (k > 0)
                {
                    query = "insert into feedback_student(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME,";
                    query = query + "FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,section,campus_code,branch_code,semester,FEEDBACK_SESSION,feedback_status)";

                    query = query + " values('" + chart[0].group_code + "','" + chart[0].regdno + "','" + chart[0].stuname + "','" + chart[0].subject_code + "','" + chart[0].course_code + "','" + chart[0].dept_code + "','" + chart[0].empid + "','" + chart[0].empname + "',getdate(),'" + chart[0].college_code + "','" + maxmarkstotal + "','" + chart[0].subjectname + "','" + chart[0].section + "','" + chart[0].campus_code + "','" + chart[0].branch_code + "','" + chart[0].semester + "','" + chart[0].feedbacksession + "','Y')";
                    j = await Task.FromResult(InsertData60feedback(query, null));
                    chart[0].flag = "success";
                }
                return chart;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<IEnumerable<FeedbackMaster>> InsertGITAM_STUDENT_OUTGOINGfeedbacknew(List<FeedbackMaster> chart)
        {
            try
            {
                int j = 0;
                string updqry = null;
                string query = "";
                int k = 0;
                bool retnval = false;
                int maxmarkstotal = 0;
                int count1 = chart.Count - 1;
                for (int p = 0; p < count1; p++)
                {
                    maxmarkstotal = maxmarkstotal + Convert.ToInt32(chart[p].maxmarks);
                }
                for (int i = 0; i < chart.Count; i++)
                {

                    if (chart[i].Q_STATUS.Equals("O"))
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        updqry = "insert into GITAM_STUDENT_OUTGOING(REGDNO,STUDENTNAME,COURSE_CODE,DEPT_CODE, FEEDBACK_TITLE,";
                        updqry = updqry + "FEEDBACK_MARK,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,FEEDBACK_ID,section,batch,campus_code,branch_code,COMMENTS,semester,FEEDBACK_SESSION,feedback_status)";
                        updqry = updqry + " values('" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','" + chart[i].maxmarks + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','','" + chart[i].semester + "','" + chart[i].feedbacksession + "','y')";
                    }
                    else
                    {
                        chart[i].ID = chart[i].ID.Replace("'", "''");
                        chart[i].FEEDBACK1 = chart[i].FEEDBACK1.Replace("'", "''");

                        updqry = "insert into GITAM_STUDENT_OUTGOING(REGDNO,STUDENTNAME,COURSE_CODE,DEPT_CODE, FEEDBACK_TITLE,";
                        updqry = updqry + "COMMENTS,FEEDBACK_DATE,COLLEGE_CODE,maxmarks,FEEDBACK_ID,section,batch,campus_code,branch_code,semester,FEEDBACK_SESSION,feedback_status)";

                        updqry = updqry + " values('" + chart[i].regdno + "','" + chart[i].stuname + "','" + chart[i].course_code + "','" + chart[i].dept_code + "','" + chart[i].ID + "','" + chart[i].FEEDBACK1 + "',getdate(),'" + chart[i].college_code + "','" + chart[i].maxmarks + "','" + chart[i].NATURE + "','" + chart[i].section + "','" + chart[i].batch + "','" + chart[i].campus_code + "','" + chart[i].branch_code + "','" + chart[i].semester + "','" + chart[i].feedbacksession + "','y')";


                    }
                    k = await Task.FromResult(InsertData60feedback(updqry, null));
                    chart[0].flag = "success";
                }

                //if (k > 0)
                //{
                //    query = "insert into feedback_student(Group_code,REGDNO,STUDENTNAME,SUBJECT,COURSE_CODE,DEPT_CODE,EMPID,EMPNAME,";
                //    query = query + "FEEDBACK_DATE,COLLEGE_CODE,maxmarks,SUBJECT_NAME,section,campus_code,branch_code,semester,FEEDBACK_SESSION,feedback_status)";

                //    query = query + " values('" + chart[0].group_code + "','" + chart[0].regdno + "','" + chart[0].stuname + "','" + chart[0].subject_code + "','" + chart[0].course_code + "','" + chart[0].dept_code + "','" + chart[0].empid + "','" + chart[0].empname + "',getdate(),'" + chart[0].college_code + "','" + maxmarkstotal + "','" + chart[0].subjectname + "','" + chart[0].section + "','" + chart[0].campus_code + "','" + chart[0].branch_code + "','" + chart[0].semester + "','" + chart[0].feedbacksession + "','Y')";
                //    j = await Task.FromResult(InsertData60feedback(query, null));
                //    chart[0].flag = "success";
                //}
                return chart;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<IEnumerable<Students>> getFacultyDetailsTimetable_main(string reg_no, string SEMESTER, string BRANCH_CODE, string course_code, string college_code, string degreecode, string section)
        {
            var query = "";
            try
            {

                /*  if (BRANCH_CODE.ToUpper() == "FSTCSE2023" || BRANCH_CODE.ToUpper() == "FSTNONCSE2023")
                  {

                      query = @" select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,EMPID,EMPNAME
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION  AND t.GROUP_CODE=h.GROUP_CODE
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION  and h.COURSE_CODE = t.COURSE_CODE 
  where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";

                  }
                  else
                  {
                       query = @" select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,EMPID,EMPNAME
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE  
   and h.COURSE_CODE = t.COURSE_CODE where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";

                  }
                  return await Task.FromResult(GetAllData42<Students>(query, null));
              }*/
                if (degreecode == "UG")
                {


                    query = @"select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,
h.SUBJECT_TYPE,CBCS_CATEGORY,EMPID,EMPNAME,CAST(t.FROMTIME AS varchar) as FROMTIME,CAST(t.TOTIME AS varchar) as TOTIME ,WEEKDAY
 from TIME_TABLE_MASTER t
 left join CBCS_STUDENT_SUBJECT_ASSIGN h  on(h.GROUP_CODE = t.GROUP_CODE  or h.GLEARN_GROUP_CODE = t.GROUP_CODE)
 and h.SEMESTER = t.SEMESTER  and h.SECTION=t.SECTION  where h.REGDNO = '" + reg_no + "' and h.SEMESTER = '" + SEMESTER + "' ";

                    if (college_code == "GSL")
                    {
                        query = @" select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,EMPID,EMPNAME,CAST(t.FROMTIME AS varchar) as FROMTIME,CAST(t.TOTIME AS varchar) as TOTIME ,WEEKDAY
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE  
   and h.COURSE_CODE = t.COURSE_CODE where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";

                    }
                }
                else
                {

                    if (degreecode == "PG")
                    {
                        query = @"select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,Faculty_id as EMPID,emp.emp_name as EMPNAME,CAST(t.FROMTIME AS varchar) as FROMTIME,CAST(t.TOTIME AS varchar) as TOTIME ,WEEKDAY
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h 
   left join employee_master emp on emp.EMPID=h.FACULTY_ID
   on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION 
   and h.COURSE_CODE = t.COURSE_CODE and h.BRANCH_CODE=t.BRANCH_CODE where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";

                    }
                    else if (BRANCH_CODE == "MBA")
                    {
                        query = @"select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,Faculty_id as EMPID,emp.emp_name as EMPNAME,CAST(t.FROMTIME AS varchar) as FROMTIME,CAST(t.TOTIME AS varchar) as TOTIME ,WEEKDAY
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h 
   left join employee_master emp on emp.EMPID=h.FACULTY_ID
   on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION 
   and h.COURSE_CODE = t.COURSE_CODE where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";
                    }
                    else
                    {
                        query = @" select distinct h.SUBJECT_CODE,h.SUBJECT_NAME,t.buildingname as building_name,t.ROOM_NO,NEW_CREDITS as CREDITS,h.SUBJECT_TYPE,CBCS_CATEGORY,EMPID,EMPNAME,CAST(t.FROMTIME AS varchar) as FROMTIME,CAST(t.TOTIME AS varchar) as TOTIME ,WEEKDAY
   from TIME_TABLE_MASTER t
   left join CBCS_STUDENT_SUBJECT_ASSIGN h on h.CAMPUS_CODE COLLATE Latin1_General_CI_AS = t.CAMPUS_CODE and
   t.SUBJECT_CODE COLLATE Latin1_General_CI_AS = h.SUBJECT_CODE and h.BATCH = t.BATCH and               
   t.COLLEGE_CODE COLLATE Latin1_General_CI_AS = h.COLLEGE_CODE 
   and h.SEMESTER = t.SEMESTER and t.SECTION COLLATE Latin1_General_CI_AS = h.SECTION and h.BRANCH_CODE = t.BRANCH_CODE  
   and h.COURSE_CODE = t.COURSE_CODE where  h.REGDNO = '" + reg_no + "' and h.SEMESTER='" + SEMESTER + "'";
                    }
                }
                return await Task.FromResult(GetAllData42<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Students>> eventsdecider(string regdno)
        {

            var query = @"select * from users where status='1' and role='User' and flag='student' and user_id ='" + regdno + "'";
            return await Task.FromResult(GetAllData73<Students>(query, null));

        }



        public async Task<StudentPhoto> getStudentapplication(string regno)
        {
            StudentPhoto pm = new StudentPhoto();
            var query = "";
            try
            {

                query = "SELECT [REGDNO],[NAME],[CAMPUS],[COURSE],[BRANCH],[MONTH],[YEAR],[ADDRESS],[LANDLINE],[MOBILE],[EMAIL],[CHALLANNO],[AMOUNT_PAID],[BANK_NAME],[CHALLAN_DATE],[STATUS],[AADHAR],[NADNO],[PINNO],[ADDRESS] FROM APPLICATION_MASTER where REGDNO = '" + regno + "'";


                pm = await Task.FromResult(GetSingleData52hall<StudentPhoto>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }


        public async Task<StudentPhoto> UpdateStudentapplication(StudentPhoto chart)
        {
            try
            {
                int j = 0;
                string Query = "";

                Query = @"update [dbo].[APPLICATION_MASTER] set
                [REGDNO] = '" + chart.REGDNO + "',[NAME]= '" + chart.NAME + "',[CAMPUS]= '" + chart.CAMPUS + "',[COURSE]= '" + chart.COURSE + "',[BRANCH]= '" + chart.BRANCH + "'," +
                "[MONTH]= '" + chart.MONTH + "',[YEAR]= '" + chart.YEAR + "',[MOBILE]= '" + chart.MOBILE + "',[EMAIL]= '" + chart.EMAIL + "',[AADHAR]= '" + chart.AADHAR + "',[Address]= '" + chart.ADDRESS + "',[PINNO]= '" + chart.PINNO + "'," +
                "[NADNO]= '" + chart.NADNO + "',[Approval_FLAG]= '" + chart.Approval_FLAG + "',[Approval_USER_ID]='" + chart.Approval_USER_ID + "' where REGDNO = '" + chart.REGDNO + "'";

                j = await Task.FromResult(Update52hall(Query, null));

                if (j == 1)
                {
                    chart.msg = "1";
                }
                else
                {
                    chart.msg = "0";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;
        }



        public async Task<StudentPhoto> InsertStudentapplication(StudentPhoto chart)
        {
            try
            {
                int j = 0;
                string Query = "";

                Query = @"INSERT INTO [dbo].[APPLICATION_MASTER]
                  ([REGDNO],[NAME],[CAMPUS],[COURSE],[BRANCH],[MONTH],[YEAR],[MOBILE],[EMAIL],[AADHAR],[address],[pinno],
[NADNO],DT_TIME,STU_BATCH)
            VALUES('" + chart.REGDNO + "','" + chart.NAME + "','" + chart.CAMPUS + "','" + chart.COURSE + "','" + chart.BRANCH + "'," +
            "'" + chart.MONTH + "','" + chart.YEAR + "','" + chart.MOBILE + "','" + chart.EMAIL + "','" + chart.AADHAR + "'," +
            "'" + chart.ADDRESS + "','" + chart.PINNO + "','" + chart.NADNO + "',getdate(),'" + chart.batch + "')";

                j = await Task.FromResult(InsertData52hall(Query, null));

                if (j == 1)
                {
                    chart.msg = "1";
                }
                else
                {
                    chart.msg = "0";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return chart;


        }



        public async Task<StudentPhoto> getStudentmasterphoto(string regno)
        {
            StudentPhoto pm = new StudentPhoto();
            var query = "";
            try
            {

                query = @"SELECT ISNULL(AADHAR_NO,'') AS NADNO,ISNULL(PINCODE,'') AS PINNO,ISNULL(PERMANENT_ADDRESS,'') AS ADDRESS,[REGDNO],[NAME],[CURR_SEM],[MOBILE],ISNULL([EMAILID],'') AS EMAIL,[CAMPUS]
     ,Course_Code as COURSE,BRANCH_CODE as BRANCH,Batch,Section FROM STUDENT_MASTER where REGDNO = '" + regno + "' and status = 's' and(BATCH like '%-2024' or BATCH like '%-2022' or BATCH like '%20-2025')";


                pm = await Task.FromResult(GetSingleData<StudentPhoto>(query, null));
            }
            catch (Exception e) { }
            return pm;

        }


        //public async Task<LMSsubject> getassignmentmarksdisplay(string sub_code, string group_code, string assignid, string regdno)
        //{
        //    try
        //    {

        //        string query = @"select Marks as final_marks ,ASSIG_FLAG from CE_MARKS_MASTER where regdno='" + regdno + "' and GROUP_CODE='" + group_code + "'  and ASSIG_FLAG='" + assignid + "'";
        //        var dis = await Task.FromResult(GetSingleData102<LMSsubject>(query, null));
        //        if (dis != null)
        //        {
        //            return dis;

        //        }
        //        else
        //        {
        //            return null;

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        public async Task<LMSsubject> getassignmentmarksdisplay(string sub_code, string group_code, string assignid, string regdno)
        {
            try
            {

                string query = @" select Marks as final_marks ,assignmentid as ASSIG_FLAG from STUDENT_ASSIGNMENTS where regdno='" + regdno + "' and groupcode='" + group_code + "'  and assignmentid='" + assignid + "'";
                var dis = await Task.FromResult(GetSingleData102<LMSsubject>(query, null));
                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<LMSsubject>> getassignmentmarksfinaldisplay(string sub_code, string group_code, string regdno)
        {

            try
            {

                string query = @"select Marks as final_marks ,assignmentid as ASSIG_FLAG,section,Type,groupcode from STUDENT_ASSIGNMENTS where regdno='" + regdno + "' and groupcode='" + group_code + "' ";
                var dis = await Task.FromResult(GetAllData102<LMSsubject>(query, null));
                if (dis != null)
                {
                    return dis;

                }
                else
                {

                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<Students> getgroup_code(string subjectcode, string faculty, string feedbacksession, string REGDNO, string sem)
        {
            try
            {

                string query = @"select top 1 group_code  from CBCS_STUDENT_SUBJECT_ASSIGN where SUBJECT_CODE='" + subjectcode + "' and regdno='" + REGDNO + "'  and SEMESTER in ('92','" + sem + "')";
                var dis = await Task.FromResult(GetSingleData42<Students>(query, null));

                if (dis != null)
                {
                    return dis;

                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async Task<FeedbackMaster> get_facultydept(string fid)
        {
            try
            {

                string query = @"select top 1 dept_code  from employee_master where empid='" + fid + "' ";
                var dis = await Task.FromResult(GetAllData19new<FeedbackMaster>(query, null));


                if (dis.Count() == 0) {

                     query = @"select top 1 dept_code  from GLEARN_GUEST_STAFF_MASTER_new where empid='" + fid + "' ";
                     dis = await Task.FromResult(GetAllData42<FeedbackMaster>(query, null));


                    
                }
                if (dis != null)
                {
                    return dis.FirstOrDefault();

                }
                else
                {
                    return null;

                }


            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public async Task<IEnumerable<Students>> getexamtimetable(string regdno)
        {

            var query = @"select campus,student_type,institute,program,branch,course_code,course_name,exam_date,timings,degree_type,isnull(room_no,'') as roomjson,sem from student_exams_data where redgno='" + regdno + "' and hall_ticket_status='y'";
            return await Task.FromResult(GetAllData74exam<Students>(query, null));

        }
        public async Task<Userlogin> get74user(string regdno)
        {

            var query = @"select isnull(id,'') as id, isnull(password,'') as password from users where empid='"+regdno+"'";
            return await Task.FromResult(GetAllData74BELLTHECATS<Userlogin>(query, null));

        }
        
        public async Task<IEnumerable<Students>> getexamtimetablebydate(string regdno, string date)
        {

            // var query = @" select top 1 name,redgno,campus,student_type,institute,program,branch,course_code,course_name,exam_date,timings,degree_type,isnull(room_no,'') as roomjson,sem from student_exams_data where redgno='" + regdno + "' and exam_date='" + date + "'";
            var query = @" select name,redgno,campus,student_type,institute,program,branch,course_code,course_name,exam_date,timings,degree_type,isnull(room_no,'') as roomjson,sem from student_exams_data where redgno='" + regdno + "' and exam_date='" + date + "'";
            return await Task.FromResult(GetAllData74exam<Students>(query, null));

        }

        public async Task<IEnumerable<Students>> GetDetailsstudent(string userid)
        {
            try
            {
                var query = @"select name,regdno,SUBSTRING((CONVERT(VARCHAR(11), dob, 106)), 0, 12) as dob,BLOOD_GROUP,MOBILE,EMAILID,GENDER,nationality,RELIGION,CATEGORY,DOORNO,LOCATION,CITY,STATE,COUNTRY,PINCODE,FATHER_NAME,MOTHERNAME,CAMPUS,COLLEGE_CODE,DEGREE_CODE,COURSE_CODE,BRANCH_CODE,BATCH,CLASS,SECTION,curr_sem,parent_mobile,GUARDIANNAME,GUARDIAN_CONTACT,PARENT_EMAIL_ID,AADHAR_NO,Fee_reimbursement,Parental_income,CATEGORY,OCCUPATION_PARENT from student_master where regdno='" + userid + "' ";
                return await Task.FromResult(GetAllData<Students>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //public async Task<Mac_details> getmac_details(string regdno)
        //{
        //    var query = "select top 1 * from GITAM_STUDENT_STAFF_MAC_DETAILS where Regdno='" + regdno + "'";
        //    return await Task.FromResult(GetSingleData<Mac_details>(query, null));

        //}
        public async Task<int> InsertMac_details(Mac_details data, string id1, string id2)
        {
            int k = 0;
            try
            {
                var query = "insert into GITAM_STUDENT_STAFF_MAC_DETAILS ([Regdno],[Name],[Gender],[Type],[Campus],[MAC1],[MAC2],[CISCO_STATUS],[ID_MAC1],[ID_MAC2]) values('" + data.Regdno + "','" + data.Name + "','" + data.Gender + "','S','" + data.Campus + "','" + data.MAC1 + "','" + data.MAC2 + "','1','" + id1 + "','" + id2 + "')";
                k = await Task.FromResult(InsertData(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }
        public async Task<int> updateMac_details(Mac_details data)
        {
            int k = 0;
            try
            {
                var query = "select [Regdno],[Name],[Gender],[Type],[Campus],[MAC1] as MAC1_old,[MAC2] as MAC2_old,id from GITAM_STUDENT_STAFF_MAC_DETAILS where Regdno='" + data.Regdno + "'";
                Mac_details list = await Task.FromResult(GetSingleData<Mac_details>(query, null));
                if (list.id != 0)
                {

                    var query2 = "update  GITAM_STUDENT_STAFF_MAC_DETAILS set [MAC1] ='" + data.MAC1 + "',[MAC2]='" + data.MAC2 + "' where id='" + data.id + "' and regdno='" + data.Regdno + "' ";
                    k = await Task.FromResult(Update(query2, null));
                    var query1 = "insert into GITAM_STUDENT_STAFF_MAC_DETAILS_History ([Regdno],[MAC1],[MAC2],dt_date) values('" + list.Regdno + "','" + list.MAC1_old + "','" + list.MAC2_old + "',getdate())";
                    k = await Task.FromResult(InsertData(query1, null));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }

        //public async Task<int> getmacdetails_check(string macid, string Regdno)
        //{
        //    int i = 0;
        //    try
        //    {
        //        var query = "select * from GITAM_STUDENT_STAFF_MAC_DETAILS where (MAC1 ='" + macid + "' or MAC2 = '" + macid + "') and Regdno !='" + Regdno + "' ";
        //        IEnumerable<Mac_details> list = await Task.FromResult(GetAllData<Mac_details>(query, null));
        //        if (list.Count() > 0)
        //        {
        //            i = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return i;
        //}
        public async Task<IEnumerable<Students>> getdailylog(string Regdnos)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"select dt_time,id,regdno, arrival_time, depart_time, org_name, mentor_name,dept_name, learning_actions from student_daily_log 
                 where regdno = ('" + Regdnos + "') order by id desc";

                list = await Task.FromResult(GetAllData<Students>(query, null));

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }
        public async Task<IEnumerable<Students>> SaveDailyLog(string Arrivaltimedata, string departtimedata, string orgnamedata, string dptnamedata, string mtrnamedata, string lernactiondata, string regdno, string semester, string collegeCode, string branchCode, string campusCode)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"Insert into student_daily_log(arrival_time, depart_time, org_name, dept_name, mentor_name, learning_actions, regdno, semester, college_code, branch_code, campus_code,dt_time)
VALUES('" + Arrivaltimedata + "', '" + departtimedata + "', '" + orgnamedata + "', '" + dptnamedata + "', '" + mtrnamedata + "', '" + lernactiondata + "', '" + regdno + "', '" + semester + "', '" + collegeCode + "', '" + branchCode + "' ,'" + campusCode + "',getdate())";

                int j = await Task.FromResult(InsertData19hf(query, null));
                //var query1 = "select * from student_daily_log where Regdno ='" + regdno + "' ";
                //    list = await Task.FromResult(GetAllData<Students>(query1, null));
            }
            catch (Exception e)
            {
                throw e;

            }
            return list;
        }

        public async Task<IEnumerable<Students>> Insertweeklyactivity(string cmpnamedata, string cmpmentordata, string stfguidedata, string actperformeddata, string keyprfindicatordata, string challengesdata, string achievementsdata, string learningpointsdata, string regdno, string semester, string collegeCode, string branchCode, string campusCode)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"Insert into student_weekly_report(company_name, mentor_name, staff_guide, activities_performed, key_per_indicators, challenges, achievements, learning_points, regdno, semester, college_code, branch_code, campus_code, dt_time)
VALUES('" + cmpnamedata + "', '" + cmpmentordata + "', '" + stfguidedata + "', '" + actperformeddata + "', '" + keyprfindicatordata + "', '" + challengesdata + "', '" + achievementsdata + "', '" + learningpointsdata + "', '" + regdno + "', '" + semester + "', '" + collegeCode + "', '" + branchCode + "' ,'" + campusCode + "',getdate())";

                int j = await Task.FromResult(InsertData19hf(query, null));
                //var query1 = "select * from student_daily_log where Regdno ='" + regdno + "' ";
                //    list = await Task.FromResult(GetAllData<Students>(query1, null));
            }
            catch (Exception e)
            {
                throw e;

            }
            return list;
        }

        public async Task<IEnumerable<Students>> getweeklyactivities(string Regdnos)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"select dt_time, id, regdno, company_name, mentor_name, staff_guide, activities_performed, key_per_indicators, challenges, achievements, learning_points from student_weekly_report
                 where regdno = ('" + Regdnos + "') order by id desc";

                list = await Task.FromResult(GetAllData<Students>(query, null));

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }
        public async Task<IEnumerable<Students>> Insertfieldworkproposal(string fwtitledata, string rprtdatedata, string rwdomaindata, string rwksubdomainddata, string rpmbackgrounddata, string prbdefinitiondata, string stdmethoddata, string expoutcomedata, string regdno, string semester, string collegeCode, string branchCode, string campusCode)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"Insert into student_field_work(field_work_title, report_date, domain_research_work, sub_domain_research_work, problem_gap, study_methods, expected_outcomes, regdno, semester, college_code, branch_code, campus_code, dt_time, rpm_background)
VALUES('" + fwtitledata + "', '" + rprtdatedata + "', '" + rwdomaindata + "', '" + rwksubdomainddata + "',  '" + prbdefinitiondata + "', '" + stdmethoddata + "', '" + expoutcomedata + "', '" + regdno + "', '" + semester + "', '" + collegeCode + "', '" + branchCode + "' ,'" + campusCode + "', getdate(), '" + rpmbackgrounddata + "')";

                int j = await Task.FromResult(InsertData19hf(query, null));
                //var query1 = "select * from student_daily_log where Regdno ='" + regdno + "' ";
                //    list = await Task.FromResult(GetAllData<Students>(query1, null));
            }
            catch (Exception e)
            {
                throw e;

            }
            return list;
        }
        public async Task<IEnumerable<Students>> getfieldworkproposal(string Regdnos)
        {
            IEnumerable<Students> list = null;
            try
            {
                string query = @"select dt_time,id,regdno, field_work_title, report_date, domain_research_work, sub_domain_research_work, problem_gap, study_methods, expected_outcomes from student_field_work
                 where regdno = ('" + Regdnos + "') order by id desc";

                list = await Task.FromResult(GetAllData<Students>(query, null));

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }

        public async Task<IEnumerable<Attendance>> Getattendance_semsterissummerterm(string user_id, string table_name, string college_code, string batchattd)
        {
            var query = "";
            try
            {



                query = query + "select distinct Regdno, SubjectCode as subjectcode,subjectname, (cast(round((((select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and Status = 'P' and SubjectCode = s.SubjectCode  and Semester = '92'   )*100.0)/";
                query = query + " (select count(*) from " + table_name + " where CollegeCode = '" + college_code + "' and Regdno = s.Regdno and SubjectCode = s.SubjectCode  and Semester = '92' )) ,2 ) as decimal(5,2))) as percentage";
                query = query + " from " + table_name + " s  where Regdno = '" + user_id + "' and CollegeCode = '" + college_code + "'  and Semester = '92'   group by regdno,SubjectCode,subjectname,status";



                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Attendance>> Getattendance_semster_byselectionissummerterm(string user_id, string table_name, string college_code, string batchattd)
        {
            var query = "";

            try
            {




                query = query + "SELECT regdno, format(Attdate,'yyyy-MM-dd') +' ' +SUBSTRING(CONVERT(varchar, Fromtime, 108), 1, 5) AS [sessiondate], ";
                query = query + "SUBSTRING(CONVERT(varchar, totime, 108), 1, 5) AS [To_time], SubjectName AS [subjectname], ";
                query = query + "PostedBy AS [Faculty_ID], postedname AS [facultyname], CASE Status WHEN 'P' THEN 'Present' ELSE 'Absent' END AS STATUS ";
                query = query + "FROM " + table_name + " ";
                query = query + "WHERE regdno = '" + user_id + "' ";
                query = query + "AND CollegeCode = '" + college_code + "' ";
                query = query + "AND Semester = '92' ";



                return await Task.FromResult(GetAllData102<Attendance>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<IEnumerable<Softwareproject>> Getsoftwareprojectslist_async(string campus)
        //{
        //    try
        //    {
        //        var query = "select * from CAPSTONE_PROJECT_LISTS   where campus='" + campus + "' and confirm_pending_status ='Y' ";
        //        return await Task.FromResult(GetAllData42<Softwareproject>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        //public async Task<IEnumerable<Softwareproject>> Getselectedsoftwareprojectslist_async(string regdno)
        //{
        //    try
        //    {
        //        var query = "WITH ProjectDetails AS (SELECT  cp.Project_Title,  cp.Project_Description,  cp.expected_outcomes,   cp.Project_domain, cp.id AS title_id, cp.unique_id, cp.Tools_Technologies,cp.EMP_NAME,cs.STATUS  FROM CAPSTONE_PROJECT_STUDENT_LIST cs LEFT JOIN CAPSTONE_PROJECT_LISTS cp  ON cp.unique_id = cs.unique_id   AND cp.Project_domain = cs.Project_domain  AND cp.id = cs.title_id  WHERE cs.regdno = '" + regdno + "') SELECT    pd.Project_Title, pd.Project_Description, pd.expected_outcomes,  pd.Project_domain, pd.title_id,  pd.unique_id,  pd.Tools_Technologies,pd.EMP_NAME,pd.STATUS,isnull(STUFF(( SELECT ', ' + cpsl2.NAME   FROM CAPSTONE_PROJECT_STUDENT_LIST cpsl2 WHERE cpsl2.title_id = pd.title_id   AND cpsl2.regdno != '" + regdno + "'    FOR XML PATH(''), TYPE   ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS team FROM ProjectDetails pd GROUP BY    pd.Project_Title,  pd.Project_Description, pd.expected_outcomes, pd.Project_domain, pd.title_id, pd.unique_id, pd.Tools_Technologies,pd.EMP_NAME,pd.STATUS;";
        //        return await Task.FromResult(GetAllData42<Softwareproject>(query, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<IEnumerable<Softwareproject>> Getprojectdomain(string campus)
        //{
        //    IEnumerable<Softwareproject> list = null;
        //    try
        //    {
        //        var query = "select distinct DOMAIN from CAPSTONE_PROJECT_DOMAINS   where campus='" + campus + "' and STATUS='A'";
        //        list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));

        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }                                                                                                                                                                                                                      
        //    return list;
        //}

        //public async Task<IEnumerable<Softwareproject>> Getprojectslist(string campus, string selectedValuesArray)
        //{
        //    IEnumerable<Softwareproject> list = null;
        //    try
        //    {
        //        var query = "";
        //        var selectedValues = JsonConvert.DeserializeObject<string[]>(selectedValuesArray);

        //        var whereClause = "";
        //        query = $"SELECT DISTINCT Project_title FROM CAPSTONE_PROJECT_LISTS WHERE campus='" + campus + "' and selected_count < 4 and confirm_pending_status ='Y' and project_domain like (";

        //        if (selectedValues != null)
        //        {

        //            for (int i = 0; i < selectedValues.Length; i++)
        //            {
        //                if (i != 0)
        //                {
        //                    query += " union SELECT DISTINCT Project_title FROM CAPSTONE_PROJECT_LISTS WHERE campus='" + campus + "' and selected_count < 4 and confirm_pending_status ='Y' and project_domain like ( ";
        //                }
        //                whereClause = selectedValues.ElementAt(i);
        //                query += "'%" + whereClause.Trim() + "%')";
        //            }

        //        }
        //        list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return list;
        //}

        //public async Task<int> insertprojectselection(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray)
        //{
        //    int j = 0;
        //    int s = 0;
        //    var query = "";
        //    var selectedValues = selectedValuesArray.Split(',').ToArray();
        //    IEnumerable<Softwareproject> list = null;

        //    var whereproject = "";
        //    try
        //    {
        //        if (selectedValues != null)
        //        {
        //            IEnumerable<Softwareproject> list2 = null;

        //            for (int i = 0; i < selectedValues.Length; i++)
        //            {
        //                whereproject = selectedValues.ElementAt(i);
        //                var maxsquery = "select count(id) as total_project_count from CAPSTONE_PROJECT_STUDENT_LIST where regdno='" + Regdno + "'";
        //                List<Softwareproject> data2 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery, null));
        //                if (Convert.ToInt32(data2.FirstOrDefault().total_project_count) < 4)
        //                {

        //                    var maxsquery1 = "select distinct selected_count,unique_id,project_domain,id as ID,empid as EMPID,project_title as Project_Title from CAPSTONE_PROJECT_LISTS where campus='" + campus + "' and project_title like '%" + whereproject + "%' and confirm_pending_status ='Y'";
        //                    List<Softwareproject> data1 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery1, null));

        //                    if (Convert.ToInt32(data1.FirstOrDefault().selected_count) < 4)
        //                    {
        //                        var maxsquery2 = "select distinct title_id from CAPSTONE_PROJECT_STUDENT_LIST where regdno='" + Regdno + "' and unique_id='" + data1.FirstOrDefault().unique_id + "' and title_id='" + data1.FirstOrDefault().ID + "'";
        //                        List<Softwareproject> data3 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery2, null));
        //                        if (data3.Count() == 0)
        //                        {
        //                            int finalmaxs = Convert.ToInt32(data1.FirstOrDefault().selected_count) + 1;

        //                            var query_annual = @"insert into CAPSTONE_PROJECT_STUDENT_LIST (RegdNo,Name,MOBILe,EMAILID,CAMPUS,COLLEGE,DEPARTEMNT,STATUS,DT_TIME,project_domain,unique_id,title_id,project_title,user_id,selected_count) values 
        //                            ('" + Regdno + "','" + stuname + "','" + mobile + "','','" + campus + "','" + college + "','" + dept + "','N',getdate(),'" + data1.FirstOrDefault().Project_domain + "','" + data1.FirstOrDefault().unique_id + "','" + data1.FirstOrDefault().ID + "','" + data1.FirstOrDefault().Project_Title + "','" + data1.FirstOrDefault().EMPID + "','" + finalmaxs + "')";
        //                            j = await Task.FromResult(InsertData42(query_annual, null));
        //                            if (j > 0)
        //                            {

        //                                var updatequery = "update CAPSTONE_PROJECT_LISTS set selected_count=selected_count +1 where Project_domain='" + data1.FirstOrDefault().Project_domain + "' and unique_id='" + data1.FirstOrDefault().unique_id + "'  and id='" + data1.FirstOrDefault().ID + "'";
        //                                s = await Task.FromResult(Update42(updatequery, null));
        //                                j = 3;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            j = 2;
        //                        }


        //                    }
        //                    else
        //                    {
        //                        j = 1;
        //                    }
        //                }
        //                else
        //                {
        //                    j = 0;
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return j;
        //}
        //public async Task<IEnumerable<Softwareproject>> selectedprojects(string regdno)
        //{
        //    IEnumerable<Softwareproject> list = null;
        //    try
        //    {
        //        var query = "select count(ID) count from CAPSTONE_PROJECT_STUDENT_LIST   where regdno='" + regdno + "'";
        //        list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));

        //    }
        //    catch (Exception e)
        //    {

        //        throw e;
        //    }
        //    return list;
        //}
        public async Task<IEnumerable<Softwareproject>> Getsoftwareprojectslist_async(string campus)
        {
            try
            {
                var query = "select * from CAPSTONE_PROJECT_LISTS   where campus='" + campus + "'";
                return await Task.FromResult(GetAllData42<Softwareproject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Softwareproject>> Getselectedsoftwareprojectslist_async(string regdno)
        {
            try
            {
                //ON cp.unique_id = cs.unique_id AND
                var query = "WITH ProjectDetails AS (SELECT  cp.Project_Title,  cp.Project_Description,  cp.expected_outcomes,   cp.Project_domain, cp.id AS title_id, cp.unique_id, cp.Tools_Technologies,cp.EMP_NAME,cs.STATUS ,cp.EMPID,cp.SELECTED_COUNT FROM CAPSTONE_PROJECT_STUDENT_LIST cs LEFT JOIN CAPSTONE_PROJECT_LISTS cp  ON  cp.Project_domain = cs.Project_domain  AND cp.id = cs.title_id  WHERE cs.regdno = '" + regdno + "') SELECT    pd.Project_Title, pd.Project_Description, pd.expected_outcomes,  pd.Project_domain, pd.title_id,  pd.unique_id,  pd.Tools_Technologies,pd.EMP_NAME,pd.STATUS,pd.EMPID,pd.SELECTED_COUNT,isnull(STUFF(( SELECT ', ' + cpsl2.NAME   FROM CAPSTONE_PROJECT_STUDENT_LIST cpsl2 WHERE cpsl2.title_id = pd.title_id   AND cpsl2.regdno != '" + regdno + "'    FOR XML PATH(''), TYPE   ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''),'') AS team FROM ProjectDetails pd GROUP BY    pd.Project_Title,  pd.Project_Description, pd.expected_outcomes, pd.Project_domain, pd.title_id, pd.unique_id, pd.Tools_Technologies,pd.EMP_NAME,pd.STATUS,pd.EMPID,pd.SELECTED_COUNT;";
                return await Task.FromResult(GetAllData42<Softwareproject>(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Softwareproject>> Getprojectdomain(string campus)
        {
            IEnumerable<Softwareproject> list = null;
            try
            {
                var query = "select distinct DOMAIN from CAPSTONE_PROJECT_DOMAINS   where campus='" + campus + "' and STATUS='A'";
                list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }

        public async Task<IEnumerable<Softwareproject>> Getprojectslist(string campus, string selectedValuesArray)
        {
            IEnumerable<Softwareproject> list = null;
            try
            {
                var query = "";
                var selectedValues = JsonConvert.DeserializeObject<string[]>(selectedValuesArray);

                var whereClause = "";
                query = $"SELECT DISTINCT Project_title FROM CAPSTONE_PROJECT_LISTS WHERE campus='" + campus + "' and selected_count < 6 and confirm_pending_status ='Y' and project_domain like (";

                if (selectedValues != null)
                {

                    for (int i = 0; i < selectedValues.Length; i++)
                    {
                        if (i != 0)
                        {
                            query += " union SELECT DISTINCT Project_title FROM CAPSTONE_PROJECT_LISTS WHERE campus='" + campus + "' and selected_count < 6 and confirm_pending_status ='Y' and project_domain like ( ";
                        }
                        whereClause = selectedValues.ElementAt(i);
                        query += "'%" + whereClause.Trim() + "%')";
                    }

                }
                list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));
            }
            catch (Exception e)
            {
                throw e;
            }
            return list;
        }

        public async Task<int> insertprojectselection(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray)
        {
            int j = 0;
            int s = 0;
            var query = "";
            var selectedValues = selectedValuesArray.Split(',').ToArray();
            IEnumerable<Softwareproject> list = null;

            var whereproject = "";
            try
            {
                if (selectedValues != null)
                {
                    IEnumerable<Softwareproject> list2 = null;

                    for (int i = 0; i < selectedValues.Length; i++)
                    {
                        whereproject = selectedValues.ElementAt(i);
                        var maxsquery = "select count(id) as total_project_count from CAPSTONE_PROJECT_STUDENT_LIST where regdno='" + Regdno + "'";
                        List<Softwareproject> data2 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery, null));
                        if (Convert.ToInt32(data2.FirstOrDefault().total_project_count) < 4)
                        {

                            var maxsquery1 = "select distinct selected_count,unique_id,project_domain,id as ID,empid as EMPID,project_title as Project_Title from CAPSTONE_PROJECT_LISTS where campus='" + campus + "' and project_title like '%" + whereproject + "%' and confirm_pending_status ='Y'";
                            List<Softwareproject> data1 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery1, null));

                            if (Convert.ToInt32(data1.FirstOrDefault().selected_count) < 6)
                            {
                                var maxsquery2 = "select distinct title_id from CAPSTONE_PROJECT_STUDENT_LIST where regdno='" + Regdno + "' and unique_id='" + data1.FirstOrDefault().unique_id + "' and title_id='" + data1.FirstOrDefault().ID + "'";
                                List<Softwareproject> data3 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery2, null));
                                if (data3.Count() == 0)
                                {
                                    int finalmaxs = Convert.ToInt32(data1.FirstOrDefault().selected_count) + 1;

                                    var query_annual = @"insert into CAPSTONE_PROJECT_STUDENT_LIST (RegdNo,Name,MOBILe,EMAILID,CAMPUS,COLLEGE,DEPARTEMNT,STATUS,DT_TIME,project_domain,unique_id,title_id,project_title,user_id,selected_count) values 
                                    ('" + Regdno + "','" + stuname + "','" + mobile + "','','" + campus + "','" + college + "','" + dept + "','N',getdate(),'" + data1.FirstOrDefault().Project_domain + "','" + data1.FirstOrDefault().unique_id + "','" + data1.FirstOrDefault().ID + "','" + data1.FirstOrDefault().Project_Title + "','" + data1.FirstOrDefault().EMPID + "','" + finalmaxs + "')";
                                    j = await Task.FromResult(InsertData42(query_annual, null));
                                    if (j > 0)
                                    {

                                        var updatequery = "update CAPSTONE_PROJECT_LISTS set selected_count=selected_count +1 where Project_domain='" + data1.FirstOrDefault().Project_domain + "' and unique_id='" + data1.FirstOrDefault().unique_id + "'  and id='" + data1.FirstOrDefault().ID + "'";
                                        s = await Task.FromResult(Update42(updatequery, null));
                                        j = 3;
                                    }
                                }
                                else
                                {
                                    j = 2;
                                }


                            }
                            else
                            {
                                j = 1;
                            }
                        }
                        else
                        {
                            j = 0;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return j;
        }
        public async Task<IEnumerable<Softwareproject>> selectedprojects(string regdno)
        {
            IEnumerable<Softwareproject> list = null;
            try
            {
                var query = "select count(ID) count from CAPSTONE_PROJECT_STUDENT_LIST   where regdno='" + regdno + "'";
                list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));

            }
            catch (Exception e)
            {

                throw e;
            }
            return list;
        }
        public async Task<IEnumerable<Softwareproject>> Getprojectslistchange(string campus, string selectedValuesArray)
        {
            IEnumerable<Softwareproject> list = null;
            try
            {
                var query = "";

                query = $"SELECT DISTINCT Project_title,id FROM CAPSTONE_PROJECT_LISTS WHERE campus='" + campus + "' and selected_count < 6 and confirm_pending_status ='Y' and project_domain like ('%" + selectedValuesArray.Trim() + "%')";

                list = await Task.FromResult(GetAllData42<Softwareproject>(query, null));
            }
            catch (Exception e)
            {
                throw e;
            }
            return list;
        }
        public async Task<int> insertprojectchange(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray, string old_title_id, string title, string count, string domain, string selectedtitleVal)
        {
            int j = 0;
            int s = 0;
            var query = "";
            IEnumerable<Softwareproject> list = null;

            var whereproject = "";
            try
            {
                if (selectedValuesArray != null)
                {
                    IEnumerable<Softwareproject> list2 = null;

                    var maxsquery1 = "select distinct title_id from CAPSTONE_PROJECT_STUDENT_LIST where regdno='" + Regdno + "'  and title_id='" + selectedtitleVal + "'";
                    List<Softwareproject> data3 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery1, null));
                    if (data3.Count() == 0)
                    {
                        var maxsquery2 = "select distinct selected_count,unique_id,project_domain,id as ID,empid as EMPID,project_title as Project_Title from CAPSTONE_PROJECT_LISTS where campus='" + campus + "' and project_title like '%" + selectedValuesArray + "%' and confirm_pending_status ='Y'";
                        List<Softwareproject> data1 = await Task.FromResult(GetAllData42<Softwareproject>(maxsquery2, null));
                        if (Convert.ToInt32(data1.FirstOrDefault().selected_count) < 6)
                        {
                            int finalmaxs = Convert.ToInt32(data1.FirstOrDefault().selected_count) + 1;

                            var query_annual = @"insert into CAPSTONE_PROJECT_STUDENT_LIST (RegdNo,Name,MOBILe,EMAILID,CAMPUS,COLLEGE,DEPARTEMNT,STATUS,DT_TIME,project_domain,unique_id,title_id,project_title,user_id,selected_count) values 
                                    ('" + Regdno + "','" + stuname + "','" + mobile + "','','" + campus + "','" + college + "','" + dept + "','N',getdate(),'" + domain + "','" + data1.FirstOrDefault().unique_id + "','" + data1.FirstOrDefault().ID + "','" + data1.FirstOrDefault().Project_Title + "','" + data1.FirstOrDefault().EMPID + "','" + finalmaxs + "')";
                            j = await Task.FromResult(InsertData42(query_annual, null));
                            if (j > 0)
                            {

                                var updatequery = "update CAPSTONE_PROJECT_LISTS set selected_count=selected_count +1 where Project_domain='" + data1.FirstOrDefault().Project_domain + "' and unique_id='" + data1.FirstOrDefault().unique_id + "'  and id='" + data1.FirstOrDefault().ID + "'";
                                s = await Task.FromResult(Update42(updatequery, null));
                                j = 3;
                            }



                            if (s > 0)
                            {
                                var updatequery = "delete CAPSTONE_PROJECT_STUDENT_LIST where regdno = '" + Regdno + "' and title_id = '" + old_title_id + "'  and Project_domain = '" + domain + "'";
                                j = await Task.FromResult(Delete42(updatequery, null));
                                var updatequery1 = "update CAPSTONE_PROJECT_LISTS set selected_count=selected_count -1 where Project_domain='" + domain + "' and unique_id='" + data1.FirstOrDefault().unique_id + "'  and id='" + old_title_id + "'";
                                s = await Task.FromResult(Update42(updatequery1, null));
                                if (j > 0 && s > 0)
                                {
                                    var query5 = @"insert into student_update_project (RegdNo,Name,mobile,campus,domain,old_title,new_title,old_title_id,new_title_id,updated_by,updated_time) values 
                                    ('" + Regdno + "','" + stuname + "','" + mobile + "','" + campus + "','" + domain + "','" + title + "','" + selectedValuesArray + "','" + old_title_id + "','" + selectedtitleVal + "','" + Regdno + "',getdate())";
                                    j = await Task.FromResult(InsertData(query5, null));
                                    s = 1;
                                }

                            }
                        }
                        else
                        {
                            s = 3;
                        }
                    }
                    else
                    {
                        s = 4;
                    }



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return s;
        }

        public async Task<Mac_details> getmac_details(string regdno)
        {
            var query = "select top 1 * from GITAM_STUDENT_STAFF_MAC_DETAILS where Regdno='" + regdno + "'";
            return await Task.FromResult(GetSingleData<Mac_details>(query, null));

        }

        public async Task<IEnumerable<EventAttendance>> Getevent_attendance(string regdno, string semester)

        {
            var query = "select ev.event_title,atd.start_time,atd.end_time ,FORMAT(CONVERT(date, atd.date), 'dd-MMM-yyyy') as date,atd.status,attendance_status,glearn_attendance_flag ,atd.* FROM[GITAM_EVENTS].[dbo].event_attendance atd,[GITAM_EVENTS].[dbo].events ev where atd.regdid = '" + regdno + "' and ev.id = atd.event_id and atd.status = 1 and atd.attendance_status = '3' order by atd.date desc";
            return await Task.FromResult(GetAllData73<EventAttendance>(query, null));

        }

        public async Task<int> InsertMac_details(Mac_details data, string id1, string id2, string IPADdress)
        {
            int k = 0;
            try
            {
                var query = "insert into GITAM_STUDENT_STAFF_MAC_DETAILS ([Regdno],[Name],[Gender],[Type],[Campus],[MAC1],[MAC2],[CISCO_STATUS],[ID_MAC1],[ID_MAC2]) values('" + data.Regdno + "','" + data.Name + "','" + data.Gender + "','S','" + data.Campus + "','" + data.MAC1 + "','" + data.MAC2 + "','1','" + id1 + "','" + id2 + "')";
                k = await Task.FromResult(InsertData(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }

        public async Task<int> updatemac_details(Mac_details data, string mac1, string mac2, string IPADdress)
        {
            int k = 0;
            try
            {

                string connStr = "server=172.22.12.15;user=cats;database=radius;password=Cq4Dn2wdX_9W_AkX;SslMode=None;";

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    var query_radcheck = "SET SQL_SAFE_UPDATES = 0;delete from radius.radcheck where userid='" + data.Regdno + "' ";
                    var query_radusergroup = "SET SQL_SAFE_UPDATES = 0;delete from radius.radusergroup where userid='" + data.Regdno + "'";

                    using (MySqlCommand cmd = new MySqlCommand(query_radcheck, conn))
                    {
                        k = await cmd.ExecuteNonQueryAsync();
                    }
                    using (MySqlCommand cmd = new MySqlCommand(query_radusergroup, conn))
                    {
                        k = await cmd.ExecuteNonQueryAsync();
                    }


                    string query_insert = $"INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{mac1}', ':=', 'Cleartext-Password');INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{mac2}', ':=', 'Cleartext-Password') ";

                    using (MySqlCommand cmd = new MySqlCommand(query_insert, conn))
                    {

                        k = await cmd.ExecuteNonQueryAsync();
                    }

                    if (k != 0)
                    {
                        string queryy_radus = $"INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{mac1}', 'STUDENT_GROUP', '1');INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{mac2}', 'STUDENT_GROUP', '1')";

                        using (MySqlCommand cmd = new MySqlCommand(queryy_radus, conn))
                        {

                            k = await cmd.ExecuteNonQueryAsync();
                        }

                    }
                    //conn.CloseAsync();
                }



                var query = "update  GITAM_STUDENT_STAFF_MAC_DETAILS set [MAC1]='" + mac1 + "' ,[MAC2]='" + mac2 + "' where  regdno='" + data.Regdno + "' ";
                k = await Task.FromResult(Update(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }

        public async Task<int> updateMac_details(Mac_details data, string IPADdress)
        {
            int k = 0;
            try
            {
                var query = "select [Regdno],[Name],[Gender],[Type],[Campus],[MAC1] as MAC1_old,[MAC2] as MAC2_old,id from GITAM_STUDENT_STAFF_MAC_DETAILS where Regdno='" + data.Regdno + "'";
                Mac_details list = await Task.FromResult(GetSingleData<Mac_details>(query, null));
                if (list.id != 0)
                {

                    var query2 = "update  GITAM_STUDENT_STAFF_MAC_DETAILS set [MAC2]='" + data.MAC2 + "' where id='" + data.id + "' and regdno='" + data.Regdno + "' ";
                    k = await Task.FromResult(Update(query2, null));
                    var query1 = "insert into GITAM_STUDENT_STAFF_MAC_DETAILS_History ([Regdno],[MAC1],[MAC2],dt_date) values('" + list.Regdno + "','" + list.MAC1_old + "','" + list.MAC2_old + "',getdate())";
                    k = await Task.FromResult(InsertData(query1, null));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }
        public async Task<int> InsertNewData_MYSQL(Mac_details data, string id1, string id2, string IPADdress)
        {
            int k = 0;
            try
            {
                // Connection string for the MySQL database
                string connStr = "server=172.22.12.15;user=cats;database=radius;password=Cq4Dn2wdX_9W_AkX;SslMode=None;";

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();
                    string query = "";
                    string queryy = "";
                    // Constructing the query string manually

                    if (data.MAC1 != null && data.MAC2 != null)
                    {
                        query = $"INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC1}', ':=', 'Cleartext-Password');INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', ':=', 'Cleartext-Password');";
                    }
                    else
                    {
                        if (data.MAC1 != null)
                            query = $"INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC1}', ':=', 'Cleartext-Password');";
                        else
                            query = $"INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', ':=', 'Cleartext-Password');";
                    }
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Executing the query and storing the number of affected rows
                        k = await cmd.ExecuteNonQueryAsync();
                    }

                    if (k != 0)
                    {
                        if (data.MAC1 != null && data.MAC2 != null)
                        {
                            queryy = $"INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC1}', 'STUDENT_GROUP', '1');INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', 'STUDENT_GROUP', '1')";
                        }
                        else
                        {
                            if (data.MAC1 != null)
                                queryy = $"INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC1}', 'STUDENT_GROUP', '1')";
                            else
                                queryy = $"INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', 'STUDENT_GROUP', '1')";


                        }
                        using (MySqlCommand cmd = new MySqlCommand(queryy, conn))
                        {
                            // Executing the query and storing the number of affected rows
                            k = await cmd.ExecuteNonQueryAsync();
                        }

                    }
                    //conn.CloseAsync();
                }
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while inserting data.", ex);
            }

            return k;
        }
        public async Task<int> UpdateNewData_MYSQL(Mac_details data, string IPADdress)
        {
            int k = 0;
            try
            {
                // Connection string for the MySQL database
                string connStr = "server=172.22.12.15;user=cats;database=radius;password=Cq4Dn2wdX_9W_AkX;SslMode=None;";

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();

                    var query2 = $"INSERT INTO radcheck (userid, username, macaddr, op, attribute) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', ':=', 'Cleartext-Password')";

                    using (MySqlCommand cmd = new MySqlCommand(query2, conn))
                    {
                        // Executing the query and storing the number of affected rows
                        k = await cmd.ExecuteNonQueryAsync();
                    }

                    if (k != 0)
                    {
                        string queryy = $"INSERT INTO radusergroup (userid, username, macaddr, groupname, priority) VALUES ('{data.Regdno}', '{IPADdress}', '{data.MAC2}', 'STUDENT_GROUP', '1')";
                        using (MySqlCommand cmd = new MySqlCommand(queryy, conn))
                        {
                            // Executing the query and storing the number of affected rows
                            k = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating data.", ex);
            }
            return k;
        }
        public async Task<int> getmacdetails_check(string macid, string Regdno)
        {
            int i = 0;
            try
            {
                var query = "SELECT * FROM GITAM_STUDENT_STAFF_MAC_DETAILS WHERE(REPLACE(REPLACE(MAC1, ':', ''), '-', '') = '" + macid + "' or REPLACE(REPLACE(MAC2, ':', ''), '-', '') = '" + macid + "') and Regdno !='" + Regdno + "' ";
                IEnumerable<Mac_details> list = await Task.FromResult(GetAllData<Mac_details>(query, null));
                if (list.Count() > 0)
                {
                    i = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return i;
        }




        public async Task<Userlogin> Getglearnhistory(string regdno, string campus, string col)
        {
            try
            {
                var query = "select top 1 id from GITAM_LOGIN_HISTORY where LOGIN_USERID = '" + regdno + "' and MODULE_CODE = 'G-Learn' order by id desc ";
                return await Task.FromResult(GetSingleData221gitam<Userlogin>(query, new { }));
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public async Task<int> InsertGlearnhistory(string regdno, string campus, string col)
        {
            int k = 0;
            try
            {
                var query = "insert into GITAM_LOGIN_HISTORY values('G-Learn','" + regdno + "',getdate(),null,'" + col + "','" + campus + "','','' ) select CAST (SCOPE_IDENTITY() As int)";
                k = await Task.FromResult(InsertData221gitamdb_id(query, null));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return k;
        }


    }
}
