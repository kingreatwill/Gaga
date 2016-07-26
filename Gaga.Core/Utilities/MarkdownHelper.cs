

namespace Gaga.Core.Utilities
{
	public static class MarkdownHelper
	{
		public static string MarkdownReplace(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var markdown = new MarkdownSharp.Markdown();

			return markdown.Transform(text);
		}
	}
}
