using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// 抽象クラスの名称をインスペクターに表示させる Editor 拡張
/// </summary>
//[CustomPropertyDrawer(typeof(BattleRecordBase), true)]
public class SerializeReferenceDrawer : PropertyDrawer {
    //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    //    EditorGUI.BeginProperty(position, label, property);
    //    //Debug.Log(property.managedReferenceFullTypename);

    //    // クラス名を取得(型情報)
    //    Type fieldType = GetManagedReferenceType(property.managedReferenceFullTypename);
    //    //Debug.Log(fieldType);

    //    // クラス名の表示
    //    if (fieldType != null) {
    //        GUIContent classLabel = new GUIContent(fieldType.Name);
    //        position = EditorGUI.PrefixLabel(position, classLabel);
    //    }

    //    // 通常のプロパティ描画
    //    EditorGUI.PropertyField(position, property, GUIContent.none, true);

    //    EditorGUI.EndProperty();
    //}

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    //    // 子プロパティが展開される高さを取得(インスペクターで矢印ボタンを押したら、その表示分だけ他の UI に重ならないように拡張する)
    //    return EditorGUI.GetPropertyHeight(property, label, true);
    //}

    //private static Type GetManagedReferenceType(string fullTypeName) {
    //    string[] typeNames = fullTypeName.Split(' ');

    //    if (typeNames.Length != 2) {
    //        Debug.Log($"Invalid type format: {fullTypeName}");
    //        return null;
    //    }

    //    string assemblyName = typeNames[0]; // "Assembly-CSharp"
    //    string typeName = typeNames[1];     // "[namespace].[classname]"

    //    // すべてのアセンブリから型を検索
    //    foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies()) {
    //        Type type = assembly.GetType(typeName);
    //        if (type != null) {
    //            return type;
    //        }
    //    }

    //    Debug.Log($"Type not found: {typeName}");
    //    return null;
    //}
}