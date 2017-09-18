﻿using System;
using System.Collections.Generic;
using System.Text;

namespace KayaStation.DAL.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeactivated { get; set; }
        public decimal Price { get; set; }

        public int HotelId { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}