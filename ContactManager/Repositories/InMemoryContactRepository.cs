using ContactManager.Models;

namespace ContactManager.Repositories
{
    public class InMemoryContactRepository : IContactRepository
    {
        private readonly List<Contact> _contacts = new();
        private readonly object _lock = new();

        // We copy Contacts in/out so callers can't accidentally mutate our in-memory "store"
        // by editing an object they got back from GetAll/GetById.
        private static Contact Copy(Contact c) => new Contact
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };

        public IReadOnlyList<Contact> GetAll()
        {
            lock (_lock) // Locking prevents two requests from changing the list at the same time.
            {
                return _contacts.Select(Copy).ToList();
            }
        }

        public Contact? GetById(Guid id)
        {
            lock (_lock)
            {
                var c = _contacts.FirstOrDefault(x => x.Id == id);
                return c is null ? null : Copy(c);
            }
        }

        public Contact Add(Contact contact)
        {
            lock (_lock)
            {
                var stored = Copy(contact); // store our own copy, keep internal state isolated
                _contacts.Add(stored);
                return Copy(stored);        // return a copy for the same reason
            }
        }

        public Contact? Update(Contact contact)
        {
            lock (_lock)
            {
                var index = _contacts.FindIndex(c => c.Id == contact.Id);
                if (index < 0) return null; // not found

                _contacts[index] = Copy(contact);
                return Copy(_contacts[index]);
            }
        }

        public bool Delete(Guid id)
        {
            lock (_lock)
            {
                var index = _contacts.FindIndex(c => c.Id == id);
                if (index < 0) return false;

                _contacts.RemoveAt(index);
                return true;
            }
        }
    }
}
