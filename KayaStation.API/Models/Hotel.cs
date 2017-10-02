using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KayaStation.API.Models
{
    public class HotelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeactivated { get; set; }

        public virtual List<RoomViewModel> Rooms { get; set; }
    }

    public class HotelInputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeactivated { get; set; }
    }
}
