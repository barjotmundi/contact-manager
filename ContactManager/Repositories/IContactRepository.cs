using ContactManager.Models;

namespace ContactManager.Repositories
{
    public interface IContactRepository
    {
        IReadOnlyList<Contact> GetAll();
        Contact? GetById(Guid id);
        Contact Add(Contact contact);
        Contact? Update(Contact contact);
        bool Delete(Guid id);
    }
}
