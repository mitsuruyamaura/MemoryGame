using System.Linq;

[System.Serializable]
public class MasteryData : IMasterData {
    public int id;
    public int weight;
    public int min_rank;
    public string name;
    public float value;
    public int[] skill_group_ids;
    public string desc;

    public int Id => id;

    public MasteryData(string[] datas) {
        id = int.Parse(datas[0]);
        weight = int.Parse(datas[1]);
        min_rank = int.Parse(datas[2]);
        name = datas[3];
        value = float.Parse(datas[4]);
        skill_group_ids = datas[5].Split('/').Select(id => int.Parse(id)).ToArray();
        desc = datas[6];
    }
}