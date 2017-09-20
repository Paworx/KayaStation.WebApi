using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KayaStation.DAL;
using KayaStation.DAL.Models;

namespace KayaStation.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext db;

        public HotelsController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet]
        public IEnumerable<Hotel> GetAll()
        {
            //EAGER LOADED 
            var data = db.Hotels
                .Include(h => h.Rooms)
                .AsNoTracking()
                .ToList();

            return data;
        }

        [HttpGet("{id}")]
        public async Task<Hotel> GetById([FromRoute] int id)
        {
            var hotel = await db.Hotels.SingleOrDefaultAsync(m => m.Id == id);

            if (hotel == null)
            {
                return null;
            }

            //EXPLICIT LOAD RELATED DATA
            db.Entry(hotel)
                .Collection(h => h.Rooms)
                .Load();

            return hotel;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Hotel hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Hotels.Add(hotel);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PutRoom([FromRoute] int id, [FromBody] Hotel room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.Id)
            {
                return BadRequest();
            }

            db.Entry(room).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var hotel = db.Hotels
                .Include(h => h.Rooms)
                .FirstOrDefault(h => h.Id == id);

            if (hotel == null)
                return NotFound();

            db.Hotels.Remove(hotel);
            await db.SaveChangesAsync();
            return NoContent();
        }

        private bool HotelExists(int id)
        {
            return db.Hotels.Any(e => e.Id == id);
        }
    }
}