using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// カードの親クラス
/// </summary>
[System.Serializable]
public abstract class CardModelBase {
    public CardData cardData;
    public int cardIndex;
    public ICardExecutor executor;   // カード実行処理インターフェース

    public bool isFaceUp;
    public bool isPair;


    protected CardModelBase(CardData cardData, int cardIndex, ICardExecutor executor) {
        this.cardData = cardData;
        this.cardIndex = cardIndex;
        this.executor = executor;
    }

    public virtual UniTask ExecuteCardAsync(CancellationToken token) {
        return executor.ExecuteCardAsync(this, token);
    }

    public virtual void SetPair() {
        isPair = true;
    }

    public virtual void ReleasePair() {
        isPair = false;
    }

    public virtual void FaceUp() {
        isFaceUp = true;
    }

    public virtual void FaceDown() {
        isFaceUp = false;
    }
}