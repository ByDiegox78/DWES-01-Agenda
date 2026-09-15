using Agenda.Entity;
using Agenda.Error.ContactoError;
using Agenda.Enums;
using Agenda.Models;
using Agenda.Repository;
using Agenda.Service;
using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;
using Vehiculos.Cache;

using var context = new AppDbContext("Data Source=agenda.db");

// dropData: true recrea la BD limpia en cada ejecución (solo para pruebas)
var repository = new ContactoRepository(context, dropData: true);
var cache = new CacheLru<int, Contacto>(3);
var service = new ContactoService(repository, cache);

// ═══════════════ CREATE (6 contactos) ═══════════════
var datos = new[] {
    new Contacto { Nombre = "Juan",    Alias = "juan",    Telefono = "123456789", Email = "juan@test.com" },
    new Contacto { Nombre = "Ana",     Alias = "ana",     Telefono = "987654321", Email = "ana@test.com" },
    new Contacto { Nombre = "Carlos",  Alias = "charly",  Telefono = "600111222", Email = "carlos@test.com" },
    new Contacto { Nombre = "Marta",   Alias = "marta",   Telefono = "600333444", Email = "marta@test.com" },
    new Contacto { Nombre = "Diego",   Alias = "diego",   Telefono = "600555666", Email = "diego@test.com" },
    new Contacto { Nombre = "Lucia",   Alias = "lucia",   Telefono = "600777888", Email = "lucia@test.com" },
};

Console.WriteLine("── Create (6 contactos) ──");
foreach (var c in datos) {
    var r = service.Create(c);
    Responder(HttpVerb.Post, r, HttpCodes.Created);
}

// ═══════════════ GETALL (paginación) ═══════════════
Console.WriteLine("\n── GetAll página 1, tamaño 3 ──");
foreach (var c in service.GetAll(1, 3))
    Console.WriteLine($"  [{c.Id}] {c.Nombre} ({c.Alias}) - {c.Telefono}");

Console.WriteLine("\n── GetAll página 2, tamaño 3 ──");
foreach (var c in service.GetAll(2, 3))
    Console.WriteLine($"  [{c.Id}] {c.Nombre} ({c.Alias}) - {c.Telefono}");

// ═══════════════ GETBYID (varios ids) ═══════════════
Console.WriteLine("\n── GetById(1) ──");
Responder(HttpVerb.Get, service.GetById(1));

Console.WriteLine("\n── GetById(3) ──");
Responder(HttpVerb.Get, service.GetById(3));

Console.WriteLine("\n── GetById(5) ──");
Responder(HttpVerb.Get, service.GetById(5));

// ═══════════════ GETBYALIAS (varios) ═══════════════
foreach (var alias in new[] { "marta", "lucia", "pepe" }) {
    Console.WriteLine($"\n── GetByAlias(\"{alias}\") ──");
    var ga = service.GetByAlias(alias);
    Console.WriteLine(ga is null ? "  No encontrado (null)" : $"  [{ga.Id}] {ga.Nombre} ({ga.Alias}) - {ga.Telefono}");
}

// ═══════════════ UPDATE (varios) ═══════════════
Console.WriteLine("\n── Update(1, Antonio) ──");
var antonio = new Contacto { Nombre = "Antonio", Alias = "antonio", Telefono = "666555444", Email = "antonio@test.com" };
Responder(HttpVerb.Put, service.Update(1, antonio));

Console.WriteLine("\n── Update(3, Carmen) ──");
var carmen = new Contacto { Nombre = "Carmen", Alias = "carmen", Telefono = "600111333", Email = "carmen@test.com" };
Responder(HttpVerb.Put, service.Update(3, carmen));

// ═══════════════ DELETE (varios) ═══════════════
Console.WriteLine("\n── Delete(4) ──");
Responder(HttpVerb.Delete, service.Delete(4));

Console.WriteLine("\n── Delete(6) ──");
Responder(HttpVerb.Delete, service.Delete(6));

// ═══════════════ FALLOS ═══════════════

Console.WriteLine("\n── Fallo: alias duplicado (juan ya existe) ──");
var dupe = new Contacto { Nombre = "Otro Juan", Alias = "juan", Telefono = "111", Email = "x@test.com" };
Responder(HttpVerb.Post, service.Create(dupe), HttpCodes.Created);

Console.WriteLine("\n── Fallo: Update(999) inexistente ──");
Responder(HttpVerb.Put, service.Update(999, antonio));

Console.WriteLine("\n── Fallo: Delete(999) inexistente ──");
Responder(HttpVerb.Delete, service.Delete(999));

Console.WriteLine("\n── Fallo: GetById(999) inexistente ──");
Responder(HttpVerb.Get, service.GetById(999));

Console.WriteLine("\n── Fallo: GetByAlias(\"pepe\") inexistente ──");
var aliasFallo = service.GetByAlias("pepe");
Console.WriteLine(aliasFallo is null ? "  No encontrado (null)" : "  Inesperado");

Console.WriteLine("\n── Fallo: DB (Telefono = null) ──");
var sinTel = new Contacto { Nombre = "SinTelefono", Alias = "sintel", Telefono = null!, Email = "s@test.com" };
Responder(HttpVerb.Post, service.Create(sinTel), HttpCodes.Created);

// ═══════════════ CACHÉ (capacidad 3) ═══════════════
Console.WriteLine("\n── GetById 1,2,3,1,4 (prueba LRU) ──");
foreach (var id in new[] { 1, 2, 3, 1, 4 }) {
    var r = service.GetById(id);
    Console.WriteLine(r.IsSuccess ? $"  GetById({id}) -> {r.Value.Nombre}" : $"  GetById({id}) -> {r.Error.Message}");
}

// ═══════════════ Estado final ═══════════════
Console.WriteLine("\n── GetAll tras los cambios (página 1, tamaño 10) ──");
foreach (var c in service.GetAll(1, 10))
    Console.WriteLine($"  [{c.Id}] {c.Nombre} ({c.Alias}) - {c.Telefono}");

// ═══════════════ Funciones locales ═══════════════

static HttpCodes Codigo(Result<Contacto, DomainError> res, HttpCodes ok) {
    if (res.IsSuccess) return ok;
    return res.Error switch {
        ContactoError.NotFound           => HttpCodes.NotFound,             // 404
        ContactoError.NameAlreadyExists  => HttpCodes.Conflict,            // 409
        ContactoError.DatabaseError      => HttpCodes.InternalServerError, // 500
        _                                => HttpCodes.BadQuestry           // 400: entrada inválida
    };
}

static void Responder(HttpVerb verbo, Result<Contacto, DomainError> res, HttpCodes ok = HttpCodes.Ok) {
    var codigo = Codigo(res, ok);
    Console.WriteLine($"  {verbo} /contactos -> {(int)codigo} {codigo}");
    if (res.IsFailure) Console.WriteLine($"  Detalle: {res.Error.Message}");
}