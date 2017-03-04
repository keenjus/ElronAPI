using System;
using System.Collections.Generic;
using ElronAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElronAPI.Controllers
{
    [Route("api/[controller]")]
    public class ElronAccountController : Controller
    {
        public IElronAccountRepository _elronAccountsRepo { get; set; }
        public ElronAccountController(IElronAccountRepository elronAccountsRepo)
        {
            _elronAccountsRepo = elronAccountsRepo;
        }

        [HttpGet("{id}", Name = "GetAccount")]
        public IActionResult GetById(string id)
        {
            var item = _elronAccountsRepo.Find(id);
            if (item == null)
            {
                Response.StatusCode = 404;
                return new ObjectResult(new { error = true, message = "Failed to find the specified account." });
            }
            return new ObjectResult(item);
        }
    }
}