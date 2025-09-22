using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardView : PoolBase {
    [SerializeField] private Image imgCard;
    [SerializeField] private Button btnCard;
    [SerializeField] private Sprite defaultSprite;

    private RectTransform rectTransform;
    private float flipDuration;
    private Sprite spriteCardType;        // カードの表面のイラスト

    public int cardIndex;

    public void Setup(int cardIndex, UnityAction<CardView> onSelectedCallback, float flipDuration, Sprite spriteCardType) {
        this.flipDuration = flipDuration;
        this.spriteCardType = spriteCardType;
        this.cardIndex = cardIndex;

        isReleased = false;
        rectTransform = (RectTransform)transform;

        imgCard.sprite = defaultSprite;

        disposable = btnCard.OnClickExt(() => onSelectedCallback?.Invoke(this), this);
    }

    public void Flip(bool faceUp) {
        rectTransform.DOScaleX(0, flipDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                if (faceUp) {
                    imgCard.sprite = spriteCardType;
                } else {
                    imgCard.sprite = defaultSprite;
                }

                rectTransform.DOScale(1.0f, flipDuration).SetEase(Ease.Linear);
            });
    }

    public void Hide() {
        rectTransform.DOScaleX(0, flipDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(() => Release());
    }

    public override void Release() {
        base.Release();

        transform.SetParent(GameData.instance.transform);
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;
        imgCard.sprite = defaultSprite;
    }
}