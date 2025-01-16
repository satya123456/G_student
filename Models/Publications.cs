using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gstudent.Models
{
    public class Publications
    {
        public string Regdno { get; set;}
        public string PUBLICATION_TITLE { get; set;}
        public string JOURNAL_NAME { get; set;}
        public string Approved_Status { get; set;}
        public string Author_type { get; set; }
        public string Volume { get; set; }
        public string article_title { get; set; }
        public string DOI_number { get; set; }
        public string Issue_number { get; set; }
        public string Page_numbers { get; set; }
        public string Issue_Year { get; set; }
        public string Indexed_by { get; set; }
        public string Citation_index { get; set; }
        public string Impact_factor { get; set; }
        public string Hyperlink { get; set; }
       public int flag { get; set; }
        public string stuname { get; set; }
        public string dept_code { get; set; }
        public string college_code { get; set; }
        public string campus_code { get; set; }
        public string batch { get; set; }
       
        public string publication_article { get; set; }
        public string publication_abstract { get; set; }

        public string msg { get; set; }
        public string absmsg { get; set; }
        public string articlemsg { get; set; }
    }
}