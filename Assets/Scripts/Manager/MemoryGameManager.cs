using Cysharp.Threading.Tasks;
using R3;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MemoryGameManager : MonoBehaviour {
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] private int pairCount = 3;

    [SerializeField] private List<CardBase> cardList = new ();
    [SerializeField] private CardBase firstSelectedCard;
    [SerializeField] private bool inputLocked = false;
    [SerializeField] private float FlipDuration = 0.3f;

    private readonly Subject<CardBase> onCardSelected = new(); // カード選択イベント
    private List<Transform> slotList = new(); // スロットを保持
    private CancellationTokenSource cts;


    private async void Start() {
        cts = new ();
        cardGenerator.InitObjectPool();

        // スロットを生成
        CreateSlots();

        // カードの情報準備と生成
        await SetupCardsAsync();

        // カード選択イベントを購読
        onCardSelected
            .Where(_ => !inputLocked) // ロック中は無視
            .Subscribe(card => HandleCardSelectionAsync(card).Forget())
            .AddTo(this); // GameObject破棄時に購読解除
    }

    private void CreateSlots() {
        int totalSlots = pairCount * 2;
        for (int i = 0; i < totalSlots; i++) {
            GameObject slot = Instantiate(slotPrefab, slotParent);
            slotList.Add(slot.transform);
        }
    }


    private async UniTask SetupCardsAsync() {
        // 各カードをペアで作成
        List<CardData> selectedCardDataList = CreateCardPairsAndShuffle();

        // カード生成
        for (int i = 0; i < selectedCardDataList.Count; i++) {
            await UniTask.Delay(200);
            CardBase card = (CardBase)cardGenerator.GetObjectFromPool(slotList[i]);
            card.Setup(selectedCardDataList[i], OnCardSelected, FlipDuration);
            cardList.Add(card);
        }
    }


    private List<CardData> CreateCardPairsAndShuffle() {
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

        // シャッフルして配置をランダム化
        return selectedCardDataList.OrderBy(_ => Random.value).ToList();
    }



    private void OnCardSelected(CardBase selectedCard) {
        onCardSelected.OnNext(selectedCard);
    }

    private async UniTask HandleCardSelectionAsync(CardBase selectedCard) {
        if (selectedCard.IsFaceUp) return;

        selectedCard.Flip(true);

        if (firstSelectedCard == null) {
            // 1枚目
            firstSelectedCard = selectedCard;
        } else {
            // 2枚目 → 判定
            inputLocked = true;
            await UniTask.Delay((int)(FlipDuration * 2 * 1000));

            // TODO 一旦、カードの種類が同じかどうかで判定
            if (firstSelectedCard.cardData.cardTypeMaster.cardType == selectedCard.cardData.cardTypeMaster.cardType) {
                firstSelectedCard.Hide();
                selectedCard.Hide();
                await UniTask.Delay((int)(FlipDuration * 1000));

                await selectedCard.ExecuteCardAsync(cts.Token);
            } else {
                firstSelectedCard.Flip(false);
                selectedCard.Flip(false);
                await UniTask.Delay((int)(FlipDuration * 1000));
            }

            firstSelectedCard = null;
            inputLocked = false;
        }
    }
}