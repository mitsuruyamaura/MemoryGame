using UnityEngine;

/// <summary>
/// フロート表示生成クラス
/// </summary>
public class FloatingViewGenerator : GeneratorBase, ISetup {
    public static FloatingViewGenerator instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期設定 
    /// </summary>
    /// <param name="entityObject"></param>
    public void SetUp(GameObject entityObject) {

        // ObjectPool の初期化
        InitObjectPool();

        //Debug.Log($"{this} Setup 完了");
    }
}