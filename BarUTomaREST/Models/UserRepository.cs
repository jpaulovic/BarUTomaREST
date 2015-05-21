using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class UserRepository : Repository<ApplicationUser>
    {
        public UserRepository(DbContext db) : base(db)
        {
        }

        public List<Bar> GetMyBars(ApplicationUser user)
        {
            UserRole ownerRole = new UserRole(1);

            List<int> myBarsIdList = db.Set<UserBar>().Where(x => x.User.Equals(user) && (x.UserRole.Role.Equals(ownerRole.Role))).Select(x => x.Bar.BarId).ToList();

            return db.Set<Bar>().Where(x => myBarsIdList.Contains(x.BarId)).ToList();
        }

        public ApplicationUser FindByName(string name)
        {
            return db.Set<ApplicationUser>().First(x => x.UserName.Equals(name));
        }
    }
}