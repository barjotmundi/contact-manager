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

        // GET /contacts
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

        // GET /contacts/list
        [HttpGet("list")]
        public IActionResult List()
        {
            var result = _service.GetAll();
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return PartialView("_ContactList", result.Data ?? new List<ContactDto>());
        }

        // GET /contacts/search?query=ann
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? query)
        {
            var result = _service.Search(query);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return PartialView("_ContactList", result.Data ?? new List<ContactDto>());
        }

        // GET /contacts/{id}
        [HttpGet("{id:guid}")]
        public IActionResult Get(Guid id)
        {
            var result = _service.GetById(id);
            if (!result.Success || result.Data is null)
                return NotFound(new { message = result.Message });

            return Ok(result.Data);
        }

        // POST /contacts
        [ValidateAntiForgeryToken]
        [HttpPost("")]
        public IActionResult Create([FromBody] ContactDto dto)
        {
            var result = _service.Add(dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // PUT /contacts/{id}
        [ValidateAntiForgeryToken]
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] ContactDto dto)
        {
            var result = _service.Update(id, dto);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result.Data);
        }

        // DELETE /contacts/{id}
        [ValidateAntiForgeryToken]
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var result = _service.Delete(id);
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return NoContent();
        }
    }
}
