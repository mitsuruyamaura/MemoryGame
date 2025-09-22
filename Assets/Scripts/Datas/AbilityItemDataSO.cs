using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityPointTableSO", menuName = "Create AbilityPointTableSO")]
public class AbilityItemDataSO : ScriptableObject
{
    public AbilityType abilityType;
    public List<AbilityItemData> abilityItemDatasList;

    [System.Serializable]
    public class AbilityItemData {
        public AbilityType abilityType;
        public int abilityLevel;
        public int abilityCost;
        public float powerUpValue;
        public string abilityName;
        public Sprite abilitySprite;
        public int rarity;
        public int abitilyNo;
        public int weight;
    }
}
