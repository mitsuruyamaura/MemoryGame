using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapActionDataSO", menuName = "Create TrapActionDataSO")]
public class TrapActionDataSO : ScriptableObject {
    public List<TrapActionData> trapActionDataList = new();    
}