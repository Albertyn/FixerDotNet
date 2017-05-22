using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FixerDotNetCore.Controllers
{
    [Produces("application/json")]
    [Route("api/Settings")]
    public class SettingsController : Controller
    {
        IFixerSettings fixerSettings;
        public SettingsController(IOptions<FixerSettings> settings)
        {
            fixerSettings = settings.Value;
        }

        // GET: api/Settings
        [HttpGet]
        public IFixerSettings Get()
        {
            return fixerSettings;
        }
        // GET: api/Settings/Spread
        [HttpGet("Spread")]
        public string[] Spread()
        {
            string host = HttpContext.Request.Host.Value;
            Random random = new Random();
            List<string> L = new List<string>();

            foreach (string c in fixerSettings.Currencies.Split(','))
                foreach (string r in fixerSettings.Currencies.Split(','))
                    if (c != r) L.Add("http://" + host + "/api/Convert/" + c + "/" +
                        (random.NextDouble() * (1000 - 1) + 1).ToString(CultureInfo.InvariantCulture) + "/" + r);            

            return L.ToArray();
        }
    }
}
