using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageName;
    public int stageNo;
    public Sprite stageView;           // ステージの背景画像
    public Transform playerIconTran;   // プレイヤーのアイコンの配置場所
    public int initStamina;            // ステージ開始時の初期スタミナ
    public int[] appearEnemyNos;       // 出現するエネミーの種類
    public int bossNo;                 // 出現するボスの種類
    public int clearBonusPoint;        // クリアしたときのボーナス
    public StageType stageType;        // ステージのタイルマップの種類

    public OrbType[] orbTypes;         // 出現するオーブの種類
    public int dropTreasureLevel;      // ドロップするトレジャーのレベル

    // TODO 他にもあれば追加 Stage ごとにシンボルの出現率変える、とか

}
