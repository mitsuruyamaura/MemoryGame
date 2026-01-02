using UnityEngine;

public class NoFlipCountEffect : IConditionEffect, IFlipPointModifier {
    public int ModifyFlipPoint(int baseFlipPoint) => 0;

    public void OnApply(ConditionProgressData data) {}

    public void OnMisstep(ConditionProgressData data) {}

    public void OnTurnEnd(ConditionProgressData data) {}
}