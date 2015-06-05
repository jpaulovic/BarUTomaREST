using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using BarUTomaModels.Models;
using BarUTomaREST.Models;
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
                db.Set<ApplicationUser>().First(x => x.UserName.Equals(user.Name));

            UserBar userBar = new UserBar {User = newUser, Bar = bar, UserRole = UserBarRepository.ADMIN_ROLE};

            bar.Drinks = new List<Drink>();
            bar.DrinksOnBar = new List<DrinkBar>();
            bar.Events = new List<Event>();
            bar.IngredientsAvailable = new List<Tuple<Ingredient, Quantity>>();
            bar.Orders = new List<Order>();
            bar.Users = new List<UserBar>();
            bar.Users.Add(userBar);

            bar.DateCreated = DateTime.Now;
            bar.DateModified = bar.DateCreated;

            Add(bar);
            Save();
        }

        public Bar EditBar(int id, Bar editedBar)
        {
            if (editedBar == null)
            {
                throw new ArgumentNullException("bar");
            }

            Bar existingBar = FindByPK(id);

            if (existingBar == null)
            {
                throw new ArgumentException("bar");
            }

            if (editedBar.Address != null)
            {
                existingBar.Address = editedBar.Address;
            }
            if (editedBar.BarType != null)
            {
                existingBar.BarType = editedBar.BarType;
            }
            if (editedBar.Drinks != null)
            {
                existingBar.Drinks = editedBar.Drinks;
            }
            if (editedBar.DrinksOnBar != null)
            {
                existingBar.DrinksOnBar = editedBar.DrinksOnBar;
            }
            if (editedBar.Events != null)
            {
                existingBar.Events = editedBar.Events;
            }
            if (editedBar.Info != null)
            {
                existingBar.Info = editedBar.Info;
            }
            if (editedBar.Name != null)
            {
                existingBar.Name = editedBar.Name;
            }
            if (editedBar.IngredientsAvailable != null)
            {
                existingBar.IngredientsAvailable = editedBar.IngredientsAvailable;
            }
            if (editedBar.Orders != null)
            {
                existingBar.Orders = editedBar.Orders;
            }
            if (editedBar.Users != null)
            {
                existingBar.Users = editedBar.Users;
            }
            existingBar.DateModified = DateTime.Now;

            Save();

            return existingBar;
        }
    }
}