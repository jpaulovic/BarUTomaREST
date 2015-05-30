using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using Microsoft.Owin.Security;

namespace BarUTomaREST.Models
{
    public class BottleRepository : Repository<Bottle>
    {
        public BottleRepository(DbContext db) : base(db)
        {
        }

        public List<Bottle> ListBottlesOnBar(Bar bar)
        {
            return dbSet.Where(x => x.Bar.BarId.Equals(bar.BarId)).ToList();
        }

        //public void AddBottle(Bar bar, Bottle bottle) //riesit priamo v controlleri
    }
}