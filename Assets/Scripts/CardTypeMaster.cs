using UnityEngine;

[System.Serializable]
public class CardTypeMaster : IMasterData {
    public CardType cardType;
    public Sprite spriteCardType;

    public int Id => (int)cardType;
}