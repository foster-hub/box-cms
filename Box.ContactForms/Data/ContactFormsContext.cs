using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Box.ContactForms.Data {
    

    public class ContactFormsContext : DbContext {

        public ContactFormsContext() : base("DefaultConnection") {         
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<ContactForm>()
                .HasRequired(c => c.Data)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete();
        }

        public DbSet<ContactForm> ContactForms{ get; set; }
        

        internal void ApplyCollectionValues<T>(ICollection<T> oldCollection, ICollection<T> newCollection, Func<T, T, bool> predicate) {
            if (oldCollection == null)
                oldCollection = new List<T>();
            if(newCollection==null)
                newCollection = new List<T>();
            var removed = oldCollection.Where(o => !newCollection.Any(n => predicate(n, o))).ToArray();
            var added = newCollection.Where(n => !oldCollection.Any(o => predicate(n, o))).ToArray();

            foreach (T r in removed)
                this.Set(typeof(T)).Remove(r);

            foreach (T a in added)
                this.Set(typeof(T)).Add(a);
        }
    }
}
