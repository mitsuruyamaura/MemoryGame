using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveDataSO", menuName = "Create WaveDataSO")]
public class WaveDataSO : ScriptableObject {
    public List<WaveData> waveDataList = new();
}
