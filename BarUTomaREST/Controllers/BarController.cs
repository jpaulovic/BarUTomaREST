using System;
using System.Collections.Generic;
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
        public ActionResult GetDrinks(int id)
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
        public ActionResult PostDrink(int id, [FromBody] string drinkToAddstr)
        {
            DrinkBar drinkToAdd = JsonConvert.DeserializeObject<DrinkBar>(drinkToAddstr); //does NOT serialize properly (all nulls)

            Bar bar = BarRepository.FindByPK(id);
            if (bar == null)
            {
                return new HttpStatusCodeResult(400, "System cannot find the specified bar.");
            }
            if (bar.DrinksOnBar.Contains(drinkToAdd))
            {
                return new HttpStatusCodeResult(400, "Drink already exists on this bar!");
            }
            if (!bar.Drinks.Contains(drinkToAdd.Drink))
            {
                bar.Drinks.Add(drinkToAdd.Drink);
            }
            bar.DrinksOnBar.Add(drinkToAdd);
            return new HttpStatusCodeResult(200);
        }
    }
}