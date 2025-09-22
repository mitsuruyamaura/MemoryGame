using UnityEngine;
using DG.Tweening;

public class CurseEffect : MonoBehaviour
{
    private float defaultScale;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void PlayEffect() {
        // 設定値用意
        defaultScale = transform.localScale.x;
        Vector3 scale = new(transform.localScale.x, 0, transform.localScale.z);
        transform.localScale = scale;
        spriteRenderer.color = new(1, 1, 1, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);

        // 地面からせりあがってきて、また地面に戻るアニメ
        sequence.Append(transform.DOScaleY(defaultScale, 1.0f).SetEase(Ease.OutBack));
        sequence.Join(spriteRenderer.DOFade(1.0f, 1.0f).SetEase(Ease.Linear));
        sequence.AppendInterval(0.5f);
        sequence.Append(transform.DOScaleY(0, 1.0f).SetEase(Ease.InBack));
        sequence.Join(spriteRenderer.DOFade(0, 1.0f).SetEase(Ease.Linear));

        Destroy(gameObject, 2.5f);
    }
}