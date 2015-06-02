using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BarUTomaModels.Models;

namespace BarUTomaREST.Models
{
    public class EventRepository : Repository<Event>
    {
        public EventRepository(DbContext db) : base(db)
        {
        }

        public List<Event> FindByBar(Bar bar)
        {
            return db.Set<Event>().Where(x => x.Bar.BarId.Equals(bar.BarId)).ToList();
        }

        public List<Event> FindEventsBefore(Bar bar, Event e)
        {
            return db.Set<Event>().Where(x => x.Bar.BarId.Equals(bar.BarId) && x.DateTime < e.DateTime).ToList();
        }

        public void AddEventToBar(Bar bar, Event e)
        {
            e.Bar = bar;
            Add(e);
            bar.Events.Add(e);
            Save();
        }

        public void EditEvent(Bar bar, Event e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            Event existingEvent = FindByPK(e.EventId);
            if (existingEvent == null)
            {
                throw new NullReferenceException();
            }

            if (e.DateTime != DateTime.MinValue)
            {
                existingEvent.DateTime = e.DateTime;
            }
            if (e.Info != null)
            {
                existingEvent.Info = e.Info;
            }
            if (e.Name != null)
            {
                existingEvent.Name = e.Name;
            }
            Save();
        }
    }
}