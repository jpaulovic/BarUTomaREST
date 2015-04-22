using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class BottleRepository : Repository<Bottle>
    {
        public BottleRepository(DbContext db) : base(db)
        {
        }
    }
}