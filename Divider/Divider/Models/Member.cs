using System.Text.RegularExpressions;

namespace Divider.Models;

public class Member
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public Guid GroupId { get; set; }

    public Group? Group { get; set; }

    public ICollection<Expense> ExpensesPaid { get; set; } = new List<Expense>();

    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
}