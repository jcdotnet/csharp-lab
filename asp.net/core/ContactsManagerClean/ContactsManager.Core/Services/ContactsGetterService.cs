using ContactsManager.Core.DTO;
using ContactsManager.Core.Entities;
using RepositoryContracts;
using ServiceContracts;

namespace Services
{
    public class ContactsGetterService : IContactsGetterService
    {
        private readonly IContactsRepository _repository;

        public ContactsGetterService(IContactsRepository repository)
        {
            _repository = repository;
        }
        public async Task<PersonResponse?> GetContact(Guid? id)
        {
            if (id == null) return null;

            Person? person = await _repository.GetContact(id.Value);

            if (person == null) { return null; }

            return person.ToPersonResponse();
        }

        public async Task<List<PersonResponse>> GetContacts()
        {
            return (await _repository.GetContacts()).Select(c => c.ToPersonResponse()).ToList();
        }

        public async Task<List<PersonResponse>> GetFilteredContacts(string searchBy, string? search)
        {
            List<Person> getFiltered;
            if (string.IsNullOrEmpty(search)) getFiltered = await _repository.GetContacts();
            else
                getFiltered = searchBy switch
                {
                    nameof(PersonResponse.Name) => await _repository.GetFilteredContacts(p =>
                        p.Name != null && p.Name.Contains(search)),
                    nameof(PersonResponse.Email) => await _repository.GetFilteredContacts(p =>
                        p.Email != null && p.Email.Contains(search)),
                    nameof(PersonResponse.Address) => await _repository.GetFilteredContacts(p =>
                        p.Address != null && p.Address.Contains(search)),
                    nameof(PersonResponse.BirthDate) => await _repository.GetFilteredContacts(p =>
                        p.BirthDate != null && p.BirthDate.Value.ToString("dd MMMM yyyy").Contains(search)),
                    nameof(PersonResponse.Gender) => await _repository.GetFilteredContacts(p =>
                        p.Gender != null && p.Gender.Contains(search)),
                    nameof(PersonResponse.CountryId) => await _repository.GetFilteredContacts(p =>
                        p.Country != null && p.Country.Name != null && p.Country.Name.Contains(search)),
                    _ => await _repository.GetContacts()
                };
            return getFiltered.Select(p => p.ToPersonResponse()).ToList();
        }
    }
}