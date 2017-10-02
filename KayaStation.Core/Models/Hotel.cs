using System;
using System.Collections.Generic;
using System.Text;

namespace KayaStation.Core.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeactivated { get; set; }

        public virtual List<Room> Rooms { get; set; }
    }
}
