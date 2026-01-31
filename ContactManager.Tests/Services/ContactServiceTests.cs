using ContactManager.Dtos;
using ContactManager.Models;
using ContactManager.Repositories;
using ContactManager.Services;
using Moq;

namespace ContactManager.Tests.Services
{
    public class ContactServiceTests
    {
        private static ContactDto MakeDto(
            string name = "Ethan Gurne",
            string email = "ethan.gurne@gmail.com",
            string phone = "(780)-555-1234")
        {
            return new ContactDto
            {
                Name = name,
                Email = email,
                Phone = phone
            };
        }

        // ----------------------------
        // GetAll / GetById mapping
        // -----------------------------

        [Fact]
        public void GetAll_ReturnsDtosForEachContact()
        {
            var repo = new Mock<IContactRepository>();

            var ethanId = Guid.NewGuid();
            var barjotId = Guid.NewGuid();

            var contacts = new List<Contact>
            {
                new Contact { Id = ethanId, Name = "Ethan Gurne", Email = "ethan.gurne@gmail.com", Phone = "(780)-555-1234" },
                new Contact { Id = barjotId, Name = "Barjot Mundi", Email = "barjot.mundi@gmail.com", Phone = "(604)-555-7788" }
            };

            repo.Setup(r => r.GetAll()).Returns(contacts);

            var service = new ContactService(repo.Object);

            var result = service.GetAll();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            var dtos = result.Data!;
            Assert.Equal(2, dtos.Count);

            Assert.Equal(ethanId, dtos[0].Id);
            Assert.Equal("Ethan Gurne", dtos[0].Name);
            Assert.Equal("ethan.gurne@gmail.com", dtos[0].Email);
            Assert.Equal("(780)-555-1234", dtos[0].Phone);

            Assert.Equal(barjotId, dtos[1].Id);
            Assert.Equal("Barjot Mundi", dtos[1].Name);
            Assert.Equal("barjot.mundi@gmail.com", dtos[1].Email);
            Assert.Equal("(604)-555-7788", dtos[1].Phone);

            repo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void GetAll_ReturnsEmptyList_WhenRepositoryHasNoContacts()
        {
            var repo = new Mock<IContactRepository>();

            repo.Setup(r => r.GetAll()).Returns(new List<Contact>());

            var service = new ContactService(repo.Object);

            var result = service.GetAll();

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data!);

            repo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_ReturnsDto_WhenFound()
        {
            var repo = new Mock<IContactRepository>();
            var id = Guid.NewGuid();

            var contact = new Contact
            {
                Id = id,
                Name = "Sandeep Mundi",
                Email = "sandeep.mundi@gmail.com",
                Phone = "(778)-555-9911"
            };

            repo.Setup(r => r.GetById(id)).Returns(contact);

            var service = new ContactService(repo.Object);

            var result = service.GetById(id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            var dto = result.Data!;
            Assert.Equal(id, dto.Id);
            Assert.Equal("Sandeep Mundi", dto.Name);
            Assert.Equal("sandeep.mundi@gmail.com", dto.Email);
            Assert.Equal("(778)-555-9911", dto.Phone);

            repo.Verify(r => r.GetById(id), Times.Once);
        }

        [Fact]
        public void GetById_ReturnsFailure_WhenMissing()
        {
            var repo = new Mock<IContactRepository>();
            var missingId = Guid.NewGuid();

            repo.Setup(r => r.GetById(missingId)).Returns((Contact?)null);

            var service = new ContactService(repo.Object);

            var result = service.GetById(missingId);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Contact not found.", result.Message);

            repo.Verify(r => r.GetById(missingId), Times.Once);
        }

        // -----------------------
        // Add validation + behavior
        // -----------------------

        [Fact]
        public void Add_RejectsBlankName_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            var result = service.Add(MakeDto(name: "   "));

            Assert.False(result.Success);
            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_RejectsBlankEmail_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            var result = service.Add(MakeDto(email: "   "));

            Assert.False(result.Success);
            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_RejectsBadEmail_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            var result = service.Add(MakeDto(email: "ethan.gurne(at)gmail.com"));

            Assert.False(result.Success);
            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_RejectsBlankPhone_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            var result = service.Add(MakeDto(phone: "   "));

            Assert.False(result.Success);
            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_RejectsBadPhone_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            // Not matching: (###)-###-####
            var result = service.Add(MakeDto(phone: "12-3"));

            Assert.False(result.Success);
            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_TrimsInputs_AndSaves_WhenValid()
        {
            var repo = new Mock<IContactRepository>();

            // Repo returns the created contact (common pattern for in-memory repos)
            repo.Setup(r => r.Add(It.IsAny<Contact>()))
                .Returns((Contact c) => c);

            var service = new ContactService(repo.Object);

            var dto = MakeDto(
                name: "  Barjot Mundi  ",
                email: "  barjot.mundi@gmail.com  ",
                phone: "  (780)-555-3322  "
            );

            var result = service.Add(dto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            // Validate repo got trimmed + properly populated Contact
            repo.Verify(r => r.Add(It.Is<Contact>(c =>
                c.Id != Guid.Empty &&
                c.Name == "Barjot Mundi" &&
                c.Email == "barjot.mundi@gmail.com" &&
                c.Phone == "(780)-555-3322" &&
                c.CreatedAt != default &&
                c.UpdatedAt == null
            )), Times.Once);

            // Validate returned dto matches what we stored
            var createdDto = result.Data!;
            Assert.Equal("Barjot Mundi", createdDto.Name);
            Assert.Equal("barjot.mundi@gmail.com", createdDto.Email);
            Assert.Equal("(780)-555-3322", createdDto.Phone);
            Assert.NotEqual(Guid.Empty, createdDto.Id);
        }

        [Fact]
        public void Add_ReturnsCreatedDto_FromRepositoryResult()
        {
            var repo = new Mock<IContactRepository>();

            // Return a specific "created" contact (simulates repo enriching something)
            var created = new Contact
            {
                Id = Guid.NewGuid(),
                Name = "Ethan Gurne",
                Email = "ethan.gurne@gmail.com",
                Phone = "(780)-555-1234",
                CreatedAt = DateTime.UtcNow
            };

            repo.Setup(r => r.Add(It.IsAny<Contact>())).Returns(created);

            var service = new ContactService(repo.Object);

            var result = service.Add(MakeDto());

            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            Assert.Equal(created.Id, result.Data!.Id);
            Assert.Equal(created.Name, result.Data.Name);
            Assert.Equal(created.Email, result.Data.Email);
            Assert.Equal(created.Phone, result.Data.Phone);

            repo.Verify(r => r.Add(It.IsAny<Contact>()), Times.Once);
        }

        // -----------------------
        // Update validation + behavior
        // -----------------------

        [Fact]
        public void Update_RejectsBadDto_AndSkipsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var service = new ContactService(repo.Object);

            var id = Guid.NewGuid();
            var result = service.Update(id, MakeDto(email: "bad"));

            Assert.False(result.Success);
            repo.Verify(r => r.Update(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Update_Fails_WhenContactMissing()
        {
            var repo = new Mock<IContactRepository>();
            var id = Guid.NewGuid();

            repo.Setup(r => r.GetById(id)).Returns((Contact?)null);

            var service = new ContactService(repo.Object);

            var result = service.Update(id, MakeDto());

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal($"Contact with Id {id} not found.", result.Message);

            repo.Verify(r => r.GetById(id), Times.Once);
            repo.Verify(r => r.Update(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Update_UpdatesExistingContact_AndCallsRepo()
        {
            var repo = new Mock<IContactRepository>();
            var id = Guid.NewGuid();

            var existing = new Contact
            {
                Id = id,
                Name = "Barjot Mundi",
                Email = "barjot.mundi@gmail.com",
                Phone = "(604)-555-7788",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = null
            };

            repo.Setup(r => r.GetById(id)).Returns(existing);

            // Return what service sends to repo (most in-memory repos do this)
            repo.Setup(r => r.Update(It.IsAny<Contact>()))
                .Returns((Contact c) => c);

            var service = new ContactService(repo.Object);

            var dto = MakeDto(
                name: "  Barjot Mundi  ",
                email: "  barjot.mundi@updated.com  ",
                phone: "  (604)-555-1122  "
            );

            var result = service.Update(id, dto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);

            repo.Verify(r => r.Update(It.Is<Contact>(c =>
                c.Id == id &&
                c.Name == "Barjot Mundi" &&
                c.Email == "barjot.mundi@updated.com" &&
                c.Phone == "(604)-555-1122" &&
                c.CreatedAt == existing.CreatedAt &&
                c.UpdatedAt != null &&
                c.UpdatedAt != default(DateTime)
            )), Times.Once);

            var updatedDto = result.Data!;
            Assert.Equal(id, updatedDto.Id);
            Assert.Equal("Barjot Mundi", updatedDto.Name);
            Assert.Equal("barjot.mundi@updated.com", updatedDto.Email);
            Assert.Equal("(604)-555-1122", updatedDto.Phone);
        }

        [Fact]
        public void Update_ReturnsFailure_WhenRepositoryReturnsNull()
        {
            var repo = new Mock<IContactRepository>();
            var id = Guid.NewGuid();

            var existing = new Contact
            {
                Id = id,
                Name = "Ethan Gurne",
                Email = "ethan.gurne@gmail.com",
                Phone = "(780)-555-1234",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = null
            };

            repo.Setup(r => r.GetById(id)).Returns(existing);

            // Your service explicitly checks for null here
            repo.Setup(r => r.Update(It.IsAny<Contact>())).Returns((Contact?)null);

            var service = new ContactService(repo.Object);

            var result = service.Update(id, MakeDto(email: "ethan.updated@gmail.com"));

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Update failed.", result.Message);

            repo.Verify(r => r.Update(It.IsAny<Contact>()), Times.Once);
        }

        // -----------------------
        // Delete behavior
        // -----------------------

        [Fact]
        public void Delete_PassesIdToRepo()
        {
            var repo = new Mock<IContactRepository>();
            var id = Guid.NewGuid();

            repo.Setup(r => r.Delete(id)).Returns(true);

            var service = new ContactService(repo.Object);

            var result = service.Delete(id);

            Assert.True(result.Success);
            Assert.Null(result.Message);

            repo.Verify(r => r.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_ReturnsFailure_WhenContactDoesNotExist()
        {
            var repo = new Mock<IContactRepository>();
            var missingId = Guid.NewGuid();

            repo.Setup(r => r.Delete(missingId)).Returns(false);

            var service = new ContactService(repo.Object);

            var result = service.Delete(missingId);

            Assert.False(result.Success);
            Assert.Equal($"Contact with Id {missingId} not found.", result.Message);

            repo.Verify(r => r.Delete(missingId), Times.Once);
        }

        // -----------------------
        // Search behavior
        // -----------------------

        [Fact]
        public void Search_BlankQuery_ReturnsAllContacts()
        {
            var repo = new Mock<IContactRepository>();

            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "Sandeep Mundi", Email = "sandeep.mundi@gmail.com", Phone = "(778)-555-9911" },
                new Contact { Id = Guid.NewGuid(), Name = "Ethan Gurne", Email = "ethan.gurne@gmail.com", Phone = "(780)-555-3322" }
            };

            repo.Setup(r => r.GetAll()).Returns(contacts);

            var service = new ContactService(repo.Object);

            var result = service.Search("");

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data!.Count);

            repo.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void Search_FindsByNameOrEmail_IgnoresCase()
        {
            var repo = new Mock<IContactRepository>();

            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "Ethan Gurne", Email = "ethan.gurne@gmail.com", Phone = "(780)-555-1234" },
                new Contact { Id = Guid.NewGuid(), Name = "Sandeep Mundi", Email = "sandeep.mundi@gmail.com", Phone = "(778)-555-9911" }
            };

            repo.Setup(r => r.GetAll()).Returns(contacts);

            var service = new ContactService(repo.Object);

            var byName = service.Search("ethan");
            var byEmail = service.Search("GMAIL");

            Assert.True(byName.Success);
            Assert.NotNull(byName.Data);
            Assert.Single(byName.Data!);
            Assert.Equal("Ethan Gurne", byName.Data![0].Name);

            Assert.True(byEmail.Success);
            Assert.NotNull(byEmail.Data);
            Assert.Equal(2, byEmail.Data!.Count);

            repo.Verify(r => r.GetAll(), Times.Exactly(2));
        }

        [Fact]
        public void Search_ReturnsOneContact_WhenOnlyOneMatches()
        {
            var repo = new Mock<IContactRepository>();

            var contacts = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid(), Name = "Ethan Gurne", Email = "ethan.gurne@gmail.com", Phone = "(780)-555-1234" },
                new Contact { Id = Guid.NewGuid(), Name = "Sandeep Mundi", Email = "sandeep.mundi@gmail.com", Phone = "(778)-555-9911" }
            };

            repo.Setup(r => r.GetAll()).Returns(contacts);

            var service = new ContactService(repo.Object);

            var result = service.Search("ethan.gurne");

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data!);
            Assert.Equal("Ethan Gurne", result.Data![0].Name);
            Assert.Equal("ethan.gurne@gmail.com", result.Data![0].Email);

            repo.Verify(r => r.GetAll(), Times.Once);
        }
    }
}
