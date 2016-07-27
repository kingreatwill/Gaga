
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using UAParser;

namespace Gaga.Core.Utilities
{
	public static class UserAgentHelper
	{
		// <summary>
		/// 判断是否在微信内置浏览器中
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool SideInWeixinBroswer(string userAgent)
		{
			if (string.IsNullOrEmpty(userAgent) || (!userAgent.Contains("MicroMessenger") && !userAgent.Contains("Windows Phone")))
			{
				//在微信外部
				return false;
			}
			//在微信内部
			return true;
		}

		// <summary>
		/// 是否手机 
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool IsMobile(string userAgent)
		{
			var uaParser= Parser.GetDefault();

			ClientInfo c = uaParser.Parse(userAgent);

			bool isMobileDevice =
						s_MobileOS.Contains(c.OS.Family) ||
						s_MobileBrowsers.Contains(c.UserAgent.Family) ||
						s_MobileDevices.Contains(c.Device.Family);
			return isMobileDevice;
		}

		// <summary>
		/// 是否平板
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool IsTablet(string userAgent)
		{
			var uaParser = Parser.GetDefault();

			ClientInfo c = uaParser.Parse(userAgent);


			bool isTablet =
						Regex.IsMatch(c.Device.Family, "iPad|Kindle Fire|Nexus 10|Xoom|Transformer|MI PAD|IdeaTab", RegexOptions.CultureInvariant) ||
						c.OS.Family == "BlackBerry Tablet OS";
			return isTablet;
		}

		public static bool IsPdfConverter(string userAgent)
		{			
			return s_pdfConverterPattern.IsMatch(userAgent);
		}

		/// <summary>
		/// 是否爬虫
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public static bool IsSpider(HttpContextBase httpContext)
		{
			var uaParser = Parser.GetDefault();

			ClientInfo c = uaParser.Parse(httpContext.Request.UserAgent);
			
			bool isBot = httpContext.Request.Browser.Crawler || c.Device.IsSpider;
			return isBot;
		}


		private static readonly Regex s_pdfConverterPattern = new Regex(@"wkhtmltopdf", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);


		#region Mobile UAs, OS & Devices

		private static readonly HashSet<string> s_MobileOS = new HashSet<string>
		{
			"Android",
			"iOS",
			"Windows Mobile",
			"Windows Phone",
			"Windows CE",
			"Symbian OS",
			"BlackBerry OS",
			"BlackBerry Tablet OS",
			"Firefox OS",
			"Brew MP",
			"webOS",
			"Bada",
			"Kindle",
			"Maemo"
		};

		private static readonly HashSet<string> s_MobileBrowsers = new HashSet<string>
		{
			"Android",
			"Firefox Mobile",
			"Opera Mobile",
			"Opera Mini",
			"Mobile Safari",
			"Amazon Silk",
			"webOS Browser",
			"MicroB",
			"Ovi Browser",
			"NetFront",
			"NetFront NX",
			"Chrome Mobile",
			"Chrome Mobile iOS",
			"UC Browser",
			"Tizen Browser",
			"Baidu Explorer",
			"QQ Browser Mini",
			"QQ Browser Mobile",
			"IE Mobile",
			"Polaris",
			"ONE Browser",
			"iBrowser Mini",
			"Nokia Services (WAP) Browser",
			"Nokia Browser",
			"Nokia OSS Browser",
			"BlackBerry WebKit",
			"BlackBerry", "Palm",
			"Palm Blazer",
			"Palm Pre",
			"Teleca Browser",
			"SEMC-Browser",
			"PlayStation Portable",
			"Nokia",
			"Maemo Browser",
			"Obigo",
			"Bolt",
			"Iris",
			"UP.Browser",
			"Minimo",
			"Bunjaloo",
			"Jasmine",
			"Dolfin",
			"Polaris",
			"Skyfire"
		};

		private static readonly HashSet<string> s_MobileDevices = new HashSet<string>
		{
			"BlackBerry",
			"MI PAD",
			"iPhone",
			"iPad",
			"iPod",
			"Kindle",
			"Kindle Fire",
			"Nokia",
			"Lumia",
			"Palm",
			"DoCoMo",
			"HP TouchPad",
			"Xoom",
			"Motorola",
			"Generic Feature Phone",
			"Generic Smartphone"
		};

		#endregion

	}

}
