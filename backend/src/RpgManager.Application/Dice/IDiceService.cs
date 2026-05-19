using RpgManager.Application.Common;

namespace RpgManager.Application.Dice;

public interface IDiceService
{
    ServiceResult<DiceRollResponse> Roll(DiceRollRequest request);
}
