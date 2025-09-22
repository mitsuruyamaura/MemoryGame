using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : PoolBase {
    public Image bottomImage;     // 半透明の背景画像
    public Image topImage;        // ゲージ画像（フェードインする画像）
    public RectTransform icon;    // アイコンのTransform

    public float cooldownTime = 5f; // クールダウン時間
    public float scaleDuration = 0.2f; // スケールアニメーションの持続時間
    public Vector3 scaleAmount = new Vector3(1.2f, 1.2f, 1.2f); // スケール倍率

    private Tweener fadeTweener;  // Tween オブジェクトを管理する変数

    void Start() {
        // 初期状態で上の画像を透明に設定
        Color color = topImage.color;
        color.a = 0;
        topImage.color = color;

        StartCooldownAnimation();
    }

    void StartCooldownAnimation() {
        // クールダウンアニメーションを開始
        fadeTweener = topImage.DOFade(1, cooldownTime)
            .OnComplete(OnGaugeFull)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(gameObject); // オブジェクトの破壊に紐づく
    }

    void OnGaugeFull() {
        icon.DOScale(scaleAmount, scaleDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => icon.DOScale(Vector3.one, scaleDuration).SetEase(Ease.InBack));
    }

    public void PauseCooldownAnimation() {
        if (fadeTweener != null && fadeTweener.IsPlaying()) {
            fadeTweener.Pause();
        }
    }

    public void ResumeCooldownAnimation() {
        if (fadeTweener != null && !fadeTweener.IsPlaying()) {
            fadeTweener.Play();
        }
    }

    public void StopCooldownAnimation() {
        if (fadeTweener != null) {
            fadeTweener.Kill();
        }
    }

    private void OnDestroy() {
        // オブジェクトが破壊されたときに Tween を停止
        StopCooldownAnimation();
    }

    private void UseEffect() {
        DebugLogger.Log("アイテムの効果が発動しました！");
        // ここでアイテムの実際の効果を実装します
    }
}
