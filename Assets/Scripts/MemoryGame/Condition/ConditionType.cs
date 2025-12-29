/// <summary>
/// コンディションの種類
/// </summary>
public enum ConditionType {
    None,
    Poison,           // 猛毒。お手付きするとダメージ
    Hallucination,    // 幻覚。集中力が回復しなくなる(フロア移動時、メモリアの断片獲得時、祝福による効果時など)
    Distraction,      // 散漫。お手付きすると集中力が減る
    Seal,             // 封印。ソウルポイントが入手できなくなる(バトル後、アイテム破棄時、祝福による効果時など)
    Curse,            // 呪詛。アイテムが操作できない(破棄できない、一時停止できない)、アイテムが入手できない
    Fortitude,        // 不屈。現在効果中のデバフをすべて解除。すべてのデバフを無効にする

    Exhausted,        // ここからは未実装で使うかも未定
    Freeze,
    Weakened
}
