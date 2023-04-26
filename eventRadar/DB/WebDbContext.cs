using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eventRadar.Models;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace eventRadar
{
    public class WebDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<BlacklistedPage> BlacklistedPages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ChangedEvent> ChangedEvents { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FollowedEvent> FollowedEvents { get; set; }
        public DbSet<FollowedLocation> FollowedLocations { get; set; }
        public DbSet<FollowedUser> FollowedUsers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<VisitedEvent> VisitedEvents { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }
        public WebDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("default"));

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUser>().ToTable("User");

            modelBuilder.Entity<IdentityUser>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<FollowedUser>()
                .HasKey(fu => fu.Id);

            modelBuilder.Entity<FollowedUser>()
                .HasOne(fu => fu.User)
                .WithMany()
                .HasForeignKey(fu => fu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FollowedUser>()
                .HasOne(fu => fu.Followed_User)
                .WithMany()
                .HasForeignKey(fu => fu.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}