using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System;


public class ItemInfoDisplayManager : AbstractSingleton<ItemInfoDisplayManager> {

    [SerializeField] private CanvasGroup itemInfoCanvas;
    [SerializeField] private Image imgItemIcon;
    [SerializeField] private Image[] imgRarityIcons;
    [SerializeField] private Text[] txtDescs;
    [SerializeField] private Text txtName;
    [SerializeField] private Transform playerItemTran;
    [SerializeField] private Transform enemyItemTran;
    [SerializeField] private Transform defaultItemTran;
    [SerializeField] private Transform itemInfoSet;
    [SerializeField] private Button btnFilter;
    [SerializeField] private CanvasGroup cgFilter;
    private IDisposable disposable;

    [SerializeField] private Image imgBagIcon;
    //[SerializeField] private Transform bagMoveEndTran;
    [SerializeField] private Ease bagEase;

    [SerializeField] private Sprite[] treasureBoxSprite;
    [SerializeField] private Image imgTreasureBoxIcon;
    [SerializeField] private Ease treasureBoxEase;

    public bool isTreasureShow;


    //void Start() {
    //    // 最初からインスペクターでアルファ 0 にしておく
    //    HideItemInfo();
    //}

    /// <summary>
    /// ホバーしたアイテムの情報表示
    /// </summary>
    /// <param name="backPackInItem"></param>
    public void ShowItemInfo(BackPackInItem backPackInItem) {
        // TODO この処理はなくても正常に動くので、一旦コメントアウト
        // アイテム情報表示中は、同じアイテム情報はマウスのホバーが終わるまで何もしない
        //if (itemInfoCanvas.alpha == 1.0f) {
        //    return;
        //}

        txtName.text = backPackInItem.itemData.itemName;

        //txtDesc.text = backPackInItem.currentCoolTime.ToString() + "\n";
        //txtDesc.text += backPackInItem.currentAccuracy.ToString() + "\n";

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
            imgItemIcon.sprite = itemIcon;
        }

        if (backPackInItem.entityType == EntityType.Player) {
            itemInfoSet.localPosition = playerItemTran.localPosition;
        } else {
            itemInfoSet.localPosition = enemyItemTran.localPosition;
        }

        if (itemInfoCanvas != null) {
            itemInfoCanvas.alpha = 1.0f;
        }
    }


    public void HideItemInfo() {
        if (itemInfoCanvas != null) {
            itemInfoCanvas.alpha = 0;
        }
    }

    /// <summary>
    /// 袋の表示
    /// </summary>
    public void ShowBagIcon() {
        imgBagIcon.gameObject.SetActive(true);

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(imgBagIcon.DOFade(1.0f, 0.25f).SetEase(Ease.InQuart));
        sequence.Join(imgBagIcon.transform.DOScale(Vector3.one, 0.25f).SetEase(bagEase));
        sequence.Join(imgBagIcon.transform.DOLocalJump(new(0, 0, 0), 25f, 2, 0.5f).SetEase(bagEase).SetRelative());
        sequence.AppendInterval(0.5f).OnComplete(() => {
            imgBagIcon.gameObject.SetActive(false);
            imgBagIcon.color = new(1, 1, 1, 0);
            imgBagIcon.transform.localPosition = Vector3.zero;
        });
    }

    /// <summary>
    /// 宝箱の表示
    /// </summary>
    /// <param name="rarity"></param>
    public void ShowTreasureBoxIcon(Rarity rarity) {
        int index = (int)rarity;

        Sprite sprite = treasureBoxSprite[index];
        imgTreasureBoxIcon.sprite = sprite;

        imgTreasureBoxIcon.gameObject.SetActive(true);
        imgTreasureBoxIcon.transform.localScale = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(imgTreasureBoxIcon.DOFade(1.0f, 0.25f).SetEase(Ease.InQuart));
        sequence.Join(imgTreasureBoxIcon.transform.DOScale(Vector3.one * 1.3f, 0.25f).SetEase(treasureBoxEase));
        sequence.Join(imgTreasureBoxIcon.transform.DOLocalJump(new(0, 0, 0), 200f, 1, 0.5f).SetEase(treasureBoxEase).SetRelative());
        sequence.Append(imgTreasureBoxIcon.transform.DOScale(Vector3.one, 0.25f).SetEase(treasureBoxEase));
        sequence.AppendInterval(0.5f).OnComplete(() => {
            imgTreasureBoxIcon.gameObject.SetActive(false);
            imgTreasureBoxIcon.color = new(1, 1, 1, 0);
            imgTreasureBoxIcon.transform.localPosition = Vector3.zero;
        });
    }

    /// <summary>
    /// 入手したアイテムの表示
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask ShowTreasureItemInfoAsync(ItemData itemData, CancellationToken token) {
        isTreasureShow = true;

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
            imgItemIcon.sprite = itemIcon;
        }
       
        itemInfoSet.localPosition = defaultItemTran.localPosition;

        await UniTask.Delay(1000);

        itemInfoCanvas.alpha = 1.0f;

        // 画面タップするまで待機(ほかの UI には触らないようにする)
        bool isTouch = false;
        cgFilter.blocksRaycasts = true;
        disposable = btnFilter.OnClickExt(() => isTouch = true, this);

        await UniTask.WaitUntil(() => isTouch == true, cancellationToken: token);

        disposable.Dispose();
        cgFilter.blocksRaycasts = false;

        isTreasureShow = false;
        HideItemInfo();
    }
}