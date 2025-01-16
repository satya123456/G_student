using Gstudent.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Gstudent.Repositories.Interface
{
    public interface IStudentRepository
    {
        Task<IEnumerable<StudentProfile>> GetDetails(string userid);
        Task<IEnumerable<StudentProfile>> getstates();
        Task<StudentProfile> Updateprofileasync(StudentProfile _Parameter);
        Task<IEnumerable<Attendance>> Getflag(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string campus, string course_code);
        Task<IEnumerable<Attendance>> Getattendance_semster_gimsr(string user_id, string table_name, string college_code, string cur_sem, string batchattd);
        Task<IEnumerable<Attendance>> Getattendance_semster(string user_id, string table_name, string college_code, string cur_sem, string batchattd);
        Task<IEnumerable<Attendance>> Getattendance_semster_byselection(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string category, string date);
        Task<IEnumerable<Attendance>> Getattendance_semster_byselection_gimsr(string user_id, string table_name, string college_code, string cur_sem, string batchattd, string category, string date);
        Task<IEnumerable<Attendance>> Getsemsterlist(string college_code, string campus_code, string branch_code, string batch);
        Task<IEnumerable<Attendance>> GetSemesterdata(string college_code, string campus_code, string branch_code, string batch, string semester);
        Task<List<Scholarship>> Getscholarshipdetails(string applicationno);
        Task<List<Scholarship>> Getscholarshipdetailsbycategory(string category, string regdno, string Applicationo);
        Task<IEnumerable<Scholarship>> Getscholarcount(string applicationno);
        Task<Scholarship> Updatescholarship(Scholarship chart);
        Task<Students> getoldPassword(string username, string type);
        string ChangeUserPasswordAsync(string NPW, string USER_ID, string type, string plainpwd);
        Task<IEnumerable<Students>> GetUnderProcess(string userid);
        Task<IEnumerable<Students>> GetApproved(string userid);
        Task<IEnumerable<Students>> GetRejected(string userid);
        Task<IEnumerable<Students>> GetHistory(string userid);
        Task<List<Students>> GetNoticeboarddata(string college_code, string campus_code, string dept_code, string CLASS);
        Task<List<Students>> GetNoticeboarddataMore(string college_code, string campus_code, string dept_code, string CLASS);
        Task<IEnumerable<Students>> GetDistinctDates(string college_code, string campus_code, string dept_code, string CLASS);


        /*satya*/
        Task<IEnumerable<studenttrack>> getfeedback_facultyasyncnew(string groupcode, string campus, string branch, string regdno, string subject_code, string semester, string college_code, string batch);

        Task<IEnumerable<studenttrack>> getfeedback_facultyasyncguest(string groupcode, string campus, string branch, string regdno, string subject_code, string semester, string college_code, string batch);

        Task<IEnumerable<Students>> get_coordinator(string campus);
        Task<IEnumerable<Students>> gethostlerdetails(string uid);
        Task<Students> createhostelpermision(Students student);
        Task<IEnumerable<Students>> gethostlerpermissions(string uid);
        Task<IEnumerable<StudentProfile>> GetCDLProfileDetails(string userid);
        Task<StudentProfile> UpdateCDLProfile(StudentProfile chart);
        Task<IEnumerable<Hostelbiometric>> gethostelbiometricsmsdetails(string regno, string curr_sem);
        Task<IEnumerable<CDL_academictrack>> CDL_studentdetails(string user_id);
        Task<IEnumerable<CDL_academictrack>> getfeedetails(string user_id);
        Task<IEnumerable<CDL_academictrack>> getmaterial_dispatch_cdl(string user_id);
        Task<IEnumerable<CDL_academictrack>> getassginments_cdl(string user_id);
        Task<IEnumerable<CDL_academictrack>> getresults_cdl(string user_id);
        Task<IEnumerable<CDL_academictrack>> Getcgpa(string user_id);
        Task<IEnumerable<CDL_academictrack>> getcertificates_cdl(string user_id);


        Task<IEnumerable<studenttrack>> getsemesterasync(string userid);
        Task<List<studenttrack>> getsubjects(string userid, string semester);
        Task<List<studenttrack>> getperformance(string userid, string semester);

        Task<List<studenttrack>> getperformancedashboard(string userid, string semester);
        Task<IEnumerable<studenttrack>> getcourselist(string semester, string userid, string coursecode, string batch, string currentsem);
        Task<IEnumerable<Timetable>> gettimetabledetails(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH,string course_code,string degree_code);
        Task<IEnumerable<Timetable>> getsessions(string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string COURSE_CODE);

        Task<IEnumerable<Timetable>> getslots(string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string COURSE_CODE,string degreecode);
        Task<Feedback> inserthostelfeedback(Feedback _Parameter);
        Task<List<Feedback>> registrationcheck(string REGDNO);
        Task<List<Feedback>> feedbackcheck(string REGDNO);
        Task<IEnumerable<Publications>> Getpublications(string regdno);
        Task<IEnumerable<Publications>> Getpublicationscheck(string regdno, string publication_number, string journal_title);
        Task<Publications> Insertpublication(Publications chart);
        Task<int> Deletepublications(string regno, string pubtit, string jname);

        Task<IEnumerable<StudentProfile>> Getstudentdetails(string username, string password);
        Task<IEnumerable<Students>> get_coordinator_dropdown(string campus);
        Task<IEnumerable<Students>> Getcoordinator_mobile(string campus, string empid);


        Task<IEnumerable<StudentProfile>> getthesisasync(string user_id);

        Task<IEnumerable<Buspass>> getstudentdetails(string user_id);
        Task<IEnumerable<Buspass>> Getbustypelist(string campus);
        Task<IEnumerable<Buspass>> getroutelist(string typeid, string campus_code);
        Task<IEnumerable<Buspass>> getfareasync(string typeid, string campus_code, string Board);
        Task<Buspass> Insertbuspassasync(Buspass chart);
        Task<List<Buspass>> busregistrationcheck(string REGDNO, string BUSPASSYEAR);
        Task<int> Insertemailpassword(string regdno, string name, string password, string mobile);
        Task<IEnumerable<Students>> fillfee(string regdno);
        Task<IEnumerable<Students>> fillhostelpayment(string regdno);
        Task<IEnumerable<Students>> get_hosteldemand(string regdno);
        Task<IEnumerable<Students>> getSeniorTutionFeeDemand(string regdno);
        Task<IEnumerable<Security>> GetSecurityReports(string userid);
        Task<IEnumerable<DiningSubscription>> getstudentinfo(string student_id, string CAMPUS);
        Task<IEnumerable<DiningSubscription>> getdininginfo(string user_id, string fromdate, string todate, string CAMPUS);
        Task<IEnumerable<residence>> Getresidenceroomdetails(string regdno);
        Task<IEnumerable<residence>> Getresidenceroomdetails19(string regdno);
        Task<IEnumerable<residence>> Getresidenceattendance(string regdno, string date);
        Task<IEnumerable<Students>> getDetails(string regno);
        DataTable GetTimetable(string reg_no, string curr_sem);
        Task<IEnumerable<Students>> getFacultyDetailsTimetable(string reg_no, string SEMESTER);
        Task<IEnumerable<Feedback>> getstudentdetailsfb(string user_id);
        Task<IEnumerable<student_displinary>> GetStudent_displinary(string regdno);
        int withdraw(string id);
        Task<IEnumerable<Students>> GetDistinctYears(string college_code, string campus_code, string dept_code, string CLASS);
        Task<IEnumerable<SportsScholarship>> getsportslist();
        Task<IEnumerable<SportsScholarship>> getlevellist();
        Task<SportsScholarship> getsportsscholarship(SportsScholarship chart);
        Task<IEnumerable<SportsScholarship>> getcompetetionnames(string levelid);
        Task<IEnumerable<SportsScholarship>> getacheivementlist();
        Task<List<SportsScholarship>> semcheck(string REGDNO);
        Task<SportsScholarship> InsertSportsScholarship(SportsScholarship chart);
        Task<IEnumerable<SportsScholarship>> getsportsscholargrid(string userid);
        Task<SportsScholarship> Updateremarks(SportsScholarship chart);
        Task<List<SportsScholarship>> getsportsschck(string REGDNO);
        Task<List<SportsScholarship>> getsportsidchck(string REGDNO);
        Task<int> Deletesportscholarship(string slno, string regdno);

        Task<IEnumerable<Students>> getAllhostlerpermissions(string uid);
        Task<IEnumerable<Students>> filldashboardhostelpayment(string regdno, string pinno);
        Task<IEnumerable<Students>> getdashboardSeniorTutionFeeDemand(string regdno, string pinno);
        Task<IEnumerable<residence>> Getbiometricattendance_dashboard(string regdno);
        Task<IEnumerable<Attendance>> Getcoursestructure(string college_code, string campus_code, string branch_code, string batch, string sem, string regdno);

        Task<IEnumerable<SportsScholarship>> gettuitionfee(string userid);
        Task<IEnumerable<SportsScholarship>> getotherfee(string userid);
        Task<IEnumerable<SportsScholarship>> getevaulfee(string userid);

        Task<IEnumerable<SportsScholarship>> Student_Admissiondata(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Student_branchdata(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Student_counseling(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Student_AdmPaymentdetails(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Student_Scholareligibility(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Student_Scholarallocated(string APPLICATION_NO);
        Task<IEnumerable<SportsScholarship>> Coursesappliedandpayment(string APPLICATION_NO);
        Task<IEnumerable<Students>> fillhostelpaymentfees(string regdno);
        Task<Contactus> contactusasync(Contactus chart);
        Task<IEnumerable<Students>> getdashboardtimetable(string regdno, string pinno, string day);
        Task<IEnumerable<Contactus>> getmentor_remarksdata(string regdno);
        Task<IEnumerable<studenttrack>> getresults(string reg, string sem, string college_code);


        Task<List<studenttrack>> getmonth(string semester, string userid, string process);
        Task<List<studenttrack>> getmonthst2(string semester, string userid, string process);
        Task<List<studenttrack>> getcarddetails(string userid);
        Task<List<studenttrack>> getStudentcarddetails(string semester, string userid);
        Task<int> DeleteDetails(string campus, string college, string course, string branch, string batch);
        Task<List<studenttrack>> getprocessdropdown(string semester, string userid);
        Task<List<studenttrack>> getGradeResult(string semester, string userid, string month, string year, string process);
        Task<List<studenttrack>> getGradeResultst2(string semester, string userid, string month, string year, string process);

        Task<IEnumerable<Students>> getarrears(string regdno, string sem);
        Task<IEnumerable<Students>> getarrearshostel(string regdno, string sem);
        Task<IEnumerable<Students>> getarrearshostelbalanacedata(string regdno, string sem);
        Task<studenttrack> InsertGradedetails(studenttrack _Parameter);
        Task<List<studenttrack>> getGradeReports(studenttrack _Parameter);
        Task<List<studenttrack>> updateGradedetails(List<studenttrack> _Parameter);
        Task<List<studenttrack>> getQrResult(string reg, string sem, string month, string process, string year);


     
        Task<IEnumerable<studenttrack>> getfeedback_subjectsasync(string userid, string sem);

        Task<IEnumerable<studenttrack>> getfeedback_facultyasync(string groupcode, string campus, string branch);
        //Task<IEnumerable<studenttrack>> getfeedback_facultyasyncnew(string groupcode, string campus, string branch, string regdno, string subject_code, string semester,string college_code);
        Task<IEnumerable<FeedbackMaster>> getquestionlist();
        Task<IEnumerable<FeedbackMaster>> getfeedback_check(string subjectname, string faculty, string feedbacksession, string REGDNO, string sem);

        Task<Students> getgroup_code(string subjectname, string faculty, string feedbacksession, string REGDNO, string sem);

        Task<IEnumerable<FeedbackMaster>> Insertfeedbacknew(List<FeedbackMaster> chart);
        Task<List<CDLCertificates>> getcdlstudycertificate(string userid);
        Task<IEnumerable<Event>> getevents(string campus);
        Task<List<CDLCertificates>> getcdltransfercertificate(string userid);
        Task<IEnumerable<studenttrack>> getCDLsemesterasync(string userid);
        Task<List<studenttrack>> getStudentcarddetailsCDL(string semester, string userid);
        Task<int> DeleteDetailsCDL(string campus, string college, string course, string branch, string batch);
        Task<List<studenttrack>> getGradeResultCDL(string semester, string userid, string month, string year, string process);
        Task<List<studenttrack>> getGradeReportsCDL(studenttrack _Parameter);
        Task<List<studenttrack>> updateGradedetailsCDL(List<studenttrack> _Parameter);
        Task<List<studenttrack>> getQrResultCDL(string reg, string sem, string month, string process, string year);
        Task<IEnumerable<Hallticket>> gethallticketdata52(string regdno);
        Task<IEnumerable<Hallticket>> getstudentdata(string regdno);
      //  Task<IEnumerable<Hallticket>> getstudentPHDhallticket(Hallticket chart);
        Task<List<CDLCertificates>> getcdlMigration(string userid);
        Task<List<CDLCertificates>> getdateofcompletion(string userid);
        Task<IEnumerable<Attendance>> Getcoursestructuremenu(string college_code, string campus_code, string branch_code, string batch, string sem, string regdno);

        Task<IEnumerable<CDLHallticket>> getCDLRegularStudent(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLRegular(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLSupply(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLBetterment(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLsubBranch(string colgcode, string branch);
        Task<IEnumerable<CDLHallticket>> getCDLSupplySubjects(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLBettermentSubjects(string userid);
        Task<IEnumerable<CDLHallticket>> getCDLSupplySubjectstIMINGS(string sub_code, string batch, string semester, string course);
        Task<IEnumerable<Certificatesnew>> getcertificate_labels(string WhreCond, string changing);
        Task<IEnumerable<Certificatesnew>> getsemester(string regdno);
        Task<IEnumerable<Certificatesnew>> getcertificate_original_duplicate(string regdno, string certificatetype);
        Task<IEnumerable<Certificatesnew>> semselectedoriginal(string sem, string month, string year, string regdno);
        Task<IEnumerable<Certificatesnew>> semselectedonchange(string sem, string month, string year, string regdno, string certype);
        Task<int> getreference_id();
        Task<int> inserttotals(string regdno, string sname, string category, string totalamount, string refid);
        Task<int> insertdatacertificate(int referinceid, string branch_code, string college, string regdno, string coursecode, string BATCH, string currentsem, string certificate, string type, string category, string sem, string month, string year, int amt, int bypost, string degreecode, string sessionid, string pubfilepath, string campus, string sname);
        Task<IEnumerable<Certificatesnew>> getcertificateconfirmation(string regdno, string refid, string stname, string sessionid);

        Task<List<studenttrack>> getGradeCardDetails(string userid);
        Task<List<studenttrack>> getmonth(string semester, string userid);
        Task<IEnumerable<Event>> getdashboardevents(string campus);
        Task<List<Phdbiometric>> getphdbiometricreport(string userid, string month, string year);



        Task<Degreeplan> getcreditperformance(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getsubjects(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getsubjectsOE(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getsubjectsFC(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getremainingcredits(String regdno);
        Task<IEnumerable<Degreeplan>> getsubjectsMIC(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getsubjectsPC(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getsubjectsPE(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getselectingcredits(Degreeplan chart);
        Task<IEnumerable<FeedbackMaster>> getfeedback_subjects(string userid, string sem);


        Task<IEnumerable<Degreeplan>> getmedicalappointment(Degreeplan chart);
        Task<IEnumerable<Degreeplan>> getmedicallists(Degreeplan chart);
        Task<List<Degreeplan>> checkappointment(string id);
        Task<Degreeplan> updateflagappointment(Degreeplan chart);
        Task<Degreeplan> updateappointment(Degreeplan chart);
        Task<Degreeplan> insertappointment(Degreeplan chart);
        Task<List<CDLEvaluationReceipts>> getEvaluationStudentDetails(string userid);
        Task<IEnumerable<CDLEvaluationReceipts>> getCDLFeeHistory(string userid);
        Task<IEnumerable<CDLEvaluationReceipts>> getCDLEvalFeeHistory(string userid);
        Task<List<CDLEvaluationReceipts>> getCDLSupplyDetails(string processtype, string degree, string course, string campus, string college);
        Task<IEnumerable<CDLEvaluationReceipts>> getCDLEvaluationSem(string college);
        Task<IEnumerable<CDLEvaluationReceipts>> getCDLlastdate(string processtype, string degree, string course, string campus, string college);
        Task<IEnumerable<CDLEvaluationReceipts>> getCDLfeechallan(string userid);
        Task<List<CDLEvaluationReceipts>> getCDLExamCentre();
        Task<List<CDLEvaluationReceipts>> getCDLSupplySubjects(string userid, string vsemlist);
        Task<List<CDLEvaluationReceipts>> getCDLProjectResults(string userid, string vsemlist);

        Task<List<CDLEvaluationReceipts>> getCDLRefid();
        Task<int> updateCDLRefid(int refid);
        Task<int> InsertEvaluationRequest(List<CDLEvaluationReceipts> list);
        Task<int> InsertTotalFee(String regdno, String name, String totalfee, String refid, String feetype);
        Task<List<CDLEvaluationReceipts>> getReceiptconfirmation(string regno, string SessionID);
        Task<int> UpdateCDLevaluationRequest(string sessionid);
        Task<List<CDLEvaluationReceipts>> getCDLRevalutionResult(string userid, string vsemlist, string month, string year);
        Task<int> InsertCertificatedata(CDLEvaluationReceipts data);
        Task<int> InsertCertificatedataTotal(CDLEvaluationReceipts data);
        Task<List<CDLEvaluationReceipts>> getReceiptconfirmationCertificate(string regno, string SessionID);
        Task<IEnumerable<FeedbackMaster>> get_groupcode(string campus, string college, string course, string branch, string curr_sem, string subject, string section);

        Task<FeedbackMaster> get_facultydept(string fid);
        Task<IEnumerable<Students>> getcredits_currsem(string regdno, string sem);

        Task<IEnumerable<Students>> getplan_currsem(string regdno, string sem);
        Task<IEnumerable<Students>> getsets(string campus, string branch, string batch, string sem);
        Task<IEnumerable<Timetable>> getsettimetable(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string set);
        Task<Students> getfeepaid(string regno, string sem);
        Task<Students> getlatefee(string regdno, string sem);
        Task<Students> getotherfee(string regdno, string sem);
        Task<IEnumerable<Students>> getaccessdata(string regdno);
        Task<IEnumerable<Timetable>> getpopupfaculty(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH, string set, string subjectcode, string sec);
        Task<IEnumerable<Students>> getSEM13_SGPA(string regdno, string sem);
        Task<IEnumerable<Students>> getTxn_flag(string regdno, string sem);
        Task<IEnumerable<Timetable>> getupcomingtimetable(string regdno, string COLLEGE_CODE, string BRANCH_CODE, string SEMESTER, string SECTION, string CAMPUS_CODE, string BATCH);
        Task<IEnumerable<Hallticket>> gethallticketPHDdata52(string regdno);
        Task<IEnumerable<Hallticket>> get_student_data52(string regdno);
        Task<IEnumerable<Hallticket>> getstudenthallticket(Hallticket chart);
        Task<List<Hallticket>> Get_Student_Data(string REGDNO);
        Task<IEnumerable<Hallticket>> getstudenthalltickettheory(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudentspecialhallticket(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudenthallticketpractical(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudentPHDhalltickettheory(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudentPHDhallticketpractical(Hallticket chart);
        Task<List<Hallticket>> Get_Hall_Ticket_Details(string REGDNO, string type1, string currsem);
        Task<IEnumerable<Hallticket>> getstudenthallticketpractical52(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudenthalltickettheory52(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudentspecialhallticketpractical52(Hallticket chart);
        Task<IEnumerable<Hallticket>> getstudentspecialhalltickettheory52(Hallticket chart);
        Task<IEnumerable<Hallticket>> gethalleligiblesubjects(string regdno, string type1, string semester);
        Task<IEnumerable<Hallticket>> gethallnoteligiblesubjects(string regdno, string type1, string semester);
        Task<IEnumerable<Hallticket>> getstudentcoursedata(string regdno);
        Task<IEnumerable<Hallticket>> gethallticketactive(string regno);
        Task<int> getlogout(string regdno);


        //lms 
        Task<IEnumerable<LMSsubject>> add_assidn_data(string folderid);
        Task<IEnumerable<LMSsubject>> getstudentanouncments(string subjectcode, string regdno);
        Task<int> Insertassigneditordata(string result, string regdno, string campus_code, string college_code, string assignid);
        Task<IEnumerable<LMSsubject>> getassignmentmasterfoldersASSIGN(string groupcode, string assignid);
        Task<IEnumerable<LMSsubject>> getassignmentmasterfoldersASSIGN(string groupcode, string assignid, string folderid, string regdno);
        Task<int> Insertassignuploaddata(string fileNameonly, string Regdno, string campus_code, string college_code, string assignid, string ext, string group_code);
        Task<IEnumerable<LMSsubject>> getlmssubjects(string regdno, string sem, string campus_code, string COLLEGE_CODE, string DEPT_CODE, string BRANCH_CODE, string curr_sem);
        Task<LMSsubject> getlmssubjectsmodeulepercentage(string sub_code, string group_code);
        Task<IEnumerable<LMSsubject>> getassignmentmasterfolders(string subcode, string groupcode, string uid, string campus_code, string COLLEGE_CODE, string DEPT_CODE, string BRANCH_CODE, string COURSE_CODE);
        Task<IEnumerable<LMSsubject>> getstudentassignments(string regdno, string groupcode);
        Task<LMSsubject> getsubjectname(string regdno, string groupcode);
        Task<IEnumerable<LMSsubject>> getfilemasterfolders(string groupcode);
        Task<IEnumerable<LMSsubject>> getfilemasterfoldersDATA(string groupcode);
        Task<IEnumerable<LMSsubject>> insertfilemaster(string groupcode);
        //resource 
        // Task<IEnumerable<LmsResource>> Getresource_unitwisedata(string subject_code, string campus,string facultyid);
        Task<IEnumerable<LmsResource>> Getresource_unitwisedata(string subject_code, string campus, string regdno, string college_code, string curr_sem);
        LmsResource getcrseresrctrns1(string folder_id, string group_code, string image, string regdno, string curr_sem);
        LmsResource getcrseresrctrns2(string folder_id, string group_code, string image);
        LmsResource getcrseresrctrns_additional(string folder_id, string group_code, string image);
        Task<LmsResource> getcrseresrctrns(string folder_id, string group_code);
        Task<IEnumerable<LmsResource>> getcrseresrcrepo(string group_code, string type);
        LmsResource getcrseresrctrnsrepo(string folder_id, string group_code);
        Task<IEnumerable<LmsResource>> Getresource_additionaldata(string group_code, string college, string campus, string userid, string dept);
        Task<IEnumerable<LmsResource>> Getresource_topicwisedata(string subject_code, string module_id);
        Task<IEnumerable<LMSsubject>> getstudentAssignmentmarks(string regdno, string groupcode,string subcode);
        Task<IEnumerable<LMSsubject>> getstudentQuizmarks(string regdno, string groupcode);
        Task<IEnumerable<LMSsubject>> getstudentMidmarks(string regdno, string groupcode);
        Task<int> Deleteassignmentfileupload(string assignid, string regdno, string group_code, string fileid, string filename);
        Task<IEnumerable<LmsResource>> gettextbooks(string subject_code,string campus);
        Task<IEnumerable<studenttrack>> getacademicsemesterasync(string userid);
        Task<IEnumerable<LMSsubject>> getstudentCEList(string regdno,string subject_code, string year, string campus_code, string sem);
        Task<IEnumerable<LMSsubject>> getassignmentpending(string regdno);
        Task<IEnumerable<Students>> getFacultyDetailsTimetable_main(string reg_no, string SEMESTER,string branch_code,string course_code, string college_code,string degree_code,string section);
        Task<IEnumerable<LMSsubject>> getstudentsectionlist(string subject_code, string year, string campus_code, string sem);
        Task<IEnumerable<Students>> eventsdecider(string regdno);

        Task<StudentPhoto> getStudentapplication(string regno);
        Task<StudentPhoto> UpdateStudentapplication(StudentPhoto chart);
        Task<StudentPhoto> InsertStudentapplication(StudentPhoto chart);
        Task<StudentPhoto> getStudentmasterphoto(string regno);
        Task<LMSsubject> getassignmentmarksdisplay(string sub_code, string group_code, string assignid, string regdno);
        Task<IEnumerable<LMSsubject>> getassignmentmarksfinaldisplay(string sub_code, string group_code, string regdno);


        Task<IEnumerable<Students>> getexamtimetable(string regdno);
        Task<IEnumerable<Students>> getexamtimetablebydate(string regdno, string date);
        Task<IEnumerable<Students>> GetDetailsstudent(string userid);

        Task<IEnumerable<FeedbackMaster>> getfeedbackfinalyear_check(string REGDNO, string sem);
        Task<IEnumerable<FeedbackMaster>> getfinalyearquestionlist();
        Task<IEnumerable<FeedbackMaster>> InsertGITAM_STUDENT_OUTGOINGfeedbacknew(List<FeedbackMaster> chart);



        //Task<int> InsertMac_details(Mac_details data, string id1, string id2);
        //Task<Mac_details> getmac_details(string regdno);
        //Task<int> updateMac_details(Mac_details data);
        //Task<int> getmacdetails_check(string macid, string Regdno);
        Task<IEnumerable<Students>> SaveDailyLog(string Arrivaltimedata, string departtimedata, string orgnamedata, string dptnamedata, string mtrnamedata, string lernactiondata, string regdno, string semester, string collegeCode, string branchCode, string campusCode);
        Task<IEnumerable<Students>> getdailylog(string Regdnos);
        Task<IEnumerable<Students>> Insertweeklyactivity(string cmpnamedata, string cmpmentordata, string stfguidedata, string actperformeddata, string keyprfindicatordata, string challengesdata, string achievementsdata, string learningpointsdata, string regdno, string semester, string collegeCode, string branchCode, string campusCode);
        Task<IEnumerable<Students>> getweeklyactivities(string Regdnos);
        Task<IEnumerable<Students>> Insertfieldworkproposal(string fwtitledata, string rprtdatedata, string rwdomaindata, string rwksubdomainddata, string rpmbackgrounddata, string prbdefinitiondata, string stdmethoddata, string expoutcomedata, string regdno, string semester, string collegeCode, string branchCode, string campusCode);
        Task<IEnumerable<Students>> getfieldworkproposal(string Regdnos);


        Task<List<studenttrack>> getStudentcarddetailsst2(string semester, string userid);
        Task<IEnumerable<Attendance>> Getattendance_semsterissummerterm(string user_id, string table_name, string college_code, string batchattd);
        Task<IEnumerable<Attendance>> Getattendance_semster_byselectionissummerterm(string user_id, string table_name, string college_code, string batchattd);
        //Task<IEnumerable<Softwareproject>> Getsoftwareprojectslist_async();

        //Task<IEnumerable<Softwareproject>> Getsoftwareprojectslist_async(string campus);
        //Task<IEnumerable<Softwareproject>> Getselectedsoftwareprojectslist_async(string regdno);

        //Task<IEnumerable<Softwareproject>> Getprojectdomain(string campus);
        //Task<IEnumerable<Softwareproject>> Getprojectslist(string campus, string selectedValuesArray);
        //Task<int> insertprojectselection(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray);
        //Task<IEnumerable<Softwareproject>> selectedprojects(string regdno);
        Task<IEnumerable<Softwareproject>> Getsoftwareprojectslist_async(string campus);
        Task<IEnumerable<Softwareproject>> Getselectedsoftwareprojectslist_async(string regdno);

        Task<IEnumerable<Softwareproject>> Getprojectdomain(string campus);
        Task<IEnumerable<Softwareproject>> Getprojectslist(string campus, string selectedValuesArray);
        Task<int> insertprojectselection(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray);
        Task<IEnumerable<Softwareproject>> selectedprojects(string regdno);
        Task<IEnumerable<Softwareproject>> Getprojectslistchange(string campus, string selectedValuesArray);
        Task<int> insertprojectchange(string Regdno, string stuname, string campus, string college, string mobile, string dept, string selectedValuesArray, string old_title_id, string title, string count, string domain, string selectedtitleVal);
        Task<Mac_details> getmac_details(string regdno);
        Task<int> getmacdetails_check(string macid, string Regdno);

        Task<int> InsertNewData_MYSQL(Mac_details data, string id1, string id2, string IPADdress);
        Task<int> UpdateNewData_MYSQL(Mac_details data, string IPADdress);
        Task<int> InsertMac_details(Mac_details data, string id1, string id2, string IPADdress);

        Task<int> updateMac_details(Mac_details data, string IPADdress);

        Task<int> updatemac_details(Mac_details data, string id1, string id2, string IPADdress);

        //Task<int> getmacdetails_check(string macid, string Regdno);
        Task<IEnumerable<EventAttendance>> Getevent_attendance(string regdno,string semester);
        Task<int> InsertGlearnhistory(string regdno, string campus, string col);

        Task<Userlogin> Getglearnhistory(string regdno, string campus, string col);

        Task<IEnumerable<Students>> gettxndatatutionbalance(string regdno, string sem);


        Task<IEnumerable<Students>> getTBL_ALLOW_STUDENTS(string regdno, string sem);


        Task<Userlogin> get74user(string regdno);

    }

}
