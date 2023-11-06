using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_functions;

public static class Current
{
    [FunctionName("Current")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "current")] HttpRequest req, ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
        TableClient tableClient = tableServiceClient.GetTableClient("functions");
        
        var data = tableClient.Query<FunctionsEntry>(filter: $"RowKey eq '0'").First();

        Function[] functions = JsonConvert.DeserializeObject<Function[]>(data.Functions);
        Function nextFunc = functions[data.Current];
        
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return new JsonResult(nextFunc);
    }
}