using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class residence
    {

        public string regdno { get; set; }
        public string FLOOR { get; set; }
        public string ROOM_NO { get; set; }
        public string HOSTEL_BLOCK { get; set; }
        public string date { get; set; }
        public string EventDateTime { get; set; }
        public string Location { get; set; }
    }
    public class student_displinary
    {
        public string regdno { get; set; }
        public string Comment { get; set; }
        public string date { get; set; }
        public string Employee_name { get; set; }
        public string Employee_ID { get; set; }
    }
}