using ContactManager.Dtos;
using ContactManager.Utilities;

namespace ContactManager.Services
{
    public interface IContactService
    {
        OperationResult<List<ContactDto>> GetAll();
        OperationResult<ContactDto> GetById(Guid id);
        OperationResult<ContactDto> Add(ContactDto? contactDto);
        OperationResult<ContactDto> Update(Guid id, ContactDto? contactDto);
        OperationResult Delete(Guid id);
        OperationResult<List<ContactDto>> Search(string? query);
    }
}
