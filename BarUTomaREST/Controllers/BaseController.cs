using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BarUTomaREST.Models;

namespace BarUTomaREST.Controllers
{
    public class BaseController : ApiController
    {
        //initialize all repositories, then add specific controllers derived from this

        protected BarRepository BarRepository;
        protected BottleRepository BottleRepository;
        protected DrinkBarRepository DrinkBarRepository;
        protected DrinkRepository DrinkRepository;
        protected EventRepository EventRepository;
        protected IngredientDrinkRepository IngredientDrinkRepository;
        protected IngredientRepository IngredientRepository;
        protected OrderDrinkRepository OrderDrinkRepository;
        protected OrderRepository OrderRepository;
        protected QuantityRepository QuantityRepository;
        protected UnitRepository UnitRepository;
        protected UserBarRepository UserBarRepository;
        protected UserRepository UserRepository;
        protected UserRoleRepository UserRoleRepository;

        public BaseController()
        {
            var db = new BarContext();
            BarRepository = new BarRepository(db);
            BottleRepository = new BottleRepository(db);
            DrinkBarRepository = new DrinkBarRepository(db);
            DrinkRepository = new DrinkRepository(db);
            EventRepository = new EventRepository(db);
            IngredientDrinkRepository = new IngredientDrinkRepository(db);
            IngredientRepository = new IngredientRepository(db);
            OrderDrinkRepository = new OrderDrinkRepository(db);
            OrderRepository = new OrderRepository(db);
            QuantityRepository = new QuantityRepository(db);
            UnitRepository = new UnitRepository(db);
            UserBarRepository = new UserBarRepository(db);
            UserRepository = new UserRepository(db);
            UserRoleRepository = new UserRoleRepository(db);
        }
    }
}
