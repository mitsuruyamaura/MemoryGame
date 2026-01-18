using NUnit.Framework.Interfaces;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoView : InfoViewBase {
    [SerializeField] private Text[] txtDescs;

    /// <summary>
    /// バックパック内のアイテム情報表示
    /// </summary>
    /// <param name="backPackInItem"></param>
    public void ShowBackPackItemInfo(BackPackInItem backPackInItem) {
        isReleased = false;

        if (backPackInItem.EnhanceLevel.Value > 0) {
            txtName.text = $"{backPackInItem.itemData.itemName} +{backPackInItem.EnhanceLevel.Value}";
        } else {
            txtName.text = $"{backPackInItem.itemData.itemName}";
        }

        // レアリティアイコンの表示
        for (int i = 0; i < imgRarityIcons.Length; i++) {
            imgRarityIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < (int)backPackInItem.itemData.rarity + 1; i++) {
            imgRarityIcons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < txtDescs.Length; i++) {
            txtDescs[i].text = string.Empty;
        }

        //txtDescs[0].text = $"ItemType : {backPackInItem.itemData.itemType}\n";

        if (backPackInItem.itemData.effectType != EffectType.Passive) {
            StringBuilder sb = new();

            AppendIfNotZero(sb, "クールタイム", backPackInItem.itemData.coolTime, "F2", "s");
            AppendIfNotZero(sb, "命中率", backPackInItem.itemData.accuracy, "F1");
            AppendIfNotZero(sb, "クリティカル率", backPackInItem.itemData.criticalRate, "F1");
            AppendIfNotZero(sb, "クリティカルダメージ率", backPackInItem.itemData.criticalDamageRate, "F1");

            txtDescs[0].text += sb.ToString();
        } else {
            StringBuilder sb = new();

            AppendIfNotZero(sb, "受流し 成功率", backPackInItem.itemData.parryRate);
            AppendIfNotZero(sb, "ダメージ吸収 成功率", backPackInItem.itemData.absorptionRate);
            AppendIfNotZero(sb, "ダメージ反射 成功率", backPackInItem.itemData.reflectionRate);
            AppendIfNotZero(sb, "交渉 成功率", backPackInItem.itemData.settlementRate);
            AppendIfNotZero(sb, "治癒力", backPackInItem.itemData.recoveryPower, suffix: "");

            AppendIfNotZero(sb, "幻覚 抵抗率", backPackInItem.itemData.hallucinationResist);
            AppendIfNotZero(sb, "猛毒 抵抗率", backPackInItem.itemData.poisonResist);
            AppendIfNotZero(sb, "散漫 抵抗率", backPackInItem.itemData.distractionResist);
            AppendIfNotZero(sb, "封印 抵抗率", backPackInItem.itemData.sealResist);
            AppendIfNotZero(sb, "呪い 抵抗率", backPackInItem.itemData.curseResist);

            txtDescs[0].text += sb.ToString();
        }

        string valueName = backPackInItem.itemData.effectType switch {
            EffectType.Physical => "ダメージ値",  // Damage
            EffectType.Magic => "ダメージ値",
            EffectType.Passive => "最大 Hp アップ値",  // MaxHp
            EffectType.Shield => "シールド値",  // Shield Power
            EffectType.Heal => "回復値",  // Heal Power
            _ => ""
        };

        if (backPackInItem.itemData.effectType != EffectType.Passive) {
            txtDescs[1].text = "行動回数 : " + backPackInItem.itemData.minAttackCount.ToString() + " - " + backPackInItem.itemData.maxAttackCount.ToString() + "\n";  // ActionCount
            txtDescs[1].text += valueName + " : " + backPackInItem.itemData.minValue.ToString() + " - " + backPackInItem.itemData.maxValue.ToString() + "\n";
        }

        string effectTypeName = backPackInItem.itemData.effectType switch {
            EffectType.Physical => "物理",
            EffectType.Magic => "魔法",
            EffectType.Passive => "パッシブ",
            EffectType.Shield => "シールド",
            EffectType.Heal => "回復",
            _ => ""
        };
        txtDescs[1].text += "EffectType : " + effectTypeName;  // backPackInItem.itemData.effectType.ToString();

        txtDescs[2].text = $"ItemType : {backPackInItem.itemData.itemType}\n\n";

        // Passive のみ HpBonus 表示
        if (backPackInItem.itemData.effectType == EffectType.Passive) {
            txtDescs[2].text += valueName + " + " + backPackInItem.itemData.hpBonus.ToString() + "\n";
        }

        // 各ステータス値
        //for (int i = 0; i < backPackInItem.itemData.statusTypes.Length; i++) {
        //    txtDescs[2].text += backPackInItem.itemData.statusTypes[i].ToString() + " + " + backPackInItem.itemData.requiredValues[i].ToString() + "\n";
        //}

        //txtDescs[3].text = "Rarity : " + backPackInItem.itemData.rarity.ToString();
        //txtDescs[0].text += backPackInItem.ItemData.Value.description;

        txtDescs[4].text = backPackInItem.itemData.price.ToString("N0");

        // アイコン画像の設定
        Sprite itemIcon = DataBaseManager.instance.GetItemIcon(backPackInItem.itemData.id);
        if (itemIcon != null) {
            imgMain.sprite = itemIcon;
        }

        cg.alpha = 1;
    }

    /// <summary>
    /// 宝箱から獲得したアイテムの情報表示
    /// </summary>
    /// <param name="itemData"></param>
    public void ShowTeasureChestInfo(ItemData itemData) {
        isReleased = false;

        // 表示内容の設定
        txtName.text = itemData.itemName;

        // レアリティアイコンの表示
        for (int i = 0; i < imgRarityIcons.Length; i++) {
            imgRarityIcons[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < (int)itemData.rarity + 1; i++) {
            imgRarityIcons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < txtDescs.Length; i++) {
            txtDescs[i].text = string.Empty;
        }

        //txtDescs[0].text = $"ItemType : {itemData.itemType}\n";

        if (itemData.effectType != EffectType.Passive) {
            StringBuilder sb = new();

            AppendIfNotZero(sb, "クールタイム", itemData.coolTime, "F2", "s");
            AppendIfNotZero(sb, "命中率", itemData.accuracy, "F1");
            AppendIfNotZero(sb, "クリティカル率", itemData.criticalRate, "F1");
            AppendIfNotZero(sb, "クリティカルダメージ率", itemData.criticalDamageRate, "F1");

            txtDescs[0].text += sb.ToString();
        } else {
            StringBuilder sb = new();

            AppendIfNotZero(sb, "受流し 成功率", itemData.parryRate);
            AppendIfNotZero(sb, "ダメージ吸収 成功率", itemData.absorptionRate);
            AppendIfNotZero(sb, "ダメージ反射 成功率", itemData.reflectionRate);
            AppendIfNotZero(sb, "交渉 成功率", itemData.settlementRate);
            AppendIfNotZero(sb, "治癒力", itemData.recoveryPower, suffix: "");

            AppendIfNotZero(sb, "幻覚 抵抗率", itemData.hallucinationResist);
            AppendIfNotZero(sb, "猛毒 抵抗率", itemData.poisonResist);
            AppendIfNotZero(sb, "散漫 抵抗率", itemData.distractionResist);
            AppendIfNotZero(sb, "封印 抵抗率", itemData.sealResist);
            AppendIfNotZero(sb, "呪い 抵抗率", itemData.curseResist);

            txtDescs[0].text += sb.ToString();
        }

        string valueName = itemData.effectType switch {
            EffectType.Physical => "ダメージ値",  // Damage
            EffectType.Magic => "ダメージ値",
            EffectType.Passive => "最大 Hp アップ値",  // MaxHp
            EffectType.Shield => "シールド値",  // Shield Power
            EffectType.Heal => "回復値",  // Heal Power
            _ => ""
        };

        if (itemData.effectType != EffectType.Passive) {
            txtDescs[1].text = "行動回数 : " + itemData.minAttackCount.ToString() + " - " + itemData.maxAttackCount.ToString() + "\n";  // ActionCount
            txtDescs[1].text += valueName + " : " + itemData.minValue.ToString() + " - " + itemData.maxValue.ToString() + "\n";
        }

        string effectTypeName = itemData.effectType switch {
            EffectType.Physical => "物理",
            EffectType.Magic => "魔法",
            EffectType.Passive => "パッシブ",
            EffectType.Shield => "シールド",
            EffectType.Heal => "回復",
            _ => ""
        };
        txtDescs[1].text += "EffectType : " + effectTypeName;  // itemData.effectType.ToString();

        txtDescs[2].text = $"ItemType : {itemData.itemType}\n\n";

        // Passive のみ HpBonus 表示
        if (itemData.effectType == EffectType.Passive) {
            txtDescs[2].text += valueName + " + " + itemData.hpBonus.ToString() + "\n";
        }

        // 各ステータス値
        //for (int i = 0; i < itemData.statusTypes.Length; i++) {
        //    txtDescs[2].text += itemData.statusTypes[i].ToString() + " + " + itemData.requiredValues[i].ToString() + "\n";
        //}

        //txtDescs[3].text = "Rarity : " + itemData.rarity.ToString();
        //txtDescs[0].text += backPackInItem.ItemData.Value.description;

        txtDescs[4].text = itemData.price.ToString("N0");

        // アイコン画像の設定
        Sprite itemIcon = DataBaseManager.instance.GetItemIcon(itemData.id);
        if (itemIcon != null) {
            imgMain.sprite = itemIcon;
        }

        cg.alpha = 1;
    }

    /// <summary>
    /// 0 以外の値をラベルを付けて表示
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="label"></param>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <param name="suffix"></param>
    private void AppendIfNotZero(StringBuilder sb, string label, float value, string format = "F2", string suffix = " %") {
        if (Mathf.Approximately(value, 0f)) {
            return; 
        }

        sb.AppendLine($"{label} : {value.ToString(format)}{suffix}");
    }
}