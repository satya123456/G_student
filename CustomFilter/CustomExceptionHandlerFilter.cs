using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gstudent.CustomFilter
{
    public class CustomExceptionHandlerFilter:FilterAttribute,IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;

           
           
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult()
            {
                ViewName = "~/Home/Error"
            };
        }


    }

  
}