using Cysharp.Threading.Tasks;
using System.Threading;


public class TreasureCard : CardBase {

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Treasure");

        await base.ExecuteCardAsync(token);
    }
}