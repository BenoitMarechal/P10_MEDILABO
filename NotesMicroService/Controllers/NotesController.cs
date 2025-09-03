using Microsoft.AspNetCore.Mvc;
using NotesMicroService.DTOs;
using NotesMicroService.Models;
using NotesMicroService.Repositories;
using NotesMicroService.Services;


namespace NotesMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase
    {
      //  private readonly ApplicationDbContext _context;
        private readonly ILogger<NotesController> _logger;
        private readonly NotesRepository _repository;

        public NotesController( ILogger<NotesController> logger, NotesRepository repository)
        {
         //   _context = context;
            _logger = logger;
            _repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            try
            {
                var notes = await _repository.GetAllAsync();
                return Ok(notes);
                //var notes = await _context.Notes.ToListAsync();
                //return Ok(notes);
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
                //var note = await _context.Notes.FindAsync(id);
                var note = await _repository.GetByIdAsync(id);

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
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByPatient(Guid patientId, [FromServices] PatientsService patientsService)
        {
            try
            {
                var patient= await patientsService.GetPatientAsync(patientId);

                if(patient == null)
                    return NotFound($"Patient with ID {patientId} not found");

              
                var notes = await _repository.GetByPatientAsync(patientId);


                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes for patient {PatientId}", patientId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/notes/patient/{patientId}
        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote(CreateNoteDTO dto, [FromServices] PatientsService patientsService)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var patientExists = await patientsService.PatientExistsAsync(dto.PatientId);
            if (!patientExists) return BadRequest($"Patient with ID {dto.PatientId} does not exist");

            var note = new Note
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
                
            };

            await _repository.CreateAsync(note);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
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

               // _context.Entry(note).State = EntityState.Modified;

                try
                {
                    await _repository.UpdateAsync(id, note);
                    // await _context.SaveChangesAsync();
                }
                catch (Exception)
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
               // var note = await _context.Notes.FindAsync(id);
               var note = await _repository.GetByIdAsync(id);
                if (note == null)
                {
                    return NotFound($"Note with ID {id} not found");
                }

                await _repository.DeleteAsync(id);
                //_context.Notes.Remove(note);
                //await _context.SaveChangesAsync();

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
           // return await _context.Notes.AnyAsync(e => e.Id == id);
           return await _repository.GetByIdAsync(id) != null;
        }
    }
}