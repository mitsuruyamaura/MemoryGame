using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyData : IMasterData, IInfoView, IHasIcon {
    public int enemyNo;
    public Rarity rarity;
    public string race;

    public int hp;
    public int exp;

    public int minMoveRate;
    public int maxMoveRate;
    public int moveCount;

    public string equipIndexStr;
    public int minEquipCount;
    public int maxEquipCount;

    // デバフ用のコンディションのデータ
    public string debuffDataStr;
    public string undefined;
    public int shieldPower;

    public int Id => enemyNo;
    public Rarity Rarity => rarity;
    public string Name => race;

    public string Description => "";

    public Sprite GetIcon() {
        return Resources.Load<Sprite>("Enemy/" + Id);
    }

    public EnemyData(string[] datas) {
        enemyNo = int.Parse(datas[0]);
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[1]);
        race = datas[2];
        hp = int.Parse(datas[3]);
        exp = int.Parse(datas[4]);
        minMoveRate = int.Parse(datas[5]);
        maxMoveRate = int.Parse(datas[6]);
        moveCount = int.Parse(datas[7]);
        equipIndexStr = datas[8];
        minEquipCount = int.Parse(datas[9]);
        maxEquipCount = int.Parse(datas[10]);
        debuffDataStr = datas[11];
        undefined = datas[12];
        shieldPower = int.Parse(datas[13]);
    }


    public List<int> GetEquipItemNoList() {
        int equipNum = UnityEngine.Random.Range(minEquipCount, maxEquipCount);

        int[] equipItemIndexs = equipIndexStr.Split('/').ToArray().Select(data => int.Parse(data)).ToArray();
        List<int> equipItemNoList = equipItemIndexs.OrderBy(x => Guid.NewGuid())  // シャッフル
            .Take(equipNum)  // 指定した数だけ取り出す
            .ToList();

        return equipItemNoList;
    }
}