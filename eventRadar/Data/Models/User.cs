using System;
using System.Collection.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Privaloma įvesti vardą")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Privaloma įvesti elektroninio pašto adresą")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Privaloma įvesti salptažodį")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Privaloma įvesti pavardę")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Privaloma įvesti slapyvardį")]
        public string Username { get; set; }
        public bool Administrator { get; set; }
    }
}
