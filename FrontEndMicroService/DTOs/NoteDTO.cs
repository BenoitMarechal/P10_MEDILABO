namespace FrontEndMicroService.DTOs
{
    public class NoteDTO
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
