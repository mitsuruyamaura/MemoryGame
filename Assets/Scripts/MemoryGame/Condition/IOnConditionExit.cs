/// <summary>
/// コンディション削除時に追加効果を発動したい場合に実装する
/// </summary>
public interface IOnConditionExit {
    void OnExit(ConditionProgressData data);
}