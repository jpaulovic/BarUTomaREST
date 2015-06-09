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
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BarContext context)
        {
            Unit kc = new Unit() { Name = "Koruna ceska", Code = "Kc", MultiplierToBase = 1 };
            Unit eur = new Unit() { Name = "Euro", Code = "Eur", MultiplierToBase = 1 };
            Unit ks = new Unit() { Name = "kus", Code = "ks", MultiplierToBase = 1 };

            BarType system = new BarType() { Name = "System" };
            BarType custom = new BarType() { Name = "Custom" };

            context.Units.AddOrUpdate(ks);
            context.Units.AddOrUpdate(kc);
            context.Units.AddOrUpdate(eur);
            context.BarTypes.AddOrUpdate(system);
            context.BarTypes.AddOrUpdate(custom);
        }
    }
}
