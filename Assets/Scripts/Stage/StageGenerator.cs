using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

public enum StageType {
    Field,
    Dungeon,
}

[System.Serializable]
public struct SymbolGenerateData {
    // 生成するシンボルのプレファブを登録
    public SymbolBase symbolBasePrefab;
    public int symbolWeight;
}

public class StageGenerator : MonoBehaviour
{
    // StageType.Field 用のタイル群
    [SerializeField] private Tile[] fieldBaseTiles;
    [SerializeField] private Tile[] fieldWalkTiles;
    [SerializeField] private Tile[] fieldCollisionTiles;

    // タイルを配置するタイルマップ
    [SerializeField] private Tilemap tileMapBase;
    [SerializeField] private Tilemap tileMapWalk;
    [SerializeField] private Tilemap tileMapCollision;
    public Tilemap TilemapColision => tileMapCollision;
    public Tilemap TilemapWalk => tileMapWalk;

    // TODO GameData の StageData からもらうように変える
    // 並べる数
    [SerializeField] private int row;      // 行/ 水平(横)方向
    [SerializeField] private int column;   // 列/ 垂直(縦)方向

    // シンボル生成用のデータリスト -> SO に変える
    [SerializeField] private List<SymbolGenerateData> symbolGenerateDatasList = new List<SymbolGenerateData>();
    [HideInInspector, SerializeField] private List<SymbolGenerateData> specialSymbolGenerateDatasList = new List<SymbolGenerateData>();
    [SerializeField] private SymbolBase orbSymbolPrefab;

    [SerializeField, Header("シンボルの生成率"), Range(0, 100)] private int generateSymbolRate;

    // 各タイルの座標に地形情報を保持する Dictionary
    public Dictionary<Vector3Int, TerrainType> terrainInfoDic = new ();


    //void Start() {
    //    // Debug 用
    //    GenerateStageFromRandomTiles();
    //    GenerateSymbols(-1);
    //}

    public void SetTilemapData(WaveData waveData) {
        row = waveData.row;
        column = waveData.column;
        generateSymbolRate = waveData.generateSymbolRate;
    }


    /// <summary>
    /// ランダムなタイルをタイルマップに配置してステージを作る
    /// </summary>
    /// <param name="stageType"></param>
    public void GenerateStageFromRandomTiles(StageType stageType = StageType.Field) {
        tileMapBase.ClearAllTiles();
        tileMapWalk.ClearAllTiles();
        tileMapCollision.ClearAllTiles();
        terrainInfoDic?.Clear();

        // Grid_Base と外壁用の Grid_Collider を配置
        for (int i = -row; i < row; i++) {
            for (int j = -column; j < column; j++) {
                Vector3Int pos = new(i, j, 0);
                switch (stageType) {
                    case StageType.Field:
                        // 一番外側の場合
                        if (i == -row || i == row - 1 || j == -column || j == column -1) {
                            // 壁用のコライダータイルを配置
                            tileMapCollision.SetTile(new Vector3Int(i, j, 0), fieldCollisionTiles[0]);
                            terrainInfoDic[pos] = TerrainType.Mountain;
                        } else {
                            // フィールド用のタイルを配置
                            tileMapBase.SetTile(new Vector3Int(i, j, 0), fieldBaseTiles[0]); 
                            terrainInfoDic[pos] = TerrainType.Desert;
                        }
                        break;

                    case StageType.Dungeon:
                    default:
                        break;
                }
            }
        }

        // Grid_Walk と Grid_Collider を配置
        int generateValue = 0;

        for (int i = -row; i < row; i++) {
            for (int j = -column; j < column; j++) {
                // 一番外側の場合とプレイヤーのスタート地点の場合
                if (i == -row || i == row - 1 || j == -column || j == column - 1 || (i == 0 && j == 0)) {
                    // 何も行わずに次の処理へ
                    continue;
                }

                // 生成値用のランダム値を取得
                int maxRandomRange = UnityEngine.Random.Range(30, 80);

                // 生成値を加算
                generateValue += UnityEngine.Random.Range(0, maxRandomRange);

                // 生成値が生成目標値(仮)を超えていない場合
                if (generateValue <= 100) {
                    // 何も行わずに次の処理へ
                    continue;
                }

                Vector3Int pos = new(i, j, 0);

                // Walk か Collision か決める(仮に、20 % の確率で Collision) 
                if (UnityEngine.Random.Range(0, 100) <= 20) {

                    // TODO 重み付ける

                    // Collision 用のタイルの中でランダムにタイルを決める
                    int index = UnityEngine.Random.Range(0, fieldCollisionTiles.Length);

                    tileMapCollision.SetTile(pos, fieldCollisionTiles[index]);
                    terrainInfoDic[pos] = (TerrainType)index + 5;
                } else {
                    // TODO ステージ(Wave)ごとに重み付ける

                    // Walk 用のタイルの中でランダムにタイルを決める
                    int index = UnityEngine.Random.Range(0, fieldWalkTiles.Length);

                    tileMapWalk.SetTile(pos, fieldWalkTiles[index]);
                    terrainInfoDic[pos] = (TerrainType)index;
                }
             
                // タイルを生成したので生成値をリセット
                generateValue = 0;
            }
        }
    }

    /// <summary>
    /// 座標から地形情報を取得する
    /// 未使用
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public TerrainType GetTerrainAtPosition(Vector3Int pos) {
        if (terrainInfoDic.ContainsKey(pos)) {
            return terrainInfoDic[pos];
        }
        return TerrainType.Grassland; // デフォルトの地形
    }


    /// <summary>
    /// 通常のシンボルをランダムに作成
    /// 未使用
    /// </summary>
    /// <param name="generateSymbolCount"></param>
    /// <returns></returns>
    //public List<SymbolBase> GenerateSymbols(int generateSymbolCount) {
    //    // List に登録する
    //    List<SymbolBase> symbolsList = new List<SymbolBase>();

    //    // 重み付けの合計値を算出
    //    int totalWeight = symbolGenerateDatasList.Select(x => x.symbolWeight).Sum();
    //    //Debug.Log(totalWeight);

    //    for (int i = -row +1; i < row -1; i++) {
    //        for (int j = -column +1; j < column -1; j++) {

    //            // プレイヤーのスタート地点の場合
    //            if (i == 0 && j == 0) {
    //                // 何も行わずに次の処理へ
    //                continue;
    //            }

    //            // タイルマップの座標に変換
    //            Vector3Int tilePos = tileMapCollision.WorldToCell(new Vector3(i, j, 0));

    //            // タイルの ColliderType が Grid ではないか確認
    //            if (tileMapCollision.GetColliderType(tilePos) == Tile.ColliderType.Grid) {
    //                // Grid の場合には配置しないので、何も行わずに次の処理へ
    //                continue;
    //            }

    //            // 80 % はシンボルなし => 264 マスの場合、大体35～55個シンボルが出来る
    //            if (UnityEngine.Random.Range(0, 100) > generateSymbolRate) {
    //                continue;
    //            }

    //            int index = 0;
    //            int value = UnityEngine.Random.Range(0, totalWeight);

    //            // 重みづけから生成するシンボルを確認
    //            for (int x = 0; x < symbolGenerateDatasList.Count; x++) {
    //                if (value <= symbolGenerateDatasList[x].symbolWeight) {
    //                    index = x;
    //                    //Debug.Log(index + " value : " + value);
    //                    break;
    //                }
    //                value -= symbolGenerateDatasList[x].symbolWeight;
    //            }

    //            // 抽選されたシンボルを生成
    //            symbolsList.Add(Instantiate(symbolGenerateDatasList[index].symbolBasePrefab, new Vector3(i, j, 0), Quaternion.identity));               

    //            generateSymbolCount--;

    //            // generateSymbolCount = -1 でスタートの場合は抽選回数なし
    //            if (generateSymbolCount == 0) {
    //                break;
    //            }
    //        }
    //        if (generateSymbolCount == 0) {
    //            break;
    //        }
    //    }

    //    // 完成したリストを戻す
    //    return symbolsList;
    //}

    /// <summary>
    /// ４つの特殊シンボルをランダムな順番に作成
    /// 未使用
    /// </summary>
    /// <returns></returns>
    //public List<SymbolBase> GenerateSpecialSymbols(OrbType[] orbTypes) {

    //    List<SymbolBase> symbolsList = new List<SymbolBase>();

    //    // ステージのデータに合わせて特殊シンボルを設定
    //    List<OrbType> randomOrbTypeList = new List<OrbType>(orbTypes);
    //    randomOrbTypeList = randomOrbTypeList.OrderBy(x => Guid.NewGuid()).ToList();

    //    int index = 0;

    //    // 最初はキャラのそばに生成
    //    for (int x = -1; x < 2; x += 2) {
    //        if (randomOrbTypeList.Count <= index) {
    //            break;
    //        }
    //        for (int y = -1; y < 2; y += 2) {
    //            symbolsList.Add(Instantiate(orbSymbolPrefab, new Vector3(x, y, 0), Quaternion.identity));
    //            index++;
    //            if (randomOrbTypeList.Count <= index) {
    //                break;
    //            }               
    //        }
    //    }

    //    //// ランダムに並び替える(ここもちゃんとうごく)
    //    //List<SymbolGenerateData> randomSymbolsList = new List<SymbolGenerateData>(specialSymbolGenerateDatasList);
    //    //randomSymbolsList = randomSymbolsList.OrderBy(x => Guid.NewGuid()).ToList();

    //    //int index = 0;

    //    //// 最初はキャラのそばに生成
    //    //for (int x = -1; x < 2; x += 2) {
    //    //    for (int y = -1; y < 2; y += 2) {
    //    //        symbolsList.Add(Instantiate(randomSymbolsList[index].symbolBasePrefab, new Vector3(x, y, 0), Quaternion.identity));
    //    //        index++;
    //    //    }
    //    //}

    //    // List に登録する
    //    return symbolsList;
    //}
}