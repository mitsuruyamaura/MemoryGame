using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using R3;

public class BottomButtonManager : MonoBehaviour
{
    [SerializeField] private Button btnEquipChange;
    [SerializeField] private Button btnLifePotion;
    [SerializeField] private Button btnCleansingPotion;  // デバフ解除
    [SerializeField] private Button btnExplore;
    [SerializeField] private Button btnWait;

    [SerializeField] private Text txtLifePotionCount;
    [SerializeField] private Text txtCleansingPotionCount;
    [SerializeField] private Text txtExploreCount;
    [SerializeField] private Text txtWaitCost;

    void Start()
    {
        // ボタンの設定、処理が終わるまでゲームステートを切り替えておき、その状態の間、ボタンを非活性化する

        
    }


}