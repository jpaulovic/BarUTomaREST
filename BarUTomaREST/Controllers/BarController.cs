using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using BarUTomaModels.Models;
using BarUTomaREST.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BarUTomaREST.Controllers
{
    [System.Web.Http.Authorize]
    public class BarController : BaseController
    {
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/")]
        public ActionResult GetAllBars()
        {
            List<Bar> bars = BarRepository.FindAll();
            return new JsonResult() { Data = bars };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/")]
        public ActionResult PostBar([FromBody] Bar newBar)
        {
            Bar existingBar = BarRepository.FindByPK(newBar.BarId);
            if (existingBar == null)
            {
                BarRepository.AddNewBar(newBar, User.Identity);
                return new JsonResult() { Data = newBar };
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, existingBar))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }
            Bar editedBar = BarRepository.EditBar(existingBar.BarId, newBar);
            return new JsonResult() { Data = editedBar };
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{id}")]
        public ActionResult DeleteBar(int id)
        {
            Bar barToDelete = BarRepository.FindByPK(id);
            if (barToDelete == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            if (!UserBarRepository.OwnsUserBar(LoggedUser, barToDelete))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }

            var userBars = UserBarRepository.FindAll().Where(x => x.Bar.BarId.Equals(id));

            foreach (var userBar in userBars)
            {
                UserBarRepository.Delete(userBar);
            }

            var drinkBars = DrinkBarRepository.FindAll().Where(x => x.Bar.BarId.Equals(id));

            foreach (var drinkBar in drinkBars)
            {
                DrinkBarRepository.Delete(drinkBar);
            }

            BarRepository.Delete(barToDelete);
            BarRepository.Save();

            return new HttpStatusCodeResult(200);
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{id}")]
        public ActionResult GetSpecificBar(int id)
        {
            var bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            return new JsonResult() { Data = bar };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/getMyBars")]
        public ActionResult GetMyBars()
        {
            if (LoggedUser == null)
            {
                return new HttpStatusCodeResult(401);
            }

            return new JsonResult() { Data = UserBarRepository.GetMyBars(LoggedUser) };
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{id}/drink")]
        public ActionResult GetDrinks(int id)
        {
            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            List<DrinkBar> drinkBars = DrinkRepository.ListAllDrinksOnBar(bar);
            return new JsonResult() { Data = drinkBars };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{id}/drink")]
        public ActionResult PostDrink(int id, [FromBody] DrinkBar drinkToAdd)
        {
            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }

            if (bar.BarId != drinkToAdd.Bar.BarId)
            {
                return new HttpStatusCodeResult(403, "You can only add drinks to a bar specified by ID in URL!");
            }

            DrinkBarRepository.AddDrinkToBar(bar, drinkToAdd);

            return new JsonResult() { Data = drinkToAdd };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/drink/{drinkId}")]
        public ActionResult PostModifyDrink(int barId, int drinkId, [FromBody] DrinkBar newDrinkBar)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }

            Drink drink = DrinkRepository.FindByPK(drinkId);
            if (drink == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified drink.");
            }

            DrinkBar drinkBar = DrinkBarRepository.Find(bar, drink);
            if (drinkBar == null)
            {
                return new HttpStatusCodeResult(404, "The specified drink is not available in this bar.");
            }

            DrinkBar modifiedDrinkBar = DrinkBarRepository.ModifyDrink(drinkBar, newDrinkBar);

            return new JsonResult() { Data = modifiedDrinkBar };
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/drink/{drinkId}")]
        public ActionResult GetSpecificDrink(int barId, int drinkId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            Drink drink = DrinkRepository.FindByPK(drinkId);
            if (drink == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified drink.");
            }
            DrinkBar drinkBar = DrinkBarRepository.Find(bar, drink);
            if (drinkBar == null)
            {
                return new HttpStatusCodeResult(404, "The specified drink is not available in this bar.");
            }
            return new JsonResult() { Data = drinkBar };
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{barId}/drink/{drinkId}")]
        public ActionResult DeleteSpecificDrink(int barId, int drinkId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            Drink drink = DrinkRepository.FindByPK(drinkId);
            if (drink == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified drink.");
            }
            DrinkBar drinkBar = DrinkBarRepository.Find(bar, drink);
            if (drinkBar == null)
            {
                return new HttpStatusCodeResult(404, "The specified drink is not available in this bar.");
            }
            DrinkBarRepository.Delete(drinkBar);
            DrinkBarRepository.Save();
            return new HttpStatusCodeResult(200);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order/{orderId}")]
        public ActionResult GetSpecificOrder(int barId, int orderId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            Order order = OrderRepository.FindByPK(orderId);
            if (order == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified order.");
            }

            if (!bar.Orders.Contains(order))
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified order.");
            }

            if (UserBarRepository.OwnsUserBar(LoggedUser, bar)) return new JsonResult() { Data = order };

            if (!LoggedUser.Orders.Contains(order))
            {
                return new HttpStatusCodeResult(403, "This order is not assigned to the current user.");
            }

            return new JsonResult() { Data = order };
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{barId}/order/{orderId}")]
        public ActionResult DeleteSpecificOrder(int barId, int orderId) //admin only
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }

            Order order = OrderRepository.FindByPK(orderId);
            if (order == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified order.");
            }

            if (!bar.Orders.Contains(order))
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified order.");
            }

            OrderRepository.Delete(order);
            OrderRepository.Save();

            return new HttpStatusCodeResult(200);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order")]
        public ActionResult ListAllOrders(int barId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return ListMyOrders(bar);
            }
            return new JsonResult() { Data = bar.Orders };
        }

        public ActionResult ListMyOrders(Bar bar) //user
        {
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            var myOrders = bar.Orders.Where(x => x.User.Equals(LoggedUser));

            return new JsonResult() { Data = myOrders };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/order/{userName}")]
        public ActionResult PostOrderAdmin(int barId, string userName, [FromBody] List<Tuple<int, int>> orderList)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of this bar can perform this action!");
            }

            ApplicationUser user = UserRepository.FindByName(userName);
            if (user == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified user.");
            }

            foreach (var item in orderList)
            {
                Drink drink = DrinkRepository.FindByPK(item.Item1);
                if (drink == null)
                {
                    return new HttpStatusCodeResult(404, "Cannot find the specified drink.");
                }
                if (!bar.Drinks.Contains(drink))
                {
                    return new HttpStatusCodeResult(404, "Cannot find this drink in current bar.");
                }
                if (DrinkBarRepository.Find(bar, drink) == null)
                {
                    return new HttpStatusCodeResult(404, "Cannot find this drink in current bar.");
                }
            }

            Order order = OrderRepository.NewOrder(bar, user, orderList);
            return new JsonResult() { Data = order };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/order")]
        public ActionResult PostOrder(int barId, [FromBody] List<Tuple<int, int>> orderList)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            foreach (var item in orderList)
            {
                Drink drink = DrinkRepository.FindByPK(item.Item1);
                if (drink == null)
                {
                    return new HttpStatusCodeResult(404, "Cannot find the specified drink.");
                }
                if (!bar.Drinks.Contains(drink))
                {
                    return new HttpStatusCodeResult(404, "Cannot find this drink in current bar.");
                }
            }

            Order o = OrderRepository.NewOrder(bar, LoggedUser, orderList);
            return new JsonResult() { Data = o };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order/user")]
        public ActionResult ListOrdersFromSpecificUserForAdmin(int barId, [FromBody] string userName) //admin only
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of the bar can perform this action!");
            }
            ApplicationUser customer = UserRepository.FindByName(userName);
            if (customer == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified user.");
            }

            var orders = bar.Orders.Where(x => x.User.Id.Equals(customer.Id)).ToList();
            return new JsonResult() { Data = orders };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/users")]
        public ActionResult GetUsersWhoOrdered(int barId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of the bar can perform this action!");
            }
            List<ApplicationUser> users = UserRepository.GetUsersWithOrder(bar);
            return new JsonResult() { Data = users };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/bottles")]
        public ActionResult GetBottles(int barId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            return new JsonResult() { Data = BottleRepository.ListBottlesOnBar(bar) };
        }

        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/notification")]
        public ActionResult GetEventsByBar(int barId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            List<Event> events = EventRepository.FindByBar(bar);
            return new JsonResult() { Data = events };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/notification")]
        public ActionResult AddEventToBar(int barId, [FromBody] Event e)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(403, "Only owner of the bar can access this function!");
            }
            e.Bar = bar;
            Event existingEvent = EventRepository.FindByPK(e.EventId);
            if (existingEvent == null)
            {
                EventRepository.AddEventToBar(bar, e);
                return new JsonResult() { Data = e };
            }
            EventRepository.EditEvent(bar, e);
            return new JsonResult() { Data = e };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/notification/{eventId}")]
        public ActionResult GetEvent(int barId, int eventId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            Event e = EventRepository.FindByPK(eventId);
            if (e == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified event.");
            }

            if (!bar.Events.Contains(e))
            {
                return new HttpStatusCodeResult(403);
            }

            return new JsonResult() { Data = e };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/notification/before/{eventId}")]
        public ActionResult GetEventsBefore(int barId, int eventId)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            Event e = EventRepository.FindByPK(eventId);
            if (e == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified event.");
            }

            if (!bar.Events.Contains(e))
            {
                return new HttpStatusCodeResult(403);
            }
            var events = EventRepository.FindEventsBefore(bar, e);
            return new JsonResult() { Data = events };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/bottle/ingredient/{ingredientId}")]
        public ActionResult PostBottle(int barId, int ingredientId, [FromBody] Quantity quantity)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            Ingredient ingredient = IngredientRepository.FindByPK(ingredientId);
            if (ingredient == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified ingredient.");
            }

            if (quantity == null)
            {
                return new HttpStatusCodeResult(400, "Quantity cannot be null.");
            }

            Bottle addedBottle = BottleRepository.AddBottle(bar, ingredient, quantity);
            return new JsonResult() { Data = addedBottle };
        }
    }
}