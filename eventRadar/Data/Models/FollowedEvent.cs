using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using eventRadar.Auth.Model;

namespace eventRadar.Models
{
    public class FollowedEvent : IUserOwnedResource
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public User User { get; set; }
		public Event Event { get; set; }
		public int EventId { get; set; }
		[Required]
		public string OwnerId { get; set; }
        public User Owner { get; set; }
    }
}