using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Gaga.Framework.Attributes
{
	public class TimerActionFilterAttribute : ActionFilterAttribute
	{
		public string Message { get; set; }

		public override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			//在Action执行之后执行 输出到输出流中文字：After Action execute xxx
			//filterContext.HttpContext.Response.Write(@"<br />After Action execute" + "\t " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff"));

			GetTimer(filterContext, "action").Stop();

			base.OnActionExecuted(filterContext);
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//在Action执行前执行
			//filterContext.HttpContext.Response.Write(@"<br />Before Action execute" + "\t " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff"));

			GetTimer(filterContext, "action").Start();

			base.OnActionExecuting(filterContext);
		}

		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			//在Result执行之后
			//filterContext.HttpContext.Response.Write(@"<br />After ViewResult execute" + "\t " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffff"));

			var renderTimer = GetTimer(filterContext, "render");
			renderTimer.Stop();

			var actionTimer = GetTimer(filterContext, "action");
			var response = filterContext.HttpContext.Response;

			if (response.ContentType == "text/html")
			{
				response.Write(
					String.Format(
						"<p>Action '{0} :: {1}', Execute: {2}ms, Render: {3}ms.</p>",
						filterContext.RouteData.Values["controller"],
						filterContext.RouteData.Values["action"],
						actionTimer.ElapsedMilliseconds,
						renderTimer.ElapsedMilliseconds
					)
				);
			}

			base.OnResultExecuted(filterContext);
		}

		public override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			GetTimer(filterContext, "render").Start();

			base.OnResultExecuting(filterContext);
		}

		private Stopwatch GetTimer(ControllerContext context, string name)
		{
			string key = "__timer__" + name;
			if (context.HttpContext.Items.Contains(key))
			{
				return (Stopwatch)context.HttpContext.Items[key];
			}

			var result = new Stopwatch();
			context.HttpContext.Items[key] = result;
			return result;
		}

	}
}
