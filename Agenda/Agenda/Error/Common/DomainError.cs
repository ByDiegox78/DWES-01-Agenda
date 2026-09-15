namespace Agenda.Error.Common;
/// <summary>
/// Es la base de todos los errores del dominio de la agenda
/// Cualquier error nuevo debe pasar por el contructor 
/// </summary>
public abstract record DomainError(string Message);