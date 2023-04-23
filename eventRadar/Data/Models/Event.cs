using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string ImageLink { get; set; }
        public double Price { get; set; }
        public string TicketLink { get; set; }
        public bool Updated { get; set; }
        public Location Location { get; set; }
        public Category Category { get; set; }
    }
}
