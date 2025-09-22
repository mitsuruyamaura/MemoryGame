using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FloatingMessageControler : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private Text txtMessage;

    /// <summary>
    /// フロート表示の設定
    /// </summary>
    /// <param name="damage"></param>
    public void SetUpFloatingMessage(int damage) {

        txtMessage.text = damage.ToString();

        transform.localPosition = new Vector3(transform.localPosition.x + Random.Range(-0.5f, 0.5f), transform.localPosition.y + Random.Range(-0.5f, 0.5f), transform.localPosition.z);

        transform.SetParent(EffectManager.instance.effectConteinerTran);

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(transform.DOShakeScale(0.25f));
        sequence.Append(canvasGroup.DOFade(0, 0.25f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); }));

    }
}