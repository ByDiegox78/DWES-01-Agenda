using Agenda.Entity;
using Agenda.Error.ContactoError;
using Agenda.Models;
using Agenda.Repository;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Agenda_Test.Repository;

[TestFixture]
public class ContactoRespositoryTest {
    [TestFixture]
    public class CasosPositivos {
        private SqliteConnection _connection = null!;
        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new ContactoRepository(_context, dropData: true);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private AppDbContext _context = null!;
        private ContactoRepository _repository = null!;

        [Test]
        public void Create_CrearContacto_CreaCorrectamente() {
            var contacto = new Contacto {
                Nombre = "Juan",
                Alias = "juan",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            var res = _repository.Create(contacto);
            res.IsSuccess.Should().BeTrue();
            res.Value.Id.Should().Be(1);
            res.Value.Nombre.Should().Be("Juan");
            res.Value.Alias.Should().Be("juan");
        }

        [Test]
        public void GetById_ConContactoExistente_DevuelveContacto() {
            var contacto = new Contacto {
                Nombre = "Juan",
                Alias = "juan",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            _repository.Create(contacto);
            var res = _repository.GetById(1);
            
            res.IsSuccess.Should().BeTrue();
            res.Value.Should().NotBeNull();
            res.Value.Id.Should().Be(1);
            res.Value.Nombre.Should().Be("Juan");
            res.Value.Alias.Should().Be("juan");
        }

        [Test]
        public void Update_ConContactoExistente_ActualizaContacto() {
            var contacto = new Contacto {
                Nombre = "Juan",
                Alias = "juan",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            var act = new Contacto {
                Nombre = "Antonio",
                Alias = "antonio",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
            };
            _repository.Create(contacto);
            var res = _repository.Update(1, act);
            res.IsSuccess.Should().BeTrue();
            res.Value.Nombre.Should().Be("Antonio");
            res.Value.Alias.Should().Be("antonio");
            
        }   
        [Test]
        public void GetAll_ConPaginacion_DebeDevolverPaginado() {
            _repository.Create(new Contacto {
                Nombre = "Ana", Alias = "ana1", Telefono = "600111001", Email = "ana1@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            });
            _repository.Create(new Contacto {
                Nombre = "Bernardo", Alias = "berni", Telefono = "600111002", Email = "berni@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            });
            _repository.Create(new Contacto {
                Nombre = "Carlos", Alias = "charly", Telefono = "600111003", Email = "carlos@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            });
            _repository.Create(new Contacto {
                Nombre = "Diana", Alias = "diana", Telefono = "600111004", Email = "diana@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            });
            var res = _repository.GetAll(page: 1, pageSize: 2);

            res.Should().NotBeNull();
            res.Should().HaveCount(2);
        }
        [Test]
        public void GetByAlias_ConContactoExistente_DevuelveContacto() {
            var contacto = new Contacto {
                Nombre = "Juan",
                Alias = "juan",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            _repository.Create(contacto);
            var res = _repository.GetByAlias("juan");

            res.Should().NotBeNull();
            res.Alias.Should().Be("juan");
        }
        [Test]
        public void Delete_ConContactoExistente_ElimnaContacto() {
            var contacto = new Contacto {
                Nombre = "Juan",
                Alias = "juan",
                Telefono = "123456789",
                Email = "dfsdfsdfsdfs",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            _repository.Create(contacto);
            var res = _repository.Delete(1);
            
            res.IsSuccess.Should().BeTrue();
            _repository.GetById(1).IsFailure.Should().BeTrue();
        }
    }

    [TestFixture]
    public class CasosNegativos {
        private SqliteConnection _connection = null!;
        [SetUp]
        public void SetUp() {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;
            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();
            _repository = new ContactoRepository(_context, dropData: true);
        }

        [TearDown]
        public void TearDown() {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        private AppDbContext _context = null!;
        private ContactoRepository _repository = null!;
        [Test]
        public void Create_CrearExistente_DebeDevolverError() {
            var contacto = new Contacto { Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "dfsdfsdfsdfs", CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow, IsDeleted = false };
            var contacto2 = new Contacto { Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "dfsdfsdfsdfs", CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow, IsDeleted = false };

            _repository.Create(contacto);
            var res = _repository.Create(contacto2);

            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.NameAlreadyExists>();
            res.Error.Message.Should()
                .Contain(
                    "El nombre Juan ya existe");
        }
        [Test]
        public void Update_ActualizarInexistente_DebeDevolverError() {
            var contacto = new Contacto { Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "dfsdfsdfsdfs", CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow, IsDeleted = false };

            var res = _repository.Update(1, contacto);

            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.NotFound>();
            res.Error.Message.Should()
                .Contain(
                    "No se ha encontrado ningun contacto con id: 1");
        }
        [Test]
        public void Delete_EliminarInexistente_DebeDevolverError() {
            var res = _repository.Delete(1);
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.NotFound>();
        }
        [Test]
        public void GetByAlias_ConAliasInexistente_DebeDevolverError() {
            var res = _repository.GetByAlias("juan");
            res.Should().BeNull();
        }
        [Test]
        public void Create_ConErrorDeBaseDeDatos_DevuelveFailure() {
            var contacto = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = null!, Email = "juan@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            };
            var res = _repository.Create(contacto);
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.DatabaseError>();
        }
        [Test]
        public void Update_ConErrorDeBaseDeDatos_DevuelveFailure() {
            var original = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            };
            _repository.Create(original);

            var invalido = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = null!, Email = "juan@test.com"
            };
            var res = _repository.Update(1, invalido);
            res.IsFailure.Should().BeTrue();
            res.Error.Should().BeOfType<ContactoError.DatabaseError>();
        }
    }
    
   
}