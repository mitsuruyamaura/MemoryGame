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
            .Subscribe(async _ => await ClosePopUp(token))
            .AddTo(this);
    }


    public virtual async UniTask OpenPopUp(CancellationToken token) {
        await UniTask.Yield(token);
    }

    public virtual async UniTask ClosePopUp(CancellationToken token) {
        await UniTask.Yield(token);
    }
}