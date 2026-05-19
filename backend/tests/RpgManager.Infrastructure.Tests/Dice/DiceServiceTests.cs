using RpgManager.Application.Dice;
using RpgManager.Infrastructure.Dice;
using Xunit;

namespace RpgManager.Infrastructure.Tests.Dice;

public sealed class DiceServiceTests
{
    [Fact]
    public void Roll_parses_quantity_sides_and_modifier()
    {
        var service = new DiceService();

        var result = service.Roll(new DiceRollRequest("2d6+3", false, false, "Dano"));

        Assert.True(result.Succeeded);
        Assert.Equal("2d6+3", result.Data!.Expression);
        Assert.Equal(2, result.Data.Rolls.Count);
        Assert.Equal(3, result.Data.Modifier);
        Assert.InRange(result.Data.Total, 5, 15);
        Assert.Equal("Dano", result.Data.Label);
    }

    [Fact]
    public void Roll_supports_advantage_only_for_one_d20()
    {
        var service = new DiceService();

        var result = service.Roll(new DiceRollRequest("1d20+5", true, false, null));

        Assert.True(result.Succeeded);
        Assert.Equal(2, result.Data!.Rolls.Count);
        Assert.Equal(result.Data.Rolls.Max() + 5, result.Data.Total);
    }

    [Theory]
    [InlineData("1d3")]
    [InlineData("2d6+")]
    [InlineData("0d20")]
    public void Roll_rejects_invalid_expressions(string expression)
    {
        var service = new DiceService();

        var result = service.Roll(new DiceRollRequest(expression, false, false, null));

        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Roll_rejects_advantage_on_non_d20()
    {
        var service = new DiceService();

        var result = service.Roll(new DiceRollRequest("2d6", true, false, null));

        Assert.False(result.Succeeded);
    }
}
