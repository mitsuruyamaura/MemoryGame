using UnityEngine;
using UnityEngine.UI;

public class ItemInfoView : InfoViewBase {
    [SerializeField] private Text[] txtDescs;

    /// <summary>
    /// バックパック内のアイテム情報表示
    /// </summary>
    /// <param name="backPackInItem"></param>
    public void ShowBackPackItemInfo(BackPackInItem backPackInItem) {
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
            txtDescs[0].text += "クールタイム : " + backPackInItem.itemData.coolTime.ToString("F2") + " s\n";  // CoolTime
            txtDescs[0].text += "命中率 : " + backPackInItem.itemData.accuracy.ToString("F1") + " %\n";  // Accuracy
            txtDescs[0].text += "クリティカル率 : " + backPackInItem.itemData.criticalRate.ToString("F1") + " %\n";  // CriticalRate
        } else {
            txtDescs[0].text += $"パリィ 成功率 : {backPackInItem.itemData.parryRate.ToString("F2")} %\n";
            txtDescs[0].text += $"ダメージ吸収 成功率 : {backPackInItem.itemData.absorptionRate.ToString("F2")} %\n";
            txtDescs[0].text += $"ダメージ反射 成功率 : {backPackInItem.itemData.reflectionRate.ToString("F2")} %\n";
            txtDescs[0].text += $"交渉 成功率 : {backPackInItem.itemData.settlementRate.ToString("F2")} %\n";
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
        for (int i = 0; i < backPackInItem.itemData.statusTypes.Length; i++) {
            txtDescs[2].text += backPackInItem.itemData.statusTypes[i].ToString() + " + " + backPackInItem.itemData.requiredValues[i].ToString() + "\n";
        }

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
            txtDescs[0].text += "クールタイム : " + itemData.coolTime.ToString("F2") + " s\n";  // CoolTime
            txtDescs[0].text += "命中率 : " + itemData.accuracy.ToString("F1") + " %\n";  // Accuracy
            txtDescs[0].text += "クリティカル率 : " + itemData.criticalRate.ToString("F1") + " %\n";  // CriticalRate
        } else {
            txtDescs[0].text += $"パリィ 成功率 : {itemData.parryRate.ToString("F2")} %\n";
            txtDescs[0].text += $"ダメージ吸収 成功率 : {itemData.absorptionRate.ToString("F2")} %\n";
            txtDescs[0].text += $"ダメージ反射 成功率 : {itemData.reflectionRate.ToString("F2")} %\n";
            txtDescs[0].text += $"交渉 成功率 : {itemData.settlementRate.ToString("F2")} %\n";
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
        for (int i = 0; i < itemData.statusTypes.Length; i++) {
            txtDescs[2].text += itemData.statusTypes[i].ToString() + " + " + itemData.requiredValues[i].ToString() + "\n";
        }

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

    public void HideInfoView() {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}