using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class Website
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string TitlePath { get; set; }
        public string LocationPath { get; set; }
        public string PricePath { get; set; }
        public string DatePath { get; set; }
        public string ImagePath { get; set; }
        public string TicketPath { get; set; }
        public string UrlExtensionForEvent { get; set; }
        public string EventLink { get; set; }
        public string CategoryLink { get; set; }
        public string PagerLink { get; set; }
        public string TicketLinkType { get; set; }
        public string FullLocationPath { get; set; }
    }
}
