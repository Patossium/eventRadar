using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eventRadar.Models;
using eventRadar.Auth.Model;

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
        public WebDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=event-radar-server.eventRadarDB.dbo");
        }
    }
}