using Cysharp.Threading.Tasks;
using System.Threading;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBase : MonoBehaviour
{
    [SerializeField]
    protected CanvasGroup canvasGroup;

    [SerializeField]
    protected Button btnClose;

    protected CancellationToken token;


    public virtual void InitializePopUp() {
        // 閉じるボタンの設定
        btnClose.OnClickAsObservable()
            .ThrottleFirst(System.TimeSpan.FromSeconds(0.5f))
            .Subscribe(async _ => await ClosePopUpAsync(token))
            .AddTo(this);
    }


    public virtual async UniTask OpenPopUpAsync(CancellationToken token) {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        await UniTask.Yield(token);
    }

    public virtual async UniTask ClosePopUpAsync(CancellationToken token) {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        await UniTask.Yield(token);
    }
}