using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// トラップの実行クラス
/// QTE でトラップ解除を試み、失敗した場合に各トラップを実行する
/// </summary>
public class TrapExecutor {
    private BattleManager battleManager;
    private TrapDisarmQTEManager trapDisarmQTEManager;
    private ConditionManager conditionManager;
    private MemoryGameManager memoryGameManager;
    private ItemInfoDisplayManager itemInfoDisplayManager;

    private readonly Dictionary<TrapActionType, ITrapEffect> executorMap;
    private readonly int symbolTypeCount = 4;    // QTEシンボル種類数
    private readonly float timeLimitSeconds = 5f; // QTE制限時間(秒)   ConstantData から取得するように変更

    public TrapExecutor(BattleManager battleManager, TrapDisarmQTEManager trapDisarmQTEManager, ConditionManager conditionManager, MemoryGameManager memoryGameManager, ItemInfoDisplayManager itemInfoDisplayManager) {
        this.battleManager = battleManager;
        this.trapDisarmQTEManager = trapDisarmQTEManager;
        this.conditionManager = conditionManager;
        this.memoryGameManager = memoryGameManager;
        this.itemInfoDisplayManager = itemInfoDisplayManager;

        executorMap = new() {
            { TrapActionType.Damage, new DamageTrapExecutor(battleManager) },
            { TrapActionType.FlipCountDown, new FlipCountDownTrapExecutor() },
            { TrapActionType.PlayerDebuff, new PlayerDebuffTrapExecutor(conditionManager, memoryGameManager)},
        };

        float timeLimitFromData = float.Parse(DataBaseManager.instance.GetConstantDataValue("LIMIT_TRAP_DISARM_TIME_SECOND"));
        timeLimitSeconds = timeLimitFromData;
    }

    /// <summary>
    /// トラップ実行
    /// </summary>
    /// <param name="trapData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ExecuteTrapAsync(TrapData trapData, CancellationToken token) {
        // QTEシンボルシーケンスの長さ(シンボル数：難しさ)を階層数から計算
        int sequenceLength = CalcSymbolLength(GameData.instance.userData.FloorCount.Value);

        // トラップ解除用の QTE 実行
        GameData.instance.CurrentGameState.Value = GameState.TrapDisarm;
        bool isSuccess = await trapDisarmQTEManager.StartTrapDisarmQTEAsync(symbolTypeCount, sequenceLength, timeLimitSeconds, token);

        if (isSuccess) {
            DebugLogger.Log("トラップ解除成功");
            GameData.instance.CalcSoulPoint(trapData.exp);

            GameData.instance.userData.TrapDisarmCount.Value++;
        } else {
            DebugLogger.Log("トラップ解除失敗。それぞれのトラップを実行");

            // 画面表示
            await itemInfoDisplayManager.ShowTrapInfoAsync(trapData, token);

            // トラップ内の複数の効果を順番に適用する
            foreach (TrapActionData trapActionData in trapData.trapActionDataList) {

                // マッピングされている TrapActionType から、トラップ用のインスタンスを見つけて生成
                if (executorMap.TryGetValue(trapActionData.trapActionType, out var trapExecutor)) {
                    DebugLogger.Log($"TrapActionType: {trapActionData.trapActionType}");

                    // 効果適用
                    await trapExecutor.ExecuteTrapEffectAsync(trapActionData, token);
                } else {
                    DebugLogger.Log($"未登録のTrapType: {trapActionData.trapActionType}");
                }
            }
            GameData.instance.userData.TrapFailureCount.Value++;
        }
        GameData.instance.CurrentGameState.Value = GameState.Play;
    }

    /// <summary>
    /// 現在の階層数から、QTEシンボルシーケンスの長さを計算する
    /// </summary>
    /// <param name="floorCount"></param>
    /// <returns></returns>
    private int CalcSymbolLength(int floorCount) {
        return floorCount switch {
            <= 5 => 2,
            <= 10 => 3,
            <= 20 => 4,
            <= 30 => 5,
            _ => 6,
        };
    }
}