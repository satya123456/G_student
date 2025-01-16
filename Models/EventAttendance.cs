using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class EventAttendance
    {
        public string event_title { get; set; }

        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public string attendance_status { get; set; }
        public string glearn_attendance_flag { get; set; }
    
    }
}