using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Contactus
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string EMP_NAME { get; set; }
        public string REGDNO { get; set; }
        public string EMAILID { get; set; }
        public string DESIGNATION { get; set; }
        public string CAMPUS { get; set; }
        public string COLLEGE_CODE { get; set; }
        public string DEPT_CODE { get; set; }
        public string EMP_STATUS { get; set; }
        public string Faculty_ID { get; set; }
        public string mentor_Mobile { get; set; }
        public string mentor_EMP_NAME { get; set; }
        public string mentor_EMAILID { get; set; }

        public string hod_Mobile { get; set; }
        public string hod_EMP_NAME { get; set; }
        public string hod_EMAILID { get; set; }
        public string mentor_remarks { get; set; }
        public int id { get; set; }
        public string time { get; set; }
        public string user_name { get; set; }
        public string user_id { get; set; }
    }
}