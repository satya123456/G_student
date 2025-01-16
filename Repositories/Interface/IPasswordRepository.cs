using Gstudent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gstudent.Repositories.Interface
{
    public interface IPasswordRepository
    {
        Task<Userlogin> getEmplogin(string userid, string password);
        Task<IEnumerable<Hallticket>> getStudentsdatahallget(string regno);
        Task<Userlogin> getEmplogindetails(string userid);
        Task<Students> getStudentsdata(string regno);
        Task<int> UpdateStudentPassword(string studentid, string userid, string password);
        Task<IEnumerable<Students>> GetSearchAutocompletestudent(string SearchName, string check);

    }
}
