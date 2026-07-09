namespace Divider.DTOs;

public class CreateExpenseDto
{
    public required string Description { get; set; }

    public decimal Amount { get; set; }

    public required Guid PaidByMemberId { get; set; }

    public required string SplitType { get; set; }

    public List<ExpenseSplitDto>? Splits { get; set; }
}