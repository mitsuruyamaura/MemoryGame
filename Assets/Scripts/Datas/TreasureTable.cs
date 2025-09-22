using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItemData {
    public AbilityType abilityType;
    public string rarities;
}

[System.Serializable]
public class TreasureTable
{
    public int treasureLevel;
    public DropItemData[] dropItemDatas;
}