using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
namespace NotesMicroService.Models
{
    public class Note
    {

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }


        [BsonRepresentation(BsonType.String)]
        public Guid PatientId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}