using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class FlipCountDownTrapExecutor : ITrap {
    public async UniTask ExecuteAsync(TrapData trapData, CancellationToken token) {
        GameData.instance.userData.FlipPoint.Value = Mathf.Max(GameData.instance.userData.FlipPoint.Value - (int)trapData.value, 0);

        //SoundManager.instance.PlaySE(SE_TYPE.Heal);

        await UniTask.Yield(token);
    }
}