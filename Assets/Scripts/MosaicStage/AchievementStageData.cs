using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AchievementStageData
{
    public int stageNo;
    public int challengeCount;
    public int clearCount;
    public int failureCount;
    public int maxFeverCount;
    public int noMissClearCount;
    public int maxMosaicCount;      // １ゲーム当たりの最高獲得ポイント
    public int maxLinkCount;        // １ゲーム当たりで、一度にまとめて消したブロックの最高数
    public float fastestClearTime;    // １ゲームにおける、最も早いクリアタイム


    public AchievementStageData(int stageNo) {
        this.stageNo = stageNo;
    }
}
