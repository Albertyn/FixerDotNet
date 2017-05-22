using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FixerDotNetCore
{
    public class FixerSettings : IFixerSettings
    {
        public FixerSettings() { }
        public string BaseUrl { get; set; }
        public string Currencies { get; set; }
        public Guid AppSecret { get; set; }
    }
}
