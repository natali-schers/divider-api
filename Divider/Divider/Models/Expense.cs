namespace Divider.Models;

/// <summary>
/// Representa uma despesa compartilhada entre membros de um grupo.
/// </summary>
public class Expense
{
    /// <summary>
    /// Identificador único da despesa.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Descrição da despesa.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Valor total da despesa.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Data em que a despesa foi realizada.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Tipo de divisão da despesa entre os membros do grupo.
    /// </summary>
    public SplitType SplitType { get; set; }

    /// <summary>
    /// Identificador do grupo ao qual esta despesa pertence.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// Grupo ao qual esta despesa pertence.
    /// </summary>
    public Group? Group { get; set; }

    /// <summary>
    /// Identificador do membro que pagou a despesa.
    /// </summary>
    public Guid PaidByMemberId { get; set; }

    /// <summary>
    /// Membro que pagou a despesa.
    /// </summary>
    public Member? PaidByMember { get; set; }

    /// <summary>
    /// Lista de divisões da despesa entre os membros do grupo.
    /// </summary>
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}