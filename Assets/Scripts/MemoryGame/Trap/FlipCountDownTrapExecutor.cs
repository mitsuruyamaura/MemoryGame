using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// めくれる回数(行動回数)を減少させる罠実行クラス
/// </summary>
public class FlipCountDownTrapExecutor : ITrap {
    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        GameData.instance.userData.FlipPoint.Value = Mathf.Max(GameData.instance.userData.FlipPoint.Value - (int)trapData.value, 0);

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}