namespace Divider.DTOs;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required List<string> MemberNames { get; set; }
}