namespace Divider.DTOs;

public class ExpenseDto
{
    public Guid Id { get; set; }

    public required string Description { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public Guid PaidByMemberId { get; set; }

    public string SplitType { get; set; } = string.Empty;

    public List<ExpenseSplitDto> Splits { get; set; } = new();
}