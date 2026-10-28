using ContactsManager.Enums;
using ContactsManager.Core.Entities;

namespace ContactsManager.Core.DTO
{
    public class PersonResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; } // FK
        public string? Country { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public double? Age { get; set; }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest() { 
                Id = Id, 
                Name = Name, 
                Email = Email, 
                BirthDate = BirthDate, 
                Gender = (Gender)Enum.Parse(typeof(Gender), Gender!, true), 
                Address = Address, 
                CountryId = CountryId,
                ReceiveNewsletters = ReceiveNewsletters,
            };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;
            return this.Id == ((PersonResponse)obj).Id;
        }

    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                Id          = person.Id,
                Name        = person.Name,
                Email       = person.Email,
                Address     = person.Address,
                BirthDate   = person.BirthDate,
                Gender      = person.Gender,
                CountryId   = person.CountryId,
                Country     = person.Country?.Name,
                ReceiveNewsletters = person.ReceiveNewsletters,
                Age = (person.BirthDate != null) ? Math.Round((DateTime.Now - person.BirthDate.Value).TotalDays / 365.25) : null
            };
        }
    } 
}
