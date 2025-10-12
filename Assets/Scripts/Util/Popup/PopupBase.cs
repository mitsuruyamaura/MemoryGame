using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour {
    public PopupAnimSettings popupAnimSettings;

    public Image filter;
    public Transform bg;
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public Button btnClose;

    public bool isCloseFilter;// 背景を押したら画面を閉じる機能ON/OFF
    public bool isPopupAnime;// ポップアップを開く時閉じる時、アニメーションをする機能ON/OFF
    public bool isDestroy;
    public float filterAplha;
    public bool isNotOpenPopupSetSerialize;   // SetSerialize 後に呼ばない場合にチェックいれる

    [Header("閉じる際に非同期処理を利用する場合は true にする")]
    public bool useAsyncClosePopup;

    protected bool isClickable;
    protected Color filterBaseColor;
    protected UnityAction popupAction;
    protected IDisposable disposable;
    protected AsyncOperationHandle<Sprite> spriteHandle = default;  // Addressables の Sprite の読み込みに使用

    protected virtual void Awake() {
        //canvasGroup.alpha = 0f;
    }

    public virtual async UniTask SetInitializeAsync(object param = null, UnityAction popupAction = null){
        await UniTask.Yield();
        SetInitialize();
    }

    /// <summary>
    /// 外部からInstantiateと同時にSetInitialize()を呼ぶ流れにする。Start()は使わない予定
    /// </summary>
    public virtual void SetInitialize() {
        filterBaseColor = filter.color;
        if (isCloseFilter) {
            // フィルターをクリックで閉じれるようにする
            Button closeButton = filter.gameObject.AddComponent<Button>();
            closeButton.transition = Selectable.Transition.None;
            filter.gameObject.GetComponent<Button>().onClick.AddListener(() => ClosePopupProc(false));
        }
        if (filterAplha == 0f) {
            filterAplha = filter.color.a;
        }
        filter.color = new Color(filter.color.r, filter.color.g, filter.color.b, 0f);
        bg.localScale = Vector3.one * popupAnimSettings.startScale;

        canvas.worldCamera = Camera.main;

        // 閉じるボタンが表示されていて、かつ有効ならボタン設定
        if (btnClose != null && btnClose.gameObject.activeSelf && btnClose.enabled) {

            // 非同期処理を利用して閉じる場合
            if (useAsyncClosePopup) {
                disposable = btnClose.OnClickExtAsync(async () => await ClosePopupProcAsync(), this, TimeSpan.FromMilliseconds(2000));
            } else {
                // 通常
                disposable = btnClose.OnClickExt(() => ClosePopupProc(), this, TimeSpan.FromMilliseconds(2000));
            }                
        }

        if (isNotOpenPopupSetSerialize) {
            return;
        }

        // ポップアップウィンドウを表示する
        OpenPopupProc(false);
    }

    /// <summary>
    /// ポップアップウィンドウを表示する処理の分岐
    /// </summary>
    public virtual void OpenPopupProc(bool isSe = true) {
        if (isClickable) {
            return;
        }

        if (!isDestroy) {
            canvas.enabled = true;
        }

        if (isSe) {
            //AudioManager.instance.PlaySE(ENUM_SE.Submit);
        }

        // PopupAnimeFlgによってアニメーションするかしないかを分ける
        if (isPopupAnime) {
            OpenPopupAsync().Forget();
        } else {
            OpenPopupWithoutAnimation();
        }
        isClickable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// ポップアップウィンドウ表示（アニメ無し）
    /// </summary>
    protected void OpenPopupWithoutAnimation() {
        bg.localScale = Vector3.one;
        filter.color = filterBaseColor;
        canvasGroup.alpha = 1.0f;
    }

    /// <summary>
    /// ポップアップウィンドウ表示（アニメ有り）
    /// </summary>
    protected virtual async UniTask OpenPopupAsync() {
        filter.DOFade(filterAplha, popupAnimSettings.openSeconds);
        canvasGroup.DOFade(1, popupAnimSettings.closeSeconds);
        bg.DOScale(Vector3.one, popupAnimSettings.openSeconds).SetEase(popupAnimSettings.openEaseType);
        await UniTask.Delay(System.TimeSpan.FromSeconds(popupAnimSettings.openSeconds));
    }

    /// <summary>
    /// ポップアップウィンドウを閉じる処理の分岐
    /// </summary>
    public virtual void ClosePopupProc(bool isSe = true) {
        // PopupAnimeFlgによってアニメーションするかしないかを分ける
        if (isClickable) {
            isClickable = false;
            if(isSe) {
                //AudioManager.instance.PlaySE(ENUM_SE.POPUP_BACK);
            }
            if (isPopupAnime) {
                ClosePopupAsync().Forget();
            } else {
                ClosePopupWithoutAnimation();
            }
        }
    }

    /// <summary>
    /// ポップアップウィンドウを閉じる処理の分岐
    /// </summary>
    public virtual async UniTask ClosePopupProcAsync(bool isSe = true) {
        // PopupAnimeFlgによってアニメーションするかしないかを分ける
        if (isClickable) {
            isClickable = false;
            if (isSe) {
                //AudioManager.instance.PlaySE(ENUM_SE.POPUP_BACK);
            }

            canvasGroup.blocksRaycasts = false;
            
            if (isPopupAnime) {
                await ClosePopupAsync();
            } else {
                ClosePopupWithoutAnimation();
            }
        }
    }

    /// <summary>
    /// ポップアップウィンドウ閉じる（アニメ無し）
    /// </summary>
    protected virtual void ClosePopupWithoutAnimation() {
        bg.localScale = Vector3.zero;

        // DestroyFalgによって削除するかしないかを分ける
        DestroyOrDisabledPopup();
    }

    /// <summary>
    /// ポップアップウィンドウ閉じる（アニメ有り）
    /// </summary>
    public virtual async UniTask ClosePopupAsync() {
        filter.DOFade(0, popupAnimSettings.closeSeconds);
        bg.DOScale(Vector3.one * popupAnimSettings.closeEndScale, popupAnimSettings.closeSeconds).SetEase(popupAnimSettings.closeEaseType);
        await UniTask.Delay(System.TimeSpan.FromSeconds(popupAnimSettings.closeSeconds));

        // DestroyFalgによって削除するかしないかを分ける
        DestroyOrDisabledPopup();
    }

    /// <summary>
    /// ポップアップを破壊、あるいは非アクティブ状態にする
    /// </summary>
    public void DestroyOrDisabledPopup() {
        disposable?.Dispose();

        // 有効なハンドルかチェックしてから Release(このポップアップ内で Addressables を使っていないならスキップ)
        if (spriteHandle.IsValid()) {
            // Addressables のメモリ解放
            Addressables.Release(spriteHandle);

            // 解放後に無効化
            spriteHandle = default;
        }

        if (isDestroy) {
            Destroy(gameObject);
        } else {
            canvas.enabled = false;
        }
    }
}