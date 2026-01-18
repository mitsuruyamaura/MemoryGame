using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;


public enum FloatingViewType {
    normalDamage,
    dodge,
    critical,
    heal,
    shield,
    reaction
}

/// <summary>
/// フロート表示用
/// </summary>
public class FloatingView : TextViewBase {

    [SerializeField] private bool isAnimOn;

    [SerializeField] private float moveScale;
    [SerializeField] private float moveDuration;
    //[SerializeField] private Ease fadeOut = Ease.InOutExpo;

    [SerializeField] private float criticalFontSize = 24.0f;
    [SerializeField] private float normalFontSize = 24.0f;

    // アニメ設定用
    [SerializeField] private float minAngle = -45.0f;    // 上空方向の角度の最小値
    [SerializeField] private float maxangle = 45.0f;     // 上空方向の角度の最大値

    [SerializeField] private float launchForce = 2.0f;   // 上空方向への発射力
    [SerializeField] private float bounceWidth = 3.5f;   // 左右方向の距離
    [SerializeField] private float fallHeight = 0.5f;    // 落下距離
    [SerializeField] private float fallTime = 0.35f;     // 落下にかかる時間

    [SerializeField] private int numBounces = 2;         // バウンドの回数
    [SerializeField] private float bouncePower = 0.5f;   // バウンドの強さ
    //[SerializeField] private float bounceRate = 1.25f;   // バウンドの補正倍率
    [SerializeField] private float duration = 0.35f;     // 落下後のバウンドの時間

    private float defaultScale = 1.0f;                   // フロート表示の初期サイズ

    [Header("文字の色")]
    [SerializeField] private Color normalDamageColor;
    [SerializeField] private Color dodgeColor;
    [SerializeField] private Color criticalColor;
    [SerializeField] private Color healColor;
    [SerializeField] private Color shieldColor;
    [SerializeField] private Color reactionColor;
    private FloatingViewType floatingViewType = FloatingViewType.normalDamage;

    protected RectTransform rectTransform;
    protected Vector2 startValuePos;
    protected float valueFontSize = 45.0f;

    /// <summary>
    /// ダメージのフロート表示用
    /// </summary>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    public override async UniTask UpdateTextAsync(string newMessage) {
        string type = floatingViewType == FloatingViewType.critical ? $"Critical!\n" : "";
        newMessage = type + newMessage;
        
        base.UpdateTextAsync(newMessage).Forget();

        // アニメさせる場合
        if (isAnimOn) {
            await BounceAsync();
        } 

        //// オブジェクトプールへ戻す
        Release();
    }

    /// <summary>
    /// めくれる回数、ソウルポイントのフロート表示用
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async UniTask FloatText(int value) {
        if(rectTransform == null) {
            rectTransform = transform as RectTransform;
            startValuePos = rectTransform.anchoredPosition;
        }

        txtView.fontSize = valueFontSize;
        rectTransform.anchoredPosition = Vector2.zero;

        // 表示する文字列の作成。0 以上の値の場合は「+」を付与
        string text = value >= 0 ? $"+{value}" : value.ToString();

        base.UpdateTextAsync(text).Forget();

        txtView.alpha = 1;
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);

        if (value >= 0) {
            sequence.Append(rectTransform.DOAnchorPosY(50, moveDuration).SetEase(Ease.Linear));
            sequence.Join(txtView.DOFade(0, moveDuration).SetEase(Ease.Linear));
        } else {
            sequence.Append(rectTransform.DOAnchorPosY(-50, moveDuration).SetEase(Ease.Linear));
            sequence.Join(txtView.DOFade(0, moveDuration).SetEase(Ease.Linear));
        }

        await sequence.AsyncWaitForCompletion();

        //// オブジェクトプールへ戻す
        Release();
    }

    /// <summary>
    /// 文字の色を設定
    /// </summary>
    /// <param name="entityType"></param>
    /// <param name="viewType"></param>
    public void SetColor(FloatingViewType viewType) {
        floatingViewType = viewType;

        txtView.color =viewType switch {
            FloatingViewType.normalDamage => normalDamageColor,
            FloatingViewType.dodge => dodgeColor,
            FloatingViewType.critical => criticalColor,
            FloatingViewType.heal => healColor,
            FloatingViewType.shield => shieldColor,
            FloatingViewType.reaction => reactionColor,
            _ => Color.white
        };
    }

    /// <summary>
    /// 文字の色を初期色(白)に戻す
    /// </summary>
    private void SetDefaultColorAndFontSize() {
        txtView.color = Color.white;
        txtView.fontSize = normalFontSize;
    }

    /// <summary>
    /// フォントサイズの変更
    /// </summary>
    /// <param name="viewType"></param>
    public void SetViewFontSize(FloatingViewType viewType)
    {
        switch (viewType){
            case FloatingViewType.dodge:
            case FloatingViewType.critical:
            case FloatingViewType.reaction:
                txtView.fontSize = criticalFontSize;
                break;

            case FloatingViewType.normalDamage:
            case FloatingViewType.shield:
            case FloatingViewType.heal:
                txtView.fontSize = normalFontSize;
                break;

            default:
                txtView.fontSize = normalFontSize;
                break;
        }
    }

    private async Task BounceAsync() {
        // リセット
        transform.localPosition = Vector3.zero;

        // World Space Canvas なので Transform.position を使う
        Vector3 startPos = transform.position;

        // プレイヤーの位置を中心にランダムな角度を生成
        float randomAngle = Random.Range(minAngle, maxangle);

        // ランダムな角度を UI の発射方向として設定
        Vector3 launchDirection = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        Sequence sequence = DOTween.Sequence();

        // UI の移動先の設定(放物線になるように順番にセット)
        // 放物線のピークと終点を計算
        Vector3 peakPos = startPos + launchDirection * launchForce;
        Vector3 endPos = new Vector3(
            startPos.x + (launchForce * launchDirection.x) * 2,
            startPos.y - (launchForce * launchDirection.y),
            startPos.z
        );

        // 放物線移動
        sequence.Append(transform.DOMove(peakPos, fallTime * 0.5f).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOMove(endPos, fallTime * 0.5f).SetEase(Ease.InQuad));

        // 落下後のバウンド
        Vector3 bouncePos = new Vector3(
            startPos.x + (bounceWidth * launchDirection.x),
            startPos.y - fallHeight,
            startPos.z
        );

        sequence.Append(transform.DOJump(bouncePos, bouncePower, numBounces, duration));

        // フェードアウト用
        sequence.Join(transform.DOScale(Vector3.zero, duration));

        sequence.SetLink(gameObject);

        await sequence.AsyncWaitForCompletion();
    }

        /// <summary>
    /// オブジェクトプールへ戻す
    /// </summary>
    public override void Release() {
        if (isReleased) {
            return;
        }

        // 文字の色を白に戻す
        SetDefaultColorAndFontSize();

        // フェードアウトした後に初期値に戻す
        if (this == null || canvasGroupTextView == null) return;
        canvasGroupTextView.alpha = 1.0f;
        transform.localScale = Vector3.one * defaultScale;
        floatingViewType = FloatingViewType.normalDamage;

        //Debug.Log(objectPool);
        ObjectPool.Release(this);
    }
}