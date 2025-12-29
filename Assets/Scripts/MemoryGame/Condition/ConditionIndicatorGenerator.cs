using UnityEngine;

public class ConditionIndicatorGenerator : GeneratorBase, ISetup {
    public void SetUp(GameObject entityObject = null) {
        InitObjectPool();
    }
}