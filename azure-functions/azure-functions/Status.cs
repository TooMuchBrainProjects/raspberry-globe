using System;
using System.IO;
using System.Net;
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

namespace Opendays;


public static class Status
{
    [FunctionName("Status")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get" , Route = "status")] HttpRequest req, ILogger log)
    {
        HttpClient client = new HttpClient();
        
        string ipInfoapiUrl = "https://ipinfo.io";
        HttpResponseMessage response = await client.GetAsync(ipInfoapiUrl);
        if (!response.IsSuccessStatusCode)
            return new InternalServerErrorResult();
        string responseBody = await response.Content.ReadAsStringAsync();
        IPInfo ipInfo = JsonConvert.DeserializeObject<IPInfo>(responseBody);

        string[] splitted_gps = ipInfo.Loc.Split(',');
        string wheaterApiUrl= $"https://api.open-meteo.com/v1/forecast?latitude={splitted_gps[0]}&longitude={splitted_gps[1]}&timezone={ipInfo.Timezone}&&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,rain,showers,snowfall,weather_code,cloud_cover,wind_speed_10m";
        response = await client.GetAsync(wheaterApiUrl);
        if (!response.IsSuccessStatusCode)
            return new InternalServerErrorResult();
        responseBody = await response.Content.ReadAsStringAsync();
        ForecastData wheaterData= JsonConvert.DeserializeObject<ForecastData>(responseBody);

        req.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        var resp = new { ipInfo, wheaterData };
        return new JsonResult(resp);
    }
}

public struct IPInfo
{
    public string IP { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public string Loc { get; set; }
    public string Org { get; set; }
    public string Postal { get; set; }
    public string Timezone { get; set; }
}

public struct ForecastData
{
    
    [JsonProperty("current_units")]
    public Units CurrentUnits { get; set; }
    
    [JsonProperty("current")]
    public CurrentData Current { get; set; }
}

public struct Units
{
    [JsonProperty("time")]
    public string Time { get; set; }
    
    [JsonProperty("interval")]
    public string Interval { get; set; }
    
    [JsonProperty("temperature_2m")]
    public string Temperature2m { get; set; }
    
    [JsonProperty("relative_humidity_2m")]
    public string RelativeHumidity2m { get; set; }
    
    [JsonProperty("apparent_temperature")]
    public string ApparentTemperature { get; set; }
    
    [JsonProperty("is_day")]
    public string IsDay { get; set; }
    
    [JsonProperty("rain")]
    public string Rain { get; set; }
    
    [JsonProperty("showers")]
    public string Showers { get; set; }
    
    [JsonProperty("snowfall")]
    public string Snowfall { get; set; }
    
    [JsonProperty("weather_code")]
    public string WeatherCode { get; set; }
    
    [JsonProperty("cloud_cover")]
    public string CloudCover { get; set; }
    
    [JsonProperty("wind_speed_10m")]
    public string WindSpeed10m { get; set; }
}

public struct CurrentData
{
    [JsonProperty("time")]
    public string Time { get; set; }
    
    [JsonProperty("interval")]
    public int Interval { get; set; }
    
    [JsonProperty("temperature_2m")]
    public double Temperature2m { get; set; }
    
    [JsonProperty("relative_humidity_2m")]
    public int RelativeHumidity2m { get; set; }
    
    [JsonProperty("apparent_temperature")]
    public double ApparentTemperature { get; set; }
    
    [JsonProperty("is_day")]
    public int IsDay { get; set; }
    
    [JsonProperty("rain")]
    public double Rain { get; set; }
    
    [JsonProperty("showers")]
    public double Showers { get; set; }
    
    [JsonProperty("snowfall")]
    public double Snowfall { get; set; }
    
    [JsonProperty("weather_code")]
    public int WeatherCode { get; set; }
    
    [JsonProperty("cloud_cover")]
    public int CloudCover { get; set; }
    
    [JsonProperty("wind_speed_10m")]
    public double WindSpeed10m { get; set; }
    
    [JsonProperty("weather_description")]
    public string WeatherDescription
    {
        get { return WeatherCodeConverter.GetWeatherDescription(WeatherCode); }
    }

}