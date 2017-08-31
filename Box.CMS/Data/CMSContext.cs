using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Box.Composition;
using System.Collections.Generic;
using System.Linq;

namespace Box.CMS.Data {
    

    public class CMSContext : DbContext {

        public CMSContext() : base("DefaultConnection") {
            Database.SetInitializer<CMSContext>(new CMSContextInitializer());
            Database.CommandTimeout = 900;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            
          

            modelBuilder.Entity<ContentHead>()
                .HasRequired(u => u.Data)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete();

            modelBuilder.Entity<ContentHead>()
                .HasOptional<ContentCommentCount>(c => c.CommentsCount)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<ContentHead>()
                .HasOptional<ContentShareCount>(c => c.ShareCount)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<ContentHead>()
                .HasOptional<ContentPageViewCount>(c => c.PageViewCount)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<ContentHead>()
                .HasOptional<ContentCustomInfo>(c => c.CustomInfo)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<File>()
                .HasRequired(f => f.Data)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(); 
        }

        public DbSet<ContentHead> ContentHeads { get; set; }
        public DbSet<ContentData> ContentDatas { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<CrossLink> CrossLinks { get; set; }

        public DbSet<ContentComment> ContentComments { get; set; }
        public DbSet<ContentCommentCount> ContentCommentCounts { get; set; }

        public DbSet<ContentShareCount> ContentSharesCounts { get; set; }
        public DbSet<ContentPageViewCount> ContentPageViewCounts { get; set; }

        public DbSet<ContentCustomInfo> ContentCustomInfos { get; set; }
        

        public DbSet<File> Files { get; set; }
        public DbSet<FileData> FileData { get; set; }


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
