using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 難易度用のトグル
/// </summary>
public class DifficultySelectToggle : MonoBehaviour {
    [SerializeField] private Toggle toggle;
    public Toggle Toggle => toggle;

    [SerializeField] private List<Text> txtMessageList;
    private int level;
    public int Level => level;

    public void Setup(int level) {
        this.level = level;
    }

    public void Choose() {
        // すべてのテキストの文字色を黒にする
        txtMessageList.ForEach(txt => txt.color = Color.black);
    }

    public void Unchoose() {
        // すべてのテキストの文字色を白にする
        txtMessageList.ForEach(txt => txt.color = Color.white);
    }
}