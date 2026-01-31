using ContactManager.Dtos;
using ContactManager.Models;
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

        public OperationResult<List<ContactDto>> GetAll()
        {
            // Pull all contacts from storage and map them to DTOs
            var contacts = _repository.GetAll();
            var dtos = contacts.Select(ToDto).ToList();
            return OperationResult<List<ContactDto>>.Ok(dtos);
        }

        public OperationResult<ContactDto> GetById(Guid id)
        {
            // Return a clear not found result instead of letting null flow upward
            var contact = _repository.GetById(id);
            if (contact is null)
                return OperationResult<ContactDto>.Fail("Contact not found.");

            return OperationResult<ContactDto>.Ok(ToDto(contact));
        }

        public OperationResult<ContactDto> Add(ContactDto? contactDto)
        {
            if (contactDto is null)
                return OperationResult<ContactDto>.Fail("Contact payload cannot be null.");

            var validation = ValidateContactDto(contactDto);
            if (!validation.Success)
                return OperationResult<ContactDto>.Fail(validation.Message ?? "Validation failed.");

            // Trim inputs once so stored data is normalized
            var name = contactDto.Name!.Trim();
            var email = contactDto.Email!.Trim();
            var phone = contactDto.Phone!.Trim();

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                Phone = phone,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var created = _repository.Add(contact);
            return OperationResult<ContactDto>.Ok(ToDto(created));
        }


        public OperationResult<ContactDto> Update(Guid id, ContactDto? contactDto)
        {
            if (contactDto is null)
                return OperationResult<ContactDto>.Fail("Contact payload cannot be null.");

            var validation = ValidateContactDto(contactDto);
            if (!validation.Success)
                return OperationResult<ContactDto>.Fail(validation.Message ?? "Validation failed.");

            // Confirm the record exists before attempting to update it
            var existingContact = _repository.GetById(id);
            if (existingContact is null)
                return OperationResult<ContactDto>.Fail($"Contact with Id {id} not found.");

            var name = contactDto.Name!.Trim();
            var email = contactDto.Email!.Trim();
            var phone = contactDto.Phone!.Trim();

            // Build a new Contact to store and preserve CreatedAt from the original record
            var toStore = new Contact
            {
                Id = existingContact.Id,
                Name = name,
                Email = email,
                Phone = phone,
                CreatedAt = existingContact.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
            };

            // Treat a null update as a failure and return a user facing message
            var updatedContact = _repository.Update(toStore);
            if (updatedContact is null)
                return OperationResult<ContactDto>.Fail("Update failed.");

            return OperationResult<ContactDto>.Ok(ToDto(updatedContact));
        }

        public OperationResult Delete(Guid id)
        {
            var deleted = _repository.Delete(id);
            if (!deleted)
                return OperationResult.Fail($"Contact with Id {id} not found.");

            return OperationResult.Ok();
        }

        public OperationResult<List<ContactDto>> Search(string? query)
        {
            IEnumerable<Contact> contacts = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(query))
            {
                // Trim once and search name or email using case insensitive contains
                var q = query.Trim();

                contacts = contacts.Where(c =>
                    (!string.IsNullOrWhiteSpace(c.Name) && c.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrWhiteSpace(c.Email) && c.Email.Contains(q, StringComparison.OrdinalIgnoreCase)));
            }

            var results = contacts.Select(ToDto).ToList();
            return OperationResult<List<ContactDto>>.Ok(results);
        }

        private static ContactDto ToDto(Contact c) => new ContactDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone
        };

        private OperationResult ValidateContactDto(ContactDto contactDto)
        {
            var name = contactDto.Name?.Trim();
            var email = contactDto.Email?.Trim();
            var phone = contactDto.Phone?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return OperationResult.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(email))
                return OperationResult.Fail("Email is required.");

            if (!IsValidEmail(email))
                return OperationResult.Fail("Email is not valid.");

            if (string.IsNullOrWhiteSpace(phone))
                return OperationResult.Fail("Phone number is required.");

            if (!IsValidPhone(phone))
                return OperationResult.Fail("Phone number is not valid.");

            return OperationResult.Ok();
        }

        private bool IsValidEmail(string email)
        {
            // Simple regex based email validation for basic formatting checks
            if (string.IsNullOrWhiteSpace(email)) return false;

            return Regex.IsMatch(email,
                @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
                RegexOptions.IgnoreCase);
        }

        private bool IsValidPhone(string phone)
        {
            // Enforces a specific stored format
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(
                phone.Trim(),
                @"^\(\d{3}\)-\d{3}-\d{4}$"
            );
        }
    }
}
