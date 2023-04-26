using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eventRadar.Auth.Model;

namespace eventRadar.Models
{
    public class FollowedLocation : IUserOwnedResource
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Location Location { get; set; }
        public int LocationId { get; set; }
        [Required]
        public string OwnerId { get; set; }
        public User Owner { get; set; }
    }
}