using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExpTableSO", menuName = "Create ExpTableSO")]
public class ExpTableSO : ScriptableObject
{
    public List<ExpTable> expTablesList = new List<ExpTable>();
}
