using UnityEngine;

public class BlessingInfoDisplayManager : MonoBehaviour {
    [SerializeField] private CanvasGroup blessingInfoCanvas;
    [SerializeField] private Transform blessingInfoViewTran;
    [SerializeField] private InfoViewGenerator blessingInfoViewGenerator;

    private BlessingInfoView blessingInfoView;

    public void Setup() {
        HideBlessingInfo();
        blessingInfoViewGenerator.InitObjectPool();
    }

    public void ShowBlessingInfo(BlessingData blessingData) {
        if (this == null || blessingInfoCanvas == null) {
            return;
        }

        DebugLogger.Log($"blessingData {blessingData} ");

        // 最初だけ生成して保持しておく
        if (blessingInfoView == null) {
            blessingInfoView = (BlessingInfoView)blessingInfoViewGenerator.GetObjectFromPool(blessingInfoViewTran);
        }

        // コンディション情報表示        
        blessingInfoView.ShowBleesingInfo(blessingData);
        blessingInfoCanvas.alpha = 1.0f;
    }

    public void HideBlessingInfo() {
        if (this == null || blessingInfoCanvas == null) {
            return;
        }
        blessingInfoCanvas.alpha = 0;
        blessingInfoView?.HideInfoView();
    }
}