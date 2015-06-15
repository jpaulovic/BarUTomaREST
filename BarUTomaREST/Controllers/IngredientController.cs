using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using BarUTomaModels.Models;

namespace BarUTomaREST.Controllers
{
    [System.Web.Http.Authorize]
    public class IngredientController : BaseController
    {
        /// <summary>
        /// Get the ingredient specified by ID.
        /// </summary>
        /// <param name="id">ID of ingredient.</param>
        /// <returns>The specified ingredient. (JSON)</returns>
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ingredient/{id}")]
        public ActionResult GetIngredient(int id)
        {
            Ingredient ingredient = IngredientRepository.FindByPK(id);
            if (ingredient == null)
            {
                return new HttpStatusCodeResult(404, "System cannot find the specified ingredient.");
            }
            return new JsonResult(){Data = ingredient};
        }
    }
}