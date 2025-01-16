using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class StudentPhoto
    {
        public string REGDNO { get; set; }
        public string NAME { get; set; }
        public string CAMPUS { get; set; }
        public string COURSE { get; set; }
        public string BRANCH { get; set; }
        public string MOBILE { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string ADDRESS { get; set; }
        public string LANDLINE { get; set; }
        public string EMAIL { get; set; }
        public string CHALLANNO { get; set; }
        public string AMOUNT_PAID { get; set; }
        public string BANK_NAME { get; set; }
        public string CHALLAN_DATE { get; set; }
        public string STATUS { get; set; }
        public string AADHAR { get; set; }
        public string NADNO { get; set; }
        public string PINNO { get; set; }

        public string Approval_FLAG { get; set; }
        public string Approval_USER_ID { get; set; } 
        public string msg { get; set; }
        public string batch { get; set; }
        public string date { get; set; }

        public string ImageUrl { get; set; }
        public string page_status { get; set; }
        public string img_status { get; set; }


    }
}