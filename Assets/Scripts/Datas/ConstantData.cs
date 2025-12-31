/// <summary>
/// 定数データ格納用クラス
/// メモリーゲーム用
/// </summary>
[System.Serializable]
public class ConstantData {
    public int id;
    public string key;
    public string value;
    public string description;

    public ConstantData(string[] datas) { 
        id = int.Parse(datas[0]);
        key = datas[1];
        value = datas[2];
        description = datas[3];
    }
}