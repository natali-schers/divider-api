using Divider.Models;

namespace Divider.Services;

public class BalanceCalculatorService
{
    /// <summary>
    /// Calcula o saldo líquido de cada membro.
    /// Positivo = tem a receber. Negativo = deve.
    /// </summary>
    public Dictionary<Guid, decimal> CalculateNetBalances(
        List<Guid> memberIds,
        List<Expense> expenses)
    {
        var balances = memberIds.ToDictionary(id => id, _ => 0m);

        foreach (var expense in expenses)
        {
            if (balances.ContainsKey(expense.PaidByMemberId))
            {
                balances[expense.PaidByMemberId] += expense.Amount;
            }

            foreach (var split in expense.Splits)
            {
                if (balances.ContainsKey(split.MemberId))
                {
                    balances[split.MemberId] -= split.Amount;
                }
            }
        }

        return balances;
    }

    /// <summary>
    /// Simplifica os saldos líquidos em uma lista mínima de transferências.
    /// </summary>
    public List<(Guid FromMemberId, Guid ToMemberId, decimal Amount)> SimplifyDebts(
        Dictionary<Guid, decimal> netBalances)
    {
        const decimal tolerance = 0.01m;

        var creditors = netBalances
            .Where(kv => kv.Value > tolerance)
            .Select(kv => (MemberId: kv.Key, Amount: kv.Value))
            .OrderByDescending(c => c.Amount)
            .ToList();

        var debtors = netBalances
            .Where(kv => kv.Value < -tolerance)
            .Select(kv => (MemberId: kv.Key, Amount: -kv.Value))
            .OrderByDescending(d => d.Amount)
            .ToList();

        var settlements = new List<(Guid, Guid, decimal)>();

        var i = 0;
        var j = 0;

        while (i < debtors.Count && j < creditors.Count)
        {
            var debtor = debtors[i];
            var creditor = creditors[j];

            var settledAmount = Math.Min(debtor.Amount, creditor.Amount);

            settlements.Add((debtor.MemberId, creditor.MemberId, settledAmount));

            debtors[i] = (debtor.MemberId, debtor.Amount - settledAmount);
            creditors[j] = (creditor.MemberId, creditor.Amount - settledAmount);

            if (debtors[i].Amount < tolerance) i++;
            if (creditors[j].Amount < tolerance) j++;
        }

        return settlements;
    }
}