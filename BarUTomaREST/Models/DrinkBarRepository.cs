using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class DrinkBarRepository : Repository<DrinkBar>
    {
        public DrinkBarRepository(DbContext db) : base(db)
        {
        }

        
    }
}