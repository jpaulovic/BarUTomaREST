using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BarUTomaREST.Models
{
    public class IngredientDrinkRepository : Repository<IngredientRepository>
    {
        public IngredientDrinkRepository(DbContext db) : base(db)
        {
        }
    }
}