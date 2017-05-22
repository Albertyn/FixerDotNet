using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FixerDotNetCore.Domain.Models;
using FixerDotNetCore.Components;
using FixerDotNetCore.Components.Repositories;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FixerDotNetCore.Controllers
{
    //[Produces("application/json")]
    [Route("api/Rates")]
    public class RatesController : Controller
    {
        private readonly IFixerComponent fixerMongoDbComponent;
        private readonly IFixerSettings _FixerSettings;
        private HttpClient _HttpClient = new HttpClient();
        public RatesController(IOptions<FixerSettings> settings, IFixerComponent service)
        {
            _FixerSettings = settings.Value;

            _HttpClient.BaseAddress = new Uri(_FixerSettings.BaseUrl);
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            fixerMongoDbComponent = service;
        }

        // GET: api/Rates/Date/2017-05-15
        [HttpGet("Date/{Date:datetime}")]
        public async Task<JsonResult> GetRatesByDateAsync(DateTime date, string list = "")
        {
            var fixes = await fixerMongoDbComponent.ListFixesByDate(date);
            object data = null;
            switch (list.ToLower())
            {
                case "currencies":
                    data = fixes.Select(f => f.@base);
                    break;
                default:
                    data = fixes.Select(f => new { @base = f.@base, rates = f.rates });
                    break;
            }
            return new JsonResult(data, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }
                
        // POST: api/Rates/Import
        [HttpPost("Import")]
        public async Task<JsonResult> ImportRates(string inValue)
        {
            if (_FixerSettings.AppSecret.Equals(Guid.Parse(inValue)))
                try
                {
                    List<string> messages = new List<string>();
                    foreach (string @base in _FixerSettings.Currencies.Split(','))
                    {
                        if (await RequestFixer(@base)) messages.Add("["+@base+"]");                        
                    }
                    return new JsonResult(new CommonResult { Success = true, Message = "Rates Updated: " + string.Concat(messages.ToArray()) });
                }
                catch (Exception e)
                {
                    string m = e.Message;
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                        m += " > InnerException > " + e.Message;
                    }
                    return new JsonResult(new CommonResult { Success = false, Message = m });
                }
            else return new JsonResult(new CommonResult { Success = false, Message = "no inValue" });
        }

        private async Task<bool> RequestFixer(string IsoAlpha3Code)
        {
            HttpResponseMessage response = await _HttpClient.GetAsync("latest?base=" + IsoAlpha3Code);
            string responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                await fixerMongoDbComponent.Add(JsonConvert.DeserializeObject<Fixer>(responseString));
                return true;
            }
            else return false;
        }
    }
}
