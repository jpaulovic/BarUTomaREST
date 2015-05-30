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

        public void NewOrder(Bar bar, ApplicationUser loggedUser, List<Tuple<int, int>> orderList)
        {
            Unit priceUnit = db.Set<Unit>().First(x => x.UnitId.Equals(loggedUser.DefaultPriceUnitId));
            Order newOrder = new Order() {Bar = bar, DateTime = DateTime.Now, User = loggedUser,
                Place = bar.Name, Price = new Quantity(){Amount = 0, Unit = priceUnit}};

            foreach (var item in orderList)
            {
                Drink drink = db.Set<Drink>().First(x => x.DrinkId.Equals(item.Item1));
                DrinkBar drinkBar =
                    db.Set<DrinkBar>()
                        .First(x => x.Drink.DrinkId.Equals(drink.DrinkId) && x.Bar.BarId.Equals(bar.BarId));
                Unit ks = db.Set<Unit>().First(x => x.Code.Equals("ks"));
                Quantity quantity = new Quantity(){Amount = item.Item2, Unit = ks};
                OrderDrink orderDrink = new OrderDrink() {Drink = drink, Order = newOrder, Quantity = quantity};
                newOrder.OrderDrinks.Add(orderDrink);
                newOrder.Price.Amount += orderDrink.Order.Price.Amount/(decimal)priceUnit.MultiplierToBase;
            }
            Save();
        }
    }
}