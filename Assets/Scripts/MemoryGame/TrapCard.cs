using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class TrapCard : CardModelBase {
    public TrapCard(CardData cardData, int cardIndex) : base(cardData, cardIndex) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Trap");

        if (cardData.masterData is TrapData trapData) {
            // 画面表示
            //await ItemInfoDisplayManager.instance.ShowBlessingInfoAsync(blessingData, token);

            TrapExecutor trapExecutor = new();
            await trapExecutor.ExecuteTrapAsync(trapData, token);
        }

        await UniTask.Yield(token);
    }
}