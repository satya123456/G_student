using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class LmsResource
    {
        public string name { get; set; }
        public string order_id { get; set; }
        public string year { get; set; }
        public string id { get; set; }
        public string action_name { get; set; }
        public string postedby { get; set; }
        public string subcode { get; set; }
        public string subname { get; set; }
        public string groupcode { get; set; }
        public string radiovalue { get; set; }
        public string status { get; set; }
        public string grp_code { get; set; }
        public string time { get; set; }
        public string Module_id { get; set; }
        public string date { get; set; }
        public string Periods { get; set; }
        public string Delivery_method { get; set; }
        public string Topic_name { get; set; }
        //textbook

        public string TEXTBOOK_NAME { get; set; }
        public string UNIT_MAPPING { get; set; }
        public string AUTHOR { get; set; }

        public string EDITION { get; set; }
        public string PUBLICATION { get; set; }
        public string PUBLICATION_PLACE { get; set; }
        public string ISBN { get; set; }

        public string GENERATED_DATE { get; set; }
        public string TYPE { get; set; }
        public string Module_duration { get; set; }
        public string faculty_id { get; set; }
        public string campus_code { get; set; }
    }

}