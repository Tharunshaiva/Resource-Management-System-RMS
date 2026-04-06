using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Final_v1.Middleware
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;

        private readonly String _logFilePath;


        public GlobalExceptionHandling(IConfiguration configuration, RequestDelegate next)

        {

            _next = next;

            _logFilePath = configuration["ErrorLogFile:Filepath"];

        }

        public async Task InvokeAsync(HttpContext context)

        {

            try

            {

                Console.WriteLine("A request has come");

                await _next(context);

                Console.WriteLine("A request has left");

            }

            catch (Exception ex)

            {

                var exceptionmessage = new ProblemDetails

                {

                    Detail = ex.Message,

                    Title = ex.GetType().Name,

                    Status = StatusCodes.Status500InternalServerError,

                    Type = "Sql Exception"

                };

                var exceptionbody = JsonConvert.SerializeObject(exceptionmessage);

                LogError(ex, context);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(exceptionbody);

            }

        }

        private void LogError(Exception ex, HttpContext context)

        {

            try

            {

                var logDirectory = Path.GetDirectoryName(_logFilePath);

                if (!Directory.Exists(logDirectory))

                {

                    Directory.CreateDirectory(logDirectory);

                }

                using (StreamWriter writer = new StreamWriter(_logFilePath, append: true))

                {

                    writer.WriteLine("----- Exception Log -----");

                    writer.WriteLine($"Timestamp: {DateTime.UtcNow}");

                    writer.WriteLine($"Request Path: {context.Request.Path}");

                    writer.WriteLine($"Request Method: {context.Request.Method}");

                    writer.WriteLine($"Status code : {StatusCodes.Status500InternalServerError}");

                    writer.WriteLine($"Exception Message: {ex.Message}");

                    writer.WriteLine($"Exception Type: {ex.GetType().Name}");

                    writer.WriteLine("-------------------------\n\n");

                }

            }

            catch (Exception)

            {
                throw new Exception("Error logging failed");
            }
        }
    }
}
