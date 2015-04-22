using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class UnitRepository : Repository<Unit>
    {
        public UnitRepository(DbContext db) : base(db)
        {
        }
    }
}