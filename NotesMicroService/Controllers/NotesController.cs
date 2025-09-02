using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesMicroService.Data;
using NotesMicroService.Models;
using NotesMicroService.Services;

namespace NotesMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotesController> _logger;

        public NotesController(ApplicationDbContext context, ILogger<NotesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            try
            {
                var notes = await _context.Notes.ToListAsync();
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all notes");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/notes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(Guid id)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);

                if (note == null)
                {
                    return NotFound($"Note with ID {id} not found");
                }

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving note with ID {NoteId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/notes/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByPatient(Guid patientId)
        {
            try
            {
                var notes = await _context.Notes
                    .Where(n => n.PatientId == patientId)
                    .ToListAsync();

                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote(Note note, [FromServices] PatientsService patientsService)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Or get full patient info if needed:
                var patient = await patientsService.GetPatientAsync(note.PatientId);
                if (patient == null)
                    return BadRequest("Patient not found");


                note.Id = Guid.NewGuid();
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating note");
                return StatusCode(500, "Internal server error");
            }
        }

    

        // PUT: api/notes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(Guid id, Note note)
        {
            try
            {
                if (id != note.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Entry(note).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await NoteExists(id))
                    {
                        return NotFound($"Note with ID {id} not found");
                    }
                    throw;
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating note with ID {NoteId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/notes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            try
            {
                var note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    return NotFound($"Note with ID {id} not found");
                }

                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note with ID {NoteId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task<bool> NoteExists(Guid id)
        {
            return await _context.Notes.AnyAsync(e => e.Id == id);
        }
    }
}