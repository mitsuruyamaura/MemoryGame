using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingDataSO", menuName = "Create BlessingDataSO")]
public class BlessingDataSO : ScriptableObject {
    public List<BlessingData> blessingDataList = new();
}