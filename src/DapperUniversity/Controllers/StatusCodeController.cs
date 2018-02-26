using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace DapperUniversity.Controllers
{
    public class StatusCodeController : Controller
    {
        private readonly ILogger<StatusCodeController> _logger;

        public StatusCodeController(ILogger<StatusCodeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            var reExecute = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            _logger.LogInformation($"Unexpected Status Code: {statusCode}, OriginalPath: {reExecute.OriginalPath}");
            return View(statusCode);
        }
    }
}
