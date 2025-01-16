using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Attendance
    {
        public string datetime { get; set; }
        public string subjectname { get; set; }
        public string facultyname { get; set; }
        public string Status { get; set; }
        public string regno { get; set; }
        public string percentage { get; set; }
        public string category { get; set; }
        public string subjectcode { get; set; }
        public string sessiondate { get; set; }
        public string To_time { get; set; }
        public string Faculty_ID { get; set; }
        public string type { get; set; }
        public string COURSE { get; set; }

        public string SEMESTER { get; set; }
        public string credits { get; set; }
        public string Subject_type { get; set; }
        /*added by satya*/
        public string course_code { get; set; }
        public string subject { get; set; }
        public string subject_category { get; set; }

        public string section { get; set; }

        public DateTime date1{get;set;}

        public string Fromtime { get; set; }
        public string passfail { get; set; }
    }
}