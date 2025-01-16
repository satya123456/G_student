using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Security
    {
        public string empid { get; set; }
        public string mobile1 { get; set; }
        public string campus1 { get; set; }
        public string emp_name { get; set; }

        public string HOSTEL_ACNO { get; set; }



        public string REGDNO { get; set; }
        public string NAME { get; set; }
        public string FATHER_NAME { get; set; }
        public string parent_mobile { get; set; }
        public string GENDER { get; set; }
        public string BRANCH_CODE { get; set; }
        public string COLLEGE_CODE { get; set; }
        public string PARENT_EMAIL_ID { get; set; }
        public string MOBILE { get; set; }
        public string CAMPUS { get; set; }

        public string DEGREE_CODE { get; set; }
        public string COURSE_CODE { get; set; }

        public string CLASS { get; set; }
        public string SECTION { get; set; }

        public string HOSTEL_CODE { get; set; }

        public string msg { get; set; }

        public string Permission_date { get; set; }
        public string CampusCode { get; set; }
        public string Approvedby { get; set; }

        public string Fromtime { get; set; }
        public string totime { get; set; }
        public string Isapprove { get; set; }
        public string Status { get; set; }
        public string TIMEIN { get; set; }
        public string TIMEOUT { get; set; }
        public string Permission { get; set; }
        public string Fromdate { get; set; }
        public string Todate { get; set; }
        public int flag { get; set; }
        public string HOSTLER { get; set; }
        public string genarated_by { get; set; }

        public string CheckIn_date { get; set; }
        public string Checkout_date { get; set; }

    }
}