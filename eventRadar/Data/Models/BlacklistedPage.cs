using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class BlacklistedPage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
    }
}
