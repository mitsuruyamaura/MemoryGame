using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
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

    public bool isPaired;
    public int cardIndex;
    public CardEventType cardEventType;
    private Color defaultColor = new(1, 1, 1, 1);
    private Color currentColor = new(1, 1, 1, 1);
    private Sprite currentSprite;


    public void Setup(int cardIndex, UnityAction<CardView> onSelectedCallback, float flipDuration, Sprite spriteCardType, CardEventType cardEventType) {
        this.flipDuration = flipDuration;
        this.spriteCardType = spriteCardType;
        this.cardIndex = cardIndex;
        this.cardEventType = cardEventType;

        isReleased = false;
        isPaired = false;
        rectTransform = (RectTransform)transform;

        imgCard.sprite = defaultSprite;
        currentSprite = imgCard.sprite;

        disposable = btnCard.OnClickExt(() => onSelectedCallback?.Invoke(this), this);
    }

    /// <summary>
    /// カードを裏返して表に向ける
    /// </summary>
    /// <param name="faceUp"></param>
    public void Flip(bool faceUp) {
        rectTransform.DOScaleX(0, flipDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                if (faceUp) {
                    // 現在の画像と色(透明度)を保持してから変更
                    currentSprite = imgCard.sprite;
                    imgCard.sprite = spriteCardType;
                    
                    currentColor = imgCard.color;
                    imgCard.color = defaultColor;
                } else {
                    // 表になる前の画像と色(透明度)に戻す
                    imgCard.sprite = currentSprite;
                    imgCard.color = currentColor;
                }
                
                rectTransform.DOScale(1.0f, flipDuration).SetEase(Ease.Linear);
            }).SetLink(gameObject);
    }

    /// <summary>
    /// カードを裏返して隠す
    /// </summary>
    public void Hide() {
        rectTransform.DOScaleX(0, flipDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(() => Release()).SetLink(gameObject);
    }

    /// <summary>
    /// アニメーションが終了するまで待機する形式で、カードを裏返して隠す
    /// 複数枚のカードを同時に隠す(破棄する)場合などに使用
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask PlayHideAnimationAsync(CancellationToken token) {
        await rectTransform.DOScaleX(0, flipDuration).SetEase(Ease.InQuart).SetLink(gameObject).AsyncWaitForCompletion();
    }

    /// <summary>
    /// カードの透視効果適用
    /// </summary>
    public void FaceUpCard() {
        imgCard.sprite = spriteCardType;
        imgCard.color = new(1, 1, 1, 0.5f);
    }

    /// <summary>
    /// カードの透視効果を終了
    /// </summary>

    public void FaceDownCard() {
        imgCard.sprite = defaultSprite;
        imgCard.color = defaultColor;
    }

    /// <summary>
    /// オブジェクトプールに戻す
    /// </summary>
    public override void Release() {
        base.Release();

        if (this == null || transform == null) return;

        transform.SetParent(GameData.instance.transform);
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = Vector3.zero;
        imgCard.sprite = defaultSprite;
        imgCard.color = defaultColor;
    }
}