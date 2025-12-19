using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class StairsCard : CardModelBase {
    public StairsCard(CardData cardData, int cardIndex) : base(cardData, cardIndex) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Stairs");

        await UniTask.Yield(token);
        
        SoundManager.instance.PlaySE(SE_TYPE.Magic_1);

        // 階段使用可能にする
        GameData.instance.userData.CanUseStairs.Value = true;
    }
}