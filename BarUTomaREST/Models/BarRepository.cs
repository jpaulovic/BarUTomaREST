using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using BarUTomaModels.Models;
using BarUTomaREST.Controllers;
using Microsoft.AspNet.Identity;

namespace BarUTomaREST.Models
{
    public class BarRepository : Repository<Bar>
    {
        public BarRepository(DbContext db)
            : base(db)
        {
        }

        public bool IsDrinkAvailable(Bar bar, Drink drink)
        {
            if (db.Set<DrinkBar>().Count(x => x.Bar == bar && x.Drink == drink) == 0) //drink is not offered at all
            {
                return false;
            }

            drink.IngredientsUsed.Select(x => new Tuple<Ingredient, Quantity>(x.Ingredient, x.Quantity)).ToList();

            foreach (var ingredient in drink.IngredientsUsed)
            {
                if (!bar.IngredientsAvailable.Any(x => x.Item1.Equals(ingredient.Ingredient)))
                {
                    return false;
                }
                var result = bar.IngredientsAvailable.Where(x => x.Item1.Equals(ingredient.Ingredient))
                    .Select(x => x.Item2.Amount)
                    .First() >= ingredient.Quantity.Amount;
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        public List<Tuple<Drink, bool>> ListBarDrinks(Bar bar)
        {
            return
                db.Set<DrinkBar>().Where(x => x.Bar.Equals(bar)).Select(x => new Tuple<Drink, bool>(x.Drink, IsDrinkAvailable(bar, x.Drink))).ToList();
        }

        public void AddNewBar(Bar bar, IIdentity user)
        {
            if (bar == null)
            {
                throw new ArgumentNullException("bar");
            }

            if (FindAll().Contains(bar))
            {
                throw new ArgumentException("Bar already exists!"); 
            }

            ApplicationUser newUser =
                db.Set<ApplicationUser>().First(x => x.Id.Equals(user.GetUserId()));

            UserBar userBar = new UserBar {User = newUser, Bar = bar, UserRole = new UserRole(UserBarRepository.ADMIN_ROLE)};
            userBar.UserRole.Users.Add(newUser);

            bar.Users.Add(userBar);

            bar.DateCreated = DateTime.Now;
            bar.DateModified = bar.DateCreated;

            Add(bar);
            Save();
        }

        public void EditBar(Bar editedBar)
        {
            if (editedBar == null)
            {
                throw new ArgumentNullException("bar");
            }

            Bar existingBar = FindByPK(editedBar.BarId);

            if (existingBar == null)
            {
                throw new ArgumentException("bar");
            }

            existingBar.Address = editedBar.Address;
            existingBar.BarType = editedBar.BarType;
            existingBar.Drinks = editedBar.Drinks;
            existingBar.DrinksOnBar = editedBar.DrinksOnBar;
            existingBar.Events = editedBar.Events;
            existingBar.Info = editedBar.Info;
            existingBar.IngredientsAvailable = editedBar.IngredientsAvailable;
            existingBar.Name = editedBar.Name;
            existingBar.Orders = editedBar.Orders;
            existingBar.Users = editedBar.Users;
            existingBar.DateModified = DateTime.Now;

            Save();
        }
    }
}