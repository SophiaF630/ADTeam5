using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;

namespace ADTeam5.clerkApi
{
    [Route("api/[controller]")]
    public class RecordDetailsController : ControllerBase
    {
        private readonly SSISTeam5Context _context;

        public RecordDetailsController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: api/RecordDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecordDetails>>> GetRecordDetails()
        {
            return await _context.RecordDetails.ToListAsync();
        }

        // GET: api/RecordDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecordDetails>> GetRecordDetails(int id)
        {
            var recordDetails = await _context.RecordDetails.FindAsync(id);

            if (recordDetails == null)
            {
                return NotFound();
            }

            return recordDetails;
        }

        // PUT: api/RecordDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecordDetails(int id, RecordDetails recordDetails)
        {
            if (id != recordDetails.Rdid)
            {
                return BadRequest();
            }

            _context.Entry(recordDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecordDetailsExists(id))
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
        [HttpGet("PF/{amount}/{rdid}")]
        public async Task<string> PFitemAsync(string amount,string rdid)
        {
            RecordDetails temp = _context.RecordDetails.Where(s => s.Rdid.ToString() == rdid).ToList().First();
            temp.QuantityDelivered = int.Parse(amount);
            _context.Update(temp);
            await _context.SaveChangesAsync();
            return "123";
        }
        [HttpGet("Remove/{id}")]
        public async Task<string> RemoveRdAsync(string id)
        {
            var recordDetails = _context.RecordDetails.Where(s=>s.Rdid.ToString() ==id).ToList().First();
            if (recordDetails == null)
            {
                return null;
            }

            _context.RecordDetails.Remove(recordDetails);
            await _context.SaveChangesAsync();

            return "ok";
        }
        //Order partially fulfilled post
        [HttpPost("PF")]
        public async Task<string> PFitem1Async(order source)
        {
            RecordDetails temp = _context.RecordDetails.Where(s => s.Rdid.ToString() == source.DepName).ToList().First();
            temp.Remark = source.status;
            _context.Update(temp);
            await _context.SaveChangesAsync();
            return "123";
        }

        // POST: api/RecordDetails
        [HttpPost]
        public async Task<ActionResult<RecordDetails>> PostRecordDetails(RecordDetails recordDetails)
        {
            _context.RecordDetails.Add(recordDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecordDetails", new { id = recordDetails.Rdid }, recordDetails);
        }

        // DELETE: api/RecordDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecordDetails>> DeleteRecordDetails(int id)
        {
            var recordDetails = await _context.RecordDetails.FindAsync(id);
            if (recordDetails == null)
            {
                return NotFound();
            }

            _context.RecordDetails.Remove(recordDetails);
            await _context.SaveChangesAsync();

            return recordDetails;
        }

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }
    }
}
