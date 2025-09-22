using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreasureTableSO", menuName = "Create TreasureTableSO")]
public class TreasureTableSO : ScriptableObject
{
    public List<TreasureTable> treasureTablesList = new List<TreasureTable>();
}
