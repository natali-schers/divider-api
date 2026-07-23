namespace Divider.Models;

/// <summary>
/// Representa a parte de uma despesa que será paga por um dos membros do grupo.
/// </summary>
public class ExpenseSplit
{
    /// <summary>
    /// Identificador único do registro de divisão de despesa.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Valor que este membro deve pagar da despesa.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Identificador da despesa à qual esta divisão pertence.
    /// </summary>
    public Guid ExpenseId { get; set; }

    /// <summary>
    /// Despesa à qual esta divisão pertence.
    /// </summary>
    public Expense? Expense { get; set; }

    /// <summary>
    /// Identificador do membro que deve pagar esta parte da despesa.
    /// </summary>
    public Guid MemberId { get; set; }

    /// <summary>
    /// Membro que deve pagar esta parte da despesa.
    /// </summary>
    public Member? Member { get; set; }
}