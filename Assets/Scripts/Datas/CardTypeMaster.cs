using UnityEngine;

[System.Serializable]
public class CardTypeMaster : IMasterData {
    public CardEventType cardEventType;
    public Sprite spriteCardType;

    public int Id => (int)cardEventType;
}