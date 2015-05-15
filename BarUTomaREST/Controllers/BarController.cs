using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using BarUTomaModels.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BarUTomaREST.Controllers
{
    //[System.Web.Http.Authorize]
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

        [System.Web.Http.Authorize(Roles = "Administrators")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("bar/")]
        public ActionResult PostBar([FromBody] string barToAddstr)
        {
            Bar newBar = JsonConvert.DeserializeObject<Bar>(barToAddstr);
            try
            {
                BarRepository.AddNewBar(newBar);
            }
            catch (ArgumentNullException e)
            {
                return new HttpStatusCodeResult(400, "Argument " + e.ParamName + "cannot be null!");
            }
            catch (ArgumentException e)
            {
                return new HttpStatusCodeResult(400, e.Message);
            }

            return new HttpStatusCodeResult(200);
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/")]
        public ActionResult DeleteBar(int id)
        {
            Bar barToDelete = BarRepository.FindByPK(id);
            if (barToDelete == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            BarRepository.Delete(barToDelete);
            
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
        public ActionResult PostDrink(int id, [FromBody] string drinkToAddstr)
        {
            DrinkBar drinkToAdd = JsonConvert.DeserializeObject<DrinkBar>(drinkToAddstr);

            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }

            DrinkBarRepository.AddDrinkToBar(bar, drinkToAdd);

            return new HttpStatusCodeResult(200);
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

            return new JsonResult() { Data = order };
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("bar/{barId}/order/{orderId}")]
        public ActionResult DeleteSpecificOrder(int barId, int orderId) //not tested
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

            OrderRepository.Delete(order);

            return new HttpStatusCodeResult(200);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order")]
        public ActionResult ListAllOrders(int barId) //admin only
        {
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
            }
            return new JsonResult() { Data = bar.Orders };
        }

        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("bar/{barId}/order")]
        //public ActionResult ListMyOrders(int barId) //user
        //{
        //    Bar bar = BarRepository.FindByPK(barId);
        //    if (bar == null)
        //    {
        //        return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
        //    }

        //    var myOrders = bar.Orders.Where(x => x.User.Equals(User.Identity));

        //    return new JsonResult() { Data = myOrders };
        //}

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{barId}/order/{userId}")]
        public ActionResult ListOrdersFromSpecificUserForAdmin(int barId, int userId)
        {
            //auth
            Bar bar = BarRepository.FindByPK(barId);
            if (bar == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified bar.");
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