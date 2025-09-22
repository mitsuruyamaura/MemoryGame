using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCondition_WalkThrough : PlayerConditionBase
{
    private SpriteRenderer spritePlayer;
    private float originAlpha = 1.0f;

    protected override void OnEnterCondition() {

        //if(mapMoveController.transform.GetChild(0).TryGetComponent(out spritePlayer)) {

        //    // プレイヤーの画像を半透明にする
        //    spritePlayer.color = new Color(1.0f, 1.0f, 1.0f, conditionValue);
        //}

        base.OnEnterCondition();
    }

    protected override void OnExitCondition() {

        // 終了時の演出


        // 元の透明度に戻す
        spritePlayer.color = new Color(1.0f, 1.0f, 1.0f, originAlpha);

        base.OnExitCondition();
    }
}
