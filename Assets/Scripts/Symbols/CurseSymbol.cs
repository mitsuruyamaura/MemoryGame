using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class CurseSymbol : SymbolBase
{
    public override void OnEnterSymbol() {
        base.OnEnterSymbol();

        float randomValue = Random.Range(-0.5f, 0.5f);
        tween = transform.DOPunchScale(Vector3.one * 0.35f, 3.0f + randomValue, 1)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject)
            .SetLoops(-1, LoopType.Restart);
    }

    /// <summary>
    /// 解除の有無でエフェクトを変えるので、ここは使わない
    /// </summary>
    /// <returns></returns>
    //public override async UniTask TriggerEffectAsync() {
    //    // 呪い自身は呪いのクラスが Null なので二重発動はしない
    //    base.TriggerEffectAsync().Forget();

    //    SoundManager.instance.PlaySE(SE_TYPE.Debuff);

    //    // 呪い発動時のエフェクト作成(破棄はエフェクトクラス内で行う)
    //    CurseEffect effectPrefab = EffectManager.instance.curseEffectPrefab.GetComponent<CurseEffect>();
    //    CurseEffect effect = Instantiate(effectPrefab, effectTran);
    //    effect.transform.SetParent(EffectManager.instance.effectConteinerTran);
    //    effect.PlayEffect();

    //    tween?.Kill();
    //    tween = null;

    //    tween = transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetLink(gameObject).OnComplete(() => {
    //        Release();
    //    });
    //    await UniTask.Delay(2500);
    //}

    /// <summary>
    /// 呪い解除成功時の処理
    /// </summary>
    /// <returns></returns>
    public async UniTask SuccessCurseEffectAsync() {
        // 呪い自身は呪いのクラスが Null なので二重発動はしない
        base.TriggerEffectAsync().Forget();

        SoundManager.instance.PlaySE(SE_TYPE.Heal);

        // アニメしているとエフェクトの高さもつられて変わってしまうので、停止して Scale を初期化する
        tween?.Kill();
        tween = null;
        transform.localScale = Vector3.one;

        GameObject effect = Instantiate(EffectManager.instance.orbGetEffectPrefab, effectTran);
        effect.transform.SetParent(EffectManager.instance.effectConteinerTran);
        Destroy(effect, 1.5f);

        tween = transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetLink(gameObject).OnComplete(() => {
            Release();
        });
        await UniTask.Delay(1500);
    }

    /// <summary>
    /// 呪い解除失敗時の処理
    /// </summary>
    /// <returns></returns>
    public async UniTask FailureCurseEffectAsync() {
        // 呪い自身は呪いのクラスが Null なので二重発動はしない
        base.TriggerEffectAsync().Forget();

        SoundManager.instance.PlaySE(SE_TYPE.Debuff);

        // アニメしているとエフェクトの高さもつられて変わってしまうので、停止して Scale を初期化する
        tween?.Kill();
        tween = null;
        transform.localScale = Vector3.one;

        // 呪い発動時のエフェクト作成(破棄はエフェクトクラス内で行う)
        CurseEffect effectPrefab = EffectManager.instance.curseEffectPrefab.GetComponent<CurseEffect>();
        CurseEffect effect = Instantiate(effectPrefab, effectTran);

        effect.transform.SetParent(EffectManager.instance.effectConteinerTran);
        effect.PlayEffect();

        tween = transform.DOScale(0, 0.5f).SetEase(Ease.InBack).SetLink(gameObject).OnComplete(() => {
            Release();
        });
        await UniTask.Delay(2500);
    }
}