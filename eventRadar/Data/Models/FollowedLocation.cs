using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eventRadar.Auth.Model;

namespace eventRadar.Models
{
    public class FollowedLocation : IUserOwnedResource
    {
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int LocationID { get; set; }
        public Location Location { get; set; }
        public int Id { get; set; }
    }
}