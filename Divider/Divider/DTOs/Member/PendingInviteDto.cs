namespace Divider.DTOs.Member;

public class PendingInviteDto
{
    public Guid MemberId { get; set; }

    public required string MemberName { get; set; }

    public Guid GroupId { get; set; }

    public required string GroupName { get; set; }
}