using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(DbContext db) : base(db)
        {
        }

        public List<Bar> GetMyBars(User user)
        {
            UserRole ownerRole = new UserRole {Role = 1};

            List<int> myBarsIdList = db.Set<UserBar>().Where(x => x.User.Equals(user) && (x.UserRole.Equals(ownerRole))).Select(x => x.Bar.BarId).ToList();

            return db.Set<Bar>().Where(x => myBarsIdList.Contains(x.BarId)).ToList();
        }
    }
}