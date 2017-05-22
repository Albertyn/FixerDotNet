using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using FixerDotNetCore.Domain.Models;
using Newtonsoft.Json;
using FixerDotNetCore.Components;

namespace FixerDotNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Convert")]
    public class ConvertController : Controller
    {
        private readonly IFixerComponent fixerMongoDbComponent;
        public ConvertController(IFixerComponent service)
        {
            fixerMongoDbComponent = service;
        }
        
        [HttpGet("{Base}/{Amount:double}/{Target}")]
        public async Task<JsonResult> ConvertAsync(string @base, double amount, string target, DateTime? date = null)
        {
            try
            {
                Fixer fixer = await fixerMongoDbComponent.GetFixerLatest(@base);
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
        
    }
}
