using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace FixerDotNetCore.Domain.Models
{
    public class Currency
    {
        [BsonId]
        public MongoDB.Bson.ObjectId _id { get; set; }
        public string Title { get; set; }
        string IsoAlpha3Code { get; set; }
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
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
