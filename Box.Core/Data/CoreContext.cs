using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.History;
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

    // Essas linha estavam gerando erro quando o projeto rodava em MS SQL
    //public class MySqlConfiguration : DbConfiguration
    //{
    //    public MySqlConfiguration()
    //    {
    //        SetHistoryContext("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
    //    }

    //}
    //public class MySqlHistoryContext : HistoryContext
    //{
    //    public MySqlHistoryContext(DbConnection connection, string defaultSchema)
    //       : base(connection, defaultSchema)
    //    { }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        base.OnModelCreating(modelBuilder);
    //        modelBuilder.Entity<HistoryRow>().Property(h => h.MigrationId).HasMaxLength(100).IsRequired();
    //        modelBuilder.Entity<HistoryRow>().Property(h => h.ContextKey).HasMaxLength(200).IsRequired();
    //    }
    //}
}
