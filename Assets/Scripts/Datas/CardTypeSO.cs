using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardTypeSO", menuName = "Create CardTypeSO")]
public class CardTypeSO : ScriptableObject {
    public List<CardTypeMaster> cardTypeList = new();
}