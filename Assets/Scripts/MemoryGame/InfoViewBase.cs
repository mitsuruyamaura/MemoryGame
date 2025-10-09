using UnityEngine;
using UnityEngine.UI;

public class InfoViewBase : PoolBase {
    [SerializeField] protected CanvasGroup cg;
    [SerializeField] protected Image imgMain;
    [SerializeField] protected Image[] imgRarityIcons;
    [SerializeField] protected Text txtName;
    [SerializeField] protected Text txtDesc;


    public void SetUp(IMasterData masterData) {
        isReleased = false;

        if (masterData is IInfoView view) {
            // 表示内容の設定
            txtName.text = view.Name;
            txtDesc.text = view.Description.Replace("\\n", "\n"); ;

            int rarity = (int)view.Rarity;
            for (int i = 0; i < (int)Rarity.Rare + 1; i++) {                
                if (i <= rarity) {
                    imgRarityIcons[i].gameObject.SetActive(true);
                } else {
                    imgRarityIcons[i].gameObject.SetActive(false);
                }
            }

            // 画像の設定
            if (masterData is IHasIcon iconData) {
                Sprite mainImage = iconData.GetIcon();
                if (mainImage != null) {
                    imgMain.sprite = mainImage;
                }
            }

            cg.alpha = 1.0f;
            cg.blocksRaycasts = true;
        }
    }

    public override void Release() {
        base.Release();

        if (isReleased) {
            DebugLogger.Log("This object has already been released.");
            return;
        }

        if (this == null || transform == null || !gameObject.activeInHierarchy) {
            return;
        }

        isReleased = true;

        objectPool.Release(this);
        transform.SetParent(PlayerInventoryManager.instance.transform);

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
}