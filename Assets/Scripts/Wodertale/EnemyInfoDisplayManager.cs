using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoDisplayManager : AbstractSingleton<EnemyInfoDisplayManager> {

    [SerializeField] private Canvas enemyInfoCanvas;
    [SerializeField] private Image imgEnemyIcon;
    [SerializeField] private Image imgShadeIcon;
    [SerializeField] private Text txtHp;
    [SerializeField] private Text txtMaxHp;
    [SerializeField] private Slider sliderHp;
    [SerializeField] private Text txtRace;
    [SerializeField] private Text txtName;
    [SerializeField] private Text txtShieldHp;

    //[SerializeField] private List<Image> imgEquipItemImageList = new();
    private Transform enemyBackPackItemTran;
    [SerializeField] List<BackPackInItem> enemyBackPackItemList = new();
    //[SerializeField] private float targetHeight = 300f; // 目標の高さ

    private float sliderAnimeDuration = 0.5f;

    // バフデバフ表示


    public void Setup(Transform enemyBackPackItemTran) {
        this.enemyBackPackItemTran = enemyBackPackItemTran;
        HideEnemyInfo();
    }

    /// <summary>
    /// エネミー情報を画面に表示
    /// </summary>
    /// <param name="enemyData"></param>
    /// <param name="name"></param>
    /// <param name="equipItemNoList"></param>
    /// <param name="gameState"></param>
    public void ShowEnemyInfo(EnemyData enemyData, List<int> equipItemNoList, GameData.GameState gameState) {
        // エネミー情報表示中(バトル中)は動作させない(バトル中に敵シンボルにマウスオーバーすると、アイテムが重複表示されるため)
        if (enemyInfoCanvas.enabled) {
            return;
        }

        // 引数で、敵の情報(Hp、バフ、デバフ、装備品)をもらって設定する

        // 名前設定
        NameData nameData = DataBaseManager.instance.GetRandomNameData();
        txtName.text = nameData.name;

        // アイコン画像の設定
        Sprite enemyIcon = DataBaseManager.instance.GetEnemyIcon(enemyData.enemyNo);
        if (enemyIcon != null) {
            imgEnemyIcon.sprite = enemyIcon;
            imgEnemyIcon.SetNativeSize();

            // 今回は使わない
            //{
                //// 足元基準にしたいので Pivot を下中央に設定
                //imgEnemyIcon.rectTransform.pivot = new Vector2(0.5f, 0f);

                //// スプライトの縦横比を計算
                //float aspect = (float)enemyIcon.rect.width / enemyIcon.rect.height;

                //// 高さを targetHeight に固定し、幅を比率で算出
                //imgEnemyIcon.rectTransform.sizeDelta = new Vector2(targetHeight * aspect, targetHeight);
            //}
        }

        // 倒したことのある敵の場合
        if (GameData.instance.CheckDefeatEnemyNo(enemyData.enemyNo)) {
            imgShadeIcon.enabled = false;
            txtRace.text = enemyData.race;
        } else {
            // まだ倒したことのない敵の場合、シルエット表示
            imgShadeIcon.sprite = enemyIcon;
            imgShadeIcon.SetNativeSize();
            imgShadeIcon.enabled = true;
            txtRace.text = $"?{enemyData.undefined}";
        }

        // Hp 表示
        txtHp.text = enemyData.hp.ToString();
        txtMaxHp.text = enemyData.hp.ToString();

        sliderHp.maxValue = enemyData.hp;
        sliderHp.value = enemyData.hp;

        UpdateEnemyShieldHp(enemyData.shieldPower);

        // 装備品のアイコン表示初期化
        //imgEquipItemImageList
        //    .ForEach(image =>
        //    {
        //        image.enabled = false;
        //        image.color = Color.white;
        //    });

        //enemyBackPackItemList.ForEach(item => item.gameObject.SetActive(false));

        // 装備品のアイコン設定
        for (int i = 0; i < equipItemNoList.Count; i++) {
            // 装備品データを設定
            BackPackInItem backPackInItem = PlayerInventoryManager.instance.GetBackPackInItem(enemyBackPackItemTran);
            ItemData itemData = DataBaseManager.instance.GetItemData(equipItemNoList[i]);

            // バトル時のみ購読設定
            if (gameState == GameData.GameState.Battle) {
                backPackInItem.SetUpBackPackItem(itemData, BattleManager.instance.Cts.Token, EntityType.Enemy);
            } else {
                backPackInItem.SetUpInfoDisplay(EntityType.Enemy);
            }
            enemyBackPackItemList.Add(backPackInItem);

            Sprite itemIconSprite = DataBaseManager.instance.GetItemIcon(equipItemNoList[i]);
            enemyBackPackItemList[i].imgItemIcon.sprite = itemIconSprite;
            //enemyBackPackItemList[i].gameObject.SetActive(true);

            // 獲得したことがないアイテムの場合
            if (!GameData.instance.CheckGetItem(equipItemNoList[i])) {
                // シルエット表示
                enemyBackPackItemList[i].imgItemIcon.color = Color.black;
            } else {
                enemyBackPackItemList[i].imgItemIcon.color = Color.white;
            }
        }

        if (enemyInfoCanvas != null) {
            enemyInfoCanvas.enabled = true;
        }
    }

    public void HideEnemyInfo() {
        enemyBackPackItemList?.ForEach(item => item.Release());

        enemyBackPackItemList?.Clear();

        if (enemyInfoCanvas != null) {
            enemyInfoCanvas.enabled = false;
        }
    }


    public void NoShadeEnemy() {
        imgShadeIcon.enabled = false;
    }


    /// <summary>
    /// Hp表示更新
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public void UpdateDisplayEnemyHp(int oldHp, int newHp) {
        sliderHp.DOValue(newHp, sliderAnimeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        txtHp.DOCounter(oldHp, newHp, sliderAnimeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        DebugLogger.Log("Hp 表示更新");
    }


    public void UpdateEnemyShieldHp(int shield) {
        txtShieldHp.text = shield.ToString();
    }
}