using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

public class TrapExecutor {
    private readonly Dictionary<TrapType, ITrap> executorMap;

    public TrapExecutor() {
        executorMap = new() {
            { TrapType.Damage, new DamageTrapExecutor() },
            { TrapType.FlipCountDown, new FlipCountDownTrapExecutor() },
        };
    }

    /// <summary>
    /// トラップ実行
    /// </summary>
    /// <param name="trapData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ExecuteTrapAsync(TrapData trapData, CancellationToken token) {
        // マッピングされている TrapType から、トラップ用のインスタンスを見つけて生成
        if (executorMap.TryGetValue(trapData.type, out var executor)) {
            DebugLogger.Log($"TrapType: {trapData.type}");
            await executor.ExecuteAsync(trapData, token);
        } else {
            DebugLogger.Log($"未登録のTrapType: {trapData.type}");
        }
    }
}
