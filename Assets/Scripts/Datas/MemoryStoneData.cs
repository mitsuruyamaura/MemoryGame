[System.Serializable]
public class MemoryStoneData : IMasterData {
    public int id;
    public int addFlipCount;
    public int memoryPoint;
    public string address;
    public int weight;
    public string name;

    public int Id => id;

    public MemoryStoneData(string[] datas) {
        id = int.Parse(datas[0]);
        addFlipCount = int.Parse(datas[1]);
        memoryPoint = int.Parse(datas[2]);
        address = datas[3];
        weight = int.Parse(datas[4]);
        name = datas[5];
    }
}