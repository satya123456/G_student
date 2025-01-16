using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Event
    {
        public string ConfirmFlag { get; set; }
        public string eventid { get; set; }
        public string EventName { get; set; }
        public string EventLocation { get; set; }
        public string EventDate { get; set; }
        public string Date { get; set; }
        public string End_date { get; set; }
        public string Uploadtype { get; set; }
        public string url { get; set; }
        public string Contact_person { get; set; }
        public string Contact_number { get; set; }
        public string Contact_email { get; set; }
        public string IMAGE_PATH { get; set; }
        public string organizedby { get; set; }
        /*satya*/
        public string Event_dates { get; set; }
        public string eventday { get; set; }
        public string Eventmonth { get; set; }
        public string campus { get; set; }
        public string EventDescription { get; set; }

    }
}