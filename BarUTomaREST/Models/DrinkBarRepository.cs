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
        public DrinkRepository DrinkRepository { get; set; }

        public DrinkBarRepository(DbContext db) : base(db)
        {
        }

        public void AddDrinkToBar(Bar bar, DrinkBar drinkBar)
        {
            if (drinkBar.Drink == null)
            {
                Drink drink = new Drink() {Bar = bar, Name = drinkBar.Name, Price = drinkBar.Price, Info = drinkBar.Info};
                drinkBar.Drink = drink;
                drinkBar.Drink.BarsThatHaveThisDrink = new List<DrinkBar> {drinkBar};
                drink.BarsThatHaveThisDrink = new List<DrinkBar> {drinkBar};
            }
            if ((bar.DrinksOnBar.Contains(drinkBar)))
            {
                throw new InvalidOperationException("drinkBar");
            }
            if (!bar.Drinks.Contains(drinkBar.Drink))
            {
                bar.Drinks.Add(drinkBar.Drink);
            }
            bar.DrinksOnBar.Add(drinkBar);
            drinkBar.Bar = bar;
            Add(drinkBar);
            Save();
        }

        public void DeleteDrinkFromBar(Bar bar, DrinkBar drinkBar)
        {
            if (!bar.DrinksOnBar.Contains(drinkBar))
            {
                throw new InvalidOperationException("drinkBar");
            }
            bar.DrinksOnBar.Remove(drinkBar);
            Save();
        }

        public DrinkBar Find(Bar bar, Drink drink)
        {
            return db.Set<DrinkBar>().First(x => x.Bar.BarId.Equals(bar.BarId) && x.Drink.DrinkId.Equals(drink.DrinkId));
        }

        public DrinkBar ModifyDrink(DrinkBar existingDrinkBar, DrinkBar newDrinkBar)
        {
            if (newDrinkBar.Info != null)
            {
                existingDrinkBar.Info = newDrinkBar.Info;
            }
            if (newDrinkBar.Name != null)
            {
                existingDrinkBar.Name = newDrinkBar.Name;
            }
            if (newDrinkBar.Price != null)
            {
                existingDrinkBar.Price = newDrinkBar.Price;
            }

            Save();
            return existingDrinkBar;
        }
    }
}