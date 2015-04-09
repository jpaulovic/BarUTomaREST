using System.Data.Entity;
using System.Linq;


namespace BarUTomaServer.Models
{
    public abstract class Repository<T> where T : class
    {
        protected DbContext db;

        protected Repository(DbContext db)
        {
            this.db = db;
        }


        public virtual void Add(T entity)
        {
            db.Set<T>().Add(entity);
        }

        public virtual IQueryable<T> FindAll()
        {
            return db.Set<T>();
        }

        public virtual T FindByPK(int pk)
        {
            return db.Set<T>().Find(pk);
        }

        public virtual void Delete(T entity)
        {
            db.Entry(entity).State = EntityState.Deleted;
        }

        public virtual void Save()
        {
            db.SaveChanges();
        }

    }
}