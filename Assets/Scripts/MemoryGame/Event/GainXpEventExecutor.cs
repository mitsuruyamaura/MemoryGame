using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ソウルポイント獲得イベント実行クラス
/// </summary>
public class GainXpEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        int soulPoint = (int)blessingData.value;
        GameData.instance.CalcSoulPoint(soulPoint);

        // TODO SE

        await UniTask.Yield(token);
    }
}