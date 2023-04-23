using System;
using System.Collection.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class ChangedEvent
    {
        public int Id { get; set; }
        public string OldInformation { get; set; }
        public string NewInformation { get; set; }
        public DateTime ChangeTime { get; set; }
    }
}
