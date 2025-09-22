using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;


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
public class FloatingView : TextViewBase, IPoolable {

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


    /// <summary>
    /// フロート表示更新
    /// </summary>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    public override async UniTask UpdateText(string newMessage) {

        string type = floatingViewType == FloatingViewType.critical ? $"Critical!\n" : "";
        newMessage = type + newMessage;
        
        base.UpdateText(newMessage).Forget();

        // アニメさせる場合
        if (isAnimOn) {
            Bounce();
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(moveDuration));

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

    /// <summary>
    /// オブジェクトプールへ戻す
    /// </summary>
    public override void Release() {
        // 文字の色を白に戻す
        SetDefaultColorAndFontSize();

        // フェードアウトした後に初期値に戻す
        canvasGroupTextView.alpha = 1.0f;
        transform.localScale = Vector3.one * defaultScale;
        transform.localPosition = Vector3.zero;
        floatingViewType = FloatingViewType.normalDamage;

        //Debug.Log(objectPool);
        ObjectPool.Release(this);
    }

    private void Bounce() {
        // プレイヤーの位置を中心にランダムな角度を生成
        float randomAngle = Random.Range(minAngle, maxangle);

        // ランダムな角度を UI の発射方向として設定
        Vector3 launchDirection = Quaternion.Euler(0, 0, randomAngle) * Vector3.up;

        Sequence sequence = DOTween.Sequence();

        // UI の移動先の設定(放物線になるように順番にセット)
        Vector3[] path = {
            new(transform.position.x, transform.position.y, 0),  // スタート地点
            transform.position + launchDirection * launchForce,  // 発射方向と最大地点
            new(transform.position.x + (launchForce * launchDirection.x) * 2, transform.position.y - (launchForce * launchDirection.y), 0)  // 落下地点
        };

        // UI の放物線移動
        sequence.Append(transform.DOPath(path, fallTime)).SetLink(gameObject);

        // 落下後の移動とサイズ
        sequence.Append(transform.DOJump(
                     new Vector3(transform.position.x + (bounceWidth * launchDirection.x) * 1.25f, transform.position.y - fallHeight, 0),
                     bouncePower, numBounces, duration))
                .SetLink(gameObject);

        sequence.Join(transform.DOScale(Vector3.zero, duration)).SetLink(gameObject);
    }
}