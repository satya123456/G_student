using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class FeedbackMaster
    {
        public string NATURE { get; set; }

        public string FEEDBACK1 { get; set; }
        public string FEEDBACK2 { get; set; }

        public string FEEDBACK3 { get; set; }

        public string FEEDBACK4 { get; set; }
        public string FEEDBACK5 { get; set; }
        public string FEEDBACK1_MARK { get; set; }

        public string FEEDBACK2_MARK { get; set; }

        public string FEEDBACK3_MARK { get; set; }

        public string FEEDBACK4_MARK { get; set; }
        public string FEEDBACK5_MARK { get; set; }

        public string ID { get; set; }
        public string maxmarks { get; set; }
        public string msg { get; set; }

        public string regdno { get; set; }
        public string stuname { get; set; }
        public string subject_code { get; set; }
        public string dept_code { get; set; }
        public string course_code { get; set; }
        public string empid { get; set; }
        public string empname { get; set; }
        public string branch_code { get; set; }
        public string college_code { get; set; }
        public string campus_code { get; set; }
        public string degree_code { get; set; }
        public string batch { get; set; }
        public string section { get; set; }
        public string subjectname { get; set; }
        public string semester { get; set; }
        public string flag { get; set; }
    public string feedbacksession { get; set; }
        public int count { get; set; }
        public string Q_STATUS { get; set; }
        public string group_code { get; set; }


    }

}