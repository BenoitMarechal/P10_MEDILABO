    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;
namespace FrontEndMicroService.DTOs

    {
        public enum Gender
        {
            Male,
            Female,
        }

        public class PatientDTO
        {
            public Guid Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        [EnumDataType(typeof(Gender))]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public Gender Gender { get; set; }

            public string? Address { get; set; }

            public string? PhoneNumber { get; set; }

        public int Age
        {
            get
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - BirthDate.Year;
                if (today < BirthDate.AddYears(age))
                    age--;
                return age;
            }
        }

    }
    }


