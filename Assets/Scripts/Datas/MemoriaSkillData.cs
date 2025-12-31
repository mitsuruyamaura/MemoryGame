[System.Serializable]
public class MemoriaSkillData : IMasterData {
    public int id;
    public string name;
    public string desc;

    public int Id => id;


    public MemoriaSkillData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        desc = datas[2];
    }
}