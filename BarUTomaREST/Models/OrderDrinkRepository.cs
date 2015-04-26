using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class OrderDrinkRepository : Repository<OrderDrink>
    {
        public OrderDrinkRepository(DbContext db) : base(db)
        {
        }
    }
}