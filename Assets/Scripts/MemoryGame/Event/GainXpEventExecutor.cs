using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ソウルポイント獲得イベント実行クラス
/// </summary>
public class GainXpEventExecutor : IEventExecutor {
    public async UniTask ExecuteAsync(BlessingData blessingData, CancellationToken token) {
        GameData.instance.userData.SoulPoint.Value += (int)blessingData.value;

        // TODO SE

        await UniTask.Yield(token);
    }
}