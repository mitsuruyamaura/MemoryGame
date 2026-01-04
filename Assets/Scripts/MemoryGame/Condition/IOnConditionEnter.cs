/// <summary>
/// コンディション付与時に追加効果を発動したい場合に実装する
/// </summary>
public interface IOnConditionEnter {
    void OnEnter(ConditionProgressData data);
}