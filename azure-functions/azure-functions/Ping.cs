using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Opendays;

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