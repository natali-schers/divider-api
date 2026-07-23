namespace Divider.Models;

/// <summary>
/// Representa o usuário propriamente dito, com suas informações de autenticação e perfil.
/// </summary>
public class User
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Endereço de email do usuário.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Hash da senha do usuário.
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Data e hora em que o usuário foi criado.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}