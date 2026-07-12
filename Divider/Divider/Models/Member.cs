namespace Divider.Models;

public class Member
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid GroupId { get; set; }

    public Group? Group { get; set; }

    // Vínculo opcional com uma conta de usuário real.
    // null = "convidado" (só nome, sem conta associada ainda).
    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public ICollection<Expense> ExpensesPaid { get; set; } = new List<Expense>();

    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
}