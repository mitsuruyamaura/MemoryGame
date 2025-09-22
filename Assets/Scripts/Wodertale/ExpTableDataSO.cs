using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpTableDataSO", menuName = "Create ExpTableDataSO")]
public class ExpTableDataSO : ScriptableObject {
    public List<ExpTableData> expTableDataList = new();
}
