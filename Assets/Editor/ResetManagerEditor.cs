using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ResetManager))]
public class ResetManagerEditor : Editor {

    //SerializedProperty useResetEveryDay;
    //SerializedProperty resetHour;
    //SerializedProperty resetDateString;
    //SerializedProperty resetDay;

    //private void OnEnable() {
    //    // SerializedProperty によるプロパティ取得
    //    useResetEveryDay = serializedObject.FindProperty("useResetEveryDay");
    //    resetHour = serializedObject.FindProperty("resetHour");
    //    resetDateString = serializedObject.FindProperty("resetDateString");
    //    resetDay = serializedObject.FindProperty("resetDay");
    //}

    //public override void OnInspectorGUI() {
    //    // 対象オブジェクトの取得
    //    ResetManager resetManager = (ResetManager)target;

    //    // SerializedObject の更新
    //    serializedObject.Update();

    //    // 毎日リセットのトグル設定
    //    EditorGUILayout.PropertyField(useResetEveryDay, new GUIContent("Use Reset EveryDay"));

    //    // 毎日リセットが無効の場合、曜日選択表示
    //    if (!useResetEveryDay.boolValue) {
    //        //resetManager.resetDay = (DayOfWeek)EditorGUILayout.EnumPopup("Reset Day", resetManager.resetDay);
    //        EditorGUILayout.PropertyField(resetDay, new GUIContent("Reset Day"));
    //    }

    //    // 時間設定 (0〜23)
    //    EditorGUILayout.IntSlider(resetHour, 0, 23);

    //    // リセット日時編集可能フィールド
    //    EditorGUI.BeginChangeCheck();
    //    string newDateString = EditorGUILayout.TextField("Reset Date (Edit)", resetDateString.stringValue);

    //    // 入力値のフォーマットチェックと反映
    //    if (EditorGUI.EndChangeCheck()) {
    //        DateTime parsedDate;
    //        if (DateTime.TryParseExact(newDateString, "yyyy/MM/dd HH", null, System.Globalization.DateTimeStyles.None, out parsedDate)) {
    //            resetDateString.stringValue = newDateString; // 入力値を保存
    //            resetManager.SetDebugDate(parsedDate); // 仮日時も更新
    //        } else {
    //            EditorGUILayout.HelpBox("フォーマットが不正です！例: 2025/01/10 04", MessageType.Error);
    //        }
    //    }

    //    // 変更を反映
    //    serializedObject.ApplyModifiedProperties();
    //}
}