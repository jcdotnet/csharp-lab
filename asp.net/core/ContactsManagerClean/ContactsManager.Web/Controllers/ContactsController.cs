using ContactsManager.Core.DTO;
using ContactsManager.Enums;
using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.AuthorizationFilters;
using ContactsManager.Filters.ResultFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;

namespace ContactsManager.Web.Controllers
{
    [Route("contacts")]
    [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Controller-Key", "MyValue"])]
    public class ContactsController : Controller
    {
        private ICountriesGetterService _countriesGetterService;
        private IContactsAdderService _contactsAdderService;
        private IContactsGetterService _contactsGetterService;
        private IContactsUpdaterService _contactsUpdaterService;
        private IContactsSorterService _contactsSorterService;
        private IContactsDeleterService _contactsDeleterService;

        public ContactsController(IContactsAdderService contactsAdderService,
            IContactsGetterService contactsGetterService,
            IContactsUpdaterService contactsUpdaterService,
            IContactsSorterService contactsSorterService,
            IContactsDeleterService contactsDeleterService,
            ICountriesGetterService countriesGetterService)
        {
            _contactsAdderService = contactsAdderService;
            _contactsGetterService = contactsGetterService;
            _contactsUpdaterService = contactsUpdaterService;
            _contactsSorterService = contactsSorterService;
            _contactsDeleterService = contactsDeleterService;
            _countriesGetterService = countriesGetterService;
        }

        [Route("/")]
        [Route("[action]")] // route token
        [TypeFilter(typeof(ContactsListActionFilter))] // [TypeFilter<ContactsListActionFilter>]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Index-Key", "MyValue"])]
        [TypeFilter(typeof(ContactsListResultFilter))]
        public async Task<IActionResult> Index(string searchBy, string? search,
            string sortBy = nameof(PersonResponse.Name),
            SortOrder sortOrder = SortOrder.Ascending)
        {
            var filteredContacts = await _contactsGetterService.GetFilteredContacts(searchBy, search);
            var sortedContacts = await _contactsSorterService.GetSortedContacts(filteredContacts, sortBy, sortOrder);
            return View(sortedContacts);
        }

        [Route("[action]")]
        [HttpGet]
        [TypeFilter(typeof(ContactsHttpPostActionFilter))]
        [TypeFilter(typeof(ResponseHeaderActionFilter), Arguments = ["X-Create-Key", "MyValue"])]
        public async Task<IActionResult> Create()
        {
            var countries = await _countriesGetterService.GetCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem()
            {
                Text = country.Name,
                Value = country.Id.ToString()
            });
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        [TypeFilter(typeof(ContactsHttpPostActionFilter))]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            await _contactsAdderService.AddContact(personRequest);

            return RedirectToAction("Index", "Contacts");
        }

        [Route("[action]/{id}")]
        [HttpGet]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid id)
        {
            var person = await _contactsGetterService.GetContact(id); 
            if (person == null)
            {
                return RedirectToAction("Index");
            }
            var countries = await _countriesGetterService.GetCountries();
            ViewBag.Countries = countries.Select(country => new SelectListItem()
            {
                Text = country.Name,
                Value = country.Id.ToString()
            });
            return View(person.ToPersonUpdateRequest());
        }

        [HttpPost]
        [Route("[action]/{id}")]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            var person = await _contactsGetterService.GetContact(personRequest.Id);

            if (person == null)
            {
                return RedirectToAction("Index");
            }
            await _contactsUpdaterService.UpdateContact(personRequest);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            var person = await _contactsGetterService.GetContact(id);
            if (person == null)
                return RedirectToAction("Index");

            return View(person);
        }

        [HttpPost]
        [Route("[action]/{id}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            var person = await _contactsGetterService.GetContact(personUpdateRequest.Id);
            if (person == null)
                return RedirectToAction("Index");

            await _contactsDeleterService.DeleteContact(personUpdateRequest.Id);
            return RedirectToAction("Index");
        }
    }
}
