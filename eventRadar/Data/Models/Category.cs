using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SourceUrl { get; set; }
    }
}
