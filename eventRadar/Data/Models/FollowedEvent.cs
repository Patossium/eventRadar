using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
	public class FollowedEvent
	{
		public int Id { get; set; }
		public int UserID { get; set; }
		public int EventID { get; set; }
	}
}