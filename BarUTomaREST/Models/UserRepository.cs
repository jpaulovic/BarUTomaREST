using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;
using BarUTomaServer.Models;

namespace BarUTomaREST.Models
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(DbContext db) : base(db)
        {
        }
    }
}