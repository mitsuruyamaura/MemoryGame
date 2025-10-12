using UnityEngine;

public class BlessingInfoView : InfoViewBase {

    public void ShowBleesingInfo(IMasterData masterData) {
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
}