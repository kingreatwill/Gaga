using Gaga.Core.Utilities;
using System;
using System.Web;
using UAParser;

namespace Gaga.Core.Extensions
{
	public static class HttpContextBaseExtensions
	{
		/// <summary>
		/// 判断是否在微信内置浏览器中
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool SideInWeixinBroswer(this HttpContextBase httpContext)
		{
			var userAgent = httpContext.Request.UserAgent;

			return UserAgentHelper.SideInWeixinBroswer(userAgent);

		}

		// <summary>
		/// 是否手机 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool IsMobile(this HttpContextBase httpContext)
		{
			var userAgent = httpContext.Request.UserAgent;
			return UserAgentHelper.IsMobile(userAgent);
		}

		// <summary>
		/// 是否平板
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool IsTablet(this HttpContextBase httpContext)
		{
			var userAgent = httpContext.Request.UserAgent;
			return UserAgentHelper.IsTablet(userAgent);
		}
		

	}
}
