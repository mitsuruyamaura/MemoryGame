using UnityEngine;
using DG.Tweening;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class TreasureBoxSymbol : SymbolBase
{
    [SerializeField]
    private TreasurePopUp treasurePopUpPrefab;

    public override void OnEnterSymbol() {
        base.OnEnterSymbol();
    }


    public override async UniTask TriggerEffectAsync() {
        // インベントリの最大サイズ以下か判定
        bool isInventoryUnderMaxSive = GameData.instance.IsInventoryUnderMaxSize();

        // インベントリの最大サイズ以上で持てない場合
        if (isInventoryUnderMaxSive == false) {
            BattleManager.instance.stageUIManager.InventoryMaxInfo();
            DebugLogger.Log("バッグがいっぱい");
            return;
        }

        tween?.Kill();
        tween = null;

        await base.TriggerEffectAsync();

        var token = this.GetCancellationTokenOnDestroy();

        // タイル内に移動するまで待機
        await UniTask.Delay(200, cancellationToken: token);

        // ! アイコン表示
        PlayerInventoryManager.instance.FindTreasureAnim();
        await UniTask.Delay(500, cancellationToken: token);

        WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);

        // 抽選用の重みの合計値を計算
        int totalRate = waveData.rates.Sum();

        // ランダムな値を取得
        int randaomValue = Random.Range(0, totalRate);

        int index = 0;

        // レアリティを抽選
        for (int i = 0; i < waveData.rates.Length; i++) {
            if (randaomValue <= waveData.rates[i]) {
                index = i;
                break;
            }
            randaomValue -= waveData.rates[i];
        }

        // レアリティを指定して、対象となるアイテムデータのリストをもらう
        List<ItemData> dropItemDataList = DataBaseManager.instance.GetItemDataListByRarity(waveData.rarities[index]);

        // ランダムなアイテムを抽出
        int randaomItemNo = Random.Range(0, dropItemDataList.Count);
        ItemData resultItemData = DataBaseManager.instance.GetItemData(dropItemDataList[randaomItemNo].id);

        // 宝箱のアイコン表示
        ItemInfoDisplayManager.instance.ShowTreasureBoxIcon((Rarity)index);

        // TODO 演出(現在はオーブと同じもの)
        //GameObject effect = Instantiate(EffectManager.instance.orbGetEffectPrefab, effectTran);
        //effect.transform.SetParent(EffectManager.instance.effectConteinerTran);
        //Destroy(effect, 1.5f);

        SoundManager.instance.PlaySE(SE_TYPE.OpenTreasure);

        await UniTask.Delay(300, cancellationToken: token);

        // 獲得表示
        await ItemInfoDisplayManager.instance.ShowTreasureItemInfoAsync(resultItemData, token);

        // アイテムをバックパックに追加。必ず強化する
        PlayerInventoryManager.instance.AddItemDataConvertBackPackInItem(resultItemData, true);

        GameData.instance.userData.FindTreasureCount.Value++;

        Release();
    }
}