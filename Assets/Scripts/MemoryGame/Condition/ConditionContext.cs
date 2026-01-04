/// <summary>
/// コンディションの外部クラスの依存関係を保持するクラス
/// ConditionManager は循環処理になってしまうので、持たせない方がいい
/// </summary>
[System.Serializable]
public class ConditionContext {
    public StageUIManager StageUIManager { get; }

    // 必要になったら追加


    public ConditionContext(StageUIManager stageUIManager) {
        StageUIManager = stageUIManager; 
    }
}