using ContactManager.Models;
using ContactManager.Dtos;
using ContactManager.Repositories;
using ContactManager.Utilities;
using System.Text.RegularExpressions;

namespace ContactManager.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;

        public ContactService(IContactRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ContactDto> GetAll()
        {
            var contacts = _repository.GetAll();

            return contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            });
        }

        public ContactDto? GetById(Guid id)
        {
            var contact = _repository.GetById(id);
            if (contact == null)
                return null;

            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone
            };
        }

        public OperationResult Add(ContactDto contactDto)
        {
            var validation = ValidateContactDto(contactDto);

            if (!validation.Success)
                return validation;

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = contactDto.Name!.Trim(),
                Email = contactDto.Email!.Trim(),
                Phone = contactDto.Phone!.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var result = _repository.Add(contact);
            if (!result.Success) return result;

            var createdDto = new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                Phone = contact.Phone
            };

            return OperationResult.Ok(createdDto);
        }

        public OperationResult Update(Guid id, ContactDto contactDto)
        {
            var validation = ValidateContactDto(contactDto);
            if (!validation.Success)
                return validation;

            var existingContact = _repository.GetById(id);
            if (existingContact == null)
                return OperationResult.Fail($"Contact with Id {id} not found.");

            existingContact.Name = contactDto.Name!.Trim();
            existingContact.Email = contactDto.Email!.Trim();
            existingContact.Phone = contactDto.Phone!.Trim();
            existingContact.UpdatedAt = DateTime.UtcNow;

            return _repository.Update(existingContact);
        }

        public OperationResult Delete(Guid id)
        {
            return _repository.Delete(id);
        }

        public IEnumerable<ContactDto> Search(string query)
        {
            var contacts = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.ToLower();
                contacts = contacts.Where(c =>
                    (c.Name?.ToLower().Contains(query) ?? false) ||
                    (c.Email?.ToLower().Contains(query) ?? false));
            }

            return contacts.Select(c => new ContactDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Phone = c.Phone
            });
        }

        private OperationResult ValidateContactDto(ContactDto contactDto)
        {
            if (contactDto == null)
                return OperationResult.Fail("Contact payload cannot be null.");

            if (string.IsNullOrWhiteSpace(contactDto.Name))
                return OperationResult.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(contactDto.Email))
                return OperationResult.Fail("Email is required.");

            if (!IsValidEmail(contactDto.Email!))
                return OperationResult.Fail("Email is not valid.");

            if (string.IsNullOrWhiteSpace(contactDto.Phone))
                return OperationResult.Fail("Phone number is required.");

            if (!IsValidPhone(contactDto.Phone!))
                return OperationResult.Fail("Phone number is not valid.");

            return OperationResult.Ok();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            return Regex.IsMatch(email,
                @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
                RegexOptions.IgnoreCase);
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;

            var digitsOnly = Regex.Replace(phone, @"[^\d]", "");
            if (digitsOnly.Length < 7 || digitsOnly.Length > 15)
                return false;

            return Regex.IsMatch(phone, @"^\+?[0-9\s\-\(\)]+$");
        }

    }
}
