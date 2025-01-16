using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Softwareproject
    {
        public int ID { get; set; }
        public string Project_Title { get; set; }
        public string Project_Description { get; set; }
        public string Expected_Outcomes { get; set; }
        public string Tools_Technologies { get; set; }
        public string EMPID { get; set; }
        public string EMP_NAME { get; set; }
        public string CAMPUS { get; set; }
        public string COLLEGE { get; set; }
        public string DEPARTEMNT { get; set; }
        public string STATUS { get; set; }
        public string USER_ID { get; set; }
        public string VERIFIED_USER_ID { get; set; }
        public string Project_domain { get; set; }
        public string DT_TIME { get; set; }
        public string VERIFIED_DT_TIME { get; set; }
        public string DOMAIN { get; set; }
        public string selected_count { get; set; }
        public string unique_id { get; set; }
        public string total_project_count { get; set; }
        public int count { get; set; }
        public string team { get; set; }
        public string title_id { get; set; }
    }
}