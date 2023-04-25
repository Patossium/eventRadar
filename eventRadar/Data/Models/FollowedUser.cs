using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eventRadar.Auth.Model;

namespace eventRadar.Models
{
    public class FollowedUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string FollowedUserId { get; set; }
        public User Followed_User { get; set; }
    }
}