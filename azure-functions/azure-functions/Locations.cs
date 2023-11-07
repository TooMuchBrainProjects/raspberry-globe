using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_functions;

public static class Locations
{
    [FunctionName("Locations")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "locations")] HttpRequest req, ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
        TableClient tableClient = tableServiceClient.GetTableClient("functions");
        
        var data = tableClient.Query<FunctionsEntry>(filter: $"RowKey eq '0'").First();

        Function[] functions = JsonConvert.DeserializeObject<Function[]>(data.Functions);

        List<IPInfo> resp = new List<IPInfo>();
        foreach (var func in functions)
        {
            HttpClient hc = new HttpClient();
            hc.DefaultRequestHeaders.Add("x-functions-key", func.Key);
            HttpResponseMessage response = await hc.GetAsync(func.Url + "/api/loc");

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                IPInfo ipInfo = JsonConvert.DeserializeObject<IPInfo>(responseBody);
                resp.Add(ipInfo);
            }

        }
        
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return new JsonResult(resp);
    }
}