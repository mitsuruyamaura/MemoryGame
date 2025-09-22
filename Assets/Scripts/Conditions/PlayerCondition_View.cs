using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// プレイヤーの視界を広くしたり狭くしたりするコンディション
/// </summary>
[System.Serializable]
public class PlayerCondition_View : PlayerConditionBase
{
    protected override async UniTask ApplyEffect(CancellationToken token) {
        // カメラのサイズを広くする
        await SymbolManager.instance.ExpandViewSizeProc();
    }

    protected override async UniTask ApplyAfterEffect(CancellationToken token) {
        // カメラのサイズを元に戻す
        await SymbolManager.instance.DefaultViewSizeProc();
    }
}