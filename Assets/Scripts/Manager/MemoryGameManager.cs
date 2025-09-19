using System.Collections.Generic;
using UnityEngine;
using R3;
using Cysharp.Threading.Tasks;

public class MemoryGameManager : MonoBehaviour {
    [SerializeField] private Transform cardParent;
    [SerializeField] private CardGenerator cardGenerator;
    [SerializeField] private int pairCount = 3;

    [SerializeField] private List<Card> cardList = new ();
    [SerializeField] private Card firstSelectedCard;
    [SerializeField] private bool inputLocked = false;

    private readonly Subject<Card> onCardSelected = new(); // カード選択イベント

    private void Start() {
        cardGenerator.InitObjectPool();

        SetupCards();

        // R3でカード選択イベントを購読
        onCardSelected
            .Where(_ => !inputLocked) // ロック中は無視
            .Subscribe(card => HandleCardSelectionAsync(card).Forget())
            .AddTo(this); // GameObject破棄時に購読解除
    }

    private void SetupCards() {
        // 1〜pairCountのIDを2枚ずつ作成
        List<int> cardIDs = new ();
        for (int i = 0; i < pairCount; i++) {
            cardIDs.Add(i);
            cardIDs.Add(i);
        }

        // シャッフル
        for (int i = 0; i < cardIDs.Count; i++) {
            int rand = Random.Range(i, cardIDs.Count);
            (cardIDs[i], cardIDs[rand]) = (cardIDs[rand], cardIDs[i]);
        }

        // カード生成
        foreach (int id in cardIDs) {
            Card card = (Card)cardGenerator.GetObjectFromPool(cardParent);
            card.Setup(id, OnCardSelected);
            cardList.Add(card);
        }
    }

    private void OnCardSelected(Card selectedCard) {
        onCardSelected.OnNext(selectedCard);
    }

    private async UniTask HandleCardSelectionAsync(Card selectedCard) {
        if (selectedCard.IsFaceUp) return;

        selectedCard.Flip(true);

        if (firstSelectedCard == null) {
            // 1枚目
            firstSelectedCard = selectedCard;
        } else {
            // 2枚目 → 判定
            inputLocked = true;
            await UniTask.Delay(500);

            if (firstSelectedCard.CardID == selectedCard.CardID) {
                firstSelectedCard.Hide();
                selectedCard.Hide();
            } else {
                firstSelectedCard.Flip(false);
                selectedCard.Flip(false);
            }

            firstSelectedCard = null;
            inputLocked = false;
        }
    }
}