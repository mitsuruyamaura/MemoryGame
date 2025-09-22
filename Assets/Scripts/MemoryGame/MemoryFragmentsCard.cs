using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class MemoryFragmentsCard : CardModelBase {
    public MemoryFragmentsCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("MemoryFragments");

        // TODO CardData を指定のマスターに変換して値を取得

        // 思い出の秘石の獲得数を加算、秘石をスロットにセット
        GameData.instance.AddMemoryStoneList(1, 3);
        
        await UniTask.Yield(token);
    }
}
