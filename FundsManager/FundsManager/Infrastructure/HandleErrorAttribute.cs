using FundsManager.Common;
using System;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;

namespace FundsManager.Infrastructure
{
    /// <summary>
    /// Apply this attribute to controller actions to log the exception via Trace.
    /// </summary>
    /// <remarks>
    /// If ExceptionHandled is true in context then no action will be taken.
    /// Marks ExceptionHandled to true.
    /// </remarks>
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Method,
        AllowMultiple = true,
        Inherited = true)]
    public class HandleErrorAttribute : System.Web.Mvc.HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                if (filterContext.Exception != null)
                {
                    Trace.TraceError(filterContext.Exception.ToString());
                    StringBuilder sb = new StringBuilder();
                    foreach(var item in filterContext.RouteData.Values)
                    {
                        sb.Append(item.Key).Append(": ").Append(item.Value).Append("\r\n");
                    }
                    ErrorUnit.WriteErrorLog(filterContext.Exception.ToString(), sb.ToString());
                }
                filterContext.ExceptionHandled = true;
            }
        }
    }
}