using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElronAPI.Api.Controllers
{
    [Route("privacy")]
    public class PrivacyPolicyController : Controller
    {
        private readonly ILogger<PrivacyPolicyController> _logger;

        public PrivacyPolicyController(ILogger<PrivacyPolicyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Someone visited the privacy policy page!");
            return View();
        }
    }
}
