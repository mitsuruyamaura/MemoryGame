using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using R3;

public class GalleryPopUp : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Button btnClose;

    [SerializeField]
    private GalleryIconDetail galleryIconPrefab;

    [SerializeField]
    private Transform[] galleryIconTrans;

    [SerializeField]
    private Sprite[] frameSprites;

    [SerializeField]
    private Transform zoomTran;

    private List<GalleryIconDetail> galleryIconList = new();
    private ReactiveProperty<bool> sharedGate = new(true);　　　//　BindToOnClick にて利用する


    public void SetUp() {
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(2))
            .Subscribe(_ => ClosePopup())
            .AddTo(gameObject);

        // ギャラリー用キャラアイコンのボタン生成
        CreateGalleryIcons();

        canvasGroup.alpha = 0;

        // ポップアップ表示
        OpenPopup();
    }

    /// <summary>
    /// ポップアップを表示する
    /// </summary>
    public void OpenPopup() {
        gameObject.SetActive(true);
        AnimePopup(1.0f);
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void ClosePopup() {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(btnClose.transform.DOScale(Vector3.one * 0.8f, 0.15f).SetEase(Ease.InOutQuart)).SetLink(gameObject);
        sequence.Append(btnClose.transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.Linear)).SetLink(gameObject)
            .OnComplete(() => AnimePopup(0f));

        //SoundManager.instance.PlaySE(SE_TYPE.Cancel);        
    }

    /// <summary>
    /// ポップアップをアニメさせる
    /// </summary>
    /// <param name="alpha"></param>
    private void AnimePopup(float alpha) {
        canvasGroup.DOFade(alpha, 0.5f).SetEase(Ease.Linear)  // .SetLoops(-1, LoopType.Yoyo)
            .OnComplete(() => {
                canvasGroup.blocksRaycasts = alpha == 0 ? false : true;

                if (alpha == 0) {
                    gameObject.SetActive(false);
                }
            }).SetLink(gameObject);
    }

    /// <summary>
    /// ギャラリー用キャラアイコンのボタン生成
    /// </summary>
    private void CreateGalleryIcons() {
        //int index = 0;
        //for (int i = 0; i < UserData.instance.GetStageCount(); i++) {
        //    for (int j = 0; j < frameSprites.Length; j++) {
        //        GalleryIconDetail galleryIcon = Instantiate(galleryIconPrefab, galleryIconTrans[index], false);
        //        index++;

        //        Sprite charaSprite = j == 0 ? UserData.instance.GetStageData(i).normalCharaSprite : UserData.instance.GetStageData(i).rareCharaSprite;
        //        galleryIcon.SetUp(charaSprite, frameSprites[j], zoomTran.position);

        //        galleryIcon.GetButton().BindToOnClick(sharedGate, _ => {
        //            // ズーム中なら
        //            if (galleryIcon.IsZoomIn) {
        //                // 元に位置に戻す
        //                galleryIcon.ZoomOutGalleryIcon();
        //            } else {
        //                // ズーム
        //                galleryIcon.ZoomInGalleryIcon();
        //            }

        //            // 1秒間押せないボタン
        //            return Observable.Timer(System.TimeSpan.FromSeconds(0.75f)).AsUnitObservable();
        //        });
        //        galleryIconList.Add(galleryIcon);
        //    }
        //}
        sharedGate.AddTo(gameObject);
    }
}