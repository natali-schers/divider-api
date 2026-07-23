namespace Divider.Models;

/// <summary>
/// Representa um grupo de usuários que podem compartilhar despesas entre si.
/// </summary>
public class Group
{
    /// <summary>
    /// Identificador único do grupo.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do grupo.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Lista de membros que pertencem a este grupo.
    /// </summary>
    public ICollection<Member> Members { get; set; } = new List<Member>();

    /// <summary>
    /// Lista de despesas associadas a este grupo.
    /// </summary>
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}