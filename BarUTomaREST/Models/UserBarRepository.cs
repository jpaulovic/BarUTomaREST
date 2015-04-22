using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class UserBarRepository : Repository<UserBar>
    {
        public UserBarRepository(DbContext db) : base(db)
        {
        }
    }
}