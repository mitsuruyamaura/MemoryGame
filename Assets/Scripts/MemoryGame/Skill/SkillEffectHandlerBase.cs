using System.Collections.Generic;

/// <summary>
/// SkillEffectHandler の共通基底クラス
/// </summary>
public abstract class SkillEffectHandlerBase {
    public abstract SkillEffectType EffectType { get; }

    public void ApplyEffect(ITarget target, float value) {
        foreach (var t in GetTargets(target)) {
            ApplyToTarget(t, value);
        }
    }

    public void RemoveEffect(ITarget target, float value) {
        foreach (var t in GetTargets(target)) {
            RemoveFromTarget(t, value);
        }
    }

    /// <summary>
    /// 効果を適用する実際の処理(個別ハンドラーで実装)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    protected abstract void ApplyToTarget(ITarget target, float value);

    /// <summary>
    /// 効果を解除する処理(任意)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    protected virtual void RemoveFromTarget(ITarget target, float value) { }

    /// <summary>
    /// 必要に応じてターゲット選択を拡張(単体やグループなど)
    /// </summary>
    /// <param name="origin"></param>
    /// <returns></returns>
    protected virtual IEnumerable<ITarget> GetTargets(ITarget origin) {
        yield return origin; // デフォルトは単一
    }
}