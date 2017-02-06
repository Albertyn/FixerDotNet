using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FixerDotNet.Models
{

    public class DataStore
    {
        public readonly string[] IsoAlpha3Codes = { "AUD", "BGN", "BRL", "CAD", "CHF", "CNY", "CZK", "DKK", "GBP", "HKD", "HRK", "HUF", "IDR", "ILS", "INR", "JPY", "KRW", "MXN", "MYR", "NOK", "NZD", "PHP", "PLN", "RON", "RUB", "SEK", "SGD", "THB", "TRY", "USD", "ZAR" };
        public static DataStore Moc { get; } = new DataStore();

        public DataStore()
        {
            Dictionary<string, double> usd = new Dictionary<string, double>();
            usd.Add("GBP", 0.80837);
            usd.Add("ZAR", 13.949);
            usd.Add("EUR", 0.9596);

            Dictionary<string, double> eur = new Dictionary<string, double>();
            eur.Add("GBP", 0.8424);
            eur.Add("USD", 1.0421);
            eur.Add("ZAR", 14.5366);

            FixerList = new List<Fixer>()
            {
                new Fixer{ @base = "USD", date = "2016-12-21", rates = usd },
                new Fixer { @base = "EUR", date = "2016-12-21", rates = eur }
            };
        }

        public List<Fixer> FixerList { get; set; }

    }
        public class Country
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        string IsoAlpha2Code { get; set; }
        string IsoAlpha3Code { get; set; }
        string IsoNumberCode { get; set; }
        string Name { get; set; }
    }
    public class Fixer
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string @base { get; set; }
        public string date { get; set; }
        public Dictionary<string, double> rates { get; set; }
    }
    public class CommonResult
    {
        public string message { get; set; }
        public Boolean success { get; set; }
    }

}
