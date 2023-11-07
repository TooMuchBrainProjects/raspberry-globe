using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azure_functions;

public static class Location
{
    [FunctionName("Location")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "loc")] HttpRequest req, ILogger log)
    {
        try
        {
            var ipInfo = await IPInfo.GetIpInfo();
            req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            return new JsonResult(ipInfo);
        }
        catch
        {
            return new InternalServerErrorResult();
        }
    }
}
public class IPInfo
{
    public string IP { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string Loc { get; set; }
    public string Org { get; set; }
    public string Postal { get; set; }
    public string Timezone { get; set; }
    
    public static async Task<IPInfo> GetIpInfo() {
        HttpClient client = new HttpClient();
        
            
        
        string ipInfoapiUrl = "https://ipinfo.io";
        HttpResponseMessage response = await client.GetAsync(ipInfoapiUrl);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("ipinfo not available");
        }
        
        string responseBody = await response.Content.ReadAsStringAsync();
        IPInfo ipInfo = JsonConvert.DeserializeObject<IPInfo>(responseBody);
        
        return ipInfo;
    }
}

