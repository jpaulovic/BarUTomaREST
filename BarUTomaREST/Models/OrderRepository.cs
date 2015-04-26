using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class OrderRepository : Repository<Order>
    {
        public OrderRepository(DbContext db) : base(db)
        {
        }
    }
}