using System;
using UnityEngine;

[System.Serializable]
public class EnemyData {

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
}

/// <summary>
/// デバフ用のコンディションの登録用
/// </summary>
[System.Serializable]
public class EnemyDebuffData {

    // デバフ用のコンディションの設定
    public ConditionType debuffConditionType;

    // デバフ用のコンディションの付与確率
    [Range(0, 100)]
    public int rate;
}
