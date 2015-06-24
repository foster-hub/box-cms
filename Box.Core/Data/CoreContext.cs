using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Box.Composition;

namespace Box.Core.Data {
    

    public class CoreContext : DbContext {
        
        public CoreContext() : base("DefaultConnection") {
            Database.SetInitializer<CoreContext>(new CoreContextInitializer());                                    
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<User>()
                .HasOptional(u => u.Password)
                .WithRequired()
                .WillCascadeOnDelete();

            //modelBuilder.Entity<GroupCollectionMembership>()
            //    .HasOptional<GroupCollection>(g => g.Collection)
            //    .WithRequired()
            //    .WillCascadeOnDelete();

        }

        public DbSet<AsyncTask> AsyncTasks { get; set; }
        
        public DbSet<User> Users { get; set; }

        public DbSet<GroupMembership> GroupMemberships { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public DbSet<UserPassword> UserPasswords { get; set; }

        public DbSet<PasswordReset> PasswordResets { get; set; }

        public DbSet<GroupCollection> GroupCollections { get; set; }

        public DbSet<GroupCollection_Group> GroupCollection_Groups { get; set; }

        public DbSet<GroupCollectionMembership> GroupCollectionMemberships { get; set; }
        
        public DbSet<Log> Logs { get; set; }
    }
}
