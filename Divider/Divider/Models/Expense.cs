using System.Text.RegularExpressions;

namespace Divider.Models;

public class Expense
{
    public Guid Id { get; set; }

    public required string Description { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public SplitType SplitType { get; set; }

    public Guid GroupId { get; set; }

    public Group? Group { get; set; }

    public Guid PaidByMemberId { get; set; }

    public Member? PaidByMember { get; set; }

    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}