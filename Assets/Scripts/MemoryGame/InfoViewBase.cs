using UnityEngine;
using UnityEngine.UI;

public class InfoViewBase : PoolBase {
    [SerializeField] protected CanvasGroup cg;
    [SerializeField] protected Image imgMain;
    [SerializeField] protected Image[] imgRarityIcons;
    [SerializeField] protected Text txtName;
    [SerializeField] protected Text txtDesc;

    public virtual void HideInfoView() {
        cg.alpha = 0;
        cg.blocksRaycasts = false;
    }
}