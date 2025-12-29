/// <summary>
/// ダメージ計算方法の種類
/// </summary>
public enum TrapDamageType {
    None,
    MaxHpRate,          // 最大HPに対する割合ダメージ
    SetRemainingHpRate, // 残りHPを◯%にする(HPを指定割合まで削る)
}