using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class PlayerCondition_Poison : PlayerConditionBase
{
    /// <summary>
    /// 毒によるダメージ効果
    /// </summary>
    protected override async UniTask ApplyEffect(CancellationToken token) {
        //GameData.instance.hp = Mathf.Clamp(GameData.instance.hp += (int)-conditionValue, 0, GameData.instance.maxHp);

        //StartCoroutine(mapMoveController.GetStage().UpdateDisplayHp(1.0f));

        await base.ApplyEffect(token);
    }
}
