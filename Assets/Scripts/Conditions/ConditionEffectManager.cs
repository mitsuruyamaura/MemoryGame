using System.Collections.Generic;
using UnityEngine;

public class ConditionEffectManager : AbstractSingleton<ConditionEffectManager> {

    public List<ConditionEffect> conditionEffectsList = new List<ConditionEffect>();

    /// <summary>
    /// 引数で指定したコンディションのエフェクト用プレファブを取得
    /// </summary>
    /// <param name="conditionType"></param>
    /// <returns></returns>
    public ConditionEffect GetConditionEffect(ConditionType conditionType) {
        return conditionEffectsList.Find(x => x.conditionType == conditionType);
    }
}