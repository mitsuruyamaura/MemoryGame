using UnityEngine;

[System.Serializable]
public class ConditionData {
    public int no;
    public ConditionType conditionType;
    public ConditionTriggerType conditionTriggerType;
    public EnchantType enchantType;
    public Sprite conditionIconSprite;
    public int duration;
    public float conditionValue;
    public float effectInterval;
}