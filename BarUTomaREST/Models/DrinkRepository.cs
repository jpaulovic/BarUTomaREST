using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class DrinkRepository : Repository<Drink>
    {
        public DrinkRepository(DbContext db)
            : base(db)
        {
        }

        // /bar/{id}/drink POST
        public void AddDrinkToBar(Bar bar, DrinkBar drinkBar)
        {
            if ((bar.DrinksOnBar.Contains(drinkBar)))
            {
                throw new InvalidOperationException("drinkBar");
            }
            bar.DrinksOnBar.Add(drinkBar);
        }

        // /bar/{id}/drink DELETE
        public void DeleteDrinkFromBar(Bar bar, DrinkBar drinkBar)
        {
            if (!bar.DrinksOnBar.Contains(drinkBar))
            {
                throw new InvalidOperationException("drinkBar");
            }
            bar.DrinksOnBar.Remove(drinkBar);
        }

        // /bar/{id}/drink GET
        public List<DrinkBar> ListAllDrinksOnBar(Bar bar)
        {
            return db.Set<DrinkBar>().Where(a => a.Bar.Equals(bar)).ToList();
        }

        
    }
}