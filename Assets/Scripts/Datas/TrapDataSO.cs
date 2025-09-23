using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapDataSO", menuName = "Create TrapDataSO")]
public class TrapDataSO : ScriptableObject {
    public List<TrapData> trapDataList = new();
}