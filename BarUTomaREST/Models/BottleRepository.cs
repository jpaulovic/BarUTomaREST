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

        /// <summary>
        /// Add bottle of given repository and quantity to bar
        /// </summary>
        /// <param name="bar">Bar to which the bottle is added.</param>
        /// <param name="ingredient"></param>
        /// <param name="quantity">How many bottles to add.</param>
        /// <returns>The added bottle.</returns>
        public Bottle AddBottle(Bar bar, Ingredient ingredient, Quantity quantity)
        {
            Bottle bottle =
                db.Set<Bottle>().First(x => x.Bar.BarId.Equals(bar.BarId) && ingredient.IngredientId.Equals(ingredient.IngredientId));
            if (bottle == null)
            {
                bottle = new Bottle()
                {
                    Bar = bar,
                    Ingredient = ingredient,
                    Quantity = quantity,
                    BottleBought = DateTime.Now
                };
            }
            else
            {
                bottle.Quantity.Amount += quantity.Amount;
            }
            if (ingredient.Bottles == null)
            {
                ingredient.Bottles = new List<Bottle>();
            }
            ingredient.Bottles.Add(bottle);

            return bottle;
        }
    }
}