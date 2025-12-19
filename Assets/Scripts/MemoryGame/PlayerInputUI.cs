using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 罠解除のプレイヤー入力用UI
/// </summary>
public class PlayerInputUI : MonoBehaviour {
    [SerializeField] private Transform root;
    [SerializeField] private SymbolIcon iconPrefab;
    private List<SymbolIcon> symbolIconList = new();


    public void GenerateRandomPlayerInputSymbols(List<QTESymbol> sequence, UnityAction<QTESymbol> clickAction) {
        Clear();

        foreach (var symbol in sequence) {
            //DebugLogger.Log($"Generating symbol icon for: {symbol}");
            SymbolIcon symbolIcon = Instantiate(iconPrefab, root);
            symbolIcon.Set(symbol, clickAction);
            symbolIconList.Add(symbolIcon);
        }
    }

    public void Clear() {
        foreach (Transform child in root) {
            Destroy(child.gameObject);
        }
        symbolIconList.Clear();
    }
}