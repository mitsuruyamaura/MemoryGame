using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : PoolBase {
    [SerializeField] private Image cardImage;
    [SerializeField] private Button button;
    [SerializeField] private Sprite defaultSprite;

    public CardData cardData;

    private RectTransform rectTransform;
    private float FlipDuration;

    public int CardID { get; private set; }
    public bool IsFaceUp { get; private set; }

    private Action<CardBase> onSelected;

    public void Setup(CardData cardData, Action<CardBase> onSelectedCallback, float FlipDuration) {
        this.cardData = cardData;
        onSelected = onSelectedCallback;
        this.FlipDuration = FlipDuration;
        
        IsFaceUp = false;
        isReleased = false;
        rectTransform = (RectTransform)transform;
        rectTransform.localScale = Vector3.one;

        disposable = button.OnClickExt(() => onSelected?.Invoke(this), this);
    }

    public void Flip(bool faceUp) {
        IsFaceUp = faceUp;

        rectTransform.DOScaleX(0, FlipDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                if (IsFaceUp) {
                    cardImage.sprite = cardData.cardTypeMaster.spriteCardType;
                } else {
                    cardImage.sprite = defaultSprite;
                }

                rectTransform.DOScale(1.0f, FlipDuration).SetEase(Ease.Linear);
            });
    }

    public void Hide() {
        rectTransform.DOScaleX(0, FlipDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(() => Release());
    }


    public virtual async UniTask ExecuteCardAsync(CancellationToken token) {
        await UniTask.Yield();
    }
}