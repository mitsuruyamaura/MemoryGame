using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class StairsCard : CardModelBase {
    public StairsCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Stairs");

        await UniTask.Yield(token);

        // 階段使用可能にする
        GameData.instance.userData.CanUseStairs.Value = true;
    }
}