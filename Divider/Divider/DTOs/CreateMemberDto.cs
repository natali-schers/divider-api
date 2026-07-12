namespace Divider.DTOs;

public class CreateMemberDto
{
    public required string Name { get; set; }

    public string? InviteEmail { get; set; }
}