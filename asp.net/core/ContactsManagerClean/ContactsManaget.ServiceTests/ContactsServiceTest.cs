using AutoFixture;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Entities;
using ContactsManager.Enums;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using Services;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;

namespace ContactsManager.ServiceTests
{
    public class ContactsServiceTest
    {
        private readonly IContactsAdderService _adderService;
        private readonly IContactsGetterService _getterService;
        private readonly IContactsUpdaterService _updaterService;
        private readonly IContactsSorterService _sorterService;
        private readonly IContactsDeleterService _deleterService;
        private readonly Mock<IContactsRepository> _repositoryMock;
        private readonly IContactsRepository _repository;

        // AutoFixture automates non-relevant test fixture by generating dummy values
        private readonly IFixture _fixture; 

        public ContactsServiceTest()
        {
            _repositoryMock = new Mock<IContactsRepository>();
            _repository     = _repositoryMock.Object;
            _adderService   = new ContactsAdderService(_repository);
            _getterService  = new ContactsGetterService(_repository);
            _updaterService = new ContactsUpdaterService(_repository);
            _sorterService  = new ContactsSorterService(_repository);
            _deleterService = new ContactsDeleterService(_repository);
            _fixture        = new Fixture();
        }

        #region AddContact

        // Requirement: when person is null, it should throw ArgumentNullException
        [Fact]
        public async Task AddContact_NullPerson_ToBeArgumentNullException()
        {
            // Arrange
            PersonAddRequest? person = null;

            // Act
            var func = async () =>
            {
                await _adderService.AddContact(person);
            };

            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        // Requirement: when person name is null, it should throw ArgumentException
        // Optional: same with email addresses, etc.
        [Fact]
        public async Task AddContact_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            var personAddRequest = _fixture.Build<PersonAddRequest>().
                With(temp => temp.Name, null as string).Create();

            var person = personAddRequest.ToPerson();

            // mocking the repository (using test double method instead of the AddContact method)
            // i.e. creating an object that look and behave like their production equivalent ones
            _repositoryMock.Setup(p => p.AddContact(It.IsAny<Person>())).ReturnsAsync(person);

            // Act
            var func = async () =>
            {
                await _adderService.AddContact(personAddRequest);
            };

            // Assert
            await func.Should().ThrowAsync<ArgumentException>();
        }

        // Requirement: when proper person details, it should insert it in the contacts
        [Fact]
        public async Task AddContact_PersonDetails_ToBeSuccessful()
        {
            // Arrange
            var personRequest = _fixture.Build<PersonAddRequest>() 
                .With(temp => temp.Email, "email@example.com")
                .Create();
            var person = personRequest.ToPerson();
            var expectedPerson = person.ToPersonResponse();

            _repositoryMock.Setup(p => p.AddContact(It.IsAny<Person>()))
                .ReturnsAsync(person);

            // Act
            PersonResponse? personResponse= await _adderService.AddContact(personRequest);
            expectedPerson.Id = personResponse.Id;

            // unit testing: we must test a single method in a single test case
            // not calling service.GetContacts now and testing AddContacts only
            //List<PersonResponse> listResponse = await _service.GetContacts();

            // Assert
            personResponse.Id.Should().NotBe(Guid.Empty);
            personResponse.Should().Be(expectedPerson);
        }

        #endregion

        #region GetContact

        // Requirement: when person id is null, it should return null
        [Fact]
        public async Task GetContact_NullPersonId_ToBeNull()
        {
            // Arrange
            // Act
            PersonResponse? personGetResponse = await _getterService.GetContact(null);

            // Assert
            //Assert.Null(personGetResponse);
            personGetResponse.Should().BeNull();
        }

        [Fact]
        public async Task GetContact_PersonDetails_ToBeSuccessful()
        {
            // Arrange
            // unit testing: we must test a single method in a single test case
            // so we'll comment the call to AddContact to test GetContact only
            //PersonAddRequest? personRequest = GenerateDummyPerson();
            //PersonResponse? personAddResponse = await _service.AddContact(personRequest);
            Person person = GenerateDummyPerson();

            // mocking the repository (using test double method instead of the GetContact method)
            // i.e. creating an object that look and behave like their production equivalent ones
            _repositoryMock.Setup(p => p.GetContact(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            PersonResponse? personGetResponse = await _getterService.GetContact(person.Id);

            // Assert
            personGetResponse.Should().Be(person.ToPersonResponse());
        }

        #endregion

        #region GetContacts

        // requirement: the contact list should be emtpy by default
        [Fact]
        public async Task GetContacts_Default_ToBeEmptyList()
        {
            // Arrange
            // mocking the repository (using test double method instead of the GetContacts method)
            // i.e. creating an object that look and behave like their production equivalent ones
            var people = new List<Person>();
            _repositoryMock.Setup(p => p.GetContacts()).ReturnsAsync(people); // mocks GetContacts

            // Act
            var contactsList = await _getterService.GetContacts();

            // Assert
            //Assert.Empty(contactsList);
            contactsList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetContacts_AddContacts_ToBeSuccessful()
        {
            // Arrange
            var people = GenerateDummyContactList();

            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test GetContacts only

            var expectedList = people.Select(p => p.ToPersonResponse()).ToList();

            // mocking the repository (using test double method instead of the GetContacts method)
            // i.e. creating an object that look and behave like their production equivalent ones
            _repositoryMock.Setup(p => p.GetContacts()).ReturnsAsync(people); // mocks GetContacts

            // Act
            var getAllResponse= await _getterService.GetContacts();

            // Assert
            //foreach (var person in addedPeople)
            //{
            //    Assert.Contains(person, getAllResponse); 
            //}
            getAllResponse.Should().BeEquivalentTo(expectedList);
        }

        #endregion

        #region GetFilteredContacts

        // requirements: if search is empty and searchBy is "Name", it should return all contacts
        [Fact]
        public async Task GetFilteredContacts_EmptySearch_ToBeSuccessful()
        {
            // Arrange
            var people = GenerateDummyContactList();

            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test this method only
            var expectedList = people.Select(p=>p.ToPersonResponse()).ToList();

            // mocking the repository (using test double method instead of the GetFilteredContacts method)
            // i.e. creating an object that look and behave like their production equivalent ones
            //_repositoryMock.Setup(temp =>
            //    temp.GetFilteredContacts(It.IsAny<Expression<Func<Person, bool>>>())
            //).ReturnsAsync(people);
            // in this case we need to mock GetContacts only (it should return all contacts)
            _repositoryMock.Setup(temp => temp.GetContacts()).ReturnsAsync(people);

            // Act
            var getFilteredResponse = await _getterService.GetFilteredContacts(nameof(Person.Name), "");

            // Assert
            //foreach (var person in addedPeople)
            //{
            //    Assert.Contains(person, getFilteredResponse);
            //}
            getFilteredResponse.Should().BeEquivalentTo(expectedList);
        }

        [Fact]
        public async Task GetFilteredContacts_PersonName_ToBeSuccessful()
        {
            // Arrange
            var people = GenerateDummyContactList();

            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test this method only
            var expectedList = people.Select(p => p.ToPersonResponse()).ToList();

            _repositoryMock.Setup(temp => temp
                .GetFilteredContacts(It.IsAny<Expression<Func<Person, bool>>>())
            ).ReturnsAsync(people);

            // Act
            var getFilteredResponse = await _getterService.GetFilteredContacts(nameof(Person.Name), "ja");

            // Assert
            getFilteredResponse.Should().BeEquivalentTo(expectedList);
            getFilteredResponse.Should()
                .Contain(p => p.Name != null && p.Name.Contains("ja", StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region GetSortedContacts

        [Fact]
        public async Task GetSortedContacts_PersonNameDescending_ToBeSuccessful()
        {
            // Arrange
            var people = GenerateDummyContactList();

            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test this method only
            //var expectedList = people.Select(p => p.ToPersonResponse()).ToList();

            _repositoryMock.Setup(temp => temp.GetContacts()).ReturnsAsync(people);

            // Act
            var getSortedResponse = await _sorterService.GetSortedContacts(
                await _getterService.GetContacts(), 
                nameof(Person.Name), 
                SortOrder.Descending);

            //var sortedAddedPeople = addedPeople.OrderByDescending(p => p.Name).ToList();

            // Assert
            //for (int i = 0; i < sortedAddedPeople.Count; i++)
            //{
            //    Assert.Equal(sortedAddedPeople[i], getSortedResponse[i]);
            //}
            getSortedResponse.Should().BeInDescendingOrder(p => p.Name);
        }

        #endregion

        #region UpdateContact

        // Requirement: when person is null, it should throw ArgumentNullException
        [Fact]
        public async Task UpdateContact_NullPerson_ToBeArgumentNullException()
        {
            // Act
            var func = async () =>
            {
                await _updaterService.UpdateContact(null);
            };
            // Assert
            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        // Requirement: when person id is invalid, it should throw ArgumentException
        [Fact]
        public async Task UpdateContact_InvalidPersonId_ToBeArgumentException()
        {
            // Arrange
            PersonUpdateRequest? request = new PersonUpdateRequest() {Id = Guid.NewGuid() };

            //Act
            var func = async () =>
            {
                await _updaterService.UpdateContact(request);
            };
            // Assert
            await func.Should().ThrowAsync<ArgumentException>();
        }

        // Requirement: when person name is null, it should throw ArgumentException
        [Fact]
        public async Task UpdateContact_NullPersonName_ToBeArgumentException()
        {
            // Arrange
            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test GetContacts only
            //PersonResponse addedPerson = await _service.AddContact(await GenerateDummyPerson());
            //PersonUpdateRequest updateRequest = addedPerson.ToPersonUpdateRequest();

            Person person = GenerateDummyPerson();
            person.Name = null;

            // Act
            var func = async () =>
            {
                await _updaterService.UpdateContact(person.ToPersonResponse().ToPersonUpdateRequest());
            };
            // Assert
            await func.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task UpdateContact_PersonDetails_ToBeSuccessful()
        {
            // Arrange
            // unit testing: we must test a single method in a single test case
            // we won't call _service.AddContact and will test GetContacts only
            Person person = GenerateDummyPerson();

            PersonResponse expectedperson = person.ToPersonResponse();
            PersonUpdateRequest personUpdateRequest = expectedperson.ToPersonUpdateRequest();

            _repositoryMock.Setup(temp => temp.UpdateContact(It.IsAny<Person>())).ReturnsAsync(person);
            _repositoryMock.Setup(temp => temp.GetContact(It.IsAny<Guid>())).ReturnsAsync(person);

            // Act
            PersonResponse updatedPerson = await _updaterService.UpdateContact(personUpdateRequest);
            //PersonResponse? getResponse = await _service.GetContact(personResponse.Id);

            // Assert
            updatedPerson.Should().Be(expectedperson);
        }

        #endregion

        #region DeleteContact

        // requirements: if person id is invalid, it should return false
        [Fact]
        public async Task DeleteContact_InvalidId_ToBeFalse()
        {
            // Arrange
            // Act
            bool isDeleted = await _deleterService.DeleteContact(Guid.NewGuid());

            // Assert
            //Assert.False(isDeleted);
            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteContact_ValidId_ToBeTrue()
        {
            // Arrange
            Person person = GenerateDummyPerson();

            _repositoryMock.Setup(temp => temp.DeleteContact(It.IsAny<Guid>())).ReturnsAsync(true);

            _repositoryMock.Setup(temp => temp.GetContact(It.IsAny<Guid>())).ReturnsAsync(person);
            // Act
            bool isDeleted = await _deleterService.DeleteContact(person.Id);

            // Assert
            //Assert.True(isDeleted);
            isDeleted.Should().BeTrue();
        }
        #endregion

        private Country GenerateDummyCountry()
        {
            // unit testing principle: we must test a single method in a single test case
            // so we will comment the call to AddCountry to test our contact methods only
            //CountryAddRequest countryRequest = _fixture.Create<CountryAddRequest>();
            //return await _countriesService.AddCountry(countryRequest);
            return _fixture.Create<Country>();
        }

        private List<Person> GenerateDummyContactList()
        {
            var country = GenerateDummyCountry();
            List<Person> contacts = [
                 _fixture.Build<Person>().With(temp => temp.Name, "John Doe")
                .With(temp => temp.Email, "john@email.com").With(temp => temp.CountryId, country.Id).Create(),
                _fixture.Build<Person>().With(temp => temp.Name, "Jane Doe")
                .With(temp => temp.Email, "jane@email.com").With(temp => temp.CountryId, country.Id).Create(),
                _fixture.Build<Person>().With(temp => temp.Name, "Jade Doe")
                .With(temp => temp.Email, "jade@email.com").With(temp => temp.CountryId, country.Id).Create(),
            ];
            return contacts;
        }

        private Person GenerateDummyPerson()
        {
            var country = GenerateDummyCountry();

            //return new PersonAddRequest() {
            //    Name = "John Doe", Email = "john@email.com", Gender = Gender.Male, CountryId = country.Id 
            //};
            //return _fixture.Create<PersonAddRequest>(); // initializes all model properties with dummy values
            return _fixture.Build<Person>()  // build allows us to customize the dummy values
                .With(temp => temp.Email, "email@example.com")
                .With(temp => temp.Gender, "Female")
                .With(temp => temp.CountryId, country.Id)
                .Create();
        }
    }
}
