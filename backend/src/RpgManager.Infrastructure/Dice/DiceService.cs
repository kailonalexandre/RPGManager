using System.Security.Cryptography;
using System.Text.RegularExpressions;
using RpgManager.Application.Common;
using RpgManager.Application.Dice;

namespace RpgManager.Infrastructure.Dice;

public sealed partial class DiceService : IDiceService
{
    private static readonly HashSet<int> AllowedSides = [4, 6, 8, 10, 12, 20, 100];

    public ServiceResult<DiceRollResponse> Roll(DiceRollRequest request)
    {
        var expression = request.Expression?.Trim().ToLowerInvariant() ?? string.Empty;
        var match = DiceExpressionRegex().Match(expression);
        if (!match.Success)
        {
            return ServiceResult<DiceRollResponse>.Failure("Expressão inválida. Use formatos como 1d20+5 ou 2d6-1.");
        }

        var quantityText = match.Groups["quantity"].Value;
        var quantity = string.IsNullOrWhiteSpace(quantityText) ? 1 : int.Parse(quantityText);
        var sides = int.Parse(match.Groups["sides"].Value);
        var modifierText = match.Groups["modifier"].Value;
        var modifier = string.IsNullOrWhiteSpace(modifierText) ? 0 : int.Parse(modifierText);

        if (!AllowedSides.Contains(sides))
        {
            return ServiceResult<DiceRollResponse>.Failure("Dado não suportado.");
        }

        if (quantity is < 1 or > 100)
        {
            return ServiceResult<DiceRollResponse>.Failure("Quantidade de dados deve ficar entre 1 e 100.");
        }

        if (request.Advantage && request.Disadvantage)
        {
            return ServiceResult<DiceRollResponse>.Failure("Vantagem e desvantagem não podem ser usadas juntas.");
        }

        if ((request.Advantage || request.Disadvantage) && (quantity != 1 || sides != 20))
        {
            return ServiceResult<DiceRollResponse>.Failure("Vantagem/desvantagem só é permitida em 1d20.");
        }

        var rolls = new List<int>();
        int diceTotal;
        if (request.Advantage || request.Disadvantage)
        {
            rolls.Add(RollDie(sides));
            rolls.Add(RollDie(sides));
            diceTotal = request.Advantage ? rolls.Max() : rolls.Min();
        }
        else
        {
            for (var index = 0; index < quantity; index++)
            {
                rolls.Add(RollDie(sides));
            }

            diceTotal = rolls.Sum();
        }

        var canonicalExpression = $"{quantity}d{sides}{(modifier == 0 ? string.Empty : modifier > 0 ? $"+{modifier}" : modifier.ToString())}";
        return ServiceResult<DiceRollResponse>.Success(new DiceRollResponse(
            canonicalExpression,
            rolls,
            modifier,
            diceTotal + modifier,
            string.IsNullOrWhiteSpace(request.Label) ? null : request.Label.Trim(),
            DateTime.UtcNow));
    }

    private static int RollDie(int sides)
        => RandomNumberGenerator.GetInt32(1, sides + 1);

    [GeneratedRegex(@"^(?<quantity>\d*)d(?<sides>\d+)(?<modifier>[+-]\d+)?$", RegexOptions.Compiled)]
    private static partial Regex DiceExpressionRegex();
}
