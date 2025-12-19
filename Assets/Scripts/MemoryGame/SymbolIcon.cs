using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 罠解除用のシンボルアイコン表示と入力処理用クラス
/// </summary>
public class SymbolIcon : MonoBehaviour {
    [SerializeField] private Image imgFrame;
    [SerializeField] private Image imgIcon;
    [SerializeField] private Button btnIcon;

    [SerializeField] private Sprite spriteA;    // 赤色
    [SerializeField] private Sprite spriteB;    // 青色
    [SerializeField] private Sprite spriteC;    // 緑色
    [SerializeField] private Sprite spriteD;    // 黄色

    [SerializeField] private Sprite frameInput;    // プレイヤー入力用
    [SerializeField] private Sprite frameCorrect;  // 正解表示用

    private IDisposable disposable;

    private readonly float posAoffset = -4.9f;
    private readonly float posBoffset = 4.5f;
    private readonly float posCoffset = 2.8f;

    public void Set(QTESymbol symbol, UnityAction<QTESymbol> clickAction = null) {
        imgIcon.sprite = symbol switch {
            QTESymbol.A => spriteA,
            QTESymbol.B => spriteB,
            QTESymbol.C => spriteC,
            QTESymbol.D => spriteD,
            _ => null
        };

        float xOffset = symbol switch {
            QTESymbol.A => posAoffset,
            QTESymbol.B => posBoffset,
            QTESymbol.C => posCoffset,
            QTESymbol.D => 0f,
            _ => 0f
        };

        imgIcon.rectTransform.anchoredPosition = new Vector2(xOffset, imgIcon.rectTransform.anchoredPosition.y);

        if (clickAction != null) {
            // ボタン利用(プレイヤー入力用)
            imgFrame.sprite = frameInput;
            disposable = btnIcon.OnClickExt(() => clickAction?.Invoke(symbol), this, TimeSpan.FromSeconds(0.1f));
        } else {
            // ボタン非利用(正解表示用)
            imgFrame.sprite = frameCorrect;

            // 半透明にしてボタン無効化
            imgFrame.color = new(1, 1, 1, 0.35f);
            btnIcon.enabled = false;
        }
    }

    /// <summary>
    /// 正解したときの表示に変更する(半透明→通常)
    /// </summary>
    public void SetSuccessCorrectSymbol() {
        imgFrame.color = Color.white;
    }

    /// <summary>
    /// 失敗したときの表示に変更する(通常→半透明)
    /// </summary>
    public void SetFailureCorrectSymbol() {
        imgFrame.color = new(1, 1, 1, 0.35f);
    }

    /// <summary>
    /// btnIcon.OnClickExt の中で購読解除しているので、ここでは不要
    /// </summary>
    //private void OnDestroy() {
    //    disposable?.Dispose();
    //}
}