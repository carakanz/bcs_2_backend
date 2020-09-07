using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondsController : ControllerBase
    {
        private readonly IdentityApplicationContext _context;

        public BondsController(IdentityApplicationContext context)
        {
            _context = context;
        }

        // GET: api/DBBonds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DBBond>> GetDBBond(int id)
        {
            var dBBond = await _context.Bonds.FindAsync(id);

            if (dBBond == null)
            {
                return NotFound();
            }

            return dBBond;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DBBond>> GetDBBonds(int id)
        {
            var dBBond = await _context.Bonds.FindAsync(id);

            if (dBBond == null)
            {
                return NotFound();
            }

            return dBBond;
        }

        // PUT: api/DBBonds/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDBBond(int id, DBBond dBBond)
        {
            if (id != dBBond.Id)
            {
                return BadRequest();
            }

            _context.Entry(dBBond).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DBBondExists(id))
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

        // POST: api/DBBonds
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DBBond>> PostDBBond(DBBond dBBond)
        {
            _context.Bonds.Add(dBBond);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDBBond", new { id = dBBond.Id }, dBBond);
        }

        // DELETE: api/DBBonds/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DBBond>> DeleteDBBond(int id)
        {
            var dBBond = await _context.Bonds.FindAsync(id);
            if (dBBond == null)
            {
                return NotFound();
            }

            _context.Bonds.Remove(dBBond);
            await _context.SaveChangesAsync();

            return dBBond;
        }

        private bool DBBondExists(int id)
        {
            return _context.Bonds.Any(e => e.Id == id);
        }
    }
}
