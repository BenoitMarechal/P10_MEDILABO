using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotesMicroService.Data;
using NotesMicroService.Models;

namespace NotesMicroService.Repositories
{
    public class NotesRepository
    {
        private readonly IMongoCollection<Note> _notes;

        public NotesRepository(IMongoClient client, IOptions<MongoDbSettings> settings)
        {
            var database = client.GetDatabase(settings.Value.Database);
            _notes = database.GetCollection<Note>("Notes");
        }

        public async Task<List<Note>> GetAllAsync() =>
            await _notes.Find(_ => true).ToListAsync();

        public async Task<Note?> GetByIdAsync(Guid id) =>
            await _notes.Find(n => n.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Note>> GetByPatientAsync(Guid patientId) =>
            await _notes.Find(n => n.PatientId == patientId)
            .SortByDescending(n => n.CreatedAt)            
            .ToListAsync();

        public async Task CreateAsync(Note note) =>
            await _notes.InsertOneAsync(note);

        public async Task UpdateAsync(Guid id, Note note) =>
            await _notes.ReplaceOneAsync(n => n.Id == id, note);

        public async Task DeleteAsync(Guid id) =>
            await _notes.DeleteOneAsync(n => n.Id == id);
    }

}
