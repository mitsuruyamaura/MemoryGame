using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 罠解除の正解シーケンスを表示するUI
/// </summary>
public class CorrectSequenceUI : MonoBehaviour {
    [SerializeField] private Transform root;
    [SerializeField] private SymbolIcon iconPrefab;
    private List<SymbolIcon> symbolIconList = new ();

    public void Set(List<QTESymbol> sequence) {
        Clear();

        foreach (var symbol in sequence) {
            SymbolIcon symbolIcon = Instantiate(iconPrefab, root);
            symbolIcon.Set(symbol);
            symbolIconList.Add(symbolIcon);
        }
    }

    /// <summary>
    /// 正解したシンボルを正解表示に変更する
    /// </summary>
    /// <param name="index"></param>
    public void SuccessSymbol(int index) {
        symbolIconList[index].SetSuccessCorrectSymbol();
    }

    /// <summary>
    /// すべてのシンボルを未入力表示に変更する
    /// </summary>
    public void ResetAllSymbols() {
        symbolIconList.ForEach(view => view?.SetFailureCorrectSymbol());
    }

    private void Clear() {
        foreach (Transform child in root) {
            Destroy(child.gameObject);
        }
        symbolIconList.Clear();
    }
}