using UnityEngine;
using R3;
using ObservableCollections;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;
using System;

public class PlayerInventoryManager : AbstractSingleton<PlayerInventoryManager> {

    public ObservableList<BackPackInItem> PlayerBackPackItemList = new();
    private CancellationTokenSource cts;

    [SerializeField] private BackPackItemGenerator backPackItemGenerator;
    [SerializeField] private Text txtCurrentInventorySize;
    [SerializeField] private Text txtMaxInventorySize;
    [SerializeField] private Text txtMaxHp;

    //[SerializeField] private Image imgExclamationIcon;

    //[SerializeField] private Image[] imgOrbIcons;
    //[SerializeField] private ShinyEffectForUGUI[] shinyEffects;
    [SerializeField] private Text[] txtStutsValues;
    //[SerializeField] private Text txtTotalStutsValue;
    ///[SerializeField] private Text txtEnchantPoint;
    [SerializeField] private Text txtParry;
    [SerializeField] private Text txtAbsorb;
    [SerializeField] private Text txtReflect;
    [SerializeField] private Text txtSettle;

    [SerializeField] private int[] enhanceRates;

    private int orbCount;

    private Transform playerBackPackItemTran;
    private CompositeDisposable disposables;

    private int minRecoveryDurabilityValue = 1;
    private int maxRecoveryDurabilityValue = 3;


    protected override void Awake() {
        base.Awake();

        //cts = new ();

        //// ReactiveCollection の変更を監視し、アイテムが追加または削除された時に処理を行う
        //PlayerBackPackItemList.ObserveAdd().Subscribe(addEvent => {
        //    // アイテムが追加された時の処理
        //    addEvent.Value.SetUpBackPackItem(addEvent.Value.ItemData.Value, cts.Token);            
        //});

        //PlayerBackPackItemList.ObserveRemove().Subscribe(removeEvent => {
        //    // アイテムが削除された時の処理
        //    removeEvent.Value.Release(); // 必要に応じてリソースを解放
        //});

        //InitOrbIcons();
    }

    private void Start() {
        // デバッグ用。Awake でやると BattleManager のインスタンスがなくて null エラーになる 
        //AddItem(BackPackInItem_1);
        //AddItem(BackPackInItem_2);
    }


    public void Setup(Transform playerBackPackItemTran) {
        backPackItemGenerator.InitObjectPool();

        disposables = new CompositeDisposable();

        // ReactiveCollection の変更を監視し、アイテムが追加または削除された時に処理を行う
        PlayerBackPackItemList.ObserveAdd().Subscribe(addEvent => {
            // アイテムが追加された時の処理
            GameData.instance.charaStatus.CalculateCharaStatus(addEvent.Value.itemData);

            // リアクション表示更新
            UpdateDisplayReactionsParam();
            //addEvent.Value.SetUpBackPackItem(addEvent.Value.ItemData.Value, BattleManager.instance.Cts.Token);
        }).AddTo(disposables);

        PlayerBackPackItemList.ObserveRemove().Subscribe(removeEvent => {
            //RemoveItem(removeEvent.Value);

            // アイテムが削除された時の処理

            // リアクション表示更新
            UpdateDisplayReactionsParam();

            //removeEvent.Value.Release(); // 必要に応じてリソースを解放
        }).AddTo(disposables);

        // サイズ変更時の購読処理
        PlayerBackPackItemList.ObserveCountChanged().Subscribe(count => { txtCurrentInventorySize.text = count.ToString(); }).AddTo(disposables);

        this.playerBackPackItemTran = playerBackPackItemTran;

        GenerateBackPackInItem();

        GameData.instance.charaStatus.MaxHp.Subscribe(maxHp => txtMaxHp.text = maxHp.ToString()).AddTo(this);

        GameData.instance.playerCombatData.MaxInventorySize.Subscribe(inventorySize => txtMaxInventorySize.text = inventorySize.ToString()).AddTo(this);

        for (int i = 0; i < txtStutsValues.Length; i++) {
            int index = i;
            GameData.instance.charaStatus.statusValueList[index].statusValue.Subscribe(value => txtStutsValues[index].text = value.ToString());
        }

        // 複数の ReactiveProperty を１つに結合する。その際にtupleにしておくことで、どの ReactiveProperty が発生元が知ることが可能
        Observable<(StatusValue StatusValue, int Value)> mergedObservable = GameData.instance.charaStatus.statusValueList
            .Select(statusValue => statusValue.statusValue
                .Select(value => (StatusValue: statusValue, Value: value)))
            .Merge();

        // 結合したいずれかの ReactiveProperty に変更があった場合、処理を行う機能を購読する
        //mergedObservable.Subscribe(result => {
        //    txtTotalStutsValue.text = $"{GameData.instance.GetTotalStatusValues():N0}";

        //    GameData.instance.EnchantPoint.Value = GameData.instance.GetTotalStatusValues() - GameData.instance.consumeEnchantPoint; ;

        //    // 変更があった際の処理
        //    DebugLogger.Log($"{result.StatusValue.statusType} changed to: {result.Value}");
        //}).AddTo(this);


        //GameData.instance.EnchantPoint.Subscribe(point => txtEnchantPoint.text = $"{point:N0}").AddTo(this);
    }

    /// <summary>
    /// ポーチ内のアイテム生成
    /// </summary>
    public void GenerateBackPackInItem() {
        // 所持しているアイテムの生成
        for (int i = 0; i < GameData.instance.userData.equipItemList.Count; i++) {
            BackPackInItem backPackInItem = (BackPackInItem)backPackItemGenerator.GetObjectFromPool(playerBackPackItemTran);
            ItemData itemData = DataBaseManager.instance.GetItemData(GameData.instance.userData.equipItemList[i]);
            backPackInItem.SetUpBackPackItem(itemData, BattleManager.instance.Cts.Token, EntityType.Player);
            AddItem(backPackInItem);
        }
    }

    public void AddItem(BackPackInItem item) {
        PlayerBackPackItemList.Add(item);
    }

    public void AddItemDataConvertBackPackInItem(ItemData itemData, bool isAutoEnhance = false) {
        // 所持しているアイテムの場合、強化する
        if (PlayerBackPackItemList.ToList().TryGetFirstOrDefault(data => data.itemData.id == itemData.id, out BackPackInItem backPackInItem)) {

            // 強化できない場合には何もしない
            if (backPackInItem.EnhanceLevel.Value >= 200) {
                DebugLogger.Log("強化できません");
                return;
            }

            // 強化値を算出して強化
            (ItemData enhanceItemData, int enhanceCount) = CaluEnhanceItemData(itemData, -1);
            if (enhanceItemData != null) {
                DebugLogger.Log($"強化 : {enhanceItemData} / {enhanceCount}");
                backPackInItem.UpdateBackPackItem(enhanceItemData, enhanceCount, BattleManager.instance.Cts.Token);

                // リアクション表示更新
                UpdateDisplayReactionsParam();
            }
            return;
        }

        // 新規アイテムの場合には追加
        backPackInItem = (BackPackInItem)backPackItemGenerator.GetObjectFromPool(playerBackPackItemTran);
        backPackInItem.SetUpBackPackItem(itemData, BattleManager.instance.Cts.Token, EntityType.Player);
        PlayerBackPackItemList.Add(backPackInItem);

        GameData.instance.userData.equipItemList.Add(itemData.id);
        GameData.instance.getItemNoList.Add(itemData.id);

        // 宝箱から入手した場合には必ず強化する
        if (isAutoEnhance) {
            AddItemDataConvertBackPackInItem(itemData, false);
            DebugLogger.Log("宝箱入手時のランダム強化");
            return;
        }

        // 入手時のランダム強化
        if (UnityEngine.Random.Range(0, 100) < 50) {
            return;
        }

        AddItemDataConvertBackPackInItem(itemData, false);
        DebugLogger.Log("初回入手時のランダム強化");

        //Debug.Log(itemData.id);
    }


    public (ItemData, int) CaluEnhanceItemData(ItemData baseItemData, int enhanceCount = -1) {
        if (enhanceCount == 0) {
            return (null, 0);
        }

        // 強化回数がランダムの場合
        if (enhanceCount == -1) {
            // 何回強化するかを設定する
            int totalRate = enhanceRates.Sum();
            int randomValue = UnityEngine.Random.Range(0, totalRate);
            int index = 0;
            for (int i = 0; i < enhanceRates.Length; i++) {
                if (randomValue < enhanceRates[i]) {
                    index = i;
                    break;
                }
                randomValue -= enhanceRates[i];
            }
            enhanceCount = index + 1;
        }
        DebugLogger.Log($"enhanceCount : {enhanceCount}");
        ItemData enhanceItemData = new();
        enhanceItemData.statusTypes = new StatusType[baseItemData.statusTypes.Length];
        enhanceItemData.requiredValues = new int[baseItemData.requiredValues.Length];

        enhanceItemData.effectType = baseItemData.effectType;

        for (int i = 0; i < enhanceCount; i++) {
            // 50% の確率で強化
            enhanceItemData.coolTime += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.01f, 0.03f) : 0;
            enhanceItemData.accuracy += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;
            enhanceItemData.criticalRate += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;

            // 耐久値と価値は必ず加算
            enhanceItemData.durability += baseItemData.durability;
            enhanceItemData.price += baseItemData.price / 2;

            // TODO 能力値の修正　最初から設定されているもののみ上げているが、加算ステータスをランダムにするか検討する
            for (int j = 0; j < enhanceItemData.statusTypes.Length; j++) {
                // 加算値
                enhanceItemData.requiredValues[j] += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(1, 4) : 0;
                
                // ここを変えるか
                enhanceItemData.statusTypes[j] = baseItemData.statusTypes[j];
                DebugLogger.Log($"{enhanceItemData.statusTypes[j]} : {baseItemData.requiredValues[j]}");
            }

            // Hp とリアクション か Value を強化
            if (baseItemData.effectType == EffectType.Passive) {
                enhanceItemData.hpBonus += (int)(baseItemData.hpBonus * 0.5f);

                // ベース値のあるものは、半分加算
                enhanceItemData.reflectionRate += baseItemData.reflectionRate * 0.5f;
                enhanceItemData.parryRate += baseItemData.parryRate * 0.5f;
                enhanceItemData.absorptionRate += baseItemData.absorptionRate * 0.5f;
                enhanceItemData.settlementRate += baseItemData.settlementRate * 0.5f;

                // 0 のリアクションも上げる対象とする
                enhanceItemData.reflectionRate += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;
                enhanceItemData.parryRate += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;
                enhanceItemData.absorptionRate += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;
                enhanceItemData.settlementRate += UnityEngine.Random.Range(0, 100) < 50 ? UnityEngine.Random.Range(0.1f, 0.3f) : 0;
            } else {
                enhanceItemData.minValue += UnityEngine.Random.Range(0, 100) < 50 ? 1 : 0;
                enhanceItemData.maxValue += UnityEngine.Random.Range(0, 100) < 50 ? 1 : 0;
            }
        }
        DebugLogger.Log($"enhanceItemData.coolTime : {enhanceItemData.coolTime}");
        DebugLogger.Log($"enhanceItemData.accuracy : {enhanceItemData.accuracy}");
        DebugLogger.Log($"enhanceItemData.minValue : {enhanceItemData.minValue}");
        DebugLogger.Log($"enhanceItemData.maxValue : {enhanceItemData.maxValue}");
        DebugLogger.Log($"enhanceItemData.criticalRate : {enhanceItemData.criticalRate}");
        DebugLogger.Log($"enhanceItemData.durability : {enhanceItemData.durability}");
        DebugLogger.Log($"enhanceItemData.hpBonus : {enhanceItemData.hpBonus}");
        DebugLogger.Log($"enhanceItemData.price : {enhanceItemData.price}");
        DebugLogger.Log($"enhanceItemData.reflectionRate : {enhanceItemData.reflectionRate}");
        DebugLogger.Log($"enhanceItemData.parryRate : {enhanceItemData.parryRate}");
        DebugLogger.Log($"enhanceItemData.absorptionRate : {enhanceItemData.absorptionRate}");
        DebugLogger.Log($"enhanceItemData.settlementRate : {enhanceItemData.settlementRate}");

        return (enhanceItemData, enhanceCount);
    }

    public void RemoveItem(BackPackInItem item) {
        PlayerBackPackItemList.Remove(item);
        GameData.instance.userData.equipItemList.Remove(item.itemData.id);
        DebugLogger.Log($"Release : {item.itemData.itemName}");
    }


    public BackPackInItem GetBackPackInItem(Transform backPackItemTran) {
        BackPackInItem backPackInItem = (BackPackInItem)backPackItemGenerator.GetObjectFromPool(backPackItemTran);
        return backPackInItem;
    }

    private void OnDestroy() {
        disposables?.Clear();
    }


    //public void SetOrbIcon(OrbType orbType) {
    //    // オーブを UI に表示、光らせる
    //    imgOrbIcons[orbCount].sprite = DataBaseManager.instance.orbDataSO.orbDatasList[(int)orbType].spriteOrb;
    //    imgOrbIcons[orbCount].enabled = true;
    //    shinyEffects[orbCount].Play(0.75f);

    //    orbCount++;
    //    orbCount = orbCount % imgOrbIcons.Length;

    //    if (orbCount == 0) {
    //        FlashIconsAsync().Forget();
    //    }
    //}


    //private async UniTask FlashIconsAsync() {
    //    await UniTask.Delay(1000);

    //    for (int i = 0; i < imgOrbIcons.Length; i++) {
    //        shinyEffects[i].Play();
    //    }

    //    await UniTask.Delay(1000);

    //    // すべてのオーブを UI から消す
    //    InitOrbIcons();
    //    GameData.instance.ClearOrbList();
    //}

    //public void InitOrbIcons() {
    //    for (int i = 0; i < imgOrbIcons.Length; i++) {
    //        imgOrbIcons[i].enabled = false;
    //    }
    //}

    //public void FindTreasureAnim(){
    //    imgExclamationIcon.transform.localScale = Vector3.zero;
    //    imgExclamationIcon.enabled = true;

    //    Sequence sequence = DOTween.Sequence();
    //    sequence.SetLink(gameObject);
    //    sequence.Append(imgExclamationIcon.transform.DOScale(Vector3.one * 1.25f, 0.25f).SetEase(Ease.InQuart));
    //    sequence.Append(imgExclamationIcon.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack));
    //    sequence.AppendInterval(0.25f).OnComplete(() => imgExclamationIcon.enabled = false);
    //}

    /// <summary>
    /// 対象のアイテムを所持しているか判定
    /// 所持していれば true
    /// </summary>
    /// <param name="searchItemId"></param>
    /// <returns></returns>
    public bool HaveTargetItem(int searchItemId) {
        return PlayerBackPackItemList.ToList().FirstOrDefault(data => data.itemData.id == searchItemId);
    }

    /// <summary>
    /// ランダムで選択されたアイテムの耐久力を回復
    /// 足踏みした場合
    /// </summary>
    public void RandomRecoveryDurability() {
        if (PlayerBackPackItemList.Count == 0) {
            DebugLogger.Log("インベントリーにアイテムなし");
            return;
        }

        int index = UnityEngine.Random.Range(0, PlayerBackPackItemList.Count);
        BackPackInItem item = PlayerBackPackItemList[index];

        int recoveryPoint = UnityEngine.Random.Range(minRecoveryDurabilityValue, maxRecoveryDurabilityValue);
        item.itemData.durability += recoveryPoint;

        item.UpdateDurabilityDisplay(item.itemData.durability);
    }

    /// <summary>
    /// 所持しているアイテム内の交渉成功率の合計値を戻す
    /// </summary>
    /// <returns></returns>
    public float GetSettlementRate() {
        return PlayerBackPackItemList.Select(data => data.itemData.settlementRate).Sum();
    }

    /// <summary>
    /// 所持しているアイテム内の受け流し(パリィ)成功率の合計値を戻す
    /// </summary>
    /// <returns></returns>
    public float GetParryRate() {
        return PlayerBackPackItemList.Select(data => data.itemData.parryRate).Sum();
    }

    /// <summary>
    /// 所持しているアイテム内のダメージ反射の成功率の合計値を戻す
    /// </summary>
    /// <returns></returns>
    public float GetReflectionRate() {
        return PlayerBackPackItemList.Select(data => data.itemData.reflectionRate).Sum();
    }

    /// <summary>
    /// 所持しているアイテム内のダメージ吸収の成功率の合計値を戻す
    /// </summary>
    /// <returns></returns>
    public float GetAbsorptionRate() {
        return PlayerBackPackItemList.Select(data => data.itemData.absorptionRate).Sum();
    }

    /// <summary>
    /// リアクション数値の画面反映
    /// </summary>
    public void UpdateDisplayReactionsParam() {
        float itemTotalParryRate = GetParryRate();
        float bonusParryRate = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Strength);
        txtParry.text = (itemTotalParryRate + bonusParryRate).ToString("F2");

        float itemTotalAbsorbRate = GetAbsorptionRate();
        float bonusAbsorbRate = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Intelligence);
        txtAbsorb.text = (itemTotalAbsorbRate + bonusAbsorbRate).ToString("F2");

        float itemTotalReflectRate = GetReflectionRate();
        float bonusReflectRate = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Dexterity);
        txtReflect.text = (itemTotalReflectRate + bonusReflectRate).ToString("F2");

        float itemTotalSettleRate = GetSettlementRate();
        float bonusSettleRate = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Charm);
        txtSettle.text = (itemTotalSettleRate + bonusSettleRate).ToString("F2");
    }

    private void OnDestory() {
        disposables?.Clear();
    }

    // 他のインベントリ関連のメソッドを追加

}