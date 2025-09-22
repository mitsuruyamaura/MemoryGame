[System.Serializable]
public class SymbolRateData {
    public int id;
    public int enemy;
    public int treasureBox;
    public int powerSpot;
    public int life;
    public int orb;
    public int exprole;

    public SymbolRateData(string[] datas) {
        id = int.Parse(datas[0]);
        enemy = int.Parse(datas[1]);
        treasureBox = int.Parse(datas[2]);
        powerSpot = int.Parse(datas[3]);
        life = int.Parse(datas[4]);
        orb = int.Parse(datas[5]);
        exprole = int.Parse(datas[6]);
    }
}