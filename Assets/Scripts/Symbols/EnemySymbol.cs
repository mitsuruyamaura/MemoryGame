using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;
using R3;
using Cysharp.Threading.Tasks;

public enum MoveDirectionTpye {
    Up,
    Down,
    Left,
    Right,
}

public class EnemySymbol : SymbolBase, IPoolable
{
    private Tilemap tilemapCollider;
    private BoxCollider2D boxCol;
    private float moveDuration = 0.05f;

    public int moveIntervalCount = 0;     // 移動するまでの待機カウント

    public EnemyData enemyData;
    public NameData nameData;

    public List<int> equipItemNoList = new();

    [SerializeField] private int enemyRange;
    [SerializeField] private bool isBoss;
    // EnemyData から、待機時間を設定する


    public override void OnEnterSymbol() {
        base.OnEnterSymbol();
        //base.OnEnterSymbol(symbolManager);

        //tilemapCollider = SymbolManager.instance.tilemapCollider;
        TryGetComponent(out boxCol);


        // TODO 引数でもらって設定する

        if (isBoss) {
            enemyData = DataBaseManager.instance.GetEnemyDataListFromRarity(Rarity.Legendary).FirstOrDefault(); //DataBaseManager.instance.GetEnemyData(DataBaseManager.instance.enemyDataSO.enemyDatasList.Count);
            gameObject.SetActive(false);
        } else {

            // 歩数により、Stage のWave が代わり、それにより敵の種類も変わる
            WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);

            Rarity rarity = WaveData.GetRandomRarity(waveData);
            List<EnemyData> rarityEnemyList = DataBaseManager.instance.GetEnemyDataListFromRarity(rarity);
            DebugLogger.Log(rarityEnemyList.Count.ToString());

            //int maxIndex = 0;
            //if (enemyRange != 0) {
            //    maxIndex = enemyRange + 1;
            //} else {
            //    maxIndex = DataBaseManager.instance.enemyDataSO.enemyDatasList.Count + 1;
            //}

            int randomIndex = UnityEngine.Random.Range(0, rarityEnemyList.Count);
            if (enemyData == null) {
                DebugLogger.Log($"EnemyData なし cardIndex :{randomIndex}");
                return;
            }
            enemyData = rarityEnemyList[randomIndex];

            int randomNameIndex = UnityEngine.Random.Range(1, DataBaseManager.instance.nameDataSO.nameDataList.Count + 1);
            nameData = DataBaseManager.instance.GetNameData(randomNameIndex);
        } 

        int equipNum = UnityEngine.Random.Range(enemyData.minEquipCount, enemyData.maxEquipCount);

        int[] equipItemIndexs = enemyData.equipIndexStr.Split('/').ToArray().Select(data => int.Parse(data)).ToArray();
        equipItemNoList = equipItemIndexs.OrderBy(x => Guid.NewGuid())  // シャッフル
            .Take(equipNum)  // 指定した数だけ取り出す
            .ToList();
    }


    public override async UniTask TriggerEffectAsync() {
        // 呪いの確認のため、一旦降ろす
        isSymbolTriggerd = false;

        await base.TriggerEffectAsync();
    }

    //public override void TriggerAppearEffect(MapMoveController mapMoveController) {

    //    base.TriggerAppearEffect(mapMoveController);

    //    Debug.Log("移動先で敵に接触");

    //    tween = transform.DOShakeScale(0.75f, 1.0f)
    //        .SetEase(Ease.OutQuart)
    //        .SetLink(gameObject)
    //        .OnComplete(() => 
    //        {
    //            Release();
    //            //OnExitSymbol();
    //        } );
    //}

    //protected override void OnExitSymbol() {

    // エネミーのシンボル用の List から削除
    //symbolManager.RemoveEnemySymbol(this);

    //base.OnExitSymbol();

    // バトルの準備
    //PreparateBattle();
    //}

    /// <summary>
    /// エネミーをランダムな方向に１マス移動するか、その場で待機
    /// </summary>
    public void EnemyMove() {

        // 移動する方向をランダムに１つ設定
        MoveDirectionTpye randomDirType = (MoveDirectionTpye)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(MoveDirectionTpye)).Length);

        Vector3 nextPos = GetMoveDirection(randomDirType);

        // 自分のコライダーをオフにして Ray が自分のコライダーに当たってしまう誤判定を防ぐ
        SwtichCollider(false);

        // 移動する方向に Ray を投射して他のシンボルが存在していないかを確認
        RaycastHit2D hit = Physics2D.Raycast(transform.position, nextPos, 0.8f, LayerMask.GetMask("Symbol"));　　　//

        // Scene ビューにて Ray の可視化
        Debug.DrawRay(transform.position, nextPos, Color.blue, 0.8f);

        SwtichCollider(true);

        // Ray の投射先に別のシンボルがある場合には => エネミーのみとりあえず除外。アイテムの上にエネミーが乗るようになるので 
        if (hit.collider != null) {
            // 終了
            return;
        }

        if (hit.collider != null &&  hit.collider.TryGetComponent(out EnemySymbol enemySymbol)) {

            return;
        }

        // 移動できるタイルかタイルマップの座標に変換して確認
        Vector3Int tilePos = tilemapCollider.WorldToCell(transform.position + nextPos);

        //Debug.Log(tilePos);
        //Debug.Log(tilemapCollider.GetColliderType(tilePos));

        // Grid のコライダーでなければ
        if (tilemapCollider.GetColliderType(tilePos) != Tile.ColliderType.Grid) {

            // 移動
            transform.DOMove(transform.position + nextPos, moveDuration * GameData.instance.moveTimeScale).SetEase(Ease.Linear);
        }
    }

    /// <summary>
    /// 移動する方向の情報を座標に変換
    /// </summary>
    /// <param name="nextDirection"></param>
    /// <returns></returns>
    private Vector3 GetMoveDirection(MoveDirectionTpye nextDirection) {
        return nextDirection switch {
            MoveDirectionTpye.Up => new Vector2(0, 1),
            MoveDirectionTpye.Down => new Vector2(0, -1),
            MoveDirectionTpye.Left => new Vector2(-1, 0),
            MoveDirectionTpye.Right => new Vector2(1, 0),
            _ => Vector2.zero
        };
    }

    /// <summary>
    /// コライダーのオンオフ切り替え
    /// </summary>
    /// <param name="isSwicth"></param>
    public void SwtichCollider(bool isSwicth) {
        boxCol.enabled = isSwicth;
    }


    public void CountDownMoveInterval() {
        // 移動カウントを停止するデバフ(睡眠・移動停止など)があるか確認し、ある場合には処理しない


        moveIntervalCount--;

        if (moveIntervalCount <= 0) {
            // 移動

        }
    }
}
