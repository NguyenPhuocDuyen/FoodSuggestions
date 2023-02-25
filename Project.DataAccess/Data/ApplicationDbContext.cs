using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Project.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HistorySearch>(b =>
            {
                b.HasKey(r => new { r.UserId });
            });
            modelBuilder.Entity<FoodEaten>(b =>
            {
                b.HasKey(r => new { r.UserId, r.FoodId });
            });
            modelBuilder.Entity<Report>(b =>
            {
                b.HasKey(r => new { r.UserId, r.FoodId });
            });
            modelBuilder.Entity<FoodOfUser>(b =>
            {
                b.HasKey(r => new { r.UserId, r.FoodId });
            });
            modelBuilder.Entity<Feedback>(b =>
            {
                b.HasKey(r => new { r.UserId, r.FoodId });
            });

            modelBuilder.Entity<Message>(b =>
            {
                b.Property(s => s.Content).IsRequired().HasMaxLength(500);

                b.HasOne(r => r.ToRoom).WithMany(m => m.Messages)
                .HasForeignKey(s => s.ToRoomId)
                .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<RoomChat>(b =>
            {
                b.Property(s => s.Name).IsRequired().HasMaxLength(100);
                b.HasOne(s => s.Admin)
                .WithMany(u => u.Rooms)
                .IsRequired();
            });


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            modelBuilder.Entity<IdentityUser>().ToTable("Users").Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<User>().ToTable("Users").Property(p => p.Id).HasColumnName("Id");

        }

        public DbSet<Message> Message { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<FoodOfUser> FoodOfUser { get; set; }
        public DbSet<Food> Food { get; set; }
        public DbSet<FoodEaten> FoodEaten { get; set; }
        public DbSet<HistorySearch> HistorySearch { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<RoomChat> RoomChat { get; set; }
        public DbSet<Restaurant> Restaurant { get; set; }


    }
}
