using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class QuantityRepository : Repository<Quantity>
    {
        public QuantityRepository(DbContext db) : base(db)
        {
        }
    }
}