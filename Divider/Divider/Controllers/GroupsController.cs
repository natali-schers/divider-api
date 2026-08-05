using Divider.Data;
using Divider.DTOs.Auth;
using Divider.DTOs.Group;
using Divider.DTOs.Member;
using Divider.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Divider.Controllers;

[ApiController]
[Route("api/groups")]
[Authorize]
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
        var currentUserId = this.GetCurrentUserId();

        var groups = await _context.Groups
            .Where(g => g.Members.Any(m => m.UserId == currentUserId))
            .Include(g => g.Members)
            .ThenInclude(m => m.User)
            .Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                Members = g.Members.Select(m => new MemberDto
                {
                    Id = m.Id,
                    InviteEmail = m.InviteEmail,
                    User = m.User != null ? new UserDto
                    {
                        Id = m.User.Id,
                        Name = m.User.Name,
                        Email = m.User.Email,
                    } : null
                }).ToList()
            })
            .ToListAsync();

        return Ok(groups);
    }

    // GET /api/groups/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GroupDto>> GetGroupById(Guid id)
    {
        var currentUserId = this.GetCurrentUserId();

        var group = await _context.Groups
            .Include(g => g.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
        {
            return NotFound();
        }

        var isMember = group.Members.Any(m => m.UserId == currentUserId);

        if (!isMember)
        {
            return Forbid();
        }

        var dto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Members = group.Members.Select(m => new MemberDto
            {
                Id = m.Id,
                InviteEmail = m.InviteEmail,
                User = m.User is not null ? new UserDto
                {
                    Id = m.User.Id,
                    Name = m.User.Name,
                    Email = m.User.Email,
                } : null
            }).ToList()
        };

        return Ok(dto);
    }

    // POST /api/groups
    [HttpPost]
    public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto request)
    {
        var currentUserId = this.GetCurrentUserId();
        var currentUser = await _context.Users.FindAsync(currentUserId);

        if (currentUser is null)
        {
            return Unauthorized("Faça login para criar um grupo.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Informe um nome de grupo.");
        }

        var members = new List<Member>
        {
            // O criador do grupo já entra como membro vinculado à própria conta.
            new()
            {
                Id = Guid.NewGuid(),
                UserId = currentUser.Id,
            },
        };

        var requestMemberEmails = request.Members
            .Where(m => !string.IsNullOrWhiteSpace(m.InviteEmail))
            .Select(m => m.InviteEmail!.Trim().ToLowerInvariant())
            .ToList();

        members.AddRange(request.Members.Select(m =>
        {
            var normalizedInviteEmail = string.IsNullOrWhiteSpace(m.InviteEmail)
                ? null
                : m.InviteEmail.Trim().ToLowerInvariant();

            var matchedUserId = (Guid?)null;

            return new Member
            {
                Id = Guid.NewGuid(),
                InviteEmail = normalizedInviteEmail,
                UserId = matchedUserId,
            };
        }));

        if (members.Count < 2)
        {
            return BadRequest("Informe ao menos 1 membro convidado, além de você.");
        }

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Members = members,
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
                InviteEmail = m.InviteEmail,
                UserId = m.UserId
            }).ToList()
        };

        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, dto);
    }

    // DELETE /api/groups/{id}
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteGroup(Guid id)
    {
        var currentUserId = this.GetCurrentUserId();

        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group is null)
            return NotFound("Grupo não encontrado.");

        var isMember = group.Members.Any(m => m.UserId == currentUserId);

        if (!isMember)
            return Forbid("Acesso negado.");

        group.Expenses = await _context.Expenses
            .Where(e => e.GroupId == group.Id)
            .ToListAsync();

        if (group.Expenses.Any())
            return BadRequest("Não é possível excluir um grupo que possui despesas pendentes.");

        _context.Groups.Remove(group);

        await _context.SaveChangesAsync();

        return Ok("Grupo excluído com sucesso.");
    }
}