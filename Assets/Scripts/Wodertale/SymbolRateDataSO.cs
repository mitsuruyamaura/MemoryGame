using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SymbolRateDataSO", menuName = "Create SymbolRateDataSO")]
public class SymbolRateDataSO : ScriptableObject
{
    public List<SymbolRateData> symbolRateDataList = new();
}