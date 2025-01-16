

using Gstudent.Models;
using System.Threading.Tasks;


namespace Gstudent.Repositories.Interface
{
    public interface Iloginrepository
    {
        Task<Userlogin> getlogin(string userid, string password);
        Task<Userlogin> getbatchdetails(string userid);

        Task<Userlogin> getTBLSTUDENTLOGIN(string userid);
        Task<StudentPhoto> getStudentmasterphoto(string regno);

        Task<Userlogin> getloginbypass(string userid, string password);

        Task<Userlogin> insertlogincoursereg(string userid, string sem);
        Task<Userlogin> getTBLBacklogattendance(string userid);

        Task<Userlogin> getTBL_ATTENDENCE_STUDENTS(string regdno);


        Task<Userlogin> getcreditscount(string userid);

        Task<Userlogin> getmacagainststudent(string userid);
    }
}
