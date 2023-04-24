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
		[Required]
		public int UserId { get; set; }
		public User User { get; set; }
		[Required]
		public int EventID { get; set; }
		public Event Event { get; set; }
	}
}