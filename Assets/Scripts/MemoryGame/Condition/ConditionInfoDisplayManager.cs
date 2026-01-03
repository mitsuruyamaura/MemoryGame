using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public class ConditionInfoDisplayManager : MonoBehaviour {
    [SerializeField] private CanvasGroup conditionInfoCanvas;
    [SerializeField] private Transform conditionInfoViewTran;
    [SerializeField] private InfoViewGenerator conditionInfoViewGenerator;

    private ConditionInfoView conditionInfoView;

    public void Setup() {
        HideConditionInfo();
        conditionInfoViewGenerator.InitObjectPool();
    }

    public void ShowConditionInfo(ConditionProgressData conditionProgressData) {
        if (this == null || conditionInfoCanvas == null) {
            return;
        }

        DebugLogger.Log($"conditionProgressData {conditionProgressData} ");

        // 最初だけ生成して保持しておく
        if (conditionInfoView == null) {
            conditionInfoView = (ConditionInfoView)conditionInfoViewGenerator.GetObjectFromPool(conditionInfoViewTran);
        }

        // コンディション情報表示        
        conditionInfoView.ShowConditionInfo(conditionProgressData);
        conditionInfoCanvas.alpha = 1.0f;
    }

    public void HideConditionInfo() {
        if (this == null || conditionInfoCanvas == null) {
            return;
        }
        conditionInfoCanvas.alpha = 0;
        conditionInfoView?.HideInfoView();
    }
}
