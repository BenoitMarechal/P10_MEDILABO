using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientsMicroService.Models;
using PatientsMicroService.Controllers.Data;
using Microsoft.AspNetCore.Authorization;

namespace PatientsMicroService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> Get()
        {
            return await _context.Patients.ToListAsync();
        }

        // GET: api/patients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");
            return Ok(patient);
        }

        //POST: api/patients
        [HttpPost]
        public async Task<ActionResult<Patient>> Post([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            patient.Id = Guid.NewGuid(); // Auto-generate ID
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
                return BadRequest("ID mismatch.");

            if (!await _context.Patients.AnyAsync(p => p.Id == id))
                return NotFound($"Patient with ID {id} not found.");

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "A concurrency error occurred.");
            }
        }

        // DELETE: api/patients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound($"Patient with ID {id} not found.");

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
