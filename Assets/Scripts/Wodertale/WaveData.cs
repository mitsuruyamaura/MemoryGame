using System.Linq;
using System;

[System.Serializable]
public class WaveData {
    public int waveNo;
    public int walkCount;
    public Rarity[] rarities;
    public int[] rates;
    public int row;
    public int column;
    public int generateSymbolRate;
    public int curseRate;
    public SymbolType[] curseTargetSymbols;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="datas"></param>
    public WaveData(string[] datas) {
        waveNo = int.Parse(datas[0]);
        walkCount = int.Parse(datas[1]);

        rarities = datas[2].Split('/').Select(type => (Rarity)Enum.Parse(typeof(Rarity), type)).ToArray();
        rates = datas[3].Split('/').Select(int.Parse).ToArray();

        row = int.Parse(datas[4]);
        column = int.Parse(datas[5]);
        generateSymbolRate = int.Parse(datas[6]);

        curseRate = int.Parse(datas[7]);
        if (!string.IsNullOrWhiteSpace(datas[8])) {
            DebugLogger.Log(datas[8]);
            curseTargetSymbols = datas[8].Split('/').Select(type => (SymbolType)Enum.Parse(typeof(SymbolType), type)).ToArray();
        }
    }

    public static Rarity GetRandomRarity(WaveData waveData) {
        int totalRate = waveData.rates.Sum(); // rates の合計を計算
        int randomValue = UnityEngine.Random.Range(0, totalRate); // 0 から totalRate-1 の範囲で乱数を取得

        int cumulativeRate = 0;

        for (int i = 0; i < waveData.rates.Length; i++) {
            cumulativeRate += waveData.rates[i];
            if (randomValue < cumulativeRate) {
                return waveData.rarities[i]; // 合計に達したら対応するRarityを返す
            }
        }

        // デフォルトの返り値（理論的にはこの部分は到達しない）
        return waveData.rarities.Last();
    }
}