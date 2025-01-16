using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gstudent.Models
{
    public class Students
    {
        public string redgno { get; set; }
        public string regdno { get; set; }
        public string name { get; set; }
        public string MOBILE { get; set; }
        ///////////////////////////////
        public string Reason { get; set; }
        public string Date { get; set; }
        public string fromtime { get; set; }
        public string totime { get; set; }
        public string Type { get; set; }
        public string emp_name { get; set; }
        /*public string Fromdate { get; set; }
        public string Todate { get; set; }
        public string ApprovedDate { get; set; }*/
        public string TITLE { get; set; }
        public string CONTENT_DATA { get; set; }
        public string CONTENT_URL { get; set; }
        public string SEARCH_ID { get; set; }
        public string content_type { get; set; }
        public string Month { get; set; }
        public string year { get; set; }
        public string flag { get; set; }


        /*keep datetime fields should be datetime fields*/
        /*Added by satya*/

        // public string regdno { get; set; }
        // public string name { get; set; }
        //public string emp_name { get; set; }
        // public string MOBILE { get; set; }
        public string empid { get; set; }
        public string empname { get; set; }
        public string mobile { get; set; }
        public string CURRSEM { get; set; }
        public string dept_code { get; set; }
        public string ACADEMIC_YEAR_TO { get; set; }
        public string BRANCH_CODE { get; set; }
        public string COURSE_CODE { get; set; }
        public string COLLEGE_CODE { get; set; }
        public string CAMPUS { get; set; }
        public string section { get; set; }
        public string degree_code { get; set; }
        public string batch { get; set; }
        public string parent_mobile { get; set; }
        public string GENDER { get; set; }
        public string hostler { get; set; }
        public string HOSTEL_BLOCK { get; set; }
        public int ID { get; set; }
        public string AccountNo { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public DateTime ApprovedDate { get; set; }
        //public string Reason { get; set; }
        public string Isapprove { get; set; }
        public string Approvedby { get; set; }
        public string ParentMobile { get; set; }
        public string group_code { get; set; }
        public DateTime AppliedDate { get; set; }
        public string HostelCode { get; set; }
        //public string Type { get; set; }
        public double Fromtime { get; set; }
        public double Totime { get; set; }
        public string biometric_id { get; set; }
        public int refno { get; set; }
        public DateTime Permissiondate { get; set; }
        public string Travelby { get; set; }
        public string Destination { get; set; }
        public string remarks { get; set; }
        public string fromdatetime { get; set; }
        public string todatetime { get; set; }
        public string Travelinginformation { get; set; }
        public string hostelconame { get; set; }


        public string TOTAL_MESS_BILL { get; set; }
        public string arrears { get; set; }
        public string advanced_payment { get; set; }
        public string total_fee { get; set; }
        public string total { get; set; }
        public string fine { get; set; }
        public string fee_demand1 { get; set; }
        public string tution_fee_arrears { get; set; }
        public string From_date { get; set; }
        public string To_date { get; set; }
        public string PARENT_PASSWORD { get; set; }
        public string NEW_PASSWORD { get; set; }

        ////////////////////
        ///
        public string SUBJECT_NAME { get; set; }
        public string CREDITS { get; set; }
        public string CBCS_CATEGORY { get; set; }
        public string building_name { get; set; }
        public string room_no { get; set; }

        public string SUBJECT_CODE { get; set; }
        public string SUBJECT_TYPE { get; set; }

        public string id { get; set; }
        public string slno { get; set; }
        public string CHALLAN_NO { get; set; }

        public string FROM_TIME { get; set; }
        public string TO_TIME { get; set; }
        public string description { get; set; }
        public string weekday { get; set; }

        public string father_name { get; set; }
        public string qrtext { get; set; }

        //course reg 
        public string UC { get; set; }
        public string FC { get; set; }
        public string PC { get; set; }
        public string PE { get; set; }
        public string OE { get; set; }
        public string MIC { get; set; }
        public string TOTAL { get; set; }
        public string class1 { get; set; }
        public string DISPLAY_TYPE { get; set; }

        /*added for fee details*/
        public string vsemfee1 { get; set; }
        public string latefee { get; set; }
        public string vafee { get; set; }
        public string vpf { get; set; }

        public string vscholorship { get; set; }
        public string file_flag { get; set; }

        public string institute { get; set; }
        public string program { get; set; }
        public string branch { get; set; }

        public string course_name { get; set; }
        public DateTime exam_date { get; set; }

        public string timings { get; set; }
        public string degree_type { get; set; }
        public string roomjson { get; set; }
        public string sem { get; set; }
        public string student_type { get; set; }
        ////kartikeya/////
        public string arrival_time { get; set; }
        public string depart_time { get; set; }
        public string org_name { get; set; }
        public string dept_name { get; set; }
        public string mentor_name { get; set; }
        public string learning_actions { get; set; }


        public string dt_time { get; set; }
        public string company_name { get; set; }

        public string staff_guide { get; set; }
        public string activities_performed { get; set; }

        public string key_per_indicators { get; set; }
        public string challenges { get; set; }
        public string achievements { get; set; }
        public string learning_points { get; set; }
        public string field_work_title { get; set; }
        public string report_date { get; set; }
        public string domain_research_work { get; set; }
        public string sub_domain_research_work { get; set; }
        public string rpm_background { get; set; }
        public string problem_gap { get; set; }
        public string study_methods { get; set; }
        public string expected_outcomes { get; set; }
        public string statusflag { get; set; }

        public int hostel_arrears { get; set; }

    }
}
