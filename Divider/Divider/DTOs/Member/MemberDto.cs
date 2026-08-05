using Divider.DTOs.Auth;

namespace Divider.DTOs.Member;

public class MemberDto
{
    public Guid Id { get; set; }

    public string? InviteEmail { get; set; }

    public UserDto? User { get; set; }

    public Guid? UserId { get; set; }
}