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


    private void Awake() {
        // GSS のデータ取得準備
        PrepareGSSLoadStartAsync().Forget();
    }

    /// <summary>
    /// GSS のデータ取得準備
    /// </summary>
    /// <returns></returns>
    private async UniTask PrepareGSSLoadStartAsync() {
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

            DataBaseManager.instance.nameDataSO.nameDataList =
                new List<NameData>(sheetDataslist.Find(x => x.SheetName == SheetName.NameData).DatasList.Select(x => new NameData(x)).ToList());

            DataBaseManager.instance.enemyDataSO.enemyDatasList =
                new List<EnemyData>(sheetDataslist.Find(x => x.SheetName == SheetName.EnemyData).DatasList.Select(x => new EnemyData(x)).ToList());

            DataBaseManager.instance.expTableDataSO.expTableDataList =
                new List<ExpTableData>(sheetDataslist.Find(x => x.SheetName == SheetName.ExpTableData).DatasList.Select(x => new ExpTableData(x)).ToList());

            DataBaseManager.instance.waveDataSO.waveDataList =
                new List<WaveData>(sheetDataslist.Find(x => x.SheetName == SheetName.WaveData).DatasList.Select(x => new WaveData(x)).ToList());

            DataBaseManager.instance.symbolRateDataSO.symbolRateDataList =
                new List<SymbolRateData>(sheetDataslist.Find(x => x.SheetName == SheetName.SymbolRateData).DatasList.Select(x => new SymbolRateData(x)).ToList());

            DataBaseManager.instance.powerSpotDataSO.powerSpotDataList =
                new List<PowerSpotData>(sheetDataslist.Find(x => x.SheetName == SheetName.PowerSpotData).DatasList.Select(x => new PowerSpotData(x)).ToList());

            DataBaseManager.instance.terrainDataSO.terrainDataList =
                new List<TerrainData>(sheetDataslist.Find(x => x.SheetName == SheetName.TerrainData).DatasList.Select(x => new TerrainData(x)).ToList());

            // TODO 他の SO を追加する
        }
    }
}