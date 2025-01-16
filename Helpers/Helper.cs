
using System;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GHMS.Helpers
{
    public class Helper
    {
        public static Task<T> GetOrSetCache<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {            
            if (!(MemoryCache.Default.Get(cacheKey) is T item))
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddDays(1));
            }
            return Task.FromResult<T>(item);
        }

        public static string RenderViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var ViewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var ViewContext = new ViewContext(controllerContext, ViewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, sw);
                ViewResult.View.Render(ViewContext, sw);
                ViewResult.ViewEngine.ReleaseView(controllerContext, ViewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }


}