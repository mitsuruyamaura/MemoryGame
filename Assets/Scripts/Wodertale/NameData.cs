[System.Serializable]
public class NameData {
    public int id;
    public string name;

    public NameData(string[] datas) { 
        id = int.Parse(datas[0]);
        name = datas[1];
    }
}