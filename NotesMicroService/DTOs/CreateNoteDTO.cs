namespace NotesMicroService.DTOs
{
    public class CreateNoteDTO
    {
        public Guid PatientId { get; set; }
        public string Content { get; set; } = null!;
    }
}
