using Microsoft.AspNetCore.Mvc;

namespace ElronAPI.Api.Controllers
{
    [Route("privacy")]
    public class PrivacyPolicyController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
