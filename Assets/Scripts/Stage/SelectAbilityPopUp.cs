using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SelectAbilityPopUp : PopUpBase
{
    //[SerializeField]
    //private Button btnExit;

    //[SerializeField]
    //private Button btnSubmit;

    //[SerializeField]
    //private Text txtAbilityPoint;

    //[SerializeField]
    //private Text txtDescription;

    //private Stage stage;

    //// アビリティ用のボタンのプレファブ
    //[SerializeField]
    //private AbilityDetail abilityDetailPrefab;

    //// 生成位置を４箇所 for 文を二重で回すので配列にする
    //[SerializeField]
    //private Transform[] abilityDetailTrans;

    //// 選択しているアビリティの保持
    //private AbilityDetail currentAbilityDetail;

    //// AbilityDetail の管理用
    //public List<AbilityDetail[]> abilityDetailsList;

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="stage"></param>
    //public void SetUpSelectAbilityPopUp(Stage stage) {

    //    canvasGroup.alpha = 0;
    //    canvasGroup.blocksRaycasts = false;

    //    this.stage = stage;

    //    // ボタンの生成と設定
    //    CreateAbilityDetails();

    //    // 初期値設定
    //    Initialize();

    //    // ボタンにメソッドを登録
    //    btnExit.onClick.AddListener(OnExitPopUp);

    //    btnSubmit.onClick.AddListener(PowerUpAbility);
    //    btnSubmit.interactable = false;
    //}

    ///// <summary>
    ///// AbilityDetail ボタンの生成と設定
    ///// </summary>
    //private void CreateAbilityDetails() {
    //    abilityDetailsList = new List<AbilityDetail[]>(4);

    //    for (int i = 0; i < (int)AbilityType.Count - 3; i++) {  // TODO あとで条件を直す
    //        abilityDetailsList.Add(new AbilityDetail[10]);

    //        for (int x = 0; x < DataBaseManager.instance.abilityItemDataSOList[i].abilityItemDatasList.Count; x++) {
    //            AbilityDetail abilityDetail = Instantiate(abilityDetailPrefab, abilityDetailTrans[i]);
    //            abilityDetail.SetUpAbilityDetail(x + 1, (AbilityType)i, this);
    //            abilityDetailsList[i][x] = abilityDetail;
    //        }
    //    }
    //}

    ///// <summary>
    ///// ポップアップを表示する
    ///// </summary>
    //public void ShowPopUp() {
    //    canvasGroup.blocksRaycasts = true;

    //    // 初期値に戻す
    //    Initialize();

    //    canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.Linear);
    //}

    //public override void OnExitPopUp() {
    //    base.OnExitPopUp();

    //    ClosePopUp();
    //}

    ///// <summary>
    ///// ポップアップを非表示にする
    ///// </summary>
    //public void ClosePopUp() {
    //    canvasGroup.blocksRaycasts = false;

    //    canvasGroup.DOFade(0.0f, 0.5f).SetEase(Ease.Linear);
    //    stage.SwitchMaskField(true);
    //}

    ///// <summary>
    ///// アビリティポイントの表示更新
    ///// </summary>
    //public void UpdateDisplayAbilityPoint() {
    //    txtAbilityPoint.text = GameData.instance.abilityPoint.ToString();
    //}

    ///// <summary>
    ///// 選択した AbilityDetail を選択中に設定
    ///// </summary>
    //public void SetAbilityDetail(AbilityDetail abilityDetail) {
    //    currentAbilityDetail = abilityDetail;
    //    btnSubmit.interactable = true;

    //    // 選択している AbilityDetail の表示更新
    //    UpdateDisplayAbilityDescription();
    //}

    ///// <summary>
    ///// 選択している AbilityDetail を利用して、対応する能力を強化
    ///// </summary>
    //private void PowerUpAbility() {
    //    btnSubmit.interactable = false;

    //    switch (currentAbilityDetail.abilityData.abilityType) {
    //        case AbilityType.Hp:
    //            GameData.instance.currentCharaData.maxHp += (int)currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //            GameData.instance.maxHp = GameData.instance.currentCharaData.maxHp;
    //            GameData.instance.hp += (int)currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //            StartCoroutine(stage.UpdateDisplayHp());
    //            break;
    //        case AbilityType.AttackPower:
    //            GameData.instance.currentCharaData.attackPower += (int)currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //            break;
    //        case AbilityType.MoveSpeed:
    //            GameData.instance.currentCharaData.moveSpeed += currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //            break;
    //        case AbilityType.ChargeSpeed:
    //            GameData.instance.currentCharaData.chargeSpeed += (int)currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //            break;
    //    }

    //    // 強化に必要なコストを支払う
    //    int startValue = GameData.instance.abilityPoint;

    //    // アビリティを取得済にする(重複習得不可の制御)
    //    currentAbilityDetail.LearnAbility();

    //    // アニメーションさせて AbilityPoint の表示を更新
    //    txtAbilityPoint.DOCounter(startValue, GameData.instance.abilityPoint -= currentAbilityDetail.abilityData.abilityTable.abilityCost, 1.0f).SetEase(Ease.InQuart);

    //    // エフェクト
    //    StartCoroutine(stage.PlayAbilityPowerUpEffect());

    //    // 初期値に戻す
    //    Initialize();
    //}

    ///// <summary>
    ///// すべての AbilityDetail のコスト支払い有無を確認してボタンの活性化/非活性化の切り替え
    ///// </summary>
    //private void SwitchActivateAbilityDetails() {
    //    for (int i = 0; i < abilityDetailsList.Count; i++) {
    //        for (int x = 0; x < abilityDetailsList[i].Length; x++) {
    //            abilityDetailsList[i][x].JudgeAbilityCost();
    //        }
    //    }
    //}

    ///// <summary>
    ///// 選択されている AbilityDetail の詳細表示の更新
    ///// </summary>
    //private void UpdateDisplayAbilityDescription() {

    //    // 選択されている AbilityDetail が存在しない場合
    //    if (currentAbilityDetail == null) {
    //        txtDescription.text = "アビリティを選択してください";
    //    } else {
    //        txtDescription.text = "強化レベル : " + currentAbilityDetail.abilityData.abilityTable.abilityLevel.ToString() + "\n";
    //        txtDescription.text += currentAbilityDetail.abilityData.abilityTable.abilityName + "\n";
    //        txtDescription.text += currentAbilityDetail.abilityData.abilityType + " : + " + currentAbilityDetail.abilityData.abilityTable.powerUpValue;
    //    }
    //}

    ///// <summary>
    ///// 初期値に戻す
    ///// </summary>
    //private void Initialize() {

    //    // コストの表示更新
    //    UpdateDisplayAbilityPoint();

    //    currentAbilityDetail = null;

    //    // 選択している AbilityDetail の表示更新
    //    UpdateDisplayAbilityDescription();

    //    // 最新のコストで支払い可能な AbilityDetail があるかをチェックし、ボタンの活性化を行う
    //    SwitchActivateAbilityDetails();
    //}
}
