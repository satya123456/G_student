using Gstudent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.ViewModels
{
    public class Studentrecords
    {
        public class Attendancesummary
        {
            public Attendance note { get; set; }
            public IEnumerable<Attendance> notes { get; set; }
            public IEnumerable<Attendance> notessecond { get; set; }
            public IEnumerable<Attendance> flag { get; set; }
            public IEnumerable<Attendance> course { get; set; }
            public IEnumerable<Degreeplan> degreeplan { get; set; }
            
        }
        public class Softwareprojectsummary
        {
            public Softwareproject note { get; set; }
            public IEnumerable<Softwareproject> notes { get; set; }
        }
        public class Scholarshipsummary
        {
            public Scholarship note { get; set; }
            public IEnumerable<Scholarship> notes { get; set; }
        }
        public class studentdatamodel
        {
            public object studentData { get; set; }
            public object othersdata { get; set; }
        }

        public class Studentrecordslist
        {
            public StudentProfile record { get; set; }
            public IEnumerable<StudentProfile> records { get; set; }

        }

        public class trackrecords
        {
            public studenttrack list { get; set; }
            public object lists { get; set; }
            public object performance { get; set; }
        }

        public class timetablesummary
        {
            public Timetable list { get; set; }
            public IEnumerable<Timetable> lists { get; set; }
        }

        public class newssummary
        {
            public Event list { get; set; }
            public IEnumerable<Event> news { get; set; }
        }
        public class Feedbacksummary
        {
            public Feedback list { get; set; }
            public IEnumerable<Feedback> lists { get; set; }
        }
        public class publicationsummary
        {
            public Publications note { get; set; }
            public IEnumerable<Publications> notes { get; set; }
        }

        public class busspasssummary
        {
            public Buspass note { get; set; }
            public IEnumerable<Buspass> notes { get; set; }
        }


        public class accountssummary
        {
            public Students fine { get; set; }
            public IEnumerable<Students> fee { get; set; }
            public IEnumerable<Students> hostlefee { get; set; }
            public IEnumerable<Students> tutionfee { get; set; }
            public IEnumerable<Students> hostledemands { get; set; }
            public IEnumerable<Students> timetable { get; set; }

            public IEnumerable<Timetable> timetablenew { get; set; }
        }

        public class residencedatamodel
        {
            public object history { get; set; }
            public object maindata { get; set; }
        }
        public class sportsscholarshipsummary
        {
            public SportsScholarship note { get; set; }
            public IEnumerable<SportsScholarship> notes { get; set; }
        }

        public class AdmissionsSummary
        {
            public IEnumerable<SportsScholarship> displinary_details { get; set; }
            public IEnumerable<SportsScholarship> branch_data { get; set; }
            public IEnumerable<SportsScholarship> admission_data { get; set; }
            public IEnumerable<SportsScholarship> counseling_data { get; set; }
            public IEnumerable<SportsScholarship> payment_details { get; set; }
            public IEnumerable<SportsScholarship> scholoreligbility { get; set; }
            public IEnumerable<SportsScholarship> scholorallocated { get; set; }
            public IEnumerable<SportsScholarship> Coursesappliedandpayment { get; set; }

        }
        public class Contactussummary
        {
            public Contactus note { get; set; }
            public IEnumerable<Contactus> notes { get; set; }
        }

        public class Feesummary
        {
            public IEnumerable<SportsScholarship> tutionfee { get; set; }
            public IEnumerable<SportsScholarship> otherfee { get; set; }
            public IEnumerable<SportsScholarship> evaluationfee { get; set; }
            public IEnumerable<Students> fillfee { get; set; }
            public IEnumerable<Students> fillhostel { get; set; }
            public IEnumerable<SportsScholarship> payment_details { get; set; }




        }

        public class hallticketsummary
        {
            public Hallticket note { get; set; }
            public IEnumerable<Hallticket> notes { get; set; }
            public IEnumerable<Hallticket> studentdetails { get; set; }
            public IEnumerable<Attendance> attendancenotes { get; set; }
            public IEnumerable<Hallticket> eligiblesub { get; set; }
            public IEnumerable<Hallticket> Totalsubcount { get; set; }
            public IEnumerable<Hallticket> eligiblesubcount { get; set; }
            public IEnumerable<Hallticket> noteligiblesub { get; set; }
            public IEnumerable<Hallticket> studentdetailslist { get; set; }
            public IEnumerable<Students> studentrooms { get; set; }

            public int activecount { get; set; }

            public string ImageUrl { get; set; }
        }
        public class CDLhallticketsummary
        {
            public IEnumerable<CDLHallticket> CDLRegular { get; set; }
            public IEnumerable<CDLHallticket> CDLSupply { get; set; }
            public IEnumerable<CDLHallticket> CDLbetterment { get; set; }
            public IEnumerable<CDLHallticket> Studentdetails { get; set; }

        }
        public class certificatescollection
        {
            public IEnumerable<Certificatesnew> certificateslist { get; set; }
            public Certificatesnew certificate { get; set; }
            public List<Certificatesnew> certificateoridupli { get; set; }
        }

        public class Degreeplansummary
        {
            public Degreeplan note { get; set; }
            public IEnumerable<Degreeplan> notes { get; set; }
            public IEnumerable<Degreeplan> deg { get; set; }
            public IEnumerable<Degreeplan> open { get; set; }
            public IEnumerable<Degreeplan> faculty { get; set; }
            public IEnumerable<Degreeplan> required { get; set; }
            public IEnumerable<Degreeplan> selected { get; set; }
            public IEnumerable<Degreeplan> program { get; set; }
            public IEnumerable<Degreeplan> programelective { get; set; }
            public IEnumerable<Degreeplan> MICS { get; set; }
            public IEnumerable<Degreeplan> medical { get; set; }

        }


        public class FeedbackCheckSummary
        {
            public List<FeedbackMaster> emp { get; set; }
            public List<FeedbackMaster> list { get; set; }


        }

        public class FeedbackCheckEmpSummary
        {
            public List<FeedbackMaster> emp { get; set; }
            public List<studenttrack> list { get; set; }


        }


        public class CDLEvaluatoionFeesSummary
        {
            public IEnumerable<CDLEvaluationReceipts> fee { get; set; }
            public IEnumerable<CDLEvaluationReceipts> eval_fee { get; set; }

        }

        public class CDLSupplimentaryFeesSummary
        {
            public List<CDLEvaluationReceipts> exam_centers { get; set; }
            public List<CDLEvaluationReceipts> subjects { get; set; }
            public List<CDLEvaluationReceipts> projectresult { get; set; }
            public List<CDLEvaluationReceipts> data { get; set; }

        }


        public class Courseregistartion
        {
            public IEnumerable<Students> semcredits { get; set; }
            public IEnumerable<Students> credits { get; set; }

            public IEnumerable<Students> sets { get; set; }

            public IEnumerable<Timetable> semlist { get; set; }
            public IEnumerable<Timetable> upcomingtimetable { get; set; }
            public Students feepaid { get; set; }


            public string lbltotalamount { get; set; }
            public int accesscount { get; set; }
            public int sem13_sgpa { get; set; }

            public int txn_flag { get; set; }
            public int arrears_flag { get; set; }
            public int hostel_arrears_flag { get; set; }
            public int hostel_balance_flag { get; set; }
            public int status_flag { get; set; }
        }
        public class Lmsdirectorysummary
        {
            public LMSsubject data { get; set; }
            public LMSsubject marksdata { get; set; }

            public IEnumerable<LMSsubject> directorydata { get; set; }
            public IEnumerable<LMSsubject> directoryfiles { get; set; }
            public IEnumerable<LMSsubject> Assignmentmarks { get; set; }
            public IEnumerable<LMSsubject> Quizmarks { get; set; }
            public IEnumerable<LMSsubject> finalmarks { get; set; }
            public IEnumerable<LMSsubject> Midmarks { get; set; }
            public List<LMSsubject> reportscount { get; set; }
            public List<LMSsubject> markscount { get; set; }
            public IEnumerable<LMSsubject> assigndata { get; set; }
            public IEnumerable<LMSsubject> Totalall { get; set; }


        }


        public class lmstextbooks
        {
            public LmsResource textbook { get; set; }
            public IEnumerable<LmsResource> textbooksummary { get; set; }
        }


        public class timetablesummarymain
        {
            public IEnumerable<Timetable> semlist { get; set; }
            public IEnumerable<Students> facultylists { get; set; }
        }


        public class gradecardsummary
        {
            public string page { get; set; }
            public string qr { get; set; }
        }
     
    }

}