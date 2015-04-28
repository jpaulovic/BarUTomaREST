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

        public void AddDrinkToBar(Bar bar, DrinkBar drinkBar)
        {
            if ((bar.DrinksOnBar.Contains(drinkBar)))
            {
                throw new InvalidOperationException("drinkBar");
            }
            bar.DrinksOnBar.Add(drinkBar);
            db.SaveChanges();
        }

        public void DeleteDrinkFromBar(Bar bar, DrinkBar drinkBar)
        {
            if (!bar.DrinksOnBar.Contains(drinkBar))
            {
                throw new InvalidOperationException("drinkBar");
            }
            bar.DrinksOnBar.Remove(drinkBar);
            db.SaveChanges();
        }

        public List<DrinkBar> ListAllDrinksOnBar(Bar bar)
        {
            return db.Set<DrinkBar>().Where(a => a.Bar.BarId.Equals(bar.BarId)).ToList();
        }

        
    }
}