namespace Divider.Models;

/// <summary>
/// Representa um membro de um grupo, que pode ser um usuário registrado ou um convidado (sem conta associada).
/// </summary>
public class Member
{
    /// <summary>
    /// Identificador único do membro dentro do grupo.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do membro.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Identificador do grupo ao qual este membro pertence.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Grupo ao qual este membro pertence.
    /// </summary>
    public Group? Group { get; set; }

    /// <summary>
    /// Vínculo opcional com uma conta de usuário real.
    /// null = "convidado" (só nome, sem conta associada ainda).
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Usuário associado a este membro, se houver. Se for null, significa que o membro é um convidado sem conta associada.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Email usado para vincular convidado a uma conta futura.
    /// </summary>
    public string? InviteEmail { get; set; }

    /// <summary>
    /// Lista de despesas que este membro pagou.
    /// </summary>
    public ICollection<Expense> ExpensesPaid { get; set; } = new List<Expense>();

    /// <summary>
    /// Lista de despesas que este membro deve pagar (ou seja, a parte da despesa que foi atribuída a ele).
    /// </summary>
    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
}