[System.Serializable]
public class NameData : IMasterData {
    public int id;
    public string name;

    public int Id => id;

    public NameData(string[] datas) { 
        id = int.Parse(datas[0]);
        name = datas[1];
    }
}