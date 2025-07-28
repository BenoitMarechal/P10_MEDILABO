using System.ComponentModel.DataAnnotations;

namespace PatientMicroService.Models
{

    public enum Gender
    {
        Male,
        Female,
        Other
    }
    public class Patient
    {
        public Guid Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateOnly BirthDate { get; set; }
        [Required]
        public Gender Gender { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }
    }
}
