using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BarUTomaModels.Models;

namespace BarUTomaServer.Models
{
    public class BarRepository : Repository<Bar>
    {
        //chceme vsetko narvat do jedneho repozitara (tohto)? Neda sa to nejako "zanorit", kedze podla specifikacie by mali exostovat len 2 controllery (bar a user)?
        //^Nie je cela tato myslienka blbost a repozitarov moze byt viac ale budu len 2 controllery? :D Stacia 2 controllery? Ako to bude fungovat?
        public BarRepository(DbContext db)
            : base(db)
        {
        }

        
    }
}