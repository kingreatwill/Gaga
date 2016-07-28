using System.Diagnostics;
using System.Web.Mvc;

namespace Gaga.Framework.ActionFilters
{
	public class TimerActionFilter : IActionFilter, IResultFilter
	{
		#region IActionFilter Members

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var controller = filterContext.Controller;
			if (controller != null)
			{
				var stopwatch = new Stopwatch();
				controller.ViewData["StopWatch"] = stopwatch;
				stopwatch.Start();
			}
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var controller = filterContext.Controller;
			if (controller != null)
			{
				var stopwatch = (Stopwatch)controller.ViewData["StopWatch"];
				stopwatch.Stop();
				controller.ViewData["Duration"] = stopwatch.Elapsed.TotalMilliseconds;
			}
		}

		#endregion IActionFilter Members

		#region IResultFilter Members

		public void OnResultExecuting(ResultExecutingContext filterContext)
		{
			//if (filterContext.Result is ViewResult)
			//{
			//	ResponseFilter responseFilter = new ResponseFilter(filterContext.HttpContext.Response.Filter, filterContext.HttpContext);
			//	responseFilter.Inserts.Add(
			//		new ResponseInsert(i => "<div>执行时间：{0}毫秒</div>".FormatWith(filterContext.Controller.ViewData["Duration"]),
			//			ResponseInsertMode.AppendTo, "body"));
			//	filterContext.HttpContext.Response.Filter = responseFilter;
			//}
		}

		public void OnResultExecuted(ResultExecutedContext filterContext)
		{
		}

		#endregion IResultFilter Members
	}
}