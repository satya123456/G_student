using Gstudent.CustomFilter;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gstudent
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
          
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ReferrerPolicyAttribute());
          
        }

        public class ReferrerPolicyAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                filterContext.HttpContext.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                base.OnResultExecuting(filterContext);
            }
        }
        public class SessionTimeoutAttribute : ActionFilterAttribute
        {
            
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                HttpContext ctx = HttpContext.Current;

                if (HttpContext.Current.Session["uid"] == null)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Login", controller = "Logout" }));
                    //return;
                }
                base.OnActionExecuting(filterContext);
            }
           
        }

    }
}

