namespace Divider.DTOs;

public class SettlementDto
{
    public Guid FromMemberId { get; set; }

    public Guid ToMemberId { get; set; }

    public decimal Amount { get; set; }
}