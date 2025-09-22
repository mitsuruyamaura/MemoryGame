using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class SymbolBase : PoolBase, IPoolable
{
    public SymbolType symbolType;
    public int no;

    [SerializeField]
    protected Transform effectTran;

    [SerializeField]
    protected SpriteRenderer spriteSymbol;

    protected Tween tween;

    protected CurseSymbol curseSymbol;
    public CurseSymbol CurseSymbol => curseSymbol;

    //protected SymbolManager symbolManager;

    public bool isSymbolTriggerd;

    public TerrainType terrainType;


    /// <summary>
    /// シンボル生成時の処理
    /// </summary>
    public virtual void OnEnterSymbol() {
        isSymbolTriggerd = false;
        isReleased = false;

        //this.symbolManager = symbolManager;
    }


    public override void Release() {
        if (isReleased) {
            return;
        }

        tween?.Kill();
        tween = null;

        // 画像の大きさをリセット

        // プールに戻ったことを通知
        //onReleaseAction?.Invoke(this);
        if (gameObject.activeInHierarchy) {
            isReleased = true;
            objectPool.Release(this);
            transform.localScale = Vector3.one;
            curseSymbol = null;
        }
        //Dispose();
    }

    //protected virtual void Start() {
    //    OnEnterSymbol();
    //}

    /// <summary>
    /// 侵入判定時のエフェクト生成用
    /// 未使用
    /// </summary>
    //public virtual void TriggerAppearEffect(MapMoveController mapMoveController) {

    //    if (isSymbolTriggerd) {
    //        return ;
    //    }

    //    isSymbolTriggerd = true;
    //}


    public void SetCurse(CurseSymbol curseSymbol) {
        this.curseSymbol = curseSymbol;
    }


    protected bool CheckCurse() {
        // 解放に必要なソウルポイントが足りているか確認
        int needSoulPoint = ConstData.POWER_SPOT_NEED_RELEASE_POINT * GameData.instance.userData.waveNo;

        if (GameData.instance.userData.SoulPoint.Value < needSoulPoint) {
            isSymbolTriggerd = false;
            DebugLogger.Log($"{needSoulPoint} < {GameData.instance.userData.SoulPoint.Value}");
            DebugLogger.Log($"SoulPoint が足りないため、解放不可");

            // 解除失敗メッセージ表示
            PowerSpotInfoDisplayManager.instance.NotReleasePowerPointInfo(ReleaseActionMessageType.NotRelease_SoulPoint);
            return false;
        }

        // ソウルポイントを減算して表示更新
        GameData.instance.userData.SoulPoint.Value -= needSoulPoint;

        // 呪い解除回数を加算
        GameData.instance.userData.UncurseCount.Value++;

        // 解除成功メッセージ表示
        PowerSpotInfoDisplayManager.instance.NotReleasePowerPointInfo(ReleaseActionMessageType.Release);

        // 呪い解除成功
        return true;
    }

    /// <summary>
    /// 現在は EnemySymbol 用
    /// </summary>
    public virtual void TriggerEffect() {

        if (isSymbolTriggerd) {
            return;
        }

        isSymbolTriggerd = true;
    }


    public virtual async UniTask TriggerEffectAsync() {
        //Debug.Log(this);
        //Debug.Log(curseSymbol);
        
        if (isSymbolTriggerd) {
            await UniTask.Yield();
            return;
        }

        //Debug.Log(curseSymbol);

        // 呪われている場合
        if (curseSymbol != null) {
            // 呪いの解除のチェック
            bool isCurseSuccessd = CheckCurse();

            // 呪いの解除に失敗した場合
            if (!isCurseSuccessd) {
                // 呪い発動
                await curseSymbol.FailureCurseEffectAsync();
                DebugLogger.Log("呪い発動");

            } else {
                await curseSymbol.SuccessCurseEffectAsync();
                DebugLogger.Log("呪い解除");
            }

            // 一度発動したら終わり
            curseSymbol = null;
        }

        isSymbolTriggerd = true;
    }

    //protected virtual void OnExitSymbol() {

    //    if (tween != null) {
    //        tween.Kill();
    //    }

    //    // オブジェクトプールにして、自動的に戻るようにすれば SymbolManager の参照いらなくなる

    //    // List からシンボルを削除
    //    symbolManager.RemoveSymbolsList(this);

    //    Destroy(gameObject);
    //}

    /// <summary>
    /// シンボル画像の表示/非表示切り替え
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchDisplaySymbol(bool isSwitch) {
        spriteSymbol.enabled = isSwitch;
    }

    /// <summary>
    /// シンボルのゲームオブジェクトの表示/非表示
    /// </summary>
    /// <param name="isSwitch"></param>
    public void SwitchActivateSymbol(bool isSwitch) {
        gameObject.SetActive(isSwitch);
    }

    public void SetTerrainType(TerrainType terrainType) {
        this.terrainType = terrainType;
    } 
}