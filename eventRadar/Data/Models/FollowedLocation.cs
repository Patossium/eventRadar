using System;
using System.Collection.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class FollowedLocation
    {
        public int UserID { get; set; }
        public int LocationID { get; set; }
        public int Id { get; set; }
    }
}