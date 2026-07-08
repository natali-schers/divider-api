namespace Divider.Models;

public class Group
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Member> Members { get; set; } = new List<Member>();

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}