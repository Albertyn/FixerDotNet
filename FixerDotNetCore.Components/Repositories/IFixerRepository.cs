using FixerDotNetCore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixerDotNetCore.Components.Repositories
{
    public interface IFixerRepository : IRepository<Fixer>
    {
        Task<Fixer> GetLatestFixer(string Base);
    }
}
