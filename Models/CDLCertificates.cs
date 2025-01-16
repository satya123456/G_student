using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class CDLCertificates
    {
        public string SLNO { get; set; }
        public string REGDNO { get; set; }
        public string NAME { get; set; }
        public string FATHER_NAME { get; set; }
        public string DOB { get; set; }
        public string NATIONALITY { get; set; }
        public string RELIGION { get; set; }
        public string DOA { get; set; }
        public string DOL { get; set; }
        public string MEDIUM { get; set; }
        public string COURSE { get; set; }
        public string COURSE_CODE { get; set; }
        public string STATUS { get; set; }
        public string course_name { get; set; }
        public string branch { get; set; }
        public string Batch { get; set; }
        public string qrtext { get; set; }


        public string sysdate { get; set; }

        public string DOB_WORDS { get; set; }
        public string sp_name { get; set; }


        public string Month_Year { get; set; }
        public string DOC { get; set; }
    }
}