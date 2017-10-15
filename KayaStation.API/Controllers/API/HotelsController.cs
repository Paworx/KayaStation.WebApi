using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KayaStation.Core.Models;
using KayaStation.Core.Data;
using Microsoft.AspNetCore.Authorization;

namespace KayaStation.API.Controllers.API
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    public class HotelsController : Controller
    {
        private readonly ApplicationDbContext db;

        public HotelsController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet()]
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

        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] Hotel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Hotels.Add(value);
            await db.SaveChangesAsync();

            return CreatedAtAction("GetHotel", new { id = value.Id }, value);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Hotel value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != value.Id)
            {
                return BadRequest();
            }

            db.Entry(value).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
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