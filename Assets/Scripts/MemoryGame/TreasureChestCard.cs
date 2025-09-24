using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class TreasureChestCard : CardModelBase {
    public TreasureChestCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("TreasureChest");

        if(cardData.masterData is ItemData itemData) {
            // 宝箱のアニメ
            ItemInfoDisplayManager.instance.ShowTreasureBoxIcon(itemData.rarity);
            await UniTask.Delay(300);

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
            PlayerInventoryManager.instance.AddItemDataConvertBackPackInItem(itemData, false);
        }        

        await UniTask.Yield(token);
    }
}