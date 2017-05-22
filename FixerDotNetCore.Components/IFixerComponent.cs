using FixerDotNetCore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FixerDotNetCore.Components
{
    public interface IFixerComponent
    {
        Task<Fixer> Get(string @base);

        Task Add(Fixer F);

        Task<List<Fixer>> ListFixesByDate(DateTime Date);
        Task<Fixer> GetFixerLatest(string Base);


        //void Update(Fixer F);
        //void Delete(Fixer F);
    }
}
