using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MonospaceText : MonoBehaviour {
    public float characterWidth = 20f; // �C�ӂ̌Œ蕝
    private Text textComponent;


    void Start() {
        TryGetComponent(out textComponent);
        UpdateText();
    }

    public void UpdateText() {
        if (textComponent == null) return;

        string originalText = textComponent.text;
        string formattedText = "";

        foreach (char c in originalText) {
            formattedText += $"<mspace={characterWidth}>{c}</mspace>";
        }

        textComponent.text = formattedText;
    }
}