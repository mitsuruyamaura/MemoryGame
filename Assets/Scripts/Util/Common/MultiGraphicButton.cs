using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiGraphicButton : Button {
    [SerializeField] public Graphic[] extraGraphics;
    [SerializeField] GameObject objaaa;

    protected override void DoStateTransition(SelectionState state, bool instant) {
        base.DoStateTransition(state, instant);

        Color tintColor;
        switch (state) {
            case SelectionState.Normal:
                tintColor = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                tintColor = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                tintColor = colors.pressedColor;
                break;
            case SelectionState.Disabled:
                tintColor = colors.disabledColor;
                break;
            default:
                tintColor = Color.white;
                break;
        }

        if (extraGraphics != null) {
            foreach (var g in extraGraphics) {
                if (g != null)
                    g.CrossFadeColor(tintColor, instant ? 0f : colors.fadeDuration, true, true);
            }
        }
    }
}