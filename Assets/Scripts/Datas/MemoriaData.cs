using System;

[Serializable]
public class MemoriaData : IMasterData {
    public int id;
    public string name;
    public Rarity rarity;
    public int weight;
    public int skill_group_id;
    public int skill_id_1;
    public int skill_type_1;
    public int skill_value_1;
    public int skill_id_2;
    public int skill_type_2;
    public int skill_value_2;
    public int skill_id_3;
    public int skill_type_3;
    public int skill_value_3;
    public int skill_id_4;
    public int skill_type_4;
    public int skill_value_4;
    public int skill_id_5;
    public int skill_type_5;
    public int skill_value_5;

    public int Id => id;

    public MemoriaData(string[] datas) {
        id = int.Parse(datas[0]);
        name = datas[1];
        rarity = (Rarity)Enum.Parse(typeof(Rarity), datas[2]);
        weight = int.Parse(datas[3]);
        skill_group_id = int.Parse(datas[4]);
        
        skill_id_1 = int.Parse(datas[5]);
        skill_type_1 = int.Parse(datas[6]);
        skill_value_1 = int.Parse(datas[7]);

        skill_id_2 = int.Parse(datas[8]);
        skill_type_2 = int.Parse(datas[9]);
        skill_value_2 = int.Parse(datas[10]);

        skill_id_3 = int.Parse(datas[11]);
        skill_type_3 = int.Parse(datas[12]);
        skill_value_3 = int.Parse(datas[13]);

        skill_id_4 = int.Parse(datas[14]);
        skill_type_4 = int.Parse(datas[15]);
        skill_value_4 = int.Parse(datas[16]);

        skill_id_5 = int.Parse(datas[17]);
        skill_type_5 = int.Parse(datas[18]);
        skill_value_5 = int.Parse(datas[19]);    
    }
}