using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using UnityEngine.Tilemaps;

/// <summary>
/// 複数の IObjectPool<PoolBase> を用意したいので、GeneratorBase は継承しないでおく
/// </summary>
public class SymbolGenerator : MonoBehaviour, ISetup {

    [SerializeField] private SymbolDataSO symbolDataSO;        // エフェクトデータのリスト   
                                                               // 並べる数
    [SerializeField] private int row;      // 行/ 水平(横)方向
    [SerializeField] private int column;   // 列/ 垂直(縦)方向
    [SerializeField, Header("シンボルの生成率"), Range(0, 100)] private int generateSymbolRate;   // いままでは20

    // エフェクトのオブジェクトプールを管理するディクショナリー。Key は SymbolType 型。Value は IObjectPool 型
    private Dictionary<SymbolType, IObjectPool<PoolBase>> symbolPools = new();
    private Vector3 offset = new(0.5f, 0.5f, 0);

    public bool useCurse;


    public void SetUp(GameObject entityObject = null) {
        InitObjectPool();
    }

    /// <summary>
    /// 各シンボルごとにオブジェクトプールを設定して Dictionary に登録
    /// </summary>
    public void InitObjectPool() {
        foreach (SymbolData symbolData in symbolDataSO.symbolDataList) {
            if (symbolData == null) {
                continue;
            }

            // Dictionary 用の値としてオブジェクトプールを設定
            IObjectPool<PoolBase> pool = CreateObjectPool(symbolData);

            // Dictionary に追加
            symbolPools.Add(symbolData.symbolType, pool);
        }
    }

    /// <summary>
    /// 各シンボル用にオブジェクトプールを初期化して作成
    /// </summary>
    /// <param name="effectPrefab"></param>
    /// <returns></returns>
    private IObjectPool<PoolBase> CreateObjectPool(SymbolData symbolData) {
        // オブジェクトプールの初期化
        IObjectPool<PoolBase> pool = new ObjectPool<PoolBase>(
            createFunc: () => CreateSymbol(symbolData),
            actionOnGet: OnGetFromPool,
            actionOnRelease: target => target.gameObject.SetActive(false),
            actionOnDestroy: target => Destroy(target.gameObject),
            collectionCheck: true,
            defaultCapacity: 10,
            maxSize: 1000);

        return pool;
    }

    /// <summary>
    /// ObjectPool.Get メソッド実行時、プールされたオブジェクトがない場合に実行し、オブジェクトを生成して返す
    /// </summary>
    /// <param name="effectPrefab"></param>
    /// <returns></returns>
    private SymbolBase CreateSymbol(SymbolData symbolData) {
        // シンボルを生成
        SymbolBase symbolBase = Instantiate(symbolData.sumbolPrefab);
        symbolBase.transform.SetParent(transform);

        // symbolType に対応するオブジェクトプールを Dictionary から取得
        IObjectPool<PoolBase> symbolPool = symbolPools[symbolData.symbolType];

        // 戻る先のプールを設定する(シンボルごとのオブジェクトプールに所属させる)
        symbolBase.ObjectPool = symbolPool;
        return symbolBase;
    }

    /// <summary>
    /// ObjectPool.Get() メソッドにより、actionOnGet として実行される
    /// </summary>
    /// <param name="target"></param>
    protected virtual void OnGetFromPool(PoolBase target) => target.gameObject.SetActive(true);

    /// <summary>
    /// 外部から呼び出すメソッド
    /// オブジェクトプールからシンボルを取得して返す
    /// </summary>
    /// <param name="effIndex"></param>
    /// <returns></returns>
    public SymbolBase GetSymbolFromPools(SymbolType symbolType) {

        // シンボルが所属するオブジェクトプールを探す
        if (symbolPools.ContainsKey(symbolType)) {

            // 見つかったオブジェクトプールを指定
            IObjectPool<PoolBase> symbolPool = symbolPools[symbolType];

            // 指定されたオブジェクトプール内から、指定されたシンボルを取得。なければ生成
            SymbolBase pooledSymbol = (SymbolBase)symbolPool.Get();

            return pooledSymbol;
        } else {
            DebugLogger.Log($"指定された SymbolType は登録がありません。{symbolType}");
            return null;
        }
    }

    /// <summary>
    /// 通常のシンボルをランダムに作成
    /// </summary>
    /// <param name="generateSymbolCount"></param>
    /// <returns></returns>
    public List<SymbolBase> GenerateSymbols(int generateSymbolCount, Tilemap tileMapCollision, Tilemap tileMapWalk, Dictionary<Vector3Int, TerrainType> terrainInfoDic) {
        // List に登録する
        List<SymbolBase> symbolsList = new List<SymbolBase>();

        // 重み付けの合計値を算出
        //int totalWeight = symbolDataSO.symbolDataList.Select(x => x.symbolWeight).Sum();
        int totalWeight = symbolDataSO.symbolDataList.Sum(data => data.symbolWeight);

        WaveData waveData = DataBaseManager.instance.GetWaveData(GameData.instance.userData.waveNo);

        //Debug.Log(totalWeight);

        for (int i = -row + 1; i < row - 1; i++) {
            for (int j = -column + 1; j < column - 1; j++) {

                // プレイヤーのスタート地点の場合
                if (i == 0 && j == 0) {
                    // 何も行わずに次の処理へ
                    continue;
                }

                // タイルマップの座標に変換
                Vector3Int tilePos = tileMapCollision.WorldToCell(new Vector3(i, j, 0));

                // タイルの ColliderType が Grid ではないか確認
                if (tileMapCollision.GetColliderType(tilePos) == Tile.ColliderType.Grid) {
                    // Grid の場合には配置しないので、何も行わずに次の処理へ
                    continue;
                }

                // Walk タイルの下にだけ、アイテムシンボルを置く(隠しておく)
                tilePos = tileMapWalk.WorldToCell(new Vector3(i, j, 0));
                if (tileMapWalk.GetColliderType(tilePos) != Tile.ColliderType.Sprite) {
                    // Grid の場合には配置しないので、何も行わずに次の処理へ
                    continue;
                }

                // TODO 敵のシンボルは別途作成する


                // 80 % はシンボルなし => 264 マスの場合、大体35～55個シンボルが出来る
                if (UnityEngine.Random.Range(0, 100) > generateSymbolRate) {
                    continue;
                }

                //int index = 0;
                int value = UnityEngine.Random.Range(0, totalWeight);

                TerrainData terrainData = DataBaseManager.instance.GetTerrainData(terrainInfoDic[new(i, j, 0)].ToString());

                // 重み付けのための総重み計算、ランダム値の作成、初期値の用意
                int[] symbolRates = terrainData.GetSymbolRateArray();
                SymbolType[] generateSymbolTypes = terrainData.GetGenerateSymbolTypes();
                for (int x = 0; x < symbolRates.Length; x++) {
                    DebugLogger.Log($"{generateSymbolTypes[x]} : {symbolRates[x]}");
                }

                int totalSymbolWeight = symbolRates.Sum();
                int randomValue = UnityEngine.Random.Range(0, totalSymbolWeight);
                int cumulativeWeight = 0;

                // 累積重み(累積和)を計算しながら重み付けに基づいて選択
                SymbolType selectedSymbol = symbolRates
                    // symbolRates 配列のインデックス index を取得し、これを使って対応する SymbolType を generateSymbolTypes[index] から取り出す(匿名型のクラスを作成)
                    .Select((rate, index) => new { rate, symbolType = generateSymbolTypes[index] })
                    .FirstOrDefault(data => {
                        // 累積重みを計算しながら、対応する SymbolType を取得
                        cumulativeWeight += data.rate;
                        return randomValue < cumulativeWeight;
                    })?.symbolType ?? SymbolType.Enemy;         // 合致しなかった場合、デフォルトとして Enemy を返す

                // 重みづけから生成するシンボルを確認
                //for (int x = 0; x < symbolDataSO.symbolDataList.Count - 1; x++) {
                //    if (value <= symbolDataSO.symbolDataList[x].symbolWeight) {
                //        index = x;
                //        //Debug.Log(index + " value : " + value);
                //        break;
                //    }
                //    value -= symbolDataSO.symbolDataList[x].symbolWeight;
                //}

                //SymbolBase symbolBase = GetSymbolFromPools((SymbolType)index);

                SymbolBase symbolBase = GetSymbolFromPools(selectedSymbol);
                symbolBase.transform.position = new Vector3(i, j, 0) + offset;
                symbolBase.SetTerrainType(terrainInfoDic[new Vector3Int(i, j, 0)]);
                symbolsList.Add(symbolBase);

                // パワースポットの場合には常に呪いの対象とする
                bool isPowerSpot = selectedSymbol == SymbolType.PowerSpot;

                // 呪い機能利用の場合
                if (useCurse) {
                    // 生成されたシンボルが呪いの対象となるシンボルか判定
                    bool isCurseTarget = CheckCurseTargetSymbol(waveData.curseTargetSymbols, selectedSymbol);

                    // 呪いの対象であるシンボルの場合
                    if (isPowerSpot || isCurseTarget) {
                        // WaveData を利用し、呪われているか判定
                        if (isPowerSpot || UnityEngine.Random.Range(0, 100) < waveData.curseRate) {
                            // 呪いの付与
                            SymbolBase curseSymbol = GetSymbolFromPools(SymbolType.Curse);
                            curseSymbol.transform.position = new Vector3(i, j, 0) + offset;
                            symbolBase.SetCurse((CurseSymbol)curseSymbol);
                            symbolsList.Add(curseSymbol);
                        }
                    }
                }

                generateSymbolCount--;

                // generateSymbolCount = -1 でスタートの場合は抽選回数なし
                if (generateSymbolCount == 0) {
                    break;
                }
            }
            if (generateSymbolCount == 0) {
                break;
            }
        }

        // 完成したリストを戻す
        return symbolsList;
    }


    /// <summary>
    /// 生成されたシンボルが呪いの対象となるシンボルか判定
    /// </summary>
    /// <param name="symbolTypes">WaveData ごとに設定されている対象シンボル</param>
    /// <param name="targetSymbol">今回生成されたシンボル</param>
    /// <returns></returns>
    private bool CheckCurseTargetSymbol(SymbolType[] symbolTypes, SymbolType targetSymbol) {
        if (symbolTypes == null) {
            return false;
        }

        return symbolTypes.Any(data => data == targetSymbol);
    }
}