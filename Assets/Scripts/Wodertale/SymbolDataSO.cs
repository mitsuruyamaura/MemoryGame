using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SymbolDataSO", menuName = "Create SymbolDataSO")]
public class SymbolDataSO : ScriptableObject
{
    public List<SymbolData> symbolDataList = new();
}