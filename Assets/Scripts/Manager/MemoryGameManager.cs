using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ObservableCollections;
using R3;
using R3.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameManager : MonoBehaviour {
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;           // カード配置用のスロット
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] private int pairCount = 3;

    [SerializeField] private List<CardView> cardViewList = new ();
    [SerializeField] private List<CardModelBase> cardModelList = new ();
    [SerializeField] private CardView firstSelectedCardView;
    [SerializeField] private CardModelBase firstSelectedCardModel;
    [SerializeField] private bool inputLocked = false;
    [SerializeField] private float flipDuration = 0.3f;

    [SerializeField] private Button btnStairs;
    [SerializeField] private Button btnRetry;
    [SerializeField] private CanvasGroup cgSlotSet;
    [SerializeField] private Text txtFlipPoint;
    [SerializeField] private Text txtFloorCount;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;
    [SerializeField] private Image[] imgMemoryStoneIcons;
    [SerializeField] private ShinyEffectForUGUI[] shinyEffects;
    [SerializeField] private Sprite spriteMemoryStone;
    [SerializeField] private Text txtMemoriaRank;
    [SerializeField] private Transform blessingIconViewTran;
    [SerializeField] private BlessingIconGenerator blessingIconGenerator;
    [SerializeField] private List<BlessingIconView> blessingIconViewList = new();
    [SerializeField] private Text txtMatchedPairCount;

    [SerializeField] private int debugFlipPoint;
    [SerializeField] private int debugFloorClearBonusFlipPoint;
    [SerializeField] private int debugComboBonusRate;
    [SerializeField] private int debugBlessingID;              // デバッグ用 BlessingData ID 番号
    [SerializeField] private int debugTrapID;

    [SerializeField] private Image imgBackGround;
    [SerializeField] private Sprite[] spriteBackGrounds;
    [SerializeField] private Sprite spriteLastFloorBackGround;
    [SerializeField] private Sprite spriteGameClear;

    private int backGroundIndex = -1;
    private int changeFloorCount = 5;

    private readonly Subject<CardView> onCardSelected = new(); // カード選択イベント
    public Observable<CardView> OnCardSelect => onCardSelected;
    private List<GameObject> slotList = new();                 // スロットを保持
    private CancellationTokenSource cts;
    private FloorData currentFloorData;

    public SerializableReactiveProperty<int> EnemyLookCount = new(0);
    public SerializableReactiveProperty<int> TrapLookCount = new(0);
    public SerializableReactiveProperty<int> MemoriaLookCount = new(0);
    public SerializableReactiveProperty<int> BlessingLookCount = new(0);
    public SerializableReactiveProperty<int> KeyLookCount = new(0);
    public SerializableReactiveProperty<int> TreasureChestLookCount = new(0);

    private CompositeDisposable enemyLookDisposables;         // 灰色だがすべて使っている
    private CompositeDisposable trapLookDisposables;
    private CompositeDisposable memoriaLookDisposables;
    private CompositeDisposable blessingLookDisposables;
    private CompositeDisposable keyLookDisposables;
    private CompositeDisposable treasureChestLookDisposables;

    private Dictionary<BlessingValueType, (SerializableReactiveProperty<int>, CompositeDisposable)> lookCountMap;

    private Subject<int> onNextFloor = new();
    private Subject<Unit> onTurnEnd = new();
    private Subject<Unit> onFloorCardsCreated = new();
    private IDisposable floorCountDisposable;    // 灰色だが使っている
    private IDisposable cardCreateDisposable;
    private IDisposable turnEndDisposable;

    private CardFactory cardFactory;
    private PlayerInventoryManager playerInventoryManager;
    private ConditionManager conditionManager;

    // デバッグ用
    //private async void Start() {
    //    SetupAsync().Forget();
    //}


    /// <summary>
    /// ステージ初期設定
    /// </summary>
    /// <returns></returns>
    public async UniTask SetUpAsync(CardFactory cardFactory, BattleManager battleManager, PlayerInventoryManager playerInventoryManager, ConditionManager conditionManager) {
        this.cardFactory = cardFactory;
        this.playerInventoryManager = playerInventoryManager;
        this.conditionManager = conditionManager;

        // 各初期化処理
        cts = new();
        cardGenerator.InitObjectPool();
        blessingIconGenerator.InitObjectPool();

        // 背景用の画像のランダム設定
        RandomChangeBackGroundImage();

        // フロア表示の更新
        GameData.instance.userData.FloorCount.Subscribe(count => UpdateDisplayFloorCount(count)).AddTo(this);
        GameData.instance.userData.FloorCount.OnNext(GameData.instance.userData.FloorCount.Value);

        // 仮
        GameData.instance.userData.FlipPoint.Value = debugFlipPoint != 0 ? debugFlipPoint : 0;

        // めくれる回数の表示更新
        GameData.instance.userData.FlipPoint
            .Zip(GameData.instance.userData.FlipPoint.Skip(1), (prevPoint, nextPoint) => (prevPoint, nextPoint))
            .Subscribe(soulPoint => UpdateDisplayFlipPoint(soulPoint.prevPoint, soulPoint.nextPoint))
            .AddTo(this);

        GameData.instance.userData.FlipPoint.OnNext(GameData.instance.userData.FlipPoint.Value);

        GameData.instance.userData.MemoriaRank.Subscribe(rank => UpdateDisplayMemoriaRank(rank)).AddTo(this);

        // 階段の状態の購読
        GameData.instance.userData.CanUseStairs.Subscribe(canUse => btnStairs.interactable = canUse).AddTo(this);

        ResetFloorState();

        await InitFloorAsync();

        // カード選択イベントを購読。選択したカードをめくる
        onCardSelected
            .Where(_ => GameData.instance.CurrentGameState.Value == GameState.Play)
            .Where(_ => !inputLocked) // ロック中は無視
            .Subscribe(cardView => HandleCardSelectionAsync(cardView).Forget())
            .AddTo(this);

        // 階段ボタンの購読
        if (this == null || btnStairs == null) return;
        btnStairs
            .OnClickExt(() => NextFloorAsync().Forget(), this);

        // リトライボタンの購読
        if (this == null || btnRetry == null) return;
        btnRetry
            .OnClickExt(() => {
                if (GameData.instance.CurrentGameState.Value != GameState.Play) {
                    return;
                }
                GameEndAsync().Forget(); }, this);

        // めくれる回数の残り回数を確認し、ゲーム終了処理へつなげる
        GameData.instance.userData.FlipPoint
            .Where(flipPoint => flipPoint <= 0)
            .Take(1)
            .Subscribe(flipPoint => battleManager.ForceGameEndAsync().Forget()).AddTo(this);  // GameEndAsync().Forget()

        // 思い出の断片獲得時の購読処理
        GameData.instance.userData.MemoryStoneSlotList.ObserveAdd()
            .Subscribe(memoryStoneIndex => SetMemoryStoneIcon(memoryStoneIndex.Value)).AddTo(this);

        GameData.instance.ComboPairCount.Subscribe(comboCount => CheckComboEffect(comboCount)).AddTo(this);

        GameData.instance.MatchedPairCount.Subscribe(machedPairCount => UpdateDisplayNextBonusFlipPoint(machedPairCount));

        // マッピング
        lookCountMap = new() {
            { BlessingValueType.Enemy, (EnemyLookCount, enemyLookDisposables = new()) },
            { BlessingValueType.Trap, (TrapLookCount, trapLookDisposables = new()) },
            { BlessingValueType.Memory, (MemoriaLookCount, memoriaLookDisposables = new()) },
            { BlessingValueType.Event, (BlessingLookCount, blessingLookDisposables = new()) },
            { BlessingValueType.Key, (KeyLookCount, keyLookDisposables = new()) },
            { BlessingValueType.TreasureChest, (TreasureChestLookCount, treasureChestLookDisposables = new()) }
        };

        // フロアが進んだときの購読処理
        floorCountDisposable = onNextFloor.Subscribe(count => {
            // 効果時間が残っているなら、時間を減算して、カードに効果適用
            if (EnemyLookCount.Value > 0) EnemyLookCount.Value--;
            if (TrapLookCount.Value > 0) TrapLookCount.Value--;
            if (MemoriaLookCount.Value > 0) MemoriaLookCount.Value--;
            if (BlessingLookCount.Value > 0) BlessingLookCount.Value--;
            if (KeyLookCount.Value > 0) KeyLookCount.Value--;
            if (TreasureChestLookCount.Value > 0) TreasureChestLookCount.Value--;

            // 最終フロア到達時の背景変更
            if (count == GameData.instance.lastFloorCount) {
                ChangeBackGroundImageLastFloor();
            } else if (count % changeFloorCount == 0) {
                // 一定フロアごとに背景変更
                RandomChangeBackGroundImage();

                SetRandomBGM();
            }
        }).AddTo(this);

        // フロアが進んでカードがすべて生成されたときの購読処理
        cardCreateDisposable = onFloorCardsCreated
            .Subscribe(_ => RefreshAllFaceUpCards()).AddTo(this);

        // ターン終了時にコンディションの残り強度を更新
        turnEndDisposable = onTurnEnd.Subscribe(_ => conditionManager.UpdateConditionRemainingPowers()).AddTo(this);

        // デバッグ用リセット機能
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ => NextFloorAsync().Forget())
            .AddTo(this);
    }

    /// <summary>
    /// フロアの状態をリセット
    /// </summary>
    private void ResetFloorState() {
        GameData.instance.userData.CanUseStairs.Value = false;
        GameData.instance.MatchedPairCount.Value = 0;

        firstSelectedCardView = null;
        firstSelectedCardModel = null;

        cardModelList.Clear();

        cardViewList.ForEach(cardView => cardView?.Release());
        cardViewList.Clear();

        slotList.ForEach(slot => Destroy(slot));
        slotList.Clear();

        cgSlotSet.blocksRaycasts = false;
    }

    /// <summary>
    /// フロアの初期化処理
    /// </summary>
    /// <returns></returns>
    private async UniTask InitFloorAsync() {
        // 最終フロアなら音楽変更
        if (GameData.instance.lastFloorCount == GameData.instance.userData.FloorCount.Value) {
            SoundManager.instance.PlayBGM(BGM_TYPE.Boss);
        }

        // 現在のフロア数より、フロアのデータを取得
        currentFloorData = DataBaseManager.instance.GetFloorDataByFloor(GameData.instance.userData.FloorCount.Value);

        // イベント系カードの残り時間減算。0 になっていないなら効果適用。背景用の画像の変更
        onNextFloor.OnNext(GameData.instance.userData.FloorCount.Value);

        // スロットを生成
        CreateSlots();

        // カードの情報準備と生成
        await SetupCardsAsync();

        if (this == null || cgSlotSet == null) return;

        // 最後のカードが裏返るまで待つ
        await UniTask.Delay(500, cancellationToken: cts.Token);

        // イベント系カードの効果適用
        onFloorCardsCreated.OnNext(default);

        // カードを触れる状態にする
        cgSlotSet.blocksRaycasts = true;
    }

    /// <summary>
    /// カードを配置するためのスロットを生成(カードが消えても詰まらないようにするため)
    /// </summary>
    private void CreateSlots() {
        // カードの枚数に応じて横方向の長さをフロアごとに変えるため、GridLayoutGroup の Constraint Count を設定
        gridLayoutGroup.constraintCount = currentFloorData.row;

        // カード配置用のスロット生成
        int totalSlots = currentFloorData.pairCount;
        for (int i = 0; i < totalSlots; i++) {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slotList.Add(slot);
        }
    }

    /// <summary>
    /// カードのペア情報の作成と生成
    /// </summary>
    /// <returns></returns>
    private async UniTask SetupCardsAsync() {
        // 各カードをペアで作成し、シャッフルされた状態でもらう
        List<CardData> selectedCardDataList = CreateCardPairsAndShuffle(pairCount);

        // カードデータを元に CardModel を作成して List に保持
        CreateCardModelList(selectedCardDataList);

        // カード生成して各スロットに１つずつ配置
        for (int i = 0; i < selectedCardDataList.Count; i++) {
            int index = i;
            await UniTask.Delay(200);

            if (this == null || cardGenerator == null || slotList == null) return;

            CardView card = (CardView)cardGenerator.GetObjectFromPool(slotList[i].transform);
            card.Setup(index, OnCardSelected, flipDuration, selectedCardDataList[i].cardTypeMaster.spriteCardType, selectedCardDataList[i].cardTypeMaster.cardEventType);
            cardViewList.Add(card);
        }
    }

    /// <summary>
    /// 指定枚数のカードをペアで作成してシャッフル
    /// </summary>
    /// <returns></returns>
    private List<CardData> CreateCardPairsAndShuffle(int pairCount) {
        // 各カードをペアで作成
        List<CardData> selectedCardDataList = new();

        // CardType の List を フロアの階数より作成
        List<CardTypeMaster> cardTypeMasterList = CreateCardTypeList();

        // カードの種類ごとにペアを作成
        for (int i = 0; i < cardTypeMasterList.Count; i++) {
            CardData cardData = new() {
                cardIndex = i,
                cardTypeMaster = cardTypeMasterList[i],
                masterData = null
            };
            selectedCardDataList.Add(cardData);
            selectedCardDataList.Add(cardData);
        }

        // デバッグ用
        //for (int i = 0; i < pairCount; i++) {
        //    CardTypeMaster cardTypeMaster = TitleDataManager.FindById<CardTypeMaster>(i);
        //    CardData cardData = new() {
        //        cardTypeMaster = cardTypeMaster,
        //        masterData = null
        //    };
        //    selectedCardDataList.Add(cardData);
        //    selectedCardDataList.Add(cardData);
        //}

        // カード配置をランダム化するためにシャッフル
        return selectedCardDataList.OrderBy(_ => Guid.NewGuid()).ToList();
    }

    /// <summary>
    /// フロアのデータに基づいて出現するカードの種類を必要数作成
    /// </summary>
    /// <returns></returns>
    private List<CardTypeMaster> CreateCardTypeList() {
        List<CardTypeMaster> cardTypeMasterList = new();

        // FloorData のフィールドを Dictionary にまとめる
        Dictionary<CardEventType, int> typeCounts = new()  {
            { CardEventType.TreasureChest,   currentFloorData.treasureChest },
            { CardEventType.Blessing,        currentFloorData.blessing },
            { CardEventType.Enemy,           currentFloorData.enemy },
            { CardEventType.Trap,            currentFloorData.trap },
            { CardEventType.MemoryFragments, currentFloorData.memoryStone },
            { CardEventType.Stairs,          currentFloorData.key },
        };

        // 1枚以上ある種類のカードタイプを順番に登録
        foreach (var kvp in typeCounts) {
            if (kvp.Value > 0) {
                CardTypeMaster cardTypeMaster = DataBaseManager.instance.GetCardType(kvp.Key);
                cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, kvp.Value));
            }
        }

        // 重み付け辞書を作成
        Dictionary<CardEventType, int> weightTable = new() {
            { CardEventType.TreasureChest,   currentFloorData.treasureChestWeight },
            { CardEventType.Blessing,        currentFloorData.blessingWeight },
            { CardEventType.Enemy,           currentFloorData.enemyWeight },
            { CardEventType.Trap,            currentFloorData.trapWeight },
            { CardEventType.MemoryFragments, currentFloorData.memoryStoneWeight },
        };

        // ランダム要素がない場合には終了
        if (currentFloorData.random == 0) {
            return cardTypeMasterList;
        }

        // 合計 weight
        int totalWeight = weightTable.Values.Sum();
        if (totalWeight == 0) {
            return cardTypeMasterList;
        }

        System.Random random = new();

        // ランダムなカードタイプの抽出        
        for (int i = 0; i < currentFloorData.random; i++) {

            // 0 ～ totalWeight - 1 の乱数
            int randomValue = random.Next(totalWeight);

            // weightTable から当選カードを選ぶ
            int cumulative = 0;
            foreach (var kvp in weightTable) {

                // 累積和を使って重み付き抽選
                cumulative += kvp.Value;

                // 抽選した場合
                if (randomValue < cumulative) {
                    // ランダムなカードタイプを追加
                    CardTypeMaster cardTypeMaster = DataBaseManager.instance.GetCardType(kvp.Key);
                    cardTypeMasterList.Add(cardTypeMaster);

                    // foreach を抜ける
                    break;
                }
            }
        }

        //// 固定のカードタイプを登録(べた書きした場合)
        //if (currentFloorData.treasureChest != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.TreasureChest);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.treasureChest));
        //}

        //if (currentFloorData.blessing != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.Blessing);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.blessing));
        //}

        //if (currentFloorData.enemy != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.Enemy);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.enemy));
        //}

        //if (currentFloorData.trap != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.Trap);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.trap));
        //}

        //if (currentFloorData.memoryStone != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.MemoryFragments);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.memoryStone));
        //}

        //if (currentFloorData.key != 0) {
        //    CardTypeMaster cardTypeMaster = GetCardType(CardEventType.Stairs);
        //    cardTypeMasterList.AddRange(Enumerable.Repeat(cardTypeMaster, currentFloorData.key));
        //}

        return cardTypeMasterList;
    }

    /// <summary>
    /// CardData を元にカードモデルを作成して List に追加
    /// </summary>
    /// <param name="selectedCardDataList"></param>
    private void CreateCardModelList(List<CardData> selectedCardDataList) {
        int cardIndex = 0;
        foreach (CardData cardData in selectedCardDataList) {
            CardModelBase cardModel = cardFactory.CreateCardModel(cardData, cardIndex);
            cardModelList.Add(cardModel);
            cardIndex++;
        }
    }

    /// <summary>
    /// 各カードをクリックした際に実行
    /// </summary>
    /// <param name="selectedCard"></param>
    private void OnCardSelected(CardView cardView) {
        // 選択したカードをめくる
        onCardSelected.OnNext(cardView);
    }

    /// <summary>
    /// カードをめくり、1枚目か2枚目以降かによって処理を分岐
    /// </summary>
    /// <param name="selectedCard"></param>
    /// <returns></returns>
    private async UniTask HandleCardSelectionAsync(CardView cardView) {
        CardModelBase selectedCardModel = cardModelList[cardView.cardIndex];

        // 同じカードを選択した場合
        if (selectedCardModel.isFaceUp) return;

        // 表向きにする
        selectedCardModel.FaceUp();
        cardView.Flip(true);

        if (firstSelectedCardView == null) {
            // 1枚目
            firstSelectedCardView = cardView;
            firstSelectedCardModel = selectedCardModel;
        } else {
            // 階段ボタンを無効化
            btnStairs.interactable = false;

            // 2枚目 → 判定
            inputLocked = true;
            await UniTask.Delay((int)(flipDuration * 2 * 1000));

            // カードの種類が同じかどうかで判定
            if (firstSelectedCardModel.cardData.cardTypeMaster.cardEventType == selectedCardModel.cardData.cardTypeMaster.cardEventType) {
                // ペアになった場合
                firstSelectedCardView.Hide();
                cardView.Hide();

                firstSelectedCardModel.SetPair();
                selectedCardModel.SetPair();

                GameData.instance.MatchedPairCount.Value++;

                await UniTask.Delay((int)(flipDuration * 1000));

                // カードの種類とフロアデータから、カードのマスターデータを設定
                selectedCardModel.cardData.masterData = CreateCardData(selectedCardModel.cardData.cardTypeMaster.cardEventType, currentFloorData);
                //DebugLogger.Log($"selectedCardModel : {selectedCardModel}");
                //DebugLogger.Log($"selectedCardModel.cardData.masterData : {selectedCardModel.cardData.masterData}");

                // カードの効果を実行
                if (this == null || selectedCardModel == null || cts == null) return;
                await selectedCardModel.ExecuteCardAsync(cts.Token);
                //DebugLogger.Log("カード実行");

                GameData.instance.ComboPairCount.Value++;

                // すべてのカードを引き終わっているか確認
                if (cardModelList.All(model => model.isPair)) {
                    // 次のフロアへ
                    NextFloorAsync().Forget();
                }
            } else {
                // 裏向きにする
                firstSelectedCardView.Flip(false);
                cardView.Flip(false);

                firstSelectedCardModel.FaceDown();
                selectedCardModel.FaceDown();

                await UniTask.Delay((int)(flipDuration * 1000));

                // ペアにならなかった(お手付きした)ので、めくれる回数の減算
                GameData.instance.userData.FlipPoint.Value--;

                GameData.instance.ComboPairCount.Value = 0;
            }

            firstSelectedCardView = null;
            inputLocked = false;

            // 2回続けてカードをめくったので、ターン終了を知らせる
            onTurnEnd.OnNext(default);
        }

        // 最終フロアではなく、階段フラグが立っている場合には、再度階段ボタンを有効化
        if (GameData.instance.lastFloorCount != GameData.instance.userData.FloorCount.Value && GameData.instance.userData.CanUseStairs.Value == true) {
            btnStairs.interactable = true;
        }
    }

    public IMasterData CreateCardData(CardEventType type, FloorData floorData) {
        IMasterData chosenData = null;
        switch (type) {
            case CardEventType.Enemy:
                chosenData = DataBaseManager.instance.GetRandomEnemyByRarity(floorData.enemyRarities, floorData.enemyRates);
                break;
            case CardEventType.TreasureChest:
                chosenData = DataBaseManager.instance.GetRandomItemByChest(floorData.enemyRarities, floorData.enemyRates);
                break;
            case CardEventType.Trap:
                chosenData = debugTrapID != 0   // debugIDが 0 以外なら、指定 ID のトラップデバッグ。0 なら通常処理
                           ? DataBaseManager.instance.trapDataSO.trapDataList.FirstOrDefault(data => data.id == debugTrapID)
                           : DataBaseManager.instance.GetRandomTrapByRarity(floorData.trapRarities, floorData.trapRates);
                break;
            case CardEventType.Blessing:
                chosenData = debugBlessingID != 0   // debugIDが 0 以外なら、指定 ID のイベントデバッグ。0 なら通常処理
                           ? DataBaseManager.instance.blessingDataSO.blessingDataList.FirstOrDefault(data => data.id == debugBlessingID)
                           : DataBaseManager.instance.GetRandomBlessingByRarity(floorData.blessingRarities, floorData.blessingRate);
                break;

            case CardEventType.MemoryFragments:
                chosenData = DataBaseManager.instance.GetRandomMemoryStoneByWeight();
                break;

            // デバッグ用
            //case CardEventType.Blessing:
            //    chosenData = DataBaseManager.instance.blessingDataSO.blessingDataList.FirstOrDefault(data => data.id == debugBlessingID);
            //    break;

                // ... 他も同様
        }

        return chosenData;
    }

    /// <summary>
    /// 次のフロアへ進む
    /// </summary>
    /// <returns></returns>
    private async UniTask NextFloorAsync() {
        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        //SceneManager.LoadScene("Main");

        // クリアしたフロアが最終フロアか確認
        if (GameData.instance.userData.FloorCount.Value >= GameData.instance.lastFloorCount) {
            GameData.instance.CurrentGameState.Value = GameState.GameUp;

            // ゲームクリア
            cgSlotSet.blocksRaycasts = false;
            btnRetry.interactable = false;
            btnStairs.interactable = false;

            SoundManager.instance.PlayBGM(BGM_TYPE.Clear);
            DebugLogger.Log("ゲームクリア");

            // ゲームクリア画像へ変更
            ChangeBackGroundImageGameClear();

            // データセーブ
            string saveId = Guid.NewGuid().ToString();
            List<ItemData> itemDatalist = new(playerInventoryManager.PlayerBackPackItemList.ToList().Select(data => new ItemData(data.itemData)).ToList());
            List<int> enhanceLevelList = playerInventoryManager.PlayerBackPackItemList.ToList().Select(data => data.EnhanceLevel.Value).ToList();
            SaveData saveData = new(saveId, GameData.instance.userData, itemDatalist, enhanceLevelList);
            PlayerPrefsHelper.ConditionalSave(saveData);

            await UniTask.Delay(1000, cancellationToken: cts.Token);
            SoundManager.instance.PlayVoice(VOICE_TYPE.Win);

            // ゲームクリアのメッセージ表示
            //.ShowRestartMessage();

            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cts.Token);

            // TODO 一旦タイトルへ
            SceneStateManager.instance.PrepareteNextScene(SceneName.Title);
            return;
        }

        // デバッグ用
        //GameData.instance.userData.FlipPoint.Value += debugFloorClearBonusFlipPoint;

        // めくれる回数のフロアクリアボーナスを加算
        //DebugLogger.Log($"currentFloorData : {currentFloorData.minFloorCount} / {currentFloorData.clearFlipBonus}");

        int bonusFlipPoint = GameData.instance.MatchedPairCount.Value / 2;
        GameData.instance.userData.FlipPoint.Value += bonusFlipPoint;
        DebugLogger.Log($"bonusFlipPoint : {bonusFlipPoint}");

        ResetFloorState();

        await UniTask.Delay(500);

        //GameData.instance.userData.FlipPoint.Value += currentFloorData.clearFlipBonus;

        // フロア数を増やす
        GameData.instance.userData.FloorCount.Value++;

        InitFloorAsync().Forget();
    }


    // UI 関連。一旦ここに書いておいてあとで分割する

    private void UpdateDisplayFlipPoint(int prevPoint, int nextPoint) {
        txtFlipPoint.DOCounter(prevPoint, nextPoint, 0.5f).SetEase(Ease.Linear).SetLink(gameObject);
    }

    private void UpdateDisplayFloorCount(int newFloorCount) {
        txtFloorCount.text = newFloorCount.ToString();
    }

    private void UpdateDisplayMemoriaRank(int newMemoriaRank) {
        txtMemoriaRank.text = newMemoriaRank.ToString();
    }

    private void UpdateDisplayNextBonusFlipPoint(int matchedPairCount) {
        int bonusFlipPoint = matchedPairCount / 2;
        txtMatchedPairCount.text = bonusFlipPoint.ToString();
    }

    /// <summary>
    /// 思い出の断片を UI に表示、光らせる
    /// </summary>
    /// <param name="memoryStoneIndex"></param>
    public void SetMemoryStoneIcon(int memoryStoneIndex) {
        //imgMemoryStoneIcons[memoryStoneCount].sprite = spriteMemoryStone;
        imgMemoryStoneIcons[memoryStoneIndex].enabled = true;
        shinyEffects[memoryStoneIndex].Play(0.75f);

        // 3つ目の思い出の断片を獲得したら、すべてを光らせる
        if (memoryStoneIndex == 2) {
            FlashIconsAsync().Forget();
        }
    }

    /// <summary>
    /// 思い出の断片アイコンを点滅させる
    /// </summary>
    /// <returns></returns>
    public async UniTask FlashIconsAsync() {
        await UniTask.Delay(1000);

        for (int i = 0; i < imgMemoryStoneIcons.Length; i++) {
            shinyEffects[i].Play();
        }

        await UniTask.Delay(1000);

        // すべての思い出の断片を UI から消す
        InitMemoryStoneIcons();
    }

    public void InitMemoryStoneIcons() {
        for (int i = 0; i < imgMemoryStoneIcons.Length; i++) {
            imgMemoryStoneIcons[i].enabled = false;
        }
    }

    /// <summary>
    /// コンボ時のソウルポイントのボーナス
    /// </summary>
    /// <param name="comboCount"></param>
    private void CheckComboEffect(int comboCount) {
        if (comboCount == 0) {
            return;
        }

        // フロアの階層分のボーナス
        float floorBonusRate = 0.9f + (float)(GameData.instance.userData.FloorCount.Value * 0.1f);
        //DebugLogger.Log($"floorBonusRate : {floorBonusRate}");

        int comboBonusPoint = (int)((comboCount - 1) * (debugComboBonusRate * floorBonusRate));
        if (comboBonusPoint == 0) {
            return;
        }
        //DebugLogger.Log($"comboBonusPoint : {comboBonusPoint}");

        GameData.instance.userData.SoulPoint.Value += comboBonusPoint;
    }

    /// <summary>
    /// 指定された種類のカードを表向き(半透明)して見えるようにする
    /// </summary>
    /// <param name="blessingData"></param>
    /// <returns></returns>
    public async UniTask FaceUpCardsByTargetTypeAsync(BlessingData blessingData) {
        CardEventType targetCardEventType = ConvertCardEventTypeByBlessingValueType(blessingData.valueType);
        List<CardModelBase> targetCardModelList = cardModelList.Where(card => card.isPair == false && card.cardData.cardTypeMaster.cardEventType == targetCardEventType).ToList();
        if (targetCardModelList.Count == 0) {
            DebugLogger.Log($"該当する種類のカードなし");
            //return;
        }

        List<CardView> targetCardViewList = cardViewList.Where(view => targetCardModelList.Exists(model => model.cardData.cardTypeMaster.cardEventType == view.cardEventType)).ToList();

        // 指定された種類のカードを見える状態にする
        targetCardViewList.ForEach(cardView => cardView?.FaceUpCard());
        //DebugLogger.Log($"blessingData.valueType : {blessingData.valueType}");

        // マッピングしてある情報と照合
        if (lookCountMap.TryGetValue(blessingData.valueType, out (SerializableReactiveProperty<int> lookCount, CompositeDisposable disposables) lookTarget)) {
            bool isFirstActivation = lookTarget.lookCount.Value == 0;

            // 該当する種類のカードの効果時間を加算
            lookTarget.lookCount.Value += (int)blessingData.value;

            // 未購読時のみ購読登録とアイコン表示(購読処理が動いているなら、同じ効果の時間加算による重複を防止したいのでやらない)
            //DebugLogger.Log($"lookTarget.disposables.Count : {lookTarget.disposables.Count}");
            if (isFirstActivation) {
                // 新しく作り直す(Dictionary 内で更新をかけない)
                // 更新をしてもクロージャ(メソッドが終わっても、生き続けるローカル変数)は古い参照を保持してしまうため
                // クロージャは「変数の値」ではなく「変数そのもの」を掴む
                lookTarget.disposables = new CompositeDisposable();
                //DebugLogger.Log($"lookTarget.disposables.Count : {lookTarget.disposables.Count}");

                // 0 になったときにカードを伏せる購読処理
                lookTarget.lookCount
                    .Where(lookCount => lookCount == 0)
                    .Take(1)
                    .Subscribe(lookCount => FaceDownCardsByTargetType(blessingData.valueType, lookTarget.disposables))
                    .AddTo(lookTarget.disposables);

                // 0 以外(効果中)のときにカードを可視にする購読
                lookTarget.lookCount
                    .Where(lookCount => lookCount > 0)
                    .Subscribe(lookCount => {
                        RefreshFaceUpCards(blessingData.valueType);
                    })
                    .AddTo(lookTarget.disposables);

                // アイコン表示
                BlessingIconView blessingIconView = (BlessingIconView)blessingIconGenerator.GetObjectFromPool(blessingIconViewTran);
                blessingIconView.Setup(blessingData);

                // 残り時間表示更新処理の購読
                lookTarget.lookCount.Subscribe(lookCount => blessingIconView.UpdateDisplayDuration(lookCount)).AddTo(lookTarget.disposables);

                blessingIconViewList.Add(blessingIconView);
            }
        }

        await UniTask.Yield();
    }

    /// <summary>
    /// すべてのカードを種類ごとに判定して、それぞれ表向き(半透明)にする
    /// フロアが進んだとき、そのフロアのカードがセットされた後に実行される
    /// </summary>
    private void RefreshAllFaceUpCards() {
        foreach (var kvp in lookCountMap) {
            BlessingValueType valueType = kvp.Key;
            var (lookCount, _) = kvp.Value;

            // 効果が残っているものだけ
            if (lookCount.Value > 0) {
                RefreshFaceUpCards(valueType);
            }
        }
    }

    /// <summary>
    /// 対象の種類のカードを表向き(半透明)にする
    /// </summary>
    /// <param name="valueType"></param>
    private void RefreshFaceUpCards(BlessingValueType valueType) {
        CardEventType targetEventType = ConvertCardEventTypeByBlessingValueType(valueType);
        DebugLogger.Log($"blessingData.valueType : {valueType}");
        
        List<CardModelBase> targetModelList = cardModelList.Where(card => card.isPair == false && card.cardData.cardTypeMaster.cardEventType == targetEventType).ToList();
        if (targetModelList.Count == 0) {
            DebugLogger.Log($"効果終了となる種類のカードなし");
            return;
        }

        List<CardView> targetViewList = cardViewList.Where(view => targetModelList.Exists(model => model.cardData.cardTypeMaster.cardEventType == view.cardEventType)).ToList();

        targetViewList.ForEach(cardView => cardView?.FaceUpCard());
    }

    /// <summary>
    /// BlessingValueType を CardEventType に変換
    /// </summary>
    /// <param name="blessingValueType"></param>
    /// <returns></returns>
    private CardEventType ConvertCardEventTypeByBlessingValueType(BlessingValueType blessingValueType) {
        return blessingValueType switch {
            BlessingValueType.Trap => CardEventType.Trap,
            BlessingValueType.Enemy => CardEventType.Enemy,
            BlessingValueType.Memory => CardEventType.MemoryFragments,
            BlessingValueType.TreasureChest => CardEventType.TreasureChest,
            BlessingValueType.Event => CardEventType.Blessing,
            BlessingValueType.Key => CardEventType.Stairs,
            _ => CardEventType.Stairs
        };
    }

    /// <summary>
    /// 残り時間 0 になったら実行される
    /// </summary>
    /// <param name="blessingData"></param>
    public void FaceDownCardsByTargetType(BlessingValueType blessingValueType, CompositeDisposable disposables) {
        // 該当するアイコン削除
        blessingIconViewList.ForEach(view => view?.CheckEndBlessing(BlessingType.Look, blessingValueType));

        CardEventType targetCardEventType = ConvertCardEventTypeByBlessingValueType(blessingValueType);
        List<CardModelBase> targetCardModelList = cardModelList.Where(card => card.isPair == false && card.cardData.cardTypeMaster.cardEventType == targetCardEventType).ToList();
        if (targetCardModelList.Count == 0) {
            DebugLogger.Log($"該当する種類のカードなし");
            return;
        }

        List<CardView> targetCardViewList = cardViewList.Where(view => targetCardModelList.Exists(model => model.cardData.cardTypeMaster.cardEventType == view.cardEventType)).ToList();

        targetCardViewList.ForEach(cardView => cardView.FaceDownCard());
        //disposables.Clear();
        //disposables = new();  // null にせず、Count 0 として判定させるため

        // 購読を完全終了(古い参照を保持している Subscribe 内のクロージャも完全破棄)
        disposables.Dispose();

        // 該当するアイコン削除
        blessingIconViewList.ForEach(view => view?.CheckEndBlessing(BlessingType.Look, blessingValueType));

        DebugLogger.Log($"効果時間終了 : {blessingValueType}");
    }

    /// <summary>
    /// 敵かトラップのカードのいずれかを選択して破壊処理へつなげる
    /// </summary>
    /// <param name="blessingData"></param>
    /// <returns></returns>
    public async UniTask<bool> ChooseDestroyEnemyOrTrapCardAsync(BlessingData blessingData) {

        // 敵かトラップの「残っている Model」を取得
        List<CardModelBase> restEnemyModels = cardModelList
            .Where(model =>
                !model.isPair &&
                model.cardData.cardTypeMaster.cardEventType == CardEventType.Enemy)
            .ToList();

        List<CardModelBase> restTrapModels = cardModelList
            .Where(model =>
                !model.isPair &&
                model.cardData.cardTypeMaster.cardEventType == CardEventType.Trap)
            .ToList();

        bool canDestroyEnemy = restEnemyModels.Count >= 2;
        bool canDestroyTrap = restTrapModels.Count >= 2;

        // どちらもペア不可なら終了
        if (!canDestroyEnemy && !canDestroyTrap)
            return false;

        // どちらを破壊するか決定
        CardEventType targetType;

        if (canDestroyEnemy && canDestroyTrap) {
            targetType = UnityEngine.Random.Range(0, 2) == 0
                ? CardEventType.Enemy
                : CardEventType.Trap;
        } else {
            targetType = canDestroyEnemy
                ? CardEventType.Enemy
                : CardEventType.Trap;
        }

        // 対象 Model を2枚選択(順番は任意)
        List<CardModelBase> targetModels = (targetType == CardEventType.Enemy
                ? restEnemyModels
                : restTrapModels)
            .Take(2)
            .ToList();

        // 念のため
        if (targetModels.Count < 2)
            return false;

        // cardIndex で View を取得(Dictionary にして安全に引く)
        Dictionary<int, CardView> viewByIndex = cardViewList.ToDictionary(v => v.cardIndex);

        if (!viewByIndex.TryGetValue(targetModels[0].cardIndex, out var firstView) ||
            !viewByIndex.TryGetValue(targetModels[1].cardIndex, out var secondView)) {
            // Model と View の不整合(設計エラー)
            DebugLogger.Log("CardView が見つかりません");
            return false;
        }

        // ペア破壊
        await DestroyTargetPairCard(targetModels[0], firstView, targetModels[1], secondView);
        return true;
    }

    /// <summary>
    /// 対象のカードのペアを破壊
    /// </summary>
    /// <param name="firstCardModel"></param>
    /// <param name="firstCardView"></param>
    /// <param name="secoundCardModel"></param>
    /// <param name="secoundCardView"></param>
    /// <returns></returns>
    private async UniTask DestroyTargetPairCard(CardModelBase firstCardModel, CardView firstCardView, CardModelBase secoundCardModel, CardView secoundCardView) {
        // View 側状態更新
        firstCardView.isPaired = true;
        secoundCardView.isPaired = true;

        firstCardModel.SetPair();
        secoundCardModel.SetPair();

        // ペア破壊アニメーションが両方終了するまで待機
        await UniTask.WhenAll(
            firstCardView.PlayHideAnimationAsync(cts.Token),
            secoundCardView.PlayHideAnimationAsync(cts.Token)
        );

        // カードの効果を実行
        if (this == null || secoundCardModel == null || cts == null) return;
        DebugLogger.Log("カード破棄実行");
    }


    public void ChangeCardsByType(BlessingData blessingData) {

    }

    /// <summary>
    /// 背景用の画像のランダム変更
    /// </summary>
    public void RandomChangeBackGroundImage() {
        // 配列の長さ
        int length = spriteBackGrounds.Length;

        // 現在の値を除外したランダムインデックス生成
        int newIndex = Enumerable.Range(0, length)   // 0 ～ length-1 までのインデックス一覧を作成
            .Where(i => i != backGroundIndex)        // 現在の index を除外
            .OrderBy(_ => Guid.NewGuid())            // 全要素をランダム順に並び替え
            .First();                                // その中の先頭(＝ランダム値)を取得

        backGroundIndex = newIndex;

        // フェード処理による画像の変更
        imgBackGround.DOFade(0f, 0.25f).SetEase(Ease.Linear)
            .OnComplete(() => {
                // 背景用の画像の設定
                imgBackGround.sprite = spriteBackGrounds[newIndex];
                imgBackGround.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);
            }).SetLink(gameObject);
    }

    /// <summary>
    /// BGM ランダム変更
    /// </summary>
    public void SetRandomBGM() {
        int randomStageBgmIndex = UnityEngine.Random.Range(3, 7);
        SoundManager.instance.PlayBGM((BGM_TYPE)randomStageBgmIndex);
    }

    /// <summary>
    /// 最終フロア用の背景画像に変更
    /// </summary>
    private void ChangeBackGroundImageLastFloor() {
        // フェード処理による画像の変更
        imgBackGround.DOFade(0f, 0.25f).SetEase(Ease.Linear)
            .OnComplete(() => {
                // 背景用の画像の設定
                imgBackGround.sprite = spriteLastFloorBackGround;
                imgBackGround.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);
            }).SetLink(gameObject);
    }

    /// <summary>
    /// ゲームクリア用の背景画像に変更
    /// </summary>
    private void ChangeBackGroundImageGameClear() {
        // フェード処理による画像の変更
        imgBackGround.DOFade(0f, 0.25f).SetEase(Ease.Linear)
            .OnComplete(() => {
                // 背景用の画像の設定
                imgBackGround.sprite = spriteGameClear;
                imgBackGround.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);
            }).SetLink(gameObject);
    }

    private void OnDestroy() {
        if (lookCountMap != null) {
            foreach (var entry in lookCountMap.Values) {
                // 動いているものだけ購読解除
                entry.Item2?.Dispose();
            }
        }

        floorCountDisposable?.Dispose();
        cardCreateDisposable?.Dispose();
    }

    private async UniTask GameEndAsync() {
        cgSlotSet.blocksRaycasts = false;
        btnRetry.interactable = false;
        btnStairs.interactable = false;

        if (cts != null) {
            cts.Cancel();  // すべての Hoge メソッドの実行をキャンセルする
            cts.Dispose(); // 古いトークンソースを破棄
            cts = null;    // 参照をクリア
            DebugLogger.Log("Cancel");
        }

        await UniTask.Delay(500);
        DebugLogger.Log($"ゲーム終了");

#if UNITY_EDITOR
        SceneStateManager.instance.PrepareteNextScene(SceneName.Stage);
        //UnityEditor.EditorApplication.isPlaying = false;  // エディター再生を停止
#else
    Application.Quit();  // ビルド後はアプリ終了
#endif
    }
}