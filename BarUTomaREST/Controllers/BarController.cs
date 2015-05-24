using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
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
            User = User;
            try
            {
                Bar existingBar = BarRepository.FindByPK(newBar.BarId);
                if (existingBar == null)
                {
                    BarRepository.AddNewBar(newBar, User.Identity);
                    return new JsonResult() {Data = newBar};
                }
                if (LoggedUser == null)
                {
                    return new HttpStatusCodeResult(401);
                }
                BarRepository.EditBar(newBar);
            }
            catch (ArgumentNullException e)
            {
                return new HttpStatusCodeResult(400, "Argument " + e.ParamName + "cannot be null!");
            }
            catch (ArgumentException e)
            {
                return new HttpStatusCodeResult(400, e.Message);
            }

            return new JsonResult(){Data = newBar};
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
                return new HttpStatusCodeResult(401, "Only owner of this bar can perform this action!");
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
            return new JsonResult() {Data = bar};
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/getMyBars")]
        public ActionResult GetMyBars()
        {
            if (LoggedUser == null)
            {
                return new HttpStatusCodeResult(401);
            }

            return new JsonResult() {Data = UserBarRepository.GetMyBars(LoggedUser)};
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
                return new HttpStatusCodeResult(401, "Only owner of this bar can perform this action!");
            }

            DrinkBarRepository.AddDrinkToBar(bar, drinkToAdd);

            return new JsonResult(){Data = drinkToAdd};
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order/{orderId}")]
        public ActionResult GetSpecificOrder(int barId, int orderId) //not tested
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

            if (UserBarRepository.OwnsUserBar(LoggedUser, bar)) return new JsonResult() {Data = order};

            if (!LoggedUser.Orders.Contains(order))
            {
                return new HttpStatusCodeResult(401, "This order is not assigned to the current user.");
            }

            return new JsonResult() { Data = order };
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{barId}/order/{orderId}")]
        public ActionResult DeleteSpecificOrder(int barId, int orderId) //not tested //admin only
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(401, "Only owner of this bar can perform this action!");
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

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order/{userId}")]
        public ActionResult ListOrdersFromSpecificUserForAdmin(int barId, int userId) //admin only
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            if (!UserBarRepository.OwnsUserBar(LoggedUser, bar))
            {
                return new HttpStatusCodeResult(401, "Only owner of the bar can access this function!");
            }
            ApplicationUser customer = UserRepository.FindByPK(userId);
            if (customer == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified user.");
            }
            var orders = bar.Orders.Where(x => x.User.Id.Equals(customer.Id));
            return new JsonResult() {Data = orders};
        }
    }
}