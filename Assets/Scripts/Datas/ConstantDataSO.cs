using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstantDataSO", menuName = "Create ConstantDataSO")]
public class ConstantDataSO : ScriptableObject {
    public List<ConstantData> constantDataList = new();
}