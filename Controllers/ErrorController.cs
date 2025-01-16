using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gstudent.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult PageNotFoundError()
        {
            return View("Error");
        }

        public ActionResult UnauthorizedError()
        {
            return View("Error");
        }

        public ActionResult InternalServerError()
        {
            return View("Error");
        }

        public ActionResult GenericError()
        {
            return View("Error");
        }
    }
}