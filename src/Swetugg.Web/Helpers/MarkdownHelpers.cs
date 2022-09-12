using Markdig;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Swetugg.Web.Helpers
{
	public static class MarkdownHelpers
	{
		public static string FromMarkdown<T>(this IHtmlHelper<T> helper, string markdown)
		{
			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.Build();
			return Markdown.ToHtml(markdown, pipeline);
		}
	}
}