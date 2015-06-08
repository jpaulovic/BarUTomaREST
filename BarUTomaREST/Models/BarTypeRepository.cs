using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class BarTypeRepository : Repository<BarType>
    {
        public BarTypeRepository(DbContext db) : base(db)
        {
        }
    }
}