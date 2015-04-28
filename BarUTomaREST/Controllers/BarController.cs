using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BarUTomaModels.Models;

namespace BarUTomaREST.Controllers
{
    public class BarController : BaseController
    {
        [System.Web.Http.HttpGet]
        public ActionResult DrinkGet(int id)
        {
            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(400);
            }
            List<DrinkBar> drinkBars = DrinkRepository.ListAllDrinksOnBar(bar);
            return new JsonResult() { Data = drinkBars };
        }

        [System.Web.Http.HttpPost]
        public ActionResult DrinkPost(int id, Drink drink, string info, Quantity price)
        {
            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(400, "System cannot find the specified bar.");
            }
            DrinkBar drinkToAdd = new DrinkBar() {Drink = drink, Bar = bar, Info = info, Price = price};
            if (bar.DrinksOnBar.Contains(drinkToAdd))
            {
                return new HttpStatusCodeResult(400, "Drink already exists on this bar!");
            }
            if (!bar.Drinks.Contains(drink))
            {
                bar.Drinks.Add(drink);
            }
            bar.DrinksOnBar.Add(drinkToAdd);
            return new HttpStatusCodeResult(200);
        }
    }
}