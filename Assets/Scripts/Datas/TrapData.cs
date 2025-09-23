using System;
using UnityEngine;

[System.Serializable]
public class TrapData : IMasterData {
    public int id;
    public string name;
    public string desc;
    public Rarity rarity;
    public int weight;
    public int exp;

    public int Id => id;

    public TrapData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        desc = datas[2];
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[3]);
        weight = int.Parse(datas[4]);
        exp = int.Parse(datas[5]);
    }
}