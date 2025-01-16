using Gstudent.Models;
using System.Collections.Generic;

namespace Gstudent.ViewModels
{
    // Common summary views
    public class permissions
    {
        public IEnumerable<Students> warden { get; set; }
        public IEnumerable<Students> Hostlers { get; set; }
    } 
}