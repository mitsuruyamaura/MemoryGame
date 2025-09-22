/// <summary>
/// 装備品に付与される一時的な効果
/// </summary>
public enum EnhanceType {
    AttackUp,        // 
    AttackDown,      // 攻撃ダウン    ダメージ n%減
    ShieldDisable,   // シールド不能  すべてのシールドの効果が無効になる(アイテム使ってもシールドが張れない。耐久値は減る)
    HpAbsorb,        // HP吸収        与えたダメージのn％を回復できる
    IgnoreShield,    // シールド無視  Magic 扱いで攻撃できる
    Stun,            // スタン        バトル終了まで処理停止
    SpeedDown,       // 速度ダウン    CoolTime の回復速度 n%減
    Forget,          // 忘却
    HealDisable,     // 回復不能      すべてのヒールの効果が無効になる(アイテム使っても回復しない。耐久値は減る)
    Freeze,          // 凍結          アイテムが停止
    Daze,            // 幻惑          命中率 n%減
    Confusion,       // 混乱          n% の確率でアイテムを使わないときがある(耐久値は減る)
    MagicRegist,     // 魔法耐性      Magic 攻撃のダメージを n%カット
    PhysicRegist,    // 物理耐性      Physical 攻撃のダメージを n%カット
}
