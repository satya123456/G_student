using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Timetable
    {
        public string name { get; set; }
        public string regdno { get; set; }
        public DateTime dt_time { get; set; }
        public string msg { get; set; }
        public string SUBJECT_CODE { get; set; }
        public string FROMTIME { get; set; }
        public string TOTIME { get; set; }
        public string WEEKDAY { get; set; }
        public string CAMPUS_CODE { get; set; }
        public string COLLEGE_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string SEMESTER { get; set; }
        public string BATCH { get; set; }
        public string SECTION { get; set; }
        public string Curr_sem { get; set; }
        public string timeslots { get; set; }


        public string emp_name { get; set; }

        public string subject_name { get; set; }

        public string roomno { get; set; }

        public string Building_name { get; set; }
        public string class1 { get; set; }

        public string empid { get; set; }




    }
}