using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerSpotSymbol : SymbolBase
{
    public PowerSpotData powerSpotData;
    public bool useStatusPoint;


    public override void OnEnterSymbol() {
        base.OnEnterSymbol();

        // 各種データ取得
        //WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo); 
        //Rarity rarity = WaveData.GetRandomRarity(waveData);

        // 指定したレアリティのみを抽出して List を作成
        //List<PowerSpotData> rarityPowerSpotList = DataBaseManager.instance.GetPowerSpotDataListFromRarity(rarity);

        // 歩数により、Stage のWave が代わり、それによりパワースポットの種類も変わる
        WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);
        Rarity rarity = WaveData.GetRandomRarity(waveData);

        // 指定したレアリティ と TerrainType とで抽出して List を作成
        List<PowerSpotData> wavePowerSpotList = DataBaseManager.instance.GetPowerSpotDataListFromRarity(rarity)
                                                    .Where(data => data.terrainType == terrainType).ToList();

        // WaveCount を指定してその Wave で出現可能な PowerSpot を抽出し、さらに TerrainType に合致するもののみを抽出する。全 Rarity が入っている
        //List<PowerSpotData> wavePowerSpotList = DataBaseManager.instance.GetPowerSpotDataListFromWaveCount(GameData.instance.userData.waveNo)
        //                                            .Where(data => data.terrainType == terrainType).ToList();
        DebugLogger.Log(wavePowerSpotList.Count.ToString());

        // 重み付けのための総重み計算、ランダム値の作成、初期値の用意
        int totalWeight = wavePowerSpotList.Sum(data => data.weight);
        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        // 累積重み(累積和)を計算しながら重み付けに基づいて選択
        var selectedPowerSpot = wavePowerSpotList.FirstOrDefault(data => {
            cumulativeWeight += data.weight;
            return randomValue < cumulativeWeight;
        });

        // 選ばれた PowerSpotData を設定
        powerSpotData = selectedPowerSpot;

        // 上記の方法でも重み付け可能
        // ここからは一般的な重み付けの計算方法
        //int index = 0;

        //for (int i = 0; i < wavePowerSpotList.Count; i++) {
        //    if (randomValue <= wavePowerSpotList[i].weight) {
        //        index = i;
        //        //Debug.Log(index + " value : " + value);
        //        break;
        //    }
        //    randomValue -= wavePowerSpotList[i].weight;
        //}


        // Linqを使って重み付けに基づいてデータを選択も可能
        // ただし、Sum を何回も行うので効率悪い
        //var selectedPowerSpot = wavePowerSpotList
        //    .Select((data, index) => new { data, cumulativeWeight = wavePowerSpotList.Take(index + 1).Sum(d => d.weight) })
        //    .FirstOrDefault(x => randomValue < x.cumulativeWeight)?.data;


        //if (powerSpotData == null) {
        //    Debug.Log($"PowerSpotData なし index :{randomValue}");
        //    return;
        //}

        // List 内からランダムなデータを設定
        //powerSpotData = wavePowerSpotList[index];

        // ホバー表示用のクラスにデータを渡す
        if (TryGetComponent(out PowerSpotHoverUI powerSpotHoverUI)) {
            powerSpotHoverUI.SetPowerSpotData(powerSpotData);
        }
    }


    public override async UniTask TriggerEffectAsync() {
        await base.TriggerEffectAsync();

        if (useStatusPoint) {
            int totalStatusPoint = GameData.instance.EnchantPoint.Value;  // GameData.instance.GetTotalStatusValues();
            bool isRequiredStatus = GameData.instance.IsStatusValueGreaterThanRequired(powerSpotData);

            bool isRelease = false;

            // トータルポイントが上回っていれば解放可能
            if (totalStatusPoint >= powerSpotData.releasePoint) {
                isRelease = true;
                DebugLogger.Log($"{totalStatusPoint} >= {powerSpotData.releasePoint} のため、解放");
            }

            // いずれかの能力値が上回っていれば解放可能
            if (!isRelease && isRequiredStatus) {
                isRelease = true;
                DebugLogger.Log($"{isRequiredStatus} のため、解放");
            }

            // 解放条件を満たしているか確認。満たしていない場合には処理しない
            if (isRelease == false) {
                isSymbolTriggerd = false;

                DebugLogger.Log($"{totalStatusPoint} < {powerSpotData.releasePoint}");
                DebugLogger.Log($"{isRequiredStatus} のため、解放不可");

                PowerSpotInfoDisplayManager.instance.NotReleasePowerPointInfo(ReleaseActionMessageType.NotRelease_Status);
                return;
            }
        }

        // 一旦、ステータスは利用せず、呪いを使ってソウルポイントが超えているかだけ見る
        if (GameData.instance.userData.SoulPoint.Value < powerSpotData.releasePoint) {
            isSymbolTriggerd = false;
            DebugLogger.Log($"{GameData.instance.userData.SoulPoint.Value < powerSpotData.releasePoint} のため、解放不可");

            PowerSpotInfoDisplayManager.instance.NotReleasePowerPointInfo(ReleaseActionMessageType.NotRelease_SoulPoint);
            return;
        } else {
            // ソウルポイントを減算して表示更新(本来は CurseSymbol でやるので、else にしておく)
            GameData.instance.userData.SoulPoint.Value -= powerSpotData.releasePoint;
        }

        PowerSpotInfoDisplayManager.instance.ReleasePowerSpotInfoByBlessing();
        DebugLogger.Log("PowerSpot 解放");
        isSymbolTriggerd = true;

        SoundManager.instance.PlaySE(SE_TYPE.PowerSpot);

        // TODO 一旦１ずつ加算
        GameData.instance.playerCombatData.MaxInventorySize.Value += 1;

        //GameData.instance.userData.Stamina.Value += 5;

        GameData.instance.EnchantPoint.Value -= powerSpotData.releasePoint;
        GameData.instance.consumeEnchantPoint += powerSpotData.releasePoint;
        GameData.instance.userData.consumeSoulPoint += powerSpotData.releasePoint;

        GameData.instance.userData.ExploreCount.Value++;

        GameObject effectPrefab = EffectManager.instance.recoveryStaminaEffectPrefab;
        GameObject effect = Instantiate(effectPrefab, effectTran);
        effect.transform.SetParent(EffectManager.instance.effectConteinerTran);

        Destroy(effect, 1.5f);

        // 祝福表示


        tween?.Kill();
        tween = null;

        tween = transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetLink(gameObject).OnComplete(() => {
            Release();
        });
        await UniTask.Delay(500);
    }
}