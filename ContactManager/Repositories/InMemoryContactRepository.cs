using ContactManager.Models;
using ContactManager.Utilities;

namespace ContactManager.Repositories
{
    public class InMemoryContactRepository : IContactRepository
    {
        private readonly List<Contact> _contacts = new();

        public IEnumerable<Contact> GetAll()
        {
            return _contacts;
        }

        public Contact? GetById(Guid id)
        {
            return _contacts.FirstOrDefault(c => c.Id == id);
        }

        public OperationResult Add(Contact contact)
        {
            if (contact == null)
                return OperationResult.Fail("Contact cannot be null.");

            _contacts.Add(contact);
            return OperationResult.Ok();
        }

        public OperationResult Update(Contact contact)
        {
            if (contact == null)
                return OperationResult.Fail("Contact cannot be null.");

            var existing = GetById(contact.Id);
            if (existing == null)
                return OperationResult.Fail($"Contact with Id {contact.Id} not found.");

            if (!string.IsNullOrWhiteSpace(contact.Name))
                existing.Name = contact.Name;
            if (!string.IsNullOrWhiteSpace(contact.Email))
                existing.Email = contact.Email;
            if (!string.IsNullOrWhiteSpace(contact.Phone))
                existing.Phone = contact.Phone;
            if (contact.UpdatedAt.HasValue)
                existing.UpdatedAt = contact.UpdatedAt;

            return OperationResult.Ok();
        }

        public OperationResult Delete(Guid id)
        {
            var contact = GetById(id);
            if (contact == null)
                return OperationResult.Fail($"Contact with Id {id} not found.");

            _contacts.Remove(contact);
            return OperationResult.Ok();
        }
    }
}
