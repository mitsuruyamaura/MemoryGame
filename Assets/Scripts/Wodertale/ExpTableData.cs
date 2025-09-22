[System.Serializable]
public class ExpTableData
{
    public int level;
    public int exp;

    public ExpTableData(string[] datas) {
        level = int.Parse(datas[0]); 
        exp = int.Parse(datas[1]);
    }
}