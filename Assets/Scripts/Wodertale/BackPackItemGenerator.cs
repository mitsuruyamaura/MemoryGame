using UnityEngine;

public class BackPackItemGenerator : GeneratorBase, ISetup
{
    public void SetUp(GameObject entityObject = null) {
        InitObjectPool();
    }
}