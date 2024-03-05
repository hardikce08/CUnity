using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
        //Task EnsureAdminIdentitiesAsync();
    }
}
