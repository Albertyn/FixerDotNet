using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FixerDotNetCore.Domain.Models;
using Newtonsoft.Json;

namespace FixerDotNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Convert")]
    public class ConvertController : Controller
    {
        IMongoClient _MongoClient = new MongoClient("mongodb://localhost:27017");
        IMongoDatabase _MongoDatabase;
        IMongoCollection<Fixer> _MongoCollection; 

        public ConvertController()
        {
            _MongoDatabase = _MongoClient.GetDatabase("ExchangeRates");
            _MongoCollection = _MongoDatabase.GetCollection<Fixer>("Fixes");
        }
        // GET: api/Convert
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "test_value1", "test_value2" };
        }

        [HttpGet("{Base}/{Amount:double}/{Target}")]
        public async Task<JsonResult> ConvertAsync(string @base, double amount, string target, DateTime? date = null)
        {
            try
            {
                Fixer fixer = await GetFixerLatest(@base);
                double rate = fixer.rates.Where(r => r.Key == target).Select(r => r.Value).SingleOrDefault();
                double data = amount * rate;
                return new JsonResult(data, new JsonSerializerSettings() { Formatting = Formatting.Indented });
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
        }

        private async Task<Fixer> GetFixerLatest(string Base)
        {
            //FilterDefinition<Fixer> filter = Builders<Fixer>.Filter.Eq(x => x.@base, Base);
            //SortDefinition<Fixer> sort = Builders<Fixer>.Sort.Descending(x => x.date);
            //var result = collection.Find(filter).Sort(sort).Limit(1);

            //IMongoCollection<Fixer> collection = _database.GetCollection<Fixer>("Fixes");

            var result = _MongoCollection.Find(x => x.@base.ToUpper() == Base.ToUpper())
                .SortByDescending(x => x.date).Limit(1);

            return await result.SingleAsync<Fixer>();
        }
    }
}
