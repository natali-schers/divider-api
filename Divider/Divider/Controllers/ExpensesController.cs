using Divider.Data;
using Divider.DTOs;
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
        var groupExists = await _context.Groups.AnyAsync(g => g.Id == groupId);
        if (!groupExists)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
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
        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
        }

        if (!Enum.TryParse<SplitType>(request.SplitType, ignoreCase: true, out var splitType))
        {
            return BadRequest($"SplitType inválido: {request.SplitType}");
        }

        var payerExists = group.Members.Any(m => m.Id == request.PaidByMemberId);

        if (!payerExists)
        {
            return BadRequest("O membro pagador não pertence a esse grupo.");
        }

        List<ExpenseSplit> splits;

        if (splitType == SplitType.Equal)
        {
            var splitAmount = Math.Round(request.Amount / group.Members.Count, 2);
            var remainder = request.Amount - (splitAmount * group.Members.Count);

            splits = group.Members.Select((m, index) => new ExpenseSplit
            {
                Id = Guid.NewGuid(),
                MemberId = m.Id,
                Amount = index == 0 ? splitAmount + remainder : splitAmount
            }).ToList();
        }
        else
        {
            if (request.Splits is null || request.Splits.Count == 0)
            {
                return BadRequest("Splits são obrigatórios para esse tipo de divisão.");
            }

            var invalidSplitMember = request.Splits
                .FirstOrDefault(s => !group.Members.Any(m => m.Id == s.MemberId));

            if (invalidSplitMember is not null)
            {
                return BadRequest($"O membro {invalidSplitMember.MemberId} não pertence a esse grupo.");
            }

            var splitSum = request.Splits.Sum(s => s.Amount);

            if (Math.Abs(splitSum - request.Amount) > 0.01m)
            {
                return BadRequest("A soma das divisões não bate com o valor total da despesa.");
            }

            splits = request.Splits.Select(s => new ExpenseSplit
            {
                Id = Guid.NewGuid(),
                MemberId = s.MemberId,
                Amount = s.Amount
            }).ToList();
        }

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

    // GET /api/groups/{groupId}/expenses/settlements
    [HttpGet("settlements")]
    public async Task<ActionResult<List<SettlementDto>>> GetSettlements(Guid groupId)
    {
        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group is null)
        {
            return NotFound($"Grupo {groupId} não encontrado.");
        }

        var expenses = await _context.Expenses
            .Where(e => e.GroupId == groupId)
            .Include(e => e.Splits)
            .ToListAsync();

        var memberIds = group.Members.Select(m => m.Id).ToList();
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
}