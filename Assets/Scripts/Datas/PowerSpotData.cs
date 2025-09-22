using System;
using System.Linq;

[System.Serializable]
public class PowerSpotData {
    public string name;
    public int no;
    public int releasePoint;               // 能力値の合計値
    public StatusType[] statusTypes;       // 単体で必要な能力値の種類
    public int[] requiredValues;
    public Rarity rarity;
    public string desc;
    public int waveCount;
    public int weight;
    public TerrainType terrainType;

    public PowerSpotData(string[] datas){
        name = datas[0];
        no = int.Parse(datas[1]);
        releasePoint = int.Parse(datas[2]);

        statusTypes = datas[3].Split('/').Select(type => (StatusType)Enum.Parse(typeof(StatusType), type)).ToArray();
        requiredValues = datas[4].Split('/').Select(int.Parse).ToArray();
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[5]);
        desc = datas[6];

        waveCount = int.Parse(datas[7]);
        weight = int.Parse(datas[8]);
        terrainType = (TerrainType)Enum.Parse(typeof(TerrainType), datas[9]);
    }
}