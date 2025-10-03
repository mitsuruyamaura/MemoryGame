using UnityEngine;
using R3;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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
    [SerializeField] private Text txtInventoryMaxInfo;
    [SerializeField] private Text txtRestartMessage;
    [SerializeField] private Text txtSettlementInfo;    // 画面優先順位の関係上、Canvas_ItemInfo オブジェクト内にあるオブジェクトを使う

    [SerializeField] private Slider sliderHp;

    [SerializeField] private Button btnPlayerLevel;

    [SerializeField] private Transform canvasTran;
    [SerializeField] private Image enemyIcon;

    [SerializeField] private GameObject timeObj;
    [SerializeField] private Image imgTime;
    [SerializeField] private Image imgBattleState;
    [SerializeField] private Sprite[] stateSprites;    // Win,Loose,Timeout の順番
    [SerializeField] private Image inventoryFilter;

    [SerializeField] private Color releaseMessageColor;
    [SerializeField] private CircleOutline circleOutline;

    private string SuccessSettlementMessage = "平和的解決に成功しました!!";
    private float defaultTime;

    public Transform playerBackPackItemTran;
    public Transform enemyBackPackItemTran;

    private float sliderAnimeDuration = 0.5f;
    private int prevStamina;
    private int maxHp;


    public void SetupStageUIManager(int stamina, int maxHp) {
        SetMaxHpDisplay(maxHp);

        //UpdateDisplayPlayerLevel();

        defaultTime = BattleManager.instance.BattleDuration.Value;

        // TODO スライダーの初期化と、最大値のセット
        BattleManager.instance.BattleDuration.Subscribe(time => 
        {
            float fil = time / defaultTime;
            imgTime.DOFillAmount(fil, fil).SetEase(Ease.Linear).SetLink(gameObject);
        }).AddTo(this);

        BattleManager.instance.OnBattleStart.Subscribe(_ => ShowTimeCanvas()).AddTo(this);
        BattleManager.instance.OnBattleEnd.Subscribe(resultType => {
            HideTimeCanvas();
            ShowBattleState(resultType);
        }).AddTo(this);

        //SetUpActionResultDisplays();

        HideTimeCanvas();
        HideBattleState(null);

        //txtWaveInfo.DOFade(0, 0).SetLink(gameObject);
        txtInventoryMaxInfo.DOFade(0, 0).SetLink(gameObject);

        GameData.instance.userData.SoulPoint
            .Zip(GameData.instance.userData.SoulPoint.Skip(1), (prevPoint, nextPoint) => (prevPoint, nextPoint))
            .Subscribe(soulPoint => UpdateSoulPoint(soulPoint.prevPoint, soulPoint.nextPoint))
            .AddTo(this);

        // ペアのコンボ回数の購読
        GameData.instance.ComboPairCount.Subscribe(comboCount => UpdateDisplayComboPairCount(comboCount)).AddTo(this);
    }

    /// <summary>
    /// OnEndBattle より購読
    /// </summary>
    /// <param name="battleResultType"></param>
    public void ShowBattleState(BattleResultType battleResultType) {
        imgBattleState.sprite = stateSprites[(int)battleResultType];
        imgBattleState.gameObject.SetActive(true);

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
    }


    private void HideTimeCanvas() {
        timeObj.SetActive(false);
    }

    public void SetBossBattleTime(float bossTime) {
        defaultTime = bossTime;
    }

    public void SetStaminaDisplay(int stamina) {
        prevStamina = stamina;
    }

    public void SetMaxHpDisplay(int maxHp) {
        txtHp.text = maxHp.ToString();
        txtMaxHp.text = maxHp.ToString();
        sliderHp.maxValue = maxHp;
        sliderHp.value = maxHp;

        this.maxHp = maxHp;
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

    // 敵側も用意する


    /// <summary>
    /// プレイヤーレベルの表示更新
    /// </summary>
    public void UpdateDisplayPlayerLevel() {
        // プレイヤーレベルの表示更新
        txtPlayerLevel.text = GameData.instance.charaStatus.level.ToString();
    }

    /// <summary>
    /// 経験値の表示更新
    /// </summary>
    /// <param name="isSliderOn"></param>
    public void UpdateDisplayExp(bool isSliderOn) {
        // 現在/目標経験値の表示更新

        // ゲージ更新
        //sliderExp.DOValue((float)GameData.instance.totalExp / DataBaseManager.instance.CalcNextLevelExp(GameData.instance.playerLevel - 1), 1.0f).SetEase(Ease.Linear);

    }


    public void UpdatePlayerShieldHp(int shield) {
        txtShieldHp.text = shield.ToString();
    }

    public void UpdateWaveNo(int waveNo) {
        txtWaveNo.text = waveNo.ToString();
    }


    private void SetUpActionResultDisplays() {
        GameData.instance.userData.DefeatedEnemyCount.Subscribe(count => UpdateDisplayDefeatedEnemyCount(count)).AddTo(this);
        GameData.instance.userData.FindTreasureCount.Subscribe(count => UpdateDisplayFindTreasureCount(count)).AddTo(this);
        GameData.instance.userData.ExploreCount.Subscribe(count => UpdateDisplayExploreCount(count)).AddTo(this);
        GameData.instance.userData.UncurseCount.Subscribe(count => UpdateDisplayUncurseCount(count)).AddTo(this);
    }

    private void UpdateDisplayDefeatedEnemyCount(int count) {
        txtDefeatedEnemyCount.text = count.ToString();
    }

    private void UpdateDisplayFindTreasureCount(int count) {
        txtFindTreasureCount.text = count.ToString();
    }

    private void UpdateDisplayExploreCount(int count) {
        txtExploreCount.text = count.ToString();
    }


    private void UpdateDisplayUncurseCount(int count) {
        txtUncurseCount.text = count.ToString();
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
    /// ポーチが Max の際の Info 表示
    /// </summary>
    public void InventoryMaxInfo() {
        // 点滅させて表示
        Sequence sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);
        sequence.Append(txtInventoryMaxInfo.DOFade(1.0f, 1.0f).SetEase(Ease.Linear)).SetLoops(2, LoopType.Yoyo);
        sequence.Append(txtInventoryMaxInfo.DOFade(0f, 0.5f).SetEase(Ease.Linear)).OnComplete(() => txtInventoryMaxInfo.DOFade(0f, 0f));  // 消えないことがあるので念のため
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
    /// ゲームオーバー、クリア時に表示する、タイトルへ戻すためのメッセージ
    /// </summary>
    public void ShowRestartMessage() {
        txtRestartMessage.gameObject.SetActive(true);
    }
}