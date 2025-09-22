using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public class EnemyCard : CardModelBase {
    public EnemyCard(CardData cardData) : base(cardData) {}

    public override async UniTask ExecuteCardAsync(CancellationToken token) {
        DebugLogger.Log("Enemy");

        await UniTask.Yield(token);
    }
}