[System.Serializable]
public class MemoryStoneData : IMasterData {
    public int id;
    public int addFlipCount;
    public int memoryPoint;
    public string address;

    public int Id => id;

    public MemoryStoneData(string[] datas) {
        id = int.Parse(datas[0]);
        addFlipCount = int.Parse(datas[1]);
        memoryPoint = int.Parse(datas[2]);
        address = datas[3];
    }
}