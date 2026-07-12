using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Divider.Data;
using Divider.DTOs;
using Divider.Models;

namespace Divider.Controllers;

[ApiController]
[Route("api/groups")]
public class GroupsController : ControllerBase
{
    private readonly DividerDbContext _context;

    public GroupsController(DividerDbContext context)
    {
        _context = context;
    }

    // GET /api/groups
    [HttpGet]
    public async Task<ActionResult<List<GroupDto>>> GetGroups()
    {
        var groups = await _context.Groups
            .Include(g => g.Members)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Members = g.Members.Select(m => new MemberDto
                {
                    Id = m.Id,
                    Name = m.Name
                }).ToList()
            })
            .ToListAsync();

        return Ok(groups);
    }

    // GET /api/groups/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GroupDto>> GetGroupById(Guid id)
    {
        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
        {
            return NotFound();
        }

        var dto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Members = group.Members.Select(m => new MemberDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList()
        };

        return Ok(dto);
    }

    // POST /api/groups
    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.MemberNames.Count < 2)
        {
            return BadRequest("Informe um nome de grupo e ao menos 2 membros.");
        }

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Members = request.MemberNames.Select(name => new Member
            {
                Id = Guid.NewGuid(),
                Name = name
            }).ToList()
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        var dto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Members = group.Members.Select(m => new MemberDto
            {
                Id = m.Id,
                Name = m.Name
            }).ToList()
        };

        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, dto);
    }
}