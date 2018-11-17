using ElronAPI.Api.Data;
using ElronAPI.Application.ElronAccount.Queries;
using ElronAPI.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ElronAPI.Api.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : BaseController
    {
        [HttpGet]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new ElronAccountQuery() { Id = id };

            try
            {
                return Json(await Mediator.Send(query));
            }
            catch (ValidationException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ex.Message, "text/plain");
            }
            catch (ScrapeException ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Content(ex.Message, "text/plain");
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content(ex.Message, "text/plain");
            }
        }
    }
}