using System.ComponentModel.DataAnnotations;
namespace NotesMicroService.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        [Required]
        public Guid PatientId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}