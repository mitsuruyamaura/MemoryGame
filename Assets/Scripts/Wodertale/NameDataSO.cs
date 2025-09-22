using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NameDataSO", menuName ="Create NameDataSO")]
public class NameDataSO : ScriptableObject {
    public List<NameData> nameDataList = new();
}