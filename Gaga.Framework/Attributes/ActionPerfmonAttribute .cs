using System;

using System.Web.Mvc;

namespace Gaga.Framework.Attributes
{
	/// <summary>
	/// 控制模块或者页面同时访问人数
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class ActionPerfmonAttribute : ActionFilterAttribute
	{
		private string ModuleName;
		private string LoginUserName;
		public ActionPerfmonAttribute(string moduleName)
		{
			this.ModuleName = moduleName;
			this.LoginUserName = "";// web.BLL.AdminBusiness.AdminService.CurrentLoginUserName();
		}
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var filer = true;//new DbPerfmon().PagePush(ModuleName, LoginUserName);
			if (filer)
				base.OnActionExecuting(filterContext);
			else
			{
				ContentResult cr = new ContentResult();
				//cr.Content = "<script>window.location.href='" + HttpContext.Current.Request.UrlReferrer.OriginalString + "';</script>";
				cr.Content = "<p style='color:Red;font-weight:bold;clear:both'>同时操作此业务人员较多，请稍候再试。</p>";
				filterContext.Result = cr;
			}

		}
		public class ExceptionFilterAttribute : HandleErrorAttribute
		{
			public override void OnException(ExceptionContext filterContext)
			{
				base.OnException(filterContext);
				ContentResult cr = new ContentResult();
				//cr.Content = "<script>window.location.href='" + HttpContext.Current.Request.UrlReferrer.OriginalString + "';</script>";
				cr.Content = "<p style='color:Red;font-weight:bold;clear:both'>此功能页面异常，请联学系校管理员或稍候再试。</p>";
				filterContext.Result = cr;
			}
		}
		public override void OnResultExecuted(ResultExecutedContext filterContext)
		{
			base.OnResultExecuted(filterContext);
			//new DbPerfmon().PageRemove(LoginUserName);
		}
	}
}
