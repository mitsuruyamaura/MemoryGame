using UnityEngine;
using UnityEditor;
using R3;
using ObservableCollections;

// EditorGUILayout.ObjectField
// https://docs.unity3d.com/ja/current/ScriptReference/EditorGUILayout.ObjectField.html


/// <summary>
/// ReactiveCollection の要素をインスペクターに表示するためのエディター拡張
/// </summary>
[CustomEditor(typeof(PlayerInventoryManager), true)]
public class BackPackInItemCollectionPreview : Editor {
    private BackPackInItem addItem;


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        // 親クラスを参照
        PlayerInventoryManager _drc = (PlayerInventoryManager)target;
        ObservableList<BackPackInItem> _iCol = _drc.PlayerBackPackItemList;

        int _count = _iCol.Count;
        if (_count == 0) {
            EditorGUILayout.HelpBox("No items or No runtime.", MessageType.None);
            return;
        }

        // リストの表示
        for (int i = 0; i < _count; i++) {
            EditorGUILayout.BeginHorizontal();

            // BackPackInItem の各フィールドを表示(allowSceneObjects の指定をしないと非推奨の警告が出る)
            _iCol[i] = (BackPackInItem)EditorGUILayout.ObjectField(obj: _iCol[i], typeof(BackPackInItem), allowSceneObjects : false);   // as BackPackInItem;

            if (GUILayout.Button("Delete")) {
                _iCol.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        // 追加のGUI
        EditorGUILayout.LabelField(" ");
        EditorGUILayout.BeginHorizontal();
        addItem = (BackPackInItem)EditorGUILayout.ObjectField("Add Item ->", addItem, typeof(BackPackInItem), allowSceneObjects : false);
        if (GUILayout.Button("Add")) {
            if (addItem != null) {
                _iCol.Add(addItem);
                addItem = null;
            }
            EditorGUILayout.EndHorizontal();
            return;
        }
        EditorGUILayout.EndHorizontal();
    }
}

public class DebugReactiveCollection : MonoBehaviour {
    public ObservableList<BackPackInItem> backPackInItemCollection = new ();

    private void Start() {
        // テスト用データの追加
        backPackInItemCollection.Add(new BackPackInItem());
        backPackInItemCollection.Add(new BackPackInItem());

        // UniRxの処理
        backPackInItemCollection.ObserveRemove()
            .Subscribe(i => Debug.Log("Remove Item:" + i.Value.itemData));

        backPackInItemCollection.ObserveAdd()
            .Subscribe(i => Debug.Log("Add Item :" + i.Value.itemData));
    }
}