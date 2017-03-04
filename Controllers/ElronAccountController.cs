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

        [HttpGet]
        public IEnumerable<ElronAccount> GetAll()
        {
            return _elronAccountsRepo.GetAll();
        }

        [HttpGet("{id}")]
        [Route("GetAccount")]
        public IActionResult GetById(string id)
        {
            var item = _elronAccountsRepo.Find(id);
            if (item == null)
            {
                return new JsonResult(new { });
            }
            return new ObjectResult(item);
        }

        [HttpGet("add/{id}")]
        public IActionResult AddById(string id)
        {
            bool success = false;
            try
            {
                _elronAccountsRepo.Add(new ElronAccount(){ Number = id });
                success = true;
            }
            catch (Exception ex)
            {

            }
            return new JsonResult(new { error = !success });
        }
    }
}