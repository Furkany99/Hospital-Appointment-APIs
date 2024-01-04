using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;



namespace Common.Exceptions
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionHandlingMiddleware> _logger;

		public ExceptionHandlingMiddleware(RequestDelegate next,ILogger<ExceptionHandlingMiddleware> logger)
        {
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (NotFoundException ex)
			{
				_logger.LogError(ex, "Resource not found.");

				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

				var response = new { error = "Resource not found" };
				await httpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response));
			}
			catch (BadRequestException ex)
			{
				_logger.LogError(ex, "Bad request.");

				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

				var response = new { error = "Bad request" };
				await httpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception has occurred.");

				httpContext.Response.ContentType = "application/json";
				httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

				var response = new { error = "An error occurred" };
				await httpContext.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(response));
			}
		}

		public class NotFoundException : Exception
		{
			public NotFoundException() : base() { }
			public NotFoundException(string message) : base(message) { }
			public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
		}

		public class BadRequestException : Exception
		{
			public BadRequestException() : base() { }
			public BadRequestException(string message) : base(message) { }
			public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
		}
	}
}
