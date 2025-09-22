using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class RecoveryItemSymbol : SymbolBase
{
    [SerializeField]
    private float recoveryRate = 0.2f;

    [SerializeField]
    private Text txtRecoveryPoint;


    public override void OnEnterSymbol() {
        base.OnEnterSymbol();

        // 左右にゆらす
        tween = transform.GetChild(0).transform.DORotate(new Vector3(0, 0, Random.Range(-15, -30)), 1.5f)
                    .SetEase(Ease.Linear)
                    .SetLink(gameObject)
                    .SetLoops(-1, LoopType.Yoyo);
    }


    public override async UniTask TriggerEffectAsync() {
        await base.TriggerEffectAsync();

        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        GameObject effectPrefab = EffectManager.instance.recoveryLifeEffectPrefab;

        int healPoint = (int)(GameData.instance.charaStatus.MaxHp.Value * recoveryRate);

        // Hp が最大値ならシールドに半分加算
        if (BattleManager.instance.PlayerHP.Value == GameData.instance.charaStatus.MaxHp.Value) {
            BattleManager.instance.UpdatePlayerShieldHp(healPoint / 2, false);
        } else {
            // Hp 回復
            BattleManager.instance.UpdatePlayerHp(healPoint, EffectType.Heal, false);
        }

        GameObject effect = Instantiate(effectPrefab, effectTran);
        effect.transform.SetParent(EffectManager.instance.effectConteinerTran);

        Destroy(effect, 1.5f);

        tween = transform.DOScale(0, 0.5f).SetEase(Ease.InQuart).SetLink(gameObject).OnComplete(() => {
            Release();
        });
        await UniTask.Delay(500);
    }
}