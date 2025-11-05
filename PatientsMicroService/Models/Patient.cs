using System.ComponentModel.DataAnnotations;

namespace PatientsMicroService.Models
{

    public enum Gender
    {
        Male,
        Female,        
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
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int Age
        {
            get
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - BirthDate.Year;
                if (today.CompareTo(BirthDate.AddYears(age)) < 0)
                {
                    age--;
                }
                return age;
            }
        }
    }
}
