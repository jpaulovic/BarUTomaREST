﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{

    public class UserBarRepository : Repository<UserBar>
    {
        public const int ADMIN_ROLE = 1;
        public const int CUSTOMER_ROLE = 2;

        public UserBarRepository(DbContext db) : base(db)
        {
        }

        public bool OwnsUserBar(ApplicationUser user, Bar bar)
        {
            if (bar == null)
            {
                throw new ArgumentNullException("bar");
            }

            if (user == null)
            {
                return false;
            }

            var relation = dbSet.FirstOrDefault(x => x.Bar.BarId.Equals(bar.BarId) && x.User.Id.Equals(user.Id));
            if (relation == null)
            {
                return false;
            }
            return relation.UserRole == ADMIN_ROLE;
        }

        public List<Bar> GetMyBars(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return db.Set<UserBar>().Where(x => x.User.Id.Equals(user.Id) && x.UserRole.Equals(ADMIN_ROLE)).Select(x => x.Bar).ToList();
        }
    }
}