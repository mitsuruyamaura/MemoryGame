using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorDataSO", menuName = "Create FloorDataSO")]
public class FloorDataSO : ScriptableObject {
    public List<FloorData> floorDataList = new();
}