

namespace Gaga.Core.Extensions
{
	public static class StructExtensions
	{
		public static bool IsNullOrDefault<T>(this T? value) where T : struct
		{
			return default(T).Equals(value.GetValueOrDefault());
		}
	}
}
