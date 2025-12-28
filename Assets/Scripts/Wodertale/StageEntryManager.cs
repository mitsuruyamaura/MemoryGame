using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

/// <summary>
/// Stage シーンにおけるエントリーマネージャー
/// シーン開始時の初期化処理
/// </summary>
public class StageEntryManager : MonoBehaviour {
    [SerializeField] private MemoryGameManager memoryGameManager; 
    [SerializeField] private StageUIManager stageUIManager;
    [SerializeField] private MemoryLinkManager memoryLinkManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private TrapDisarmQTEManager trapDisarmQTEManager;

    [SerializeField] private ItemInfoDisplayManager itemInfoDisplayManager;
    [SerializeField] private EnemyInfoDisplayManager enemyInfoDisplayManager;
    [SerializeField] private FloatingViewGenerator floatingViewGenerator;
    [SerializeField] private PlayerInventoryManager playerInventoryManager;
    [SerializeField] private ConditionManager conditionManager;

    [SerializeField] private ConditionInfoDisplayManager conditionInfoDisplayManager;

    private CancellationTokenSource cts;
    private CardFactory cardFactory;


    private void Start() {
        // 自動的に FadOut する
        SceneStateManager.instance.FadeIn().Forget();
        InitializeStageAsync().Forget();
    }

    /// <summary>
    /// Stage シーンの初期化
    /// </summary>
    /// <returns></returns>
    private async UniTask InitializeStageAsync() {
        int randomStageBgmIndex = Random.Range(3, 7);
        SoundManager.instance.PlayBGM((BGM_TYPE)randomStageBgmIndex);

        cts = new();

        // カードファクトリーの初期化。各 Executor の生成もここで行われる
        cardFactory = new(memoryGameManager, battleManager, trapDisarmQTEManager, memoryLinkManager, playerInventoryManager, itemInfoDisplayManager);

        trapDisarmQTEManager.SetUp(stageUIManager);

        enemyInfoDisplayManager.Setup(null, battleManager, playerInventoryManager, floatingViewGenerator, itemInfoDisplayManager);
        floatingViewGenerator.SetUp(gameObject);
        itemInfoDisplayManager.Setup();

        memoryLinkManager.Setup();

        GameData.instance.Setup(playerInventoryManager);

        battleManager.SetUp(stageUIManager, cardFactory, enemyInfoDisplayManager, floatingViewGenerator, playerInventoryManager);

        // UI 初期設定
        stageUIManager.Setup(GameData.instance.charaStatus.MaxHp.Value, memoryGameManager, battleManager);

        // BackPackInItem のオブジェクトプールの初期化、List の購読などを設定
        playerInventoryManager.Setup(memoryGameManager, stageUIManager, battleManager, floatingViewGenerator, itemInfoDisplayManager);
        
        conditionManager.Setup(conditionInfoDisplayManager);

        GameData.instance.CurrentGameState.Value = GameState.Play;

        await memoryGameManager.SetUpAsync(cardFactory, battleManager, playerInventoryManager, conditionManager);
    }
}