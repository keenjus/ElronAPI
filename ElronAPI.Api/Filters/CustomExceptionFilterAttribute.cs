using ElronAPI.Api.Models;
using ElronAPI.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Net;

namespace ElronAPI.Api.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ValidationException validationException:
                    _logger.LogError(context.Exception, "Validation error");

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.Result = new JsonResult(new JsonErrorResponseModel()
                    {
                        Error = string.Join(", ", validationException.Errors.Select(x => x.ErrorMessage))
                    });
                    return;
                case ScrapeException scrapeException:
                    _logger.LogError(context.Exception, "Scraping error");

                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.HttpContext.Response.ContentType = "application/json";
                    context.Result = new JsonResult(new JsonErrorResponseModel()
                    {
                        Error = scrapeException.Message
                    });
                    return;
            }

            _logger.LogError(context.Exception, "Unhandled error");

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.HttpContext.Response.ContentType = "application/json";
            context.Result = new JsonResult(new JsonErrorResponseModel()
            {
                Error = context.Exception.Message
            });
        }
    }
}