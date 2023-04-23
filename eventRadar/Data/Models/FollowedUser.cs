using System;
using System.Collection.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class FollowedUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FollowedUserID { get; set; }
    }
}