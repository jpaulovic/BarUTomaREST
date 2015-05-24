using System.Collections.Generic;
using System.ComponentModel;
using BarUTomaModels.Models;
using BarUTomaREST.Models;

namespace BarUTomaREST.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BarUTomaREST.Models.BarContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BarContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            Bar myBar = new Bar()
            {
                Name = "myBar1",
                Address =
                    new Address()
                    {
                        City = "Brno",
                        Country = "CZ",
                        PostCode = "61200",
                        StreetWithNumber = "Botanicka 68a"
                    },
                BarType = new BarType() { Name = "Custom" },
                DateCreated = DateTime.Now,
                DateModified = DateTime.Now
            };

            Unit kc = new Unit() { Name = "Koruna ceska", Code = "Kc", MultiplierToBase = 1 };

            Drink myDrink = new Drink()
            {
                //Bar = myBar,
                Name = "MojDrink",
                Info = "Toto si fakt dajte!",
                Price = new Quantity()
                {
                    Amount = new decimal(0),
                    Unit = kc
                },
            };

            Quantity price = new Quantity()
            {
                Amount = new decimal(13.00),
                Unit = kc
            };

            DrinkBar myDrinkBar = new DrinkBar()
            {
                Bar = myBar,
                Drink = myDrink,
                Info = "Moj super drink",
                Price = price
            };
            var barsThatHavemyDrink = new List<DrinkBar> { myDrinkBar };
            myDrink.BarsThatHaveThisDrink = barsThatHavemyDrink;

            context.Drinks.AddOrUpdate(myDrink);
            context.Bars.AddOrUpdate(myBar);
            context.Drinks.AddOrUpdate(myDrink);
        }
    }
}
