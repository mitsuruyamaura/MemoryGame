using UnityEngine;

/// <summary>
/// スクリプタブルオブジェクトでゲームの難易度管理を行う(ゲームクリアやゲームオーバー時に初期化される)
/// 一旦、DataBaseManager で管理している
/// </summary>
[CreateAssetMenu(fileName = "EntryData", menuName = "Create EntryData")]
public class EntryData : ScriptableObject {
    public int selectLevel;
}