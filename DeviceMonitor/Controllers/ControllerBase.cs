using System;
using System.Web.Mvc;
using NLog;

namespace DeviceMonitor.Controllers
{
    public abstract class ControllerBase : Controller
    {
        public static Logger Log = LogManager.GetLogger("ReflectInsight");
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                base.OnException(null);
                return;
            }
            Log.Debug(filterContext.Exception.Message);
            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;
                View("Error").ExecuteResult(ControllerContext);
            }
        }

        
    }
}