using ContactManager.Dtos;
using ContactManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{
    [Route("contacts")]
    public class ContactsController : Controller
    {
        private readonly IContactService _service;

        public ContactsController(IContactService service)
        {
            _service = service;
        }

        // GET /contacts loads the full page view
        [HttpGet("")]
        public IActionResult Index()
        {
            var result = _service.GetAll();

            if (!result.Success)
            {
                ViewData["Error"] = result.Message;
                return View(new List<ContactDto>());
            }

            return View(result.Data ?? new List<ContactDto>());
        }

        // GET /contacts/list returns the _ContactList partial for AJAX refresh
        [HttpGet("list")]
        public IActionResult List()
        {
            var result = _service.GetAll();
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return PartialView("_ContactList", result.Data ?? new List<ContactDto>());
        }

        // GET /contacts/search?query=ann returns the _ContactList partial filtered by the query
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? query)
        {
            var result = _service.Search(query);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return PartialView("_ContactList", result.Data ?? new List<ContactDto>());
        }
    }
}
