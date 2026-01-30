using ContactManager.Dtos;
using ContactManager.Services;
using ContactManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{
    [ApiController]
    [Route("contacts")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactController(IContactService service)
        {
            _service = service;
        }

        // GET /contacts
        [HttpGet]
        public ActionResult<IEnumerable<ContactDto>> GetAll()
        {
            var contacts = _service.GetAll();
            return Ok(contacts);
        }

        // GET /contacts/{id}
        [HttpGet("{id:guid}")]
        public ActionResult<ContactDto> Get(Guid id)
        {
            var contact = _service.GetById(id);

            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        // GET /contacts/search?q=joe
        [HttpGet("search")]
        public ActionResult<IEnumerable<ContactDto>> Search(string q)
        {
            var results = _service.Search(q);
            return Ok(results);
        }

        // POST /contacts
        [HttpPost]
        public ActionResult<ContactDto> Create([FromBody] ContactDto contactDto)
        {
            var result = _service.Add(contactDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Data);
        }

        // PUT /contacts/{id}
        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, [FromBody] ContactDto contactDto)
        {

            var result = _service.Update(id, contactDto);

            if (!result.Success)
                return BadRequest(result.Message);

            return NoContent();
        }

        // DELETE /contacts/{id}
        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            var result = _service.Delete(id);

            if (!result.Success)
                return NotFound(result.Message);

            return NoContent();
        }
    }
}
