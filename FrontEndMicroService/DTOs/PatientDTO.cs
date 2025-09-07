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

            public DateTime BirthDate { get; set; }

            [EnumDataType(typeof(Gender))]
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public Gender Gender { get; set; }

            public string Address { get; set; }

            public string PhoneNumber { get; set; }
            // public int Age { get; }
            public int Age
            {
                get
                {
                    var today = DateTime.Today; var age = today.Year - BirthDate.Year; // If birthday hasn't occurred this year yet, subtract 1
                    if (BirthDate > today.AddYears(-age)) age--;
                    return age;
                }
            }

        }
    }


