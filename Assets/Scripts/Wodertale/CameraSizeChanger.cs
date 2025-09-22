using UnityEngine.UI;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CameraSizeChanger : MonoBehaviour
{
    // カメラのサイズ変更関連
    [SerializeField] private SpriteMask defaultMask;
    [SerializeField] private SpriteMask expandMask;
    [SerializeField] private Image defaultBackFrame;
    [SerializeField] private Image expandBackFrame;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float targetOrthographicSize;
    [SerializeField] private float defaultOrthographicSize;


    /// <summary>
    /// カメラのサイズを広くする
    /// </summary>
    /// <returns></returns>
    public async UniTask ExpandViewSize() {
        defaultMask.enabled = false;
        expandMask.enabled = true;

        defaultBackFrame.enabled = false;
        expandBackFrame.enabled = true;

        // マスクのスケールを操作して、視界のサイズを変更
        //Tweener tweener = virtualCamera.m_Lens.OrthographicSize = 5;  spriteMaskTran.DOScale(Vector3.one * conditionValue, viewAnimeDuration).SetEase(Ease.InBack).SetLink(SymbolManager.instance.gameObject);

        Tweener tweener = DOTween.To(
            () => virtualCamera.Lens.OrthographicSize,
            size => virtualCamera.Lens.OrthographicSize = size,
            targetOrthographicSize,  // 目標のオーソグラフサイズ
            0.5f         // アニメーションの所要時間
         ).SetEase(Ease.OutBack).SetLink(gameObject);

        await tweener.AsyncWaitForCompletion();
        tweener = null;
    }


    // TODO 狭くする

    /// <summary>
    /// 元のサイズに戻す
    /// </summary>
    /// <returns></returns>

    public async UniTask DefaultViewSize() {
        defaultMask.enabled = true;
        expandMask.enabled = false;

        defaultBackFrame.enabled = true;
        expandBackFrame.enabled = false;

        Tweener tweener = DOTween.To(
            () => virtualCamera.Lens.OrthographicSize,
            size => virtualCamera.Lens.OrthographicSize = size,
            defaultOrthographicSize,  // 目標のオーソグラフサイズ
            0.5f         // アニメーションの所要時間
         ).SetEase(Ease.InBack).SetLink(gameObject);

        await tweener.AsyncWaitForCompletion();
        tweener = null;
    }
}
