using System;
using System.Collections.Generic;
using System.Linq;

public enum Rarity {
    Common,
    Uncommon,
    Rare,
    Mystic,
    Epic,
    Legendary,
    None
}

public enum ItemType {
    Sword,
    Axe,
    Bow,
    Ring,
    Spear,
    Dagger,
    Armor,
    Shield,
    Potion,
    Book,
    Shoes,
    Helm,
    Hat,
    Food,
    Gauntlet,
    Greave,
    GreatSword,
    Hammer,
    Flail,
    Mace,
    Boots,
    Crossbow,
    Wand,
    Gem,
    Brooch,
    Coin,
    Amulet,
    Bracelet,
    Necklace,
    Crown,

}


public enum EffectType {
    Physical,   // 物理系の攻撃
    Magic,      // 魔法系の攻撃
    Passive,    // 装備しただけで得られるパッシブ効果
    Shield,
    Heal,

}

public enum StatusType {
    Strength,       // 力、体力、筋力
    Intelligence,   // 知性、知力
    Dexterity,      // 器用さ、素早さ
    Charm,          // 魅力
    Luck            // 運の良さ
}

public enum BuffDebuffType {
    // バフ (Beneficial Effects)
    // デバフ (Harmful Effects)
    なし,
    クリティカル,   // ダメージ増加　CriticalDamageIncrease
    攻撃ダウン,     // 攻撃力(ダメージ) -20％位  DamageDown
    防御ダウン,     // 防御力 -20％位　DefenseDown
    HP吸収,         // ダメージの20％位　HpAbsorption    
    防御無視,       // シールド破壊　DefenseIgnore
    スタン,         // 攻撃停止。マヒ　Stun
    幻惑,           // 命中力へのデバフ。1スタック当たり、-30%位　Confusion
    猛毒,           // 一定時間ごとにダメージ　Poison
    即死,           // Hp 無視で倒す　InstantDeath
    速度ダウン,     // クールタイム延長。1スタック当たり ⁺30秒　Slow
    呪い,           // 一定確率で行動不能(攻撃前にチェックして、その効果を無視する。複数回攻撃する前に行う)　Curse
    忘却,           // クリティカル発生率 0％　Forget
    魔法耐性,
}

[System.Serializable]
public class ItemData : IMasterData {
    public int id;
    public string itemName;
    public Rarity rarity;
    public ItemType itemType;
    public string description;
    public int price;               // 高いほど効果に売れる。クリア時のボーナスになる

    public float coolTime;          // アイテム使用までの準備時間。短い方が早くアイテムを使う
    public float accuracy;          // 命中力。0-100。高い方が命中しやすい。
    public int minValue;            // 効果の最小値。ダメージ、シールド、回復
    public int maxValue;            // 効果の最大値

    public int minAttackCount;      // 1回当たりの攻撃回数
    public int maxAttackCount;

    public EffectType effectType;

    // コストは能力値とセットにする STR 5、のように。AP 消費の概念はなくす
    public StatusType[] statusTypes;       // 必要な能力値の種類
    public int[] requiredValues;
    public float criticalRate;
    public BuffDebuffType buffDebuffType;  // 複数セットでもいい クリティカル ⁺ 攻撃ダウン、など

    public string effect;
    public int hpBonus;
    public int durability;                 // 耐久値。バトルごとに減少
    public EquipmentType equipmentType;

    public float reflectionRate;           // リアクション性能。ダメージ半分反射
    public float parryRate;                // ダメージ 0 受け流し
    public float absorptionRate;           // ダメージ半分吸収
    public float settlementRate;           // 交渉

    public float criticalDamageRate;       // クリティカルの倍率ボーナス

    public int Id => id;

    // サイズ(ウェイト)


    public ItemData() { }


    /// <summary>
    /// ディープコピー用のコンストラクタ
    /// </summary>
    /// <param name="otherItemData"></param>
    public ItemData (ItemData otherItemData) {
        id = otherItemData.id;
        itemName = otherItemData.itemName;
        rarity = otherItemData.rarity;
        itemType = otherItemData.itemType;
        description = otherItemData.description;
        price = otherItemData.price;

        coolTime = otherItemData.coolTime;
        accuracy = otherItemData.accuracy;
        minValue = otherItemData.minValue;
        maxValue = otherItemData.maxValue;

        minAttackCount = otherItemData.minAttackCount;
        maxAttackCount = otherItemData.maxAttackCount;
        effectType = otherItemData.effectType;

        statusTypes = otherItemData.statusTypes != null ? (StatusType[])otherItemData.statusTypes.Clone() : null;
        requiredValues = otherItemData.requiredValues != null ? (int[])otherItemData.requiredValues.Clone() : null;
        criticalRate = otherItemData.criticalRate;
        buffDebuffType = otherItemData.buffDebuffType;

        effect = otherItemData.effect;
        hpBonus = otherItemData.hpBonus;
        durability = otherItemData.durability;
        equipmentType = otherItemData.equipmentType;

        reflectionRate = otherItemData.reflectionRate;
        parryRate = otherItemData.parryRate;
        absorptionRate = otherItemData.absorptionRate;
        settlementRate = otherItemData.settlementRate;

        criticalDamageRate = otherItemData.criticalDamageRate;
    }

    public ItemData(string[] datas) {
        id = int.Parse(datas[0]);
        itemName = datas[1];
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[2]);
        itemType = (ItemType)Enum.Parse(typeof(ItemType), datas[3]);
        description = datas[4];
        price = int.Parse(datas[5]);

        coolTime = float.Parse(datas[6]);
        accuracy = float.Parse(datas[7]);
        minValue = int.Parse(datas[8]);
        maxValue = int.Parse(datas[9]);

        minAttackCount = int.Parse(datas[10]);
        maxAttackCount = int.Parse(datas[11]);
        effectType = (EffectType)Enum.Parse(typeof(EffectType), datas[12]);

        // 半角スラッシュで区切って配列に変換
        statusTypes = datas[13].Split('/').Select(type => (StatusType)Enum.Parse(typeof(StatusType), type)).ToArray();
        requiredValues = datas[14].Split('/').Select(int.Parse).ToArray();
        criticalRate = float.Parse(datas[15]);
        buffDebuffType = (BuffDebuffType)Enum.Parse(typeof(BuffDebuffType), datas[16]);

        effect = datas.Length > 17 ? datas[17] : string.Empty;
        hpBonus = int.Parse(datas[18]);
        durability = int.Parse(datas[19]);
        equipmentType = (EquipmentType)Enum.Parse(typeof(EquipmentType), datas[20]);

        reflectionRate = float.Parse(datas[21]);
        parryRate = float.Parse(datas[22]);
        absorptionRate = float.Parse(datas[23]);
        settlementRate = float.Parse(datas[24]);

        criticalDamageRate = float.Parse(datas[25]);
    }


    public static ItemData Calculate(ItemData baseItemData, List<BuffDebuffType> buffDebuffTypeList) {
        ItemData totalItemData = baseItemData;


        return totalItemData;
    }
}