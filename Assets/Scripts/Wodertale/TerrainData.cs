[System.Serializable]
public class TerrainData {
    public string name;
    public int no;
    public int enemy;
    public int treasureBox;
    public int powerSpot;
    public int life;
    public int orb;
    public int exprole;
    public int curseBase;
    public int curseRate;

    public TerrainData(string[] datas) {
        name = datas[0];
        no = int.Parse(datas[1]);
        enemy = int.Parse(datas[2]);
        treasureBox = int.Parse(datas[3]);
        powerSpot = int.Parse(datas[4]);
        life = int.Parse(datas[5]);
        orb = int.Parse(datas[6]);
        exprole = int.Parse(datas[7]);
        curseBase = int.Parse(datas[8]);
        curseRate = int.Parse(datas[9]);
    }


    public int[] GetSymbolRateArray() {
        return new int[] { enemy, treasureBox, powerSpot, life, orb, exprole };
    }

    public SymbolType[] GetGenerateSymbolTypes() {
        return new SymbolType[] {
            SymbolType.Enemy,
            SymbolType.TreasureBox,
            SymbolType.PowerSpot,
            SymbolType.Life,
            SymbolType.Orb,
            SymbolType.Exprole
        };
    }

    public int GetTotalSymbolRate() {
        return enemy + treasureBox + powerSpot + life + orb + exprole;
    }
}