using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using BarUTomaModels.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BarUTomaREST.Models
{
    public class BarContext : DbContext
    {
        // Your context has been configured to use a 'BarContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'BarUTomaServer.Models.BarContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'BarContext' 
        // connection string in the application configuration file.
        public BarContext()
            : base("name=BarContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Bar> Bars { get; set; }
        public virtual DbSet<BarType> BarTypes { get; set; }
        public virtual DbSet<Bottle> Bottles { get; set; }
        public virtual DbSet<Drink> Drinks { get; set; }
        public virtual DbSet<DrinkBar> DrinkBars { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public virtual DbSet<IngredientDrink> IngredientDrinks { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDrink> OrderDrinks { get; set; }
        public virtual DbSet<Quantity> Quantities { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<ApplicationUser> Users { get; set; }
        public virtual DbSet<UserBar> UserBars { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new {r.RoleId, r.UserId});
        }

        public static BarContext Create()
        {
            return new BarContext();
        }
    }

}