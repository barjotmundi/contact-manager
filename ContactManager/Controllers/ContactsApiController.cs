using ContactManager.Dtos;
using ContactManager.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/contacts")]
public class ContactsApiController : ControllerBase
{
    private readonly IContactService _service;

    public ContactsApiController(IContactService service) => _service = service;

    // GET /api/contacts/{id}
    [HttpGet("{id:guid}")]
    public ActionResult<ContactDto> Get(Guid id)
    {
        var result = _service.GetById(id);
        if (!result.Success || result.Data is null)
            return NotFound(new { message = result.Message });

        return Ok(result.Data);
    }

    // POST /api/contacts
    [HttpPost]
    public ActionResult<ContactDto> Create([FromBody] ContactDto contactDto)
    {
        var result = _service.Add(contactDto);
        if (!result.Success || result.Data is null)
            return BadRequest(new { message = result.Message });

        var created = result.Data;
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // PUT /api/contacts/{id}
    [HttpPut("{id:guid}")]
    public ActionResult<ContactDto> Update(Guid id, [FromBody] ContactDto contactDto)
    {
        var result = _service.Update(id, contactDto);
        if (!result.Success || result.Data is null)
            return BadRequest(new { message = result.Message });

        return Ok(result.Data);
    }

    // DELETE /api/contacts/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var result = _service.Delete(id);
        if (!result.Success)
            return NotFound(new { message = result.Message });

        return NoContent();
    }
}
