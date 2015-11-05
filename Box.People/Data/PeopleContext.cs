using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Box.People.Data {

    public class PeopleContext : DbContext {

        public DbSet<Person> Person { get; set; }

        public PeopleContext() : base("DefaultConnection") {            
        }
    }
}
