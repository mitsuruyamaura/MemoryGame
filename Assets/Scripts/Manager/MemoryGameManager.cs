using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using R3.Triggers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGameManager : MonoBehaviour {
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] private int pairCount = 3;

    [SerializeField] private List<CardView> cardViewList = new ();
    [SerializeField] private List<CardModelBase> cardModelList = new ();
    [SerializeField] private CardView firstSelectedCardView;
    [SerializeField] private CardModelBase firstSelectedCardModel;
    [SerializeField] private bool inputLocked = false;
    [SerializeField] private float flipDuration = 0.3f;

    [SerializeField] private Button btnStairs;
    [SerializeField] private CanvasGroup cgSlotSet;
    [SerializeField] private TMP_Text txtFlipPoint;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [SerializeField] private int debugFlipPoint;
    [SerializeField] private int debugFloorClearBonusFlipPoint;

    private readonly Subject<CardView> onCardSelected = new(); // カード選択イベント
    private List<GameObject> slotList = new();                 // スロットを保持
    private CancellationTokenSource cts;


    private async void Start() {
        cts = new ();
        cardGenerator.InitObjectPool();

        // 仮
        GameData.instance.userData.FlipPoint.Value = debugFlipPoint != 0 ? debugFlipPoint : 0;

        // めくれる回数の表示更新
        GameData.instance.userData.FlipPoint
            .Zip(GameData.instance.userData.FlipPoint.Skip(1), (prevPoint, nextPoint) => (prevPoint, nextPoint))
            .Subscribe(soulPoint => UpdateDisplayFlipPoint(soulPoint.prevPoint, soulPoint.nextPoint))
            .AddTo(this);

        GameData.instance.userData.FlipPoint.OnNext(GameData.instance.userData.FlipPoint.Value);

        // 階段の状態の購読
        GameData.instance.userData.CanUseStairs.Subscribe(canUse => btnStairs.interactable = canUse).AddTo(this);

        ResetFloorState();

        await InitFloorAsync();

        // カード選択イベントを購読。選択したカードをめくる
        onCardSelected
            .Where(_ => !inputLocked) // ロック中は無視
            .Subscribe(cardView => HandleCardSelectionAsync(cardView).Forget())
            .AddTo(this);

        // 階段ボタンの購読
        btnStairs.OnClickExt(() => NextFloorAsync().Forget(), this);

        // めくれる回数の残り回数を確認し、ゲーム終了処理へつなげる
        GameData.instance.userData.FlipPoint
            .Where(flipPoint => flipPoint <= 0)
            .Take(1)
            .Subscribe(flipPoint => GameEndAsync().Forget()).AddTo(this);

        // デバッグ用リセット機能
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.A))
            .Subscribe(_ => NextFloorAsync().Forget())
            .AddTo(this);
    }

    /// <summary>
    /// フロアの初期化処理
    /// </summary>
    /// <returns></returns>
    private async UniTask InitFloorAsync() {
        // スロットを生成
        CreateSlots();

        // カードの情報準備と生成
        await SetupCardsAsync();

        cgSlotSet.blocksRaycasts = true;
    }

    /// <summary>
    /// カードを配置するためのスロットを生成(カードが消えても詰まらないようにするため)
    /// </summary>
    private void CreateSlots() {
        // GridLayoutGroup の Constraint Count を設定
        //gridLayoutGroup.constraintCount = 3;

        int totalSlots = pairCount * 2;
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
            CardView card = (CardView)cardGenerator.GetObjectFromPool(slotList[i].transform);
            card.Setup(index, OnCardSelected, flipDuration, selectedCardDataList[i].cardTypeMaster.spriteCardType);
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
        for (int i = 0; i < pairCount; i++) {
            CardTypeMaster cardTypeMaster = TitleDataManager.FindById<CardTypeMaster>(i);
            CardData cardData = new() {
                cardTypeMaster = cardTypeMaster,
                masterData = null
            };
            selectedCardDataList.Add(cardData);
            selectedCardDataList.Add(cardData);
        }

        // カード配置をランダム化するためにシャッフル
        return selectedCardDataList.OrderBy(_ => Random.value).ToList();
    }

    /// <summary>
    /// CardData を元にカードモデルを作成して List に追加
    /// </summary>
    /// <param name="selectedCardDataList"></param>
    private void CreateCardModelList(List<CardData> selectedCardDataList) {
        foreach (CardData cardData in selectedCardDataList) {
            CardModelBase cardModel = CreateCardModel(cardData);
            cardModelList.Add(cardModel);
        }
    }

    /// <summary>
    /// カードモデル作成
    /// </summary>
    /// <param name="cardData"></param>
    /// <returns></returns>
    private CardModelBase CreateCardModel(CardData cardData) {
        return cardData.cardTypeMaster.cardEventType switch {
            CardEventType.MemoryFragments => new MemoryFragmentsCard(cardData),
            CardEventType.TreasureChest => new TreasureChestCard(cardData),
            CardEventType.Blessing => new BlessingCard(cardData),
            CardEventType.Enemy => new EnemyCard(cardData),
            CardEventType.Stairs => new StairsCard(cardData),
            CardEventType.Trap => new TrapCard(cardData),
            _ => new StairsCard(cardData)
        };
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

        if (selectedCardModel.isFaceUp) return;

        cardView.Flip(true);

        if (firstSelectedCardView == null) {
            // 1枚目
            firstSelectedCardView = cardView;
            firstSelectedCardModel = selectedCardModel;
        } else {
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

                await UniTask.Delay((int)(flipDuration * 1000));

                // カードの効果を実行
                await selectedCardModel.ExecuteCardAsync(cts.Token);

                // すべてのカードを引き終わっているか確認
                if (cardModelList.All(model => model.isPair)) {
                    // 次のフロアへ
                    NextFloorAsync().Forget();
                }
            } else {
                firstSelectedCardView.Flip(false);
                cardView.Flip(false);
                await UniTask.Delay((int)(flipDuration * 1000));

                // ペアにならなかったので、めくれる回数の減算
                GameData.instance.userData.FlipPoint.Value--;
            }

            firstSelectedCardView = null;
            inputLocked = false;
        }
    }

    /// <summary>
    /// 次のフロアへ進む
    /// </summary>
    /// <returns></returns>
    private async UniTask NextFloorAsync() {
        //SceneManager.LoadScene("Main");

        // TODO 最終フロアか確認


        ResetFloorState();

        await UniTask.Delay(500);

        GameData.instance.userData.FlipPoint.Value += debugFloorClearBonusFlipPoint;

        InitFloorAsync().Forget();
    }

    /// <summary>
    /// フロアの状態をリセット
    /// </summary>
    private void ResetFloorState() {
        GameData.instance.userData.CanUseStairs.Value = false;

        firstSelectedCardView = null;
        firstSelectedCardModel = null;

        cardModelList.Clear();

        cardViewList.ForEach(cardView => cardView?.Release());
        cardViewList.Clear();

        slotList.ForEach(slot => Destroy(slot));
        slotList.Clear();

        cgSlotSet.blocksRaycasts = false;
    }


    private void UpdateDisplayFlipPoint(int prevPoint, int nextPoint) {
        txtFlipPoint.DOCounter(prevPoint, nextPoint, 0.5f).SetEase(Ease.Linear).SetLink(gameObject);
    }

    private async UniTask GameEndAsync() {
        cgSlotSet.blocksRaycasts = false;

        await UniTask.Delay(500);
        DebugLogger.Log($"ゲーム終了");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // エディター再生を停止
#else
    Application.Quit();  // ビルド後はアプリ終了
#endif
    }
}