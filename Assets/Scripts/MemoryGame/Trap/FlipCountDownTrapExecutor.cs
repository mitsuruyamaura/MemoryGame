using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// めくれる回数(行動回数)を減少させる罠実行クラス
/// </summary>
public class FlipCountDownTrapExecutor : ITrap {
    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        int subtractFlipPoint = (int)trapData.value;
        GameData.instance.CalcFlipPoint(-subtractFlipPoint);

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}