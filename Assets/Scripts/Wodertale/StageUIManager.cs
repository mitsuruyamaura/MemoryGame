using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour {
    [SerializeField] private Text txtHp;
    [SerializeField] private Text txtMaxHp;
    [SerializeField] private Text txtPlayerLevel;
    [SerializeField] private Text txtShieldHp;
    [SerializeField] private Text txtWaveNo;
    [SerializeField] private Text txtDefeatedEnemyCount;
    [SerializeField] private Text txtFindTreasureCount;
    [SerializeField] private Text txtExploreCount;
    [SerializeField] private Text txtUncurseCount;
    [SerializeField] private Text txtSoulPoint;
    [SerializeField] private Image imgSoulIcon;
    [SerializeField] private Text txtComboPairCount;

    [SerializeField] private Text txtWaveInfo;
    [SerializeField] private Text txtRestartMessage;
    [SerializeField] private Text txtSettlementInfo;    // 画面優先順位の関係上、Canvas_ItemInfo オブジェクト内にあるオブジェクトを使う
    [SerializeField] private Text txtTrapInfo;

    [SerializeField] private Slider sliderHp;

    [SerializeField] private Button btnPlayerLevel;

    [SerializeField] private Button btnMemoria;
    [SerializeField] private PlayerInfoListPopup playerInfoListPopup;

    [SerializeField] private Transform canvasTran;
    [SerializeField] private Image enemyIcon;

    [SerializeField] private GameObject timeObj;
    [SerializeField] private GameObject timeObjTrapDisarm;   // 罠解除QTE用
    [SerializeField] private Image imgTime;
    [SerializeField] private Image imgTimeTrapDisarm;        // 罠解除QTE用
    [SerializeField] private Image imgBattleState;
    [SerializeField] private Sprite[] stateSprites;    // Win,Loose,Timeout の順番
    [SerializeField] private Image inventoryFilter;

    [SerializeField] private Color releaseMessageColor;
    [SerializeField] private CircleOutline circleOutline;

    [SerializeField] private RectTransform playerLifeDefaultTran;
    [SerializeField] private RectTransform playerLifeBattleTran;
    [SerializeField] private RectTransform playerLifeSetTran;

    private int flipGainPoint = 1;
    [SerializeField] private Text txtFlipGainPoint;              // 回復量
    [SerializeField] private Text txtFlipGainInfo;               // めくれる回数ボタンの上に表示するメッセージ。XP 値か、不足と出す
    [SerializeField] private Button btnShowFlipCountGainPop;     // めくれる回数部分のボタン。めくれる回数回復ポップを開く
    [SerializeField] private Button btnSubmitFlipCountGain;      // めくれる回数回復ボタン
    [SerializeField] private Button btnCloseFlipCountGainPop;    // めくれる回数回復ポップを閉じるボタン
    [SerializeField] private CanvasGroup cgFlipCountGainPop;     // めくれる回数回復ポップの CanvasGroup

    private float lifeGainRate = 0.3f;
    [SerializeField] private Text txtLifeGainPoint;         // 回復量
    [SerializeField] private Text txtLifeGainInfo;          // ライフボタンの上に表示するメッセージ。XP 値か、不足と出す
    [SerializeField] private Button btnShowLifeGainPop;     // ライフ部分のボタン。ライフ回復ポップを開く
    [SerializeField] private Button btnSubmitLifeGain;      // ライフ回復ボタン
    [SerializeField] private Button btnCloseLifeGainPop;    // ライフ回復ポップを閉じるボタン
    [SerializeField] private CanvasGroup cgLifeGainPop;     // ライフ回復ポップの CanvasGroup

    [SerializeField] private CanvasGroup cgTrapDisarm;      // 罠解除QTE用の CanvasGroup
    [SerializeField] private Transform playerBackPackItemTran;
    public Transform PlayerBackPackItemTran => playerBackPackItemTran;

    [SerializeField] private CanvasGroup cgItemRestrictionShade;


    private string SuccessSettlementMessage = "平和的解決に成功しました!!";
    private string SuccessTrapMessage = "罠の解除に成功しました!!";
    private string FailureTrapMessage = "罠の解除に失敗しました...";

    private string UnobtainableItemMessage = "呪いにより、アイテム獲得できません";

    private float defaultTime;                   // バトルのデフォルト時間
    private float sliderAnimeDuration = 0.5f;

    private CancellationTokenSource cts;
    private BattleManager battleManager;


    public void Setup(int maxHp, MemoryGameManager memoryGameManager, BattleManager battleManager) {
        this.battleManager = battleManager;

        cts = new();

        SetMaxHpDisplay(maxHp);

        defaultTime = battleManager.BattleDuration.Value;

        // TODO スライダーの初期化と、最大値のセット
        battleManager.BattleDuration.Subscribe(time => 
        {
            float fil = time / defaultTime;
            imgTime.DOFillAmount(fil, fil).SetEase(Ease.Linear).SetLink(gameObject);
        }).AddTo(this);

        battleManager.OnBattleStart.Subscribe(_ => ShowTimeCanvas()).AddTo(this);

        battleManager.OnBattleEnd.Subscribe(resultType => {
            HideTimeCanvas();
            ShowBattleState(resultType);
        }).AddTo(this);

        HideTimeCanvas();
        HideBattleState(null);
        HideItemLockShade();

        GameData.instance.userData.SoulPoint
            .Zip(GameData.instance.userData.SoulPoint.Skip(1), (prevPoint, nextPoint) => (prevPoint, nextPoint))
            .Subscribe(soulPoint => UpdateSoulPoint(soulPoint.prevPoint, soulPoint.nextPoint))
            .AddTo(this);

        // クラス一覧、スキル一覧ポップアップ表示
        playerInfoListPopup.InitializePopUp();
        btnMemoria.OnClickExt(() => ShowPlayerInfoListPop(), this);

        HidePlayerInfoListPop();

        // ペアのコンボ回数の購読
        GameData.instance.ComboPairCount.Subscribe(comboCount => UpdateDisplayComboPairCount(comboCount)).AddTo(this);

        // めくれる回数回復関連のボタン
        btnShowFlipCountGainPop
            .OnClickExt(() => {
                if (cgFlipCountGainPop.blocksRaycasts == false) {
                    ShowFlipCountGainPopup();
                } else {
                    HideFlipCountGainPopup();
                }
            }, this, TimeSpan.FromMilliseconds(500));
        btnSubmitFlipCountGain.OnClickExt(() => GainFlipCount(), this);
        btnCloseFlipCountGainPop.OnClickExt(() => HideFlipCountGainPopup(), this);

        // ライフ回復関連のボタン
        btnShowLifeGainPop
            .OnClickExt(() => {
                if (cgLifeGainPop.blocksRaycasts == false) {
                    ShowLifeGainPopup();
                } else {
                    HideLifeGainPopup();
                }
            }, this, TimeSpan.FromMilliseconds(500));
        btnSubmitLifeGain.OnClickExt(() => GainLife(), this);
        btnCloseLifeGainPop.OnClickExt(() => HideLifeGainPopup(), this);

        HidePopups();

        // カードをめくったときの購読処理。開いているポップアップを閉じる
        memoryGameManager.OnCardSelect.Subscribe(_ => HidePopups());
    }

    /// <summary>
    /// OnEndBattle より購読
    /// </summary>
    /// <param name="battleResultType"></param>
    public void ShowBattleState(BattleResultType battleResultType) {
        imgBattleState.sprite = stateSprites[(int)battleResultType];
        imgBattleState.gameObject.SetActive(true);

        playerLifeSetTran.SetParent(playerLifeDefaultTran);
        playerLifeSetTran.transform.localPosition = Vector3.zero;

        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.AppendInterval(0.25f);
        sequence.Append(imgBattleState.DOFade(1.0f, 0.75f).SetEase(Ease.OutBack));
        sequence.AppendInterval(0.75f);
        sequence.Append(imgBattleState.DOFade(0, 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => HideBattleState(sequence)));        
    }

    public void HideBattleState(Sequence sequence) {
        imgBattleState.gameObject.SetActive(false);
        imgBattleState.color = new(1, 1, 1, 0);

        sequence?.Kill();
        sequence = null;

        inventoryFilter.gameObject.SetActive(false);
    }

    private void ShowTimeCanvas() {
        imgTime.fillAmount = 1.0f;
        timeObj.SetActive(true);

        inventoryFilter.gameObject.SetActive(true);

        playerLifeSetTran.SetParent(playerLifeBattleTran);
        playerLifeSetTran.transform.localPosition = Vector3.zero;
    }

    public void ShowTimeCanvasTrapDisarm() {
        imgTimeTrapDisarm.fillAmount = 1.0f;
        timeObjTrapDisarm.SetActive(true);
        cgTrapDisarm.alpha = 1f;
        cgTrapDisarm.blocksRaycasts = true;
    }

    private void HideTimeCanvas() {
        timeObj.SetActive(false);
    }

    public void HideTimeCanvasTrapDisarm() {
        cgTrapDisarm.alpha = 0f;
        cgTrapDisarm.blocksRaycasts = false;
        timeObjTrapDisarm.SetActive(false);
    }

    /// <summary>
    /// 罠解除QTE用の残り時間表示更新
    /// </summary>
    /// <param name="timeLimit"></param>
    /// <param name="elapsedTime"></param>
    public void UpdateTimeCanvasTrapDisarm(float timeLimit, float elapsedTime) {
        //float fil = remainTime / timeLimit;
        //imgTimeTrapDisarm.DOFillAmount(fil, fil).SetEase(Ease.Linear).SetLink(gameObject);
        float remainTime = Mathf.Max(timeLimit - elapsedTime, 0f);
        imgTimeTrapDisarm.fillAmount = remainTime / timeLimit;
    }

    public void SetMaxHpDisplay(int maxHp) {
        txtHp.text = maxHp.ToString();
        txtMaxHp.text = maxHp.ToString();

        sliderHp.maxValue = maxHp;
        sliderHp.value = maxHp;
    }

    /// <summary>
    /// Hp表示更新
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public void UpdateDisplayPlayerHp(int oldHp, int newHp) {
        sliderHp.DOValue(newHp, sliderAnimeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        txtHp.DOCounter(oldHp, newHp, sliderAnimeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        DebugLogger.Log("Hp 表示更新");
    }

    public void UpdatePlayerShieldHp(int shield) {
        txtShieldHp.text = shield.ToString();
        DebugLogger.Log($"shield : {shield}");
    }

    public void UpdateWaveNo(int waveNo) {
        txtWaveNo.text = waveNo.ToString();
    }

    private void UpdateDisplayComboPairCount(int count) {
        // 現在はデバッグ用の画面右上に出す
        txtComboPairCount.text = count.ToString();

        // TODO コンボしたときだけ、演出を出す

    }

    private void UpdateSoulPoint(int prevPoint, int nextPoint) {
        // アニメさせるだけならこちらで OK
        //txtSoulPoint.DOCounter(prevPoint, nextPoint, 0.5f).SetEase(Ease.Linear).SetLink(gameObject);

        // アニメさせつつ、カンマ区切りしたい場合は To で対応する
        DOTween.To(
            () => prevPoint,    // 第1引数：開始時の値を提供
            currentValue =>     // 第2引数：アニメーション中の値を受け取るための変数
            {
                txtSoulPoint.text = string.Format("{0:n0}", currentValue);
            },
            nextPoint,          // 第3引数：目標値
            0.5f)               // 第4引数：アニメーションの持続時間
            .SetEase(Ease.Linear).SetLink(gameObject);

        imgSoulIcon.transform.DOPunchScale(Vector3.one * 1.25f, 0.5f, 5)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject)
            .OnComplete(() => imgSoulIcon.transform.localScale = Vector3.one);
    }

    /// <summary>
    /// 次の Wave にうつる前の Info 表示
    /// </summary>
    public void NextWaveInfo() {
        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtWaveInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtWaveInfo.DOFade(0f, 0.5f).SetEase(Ease.Linear));
    }

    /// <summary>
    /// 交渉に成功したときのメッセージ表示
    /// </summary>
    public void SuccessSettlementInfo() {
        circleOutline.SetEffectColor(releaseMessageColor);
        txtSettlementInfo.text = SuccessSettlementMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtSettlementInfo.DOFade(1.0f, 0.8f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtSettlementInfo.DOFade(0f, 0f).SetEase(Ease.Linear)).OnComplete(() => txtSettlementInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }

    /// <summary>
    /// 罠の解除に成功したときのメッセージ表示
    /// </summary>
    public void SuccessTrapDisarmInfo() {
        txtTrapInfo.text = SuccessTrapMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtTrapInfo.DOFade(1.0f, 0.8f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtTrapInfo.DOFade(0f, 0f).SetEase(Ease.Linear)).OnComplete(() => txtTrapInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }

    /// <summary>
    /// 罠の解除に失敗したときのメッセージ表示
    /// </summary>
    public void FailureTrapDisarmInfo() {
        txtTrapInfo.text = FailureTrapMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtTrapInfo.DOFade(1.0f, 0.8f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtTrapInfo.DOFade(0f, 0f).SetEase(Ease.Linear)).OnComplete(() => txtTrapInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }

    /// <summary>
    /// デバフによりアイテムが獲得できないときのメッセージ表示
    /// </summary>
    public void UnobtainableItemInfo() {
        txtTrapInfo.text = UnobtainableItemMessage;

        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtTrapInfo.DOFade(1.0f, 0.8f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtTrapInfo.DOFade(0f, 0f).SetEase(Ease.Linear)).OnComplete(() => txtTrapInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
    }


    /// <summary>
    /// ゲームオーバー、クリア時に表示する、タイトルへ戻すためのメッセージ
    /// </summary>
    public void ShowRestartMessage() {
        txtRestartMessage.gameObject.SetActive(true);
    }

    /// <summary>
    /// メモリアランクやクラスやスキルなどのデータを管理するポップアップ表示
    /// </summary>
    public void ShowPlayerInfoListPop() {
        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        playerInfoListPopup.OpenPopUpAsync(cts.Token).Forget();
    }

    private void HidePlayerInfoListPop() {
        playerInfoListPopup.ClosePopUpAsync(cts.Token).Forget();
    }
    
    private void ShowFlipCountGainPopup() {
        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        cgFlipCountGainPop.alpha = 1.0f;
        cgFlipCountGainPop.blocksRaycasts = true;

        // 回復量表示
        txtFlipGainPoint.text = $"{GameData.instance.userData.FlipPoint.Value} >>> {GameData.instance.userData.FlipPoint.Value + flipGainPoint}";

        // 必要なポイント表示(一旦、固定値で)
        int flipGainRequiredPoint = GameData.instance.flipGainRequiredXP;
        txtFlipGainInfo.text = $"{flipGainRequiredPoint}";

        // ポイントが足りていないなら
        if (flipGainRequiredPoint > GameData.instance.userData.SoulPoint.Value) {
            txtFlipGainInfo.color = Color.red;
            btnSubmitFlipCountGain.interactable = false;
        } else {
            txtFlipGainInfo.color = new(1, 1, 1, 1);
            btnSubmitFlipCountGain.interactable = true;
        }
    }

    private void HideFlipCountGainPopup() {
        cgFlipCountGainPop.alpha = 0f;
        cgFlipCountGainPop.blocksRaycasts = false;
    }

    /// <summary>
    /// ソウルポイントを消費して、めくれる回数を回復する
    /// </summary>
    private void GainFlipCount() {
        // ポイント消費
        int flipGainRequiredPoint = GameData.instance.flipGainRequiredXP;
        GameData.instance.CalcSoulPoint(-flipGainRequiredPoint);

        // 消費ポイントの累計を更新
        GameData.instance.userData.consumeSoulPoint += flipGainRequiredPoint;

        GameData.instance.CalcFlipPoint(flipGainPoint);

        // ポップ表示内容更新
        ShowFlipCountGainPopup();
    }

    private void ShowLifeGainPopup() {
        if (GameData.instance.CurrentGameState.Value != GameState.Play) {
            return;
        }

        cgLifeGainPop.alpha = 1.0f;
        cgLifeGainPop.blocksRaycasts = true;

        // 回復量を計算し、最大値以内に抑えたうえで回復量表示
        int gainPoint = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * lifeGainRate);
        int limitPoint = Mathf.Min(battleManager.PlayerHP.Value + gainPoint, GameData.instance.charaStatus.MaxHp.Value);
        txtLifeGainPoint.text = $"{battleManager.PlayerHP.Value} >>> {limitPoint}";

        // 必要なポイント表示(一旦、固定値で)
        int lifeGainRequiredPoint = GameData.instance.lifeGainRequiredXP;
        txtLifeGainInfo.text = $"{lifeGainRequiredPoint}";

        // ポイントが足りていないなら
        if (lifeGainRequiredPoint > GameData.instance.userData.SoulPoint.Value) {
            txtLifeGainInfo.color = Color.red;
            btnSubmitLifeGain.interactable = false;
        } else {
            txtLifeGainInfo.color = new(1, 1, 1, 1);
            btnSubmitLifeGain.interactable = true;
        }
    }

    private void HideLifeGainPopup() {
        cgLifeGainPop.alpha = 0f;
        cgLifeGainPop.blocksRaycasts = false;
    }

    private void GainLife() {
        // ポイント消費
        int lifeGainRequiredPoint = GameData.instance.lifeGainRequiredXP;
        GameData.instance.CalcSoulPoint(-lifeGainRequiredPoint);

        // 消費ポイントの累計を更新
        GameData.instance.userData.consumeSoulPoint += lifeGainRequiredPoint;

        // TODO クラス特典、回復量増量など



        // 回復量を計算し、最大値以内に抑えたうえで回復量表示
        int gainPoint = Mathf.FloorToInt(GameData.instance.charaStatus.MaxHp.Value * lifeGainRate);
        gainPoint = Mathf.Min(gainPoint, GameData.instance.charaStatus.MaxHp.Value);

        // この中で回復処理実行
        battleManager.UpdatePlayerHp(gainPoint, EffectType.Heal, false);

        // ポップ表示内容更新
        ShowLifeGainPopup();
    }

    /// <summary>
    /// 呪詛効果時のアイテムロック演出表示
    /// </summary>
    public void ShowItemLockShade() {
        cgItemRestrictionShade.alpha = 1.0f;
        cgItemRestrictionShade.blocksRaycasts = true;
    }

    /// <summary>
    /// アイテムロック演出非常時
    /// </summary>
    public void HideItemLockShade() {
        cgItemRestrictionShade.alpha = 0;
        cgItemRestrictionShade.blocksRaycasts = false;
    }


    public void HidePopups() {
        HideFlipCountGainPopup();
        HideLifeGainPopup();
    }

    // 現在は未使用
    // ------------------ Action Result Displays ------------------

    //private void SetUpActionResultDisplays() {
    //    GameData.instance.userData.DefeatedEnemyCount.Subscribe(count => UpdateDisplayDefeatedEnemyCount(count)).AddTo(this);
    //    GameData.instance.userData.FindTreasureCount.Subscribe(count => UpdateDisplayFindTreasureCount(count)).AddTo(this);
    //    GameData.instance.userData.BlessingCount.Subscribe(count => UpdateDisplayExploreCount(count)).AddTo(this);
    //    GameData.instance.userData.MemoriaCount.Subscribe(count => UpdateDisplayUncurseCount(count)).AddTo(this);
    //}

    //private void UpdateDisplayDefeatedEnemyCount(int count) {
    //    txtDefeatedEnemyCount.text = count.ToString();
    //}

    //private void UpdateDisplayFindTreasureCount(int count) {
    //    txtFindTreasureCount.text = count.ToString();
    //}

    //private void UpdateDisplayExploreCount(int count) {
    //    txtExploreCount.text = count.ToString();
    //}


    //private void UpdateDisplayUncurseCount(int count) {
    //    txtUncurseCount.text = count.ToString();
    //}
}