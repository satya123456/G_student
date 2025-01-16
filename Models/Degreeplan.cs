using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Degreeplan
    {
        public string NAME { get; set; }
        public string REGDNO { get; set; }
        public string total_credits { get; set; }
        public string SEM_CREDITS { get; set; }
        public string CUM_CREDITS { get; set; }
        public string CAMPUS { get; set; }
        public string COLLEGE { get; set; }
        public string DEGREE { get; set; }
        public string BRANCH { get; set; }
        public string COURSE { get; set; }
        public string SEMESTER { get; set; }
        public string DEPARTMENT { get; set; }
        public string BATCH { get; set; }
        public string BATCH1 { get; set; }
        public string UC_COUNT { get; set; }
        public string FC_COUNT { get; set; }
        public string PC_COUNT { get; set; }
        public string PE_COUNT { get; set; }
        public string OE_COUNT { get; set; }
        public string MIC_COUNT { get; set; }
        public string TOTAL_CREDITS { get; set; }
        public string STATUS { get; set; }
        public string DT_TIME { get; set; }
        public string SGPA { get; set; }
        public string CGPA { get; set; }
        public string UC { get; set; }
        public string FC { get; set; }
        public string PC { get; set; }
        public string PE { get; set; }
        public string OE { get; set; }
        public string MIC { get; set; }
        public string UC_percent { get; set; }
        public string FC_percent { get; set; }
        public string PC_percent { get; set; }
        public string PE_percent { get; set; }
        public string OE_percent { get; set; }
        public string MIC_percent { get; set; }
        public string totalcreditssum { get; set; }
        public string totalcreditssum2 { get; set; }
        public string SUBJECT_CODE { get; set; }
        public string SUBJECT_NAME { get; set; }
        public string CBCS_CATEGORY { get; set; }
        public string Category { get; set; }
        public string UC_remain { get; set; }
        public string FC_remain { get; set; }
        public string PC_remain { get; set; }
        public string PE_remain { get; set; }
        public string OE_remain { get; set; }
        public string MIC_remain { get; set; }
        public string UC_select { get; set; }
        public string FC_select { get; set; }
        public string PC_select { get; set; }
        public string PE_select { get; set; }
        public string OE_select { get; set; }
        public string MIC_select { get; set; }

        public string Total { get; set; }
        public string ID { get; set; }
        public string ENAME { get; set; }
        public string EVENUE { get; set; }
        public string EDATE { get; set; }
        public DateTime datee { get; set; }

        public string FROMTIME { get; set; }
        public string TOTIME { get; set; }
        public string slot { get; set; }
        public string DTTIME { get; set; }
        public string USERID { get; set; }
        public string MAXCOUNT { get; set; }
        public string ALLOTEDCOUNT { get; set; }
        public string AppointmentID { get; set; }
        public string row { get; set; }
        public string msg { get; set; }
        public string flag2 { get; set; }
        public string flag3 { get; set; }

        public int flag { get; set; }
        public string appointment_flag { get; set; }

    }
}