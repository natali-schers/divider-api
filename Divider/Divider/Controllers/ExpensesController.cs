using Divider.Data;
using Divider.DTOs.Expense;
using Divider.Models;
using Divider.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Divider.Controllers;

[ApiController]
[Route("api/groups/{groupId:guid}/expenses")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly DividerDbContext _context;
    private readonly BalanceCalculatorService _balanceCalculator;

    public ExpensesController(DividerDbContext context, BalanceCalculatorService balanceCalculator)
    {
        _context = context;
        _balanceCalculator = balanceCalculator;
    }

    // GET /api/groups/{groupId}/expenses
    [HttpGet]
    public async Task<ActionResult<List<ExpenseDto>>> GetExpenses(Guid groupId)
    {
        var (group, isMember) = await GetGroupAndCheckMembership(groupId);

        if (group is null)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
        }

        if (!isMember)
        {
            return Forbid();
        }

        var expenses = await _context.Expenses
            .Where(e => e.GroupId == groupId)
            .Include(e => e.Splits)
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Description = e.Description,
                Amount = e.Amount,
                Date = e.Date,
                PaidByMemberId = e.PaidByMemberId,
                SplitType = e.SplitType.ToString(),
                Splits = e.Splits.Select(s => new ExpenseSplitDto
                {
                    MemberId = s.MemberId,
                    Amount = s.Amount
                }).ToList()
            })
            .ToListAsync();

        return Ok(expenses);
    }

    // POST /api/groups/{groupId}/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(Guid groupId, CreateExpenseDto request)
    {
        var currentUserId = this.GetCurrentUserId();

        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
        }

        var isMember = group.Members.Any(m => m.UserId == currentUserId);

        if (!isMember)
        {
            return Forbid();
        }

        if (!Enum.TryParse<SplitType>(request.SplitType, ignoreCase: true, out var splitType)
            || !Enum.IsDefined(splitType))
        {
            return BadRequest($"SplitType inválido: {request.SplitType}");
        }

        var payerExists = group.Members.Any(m => m.Id == request.PaidByMemberId);

        if (!payerExists)
        {
            return BadRequest("O membro pagador não pertence a esse grupo.");
        }

        // Divisão igualitária: o resto dos centavos fica com o primeiro membro.
        var splitAmount = Math.Round(request.Amount / group.Members.Count, 2);
        var remainder = request.Amount - (splitAmount * group.Members.Count);

        var splits = group.Members.Select((m, index) => new ExpenseSplit
        {
            Id = Guid.NewGuid(),
            MemberId = m.Id,
            Amount = index == 0 ? splitAmount + remainder : splitAmount
        }).ToList();

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            Description = request.Description,
            Amount = request.Amount,
            Date = DateTime.UtcNow,
            PaidByMemberId = request.PaidByMemberId,
            SplitType = splitType,
            Splits = splits
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        var dto = new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            PaidByMemberId = expense.PaidByMemberId,
            SplitType = expense.SplitType.ToString(),
            Splits = expense.Splits.Select(s => new ExpenseSplitDto
            {
                MemberId = s.MemberId,
                Amount = s.Amount
            }).ToList()
        };

        return CreatedAtAction(nameof(GetExpenses), new { groupId }, dto);
    }

    // DELETE /api/groups/{groupId}/expenses/{expenseId}
    [HttpDelete("{expenseId:guid}")]
    public async Task<ActionResult> DeleteExpense(Guid groupId, Guid expenseId)
    {
        var currentUserId = this.GetCurrentUserId();

        var (group, isMember) = await GetGroupAndCheckMembership(groupId);

        if (group is null)
            return NotFound("Grupo não encontrado.");

        if (!isMember)
            return StatusCode(StatusCodes.Status403Forbidden, "Acesso negado.");

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.GroupId == groupId);

        if (expense == null)
            return NotFound("Despesa não encontrada.");

        var currentMemberId = group!.Members
            .FirstOrDefault(m => m.UserId == currentUserId)?.Id;

        if (currentMemberId is null || expense.PaidByMemberId != currentMemberId)
            return StatusCode(StatusCodes.Status403Forbidden, "Apenas o membro que pagou a despesa pode deletá-la.");

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return Ok("Despesa excluída com sucesso.");
    }

    // GET /api/groups/{groupId}/expenses/settlements
    [HttpGet("settlements")]
    public async Task<ActionResult<List<SettlementDto>>> GetSettlements(Guid groupId)
    {
        var (group, isMember) = await GetGroupAndCheckMembership(groupId);

        if (group is null)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
        }

        if (!isMember)
        {
            return Forbid();
        }

        var expenses = await _context.Expenses
            .Where(e => e.GroupId == groupId)
            .Include(e => e.Splits)
            .ToListAsync();

        var memberIds = group!.Members.Select(m => m.Id).ToList();
        var netBalances = _balanceCalculator.CalculateNetBalances(memberIds, expenses);
        var settlements = _balanceCalculator.SimplifyDebts(netBalances);

        var dtos = settlements.Select(s => new SettlementDto
        {
            FromMemberId = s.FromMemberId,
            ToMemberId = s.ToMemberId,
            Amount = s.Amount
        }).ToList();

        return Ok(dtos);
    }

    #region Métodos privados
    private async Task<(Group? Group, bool IsMember)> GetGroupAndCheckMembership(Guid groupId)
    {
        var currentUserId = this.GetCurrentUserId();

        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
        {
            return (null, false);
        }

        var isMember = group.Members.Any(m => m.UserId == currentUserId);
        return (group, isMember);
    }
    #endregion Métodos privados
}