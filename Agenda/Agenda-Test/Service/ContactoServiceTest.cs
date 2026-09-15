using Agenda.Models;
using Agenda.Repository;
using Agenda.Service;
using FluentAssertions;
using GestionEsports.Esports.Error.Common;
using Moq;
using Vehiculos.Cache;
using CSharpFunctionalExtensions;

namespace Agenda_Test.Service;

[TestFixture]
public class ContactoServiceTest {
    [SetUp]
    public void Setup() {
        _repository = new Mock<IContactoRepository>();
        _cache = new Mock<ICached<int, Contacto>>();
        _service = new ContactoService(_repository.Object, _cache.Object);
    }
    private ContactoService _service = null!;
    private Mock<IContactoRepository> _repository = null!;
    private Mock<ICached<int, Contacto>> _cache = null!;
    
    [TestFixture]
    public class CasosPositivos : ContactoServiceTest {
        [Test]
        public void GetAll_SinParametro_DevuelveTodo() {
            var contactos = new List<Contacto> {
                new() { Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com" },
                new() { Nombre = "Ana", Alias = "ana", Telefono = "987654321", Email = "ana@test.com" }
            };
            _repository.Setup(r => r.GetAll(1, 3)).Returns(contactos);

            var resultado = _service.GetAll().ToList();

            resultado.Should().HaveCount(2);
            _repository.Verify(r => r.GetAll(1, 3), Times.Once);
        }
        [Test]
        public void GetById_ConCache_DevuelveDeLaCache() {
            var original = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            };
            _cache.Setup(c => c.Get(1)).Returns(original);
            var res = _service.GetById(1);
            
            res.IsSuccess.Should().BeTrue();
            res.Value.Nombre.Should().Be("Juan");
            _cache.Verify(c => c.Get(1), Times.Once);
            _repository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        }
        [Test]
        public void GetById_SinCache_RetornaDesdeRepo() {
            var original = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com",
                CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, IsDeleted = false
            };
            _cache.Setup(c => c.Get(1)).Returns((Contacto?)null);
            _repository.Setup(r => r.GetById(1)).Returns(original);
            
            var res = _service.GetById(1);
            
            res.IsSuccess.Should().BeTrue();
            res.Value.Nombre.Should().Be("Juan");
            _cache.Verify(c => c.Get(1), Times.Once);
            _cache.Verify(c => c.Add(1, original), Times.Once);
            _repository.Verify(r => r.GetById(1), Times.Once);
        }
        [Test]
        public void Create_ConContactoValido_DebeGuardar() {
            var contacto = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com"
            };
            var creado = contacto with { Id = 1 };
            _repository.Setup(r => r.Create(It.IsAny<Contacto>()))
                .Returns(Result.Success<Contacto, DomainError>(creado));

            var res = _service.Create(contacto);

            res.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.Create(It.IsAny<Contacto>()), Times.Once);
            _cache.Verify(c => c.Add(It.IsAny<int>(), It.IsAny<Contacto>()), Times.Once);
        }
        [Test]
        public void Update_ConContactoExistente_DebeActualizarYLimpiarCache() {
            var contacto = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com"
            };
            var actualizado = new Contacto { Nombre = "Antonio", Alias = "antonio" };
            
            _repository.Setup(r => r.GetById(1))
                .Returns(Result.Success<Contacto, DomainError>(contacto));
            _repository.Setup(r => r.Update(1, It.IsAny<Contacto>()))
                .Returns(Result.Success<Contacto, DomainError>(actualizado));

            var res = _service.Update(1, actualizado);

            res.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.GetById(1), Times.Once);
            _repository.Verify(r => r.Update(1, It.IsAny<Contacto>()), Times.Once);
            _cache.Verify(c => c.Remove(1), Times.Once);
        }

        [Test]
        public void Delete_ConContactoExistente_DebeEliminarYLimpiarCache() {
            var contacto = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com"
            };
            _repository.Setup(r => r.GetById(1))
                .Returns(Result.Success<Contacto, DomainError>(contacto));
            _repository.Setup(r => r.Delete(1))
                .Returns(Result.Success<Contacto, DomainError>(contacto));

            var res = _service.Delete(1);

            res.IsSuccess.Should().BeTrue();
            _repository.Verify(r => r.GetById(1), Times.Once);
            _repository.Verify(r => r.Delete(1), Times.Once);
            _cache.Verify(c => c.Remove(1), Times.Once);
        }
        [Test]
        public void GetByAlias_ConAliasExistente_DevuelveContacto() {
            var contacto = new Contacto {
                Nombre = "Juan", Alias = "juan", Telefono = "123456789", Email = "juan@test.com"
            };
            _repository.Setup(r => r.GetByAlias("juan")).Returns(contacto);

            var res = _service.GetByAlias("juan");

            res.Should().NotBeNull();
            res.Alias.Should().Be("juan");
            _repository.Verify(r => r.GetByAlias("juan"), Times.Once);
        }
        [Test]
        public void GetByAlias_ConAliasInexistente_DevuelveNull() {
            _repository.Setup(r => r.GetByAlias("juan")).Returns((Contacto?)null);

            var res = _service.GetByAlias("juan");

            res.Should().BeNull();
            _repository.Verify(r => r.GetByAlias("juan"), Times.Once);
        }
    }
}