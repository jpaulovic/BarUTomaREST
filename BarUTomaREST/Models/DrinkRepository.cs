using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class DrinkRepository : Repository<Drink>
    {
        public DrinkRepository(DbContext db)
            : base(db)
        {
        }

        public List<DrinkBar> ListAllDrinksOnBar(Bar bar)
        {
            return db.Set<DrinkBar>().Where(a => a.Bar.BarId.Equals(bar.BarId)).ToList();
        }

        
    }
}