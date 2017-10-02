using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace KayaStation.Core.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]
        public virtual Hotel Hotel { get; set; }
        public int HotelId { get; set; }

    }

    public enum RoomType
    {
        Fan, Economy, Deluxe
    }
}
