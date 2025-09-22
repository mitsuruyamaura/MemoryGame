using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[CreateAssetMenu(fileName = "EnemyMoveEventDataSO", menuName = "Create EnemyMoveEventDataSO")]
public class EnemyMoveEventDataSO : ScriptableObject
{
    public UnityAction<Transform, float> GetEnemyMove(EnemyMoveType enemyMoveType) {

        return enemyMoveType switch {
            EnemyMoveType.Archer => MoveTypeArcher,
            EnemyMoveType.Defender => MoveTypeDefender,
            EnemyMoveType.Assasin => MoveTypeAssasin,
            _ => MoveTypeArcher
        };
    }

    /// <summary>
    /// アーチャータイプの移動
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="duration"></param>
    public void MoveTypeArcher(Transform tran, float duration) {
        DebugLogger.Log("アーチャータイプの移動");

        tran.DOMoveX(tran.position.x + Random.Range(-15.0f, 15.0f), duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// ディフェンダータイプの移動
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="duration"></param>
    public void MoveTypeDefender(Transform tran, float duration) {
        DebugLogger.Log("ディフェンダータイプの移動");

        tran.DOMoveZ(tran.position.z + Random.Range(-5.0f, 5.0f), duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// アサシンタイプの移動
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="duration"></param>
    public void MoveTypeAssasin(Transform tran, float duration) {
        //tran.DOMove(tran.position, duration).SetEase(Ease.InOutQuart);
    }
}
