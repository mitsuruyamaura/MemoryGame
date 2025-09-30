using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class MemoryFragmentsCard : CardModelBase {
    public MemoryFragmentsCard(CardData cardData) : base(cardData) { }

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("MemoryFragments");

        SoundManager.instance.PlaySE(SE_TYPE.PowerSpot);

        // TODO CardData を指定のマスターに変換して値を取得
        if (cardData.masterData is MemoryStoneData memoryStoneData) {
            // 思い出の秘石の獲得数を加算、秘石をスロットにセット
            GameData.instance.AddMemoryStoneList(memoryStoneData.id, memoryStoneData.addFlipCount);
        } else {
            // 見つからない場合には固定値で加算
            GameData.instance.AddMemoryStoneList(1, 3);
        }

        await UniTask.Yield(token);
    }
}
