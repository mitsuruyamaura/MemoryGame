using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 鍵カードの実行クラス
/// </summary>
public class StairsCardExecutor : ICardExecutor {

    public async UniTask ExecuteCardAsync(CardModelBase card, CancellationToken token) {
        //DebugLogger.Log("Stairs");

        if (card is not StairsCard stairsCard) {
            DebugLogger.Log($"StairsCard ではありません : {card.GetType().Name}");
            return;
        }

        await UniTask.Yield(token);

        SoundManager.instance.PlaySE(SE_TYPE.Magic_1);

        // 階段使用可能にする
        GameData.instance.userData.CanUseStairs.Value = true;
    }
}