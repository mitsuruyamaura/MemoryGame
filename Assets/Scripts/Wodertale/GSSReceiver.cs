using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;

/// <summary>
/// スプレッドシートから取得したデータをシート単位で任意のスクリプタブル・オブジェクトに値として取り込む
/// </summary>
[RequireComponent(typeof(GSSReader))]
public class GSSReceiver : MonoBehaviour {

    public bool IsLoading { get; set; }


    //private void Awake() {
    //    // GSS のデータ取得準備
    //    PrepareGSSLoadStartAsync().Forget();
    //}

    /// <summary>
    /// GSS のデータ取得準備
    /// </summary>
    /// <returns></returns>
    public async UniTask PrepareGSSLoadStartAsync() {
        IsLoading = false;

        await GetComponent<GSSReader>().GetFromWebAsync();

        OnGSSLoadEnd();
        IsLoading = true;

        DebugLogger.Log("GSS データを SO に取得");
    }

    /// <summary>
    /// インスペクターから GSSReader の OnLoadEnd にこのメソッドを追加することで GSS の読み込み完了時にコールバックされる
    /// </summary>
    public void OnGSSLoadEnd() {

        GSSReader reader = GetComponent<GSSReader>();

        // スプレッドシートから取得した各シートの配列を List に変換
        List<SheetData> sheetDataslist = reader.sheetDatas.ToList();

        // 情報が取得できた場合
        if (sheetDataslist != null) {

            // スクリプタブル・オブジェクトに代入
            DataBaseManager.instance.itemDataSO.itemDataList =
                new List<ItemData>(sheetDataslist.Find(x => x.SheetName == SheetName.ItemData).DatasList.Select(x => new ItemData(x)).ToList());

            //DataBaseManager.instance.nameDataSO.nameDataList =
            //    new List<NameData>(sheetDataslist.Find(x => x.SheetName == SheetName.NameData).DatasList.Select(x => new NameData(x)).ToList());

            DataBaseManager.instance.enemyDataSO.enemyDatasList =
                new List<EnemyData>(sheetDataslist.Find(x => x.SheetName == SheetName.EnemyData).DatasList.Select(x => new EnemyData(x)).ToList());

            DataBaseManager.instance.memoryStoneDataSO.memoryStoneList =
                new List<MemoryStoneData>(sheetDataslist.Find(x => x.SheetName == SheetName.MemoryStoneData).DatasList.Select(x => new MemoryStoneData(x)).ToList());

            DataBaseManager.instance.floorDataSO.floorDataList =
                new List<FloorData>(sheetDataslist.Find(x => x.SheetName == SheetName.FloorData).DatasList.Select(x => new FloorData(x)).ToList());

            DataBaseManager.instance.trapDataSO.trapDataList =
                new List<TrapData>(sheetDataslist.Find(x => x.SheetName == SheetName.TrapData).DatasList.Select(x => new TrapData(x)).ToList());

            DataBaseManager.instance.blessingDataSO.blessingDataList =
                new List<BlessingData>(sheetDataslist.Find(x => x.SheetName == SheetName.BlessingData).DatasList.Select(x => new BlessingData(x)).ToList());

            DataBaseManager.instance.constantDataSO.constantDataList =
                new List<ConstantData>(sheetDataslist.Find(x => x.SheetName == SheetName.ConstantData).DatasList.Select(x => new ConstantData(x)).ToList());

            //DataBaseManager.instance.terrainDataSO.terrainDataList =
            //    new List<TerrainData>(sheetDataslist.Find(x => x.SheetName == SheetName.TerrainData).DatasList.Select(x => new TerrainData(x)).ToList());

            // TODO 他の SO を追加する
        }
    }
}