using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eventRadar.Auth.Model;

namespace eventRadar.Models
{
    public class FollowedLocation : IUserOwnedResource
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public Location Location { get; set; }
        public string LocationId { get; set; }
        public string Id { get; set; }
    }
}