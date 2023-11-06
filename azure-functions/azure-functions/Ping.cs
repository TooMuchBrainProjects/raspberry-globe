using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace azure_functions;

public static class Ping
{
    [FunctionName("Ping")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get",  Route = "ping")] HttpRequest req, ILogger log)
    {
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return new JsonResult(DateTime.Now);
    }
}