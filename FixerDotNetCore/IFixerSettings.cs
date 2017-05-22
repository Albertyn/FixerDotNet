using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FixerDotNetCore
{
    public interface IFixerSettings
    {
        string BaseUrl { get; set; }
        string Currencies { get; set; }

        Guid AppSecret { get; set; }
    }
}
