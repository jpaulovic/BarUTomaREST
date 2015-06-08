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
        public BottleRepository(DbContext db)
            : base(db)
        {
        }

        public List<Bottle> ListBottlesOnBar(Bar bar)
        {
            return dbSet.Where(x => x.Bar.BarId.Equals(bar.BarId)).ToList();
        }

        public Bottle AddBottle(Bar bar, Ingredient ingredient, Quantity quantity)
        {
            Bottle bottle = new Bottle() { Bar = bar, Ingredient = ingredient, Quantity = quantity, BottleBought = DateTime.Now };
            if (ingredient.Bottles == null)
            {
                ingredient.Bottles = new List<Bottle>();
            }
            ingredient.Bottles.Add(bottle);

            return bottle;
        }
    }
}