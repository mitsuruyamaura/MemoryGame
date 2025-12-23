using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// バトル実行クラス
/// 将来的に必要になったらインターフェースを切って、実行クラスと分ける
/// 現在は EnemyCardExecutor が実行クラスである BattleExecutor を直接呼んでいるため、分けていない
/// この設計だと EnemyCardExecutor が存在と実装を知っていることになるが、将来的に変更する可能性が生じたときまで、現状はこのままにしておく
/// </summary>
public class BattleExecutor {
    private BattleManager battleManager;

    public BattleExecutor(BattleManager battleManager) {
        this.battleManager = battleManager;
    }

    public async UniTask ExecuteBattleAsync(EnemyData enemyData, CancellationToken token) { 
        await battleManager.StartBattleAsync(enemyData, TurnState.Player);
    }
}