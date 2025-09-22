using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BossEffect : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroupMain;

    [SerializeField]
    private Image imgLogo;

    /// <summary>
    /// ボス出現エフェクトの再生
    /// 未使用
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayEffect() {

        canvasGroupMain.alpha = 0;

        imgLogo.color = new Color(1.0f, 1.0f, 1.0f, 0);

        Sequence sequence = DOTween.Sequence();

        sequence.SetLink(gameObject);
        sequence.Append(canvasGroupMain.DOFade(1.0f, 0.75f));
        Tween tween = sequence.Append(imgLogo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo));
        sequence.Append(canvasGroupMain.DOFade(0f, 0.75f)).OnComplete(() => { tween.Kill(); });

        yield return new WaitForSeconds(5.0f);

        Destroy(gameObject);
    }


    public async UniTask PlayEffectAsync(CancellationToken token) {
        canvasGroupMain.alpha = 0;

        imgLogo.color = new Color(1.0f, 1.0f, 1.0f, 0);

        Sequence sequence = DOTween.Sequence();

        // Sequence は処理が終了すると自動的に解放されるため、Tween.Kill は不要
        //Tween tween = sequence;

        // SetLink は Append ごとにつける必要はない
        sequence.SetLink(gameObject);
        sequence.Append(canvasGroupMain.DOFade(1.0f, 0.75f));
        sequence.Append(imgLogo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear).SetLoops(3, LoopType.Yoyo));
        sequence.Append(canvasGroupMain.DOFade(0f, 0.75f));
        
        // キャンセルの下に書いてしまうと、万が一キャンセルされたときに破棄されないため、先に時間を設定して処理しておく 
        Destroy(gameObject, 5.0f);
        await UniTask.Delay(5000, cancellationToken: token);

        //try {
        //    await UniTask.Delay(5000, cancellationToken: token);
        //} catch {
        //    Debug.Log("Cancel");
        //} finally {
        //    Destroy(gameObject);
        //}
    }
}