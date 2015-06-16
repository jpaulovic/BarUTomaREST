using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// GET method to get all bars in db.
        /// </summary>
        /// <returns>All bars in the database. (JSON)</returns>
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/")]
        public ActionResult GetAllBars()
        {
            List<Bar> bars = BarRepository.FindAll();
            return new JsonResult() { Data = bars };
        }

        /// <summary>
        /// Add a new bar to database or edit existing.
        /// </summary>
        /// <param name="newBar">Note: The DateCreated and DateModified parameter in Bar is filled by the application, do not send it in request.</param>
        /// <returns>The added or edited bar. (JSON)</returns>
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
        /// <summary>
        /// Deletes bar from the db.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Http status code.</returns>
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
        /// <summary>
        /// Get specific bar by ID.
        /// </summary>
        /// <param name="id">ID of the wanted bar.</param>
        /// <returns>Specified bar. (JSON)</returns>
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
        /// <summary>
        /// Returns all bars owned by currently logged user.
        /// </summary>
        /// <returns>All bars owned by currently logged user. (JSON)</returns>
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
        /// <summary>
        /// Gets information about a specific drink.
        /// </summary>
        /// <param name="id">ID of the specified drink</param>
        /// <returns>Information about drink specified by id. (JSON)</returns>
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
        /// <summary>
        /// Adds a new drink to a specified bar, if currently logged user is the owner of the bar.
        /// </summary>
        /// <param name="id">ID of the bar.</param>
        /// <param name="drinkToAdd">Drink to be added to bar.</param>
        /// <returns>The added drink. (JSON)</returns>
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

        /// <summary>
        /// Modify existing drink.
        /// </summary>
        /// <param name="barId">ID of the bar.</param>
        /// <param name="drinkId">ID of the drink to modify.</param>
        /// <param name="newDrinkBar">New parameters for drink in specified bar.</param>
        /// <returns>The modified drink in specified bar. (JSON)</returns>
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
        /// <summary>
        /// Get information about drink specified by ID in a bar specified by ID.
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="drinkId">ID of drink.</param>
        /// <returns>Information about specified drink in the specified bar. (JSON)</returns>
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
        /// <summary>
        /// Delete drink from bar (if currently logged user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="drinkId">ID of drink.</param>
        /// <returns>Http status code.</returns>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{barId}/drink/{drinkId}")]
        public ActionResult DeleteSpecificDrink(int barId, int drinkId)
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
            DrinkBarRepository.Delete(drinkBar);
            DrinkBarRepository.Save();
            return new HttpStatusCodeResult(200);
        }
        /// <summary>
        /// Get specific order for specific bar (both specified by ID).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="orderId">ID of order.</param>
        /// <returns>The specified order. (JSON)</returns>
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
        /// <summary>
        /// Cancel (delete) specific order (if currently logged user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="orderId">ID of order.</param>
        /// <returns>Http status code.</returns>
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
        /// <summary>
        /// Get list of all orders in a specified bar (if currently logged user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <returns>List of all orders in bar. (JSON)</returns>
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
        /// <summary>
        /// Get list of user's orders in specified bar.
        /// </summary>
        /// <param name="bar">ID of bar.</param>
        /// <returns>List of orders for currently logged user for specified bar. (JSON)</returns>
        public ActionResult ListMyOrders(Bar bar) //user
        {
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            var myOrders = bar.Orders.Where(x => x.User.Equals(LoggedUser));

            return new JsonResult() { Data = myOrders };
        }
        /// <summary>
        /// Create new order for specified customer in specified bar (if currently logged user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="userName">Username of the user for whom the order is being created.</param>
        /// <param name="orderList">List of tuples where each tuple consists of two integers.
        /// The first integer represents the drink ID, the second one represents the quantity how many drinks to order.</param>
        /// <returns>The created order. (JSON)</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/order/{userName}")]
        public ActionResult PostOrderAdmin(int barId, string userName, [FromBody] List<string> orderList)
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

            List<Tuple<int, int>> parsedOrderList = ParseOrderList(orderList);
            foreach (var item in parsedOrderList)
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

            Order order = OrderRepository.NewOrder(bar, user, parsedOrderList);
            return new JsonResult() { Data = order };
        }
        /// <summary>
        /// Create new order for currently logged user.
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="orderList">List of tuples where each tuple consists of two integers.
        /// The first integer represents the drink ID, the second one represents the quantity how many drinks to order.</param>
        /// <returns>The created order. (JSON)</returns>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/{barId}/order")]
        public ActionResult PostOrder(int barId, [FromBody] List<string> orderList)
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            List<Tuple<int, int>> parsedOrderList = ParseOrderList(orderList);
            foreach (var item in parsedOrderList)
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

            Order o = OrderRepository.NewOrder(bar, LoggedUser, parsedOrderList);
            return new JsonResult() { Data = o };
        }

        List<Tuple<int, int>> ParseOrderList(List<string> orderList)
        {
            List<Tuple<int, int>> parsedOrderList = new List<Tuple<int, int>>();
            foreach (var item in orderList)
            {
                Regex parse = new Regex(@"\((\d+), (\d+)\)");
                var match = parse.Match(item);
                int drinkId;
                int quantity;
                int.TryParse(match.Groups[1].Value, out drinkId);
                int.TryParse(match.Groups[2].Value, out quantity);
                Tuple<int, int> orderItem = new Tuple<int, int>(drinkId, quantity);
                parsedOrderList.Add(orderItem);
            }
            return parsedOrderList;
        }
            /// <summary>
        /// List all orders for specified user in specified bar (if currently logged user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="userName">Username of the user whose orders we want to return.</param>
        /// <returns>All orders in the specified bar for the specified user. (JSON)</returns>
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
        /// <summary>
        /// Get a list of users who made at least one order in specified bar.
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <returns>List of users who made at least one order in specified bar. (JSON)</returns>
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
        /// <summary>
        /// Get all bottles in specified bar.
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <returns>List of bottles on bar. (JSON)</returns>
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
        /// <summary>
        /// Get all notifications (events) for specified bar.
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <returns>All notifications (events) for specified bar.</returns>
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
        /// <summary>
        /// Create new notification (event) or edit an existing one for specified bar (if current user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="e">Event to add/edit.</param>
        /// <returns>Created/edited event.</returns>
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
        /// <summary>
        /// Get specific event for specified bar (both are specified by ID).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="eventId">ID of event.</param>
        /// <returns>The specified event.</returns>
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
        /// <summary>
        /// Get list of all events before a specified event (if current user owns the bar).
        /// </summary>
        /// <param name="barId">ID of bar.</param>
        /// <param name="eventId">ID of event.</param>
        /// <returns>List of all events before a specified event. </returns>
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
        /// <summary>
        /// Add new bottle of specified ingredient to bar.
        /// </summary>
        /// <param name="barId">ID of bar</param>
        /// <param name="ingredientId">ID of ingredient.</param>
        /// <param name="quantity">Amount of bottles.</param>
        /// <returns>The added bottle.</returns>
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