using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Doghouse.Services
{
    public class GeneralExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GeneralExceptionFilter> _logger;

        public GeneralExceptionFilter(ILogger<GeneralExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {

            var exceptionMessage = context.Exception.Message;
            var exceptionStackTrace = context.Exception.StackTrace;

            _logger.LogError(context.Exception, "An unexpected error occurred: {Message}", exceptionMessage);
            _logger.LogInformation("Exception stack trace: {StackTrace}", exceptionStackTrace);


            context.Result = new ObjectResult($"An unexpected error occurred: {exceptionMessage}")
            {
                StatusCode = 500 
            };

            context.ExceptionHandled = true; 
        }

    }
}
