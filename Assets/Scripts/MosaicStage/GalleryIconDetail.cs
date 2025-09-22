using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class GalleryIconDetail : MonoBehaviour
{
    [SerializeField]
    private Image imgGalleryChara;

    [SerializeField]
    private Image imgFrame;

    [SerializeField]
    private Button btnGalleryIcon;

    private bool isZoomIn;
    public bool IsZoomIn { get => isZoomIn; }

    private Vector3 startPos;
    private Vector3 zoomInPos;
    private HoverButton hoverButton;

    
    public void SetUp(Sprite charaSprite, Sprite frameSprite, Vector3 zoomInPos) {
        startPos = transform.localPosition;

        imgGalleryChara.sprite = charaSprite;
        imgFrame.sprite = frameSprite;
        this.zoomInPos = zoomInPos;

        TryGetComponent(out hoverButton);
    }


    public Button GetButton() {
        return btnGalleryIcon;
    }


    public void ZoomInGalleryIcon() {
        isZoomIn = true;
        // 画像の優先順位を最前面に変更
        transform.parent.SetAsLastSibling();

        // ボタンのホバーを切る
        hoverButton.enabled = false;

        // Sequence を初期化して利用できる状態にする
        Sequence sequence = DOTween.Sequence();

        // ポップアップをボタンの位置から画面の中央(Canvas ゲームオブジェクトの位置)に移動させつつ
        sequence.Append(transform.DOMove(zoomInPos, 0.5f).SetEase(Ease.Linear));

        // ポップアップを徐々に大きくしながら表示。指定したサイズになったら、元のポップアップの大きさに戻す
        sequence.Join(transform.DOScale(Vector2.one * 4.2f, 0.5f).SetEase(Ease.InBack)).OnComplete(() => { transform.DOScale(Vector2.one * 3.8f, 0.2f); }).SetLink(gameObject);
    }


    public void ZoomOutGalleryIcon() {

        // Sequence を初期化して利用できる状態にする
        Sequence sequence = DOTween.Sequence();

        // ポップアップの大きさを徐々に 0 にして見えない状態にさせつつ
        sequence.Append(transform.DOScale(Vector2.one, 0.3f).SetEase(Ease.Linear));

        // それに合わせてポップアップをアルバムボタンの位置に移動させる。移動後にポップアップを破棄
        // DOLocalMove メソッドにするとボタンの位置に戻らないため、DOMove メソッドを使う
        sequence.Join(transform.DOLocalMove(startPos, 0.5f)
            .SetEase(Ease.Linear))
            .SetLink(gameObject)
            .OnComplete(() => 
            {
                // ズーム状態を戻し、ホバー機能を入れなおす
                isZoomIn = false;
                hoverButton.enabled = true;
            });
    }
}