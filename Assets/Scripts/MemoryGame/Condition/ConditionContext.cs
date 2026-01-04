/// <summary>
/// コンディションの外部クラスの依存関係を保持するクラス
/// </summary>
[System.Serializable]
public class ConditionContext {
    public StageUIManager StageUIManager { get; }
    public ConditionManager ConditionManager { get; }

    // 必要になったら追加


    public ConditionContext(StageUIManager stageUIManager, ConditionManager conditionManager) {
        StageUIManager = stageUIManager; 
        ConditionManager = conditionManager; 
    }
}