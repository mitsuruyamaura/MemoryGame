using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class MemoryFragmentsCard : CardModelBase {
    public MemoryFragmentsCard(CardData cardData, int cardIndex) : base(cardData, cardIndex) { }

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("MemoryFragments");

        SoundManager.instance.PlaySE(SE_TYPE.PowerSpot);

        // CardData を指定のマスターに変換して値を取得
        if (cardData.masterData is MemoryStoneData memoryStoneData) {
            // 思い出の秘石の獲得数を加算、秘石をスロットにセット
            GameData.instance.AddMemoryStoneList(memoryStoneData);
        } 
            
        // 見つからない場合には何もせずに終了        
        await UniTask.Yield(token);
    }
}