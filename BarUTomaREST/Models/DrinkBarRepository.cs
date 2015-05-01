﻿using System;
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

        public void AddDrinkToBar(Bar bar, DrinkBar drinkBar)
        {
            if ((bar.DrinksOnBar.Contains(drinkBar)))
            {
                throw new InvalidOperationException("drinkBar");
            }
            if (!bar.Drinks.Contains(drinkBar.Drink))
            {
                bar.Drinks.Add(drinkBar.Drink);
            }
            bar.DrinksOnBar.Add(drinkBar);
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
    }
}