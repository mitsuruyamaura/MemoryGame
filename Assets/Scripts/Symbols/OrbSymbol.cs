using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class OrbSymbol : SymbolBase {

    [SerializeField]
    private int bonusStaminaPoint;

    public OrbType orbType;


    public override void OnEnterSymbol() {
        base.OnEnterSymbol();

        SetOrbData();
    }


    public override async UniTask TriggerEffectAsync() {
        await base.TriggerEffectAsync();

        tween.Kill();

        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        // 獲得したオーブを表示
        PlayerInventoryManager.instance.SetOrbIcon(orbType);

        GameData.instance.AddOrbList(orbType);

        // スタミナ回復
        //GameData.instance.staminaPoint.Value += bonusStaminaPoint;
        GameData.instance.userData.Stamina.Value += bonusStaminaPoint;

        // ボーナスチェック
        GameData.instance.CheckStaminaBonusByOrbs();

        GameObject effect = Instantiate(EffectManager.instance.orbGetEffectPrefab, effectTran);
        effect.transform.SetParent(EffectManager.instance.effectConteinerTran);

        Destroy(effect, 1.5f);

        tween = transform.DOScale(0, 0.5f).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => {
            Release();
            //base.OnExitSymbol();
        });

        // 続けて取ったときに処理が間に合わないので長めに取る
        await UniTask.Delay(1500);
    }

    //public override void TriggerAppearEffect(MapMoveController mapMoveController) {

    //    tween.Kill();

    //    base.TriggerAppearEffect(mapMoveController);

    //    //GameData.instance.orbs[no] = true;

    //    // 獲得したオーブを表示
    //    PlayerInventoryManager.instance.SetOrbIcon(orbType);

    //    GameData.instance.AddOrbList(orbType);

    //    // スタミナ回復
    //    //GameData.instance.staminaPoint.Value += bonusStaminaPoint;
    //    GameData.instance.userData.Stamina.Value += bonusStaminaPoint;

    //    // ボーナスチェック
    //    GameData.instance.CheckStaminaBonusByOrbs();

    //    GameObject effect = Instantiate(EffectManager.instance.orbGetEffectPrefab, effectTran);
    //    effect.transform.SetParent(EffectManager.instance.effectConteinerTran);

    //    Destroy(effect, 1.5f);

    //    tween = transform.DOScale(0, 0.5f).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => {
    //        Release();
    //        //base.OnExitSymbol();
    //    });
    //}

    /// <summary>
    /// エネミーの上に配置のための移動
    /// </summary>
    /// <param name="newPos"></param>
    public void SetPositionOrbSymbol(Vector3 newPos) {
        // 配置移動中はキャラが取れないようにする
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        boxCol.enabled = false;
        transform.DOMove(newPos, 1.0f).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => { boxCol.enabled = true; });
    }

    /// <summary>
    /// オーブの情報設定
    /// </summary>
    /// <param name="setOrbType"></param>
    public void SetOrbData() {  // OrbType setOrbType, int orbNo

        //no = orbNo;

        // ランダムの場合
        //if (setOrbType == OrbType.Random) {
        orbType = (OrbType)Random.Range(0, System.Enum.GetValues(typeof(OrbType)).Length);
        //} else {
        //    orbType = setOrbType;
        //}

        // 画像設定
        spriteSymbol.sprite = DataBaseManager.instance.orbDataSO.orbDatasList.Find(x => x.orbType == this.orbType).spriteOrb;
    }
}
