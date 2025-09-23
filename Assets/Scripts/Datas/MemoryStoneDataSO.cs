using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MemoryStoneDataSO", menuName = "Create MemoryStoneDataSO")]
public class MemoryStoneDataSO : ScriptableObject {
    public List<MemoryStoneData> memoryStoneList = new();
}