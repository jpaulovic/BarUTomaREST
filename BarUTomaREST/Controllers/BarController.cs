using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using BarUTomaModels.Models;
using Newtonsoft.Json;

namespace BarUTomaREST.Controllers
{
    public class BarController : BaseController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/")]
        public ActionResult GetAllBars()
        {
            List<Bar> bars = BarRepository.FindAll();
            return new JsonResult() {Data = bars};
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("bar/{id}/drink")]
        public ActionResult GetDrinks(int id)
        {
            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(400, "System cannot find the specified bar.");
            }
            List<DrinkBar> drinkBars = DrinkRepository.ListAllDrinksOnBar(bar);
            return new JsonResult() {Data = drinkBars};
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

            return new JsonResult() {Data = order};
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
}
}