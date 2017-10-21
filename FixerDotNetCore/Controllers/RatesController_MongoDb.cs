using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FixerDotNetCore.Domain.Models;
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
        IOptions<FixerSettings> _FixerSettings;
        private HttpClient _HttpClient = new HttpClient();

        IMongoClient _MongoClient = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase _MongoDatabase;
        IMongoCollection<Fixer> _MongoCollection;

        public RatesController(IOptions<FixerSettings> settings)
        {
            _FixerSettings = settings;

            _HttpClient.BaseAddress = new Uri(_FixerSettings.Value.BaseUrl);
            _HttpClient.DefaultRequestHeaders.Accept.Clear();
            _HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _MongoDatabase = _MongoClient.GetDatabase("ExchangeRates");
            _MongoCollection = _MongoDatabase.GetCollection<Fixer>("Fixes");
        }
        
        // GET: api/Rates
        [HttpGet]
        public IEnumerable<string> Get() => new string[] { "foo", "bar" };


        [HttpGet("Date/{Date:datetime}")]
        public async Task<JsonResult> GetRatesByDateAsync(DateTime date, string list = "")
        {
            var fixes = await ListFixesByDate(date);
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
        private async Task<List<Fixer>> ListFixesByDate(DateTime Date)
        {
            string _date = Date.ToString("yyyy-MM-dd");
            var result = _MongoCollection.Find<Fixer>(x => x.date == _date);
            return await result.ToListAsync<Fixer>();
        }
                
        // POST: api/Rates/Import
        [HttpPost("Import")]
        public async Task<JsonResult> ImportRates(string inValue)
        {
            if (_FixerSettings.Value.AppSecret.Equals(Guid.Parse(inValue)))
                try
                {
                    //List<Fixer> fix = new List<Fixer>();
                    foreach (string @base in _FixerSettings.Value.Currencies.Split(','))
                    {
                        //Fixer f = GetFixerAsync(@base).GetAwaiter().GetResult();
                        //fix.Add(f);
                        //SaveFixerAsync(f).Wait();

                        await SaveFixerAsync(await GetFixerAsync(@base));

                        //Task<Fixer> F = GetFixerAsync(@base);
                        //Task S = F.ContinueWith(f => SaveFixerAsync(f.Result));
                    }
                    //return new JsonResult(fix);
                    return new JsonResult(new CommonResult { Success = true, Message = "Rates Updated!" });
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
        private async Task<Fixer> GetFixerAsync(string IsoAlpha3Code)
        {
            HttpResponseMessage response = await _HttpClient.GetAsync("latest?base=" + IsoAlpha3Code);
            string responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Fixer>(responseString);
        }
        private async Task SaveFixerAsync(Fixer fixer)
        {
            await _MongoCollection.InsertOneAsync(fixer);
        }
    }
}
