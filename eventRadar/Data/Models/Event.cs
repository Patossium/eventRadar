using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventRadar.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string ImageLink { get; set; }
        public string Price { get; set; }
        public string TicketLink { get; set; }
        public bool Updated { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
    }
}
