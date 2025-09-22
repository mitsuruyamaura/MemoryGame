using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

[System.Serializable]
public class AbilityDetail : MonoBehaviour
{
    //[SerializeField]
    //private Button btnAbility;

    //[SerializeField]
    //private Image imgAbility;

    //[SerializeField]
    //private Text txtAbilityCost;

    //public AbilityData abilityData;

    //private SelectAbilityPopUp selectAbilityPopUp;

    //private bool haveAbility = false;       // アビリティ用のアイテムを所持しているかどうかの判定

    //private bool learnedAbility = false;    // アビリティを習得しているかどうかの判定

    ///// <summary>
    ///// AbilityButtonDetail の設定
    ///// </summary>
    ///// <param name="level"></param>
    ///// <param name="abilityType"></param>
    ///// <param name="selectAbilityPopUp"></param>
    //public void SetUpAbilityDetail(int level, AbilityType abilityType, SelectAbilityPopUp selectAbilityPopUp) {
    //    this.selectAbilityPopUp = selectAbilityPopUp;
    //    abilityData.abilityType = abilityType;

    //    // AbilityLevel と AbilityType から AbilityTable を取得
    //    abilityData.abilityTable = DataBaseManager.instance.GetAbilityPointTable(level, abilityType);

    //    btnAbility.onClick.AddListener(OnClickAbilityDetail);
    //}

    ///// <summary>
    ///// ボタンをクリックした際の処理
    ///// </summary>
    //public void OnClickAbilityDetail() {
    //    transform.DOShakeScale(0.5f).SetEase(Ease.OutQuart);

    //    selectAbilityPopUp.SetAbilityDetail(this);
    //}

    ///// <summary>
    ///// コストの支払い有無を確認してボタンの活性化/非活性化の切り替え
    ///// </summary>
    //public void JudgeAbilityCost() {
    //    btnAbility.interactable = false;

    //    // すでに習得済か判定
    //    if (learnedAbility) {
    //        return;
    //    }

    //    // 対応するアビリティアイテムを所持しているか判定
    //    CheckHaveAbilityItem();

    //    // 所持していない場合
    //    if (!haveAbility) {
    //        return;
    //    }

    //    // アビリティのコストが支払えるかどうかを判定
    //    if (GameData.instance.abilityPoint >= abilityData.abilityTable.abilityCost) {
    //        btnAbility.interactable = true;
    //    }
    //}

    ///// <summary>
    ///// 対応するアビリティアイテムを所持しているか判定
    ///// </summary>
    //private void CheckHaveAbilityItem() {

    //    // アビリティアイテムをすでに所持している場合
    //    if (haveAbility) {
    //        // チェック不要
    //        return;
    //    }

    //    // このアビリティのタイプと同じアビリティタイプのみを抽出
    //    List<InventryAbilityItemData> checkList = GameData.instance.abilityItemDatasList.Where(x => x.abilityType == abilityData.abilityType).ToList();

    //    //Debug.Log(checkList.Count);

    //    // チェックリスト内のアビリティアイテムとこのアビリティの番号が合致したら、所持していると判定
    //    if(checkList.Exists(x => x.abilityNo == abilityData.abilityTable.abitilyNo)) {
    //        // 取得した情報を使ってアンロック設定
    //        imgAbility.sprite = abilityData.abilityTable.abilitySprite;
    //        txtAbilityCost.text = abilityData.abilityTable.abilityCost.ToString();

    //        haveAbility = true;
    //    } else {
    //        // ロック設定
    //        txtAbilityCost.text = "";
    //    }
    //}

    ///// <summary>
    ///// アビリティを習得済にする
    ///// </summary>
    //public void LearnAbility() {
    //    learnedAbility = true;
    //}
}
