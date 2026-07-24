using Divider.DTOs.Member;

namespace Divider.DTOs.Group;

public class CreateGroupDto
{
    public required string Name { get; set; }
    public required List<CreateMemberDto> Members { get; set; }
}