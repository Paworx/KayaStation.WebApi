using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KayaStation.Core.Models;
using Microsoft.EntityFrameworkCore;
using KayaStation.Core.Data;

namespace KayaStation.API.Controllers.API
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext db;

        public RoomsController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpGet("")]
        public IEnumerable<Room> GetAll()
        {
            //EAGER LOADED 
            var data = db.Rooms
                .AsNoTracking()
                .ToList();

            return data;
        }

        [HttpGet("{id}")]
        public async Task<Room> GetById([FromRoute] int id)
        {
            var room = await db.Rooms.SingleOrDefaultAsync(m => m.Id == id);

            if (room == null)
            {
                return null;
            }

            return room;
        }

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] Room value)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Rooms.Add(value);
            await db.SaveChangesAsync();

            return CreatedAtAction("","Rooms", new { id = value.Id }, value);
        }

        [HttpPost("update/{id}")]
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
                if (!RoomExists(id))
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

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            var room = db.Rooms
                .FirstOrDefault(h => h.Id == id);

            if (room == null)
                return NotFound();

            db.Rooms.Remove(room);
            await db.SaveChangesAsync();
            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return db.Rooms.Any(e => e.Id == id);
        }
    }
}