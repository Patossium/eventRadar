﻿using System;
using System.Collection.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace eventRadar.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WebsiteName { get; set; }
    }
}
