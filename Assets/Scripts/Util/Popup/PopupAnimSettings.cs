using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PopupAnimSettings : ScriptableObject {
    [Header("Open")]
    public Ease openEaseType = Ease.OutBack;
    public float startScale = 0.8f;
    public float openSeconds = 0.6f;

    [Header("Close")]
    public Ease closeEaseType = Ease.InCirc;
    public float closeEndScale = 0.8f;
    public float closeSeconds = 0.6f;

#if UNITY_EDITOR 
    [MenuItem("Assets/Create/PopupAnimSettings")]
    public static void CreateInstance() {
        PopupAnimSettings obj = ScriptableObject.CreateInstance<PopupAnimSettings>();
        Generic.ScriptableObjectCreator.Create<PopupAnimSettings>(obj, name: "NewPopupAnimSettings");
    }
#endif
}