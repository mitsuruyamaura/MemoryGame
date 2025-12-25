using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 宝箱・ドロップアイテム入手イベント実行クラス
/// </summary>
public class TreasureGetExecutor {
    private ItemInfoDisplayManager itemInfoDisplayManager;
    private PlayerInventoryManager playerInventoryManager;

    public TreasureGetExecutor(ItemInfoDisplayManager itemInfoDisplayManager, PlayerInventoryManager playerInventoryManager) {
        this.itemInfoDisplayManager = itemInfoDisplayManager;
        this.playerInventoryManager = playerInventoryManager;
    }

    public async UniTask ExecuteTreasureGetEventAsync(ItemData itemData, bool isEnemyDrop, CancellationToken token) {
        // 敵のドロップアイテムではなく宝箱カードから入手した場合
        if (isEnemyDrop) {
            GameData.instance.userData.FindTreasureCount.Value++;
        }

        // 回収方法で演出分岐
        if (isEnemyDrop) {
            // 袋アイコン表示
            itemInfoDisplayManager.ShowBagIcon();
            SoundManager.instance.PlaySE(SE_TYPE.Drop);
        } else {
            // 宝箱のアニメ
            itemInfoDisplayManager.ShowTreasureBoxIcon(itemData.rarity);
            SoundManager.instance.PlaySE(SE_TYPE.OpenTreasure);
        }

        await UniTask.Delay(300, cancellationToken: token).SuppressCancellationThrow();

        // 入手したアイテムの情報表示
        await itemInfoDisplayManager.ShowTreasureItemInfoAsync(itemData, token);

        // アイテムを所持しているか判定
        bool haveItem = playerInventoryManager.HaveTargetItem(itemData.id);

        // インベントリの最大サイズ以下か判定
        bool isInventoryUnderMaxSive = GameData.instance.IsInventoryUnderMaxSize();

        // 獲得アイテムを所持しておらず、かつ、インベントリが最大の場合
        if (!haveItem && !isInventoryUnderMaxSive) {
            itemInfoDisplayManager.InventoryMaxInfo();
            DebugLogger.Log("バッグがいっぱい");

            // ポイント半分獲得
            GameData.instance.userData.SoulPoint.Value += itemData.price / 2;

            // 獲得できずに終了
            return;
        }

        // インベントリにアイテムを追加
        if (isEnemyDrop) {
            // 敵の場合には、1回分の強化を確定
            playerInventoryManager.AddItemDataConvertBackPackInItem(itemData, true);
        } else {
            playerInventoryManager.AddItemDataConvertBackPackInItem(itemData, false);
        }
    }
}
