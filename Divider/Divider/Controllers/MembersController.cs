using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Divider.Data;
using Divider.DTOs.Member;

namespace Divider.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class MembersController : ControllerBase
{
    private readonly DividerDbContext _context;

    public MembersController(DividerDbContext context)
    {
        _context = context;
    }

    // GET /api/members/pending-invites
    [HttpGet("members/pending-invites")]
    public async Task<ActionResult<List<PendingInviteDto>>> GetPendingInvites()
    {
        var currentUserId = this.GetCurrentUserId();
        var currentUser = await _context.Users.FindAsync(currentUserId);

        if (currentUser is null)
        {
            return Unauthorized();
        }

        var invites = await _context.Members
            .Where(m => m.UserId == null && m.InviteEmail == currentUser.Email)
            .Include(m => m.Group)
            .Select(m => new PendingInviteDto
            {
                MemberId = m.Id,
                GroupId = m.GroupId,
                GroupName = m.Group!.Name,
            })
            .ToListAsync();

        return Ok(invites);
    }

    // POST /api/members/{memberId}/claim
    [HttpPost("members/{memberId:guid}/claim")]
    public async Task<IActionResult> ClaimMember(Guid memberId)
    {
        var currentUserId = this.GetCurrentUserId();
        var currentUser = await _context.Users.FindAsync(currentUserId);

        if (currentUser is null)
        {
            return Unauthorized();
        }

        var member = await _context.Members.FindAsync(memberId);

        if (member is null)
        {
            return NotFound("Membro não encontrado.");
        }

        if (member.InviteEmail != currentUser.Email)
        {
            return Forbid();
        }

        member.UserId = currentUser.Id;
        member.InviteEmail = null;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}