using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class TreasureChestCard : CardModelBase {
    public bool isEnemyDrop;

    public TreasureChestCard(CardData cardData, bool isEnemyDrop) : base(cardData) {
        this.isEnemyDrop = isEnemyDrop;
    }

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("TreasureChest");

        if (cardData.masterData is ItemData itemData) {
            // 回収方法で演出分岐
            if (isEnemyDrop) {
                // 袋アイコン表示
                ItemInfoDisplayManager.instance.ShowBagIcon();
                SoundManager.instance.PlaySE(SE_TYPE.Drop);
            } else {
                // 宝箱のアニメ
                ItemInfoDisplayManager.instance.ShowTreasureBoxIcon(itemData.rarity);
                SoundManager.instance.PlaySE(SE_TYPE.OpenTreasure);
            }

            await UniTask.Delay(300, cancellationToken: token).SuppressCancellationThrow();

            // 入手したアイテムの情報表示
            await ItemInfoDisplayManager.instance.ShowTreasureItemInfoAsync(itemData, token);

            // アイテムを所持しているか判定
            bool haveItem = PlayerInventoryManager.instance.HaveTargetItem(itemData.id);

            // インベントリの最大サイズ以下か判定
            bool isInventoryUnderMaxSive = GameData.instance.IsInventoryUnderMaxSize();

            // 獲得アイテムを所持しておらず、かつ、インベントリが最大の場合
            if (!haveItem && !isInventoryUnderMaxSive) {
                BattleManager.instance.stageUIManager.InventoryMaxInfo();
                DebugLogger.Log("バッグがいっぱい");

                // 獲得できずに終了
                return;
            }

            // インベントリにアイテムを追加
            if (isEnemyDrop) {
                // 敵の場合には、1回分の強化を確定
                PlayerInventoryManager.instance.AddItemDataConvertBackPackInItem(itemData, true);
            } else {                
                PlayerInventoryManager.instance.AddItemDataConvertBackPackInItem(itemData, false);
            }
        }        

        await UniTask.Yield(token);
    }
}