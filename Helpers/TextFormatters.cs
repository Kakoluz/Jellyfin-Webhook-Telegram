using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace TelegramWebhooks.Helpers
{
	public class TextPlainInputFormatter : TextInputFormatter
	{
		public TextPlainInputFormatter()
		{
			SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
			SupportedEncodings.Add(Encoding.UTF8);
		}

		protected override bool CanReadType(Type type)
		{
			return type == typeof(string);
		}

		public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
		{
			string data = await new StreamReader(context.HttpContext.Request.Body).ReadToEndAsync();
			return await InputFormatterResult.SuccessAsync(data);
		}
	}

	public class TextPlainOutputFormatter : TextOutputFormatter
	{
		public TextPlainOutputFormatter()
		{
			SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/plain"));
			SupportedEncodings.Add(Encoding.UTF8);
		}

		protected override bool CanWriteType(Type type)
		{
			return type == typeof(string);
		}

		public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding encoding)
		{
			string data = context.Object as string;
			await context.HttpContext.Response.WriteAsync(data);
		}
	}

}
