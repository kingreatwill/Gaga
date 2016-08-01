
namespace Gaga.Core.Utilities
{
	public class ColorConverter
	{
		#region [颜色：16进制转成RGB]
		/// <summary>
		/// [颜色：16进制转成RGB]
		/// </summary>
		/// <param name="strColor">设置16进制颜色 [返回RGB]</param>
		/// <returns></returns>
		public static System.Drawing.Color ConvertoHx16toRGB(string strHxColor)
		{
			try
			{
				if (strHxColor.Length == 0)
				{//如果为空
					return System.Drawing.Color.FromArgb(0, 0, 0);//设为黑色
				}
				else
				{//转换颜色
					return System.Drawing.Color.FromArgb(System.Int32.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier), System.Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
				}
			}
			catch
			{//设为黑色
				return System.Drawing.Color.FromArgb(0, 0, 0);
			}
		}
		#endregion

		#region [颜色：RGB转成16进制]
		/// <summary>
		/// [颜色：RGB转成16进制]
		/// </summary>
		/// <param name="R">红 int</param>
		/// <param name="G">绿 int</param>
		/// <param name="B">蓝 int</param>
		/// <returns></returns>
		public static string ConverterRGBtoHx16(int R, int G, int B)
		{
			return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(R, G, B));
		}
		#endregion
	}
}
