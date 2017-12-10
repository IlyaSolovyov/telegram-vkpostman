using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VKPostman.Models;

namespace VKPostman.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<PublicPage> PublicPages { get; set; }
        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasKey(t => new { t.SubscriberId, t.PublicPageId });

            modelBuilder.Entity<Subscription>()
                .HasOne(sc => sc.Subscriber)
                .WithMany(s => s.Subscriptions)
                .HasForeignKey(sc => sc.SubscriberId);

            modelBuilder.Entity<Subscription>()
                .HasOne(sc => sc.PublicPage)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(sc => sc.PublicPageId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
              optionsBuilder.UseSqlite("Filename=VkPostman.db");
        }
    }
}
