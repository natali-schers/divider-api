using Divider.DTOs.Member;

namespace Divider.DTOs.Group;

public class GroupDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MemberDto> Members { get; set; } = new();
}