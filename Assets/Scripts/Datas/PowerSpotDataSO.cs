using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerSpotDataSO", menuName = "Create PowerSpotDataSO")]
public class PowerSpotDataSO : ScriptableObject {
    public List<PowerSpotData> powerSpotDataList = new();
}