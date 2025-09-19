using TMPro;
using DG.Tweening;

public static class TextMeshProExtensions {

    /// <summary>
    /// TextMeshPro 用の DOCounter メソッド
    /// </summary>
    /// <param name="text"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener DOCounter(this TMP_Text text, int from, int to, float duration) {
        int currentValue = from;
        return DOTween.To(() => currentValue, x => {
            currentValue = x;
            text.text = currentValue.ToString();
        }, to, duration);
    }

    /// <summary>
    /// 上記のオーバーロード
    /// </summary>
    /// <param name="text"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static Tweener DOCounter(this TextMeshProUGUI text, int from, int to, float duration) {
        int currentValue = from;
        return DOTween.To(() => currentValue, x => {
            currentValue = x;
            text.text = currentValue.ToString();
        }, to, duration);
    }
}