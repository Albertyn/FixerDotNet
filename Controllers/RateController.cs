using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using FixerDotNet.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FixerDotNet.Controllers
{
    public class RateController : Controller
    {
        private DataStore context = new DataStore();
        private HttpClient client = new HttpClient();

        // TODO : use DI 
        IMongoClient _client = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase _database;

        public RateController()
        {
            client.BaseAddress = new Uri("http://api.fixer.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _database = _client.GetDatabase("ExchangeRates");
        }
        private async Task<Fixer> GetFixerAsync(string IsoAlpha3Code)
        {
            HttpResponseMessage response = await client.GetAsync("latest?base=" + IsoAlpha3Code);
            return JsonConvert.DeserializeObject<Fixer>(await response.Content.ReadAsStringAsync());
        }
        private async Task SaveFix(Fixer fix)
        {
            var collection = _database.GetCollection<Fixer>("Fixes");
            await collection.InsertOneAsync(fix);
        }
        [HttpGet("api/Rate/Import")]
        public JsonResult ImportRates()
        {
            CommonResult result = new CommonResult();

            List<Fixer> fix = new List<Fixer>();
            foreach (string @base in context.IsoAlpha3Codes)
            //fix.Add(GetFixerAsync(@base).GetAwaiter().GetResult());
            {
                Fixer f = GetFixerAsync(@base).GetAwaiter().GetResult();
                fix.Add(f);
                SaveFix(f).Wait();
            }
            return new JsonResult(fix);
        }
        [HttpGet("api/Rates/Date/{Date:datetime}")]
        public async Task<JsonResult> GetRatesByDateAsync(DateTime Date)
        {
            IMongoCollection<Fixer> collection = _database.GetCollection<Fixer>("Fixes");
            string _date = Date.ToString("yyyy-MM-dd");
            var result = collection.Find<Fixer>(x => x.date == _date);
            List<Fixer> list = await result.ToListAsync<Fixer>();
            return new JsonResult(list, new JsonSerializerSettings() { Formatting = Formatting.Indented });
        }
        [HttpGet("api/Rates/Date/{Date:datetime}/Base/{}")]
        public async Task<JsonResult> GetIsoRatesByDateAsync(string IsoAlpha3Code, DateTime Date)
        {
            IMongoCollection<Fixer> collection = _database.GetCollection<Fixer>("Fixes");
            string _date = Date.ToString("yyyy-MM-dd");
            var filter = Builders<Fixer>.Filter.Eq("date", _date);
            Dictionary<string, object> list = new Dictionary<string, object>();
            var count = 0;
            using (var cursor = await collection.FindAsync(filter))
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    {
                        // process document
                        count++;
                        list.Add(document._id.Timestamp.ToString(), 
                            new { currency = document.@base, date = document.date });
                    }
                }
                return new JsonResult(list, new JsonSerializerSettings() { Formatting = Formatting.Indented });
            }

        }
        [HttpGet("api/Rates")]
        public JsonResult GetRates()
        {
            return new JsonResult(DataStore.Moc.FixerList);
        }
    }
}
