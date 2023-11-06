using System;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_functions;

public static class NextFunction
{
    [FunctionName("NextFunction")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "next")] HttpRequest req, ILogger log)
    {
        var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
        TableServiceClient tableServiceClient = new TableServiceClient(connectionString);
        TableClient tableClient = tableServiceClient.GetTableClient("functions");
        
        var data = tableClient.Query<FunctionsEntry>(filter: $"RowKey eq '0'").FirstOrDefault();
        if (data == null)
        {
           data = new FunctionsEntry()
           {
               PartitionKey = "rg",
               RowKey = "0",
               Functions = JsonConvert.SerializeObject(new Function[] {new Function() { Url = "func1", Key = "key1"}, new Function() { Url = "func2", Key = "key2"}}),
               Current = 0,
           };
            await tableClient.AddEntityAsync(data);
        }

        Function[] functions = JsonConvert.DeserializeObject<Function[]>(data.Functions);
        
        data.Current += 1;
        if (data.Current >= functions.Length)
            data.Current = 0;
        
        Function nextFunc = functions[data.Current]; 
        
        await tableClient.UpdateEntityAsync(data, ETag.All, TableUpdateMode.Replace);
        
        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        return new JsonResult(nextFunc);
    }
}

public class FunctionsEntry : ITableEntity
{
    public string Functions { get; set; }
    public int Current { get; set; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

public class Function
{
    public string Url { get; set; }
    public string Key {get; set; }
}