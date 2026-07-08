namespace Divider.Models;

public class ExpenseSplit
{
    public Guid Id { get; set; }

    public decimal Amount { get; set; }

    public Guid ExpenseId { get; set; }

    public Expense? Expense { get; set; }

    public Guid MemberId { get; set; }

    public Member? Member { get; set; }
}