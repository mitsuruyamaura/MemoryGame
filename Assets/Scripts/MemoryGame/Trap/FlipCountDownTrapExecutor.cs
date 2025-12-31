using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// めくれる回数(行動回数)を減少させる罠実行クラス
/// </summary>
public class FlipCountDownTrapExecutor : ITrapEffect {
    public async UniTask ExecuteTrapEffectAsync(TrapActionData trapActionData, CancellationToken token) {
        int subtractFlipPoint = (int)trapActionData.value;
        GameData.instance.CalcFlipPoint(-subtractFlipPoint);

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}