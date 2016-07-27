
using System.IO;

namespace Gaga.Core.Extensions
{
	public static class StringFileExtensions
	{
		/*
		1     @"C:\Directory".CreateDirectory();
2     @"C:\Directory\readme.txt".WriteText("this file is created by string!");
3     @"C:\abc.txt".DeleteFile();
			*/
		public static void CreateDirectory(this string path)
		{
			Directory.CreateDirectory(path);
		}
		public static void WriteText(this string path, string contents)
		{
			File.WriteAllText(path, contents);
		}
		public static void DeleteFile(this string path)
		{
			if (File.Exists(path)) File.Delete(path);
		}
	}
}
