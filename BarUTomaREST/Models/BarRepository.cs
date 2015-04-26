using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class BarRepository : Repository<Bar>
    {
        ////chceme vsetko narvat do jedneho repozitara (tohto)? Neda sa to nejako "zanorit", kedze podla specifikacie by mali exostovat len 2 controllery (bar a user)?
        //^Nie je cela tato myslienka blbost a repozitarov moze byt viac ale budu len 2 controllery? :D Stacia 2 controllery? Ako to bude fungovat?
        public BarRepository(DbContext db)
            : base(db)
        {
        }

        public bool IsDrinkAvailable(Bar bar, Drink drink)
        {
            if (db.Set<DrinkBar>().Count(x => x.Bar == bar && x.Drink == drink) == 0) //drink is not offered at all
            {
                return false;
            }

            drink.IngredientsUsed.Select(x => new Tuple<Ingredient, Quantity>(x.Ingredient, x.Quantity)).ToList();

            foreach (var ingredient in drink.IngredientsUsed)
            {
                if (!bar.IngredientsAvailable.Any(x => x.Item1.Equals(ingredient.Ingredient)))
                {
                    return false;
                }
                var result = bar.IngredientsAvailable.Where(x => x.Item1.Equals(ingredient.Ingredient))
                    .Select(x => x.Item2.Amount)
                    .First() >= ingredient.Quantity.Amount;
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Tuple<Drink, bool>> ListBarDrinks(Bar bar)
        {
            return
                db.Set<DrinkBar>().Where(x => x.Bar.Equals(bar)).Select(x => new Tuple<Drink, bool>(x.Drink, IsDrinkAvailable(bar, x.Drink))).ToList();
        }

    }
}