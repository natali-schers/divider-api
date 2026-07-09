namespace Divider.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<MemberDto> Members { get; set; } = new();
}