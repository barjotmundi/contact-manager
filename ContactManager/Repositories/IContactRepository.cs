using ContactManager.Models;
using ContactManager.Utilities;


namespace ContactManager.Repositories
{
    public interface IContactRepository
    {
        IEnumerable<Contact> GetAll();
        Contact? GetById(Guid id);
        OperationResult Add(Contact contact);
        OperationResult Update(Contact contact);
        OperationResult Delete(Guid id);
    }
}
