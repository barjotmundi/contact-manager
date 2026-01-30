using ContactManager.Models;
using ContactManager.Dtos;
using ContactManager.Utilities;

namespace ContactManager.Services
{
    public interface IContactService
    {
        IEnumerable<ContactDto> GetAll();
        ContactDto? GetById(Guid id);
        OperationResult Add(ContactDto contact);
        OperationResult Update(Guid id, ContactDto contact);
        OperationResult Delete(Guid id);
        IEnumerable<ContactDto> Search(string query);
    }
}
