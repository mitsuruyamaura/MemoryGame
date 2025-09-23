using Cysharp.Threading.Tasks;
using System.Threading;

[System.Serializable]
public abstract class CardModelBase {
    public CardData cardData;

    public bool isFaceUp;
    public bool isPair;


    protected CardModelBase(CardData cardData) {
        this.cardData = cardData;
    }

    public abstract UniTask ExecuteCardAsync(CancellationToken token);

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