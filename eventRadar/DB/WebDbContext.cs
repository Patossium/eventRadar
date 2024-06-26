using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eventRadar.Models;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace eventRadar
{
    [ExcludeFromCodeCoverage]
    public class WebDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<BlacklistedPage> BlacklistedPages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<FollowedEvent> FollowedEvents { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<VisitedEvent> VisitedEvents { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRoles { get; set; }
        public DbSet<BlacklistedCategoryName> BlacklistedCategoryNames { get; set; }
        public DbSet<City> Cities { get; set; }
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
        }
    }
}