using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using System;
using R3;

public enum ReleaseType {
    Destroy,        // 耐久値 0 で破壊された場合
    SelfRelease,    // 自分でポーチから破棄した場合
    Distribute,     // 供物として捧げた場合
}


public class BackPackInItem : PoolBase {

    public Image imgItemIcon;
    public Image imgIconGauge;
    [SerializeField] private Image imgEnhanceFrame;
    [SerializeField] private Image imgReleaseIcon;
    [SerializeField] private Text txtEnhanceLevel;
    [SerializeField] private Text txtDurability;
    [SerializeField] private Button btnDisuse;       // 使用停止
    [SerializeField] private Button btnRelease;      // リリース
    [SerializeField] private CanvasGroup canvasGroupShade;

    public ItemData itemData;
    public ReactiveProperty<int> EnhanceLevel = new(0);

    public float currentCoolTime;
    public float currentAccuracy;
    public int currentMinDamage;
    public int currentMaxDamage;
    public int currentMinAttackCount;
    public int currentMaxAttackCount;

    private Tweener tweener;
    private Subject<Unit> onCancel = new Subject<Unit>();
    public Observable<Unit> OnCancel => onCancel;

    private IDisposable disuseButtonSubscribe;
    private IDisposable releaseButtonSubscribe;

    private IDisposable battleStartDisposable;
    private IDisposable enhanceLevelDispose;
    private IDisposable battleEndDisposable;
    private IDisposable successSettlementDisposable;

    public EntityType entityType;

    private BattleResultType battleResultType = BattleResultType.Ready;
    private bool isDisuse = false;
    private int prevDurability = 0;


    /// <summary>
    /// 初期設定
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="token"></param>
    /// <param name="entityType"></param>
    public void SetUpBackPackItem(ItemData itemData, CancellationToken token, EntityType entityType = EntityType.Player) {
        // 新しいインスタンスで ItemData のコピーを使う(参照すると、全 ItemData が変わってしまうため)
        this.itemData = new(itemData);

        this.entityType = entityType;
        battleResultType = BattleResultType.Ready;
        imgItemIcon.color = Color.white;
        imgIconGauge.fillAmount = 1.0f;

        prevDurability = 0;
        isReleased = false;

        Sprite itemIconSprite = DataBaseManager.instance.GetItemIcon(this.itemData.id);
        imgItemIcon.sprite = itemIconSprite;

        //Debug.Log("SetupBackPackItem");
        float scale = transform.localScale.x;
        UpdateEnhanceLevelDisplay(0);

        transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localScale = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetLink(gameObject);

        battleStartDisposable?.Dispose();

        // バトル開始の購読
        battleStartDisposable = BattleManager.instance.OnBattleStart
            .Subscribe(tuple => {
                var (cts, resultType) = tuple;
                ExecuteBackPackItem(this.itemData, cts.Token, entityType, resultType).Forget();
            });

        EnhanceLevel.Value = 0;
        enhanceLevelDispose = EnhanceLevel.Subscribe(level => UpdateEnhanceLevelDisplay(level));

        battleEndDisposable = BattleManager.instance.OnBattleEnd
            .Subscribe(async battleResult => {
                battleResultType = battleResult;

                // プレイヤーの際、バトルで1回も消費していない場合、あるいはパッシブの場合には耐久値を1回分だけ減らす
                // 必ず this で、新しく作成した ItemData を参照する
                if (entityType == EntityType.Player &&
                    (prevDurability == this.itemData.durability || this.itemData.effectType == EffectType.Passive)) {
                    this.itemData.durability--;
                    if (this.itemData.durability <= 0) {
                        UpdateDurabilityDisplay(0);
                        AddPriceToMoney(ReleaseType.Destroy);  // 0 だが一応
                        Release();
                    } else {
                        UpdateDurabilityDisplay(this.itemData.durability);
                    }
                    DebugLogger.Log(this.itemData.durability);
                }

                // 穢れのコンディションの場合に、1回ずつ減らす


                tweener?.Kill();
                int attackInterval = Mathf.CeilToInt(currentCoolTime * 1000); // 1000 でスケールして切り上げ
                await UniTask.Delay(attackInterval, cancellationToken: token).SuppressCancellationThrow();

                if (this == null || imgIconGauge == null) return;
                tweener = imgIconGauge.DOFillAmount(1.0f, 0.2f).SetEase(Ease.Linear).SetLink(gameObject);
            });

        // 耐久値表示更新
        if (entityType == EntityType.Player) {
            UpdateDurabilityDisplay(this.itemData.durability);
            //UpdateDurabilityDisplay(-1);  // 耐久力を減算しない場合はこちらにする
        }

        // 使用停止のシェードの設定
        isDisuse = false;
        canvasGroupShade.gameObject.SetActive(isDisuse);

        // 使用停止ボタンの設定
        disuseButtonSubscribe = btnDisuse.OnClickAsObservable()
            .Where(_ => GameData.instance.CurrentGameState.Value == GameData.GameState.Play)
            .ThrottleFirst(TimeSpan.FromSeconds(0.25f))
            .Subscribe(_ => ChangeDisuseshade());

        // リリースボタンの設定
        releaseButtonSubscribe = btnRelease.OnClickAsObservable()
            .Where(_ => GameData.instance.CurrentGameState.Value == GameData.GameState.Play)
            .ThrottleFirst(TimeSpan.FromSeconds(1.0f))
            .Subscribe(_ => {
                AddPriceToMoney(ReleaseType.SelfRelease);
                Release();
            });

        // 画像の透明部分はタップ感知しない設定にする
        imgReleaseIcon.alphaHitTestMinimumThreshold = 1;

        // 交渉成功時
        successSettlementDisposable = BattleManager.instance.OnSuccessSettlement
            .Subscribe(_ => {
                // パッシブの場合には耐久値を1回分だけ減らす
                // 必ず this で、新しく作成した ItemData を参照する
                if (entityType == EntityType.Player && this.itemData.effectType == EffectType.Passive) {
                    this.itemData.durability--;
                    if (this.itemData.durability <= 0) {
                        UpdateDurabilityDisplay(0);
                        AddPriceToMoney(ReleaseType.Destroy);  // 0 だが一応
                        Release();
                    } else {
                        UpdateDurabilityDisplay(this.itemData.durability);
                    }
                    DebugLogger.Log(this.itemData.durability);
                }
            });
    }

    /// <summary>
    /// アイテムの利用状態を変更し、状態に応じてシェードをかける
    /// </summary>
    private void ChangeDisuseshade() {
        isDisuse = !isDisuse;
        canvasGroupShade.gameObject.SetActive(isDisuse);
        //DebugLogger.Log(isDisuse);
    }

    /// <summary>
    /// アイテムデータ更新
    /// 同じアイテムの強化を行った場合など
    /// </summary>
    /// <param name="enhanceItemData"></param>
    /// <param name="enhanceCount"></param>
    /// <param name="token"></param>
    public void UpdateBackPackItem(ItemData enhanceItemData, int enhanceCount, CancellationToken token) {
        // 強化
        itemData.coolTime = Mathf.Max(itemData.coolTime - enhanceItemData.coolTime, 0.02f);
        itemData.accuracy = Mathf.Min(itemData.accuracy + enhanceItemData.accuracy, 150);
        itemData.minValue = Mathf.Min(itemData.minValue + enhanceItemData.minValue, 100);
        itemData.maxValue = Mathf.Min(itemData.maxValue + enhanceItemData.maxValue, 100);

        itemData.criticalRate = Mathf.Min(itemData.criticalRate + enhanceItemData.criticalRate, 150);
        itemData.durability = Mathf.Min(itemData.durability + enhanceItemData.durability, 200);
        itemData.price = Mathf.Min(itemData.price + enhanceItemData.price, 100000);
        itemData.hpBonus = Mathf.Min(itemData.hpBonus + enhanceItemData.hpBonus, 100000);

        itemData.reflectionRate = Mathf.Min(itemData.reflectionRate + enhanceItemData.reflectionRate, 150);
        itemData.parryRate = Mathf.Min(itemData.parryRate + enhanceItemData.parryRate, 150);
        itemData.absorptionRate = Mathf.Min(itemData.absorptionRate + enhanceItemData.absorptionRate, 150);
        itemData.settlementRate = Mathf.Min(itemData.settlementRate + enhanceItemData.settlementRate, 150);

        // ステータスの強化
        DebugLogger.Log(enhanceItemData.statusTypes.Length);
        for (int i = 0; i < enhanceItemData.statusTypes.Length; i++) {
            itemData.requiredValues[i] += enhanceItemData.requiredValues[i];
        }

        // 強化分をステータスに加算
        GameData.instance.charaStatus.CalculateCharaStatus(enhanceItemData);

        // Hp だけ加算の場合
        //if (enhanceItemData.effectType == EffectType.Passive) {
        //    GameData.instance.charaStatus.CalculateMaxHp(enhanceItemData.hpBonus);
        //    Debug.Log($"hpBonus : {enhanceItemData.hpBonus}");
        //}

        // 強化回数を加算 = 表示更新の購読処理が動く
        EnhanceLevel.Value += enhanceCount;
        DebugLogger.Log($"enhance level : {EnhanceLevel.Value} / durability : {itemData.durability} / enhance count : {enhanceCount}");

        battleStartDisposable?.Dispose();

        // バトル開始の再購読
        battleStartDisposable = BattleManager.instance.OnBattleStart
            .Subscribe(tuple => {
                var (cts, resultType) = tuple;
                ExecuteBackPackItem(itemData, cts.Token, entityType, resultType).Forget();
            });

        UpdateDurabilityDisplay(itemData.durability);
        DebugLogger.Log("強化");
    }


    private void UpdateEnhanceLevelDisplay(int level) {
        if(level == 0) {
            txtEnhanceLevel.text = string.Empty;
            imgEnhanceFrame.gameObject.SetActive(false);
            return; 
        }

        txtEnhanceLevel.text = $"+{level}";
        imgEnhanceFrame.gameObject.SetActive(true);
    }

    public void UpdateDurabilityDisplay(int durability) {
        if (durability == -1) {
            txtDurability.text = string.Empty;
        }
        else {
            if (this == null || txtDurability == null) return;
            txtDurability.text = durability.ToString();
        }        
    }

    /// <summary>
    /// 自動バックパック読み込み開始
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="token"></param>
    /// <param name="myEntityType"></param>
    /// <returns></returns>
    public async UniTask ExecuteBackPackItem(ItemData itemData, CancellationToken token, EntityType myEntityType, BattleResultType battleResultType) {
        DebugLogger.Log("ExecuteBackPackItem");
        //// 引数の token を使ってキャンセル可能にするため、CancellationTokenSource を生成
        //CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        //// btnStop.OnClickAsObservable を購読し、ボタン押下時に cts を利用して UniTask をキャンセル
        //stopButtonSubscription = btnStop.OnClickAsObservable()
        //    .ThrottleFirst(System.TimeSpan.FromSeconds(1.0f))
        //    .Subscribe(_ => cts.Cancel());

        //Debug.Log(itemData);

        if (isDisuse) {
            DebugLogger.Log("使用停止中なので動かない");
            return;
        }

        // パッシブの場合、バトル後に1回分減らすので処理しない
        //if (itemData.effectType == EffectType.Passive) {
        //    return;
        //}

        // 直前の耐久値を保持
        prevDurability = itemData.durability;

        // デバッグ用
        if (itemData == null) {
            currentCoolTime = UnityEngine.Random.Range(1.0f, 2.0f);
            currentAccuracy = UnityEngine.Random.Range(50.0f, 100.0f);
            currentMinDamage = GetRandomValue(1, 4);
            currentMaxDamage = GetRandomValue(1, 4);
            currentMinAttackCount = 1;
            currentMaxAttackCount = GetRandomValue(1, 4);
            DebugLogger.Log($"デバッグ用 : クールタイム{currentCoolTime}");
        } else {
            // バフ計算

            // エンハンス計算


            currentCoolTime = itemData.coolTime;
            currentAccuracy = itemData.accuracy;
            currentMinDamage = itemData.minValue;
            currentMaxDamage = itemData.maxValue;
            currentMinAttackCount = itemData.minAttackCount;
            currentMaxAttackCount = itemData.maxAttackCount;
        }

        this.battleResultType = battleResultType;//BattleResultType.Battle;

        // token = BattleManager.instance.Cts.Token;


        try {
            // バトルがキャンセルされていない間、あるいは制限時間が来たら
            while (battleResultType == BattleResultType.Battle || !token.IsCancellationRequested) {

                if (entityType == EntityType.Player && itemData.durability <= 0) {
                    break;
                }

                // 例えば、特定の行動回数のときだけ処理変わる、など (count == 0 なら fillAmount = 0.5f スタートなど。)

                if (this == null || imgIconGauge == null) return;
                imgIconGauge.fillAmount = 1.0f;

                // バフ、デバフを適用して現在値を算出

                // 
                tweener = null;
                tweener = imgIconGauge.DOFillAmount(0f, currentCoolTime).SetEase(Ease.Linear).SetLink(gameObject);

                // キャンセル時の処理を登録(このスコープ内でキャンセルされた場合に、この処理が実行される)
                using (token.Register(() => {
                    tweener?.Kill();
                    imgIconGauge.fillAmount = 1.0f;
                })) {
                    // 攻撃までの待機時間
                    // 小数点以下も含めた精度を保つために、適切なスケーリングを行う
                    int attackInterval = Mathf.CeilToInt(currentCoolTime * 1000); // 1000 でスケールして切り上げ
                    //Debug.Log(attackInterval);
                    await UniTask.Delay(attackInterval, cancellationToken: token);
                }

                await UniTask.Yield(cancellationToken: token);

                // 攻撃回数分だけ、行動を処理
                int currentAttackCount = GetRandomValue(currentMinAttackCount, currentMaxAttackCount);

                // 耐久値以内に制限
                if (entityType == EntityType.Player) {
                    currentAttackCount = Mathf.Min(currentAttackCount, itemData.durability);
                }
                DebugLogger.Log($"攻撃回数 : {currentAttackCount}");

                for (int i = 0; i < currentAttackCount; i++) {
                    // 行動内容
                    switch (itemData.effectType) {
                        case EffectType.Physical:
                        case EffectType.Magic:
                            AutoAttack(myEntityType);
                            break;
                        case EffectType.Shield:
                            AutoShield(myEntityType);
                            break;
                        case EffectType.Heal:
                            AutoHeal(myEntityType);
                            break;
                        case EffectType.Passive:
                            // 効果は適用中なので、何もしないで抜ける
                            break;
                    }
                    // 耐久値を減算
                    if (entityType == EntityType.Player) {
                        itemData.durability--;
                        if (itemData.durability <= 0) {
                            UpdateDurabilityDisplay(0);
                            AddPriceToMoney(ReleaseType.Destroy);  // 0 だが一応
                            Release();
                            break;
                        } else {
                            UpdateDurabilityDisplay(itemData.durability);
                            //Debug.Log(itemData.durability);
                        }
                    }

                    // for 文なのでフレーム跨がせる。そうしないと、ここですべて処理しようとして処理が一時止まる
                    await UniTask.Yield(cancellationToken: token);
                }
            }
        }
        catch (OperationCanceledException) {
            // while 文を抜けてキャンセルされた場合の処理
            //tweener?.Kill();
            //tweener = null;
            // キャンセルされたことを通知
            onCancel.OnNext(Unit.Default);
        }

        if (entityType == EntityType.Player && itemData.durability <= 0) {
            UpdateDurabilityDisplay(0);
            AddPriceToMoney(ReleaseType.Destroy);  // 0 だが一応
            Release();
        }
        //imgIconGauge.fillAmount = 1.0f;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    /// <param name="myEntityType"></param>
    private void AutoAttack(EntityType myEntityType) {
        float randomValue = UnityEngine.Random.Range(0, 100.0f);

        // TODO バフ確認　命中率半減の場合
        //float debuffAccuracy = currentAccuracy;
        //debuffAccuracy *= 0.5f;/// 小数点第2位まで


        // 必中の場合(同時にはかからない)
        //randomValue = 0;


        // 命中判定
        if (randomValue <= currentAccuracy) {  // ←　変える
            // 命中した場合の処理
            //Debug.Log($"Hit : {randomValue} %");

            // ダメージ値の設定
            int damage = GetRandomValue(currentMinDamage, currentMaxDamage);

            // クリティカル判定用のランダム値
            randomValue = UnityEngine.Random.Range(0, 100.0f);

            // 必ずクリティカルの場合
            //randomValue = 0;

            // クリティカル判定
            bool isCritical = IsCritical(randomValue, itemData.criticalRate);

            if (isCritical) {
                DebugLogger.Log("Critical");
                damage = Mathf.FloorToInt(damage * 1.5f);
            }

            // バフ確認

            // シールド無視の場合には、EffectType を変える

            // HP 吸収の場合には、最後に処理を追加

            // 攻撃力ダウンの場合にはダメージ半減



            // 対象先を設定してダメージを処理する
            if (myEntityType == EntityType.Player) {
                BattleManager.instance.UpdateEnemyHp(-damage, itemData.effectType, isCritical);

                // ボイスを再生
                int voiceIndex = UnityEngine.Random.Range(0, 2);
                SoundManager.instance.PlayVoice((VOICE_TYPE)voiceIndex);
            } else {
                // プレイヤーの場合のみ、敵からの攻撃に対してリアクション判定する

                // ①受け流し(パリィ)
                float parryRate = PlayerInventoryManager.instance.GetParryRate();
                DebugLogger.Log($"parryRate : {parryRate}");

                float parryBonus = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Strength);
                DebugLogger.Log($"parryBonus : {parryBonus}");

                float randomParryValue = UnityEngine.Random.Range(0, 100.00f);
                DebugLogger.Log($"Parry randomValue : {randomParryValue}");

                // 合計
                parryRate += parryBonus;

                // 受け流し判定
                if (randomParryValue <= parryRate) {
                    // パリィ成功
                    FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(BattleManager.instance.playerFloatingViewTran);
                    floatingView.SetColor(FloatingViewType.dodge);
                    floatingView.SetViewFontSize(FloatingViewType.dodge);
                    floatingView.UpdateText("Parry!!").Forget();

                    SoundManager.instance.PlaySE(SE_TYPE.Parry);
                } else {
                    // 失敗時は通常ダメージ処理
                    BattleManager.instance.UpdatePlayerHp(-damage, itemData.effectType, isCritical);
                    
                    // ボイスを再生
                    int voiceIndex = UnityEngine.Random.Range(5, 7);
                    SoundManager.instance.PlayVoice((VOICE_TYPE)voiceIndex);
                }

                // Hp が 0 以下ならリアクションしない
                if (BattleManager.instance.PlayerHP.Value <= 0) {
                    return;
                }

                // ②反射
                float reflectionRate = PlayerInventoryManager.instance.GetReflectionRate();
                DebugLogger.Log($"reflectionRate : {reflectionRate}");

                float reflectionBonus = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Dexterity);
                DebugLogger.Log($"reflectionBonus : {reflectionBonus}");

                float randomReflectionValue = UnityEngine.Random.Range(0, 100.00f);
                DebugLogger.Log($"Reflection randomValue : {randomReflectionValue}");

                // 合計
                reflectionRate += reflectionBonus;

                // 反射判定
                if (randomReflectionValue <= reflectionRate) {
                    // ダメージの半分を敵に反射
                    BattleManager.instance.UpdateEnemyHp(-damage / 2, itemData.effectType, false);
                    DebugLogger.Log($"反射 : {-damage / 2}");

                    // 自分のところにもフロート表示
                    FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(BattleManager.instance.playerFloatingViewTran);
                    floatingView.SetColor(FloatingViewType.reaction);
                    floatingView.SetViewFontSize(FloatingViewType.reaction);
                    floatingView.UpdateText("Reflect!!").Forget();

                    SoundManager.instance.PlaySE(SE_TYPE.Reflect);
                }

                // ③吸収
                float absorptionRate = PlayerInventoryManager.instance.GetAbsorptionRate();
                DebugLogger.Log($"absorptionRate : {absorptionRate}");

                float absorptionBonus = GameData.instance.charaStatus.GetReactionBonusRate(StatusType.Intelligence);
                DebugLogger.Log($"absorptionBonus : {absorptionBonus}");

                float randomAbsorptionValue = UnityEngine.Random.Range(0, 100.00f);
                DebugLogger.Log($"Absorption randomValue : {randomAbsorptionValue}");

                // 合計
                absorptionRate += absorptionBonus;

                // 吸収判定
                if (randomAbsorptionValue <= absorptionRate) {
                    // ダメージの半分回復
                    BattleManager.instance.UpdatePlayerHp(damage / 2, EffectType.Heal, false);
                    DebugLogger.Log($"吸収 : {damage / 2}");

                    // 追加のフロート表示
                    FloatingView floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(BattleManager.instance.playerFloatingViewTran);
                    floatingView.SetColor(FloatingViewType.heal);
                    floatingView.SetViewFontSize(FloatingViewType.heal);
                    floatingView.UpdateText("Absorb!!").Forget();

                    SoundManager.instance.PlaySE(SE_TYPE.Absorb);
                }

                // リアクションなしの場合
                //BattleManager.instance.UpdatePlayerHp(-damage, itemData.effectType, isCritical);

                // ボイスを再生
                //int voiceIndex = UnityEngine.Random.Range(5, 7);
                //SoundManager.instance.PlayVoice((VOICE_TYPE)voiceIndex);
            }
        } else {
            // 失敗した場合の処理
            DebugLogger.Log("Miss!");

            // Miss 表示
            FloatingView floatingView;
            if (myEntityType == EntityType.Player) {
                floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(BattleManager.instance.enemyFloatingViewTran);
            } else {
                floatingView = (FloatingView)FloatingViewGenerator.instance.GetObjectFromPool(BattleManager.instance.playerFloatingViewTran);
            }
            floatingView.SetColor(FloatingViewType.dodge);
            floatingView.SetViewFontSize(FloatingViewType.dodge);
            floatingView.UpdateText("Miss").Forget();

            SoundManager.instance.PlaySE(SE_TYPE.Miss);
        }
    }

    /// <summary>
    /// シールド
    /// </summary>
    /// <param name="myEntityType"></param>
    private void AutoShield(EntityType myEntityType) {
        // シールド利用不能の場合


        // シールド値の設定
        int shieldPower = GetRandomValue(currentMinDamage, currentMaxDamage);

        // クリティカル判定
        float randomValue = UnityEngine.Random.Range(0, 100.0f);
        bool isCritical = IsCritical(randomValue, itemData.criticalRate);

        if (isCritical) {
            DebugLogger.Log("Critical");
            shieldPower = Mathf.FloorToInt(shieldPower * 1.5f);
        }

        // 対象先設定
        if (myEntityType == EntityType.Player) {
            // シールド加算
            BattleManager.instance.UpdatePlayerShieldHp(shieldPower, isCritical);
        } else {
            // シールド加算
            BattleManager.instance.UpdateEnemyShieldHp(shieldPower, isCritical);
        }
        SoundManager.instance.PlaySE(SE_TYPE.Shield);
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="myEntityType"></param>
    private void AutoHeal(EntityType myEntityType) {
        // 回復不能の場合


        // ヒール値の設定
        int healPower = GetRandomValue(currentMinDamage, currentMaxDamage);

        // クリティカル判定
        float randomValue = UnityEngine.Random.Range(0, 100.0f);
        bool isCritical = IsCritical(randomValue, itemData.criticalRate);

        if (isCritical) {
            DebugLogger.Log("Critical");
            healPower = Mathf.FloorToInt(healPower * 1.5f);
        }

        // 対象先設定
        if (myEntityType == EntityType.Player) {
            BattleManager.instance.UpdatePlayerHp(healPower, EffectType.Heal, isCritical);
        } else {
            BattleManager.instance.UpdateEnemyHp(healPower, EffectType.Heal, isCritical);
        }
        SoundManager.instance.PlaySE(SE_TYPE.Heal);
    }

    private bool IsCritical(float randomValue, float rate) {
        //Debug.Log($"{randomValue} <= {rate}");

        // クリティカル判定
        if (randomValue <= rate) {
            //Debug.Log("Critical");
            return true;
        }
        return false;
    }

    /// <summary>
    /// 乱数作成
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private int GetRandomValue(int min, int max) {
        // maxが2以上の場合にランダムな値を返す
        if (max >= 2) {
            return UnityEngine.Random.Range(min, max);
        }
        // maxが2未満の場合、minをそのまま返す
        else {
            return min;
        }
    }

    private void AddPriceToMoney(ReleaseType releaseType) {
        int price = releaseType switch {
            ReleaseType.Destroy => 0, //itemData.price / 2,
            ReleaseType.SelfRelease => itemData.price,
            ReleaseType.Distribute => itemData.price,
            _ => 0
        };

        // 加算
        GameData.instance.userData.SoulPoint.Value += price;
    }

    public override void Release() {
        if (isReleased) {
            DebugLogger.Log("This object has already been released.");
            return;
        }

        if (gameObject.activeInHierarchy) {
            isReleased = true;

            disuseButtonSubscribe?.Dispose();
            battleStartDisposable?.Dispose();
            enhanceLevelDispose?.Dispose();
            battleEndDisposable?.Dispose();
            successSettlementDisposable?.Dispose();

            disuseButtonSubscribe?.Dispose();
            releaseButtonSubscribe?.Dispose();

            objectPool.Release(this);

            if (entityType == EntityType.Player) {
                // パッシブの場合のみ HP を減らす
                if (itemData.effectType == EffectType.Passive) {
                    GameData.instance.charaStatus.CalculateMaxHp(-itemData.hpBonus);
                    DebugLogger.Log($"うしなったので Hp を減少させる : {-itemData.hpBonus}");
                }

                PlayerInventoryManager.instance.RemoveItem(this);

                // 一旦 Content の配下から抜く。
                // そうしないと、オブジェクトプールのあった位置に再表示されるため、新しく追加したものの位置が正しく表示されない
                transform.SetParent(PlayerInventoryManager.instance.transform);
            } else {
                transform.SetParent(EnemyInfoDisplayManager.instance.transform);
            }
        }
    }

    /// <summary>
    /// 主に敵の Info 表示用の設定(購読などなし)
    /// オブジェクトプールはプレイヤー・敵を問わず使いまわすので初期化が必要
    /// </summary>
    /// <param name="entityType"></param>
    public void SetUpInfoDisplay(EntityType entityType) {
        isReleased = false;
        this.entityType = entityType;

        UpdateEnhanceLevelDisplay(0);
        UpdateDurabilityDisplay(-1);
        battleResultType = BattleResultType.Ready;

        prevDurability = 0;

        // 使用停止のシェードの設定
        isDisuse = false;
        canvasGroupShade.gameObject.SetActive(isDisuse);

        imgItemIcon.color = Color.white;
        imgIconGauge.fillAmount = 1.0f;
    }

    /// <summary>
    /// LeaderBoard 用の設定
    /// </summary>
    /// <param name="itemData"></param>
    public void SetUpResult(ItemData itemData, int level) {
        // 新しいインスタンスで ItemData のコピーを使う(参照すると、全 ItemData が変わってしまうため)
        this.itemData = new(itemData);

        imgItemIcon.color = Color.white;
        imgIconGauge.fillAmount = 0f;

        Sprite itemIconSprite = DataBaseManager.instance.GetItemIcon(this.itemData.id);
        imgItemIcon.sprite = itemIconSprite;

        transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, 0);
        transform.localEulerAngles = Vector3.zero;

        // 耐久値表示更新
        UpdateDurabilityDisplay(this.itemData.durability);

        // 使用停止のシェードの設定
        isDisuse = false;
        canvasGroupShade.gameObject.SetActive(isDisuse);

        UpdateEnhanceLevelDisplay(level);
    }

    private void OnDestroy() {
        Release();
    }
}