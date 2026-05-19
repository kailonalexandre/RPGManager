namespace RpgManager.Application.Dice;

public sealed record DiceRollRequest(
    string Expression,
    bool Advantage,
    bool Disadvantage,
    string? Label);

public sealed record DiceRollResponse(
    string Expression,
    IReadOnlyList<int> Rolls,
    int Modifier,
    int Total,
    string? Label,
    DateTime RolledAt);
